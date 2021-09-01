using PersonalTuringMachine.Model;
using System;
using System.Collections.ObjectModel;

namespace PersonalTuringMachine.ViewModel
{
    public class TapeViewModel : ViewModelBase
    {
        public TapeViewModel(int number, TapeType type)
        {
            Number = number;
            Type = type;
            Cells = new ObservableCollection<CellViewModel>();
            AppendCells(50);
        }

        public int Number { get; }

        public TapeType Type { get; }

        public string TypeDescription => Enum.GetName(typeof(TapeType), Type);

        public string ShortName => $"T{Number}";

        public string LongName => $"Tape {Number} ({TypeDescription})";

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
