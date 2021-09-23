using System.Collections.ObjectModel;
using System.Linq;

namespace PersonalTuringMachine.ViewModel
{
    public class RegisterViewModel : ViewModelBase
    {
        private const char _nullSymbol = ' '; //Some symbol other than the empty symbol that signifies a tape value has not been placed in the register yet

        public RegisterViewModel(string[] tapeNames)
        {
            RegisterItems = new ObservableCollection<RegisterItemViewModel>();

            foreach (string tapeName in tapeNames)
            {
                RegisterItems.Add(new RegisterItemViewModel(tapeName)
                {
                    Value = _nullSymbol,
                    MoveSymbol = _nullSymbol
                });
            }
        }

        public ObservableCollection<RegisterItemViewModel> RegisterItems { get; }

        public void EnterValue(string tapeName, char value)
        {
            RegisterItems.Where(x => x.TapeName == tapeName).First().Value = value;
        }

        public bool ValuesAllFilled()
        {
            return RegisterItems.All(x => x.Value != _nullSymbol);
        }

        public char[] ReadRegisterValues()
        {
            return RegisterItems.Select(x => x.Value).ToArray();
        }

        public void FillVirtualHeadMovements(char[] movementCharsInOrder)
        {
            for (int i = 0; i < movementCharsInOrder.Length; i++)
            {
                RegisterItems[i].MoveSymbol = movementCharsInOrder[i];
            }
        }

        public void FillAllValues(char[] charsToWriteInOrder)
        {
            for (int i = 0; i < charsToWriteInOrder.Length; i++)
            {
                RegisterItems[i].Value = charsToWriteInOrder[i];
            }
        }

        public RegisterItemViewModel GetRegisterItemByTapeName(string tapeName)
        {
            return RegisterItems.Where(x => x.TapeName == tapeName).First();
        }

        public void ClearRegister()
        {
            foreach (RegisterItemViewModel item in RegisterItems)
            {
                item.Value = _nullSymbol;
                item.MoveSymbol = _nullSymbol;
            }
        }
    }

    public class RegisterItemViewModel : ViewModelBase
    {
        public RegisterItemViewModel(string tapeName)
        {
            TapeName = tapeName;
        }
        public string TapeName { get; }

        private char _value;
        public char Value
        {
            get { return _value; }
            set { SetField(ref _value, value); }
        }

        private char _moveSymbol;
        public char MoveSymbol
        {
            get { return _moveSymbol; }
            set { SetField(ref _moveSymbol, value); }
        }

    }
}
