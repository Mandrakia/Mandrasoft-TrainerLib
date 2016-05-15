using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MandraSoft.TrainerLib.Packer
{
    class Program
    {
        static void Main(string[] args)
        {
            string dir = args[0];
            string outputFolder = args[1];
            string tmpPath = Path.GetTempPath() + Path.GetRandomFileName();
            Directory.CreateDirectory(tmpPath);
            foreach (var f in Directory.EnumerateFiles(dir, "*.exe"))
            {
                if (f.ToLower().Contains("vshost") || f.ToLower().Contains("easyhook32svc.exe") || f.ToLower().Contains("easyhook64svc.exe")) continue;
                File.Copy(f, Path.Combine(tmpPath, Path.GetFileName(f)));
            }
            foreach (var f in Directory.EnumerateFiles(dir, "*.dll"))
            {
                if (f.ToLower().Contains("vshost") || f.ToLower().Contains("easyhook32svc.exe") || f.ToLower().Contains("easyhook64svc.exe")) continue;
                File.Copy(f, Path.Combine(tmpPath, Path.GetFileName(f)));
            }
            if (!Directory.Exists(outputFolder)) Directory.CreateDirectory(outputFolder);
            if (File.Exists(Path.Combine(outputFolder,"Packed.zip"))) File.Delete(Path.Combine(outputFolder,"Packed.zip"));
            ZipFile.CreateFromDirectory(tmpPath,Path.Combine(outputFolder,"Packed.zip"), CompressionLevel.Fastest, false);
        }
    }
}
