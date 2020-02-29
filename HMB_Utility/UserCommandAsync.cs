using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace HMB_Utility
{
    public class UserCommandAsync : ICommand
    {
        public event EventHandler CanExecuteChanged;
        private Func<object, Task<bool>> execute;
        private Func<object, bool> canExecute;
        public UserCommandAsync(Func<object, Task<bool>> execute, Func<object, bool> canExecute = null)
        {
            this.execute = execute;
            this.canExecute = canExecute;
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
