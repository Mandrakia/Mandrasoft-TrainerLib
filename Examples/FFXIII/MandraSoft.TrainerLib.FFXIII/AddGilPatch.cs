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

        public override bool ApplyPatch(IGameWriter writer)
        {
            throw new NotImplementedException();
        }
    }
}
