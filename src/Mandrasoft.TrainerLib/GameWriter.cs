using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static Mandrasoft.TrainerLib.ImportsWin32;

namespace Mandrasoft.TrainerLib
{
    public class GameWriter : IGameWriter
    {
        private IntPtr pHandle;
        private ProcessModule pMainModule;
        public GameWriter(Process process)
        {
            pMainModule = process.MainModule;
            pHandle = ImportsWin32.OpenProcess(ImportsWin32.ProcessAccessFlags.QueryInformation | ImportsWin32.ProcessAccessFlags.VirtualMemoryOperation | ImportsWin32.ProcessAccessFlags.VirtualMemoryRead | ImportsWin32.ProcessAccessFlags.VirtualMemoryWrite, false, process.Id);
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
                    success &= 1 == Write(mt.Start + i, new byte[] {bytes[iCapture] });
                    iCapture++;
                }
            }
            return success;
        }
        public byte[] Read(IntPtr offset, int length)
        {
            var buffer = new byte[length];
            int iRead = 0;
            bool result = false;
            try
            {
                result = ImportsWin32.ReadProcessMemory(pHandle, offset, buffer, length, ref iRead);
            }
            catch
            {
                throw;
            }
            if (result)
            {
                if (iRead == length) return buffer;
                else return null;
            }
            else
            {
                throw new Exception("Read error", new Win32Exception(Marshal.GetLastWin32Error()));
            }
        }
        public IntPtr ReadIntPtr(IntPtr offset)
        {
            throw new NotImplementedException();
        }
        public MaskMatchResult SearchMask(MaskItem[] mask)
        {
            MEMORY_BASIC_INFORMATION reg = new MEMORY_BASIC_INFORMATION() { BaseAddress = pMainModule.BaseAddress, RegionSize = (IntPtr)pMainModule.ModuleMemorySize };
            return SearchMask(mask, pMainModule.BaseAddress, pMainModule.ModuleMemorySize);
        }
        public MaskMatchResult SearchMask(MaskItem[] mask, IntPtr start, int length)
        {
            var maxBLength = 1024 * 1024 * 5;
            var result = new MaskMatchResult();
            var bytesRead = 0;
            byte[] leftOvers = null;
            var mLength = mask.Length;
            while (bytesRead < length)
            {
                var bytesToRead = (length - bytesRead) < maxBLength ? length - bytesRead :maxBLength;
                var buffer = Read(start + bytesRead, bytesToRead);
                IntPtr bufferStartOffset = start + bytesRead;
                if (leftOvers != null)
                {
                   var tBuffer = new byte[leftOvers.Length + buffer.Length];
                    Array.Copy(leftOvers, tBuffer, leftOvers.Length);
                    Array.Copy(buffer, 0, tBuffer, leftOvers.Length, buffer.Length);
                    buffer = tBuffer;
                    bufferStartOffset -= leftOvers.Length;
                }
                bytesRead += bytesToRead;
                var mStart = 0;
                while (mStart <= buffer.Length - mLength)
                {
                    if(IsMatch(mask, buffer,mStart, bufferStartOffset + mStart))
                    {
                        result.Success = true;
                        result.Matches.Add(Match(mask,buffer,mStart,bufferStartOffset+mStart));
                    }
                    mStart++;
                }
                leftOvers = new byte[buffer.Length - mStart];
                Array.Copy(buffer, mStart, leftOvers, 0, leftOvers.Length);
            }
            return result;
        }
        public bool IsMatch(MaskItem[] mask, byte[] bytes, int offset, IntPtr start)
        {
            if (bytes.Length - offset - mask.Length < 0) throw new Exception("Mask/Bytes length mismatch");
            for (var i = 0; i < mask.Length; i++)
            {
                if (mask[i].Type== MaskItem.MaskType.Byte && mask[i].Byte != bytes[offset + i])return false;
            }
            return true;
        }
        public MaskMatch Match(MaskItem[] mask, byte[] bytes, int offset, IntPtr start)
        {
            if (bytes.Length - offset - mask.Length < 0) throw new Exception("Mask/Bytes length mismatch");
            MaskMatch m = new MaskMatch() { Start = start };
            for (var i = 0; i < mask.Length; i++)
            {
                if (m == null) break;
                switch (mask[i].Type)
                {
                    case MaskItem.MaskType.Byte:
                        if (mask[i].Byte != bytes[offset + i]) m = null;
                        break;
                    case MaskItem.MaskType.Capture:
                        m.Captures.Add(bytes[offset + i]);
                        break;
                        //Ignore wildcard nothing to do there.
                }
            }
            return m;
        }
        public int Write(IntPtr offset, byte[] bytes)
        {
            int bytesWritten = 0;
            bool success = ImportsWin32.WriteProcessMemory(pHandle, offset, bytes, bytes.Length, ref bytesWritten);
            if(success)
            return bytesWritten;
            else throw new Exception("Write error", new Win32Exception(Marshal.GetLastWin32Error()));
        }
    }
    public struct MaskItem
    {
        public enum MaskType
        {
            Byte,
            Wildcard,
            Capture
        }
        public MaskType Type { get; set; }
        public byte Byte { get; set; }       

        public static implicit operator byte(MaskItem d)
        {
            return d.Byte;
        }
        public static implicit operator MaskItem(string c)
        {
            MaskItem m = new MaskItem();
            if (c == "?") m.Type = MaskType.Capture;
            else if (c == "*") m.Type = MaskType.Wildcard;
            else throw new Exception("Unrecognized mask");
            return m;
        }
        public static implicit operator MaskItem(byte b) 
        {
            MaskItem m = new MaskItem() { Type = MaskType.Byte, Byte = b };
            return m;
        }
    }
}
