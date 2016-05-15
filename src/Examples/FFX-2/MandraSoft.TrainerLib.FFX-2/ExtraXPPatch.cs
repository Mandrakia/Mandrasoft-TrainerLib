using Mandrasoft.TrainerLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using EasyHook;

namespace MandraSoft.TrainerLib.FFX_2
{
    class ExtraXPPatch : TogglePatch
    {
        private IntPtr changeXpFctAddr;
        private ChangeXpForCharacterID originalChangeXpForCharacterID;
        private LocalHook _hook;
        public override string Description => "Xp gain is multiplied by 10";

        public override string Title => "Xp x 10";

        public override bool ApplyPatch(IGameWriter writer)
        {
            if (changeXpFctAddr != IntPtr.Zero)
            {
                originalChangeXpForCharacterID = (ChangeXpForCharacterID)Marshal.GetDelegateForFunctionPointer(changeXpFctAddr,typeof(ChangeXpForCharacterID));
                _hook = ((IInjectedGameWriter)writer).HookFunction(changeXpFctAddr, new ChangeXpForCharacterID(CustomChangeXpForCharacterID));
                return true;
            }
            return false;
        }

        public override bool DisablePatch(IGameWriter writer)
        {
            if (_hook != null)
            {
                _hook.Dispose();
                _hook = null;
            }
            return true;
        }

        public override void Init(IGameWriter writer)
        {
            var res = writer.SearchMask(new MaskItem[] {0x55, 0x8B, 0xEC, 0x56, 0x8B, 0x75, 0x08, 0x81, 0xE6, 0xFF, 0x00, 0x00, 0x00, 0x56, 0xE8, "*","*","*","*", 0x83, 0xC4, 0x04, 0x85, 0xC0, 0x74, 0x27, 0xC1, 0xE6, 0x07 });
            if (res.Success)
            {
                changeXpFctAddr = res.Matches.Single().Start;
            }
        }
        private int CustomChangeXpForCharacterID(ushort charID, int deltaXp)
        {
            return originalChangeXpForCharacterID(charID, deltaXp * 10);
        }
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate int ChangeXpForCharacterID(ushort charID, int deltaXp);
    }
}
