using System.Collections.ObjectModel;
using System.Windows.Input;
using PersonalTuringMachine.Extensions;
using PersonalTuringMachine.CommandBinding;
using System;
using System.Linq;
using System.Collections.Generic;

namespace PersonalTuringMachine.ViewModel
{
    public class PtmViewModel : ViewModelBase
    {
        public PtmViewModel(char[] alphabet, IEnumerable<TapeViewModel> defaultTapes, IEnumerable<StateViewModel> defaultStates)
        {
            Alphabet = alphabet;
            Tapes = new ObservableCollection<TapeViewModel>();
            States = new ObservableCollection<StateViewModel>();
            TransitionFunctions = new ObservableCollection<TransitionFunctionViewModel>();

            if (defaultTapes != null) { foreach (TapeViewModel tape in defaultTapes) { Tapes.Add(tape); } }
            if (defaultStates != null) { foreach (StateViewModel state in defaultStates) { States.Add(state); } }

            AddState = new DelegateCommand(OnAddState);
            AddTransitionFunction = new DelegateCommand(OnAddTransitionFunction);
            CalculateAllTransitionFunctions();
        }

        public char[] Alphabet { get; }

        public string AlphabetConcat => Alphabet.ToDelimitedList<char>(", ");

        public ObservableCollection<TapeViewModel> Tapes { get; }

        public ObservableCollection<StateViewModel> States { get; }

        public ObservableCollection<TransitionFunctionViewModel> TransitionFunctions { get; }

        public ICommand AddState { get; }

        public ICommand AddTransitionFunction { get; }

        private void OnAddState()
        {
            string newStateName = "Qnewstate";
            int counter = 1;
            while (States.Any(x => x.Name == newStateName))
            {
                newStateName = newStateName.TrimEnd(counter.ToString());
                counter++;
                newStateName = newStateName + counter;
            }
            States.Add(new StateViewModel(newStateName, true));
            CalculateAllTransitionFunctions();
        }

        private void OnAddTransitionFunction()
        {
            TransitionFunctionViewModel viewModel = new TransitionFunctionViewModel(Alphabet, Tapes, States);

            this.OpenInModal(viewModel, (exitCode) => { 
                if (exitCode == ExitCode.Saved) TransitionFunctions.Add(viewModel);
            });
        }

        private void CalculateAllTransitionFunctions()
        {
            //Turns out this is difficult...
        //    TransitionFunctions.Clear();

        //    foreach (StateViewModel state in States)
        //    {
        //        List<string> inputArray = new List<string>();
        //        inputArray.Add(state.Name);
        //        int[] alphabetTapeCounter = new int[Tapes.Count];
        //        AppendAlphabetToInput(inputArray, alphabetTapeCounter);

        //        int[] data = new int[Tapes.Count];
        //        CombinationUtil(Alphabet, Alphabet.Length, Tapes.Count, 0, data, 0);
        //    }
        }
    }
}