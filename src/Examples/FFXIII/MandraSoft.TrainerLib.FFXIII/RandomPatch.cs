using Mandrasoft.TrainerLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MandraSoft.TrainerLib.FFXIII
{
    class RandomPatch : Patch
    {
        public override string Description => "Random patch";

        public override string Title => "Random patch!!";

        public override bool ApplyPatch(IGameWriter writer)
        {
            return true;
        }
    }
}
