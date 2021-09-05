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

            InputHeadReadArgs = new ObservableCollection<HeadReadWriteCommandViewModel>();
            foreach (TapeViewModel tape in Tapes) { InputHeadReadArgs.Add(new HeadReadWriteCommandViewModel(tape, alphabet, HeadReadOrWrite.Read)); }

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

        public ObservableCollection<HeadReadWriteCommandViewModel> InputHeadReadArgs { get; }


        private StateViewModel _selectedOutputState;
        public StateViewModel SelectedOutputState
        {
            get { return _selectedOutputState; }
            set { SetField(ref _selectedOutputState, value); }
        }

        public ObservableCollection<HeadReadWriteCommandViewModel> OutputHeadWriteArgs { get; }

        public ObservableCollection<HeadMoveCommandViewModel> OutputHeadMoveArgs { get; }

        public string DisplayStatement => BuildDisplayStatement();

        public bool IsMatch(StateViewModel inputState, char[] tapeValues)
        {
            if (SelectedInputState.Name == inputState.Name)
            {
                char[] inputArgs = InputHeadReadArgs.Select(x => x.ReadWriteValue).ToArray();
                return Enumerable.SequenceEqual(inputArgs, tapeValues);
            }
            else return false;
        }

        public void AddInputState(StateViewModel currentState, char[] tapeValues)
        {
            this.SelectedInputState = currentState;
            for (int i = 0; i < tapeValues.Length; i++)
            {
                this.InputHeadReadArgs[i].ReadWriteValue = tapeValues[i];
            }
        }

        private string BuildDisplayStatement()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("(").Append(SelectedInputState?.Name);
            foreach (HeadReadWriteCommandViewModel item in InputHeadReadArgs)
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
