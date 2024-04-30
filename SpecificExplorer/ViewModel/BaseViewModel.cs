using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SpecificExplorer.ViewModel
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyOfPropertyChange([CallerMemberName] string _propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(_propertyName));
            }
        }

        protected void SetProperty<T>(ref T _propertyReference, T value, [CallerMemberName] string _propertyName = "")
        {


            if (EqualityComparer<T>.Default.Equals(_propertyReference, value))
                return;

            //if value changed but didn't validate
            //if (validateValue != null )
            //	return;

            _propertyReference = value;
            NotifyOfPropertyChange(_propertyName);
        }
    }
}
