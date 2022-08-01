using System;
using System.Windows.Input;

namespace SimpleRecorderWpf.Core
{
    /// <summary title="동영상 문의 RelayCommand">
    /// 1. Create Date :  2021.07.19
    /// 2. Creator : 홍석원
    /// 3. Description : RelayCommand
    /// 4. Precaution :
    /// 5. History : 
    /// 6. MenuPath :  
    /// 7. OldName : NEW
    /// </summary>
    public class RelayCommand : ICommand
    {
        private Action<object> execute;
        private Func<object, bool> canExecute;

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return this.canExecute == null || this.canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            this.execute(parameter);
        }
    }
}