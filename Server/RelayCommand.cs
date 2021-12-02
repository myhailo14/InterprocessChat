using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SocketChat
{
    public class RelayCommand : ICommand
    {
        protected readonly Func<bool> canExecute;

        protected readonly Action execute;

        public RelayCommand(Action execute, Func<bool> canExecute)
        {
            this.execute = execute ?? throw new ArgumentNullException("execute");
            this.canExecute = canExecute;
        }

        public RelayCommand(Action execute) : this(execute, null)
        {
        }

        public event EventHandler CanExecuteChanged
        {
            add
            {
                if (canExecute != null)
                {
                    CommandManager.RequerySuggested += value;
                }
            }

            remove
            {
                if (canExecute != null)
                {
                    CommandManager.RequerySuggested -= value;
                }
            }
        }

        public virtual bool CanExecute(object parameter)
        {
            return canExecute == null || canExecute();
        }

        public virtual void Execute(object parameter)
        {
            execute();
        }
    }
}