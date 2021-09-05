using System.Collections.ObjectModel;
using System.Windows.Input;
using PersonalTuringMachine.Extensions;
using PersonalTuringMachine.CommandBinding;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection.PortableExecutable;
using System.Threading;
using PersonalTuringMachine.Model;

namespace PersonalTuringMachine.ViewModel
{
    public class PtmViewModel : ViewModelBase
    {
        private const string _startState = "Qstart";
        private const string _haltState = "Qhalt";
        private readonly Timer _timer;
        private SynchronizationContext _context;

        public PtmViewModel(char[] alphabet, char startSymbol, char emptySymbol, IEnumerable<TapeViewModel> initialTapes, IEnumerable<StateViewModel> initialStates, IEnumerable<TransitionFunctionViewModel> initialTransitionFunctions)
        {
            if (alphabet == null) throw new ArgumentNullException();
            if (!alphabet.Contains(startSymbol) || !alphabet.Contains(emptySymbol)) throw new ArgumentOutOfRangeException("Alphabet does not contain start or empty symbol");
            _context = SynchronizationContext.Current;
            _timer = new Timer(OnTick, this, Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);

            Alphabet = alphabet;
            StartSymbol = startSymbol;
            EmptySymbol = emptySymbol;
            Tapes = new ObservableCollection<TapeViewModel>();
            States = new ObservableCollection<StateViewModel>();
            TransitionFunctions = new ObservableCollection<TransitionFunctionViewModel>();

            if (initialTapes != null) { foreach (TapeViewModel tape in initialTapes) { Tapes.Add(tape); } }

            if (Tapes != null && Tapes.Count > 0)
            {
                Tapes.First().PropertyChanged += (sender, args) =>
                {
                    if (args.PropertyName == nameof(TapeViewModel.ActiveCellCount)) OnPropertyChanged(nameof(InputLength));
                };
            }

            if (initialStates != null) { foreach (StateViewModel state in initialStates) { States.Add(state); } }
            if (initialTransitionFunctions != null) { foreach (TransitionFunctionViewModel func in initialTransitionFunctions) { TransitionFunctions.Add(func); } }

            AddTape = new DelegateCommand(OnAddTape);
            DeleteTape = new DelegateCommand<TapeViewModel>(OnDeleteTape);
            AddState = new DelegateCommand(OnAddState);
            AddTransitionFunction = new DelegateCommand(OnAddTransitionFunction);
            ToggleMachineOnOff = new DelegateCommand(OnToggleMachineOnOff);

            MachineOn = false;
            CurrentState = States.Where(x => x.Name == _startState).FirstOrDefault();
            ExecutionSpeed = 5;
        }

        public char[] Alphabet { get; }
        public char StartSymbol { get; }
        public char EmptySymbol { get; }

        public string AlphabetConcat => Alphabet.ToDelimitedList<char>(", ");

        public ObservableCollection<TapeViewModel> Tapes { get; }

        public ObservableCollection<StateViewModel> States { get; }

        public ObservableCollection<TransitionFunctionViewModel> TransitionFunctions { get; }

        public ICommand AddTape { get; }

        public ICommand DeleteTape { get; }

        public ICommand AddState { get; }

        public ICommand AddTransitionFunction { get; }

        public ICommand ToggleMachineOnOff { get; }


        private bool _machineOn;
        public bool MachineOn
        {
            get { return _machineOn; }
            set
            {
                SetField(ref _machineOn, value);
                OnPropertyChanged(nameof(MachineOnOffDisplay));
            }
        }

        private string _machineOnOffDisplay;
        public string MachineOnOffDisplay
        {
            get
            {
                if (MachineOn) return "Stop Machine";
                else return "Start Machine";
            }
        }

        private StateViewModel _currentState;
        public StateViewModel CurrentState
        {
            get { return _currentState; }
            private set { SetField(ref _currentState, value); }
        }

