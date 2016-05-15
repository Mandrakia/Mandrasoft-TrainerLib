using Mandrasoft.TrainerLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Windows.Media.Imaging;

namespace MandraSoft.TrainerLib.FFX_2
{
    class FFX2Trainer : InjectedTrainerBase
    {
        public override string ExecutableName => "FFX-2";

        public override string GameName => "Final Fantasy X-2 HD Remaster";
        public override Stream DisableSound => Properties.Resources.ff7cancel;
        public override Stream EnableSound => Properties.Resources.ff7ok;
        public override BitmapImage HeaderImage => new BitmapImage(new Uri("pack://application:,,,/Resources/header.jpg"));
        public override BitmapImage Icon => new BitmapImage(new Uri("pack://application:,,,/Resources/trainerIcon.ico"));

        public override Dictionary<Keys, Patch> Patches => new Dictionary<Keys, Patch>() { { Keys.NumPad0, new InfiniteHealthPatch() }, { Keys.NumPad1, new ExtraXPPatch() } };

        [STAThread]
        static public void Main()
        {
            InjectedTrainerHost.Run<FFX2Trainer>();
        }

        static public int EntryPoint(string args)
        {
            InjectedTrainerHost.SetHooks<FFX2Trainer>(args);
            return 1;
            //Can run any code you want here will be run inside the game Process !
        }
    }
}
