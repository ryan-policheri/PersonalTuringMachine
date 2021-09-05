using System.Linq;

namespace PersonalTuringMachine.ViewModel
{
    public class CellViewModel : ViewModelBase
    {
        private char[] _alphabet;

        public CellViewModel(char[] alphabet, char initialValue, bool hasHead = false)
        {
            _alphabet = alphabet;
            Value = initialValue;
            HasHead = hasHead;
        }

        private char _value;
        public char Value
        {
            get { return _value; }
            set 
            { 
                if (_alphabet.Contains(value)) SetField(ref _value, value);
            }
        }

        private bool _isReadOnly;
        public bool IsReadOnly 
        {
            get { return _isReadOnly; }
            set { SetField(ref _isReadOnly, value); }
        }

        private bool _hasHead;
        public bool HasHead 
        {
            get { return _hasHead; }
            set { SetField(ref _hasHead, value); }
        }
    }
}