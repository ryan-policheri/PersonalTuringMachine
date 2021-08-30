using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PersonalTuringMachine.ViewModel
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propName = null)
        {
            PropertyChangedEventArgs args = new PropertyChangedEventArgs(propName);
            PropertyChanged?.Invoke(this, args);
        }

        protected void SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (!EqualityComparer<T>.Default.Equals(field, value))
            {
                field = value;
                OnPropertyChanged(propertyName);
            }
        }

        protected void OpenInModal(ViewModelBase viewModel, Action<ExitCode> closeCallback)
        {
            ModalWindow modalWindow = new ModalWindow();
            modalWindow.DataContext = viewModel;
            modalWindow.Closed += (sender, args) =>
            {
                closeCallback(ExitCode.Saved);
            };
            modalWindow.ShowDialog();
        }
    }
}
