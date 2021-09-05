using PersonalTuringMachine.ViewModel;
using System.Windows.Controls;
using System.Windows.Input;

namespace PersonalTuringMachine.View
{
    public partial class PtmView : UserControl
    {
        public PtmView()
        {
            InitializeComponent();
        }

        private PtmViewModel ViewModel => (this.DataContext as PtmViewModel);

        private void TransitionFunction_MouseDoubleClick(object sender, MouseButtonEventArgs args)
        {
            TransitionFunctionViewModel funcViewModel = (sender as Button)?.DataContext as TransitionFunctionViewModel;
            if(funcViewModel != null && ViewModel != null) { ViewModel.EditTransitionFunction(funcViewModel); }
        }
    }
}
