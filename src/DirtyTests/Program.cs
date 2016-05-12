using Mandrasoft.TrainerLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DirtyTests
{
    class Program
    {
        static void Main(string[] args)
        {
            GameWriter g = new GameWriter(System.Diagnostics.Process.GetProcessesByName("ffxiiiimg")[0]);
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            var res = g.ApplyPatch(new MaskItem[] { 0x8b, 0x84, 0x24, 0x28, 0x01, 0x00, 0x00, 0x8b, 0x88, 0xc0, 0x44, 0x00, 0x00, "?", "?", "?", "?" },new byte[] { 0x90, 0x90, 0x90, 0x90 });
            Console.WriteLine("Old version : " + sw.ElapsedMilliseconds.ToString() + "ms");    
            Console.Write(res);
            Console.ReadLine();

            AddMoney mn = Toto;
            IntPtr pt = Marshal.GetFunctionPointerForDelegate(mn);
        }
        static private int Toto(object k, int m)
        {
            var p = k.GetType();
            return 0;
        }
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate int AddMoney(object unknown,int amount);
    }
}
