using System;
using System.Threading.Tasks;
using System.ComponentModel;

namespace MyApp.Models
{
    public class Mechanic : IMechanic, INotifyPropertyChanged
    {
        private bool _isBusy;
        
        public bool IsBusy
        {
            get => _isBusy;
            private set
            {
                if (_isBusy != value)
                {
                    _isBusy = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsBusy)));
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public async void FixConveyor()
        {
            if (IsBusy) return;
            
            IsBusy = true;
            await Task.Delay(3000);
            IsBusy = false;
        }
    }
}