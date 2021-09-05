using PersonalTuringMachine.CommandBinding;
using PersonalTuringMachine.Extensions;
using PersonalTuringMachine.ViewModel;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PersonalTuringMachine.View
{
    public partial class TapeView : UserControl
    {
        private ListBox _cellList;
        private Window _currentWindow;

        private TapeViewModel _viewModel => this.DataContext as TapeViewModel;

        public TapeView()
        {
            InitializeComponent();
            Loaded += TapeView_Loaded;
        }

        public ICommand OnDelete
        {
            get { return (ICommand)GetValue(OnDeleteProperty); }
            set { SetValue(OnDeleteProperty, value); }
        }

        // Using a DependencyProperty as the backing store for OnDelete.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OnDeleteProperty =
            DependencyProperty.Register("OnDelete", typeof(ICommand), typeof(TapeView));

        private void TapeView_Loaded(object sender, RoutedEventArgs args)
        {
            _cellList = this.GetChildByName<ListBox>("CellList");
            _currentWindow = Window.GetWindow(this);
            _currentWindow.SizeChanged += (sender, args) => { ResizeTape(); };
            _currentWindow.StateChanged += (sender, args) => { ResizeTape(); };
            ResizeTape();
        }

        private void ResizeTape()
        {
            if(_cellList != null && _currentWindow != null)
            {
                _cellList.Width = _currentWindow.ActualWidth - 50;
            }
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            tb.SelectAll();
        }

        private void TextBox_GotMouseCapture(object sender, MouseEventArgs e)
        {
            TextBox tb = sender as TextBox;
            tb.SelectAll();
        }

        private void TextBox_IsMouseCaptureWithinChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            tb.SelectAll();
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs args)
        {
            if (args.Key == Key.Tab || args.Key == Key.Right || args.Key == Key.Left)
            {
                TextBox tb = sender as TextBox;
                CellViewModel viewModel = tb.DataContext as CellViewModel;
                if(viewModel != null)
                {
                    int index = _cellList.Items.IndexOf(viewModel);
                    if (index >= 0 && index < _cellList.Items.Count - 1 && args.Key != Key.Left)
                    {
                        CellViewModel targetViewModel = _cellList.Items.GetItemAt(index + 1) as CellViewModel;
                        if (targetViewModel != null)
                        {
                            IEnumerable<TextBox> textBoxes = _cellList.GetChildrenOfType<TextBox>();
                            TextBox targetTextBox = textBoxes.Where(x => (x.DataContext as CellViewModel) == targetViewModel).FirstOrDefault();
                            if (targetTextBox != null) targetTextBox.Focus();
                            args.Handled = true;
                        }
                    }
                    else if (index > 0 && index <= _cellList.Items.Count - 1 && args.Key == Key.Left)
                    {
                        CellViewModel targetViewModel = _cellList.Items.GetItemAt(index - 1) as CellViewModel;
                        if (targetViewModel != null)
                        {
                            IEnumerable<TextBox> textBoxes = _cellList.GetChildrenOfType<TextBox>();
                            TextBox targetTextBox = textBoxes.Where(x => (x.DataContext as CellViewModel) == targetViewModel).FirstOrDefault();
                            if (targetTextBox != null) targetTextBox.Focus();
                            args.Handled = true;
                        }
                    }
                }
            }
        }

        private void DeleteTape_Click(object sender, RoutedEventArgs args)
        {
            if(this._viewModel != null && OnDelete != null)
            {
                OnDelete.Execute(this._viewModel);
            }
        }
    }
}
