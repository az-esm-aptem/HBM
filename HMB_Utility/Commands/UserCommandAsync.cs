using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace HMB_Utility
{
    public class UserCommandAsync : ICommand
    {
        private Func<object, Task<bool>> execute;
        private Func<object, bool> canExecute;
        public UserCommandAsync(Func<object, Task<bool>> execute, Func<object, bool> canExecute = null)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            return canExecute == null || canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            execute(parameter);
        }
    }
}
