using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Mandrasoft.TrainerLib
{
    public interface ITrainer
    {
        Stream EnableSound { get; }
        Stream DisableSound { get; }
        string GameName { get; }
        string ExecutableName { get; }
        BitmapImage HeaderImage { get; }
        Dictionary<Keys, Patch> Patches { get; }
    }
    public interface IInjectedTrainer : ITrainer
    {
       
    }
}
