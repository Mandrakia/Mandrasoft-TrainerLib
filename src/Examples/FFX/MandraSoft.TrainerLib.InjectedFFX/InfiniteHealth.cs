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
    unsafe class InfiniteHealth : TogglePatch
    {
        public override string Description => "Can't lose health in battle";

        public override string Title => "Infinite Health";

        private IntPtr addrCharacters;
        private IntPtr addrFctDamageChara;
        private LocalHook _hook;
        private DamageCharacter originalCall;
        private IGameWriter _writer;
        public override bool ApplyPatch(IGameWriter writer)
        {
            _writer = writer;
            _hook = ((IInjectedGameWriter)writer).HookFunction(addrFctDamageChara, new DamageCharacter(CustomDamageCharacter));
            return true;
        }
        private int CustomDamageCharacter(int a1, IntPtr a2, int hpChange_1, int a4, int a5, int a6, int a7)
        {
            var battleStruct = _writer.ReadIntPtr(addrCharacters);
            var index = ((int)a2 - (int)battleStruct) / 0xf90;
            if (index <= 7) return originalCall(a1, a2, 0, a4, a5, a6, a7);
            else return originalCall(a1, a2, hpChange_1, a4, a5, a6, a7);
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
            var res = writer.SearchMask(new MaskItem[] { 0x55, 0x8B, 0xEC, 0x53, 0x56, 0x57, "*", "*", "*", "*", "*", 0x8B, 0x75, 0x0C, 0x8B, 0x5D, 0x08 });
            if (res.Success)
            {
                addrFctDamageChara = res.Matches.First().Start;
                originalCall = (DamageCharacter)Marshal.GetDelegateForFunctionPointer(addrFctDamageChara,typeof(DamageCharacter));
            }
            res = writer.SearchMask(new MaskItem[] { 0x68, 0x70, 0xE2, 0x01, 0x00, 0xE8, "*", "*", "*", "*", 0xA3, "?", "?", "?", "?", 0x05, 0x40, 0x37, 0x01, 0x00, 0x68, 0xe0, 0xdc, 0x00, 0x00 });
            if (res.Success)
            {
                addrCharacters = (IntPtr)BitConverter.ToInt32(res.Matches.First().Captures.ToArray(), 0);
            }
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate int DamageCharacter(int a1, IntPtr a2, int hpChange_1, int a4, int a5, int a6, int a7);
    }
}
