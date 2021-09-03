namespace PersonalTuringMachine.ViewModel
{
    public class CellViewModel : ViewModelBase
    {
        private char _value;
        private char[] alphabet;

        public CellViewModel(char[] alphabet, char initialValue)
        {
            this.alphabet = alphabet;
            Value = initialValue;
        }

        public char Value
        {
            get { return _value; }
            set { SetField(ref _value, value); }
        }
    }
}