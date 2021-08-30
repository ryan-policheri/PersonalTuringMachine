namespace PersonalTuringMachine.ViewModel
{
    public class StateViewModel : ViewModelBase
    {
        public StateViewModel(string name)
        {
            Name = name;
        }

        private string _name;
        public string Name 
        {
            get { return _name; }
            set { SetField(ref _name, value); }
        }
    }
}
