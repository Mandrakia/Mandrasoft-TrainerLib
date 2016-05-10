using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Mandrasoft.TrainerLib.ImportsWin32;

namespace Mandrasoft.TrainerLib
{
    unsafe class InProcessGameWriter : IGameWriter
    {
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
            var maxBLength = 1024 * 1024 * 5;
            var result = new MaskMatchResult();
            var bytesRead = 0;
            byte[] leftOvers = null;
            var mLength = mask.Length;
            while (bytesRead < length)
            {
                var bytesToRead = (length - bytesRead) < maxBLength ? length - bytesRead : maxBLength;
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
                    if (IsMatch(mask, buffer, mStart, bufferStartOffset + mStart))
                    {
                        result.Success = true;
                        result.Matches.Add(Match(mask, buffer, mStart, bufferStartOffset + mStart));
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
                if (mask[i].Type == MaskItem.MaskType.Byte && mask[i].Byte != bytes[offset + i]) return false;
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
            byte* b = (byte*)offset;
            foreach (byte by in bytes)
            {
                *b = by;
                b++;
            }
            return bytes.Length;
        }
    }
}
