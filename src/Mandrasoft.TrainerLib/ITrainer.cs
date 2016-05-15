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
        BitmapImage Icon { get; }
        Dictionary<Keys, Patch> Patches { get; }
    }
    public interface IInjectedTrainer : ITrainer
    {
       
    }
    public abstract class InjectedTrainerBase : IInjectedTrainer
    {
        public virtual Stream DisableSound =>null;

        public virtual Stream EnableSound => null;

        public abstract string ExecutableName {get;}

        public abstract string GameName { get; }

        public virtual BitmapImage HeaderImage => null;

        public virtual BitmapImage Icon => null;

        public abstract Dictionary<Keys, Patch> Patches { get; }
        
    }
}
