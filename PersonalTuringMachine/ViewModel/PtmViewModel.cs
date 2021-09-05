using System.Collections.ObjectModel;
using System.Windows.Input;
using PersonalTuringMachine.Extensions;
using PersonalTuringMachine.CommandBinding;
using System;
using System.Linq;
using System.Collections.Generic;
using PersonalTuringMachine.Model;

namespace PersonalTuringMachine.ViewModel
{
    public class PtmViewModel : ViewModelBase
    {
        public PtmViewModel(char[] alphabet, char startSymbol, char emptySymbol, IEnumerable<TapeViewModel> initialTapes, IEnumerable<StateViewModel> initialStates, IEnumerable<TransitionFunctionViewModel> initialTransitionFunctions)
        {
            if (alphabet == null) throw new ArgumentNullException();
            if (!alphabet.Contains(startSymbol) || !alphabet.Contains(emptySymbol)) throw new ArgumentOutOfRangeException("Alphabet does not contain start or empty symbol");


            Alphabet = alphabet;
            StartSymbol = startSymbol;
            EmptySymbol = emptySymbol;
            Tapes = new ObservableCollection<TapeViewModel>();
            States = new ObservableCollection<StateViewModel>();
            TransitionFunctions = new ObservableCollection<TransitionFunctionViewModel>();

            if (initialTapes != null) { foreach (TapeViewModel tape in initialTapes) { Tapes.Add(tape); } }
            if (initialStates != null) { foreach (StateViewModel state in initialStates) { States.Add(state); } }
            if (initialTransitionFunctions != null) { foreach (TransitionFunctionViewModel func in initialTransitionFunctions) { TransitionFunctions.Add(func); } }

            AddTape = new DelegateCommand(OnAddTape);
            AddState = new DelegateCommand(OnAddState);
            AddTransitionFunction = new DelegateCommand(OnAddTransitionFunction);
            CalculateAllTransitionFunctions();
        }

        public char[] Alphabet { get; }
        public char StartSymbol { get; }
        public char EmptySymbol { get; }

        public string AlphabetConcat => Alphabet.ToDelimitedList<char>(", ");

        public ObservableCollection<TapeViewModel> Tapes { get; }

        public ObservableCollection<StateViewModel> States { get; }

        public ObservableCollection<TransitionFunctionViewModel> TransitionFunctions { get; }

        public ICommand AddTape { get; }

        public ICommand AddState { get; }

        public ICommand AddTransitionFunction { get; }

        public void EditTransitionFunction(TransitionFunctionViewModel funcViewModel)
        {
            TransitionFunctionViewModel copied = funcViewModel.Copy();

            this.OpenInModal(copied, (exitCode) =>
            {
                if (exitCode == ExitCode.Saved)
                {
                    int index = TransitionFunctions.IndexOf(funcViewModel);
                    TransitionFunctions.Insert(index, copied);
                    TransitionFunctions.Remove(funcViewModel);
                }
            });
        }

        public MachineSpec GetMachineSpec()
        {
            MachineSpec machineSpec = new MachineSpec
            {
                Alphabet = Alphabet,
                EmptySymbol = EmptySymbol,
                StartSymbol = StartSymbol,
                Tapes = new List<TapeSaveModel>(),
                States = new List<StateSaveModel>(),
                TransitionFunctions = new List<TransitionFunctionSaveModel>()
            };

            foreach (TapeViewModel tape in Tapes) { machineSpec.Tapes.Add(new TapeSaveModel { Number = tape.Number, Type = tape.Type }); }
            foreach (StateViewModel state in States) { machineSpec.States.Add(new StateSaveModel { Name = state.Name, IsReadOnly = state.IsReadOnly }); }
            foreach (TransitionFunctionViewModel func in TransitionFunctions)
            {
                TransitionFunctionSaveModel savedFunc = new TransitionFunctionSaveModel();
                savedFunc.InputStateName = func.SelectedInputState.Name;
                savedFunc.InputHeadReadArgs = func.InputHeadReadArgs.Select(x => x.ReadWriteValue).ToArray();
                savedFunc.OutputStateName = func.SelectedOutputState.Name;
                savedFunc.OutputWriteArgs = func.OutputHeadWriteArgs.Select(x => x.ReadWriteValue).ToArray();
                savedFunc.OutputMoveArgs = func.OutputHeadMoveArgs.Select(x => x.SelectedMoveSymbol.Symbol).ToArray();
                machineSpec.TransitionFunctions.Add(savedFunc);
            }

            return machineSpec;
        }

        public char[] GetInput()
        {
            TapeViewModel readTape = Tapes.Where(x => x.Type == TapeType.ReadOnly).First();
            return readTape.Cells.Select(x => x.Value).ToArray();
        }

        public void LoadInputOntoTape(char[] input)
        {
            TapeViewModel readTape = Tapes.Where(x => x.Type == TapeType.ReadOnly).First();
            readTape.Clear();
            foreach (char letter in input) { readTape.AddCell(letter); }
        }

        private void OnAddTape()
        {
            Tapes.Add(new TapeViewModel(Tapes.Count + 1, TapeType.ReadWrite, Alphabet, EmptySymbol, new List<CellViewModel> { new CellViewModel(Alphabet, StartSymbol) }));
        }

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

            this.OpenInModal(viewModel, (exitCode) =>
            {
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