using System.Linq;

namespace PersonalTuringMachine.ViewModel
{
    public class CellViewModel : ViewModelBase
    {
        private readonly TapeViewModel _tape;

        public CellViewModel(char[] alphabet, char initialValue, bool hasHead = false) : this(alphabet, initialValue, hasHead, null)
        {
        }

        public CellViewModel(char[] alphabet, char initialValue, bool hasHead = false, TapeViewModel tape = null)
        {
            Alphabet = alphabet;
            Value = initialValue;
            HasHead = hasHead;
            _tape = tape;
        }

        public char[] Alphabet;

        private char _value;
        public char Value
        {
            get { return _value; }
            set
            {
                if (Alphabet.Contains(value)) SetField(ref _value, value);
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

        public bool ShowToolTip => this.OwningTapeDescription != null;

        public string OwningTapeDescription
        {
            get
            {
                if (_tape != null) { return $"Belongs to {_tape.ShortName}"; }
                else return null;
            }
        }

        public int OwningTapeNumber
        {
            get
            {
                if (_tape != null) { return _tape.Number; }
                else return -1;
            }
        }

        public string OwningTapeShortName
        {
            get
            {
                if (_tape != null) { return _tape.ShortName; }
                else return null;
            }
        }

        public bool _hasVirtualHead;
        public bool HasVirtualHead
        {
            get { return _hasVirtualHead; }
            set { SetField(ref _hasVirtualHead, value); }
        }
    }
}