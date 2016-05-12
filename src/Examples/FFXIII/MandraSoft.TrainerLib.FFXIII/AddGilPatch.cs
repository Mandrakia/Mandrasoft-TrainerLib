using Mandrasoft.TrainerLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MandraSoft.TrainerLib.FFXIII
{
    class AddGilPatch : Patch
    {
        public override string Description => "Add 50.000 gils";

        public override string Title => "Add 50.000 gils";

        private IntPtr moneyPtr = IntPtr.Zero;

        public override bool ApplyPatch(IGameWriter writer)
        {
            if (moneyPtr == IntPtr.Zero)
            {
                var res = writer.SearchMask(new MaskItem[] { 0x83, 0xec, 0x14, 0x6a, 0x00, 0x8b, 0x0d, "?", "?", "?", "?", 0x81, 0xc1, 0x10, 0x72, 0x02, 0x00 });
                if (res.Success)
                {
                    var addr = BitConverter.ToInt32(res.Matches.First().Captures.ToArray(), 0);
                    moneyPtr = writer.ReadIntPtr((IntPtr)addr) + 0x2578;

                }
            }
            if (moneyPtr == IntPtr.Zero) return false;
            var currMoney = writer.ReadIntPtr(moneyPtr);
            currMoney += 50000;
            return writer.Write(moneyPtr, BitConverter.GetBytes(currMoney.ToInt32())) == 4;
        }
    }
}
