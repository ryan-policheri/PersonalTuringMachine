namespace PersonalTuringMachine.ViewModel
{
    public class CellViewModel : ViewModelBase
    {
        private char _value;

        public char Value
        {
            get { return _value; }
            set
            { 
                if(value != _value)
                {
                    _value = value;
                    OnPropertyChanged();
                }           
            }
        }
    }
}