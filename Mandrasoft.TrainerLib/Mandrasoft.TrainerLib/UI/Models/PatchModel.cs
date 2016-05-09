using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace Mandrasoft.TrainerLib.UI.Models
{
    class PatchModel : INotifyPropertyChanged
    {
        public Keys Key { get; set; }
        public bool Toggleable { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        private bool _Enabled;
        public bool Enabled { get { return _Enabled; } set { _Enabled = value; OnPropertyChanged(nameof(Enabled)); } }
        public Patch Patch { get; set; }

        public PatchModel() {}
        public PatchModel(Keys key, Patch patch)
        {
            Enabled = false;
            Key = key;
            Patch = patch;
            Title = patch.Title;
            Description = patch.Description;
            Toggleable = patch is TogglePatch;
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
