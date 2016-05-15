using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Mandrasoft.TrainerLib.UI.Models
{
    class TrainerModel : INotifyPropertyChanged
    {
        private bool _GameFound;
        public bool GameFound { get { return _GameFound; } set {  _GameFound = value; OnPropertyChanged(nameof(GameFound)); } }
        public BitmapImage HeaderImage { get; set; }
        public BitmapImage Icon { get; set; }
        public string TitleWindow { get; set; }
        public List<PatchModel> Patches { get; set; }
        internal ITrainer Trainer { get; set; }
        public IGameWriter Writer { get; set; }

        public TrainerModel() { }
        public TrainerModel(ITrainer trainer)
        {
            Trainer = trainer;
            TitleWindow = trainer.GameName;
            Patches = new List<PatchModel>();
            try
            {
                HeaderImage = trainer.HeaderImage;
                Icon = trainer.Icon;
            }
            catch { }
            foreach (var p in trainer.Patches)
            {
                Patches.Add(new PatchModel(p.Key,p.Value));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
