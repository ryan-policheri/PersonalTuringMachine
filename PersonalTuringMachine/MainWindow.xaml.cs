using System.Windows;

namespace PersonalTuringMachine
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            MainViewModel mainViewModel = new MainViewModel();
            this.DataContext = mainViewModel;
            mainViewModel.Load();
        }
    }
}
