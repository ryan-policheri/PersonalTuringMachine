using PersonalTuringMachine.ViewModel;
using System.Windows;

namespace PersonalTuringMachine
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            PtmViewModel ptmViewModel = new PtmViewModel();
            MainViewModel mainViewModel = new MainViewModel(ptmViewModel);
            this.DataContext = mainViewModel;
        }
    }
}
