using PersonalTuringMachine.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace PersonalTuringMachine.ViewModel
{
    public class TapeViewModel : ViewModelBase
    {
        private const int _emptyCellBuffer = 50;
        private readonly char[] _alphabet;

        public TapeViewModel(int number, TapeType type, char[] alphabet, char emptySymbol, IEnumerable<CellViewModel> initialCells)
        {
            Number = number;
            Type = type;

            _alphabet = alphabet;
            EmptySymbol = emptySymbol;

            Cells = new ObservableCollection<CellViewModel>();
            if (initialCells != null) { foreach (CellViewModel cell in initialCells) { AddCell(cell.Value); } }
            MaintainEmptyCellBuffer();
        }

        public int Number { get; }

        public TapeType Type { get; }

        public string TypeDescription => Enum.GetName(typeof(TapeType), Type);

        public string ShortName => $"T{Number}";

        public string LongName => $"Tape {Number} ({TypeDescription})";

        public char EmptySymbol { get; }

        public ObservableCollection<CellViewModel> Cells { get; }

        private void AddCell(char value)
        {
            CellViewModel cell = new CellViewModel(_alphabet, value);
            Cells.Add(cell);
            cell.PropertyChanged += Cell_PropertyChanged;
        }

        private void Cell_PropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if(args.PropertyName == nameof(CellViewModel.Value))
            {
                MaintainEmptyCellBuffer();
            }
        }

        private void MaintainEmptyCellBuffer() //Create a buffer of 50 empty cells at the end to keep the appearance of infinity
        {
            for (int i = Cells.Count - 1; i >= 0; i--) //Find last non-empty value
            {
                if (Cells[i].Value != EmptySymbol)
                {
                    int desiredLength = i + _emptyCellBuffer;
                    int cellsToAdd = desiredLength - Cells.Count;
                    for (int j = 0; j <= cellsToAdd; j++)
                    {
                        AddCell(EmptySymbol);
                    }
                    break;
                }
            }
        }
    }
}
