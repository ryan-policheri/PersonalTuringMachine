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
            IEnumerable<TapeViewModel> tapes = new List<TapeViewModel> { 
                new TapeViewModel(1, TapeType.ReadOnly, alphabet, alphabet[3], new List<CellViewModel>{ new CellViewModel(alphabet, alphabet[0]) }),
                new TapeViewModel(2, TapeType.ReadWrite, alphabet, alphabet[3], new List<CellViewModel>{ new CellViewModel(alphabet, alphabet[0]) }), 
                new TapeViewModel(3, TapeType.ReadWrite, alphabet, alphabet[3], new List<CellViewModel>{ new CellViewModel(alphabet, alphabet[0]) })
            };

            IEnumerable<StateViewModel> states = new List<StateViewModel> {
                new StateViewModel("Qstart", false), 
                new StateViewModel("Qhalt", false)
            };

            PtmViewModel ptmViewModel = new PtmViewModel(alphabet, alphabet[0], alphabet[3], tapes, states, null);

            MainViewModel mainViewModel = new MainViewModel(ptmViewModel);
            this.DataContext = mainViewModel;
        }
    }
}
