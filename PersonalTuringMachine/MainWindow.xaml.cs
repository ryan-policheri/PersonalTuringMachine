using PersonalTuringMachine.Model;
using PersonalTuringMachine.ViewModel;
using System.Collections.Generic;
using System.Windows;

namespace PersonalTuringMachine
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            char[] alphabet = { '>', '1', '0', '□' };
            IEnumerable<TapeViewModel> tapes = new List<TapeViewModel> { new TapeViewModel(1, TapeType.ReadOnly), new TapeViewModel(2, TapeType.ReadWrite), new TapeViewModel(3, TapeType.WriteOnly) };
            IEnumerable<StateViewModel> states = new List<StateViewModel> { new StateViewModel("Qstart", false), new StateViewModel("Qhalt", false) };

            PtmViewModel ptmViewModel = new PtmViewModel(alphabet, tapes, states);

            MainViewModel mainViewModel = new MainViewModel(ptmViewModel);
            this.DataContext = mainViewModel;
        }
    }
}
