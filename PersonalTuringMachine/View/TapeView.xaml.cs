using PersonalTuringMachine.Extensions;
using System.Windows;
using System.Windows.Controls;

namespace PersonalTuringMachine.View
{
    public partial class TapeView : UserControl
    {
        private ListBox _cellList;
        private Window _currentWindow;

        public TapeView()
        {
            InitializeComponent();
            Loaded += TapeView_Loaded;
        }

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
    }
}
