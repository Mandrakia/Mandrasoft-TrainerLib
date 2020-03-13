using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using EasyHook;
using static Mandrasoft.TrainerLib.ImportsWin32;

namespace Mandrasoft.TrainerLib
{
    unsafe class InProcessGameWriter : IInjectedGameWriter
    {
        public Process Process => Process.GetCurrentProcess();
        public IntPtr MainModulePtr => Process.GetCurrentProcess().MainModule.BaseAddress;

        public InProcessGameWriter()
        {
            UnprotectMemory();
        }
        public byte[] Read(IntPtr offset, int length)
        {
            byte* b = (byte*)offset;
            var result = new byte[length];
            for (int i = 0; i < length; i++)
            {
                result[i] = *b;
                b++;
            }
            return result;
        }

        public bool ApplyPatch(MaskItem[] mask, byte[] bytes)
        {
            return ApplyPatch(mask, bytes, false);
        }
        public bool ApplyPatch(MaskItem[] mask, byte[] bytes, bool greedy)
        {
            bool success = true;
            var res = SearchMask(mask);
            if (res.Success)
            {
                if (greedy)
                    foreach (var match in res.Matches) success &= ApplyPatch(mask, bytes, match);
                else success = ApplyPatch(mask, bytes, res.Matches.First());
            }
            else success = false;
            return success;
        }
        public bool ApplyPatch(MaskItem[] mask, byte[] bytes, MaskMatch mt)
        {
            bool success = true;
            int iCapture = 0;
            for (var i = 0; i < mask.Length; i++)
            {
                if (mask[i].Type == MaskItem.MaskType.Capture)
                {
                    success &= 1 == Write(mt.Start + i, new byte[] { bytes[iCapture] });
                    iCapture++;
                }
            }
            return success;
        }
        public IntPtr ReadIntPtr(IntPtr offset)
        {
            var bytes = Read(offset, 4);
            return (IntPtr)BitConverter.ToInt32(bytes, 0);
        }
        public MaskMatchResult SearchMask(MaskItem[] mask)
        {
            return SearchMask(mask, Process.GetCurrentProcess().MainModule.BaseAddress, Process.GetCurrentProcess().MainModule.ModuleMemorySize);
        }
        public MaskMatchResult SearchMask(MaskItem[] mask, IntPtr start, int length)
        {
            MaskMatchResult res = new MaskMatchResult();
            for (var i = 0; i < length - mask.Length; i++)
            {
                if (IsMatch(mask, start + i))
                {
                    res.Success = true;
                    res.Matches.Add(Match(mask, start + i));
                }
            }
            return res;
        }
        public bool IsMatch(MaskItem[] mask, IntPtr start)
        {
            for (var i = 0; i < mask.Length; i++)
            {
                if (mask[i].Type == MaskItem.MaskType.Byte && mask[i].Byte != *(byte*)(start + i)) return false;
            }
            return true;
        }
        public MaskMatch Match(MaskItem[] mask, IntPtr start)
        {

            MaskMatch m = new MaskMatch() { Start = start };
            for (var i = 0; i < mask.Length; i++)
            {
                if (m == null) break;
                switch (mask[i].Type)
                {
                    case MaskItem.MaskType.Byte:
                        if (mask[i].Byte != *(byte*)(start + i)) m = null;
                        break;
                    case MaskItem.MaskType.Capture:
                        m.Captures.Add(*(byte*)(start + i));
                        break;
                        //Ignore wildcard nothing to do there.
                }
            }
            return m;
        }

        public int Write(IntPtr offset, byte[] bytes)
        {
            for (var i = 0; i < bytes.Length; i++)
            {
                *(byte*)(offset + i) = bytes[i];
            }
            return bytes.Length;
        }
        private void UnprotectMemory()
        {
            uint oldProt;
            VirtualProtect(Process.GetCurrentProcess().MainModule.BaseAddress, (uint)Process.GetCurrentProcess().MainModule.ModuleMemorySize, 0x40, out oldProt);
        }
        public LocalHook HookFunction(IntPtr fctAdress, Delegate deleg)
        {
            var res = LocalHook.Create(fctAdress, deleg, null);
            res.ThreadACL.SetExclusiveACL(null);
            return res;
        }

        public void Click(int x, int y,bool isFocused = false)
        {
            throw new NotImplementedException();
        }

        public void RightClick(int x, int y,bool isFocused = false)
        {
            throw new NotImplementedException();
        }

        public void PressKey(Keys key)
        {
            throw new NotImplementedException();
        }

        public void ReleaseKey(Keys key)
        {
            throw new NotImplementedException();
        }
    }
}
