using System;
using System.Windows;

namespace PersonalTuringMachine
{
    public partial class ModalWindow : Window
    {
        private readonly Action<ExitCode> _closeCallback;

        public ModalWindow(Action<ExitCode> closeCallback)
        {
            InitializeComponent();
            _closeCallback = closeCallback;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            _closeCallback(ExitCode.Canceled);
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            _closeCallback(ExitCode.Saved);
            this.Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            _closeCallback(ExitCode.Canceled);
            this.Close();
        }
    }
}
