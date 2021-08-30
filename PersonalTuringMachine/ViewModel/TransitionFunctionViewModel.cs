using PersonalTuringMachine.Extensions;
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
        }

        public ObservableCollection<char> Alphabet { get; }

        public ObservableCollection<TapeViewModel> Tapes { get; }

        public ObservableCollection<StateViewModel> States { get; }

        private StateViewModel _selectedState;
        public StateViewModel SelectedState 
        {
            get { return _selectedState; }
            set { SetField(ref _selectedState, value); OnPropertyChanged(nameof(InputDisplay)); }
        }

        public string[] Input { get; set; }

        public string InputDisplay
        {
            get 
            {
                if (Input == null) return SelectedState?.Name;
                else return SelectedState?.Name + "(" + Input.ToDelimitedList<string>(", ") + ")"; 
            }
        }

        public string[] Output { get; set; }

        public string OutputDisplay { get; set; }
    }
}
