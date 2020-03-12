using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media.Imaging;

namespace Mandrasoft.TrainerLib.Wolcen
{
    class WolcenTrainer : ITrainer
    {
        public string ExecutableName => "wolcen";

        public string GameName => "Wolcen";
        public Stream DisableSound => Properties.Resources.ff7cancel;
        public Stream EnableSound => Properties.Resources.ff7ok;

        public BitmapImage Icon => new BitmapImage(new Uri("pack://application:,,,/Resources/trainerIcon.ico"));

        private BitmapImage img;
        public BitmapImage HeaderImage
        {
            get
            {
                if (img == null)
                {
                    img = new BitmapImage(new Uri("pack://application:,,,/Resources/header.jpg"));
                }
                return img;
            }
        }

        public Dictionary<Keys, Patch> Patches => new Dictionary<Keys, Patch> { { Keys.F1, new AutoCraft() }, { Keys.F2, new AutoArmorCraft() },{ Keys.F3, new Dupe() } };

        [STAThread]
        public static void Main(string[] args)
        {
            TrainerHost.Run<WolcenTrainer>();
        }
    }
}
