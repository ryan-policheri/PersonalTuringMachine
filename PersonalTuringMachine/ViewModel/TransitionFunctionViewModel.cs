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
            foreach (TapeViewModel tape in Tapes) { InputHeadReadWriteArgs.Add(new HeadReadWriteCommandViewModel(tape, alphabet, HeadReadOrWrite.Read)); }

            OutputHeadWriteArgs = new ObservableCollection<HeadReadWriteCommandViewModel>();
            foreach (TapeViewModel tape in Tapes)
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


        private StateViewModel _selectedInputState;
        public StateViewModel SelectedInputState
        {
            get { return _selectedInputState; }
            set { SetField(ref _selectedInputState, value); }
        }

        public ObservableCollection<HeadReadWriteCommandViewModel> InputHeadReadWriteArgs { get; }


        private StateViewModel _selectedOutputState;
        public StateViewModel SelectedOutputState
        {
            get { return _selectedOutputState; }
            set { SetField(ref _selectedOutputState, value); }
        }

        public ObservableCollection<HeadReadWriteCommandViewModel> OutputHeadWriteArgs { get; }

        public ObservableCollection<HeadMoveCommandViewModel> OutputHeadMoveArgs { get; }

        public string DisplayStatement => BuildDisplayStatement();

        private string BuildDisplayStatement()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("(").Append(SelectedInputState?.Name);
            foreach (HeadReadWriteCommandViewModel item in InputHeadReadWriteArgs)
            {
                builder.Append(", " + item.ReadWriteValue);
            }
            builder.Append(")  ⟶  (")
                .Append(SelectedOutputState?.Name);

            foreach (HeadReadWriteCommandViewModel item in OutputHeadWriteArgs)
            {
                builder.Append(", " + item.ReadWriteValue);
            }

            foreach(HeadMoveCommandViewModel item in OutputHeadMoveArgs)
            {
                builder.Append(", " + item.SelectedMoveSymbol?.Symbol);
            }
            builder.Append(")");

            return builder.ToString();
        }
    }
}
