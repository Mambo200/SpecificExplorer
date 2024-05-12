using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SpecificExplorer.ViewModel
{
    public class RelayCommand : ICommand
    {

        private Action<object> m_executionReference;
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter) => m_executionReference(parameter);

        public RelayCommand(Action<object> _execute)
        {
            m_executionReference = _execute;
        }
    }
}
