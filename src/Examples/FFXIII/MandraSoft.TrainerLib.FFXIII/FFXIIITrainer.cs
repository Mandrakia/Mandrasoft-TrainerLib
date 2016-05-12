using Mandrasoft.TrainerLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace MandraSoft.TrainerLib.FFXIII
{
    class FFXIIITrainer : ITrainer
    {
        public string ExecutableName => "ffxiiiimg";

        public string GameName => "Final Fantasy XIII";

        private BitmapImage img;
        public BitmapImage HeaderImage { get
            {
                if (img == null)
                {
                    img = new BitmapImage(new Uri("pack://application:,,,/Resources/header.jpg"));
                }
                return img;
            } }

        public Dictionary<Keys, Patch> Patches => new Dictionary<Keys, Patch> { { Keys.NumPad0, new NoCostCrystallium() }, { Keys.NumPad1, new AddGilPatch() }, { Keys.NumPad2, new RandomPatch() }  };

        [STAThread]
        public static void Main(string[] args)
        {
            TrainerHost.Run<FFXIIITrainer>();
        }
    }
}
