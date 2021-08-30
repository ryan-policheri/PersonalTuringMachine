namespace PersonalTuringMachine.ViewModel
{
    public class StateViewModel : ViewModelBase
    {
        public StateViewModel(string name, bool canEdit)
        {
            Name = name;
            IsReadOnly = !canEdit;
        }

        private string _name;
        public string Name 
        {
            get { return _name; }
            set { SetField(ref _name, value); }
        }

        private bool _isReadOnly;
        public bool IsReadOnly 
        { 
            get { return _isReadOnly; }
            set { SetField(ref _isReadOnly, value); }
        }
    }
}
