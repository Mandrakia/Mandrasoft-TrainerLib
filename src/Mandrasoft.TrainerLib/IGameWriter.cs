using EasyHook;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Mandrasoft.TrainerLib
{
    public sealed class MaskMatchResult
    {
        public bool Success { get; set; }
        public List<MaskMatch> Matches { get; set; } = new List<MaskMatch>();
    }

    public sealed class MaskMatch
    {
        public IntPtr Start { get; set; }
        public List<byte> Captures { get; set; } = new List<byte>();
    }

    public interface IGameWriter
    {
        Process Process { get; }
        IntPtr MainModulePtr { get; }
        int Write(IntPtr offset, byte[] bytes);
        byte[] Read(IntPtr offset, int length);
        IntPtr ReadIntPtr(IntPtr offset);
        MaskMatchResult SearchMask(MaskItem[] mask);
        bool ApplyPatch(MaskItem[] mask, byte[] bytes);
        bool ApplyPatch(MaskItem[] mask, byte[] bytes, bool greedy);
        bool ApplyPatch(MaskItem[] mask, byte[] bytes, MaskMatch mt);
        void Click(int x, int y);
        void RightClick(int x, int y);
        void PressKey(Keys key);
        void ReleaseKey(Keys key);
    }
    public interface IInjectedGameWriter : IGameWriter
    {
        LocalHook HookFunction(IntPtr fctAdress,Delegate deleg);
    }

}
