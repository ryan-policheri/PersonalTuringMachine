using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using PersonalTuringMachine.Model;

namespace PersonalTuringMachine.ViewModel
{
    public class ObliviousTmViewModel : PtmViewModel
    {
        private TapeViewModel _inputTape;
        private MultiTapeViewModel _multiTape;

        private int _cycleTracker = 0;
        private bool _movingLeft; //If false moving right is assumed
        private IList<string> _tapesToMoveRight;
        private IList<string> _tapesToMoveLeft;

        private ObliviousTmState _obliviousTmState;

        public ObliviousTmViewModel(char[] alphabet, char startSymbol, char emptySymbol,
            IEnumerable<TapeViewModel> initialTapes,
            IEnumerable<StateViewModel> initialStates,
            IEnumerable<TransitionFunctionViewModel> initialTransitionFunctions) : base(alphabet, startSymbol, emptySymbol, initialTapes, initialStates, initialTransitionFunctions)
        {
            InitializeRegister();
            _tapesToMoveRight = new List<string>();
            _tapesToMoveLeft = new List<string>();
            _obliviousTmState = ObliviousTmState.LoadHeadValuesToRegister;
        }

        public bool HasRegister => Register != null;

        public RegisterViewModel Register { get; private set; }

        public int MaxIndex => CalculateMaxIndex();

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

            _multiTape = new MultiTapeViewModel(2, TapeType.ReadWrite, Alphabet, EmptySymbol, workTapes);
            Tapes.Add(_multiTape);
            InitializeRegister();
        }

        private void InitializeRegister()
        {
            Register = new RegisterViewModel(_multiTape.GetSourceTapes().Select(x => x.ShortName).ToArray());

            //foreach (CellViewModel cell in _multiTape.Cells)
            //{
            //    if (cell.HasVirtualHead) { Register.EnterValue(cell.OwningTapeShortName, cell.Value); }
            //}
        }

        private int CalculateMaxIndex()
        {
            int maxIndex = _cycleTracker * _multiTape.TapeCount + _multiTape.TapeCount;
            return --maxIndex;
        }

        private char[] ReadTapeValues()
        {
            char[] values = Register.ReadRegisterValues();
            values = values.Prepend(_inputTape.ReadHeadValue()).ToArray();
            return values;
        }

