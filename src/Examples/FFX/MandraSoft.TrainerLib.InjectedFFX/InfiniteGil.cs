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
    unsafe class InfiniteGil : TogglePatch
    {
        public override string Description => "Gils doesn't reduce when spent";

        public override string Title => "Infinite Gils";

        private IntPtr addSpendGilFct;
        private SpendGil _origFcn;
        private LocalHook _hook;
        private IGameWriter _writer;
        public override bool ApplyPatch(IGameWriter writer)
        {
            _writer = writer;
            _hook = ((IInjectedGameWriter)writer).HookFunction(addSpendGilFct, new SpendGil(SpendGilCustom));
            _origFcn = (SpendGil)Marshal.GetDelegateForFunctionPointer(addSpendGilFct,typeof(SpendGil));
            return true;
        }
        private int SpendGilCustom(int a1)
        {
            if (a1 > 0) _origFcn(a1);
            return 0;
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
            var res = writer.SearchMask(new MaskItem[] { 0x8B, 0x43, 0x09, 0x83, 0xC4, 0x18, 0x85, 0xC0, 0x7E, 0x1E, 0xF7, 0xD8, 0x50, 0xE8, "?", "?", "?", "?" });
            if (res.Success)
            {
                addSpendGilFct = res.Matches.First().Start + 18 + BitConverter.ToInt32(res.Matches.First().Captures.ToArray(), 0);
            }
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate int SpendGil(int a1);
    }
}
