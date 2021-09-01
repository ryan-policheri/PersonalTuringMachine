using PersonalTuringMachine.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonalTuringMachine.ViewModel
{
    public class HeadReadWriteViewModel : ViewModelBase
    {
        public HeadReadWriteViewModel(TapeViewModel tape, IEnumerable<char> alphabet)
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
}
