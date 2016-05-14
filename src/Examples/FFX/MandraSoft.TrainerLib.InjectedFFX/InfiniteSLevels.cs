using EasyHook;
using Mandrasoft.TrainerLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MandraSoft.TrainerLib.InjectedFFX
{
    unsafe class InfiniteSLevel : TogglePatch
    {
        public override string Description => "Moving in the sphere will not spend sLevel";

        public override string Title => "Infinite sLevels";

        private IntPtr getSLevelAddr;
        private LocalHook _hook;
        private IGameWriter _writer;
        public override bool ApplyPatch(IGameWriter writer)
        {
            _writer = writer;
            _hook = ((IInjectedGameWriter)writer).HookFunction(getSLevelAddr, new GetSLevel(GetSLevelCustom));
            return true;
        }
        private int GetSLevelCustom(int a1)
        {
            return 99;
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
            var res = writer.SearchMask(new MaskItem[] { 0x55 ,0x8B, 0xEC, 0x0F, 0xB6, 0x45, 0x08, 0x69, 0xC0, 0x94, 0x00, 0x00, 0x00, 0x0F, "*","*","*","*","*","*", 0x5D, 0xC3 });
            if (res.Success)
            {
                getSLevelAddr = res.Matches.First().Start;
            }
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate int GetSLevel(int a1);
    }
}
