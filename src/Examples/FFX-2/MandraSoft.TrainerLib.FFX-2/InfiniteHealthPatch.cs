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
    class InfiniteHealthPatch : TogglePatch
    {
        public override string Description => "Can't lose health in battle";

        public override string Title => "Infinite HP";

        private IntPtr hpChangeFtcAddr;
        private HpChange originalHpChange;
        private LocalHook _hook;
        public override bool ApplyPatch(IGameWriter writer)
        {
            if (hpChangeFtcAddr != IntPtr.Zero)
            {
                originalHpChange = (HpChange)Marshal.GetDelegateForFunctionPointer(hpChangeFtcAddr,typeof(HpChange));
                _hook = ((IInjectedGameWriter)writer).HookFunction(hpChangeFtcAddr, new HpChange(CustomHpChange));
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
            var res = writer.SearchMask(new MaskItem[] { 0x55, 0x8B, 0xEC, 0x56, 0x8B, 0x75, 0x0C, 0x57, 0x8B, 0x7D, 0x10, 0x85, 0xFF, 0x7E, 0x20, 0x8A, 0x86, 0xAD, 0x05, 0x00, 0x00, 0x3C, 0x63, 0x73, 0x16, 0xFE, 0xC0, 0x88, 0x86, 0xAD, 0x05, 0x00 });
            if (res.Success)
            {
                hpChangeFtcAddr = res.Matches.Single().Start;
            }
        }
        private int CustomHpChange(int characterID, int character, int deltaHp)
        {
            if (characterID <= 8)
                return originalHpChange(characterID, character, 0);
            else
                return originalHpChange(characterID, character, deltaHp);
        }
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate int HpChange(int characterID, int character, int deltaHp);
    }
}
