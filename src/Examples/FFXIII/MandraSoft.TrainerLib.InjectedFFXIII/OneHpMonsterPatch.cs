using EasyHook;
using Mandrasoft.TrainerLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MandraSoft.TrainerLib.InjectedFFXIII
{
    unsafe class OneHpMonsterPatch : TogglePatch
    {
        public override string Description => "Changes the Max Hp of all monsters to 1";

        public override string Title => "1hp Monsters";

        private IntPtr HpPtr { get; set; }
        private LocalHook _hook2;

        public override bool ApplyPatch(IGameWriter writer)
        {
            if (HpPtr == IntPtr.Zero)
            {
                var res = writer.SearchMask(new MaskItem[] { 0x55, 0x8B, 0xEC, 0x51, 0x89, 0x4D, 0xFC, 0x6A, 0x04, 0x6A, 0x01, 0x8B, 0x4D, 0xFC, 0x81, 0xC1, 0xC4, 0x02, 0x00, 0x00 });
                if (res.Success)
                    HpPtr = res.Matches.First().Start;
            }
            _hook2 = ((IInjectedGameWriter)writer).HookFunction(HpPtr, new GetMaxHpDelegate(GetMaxHp));
            return true;
        }

        public override bool DisablePatch(IGameWriter writer)
        {
            _hook2.Dispose();
            _hook2 = null;
            return true;
        }

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        delegate int GetMaxHpDelegate(BattleChara* charStats);
        public int GetMaxHp(BattleChara* charStats)
        {
            if (charStats->CharacterIndex == -1) return 1;
            else return 9999;
        }
    }
}
