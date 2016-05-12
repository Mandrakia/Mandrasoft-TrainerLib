using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Mandrasoft.TrainerLib
{
    [StructLayout(LayoutKind.Sequential)]
    unsafe struct TrainerStateStructure
    {
        public bool ShouldStop;
        public fixed bool PatchesState[11];
    }
}
