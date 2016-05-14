using Mandrasoft.TrainerLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MandraSoft.TrainerLib.InjectedFFX
{
    unsafe class Add99Spheres : Patch
    {
        public override string Description => "Sets the number of Spheres for all kind to 99";

        public override string Title => "Add all 99 Spheres";
        private IntPtr spheresAddr;
        public override bool ApplyPatch(IGameWriter writer)
        {
            if (spheresAddr == IntPtr.Zero) return false;
            for (var i = 0; i < 112; i++)
            {
                *(byte*)(spheresAddr + i) = 99;
            }
            return true;
        }

        public override void Init(IGameWriter writer)
        {
            var res = writer.SearchMask(new MaskItem[] { 0x8A, 0x87, "?", "?", "?", "?", 0x84, 0xC0, 0x74, 0x0B, 0x85, 0xDB, 0x79, 0x5A, 0x0F, 0xB6, 0xC0, 0x03, 0xC3, 0x79, 0x53 });
            if (res.Success)
                spheresAddr = (IntPtr)BitConverter.ToInt32(res.Matches.First().Captures.ToArray(), 0);
        }
    }
}
