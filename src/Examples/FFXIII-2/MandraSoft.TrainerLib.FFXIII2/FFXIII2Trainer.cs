using Mandrasoft.TrainerLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace MandraSoft.TrainerLib.FFXIII2
{
    class FFXIII2Trainer : ITrainer
    {
        public string ExecutableName => "ffxiii2img";

        public string GameName => "Final Fantasy XIII-2";

        private BitmapImage img;
        public BitmapImage HeaderImage
        {
            get
            {
                if (img == null)
                {
                    img = new BitmapImage(new Uri("pack://application:,,,/Resources/FinalFantasy_XIII-2_Logo.png"));
                }
                return img;
            }
        }

        public Dictionary<System.Windows.Forms.Keys, Patch> Patches => new Dictionary<System.Windows.Forms.Keys, Patch>();
        [STAThread]
        static public void Main()
        {
            TrainerHost.Run<FFXIII2Trainer>();
        }
    }
}
