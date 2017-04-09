using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Apocalypse.Utility
{
    public abstract class ObservableObject : INotifyPropertyChanged
    {
        protected ObservableObject()
        {
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (propertyName == null)
            {
                throw new ArgumentNullException(nameof(propertyName));
            }

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
