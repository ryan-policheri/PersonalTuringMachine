using PersonalTuringMachine.Extensions;
using PersonalTuringMachine.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonalTuringMachine.ViewModel
{
    public class TransitionFunctionViewModel : ViewModelBase
    {
        public TransitionFunctionViewModel(IEnumerable<char> alphabet, ObservableCollection<TapeViewModel> tapes, ObservableCollection<StateViewModel> states)
        {
            Alphabet = new ObservableCollection<char>();
            foreach (char letter in alphabet) Alphabet.Add(letter);
            Tapes = tapes;
            States = states;

            InputHeadReadWriteArgs = new ObservableCollection<HeadReadWriteCommandViewModel>();
            foreach(TapeViewModel tape in Tapes) { InputHeadReadWriteArgs.Add(new HeadReadWriteCommandViewModel(tape, alphabet, HeadReadOrWrite.Read)); }

            OutputHeadWriteArgs = new ObservableCollection<HeadReadWriteCommandViewModel>();
            foreach(TapeViewModel tape in Tapes)
            {
                HeadReadWriteCommandViewModel readWriteCommand = new HeadReadWriteCommandViewModel(tape, alphabet, HeadReadOrWrite.Write);
                OutputHeadWriteArgs.Add(readWriteCommand);
            }

            OutputHeadMoveArgs = new ObservableCollection<HeadMoveCommandViewModel>();
            foreach (TapeViewModel tape in Tapes)
            {
                HeadMoveCommandViewModel moveCommand = new HeadMoveCommandViewModel(tape);
                OutputHeadMoveArgs.Add(moveCommand);
            }
        }

        public ObservableCollection<char> Alphabet { get; }

        public ObservableCollection<TapeViewModel> Tapes { get; }

        public ObservableCollection<StateViewModel> States { get; }

        private StateViewModel _selectedState;
        public StateViewModel SelectedState 
        {
            get { return _selectedState; }
            set { SetField(ref _selectedState, value); OnPropertyChanged(nameof(InputDisplay)); }
        }

        public ObservableCollection<HeadReadWriteCommandViewModel> InputHeadReadWriteArgs { get; }

        public string[] Input { get; set; }

        public string InputDisplay
        {
            get 
            {
                if (Input == null) return SelectedState?.Name;
                else return SelectedState?.Name + "(" + Input.ToDelimitedList<string>(", ") + ")"; 
            }
        }

        public string[] Output { get; set; }

        public string OutputDisplay { get; set; }

        public ObservableCollection<HeadReadWriteCommandViewModel> OutputHeadWriteArgs { get; }

        public ObservableCollection<HeadMoveCommandViewModel> OutputHeadMoveArgs { get; }

        public DisplayStatement => BuildDisplayStatement
        
    }
}
