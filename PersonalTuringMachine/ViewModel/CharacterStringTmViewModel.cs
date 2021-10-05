using PersonalTuringMachine.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonalTuringMachine.ViewModel
{
    public class CharacterStringTmViewModel : PtmViewModel
    {
        private TapeViewModel _inputTape;
        private TapeViewModel _segmentedTape;

        public CharacterStringTmViewModel(char[] alphabet, char startSymbol, char emptySymbol, 
            IEnumerable<TapeViewModel> initialTapes,
            IEnumerable<StateViewModel> initialStates,
            IEnumerable<TransitionFunctionViewModel> initialTransitionFunctions) 
            : base(alphabet, startSymbol, emptySymbol, initialTapes, initialStates, initialTransitionFunctions)
        {
            InitializeRegister();
        }

        protected override void LoadTapes(IEnumerable<TapeViewModel> initialTapes)
        {
            ICollection<TapeViewModel> workTapes = new List<TapeViewModel>();

            if (initialTapes != null)
            {
                foreach (TapeViewModel tape in initialTapes)
                {
                    if (tape.Type == TapeType.ReadOnly)
                    {
                        Tapes.Add(tape);
                        tape.PropertyChanged += (sender, args) =>
                        {
                            if (args.PropertyName == nameof(TapeViewModel.ActiveCellCount)) OnPropertyChanged(nameof(InputLength));
                        };
                        _inputTape = tape;
                    }
                    else { workTapes.Add(tape); }
                }
            }

            List<CellViewModel> initCells = new List<CellViewModel>() { new CellViewModel(Alphabet, StartSymbol, true), new CellViewModel(Alphabet, StartSymbol, false), new CellViewModel(Alphabet, StartSymbol, false), new CellViewModel(Alphabet, StartSymbol, false) };
            _segmentedTape = new SegmentedTapeViewModel(2, TapeType.ReadWrite, Alphabet, EmptySymbol, initCells);
            Tapes.Add(_segmentedTape);
            InitializeRegister();
        }

        public bool HasRegister => Register != null;

        public RegisterViewModel Register { get; private set; }

        private void InitializeRegister()
        {
            Register =  new RegisterViewModel(new string[] { "T2_Slot1", "T2_Slot2", "T2_Slot3", "T2_Slot4" });
        }
    }
}