        protected override void OnTick(object stateInfo)
        {
            if (!MachineOn) return;
            _context.Post(stateInfo =>
            {
                if (_obliviousTmState == ObliviousTmState.LoadHeadValuesToRegister)
                {
                    if (Register.ValuesAllFilled()) _obliviousTmState = ObliviousTmState.ResetAfterValuesLoaded;
                    else if (_multiTape.PhysicalHeadIsOnVirtualHead())
                    {
                        string owningTapeId = _multiTape.GetCurrentTapeName();
                        char virtualHeadValue = _multiTape.ReadHeadValue();
                        Register.EnterValue(owningTapeId, virtualHeadValue);
                    }
                }

                else if (_obliviousTmState == ObliviousTmState.ExecuteRightMovesAndValueWrites)
                {
                    if (_multiTape.PhysicalHeadIsOnVirtualHead())
                    {
                        string currentTapeName = _multiTape.GetCurrentTapeName();
                        RegisterItemViewModel registerItem = Register.GetRegisterItemByTapeName(currentTapeName);

                        _multiTape.WriteHeadValue(registerItem.Value);

                        if (registerItem.MoveSymbol == 'R')
                        {
                            _tapesToMoveRight.Add(currentTapeName);
                            _multiTape.ToggleVirtualHead();
                        }
                    }
                    else if (_tapesToMoveRight.Count() > 0 && _multiTape.GetCurrentTapeName() == _tapesToMoveRight.First())
                    {
                        _multiTape.ToggleVirtualHead();
                        _tapesToMoveRight.RemoveAt(0);
                    }
                }

                else if (_obliviousTmState == ObliviousTmState.ExecuteLeftMoves)
                {
                    if (_multiTape.PhysicalHeadIsOnVirtualHead())
                    {
                        string currentTapeName = _multiTape.GetCurrentTapeName();
                        RegisterItemViewModel registerItem = Register.GetRegisterItemByTapeName(currentTapeName);

                        if (registerItem.MoveSymbol == 'L')
                        {
                            _tapesToMoveLeft.Add(currentTapeName);
                            _multiTape.ToggleVirtualHead();
                        }
                    }
                    else if (_tapesToMoveLeft.Count() > 0 && _multiTape.GetCurrentTapeName() == _tapesToMoveLeft.First())
                    {
                        _multiTape.ToggleVirtualHead();
                        _tapesToMoveLeft.RemoveAt(0);
                    }
                }

                if (_movingLeft == false) //Moving right
                {
                    int maxIndex = CalculateMaxIndex();
                    bool reachedEnd = _multiTape.MoveHeadRight(maxIndex);
                    _movingLeft = reachedEnd;
                    if (_movingLeft && _obliviousTmState == ObliviousTmState.ExecuteRightMovesAndValueWrites)
                    {
                        if (_tapesToMoveRight.Count() > 0 && _multiTape.GetCurrentTapeName() == _tapesToMoveRight.First())
                        {
                            _multiTape.ToggleVirtualHead();
                            _tapesToMoveRight.RemoveAt(0);
                        }
                        _obliviousTmState = ObliviousTmState.ExecuteLeftMoves;
                    }
                }
                else
                {
                    bool reachedStart = _multiTape.MoveHeadLeft();
                    if (reachedStart)
                    {
                        _movingLeft = false; //moving right
                        if (_obliviousTmState == ObliviousTmState.ResetAfterValuesLoaded) //Find transition function
                        {
                            char[] tapeValues = ReadTapeValues();
                            TransitionFunctionViewModel tf = this.GetCurrentTransitionFunction(tapeValues);
                            if (tf == null)
                            {
                                PromptTransitionFunctionDesigner(tapeValues);
                                return;
                            }
                            CurrentTransitionFunction = tf;

                            var allWrites = tf.OutputHeadWriteArgs.Select(x => x.ReadWriteValue).ToList();
                            allWrites.RemoveAt(0);
                            var multiTapeWrites = allWrites.ToArray();
                            Register.FillAllValues(multiTapeWrites);

                            var allMovements = tf.OutputHeadMoveArgs.Select(x => x.SelectedMoveSymbol).ToList();
                            var inputTapeMovement = allMovements[0];
                            allMovements.RemoveAt(0);
                            char[] multiTapeMovements = allMovements.Select(x => x.Symbol).ToArray();
                            Register.FillVirtualHeadMovements(multiTapeMovements);
                            _inputTape.MoveHead(inputTapeMovement);

                            _obliviousTmState = ObliviousTmState.ExecuteRightMovesAndValueWrites;
                            _cycleTracker++;
                            OnPropertyChanged(nameof(MaxIndex));
                        }
                        else if (_obliviousTmState == ObliviousTmState.ExecuteLeftMoves)
                        {
                            if (_tapesToMoveLeft.Count() > 0 && _multiTape.GetCurrentTapeName() == _tapesToMoveLeft.First())
                            {
                                _multiTape.ToggleVirtualHead();
                                _tapesToMoveLeft.RemoveAt(0);
                            }
                            CurrentState = CurrentTransitionFunction.SelectedOutputState;
                            Register.ClearRegister();
                            CurrentTransitionFunction = null;
                            _obliviousTmState = ObliviousTmState.LoadHeadValuesToRegister;
                        }
                    }
                }

                StepCount++;
                if (CurrentState.Name == _haltState) OnToggleMachineOnOff();

            }, null);
        }
    }

    public enum ObliviousTmState
    {
        LoadHeadValuesToRegister,
        ResetAfterValuesLoaded,
        ExecuteRightMovesAndValueWrites,
        ExecuteLeftMoves
    }
}