        private int _executionSpeed;
        public int ExecutionSpeed
        {
            get { return _executionSpeed; }
            set
            {
                SetField(ref _executionSpeed, value);
                if (MachineOn) _timer.Change(0, CalculateTickSpeed());
            }
        }

        private TransitionFunctionViewModel _currentTransitionFunction;
        public TransitionFunctionViewModel CurrentTransitionFunction
        {
            get { return _currentTransitionFunction; }
            set { SetField(ref _currentTransitionFunction, value); }
        }

        public int InputLength
        {
            get
            {
                if (Tapes == null || Tapes.Count == 0) return 0;
                else return Tapes.First().GetLastValueIndex() + 1;
            }
        }

        private int _stepCount;
        public int StepCount
        {
            get { return _stepCount; } private set { SetField(ref _stepCount, value); }
        }

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
            Tapes.Add(new TapeViewModel(Tapes.Count + 1, TapeType.ReadWrite, Alphabet, EmptySymbol, new List<CellViewModel> { new CellViewModel(Alphabet, StartSymbol, true) }));
        }

        private void OnDeleteTape(TapeViewModel obj)
        {
            Tapes.Remove(obj);
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
        }

        private void OnAddTransitionFunction()
        {
            TransitionFunctionViewModel viewModel = new TransitionFunctionViewModel(Alphabet, Tapes, States);

            this.OpenInModal(viewModel, (exitCode) =>
            {
                if (exitCode == ExitCode.Saved) TransitionFunctions.Add(viewModel);
            });
        }

        private void OnToggleMachineOnOff()
        {
            MachineOn = !MachineOn;
            if (MachineOn) _timer.Change(0, CalculateTickSpeed());
            else _timer.Change(Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
        }

        private int CalculateTickSpeed()
        {
            int subtractOne = ExecutionSpeed - 1;
            double fraction = subtractOne / 20f; //20 is max value
            double inverseFraction = 1 - fraction;
            int milliseconds = (int)(inverseFraction * 1000);
            return milliseconds;
        }

        private void OnTick(object stateInfo)
        {
            _context.Post(stateInfo =>
            {
                CurrentTransitionFunction = GetCurrentTransitionFunction();
                if (CurrentTransitionFunction == null)
                {
                    var function = new TransitionFunctionViewModel(Alphabet, Tapes, States);
                    char[] tapeValues = ReadTapeValues();
                    function.AddInputState(this.CurrentState, tapeValues);
                    _timer.Change(Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
                    this.OpenInModal(function, (exitCode) =>
                    {
                        if (exitCode == ExitCode.Saved) TransitionFunctions.Add(function);
                        OnTick(stateInfo);
                    });
                    _timer.Change(0, CalculateTickSpeed());
                }
                else
                {
                    CurrentState = CurrentTransitionFunction.SelectedOutputState;
                    for (int i = 0; i < Tapes.Count; i++)
                    {
                        Tapes[i].WriteHeadValue(CurrentTransitionFunction.OutputHeadWriteArgs[i].ReadWriteValue);
                    }
                    for (int i = 0; i < Tapes.Count; i++)
                    {
                        Tapes[i].MoveHead(CurrentTransitionFunction.OutputHeadMoveArgs[i].SelectedMoveSymbol);
                    }

                    StepCount++;
                }
            }, null);
        }

        private TransitionFunctionViewModel GetCurrentTransitionFunction()
        {
            char[] tapeValues = ReadTapeValues();

            foreach (TransitionFunctionViewModel function in TransitionFunctions)
            {
                bool matchesState = function.IsMatch(this.CurrentState, tapeValues);
                if (matchesState) return function;
            }

            return null;
        }

        private char[] ReadTapeValues()
        {
            ICollection<char> values = new List<char>();
            foreach (TapeViewModel tape in Tapes)
            {
                values.Add(tape.ReadHeadValue());
            }

            return values.ToArray();
        }
    }
}