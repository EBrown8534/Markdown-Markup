using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Markdown_Markup
{
    public class DelegateCommand : ICommand
    {
        private readonly Predicate<object> _canExecute;
        private readonly Action<object> _execute;

        public DelegateCommand(Action<object> execute, Predicate<object> canExecute = null)
        {
            _canExecute = canExecute;
            _execute = execute;
        }

        private bool _canExecuteState;
        public bool CanExecute(object parameter)
        {
            var previousState = _canExecuteState;
            _canExecuteState = _canExecute == null || _canExecute.Invoke(parameter);

            if (previousState != _canExecuteState)
            {
                //OnCanExecuteChanged();
            }

            return _canExecuteState;
        }

        public void Execute(object parameter)
        {
            _execute.Invoke(parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        //public event EventHandler CanExecuteChanged;
        //protected void OnCanExecuteChanged()
        //{
        //    var handler = CanExecuteChanged;
        //    handler?.Invoke(this, EventArgs.Empty);
        //}        
    }
}
