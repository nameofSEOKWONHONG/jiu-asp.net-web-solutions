using System;
using System.Windows.Input;

namespace SimpleRecorderWpf.Core
{
    /// <summary title="동영상 문의 SimpleCommand">
    /// 1. Create Date :  2021.07.19
    /// 2. Creator : 홍석원
    /// 3. Description : SimpleCommand
    /// 4. Precaution :
    /// 5. History : 
    /// 6. MenuPath :  
    /// 7. OldName : NEW
    /// </summary>
    public class SimpleCommand : ICommand
    {
        public SimpleCommand(Func<object, bool> canExecute = null, Action<object> execute = null)
        {
            this.CanExecuteDelegate = canExecute;
            this.ExecuteDelegate = execute;
        }

        public Func<object, bool> CanExecuteDelegate { get; set; }

        public Action<object> ExecuteDelegate { get; set; }

        public bool CanExecute(object parameter)
        {
            var canExecute = this.CanExecuteDelegate;
            return canExecute == null || canExecute(parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public void Execute(object parameter)
        {
            this.ExecuteDelegate?.Invoke(parameter);
        }
    }
}