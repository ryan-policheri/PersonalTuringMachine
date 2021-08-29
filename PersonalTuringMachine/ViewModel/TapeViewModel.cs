using System.Collections.ObjectModel;

namespace PersonalTuringMachine.ViewModel
{
    public class TapeViewModel : ViewModelBase
    {
        public TapeViewModel(string tapeName)
        {
            TapeName = tapeName;
            Cells = new ObservableCollection<CellViewModel>();
            AppendCells(50);
            
        }

        public string TapeName { get; }

        public ObservableCollection<CellViewModel> Cells { get; }

        private void AppendCells(int appendCount)
        {
            for(int i = 0; i < appendCount; i++)
            {
                CellViewModel cell = new CellViewModel();
                cell.Value = 'U';
                Cells.Add(cell);
            }
        }
    }
}
