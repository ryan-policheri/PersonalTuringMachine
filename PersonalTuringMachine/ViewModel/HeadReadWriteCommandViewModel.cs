using PersonalTuringMachine.Extensions;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace PersonalTuringMachine.ViewModel
{
    public class HeadReadWriteCommandViewModel : ViewModelBase
    {
        public HeadReadWriteCommandViewModel(TapeViewModel tape, IEnumerable<char> alphabet, HeadReadOrWrite readOrWrite)
        {
            TapeName = tape.ShortName;
            ToolTip = tape.LongName;
            ReadWriteValueOptions = alphabet.ToObservableCollection();
        }

        public string TapeName { get; }

        public string ToolTip { get; }

        public ObservableCollection<char> ReadWriteValueOptions { get; }

        private char _readWriteValue;
        public char ReadWriteValue 
        {
            get { return _readWriteValue; }
            set { SetField(ref _readWriteValue, value); }
        }
    }

    public enum HeadReadOrWrite
    {
        Read,
        Write
    }
}
