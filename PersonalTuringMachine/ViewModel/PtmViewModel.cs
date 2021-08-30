using System.Collections.ObjectModel;
using System.Windows.Input;
using PersonalTuringMachine.Extensions;
using PersonalTuringMachine.CommandBinding;
using System;
using System.Linq;
using System.Collections.Generic;

namespace PersonalTuringMachine.ViewModel
{
    public class PtmViewModel
    {
        public PtmViewModel(char[] alphabet, IEnumerable<TapeViewModel> defaultTapes, IEnumerable<StateViewModel> defaultStates)
        {
            Alphabet = alphabet;
            Tapes = new ObservableCollection<TapeViewModel>();
            States = new ObservableCollection<StateViewModel>();

            if (defaultTapes != null) { foreach (TapeViewModel tape in defaultTapes) { Tapes.Add(tape); } }
            if (defaultStates != null) { foreach (StateViewModel state in defaultStates) { States.Add(state); } }

            AddState = new DelegateCommand(OnAddState);
        }

        public char[] Alphabet { get; }

        public string AlphabetConcat => Alphabet.ToDelimitedList<char>(' ');

        public ObservableCollection<TapeViewModel> Tapes { get; }

        public ObservableCollection<StateViewModel> States { get; }

        public ICommand AddState { get; }

        private void OnAddState()
        {
            string newStateName = "Qnewstate";
            int counter = 1;
            while(States.Any(x => x.Name == newStateName))
            {
                newStateName = newStateName.TrimEnd(counter.ToString());
                counter++;
                newStateName = newStateName + counter;
            }
            States.Add(new StateViewModel(newStateName));
        }
    }
}
