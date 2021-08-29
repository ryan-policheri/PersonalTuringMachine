using PersonalTuringMachine.ViewModel;

namespace PersonalTuringMachine
{
    public class MainViewModel
    {
        public MainViewModel(PtmViewModel ptmViewModel)
        {
            Ptm = ptmViewModel;
        }

        public PtmViewModel Ptm { get; }
    }
}
