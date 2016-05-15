using Mandrasoft.TrainerLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using System.IO;

namespace MandraSoft.TrainerLib.InjectedFFX
{
    public class FFXTrainer : InjectedTrainerBase
    {
        public override string ExecutableName => "FFX";
        public override string GameName => "Final Fantasy X";  
        public override BitmapImage HeaderImage => new BitmapImage(new Uri("pack://application:,,,/Resources/header.jpg"));
        public override BitmapImage Icon => new BitmapImage(new Uri("pack://application:,,,/Resources/trainerIcon.ico"));
        public override Dictionary<System.Windows.Forms.Keys, Patch> Patches => new Dictionary<Keys, Patch> { { Keys.NumPad0, new InfiniteHealth() }, { Keys.NumPad1, new InfiniteGil() }, { Keys.NumPad2, new InfiniteMana() }, { Keys.NumPad3, new InfiniteSLevel() }, { Keys.NumPad4, new Add99Spheres() } };
        public override Stream EnableSound => Properties.Resources.ff7ok;
        public override Stream DisableSound => Properties.Resources.ff7cancel;

        [STAThread]
        public static void Main(string[] args)
        {
            InjectedTrainerHost.Run<FFXTrainer>();
        }
        public static int EntryPoint(string args)
        {
            InjectedTrainerHost.SetHooks<FFXTrainer>(args);
            return 1;
            //throw new NotImplementedException();
        }
    }
}
