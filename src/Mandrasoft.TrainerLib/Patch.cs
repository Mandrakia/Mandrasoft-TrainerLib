using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mandrasoft.TrainerLib
{
    public abstract class Patch
    {
        public abstract void Init(IGameWriter writer);
        public abstract string Title { get; }
        public abstract string Description { get; }
        public abstract bool ApplyPatch(IGameWriter writer);
    }
    public abstract class TogglePatch : Patch
    {
        public abstract bool DisablePatch(IGameWriter writer);
    }
}
