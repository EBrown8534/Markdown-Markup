using System;
using System.Collections.Generic;
using System.Globalization;
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

        public KeyGesture Gesture { get; set; }

        public string GestureText
        {
            get
            {
                return Gesture?.GetDisplayStringForCulture(CultureInfo.CurrentUICulture);
            }
        }

        public DelegateCommand(Action<object> execute, Predicate<object> canExecute = null)
        {
            _canExecute = canExecute;
            _execute = execute;
        }
        
        public bool CanExecute(object parameter) => _canExecute == null || _canExecute.Invoke(parameter);

        public void Execute(object parameter)
        {
            _execute.Invoke(parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }      
    }
}
