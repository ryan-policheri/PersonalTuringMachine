using System.Collections.ObjectModel;

namespace PersonalTuringMachine.ViewModel
{
    public class PtmViewModel
    {
        public PtmViewModel()
        {
            Tapes = new ObservableCollection<TapeViewModel>();

            Tapes.Add(new TapeViewModel("Tape 1 (Input) (Read-Only)"));
            Tapes.Add(new TapeViewModel("Tape 2 (Working) (Read-Write)"));
            Tapes.Add(new TapeViewModel("Tape 3 (Output) (Read-Write)"));
        }

        public ObservableCollection<TapeViewModel> Tapes { get; }
    }
}
