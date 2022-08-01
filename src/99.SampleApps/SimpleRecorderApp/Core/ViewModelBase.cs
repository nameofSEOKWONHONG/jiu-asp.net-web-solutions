using JetBrains.Annotations;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SimpleRecorderWpf.Core
{
    /// <summary title="동영상 문의 ViewModelBase">
    /// 1. Create Date :  2021.07.19
    /// 2. Creator : 홍석원
    /// 3. Description : ViewModelBase
    /// 4. Precaution :
    /// 5. History : 
    /// 6. MenuPath :  
    /// 7. OldName : NEW
    /// </summary>
    public class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected bool Set<T>(ref T field, T newValue = default, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, newValue))
            {
                return false;
            }

            field = newValue;

            this.OnPropertyChanged(propertyName);

            return true;
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}