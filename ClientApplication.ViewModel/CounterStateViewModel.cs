using Application.Abstract;

namespace ClientApplication.ViewModel
{
    public class CounterStateViewModel : ViewModelBase
    {
        private int _count;
        public int Count
        {
            get => _count;
            set
            {
                _count = value;
                OnPropertyChanged();
            }
        }
        
        public override void Dispose()
        {
            
        }
    }
}