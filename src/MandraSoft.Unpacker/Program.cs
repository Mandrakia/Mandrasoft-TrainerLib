using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MandraSoft.Unpacker
{
    class Program
    {
        static void Main(string[] args)
        {
            var tmp = Properties.Resources.Packed;
            var tmpPath = Path.GetTempPath() + Path.GetRandomFileName() + "\\";
            Directory.CreateDirectory(tmpPath);
            File.WriteAllBytes(Path.Combine(tmpPath, "Packed.zip"), tmp);
            ZipFile.ExtractToDirectory(Path.Combine(tmpPath, "Packed.zip"), tmpPath);
            File.Delete(Path.Combine(tmpPath, "Packed.zip"));
            var exePath = Directory.EnumerateFiles(tmpPath, "*.exe").First();

            var psi = new ProcessStartInfo(exePath);
            psi.WorkingDirectory = tmpPath;
            psi.LoadUserProfile = true;     
            var proc = Process.Start(psi);
            proc.WaitForExit();
            try
            {
                Directory.Delete(tmpPath, true);
            }
            catch { }
        }
    }
}
