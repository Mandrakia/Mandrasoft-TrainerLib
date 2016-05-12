using EasyHook;
using Mandrasoft.TrainerLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MandraSoft.TrainerLib.InjectedFFXIII
{
    unsafe class AddGilPatch : Patch
    {
        public override string Description => "Add 50.000 gils";

        public override string Title => "Add 50.000 gils";

        private IntPtr moneyPtr = IntPtr.Zero;


        public override bool ApplyPatch(IGameWriter writer)
        {
            if (moneyPtr == IntPtr.Zero)
            {
                var res = writer.SearchMask(new MaskItem[] { 0x55,0x8b,0xec,0x83, 0xec, 0x14, 0x6a, 0x00, 0x8b, 0x0d, "?", "?", "?", "?", 0x81, 0xc1, 0x10, 0x72, 0x02, 0x00,0xe8,"*","*","*","*",0xa1,"*","*","*","*",0x8b,0x88,0x78,0x25,0x00,0x00,0x03,0x4d,0x08 });
                if (res.Success)
                {
                    moneyPtr = res.Matches.First().Start;

                }               
            }
            AddMoney addMoneyMethod = (AddMoney)Marshal.GetDelegateForFunctionPointer(moneyPtr, typeof(AddMoney));
            addMoneyMethod(50000);
            return true;
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate int AddMoney(int amount);
     
    }
}
