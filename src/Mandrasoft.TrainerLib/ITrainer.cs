using System;
using System.Collections.Generic;
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
        string GameName { get; }
        string ExecutableName { get; }
        BitmapImage HeaderImage { get; }
        Dictionary<Keys, Patch> Patches { get; }
    }
    public interface IInjectedTrainer : ITrainer
    {
       
    }
}
