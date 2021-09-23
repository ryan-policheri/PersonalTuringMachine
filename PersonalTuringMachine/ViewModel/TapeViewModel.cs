using PersonalTuringMachine.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace PersonalTuringMachine.ViewModel
{
    public class TapeViewModel : ViewModelBase
    {
        private const int _emptyCellBuffer = 50;

        public TapeViewModel(int number, TapeType type, char[] alphabet, char emptySymbol, IEnumerable<CellViewModel> initialCells, bool inititialzeWithBuffer = true)
        {
            Number = number;
            Type = type;

            Alphabet = alphabet;
            EmptySymbol = emptySymbol;

            Cells = new ObservableCollection<CellViewModel>();
            if (initialCells != null && initialCells.Count() > 0) { foreach (CellViewModel cell in initialCells) { AddCell(cell.Value, cell.HasHead); } }
            if (inititialzeWithBuffer) MaintainEmptyCellBuffer();
        }

        public readonly char[] Alphabet;

        public int Number { get; }

        public TapeType Type { get; }

        public string TypeDescription => Enum.GetName(typeof(TapeType), Type);

        public string ShortName => $"T{Number}";

        public string LongName => $"Tape {Number} ({TypeDescription})";

        public char EmptySymbol { get; }

        public ObservableCollection<CellViewModel> Cells { get; }

        public int ActiveCellCount => GetLastValueIndex() + 1;

        public void Clear() => Cells.Clear();

        public virtual CellViewModel AddCell(char value, bool hasHead = false)
        {
            CellViewModel cell = new CellViewModel(Alphabet, value, hasHead);
            Cells.Add(cell);
            cell.PropertyChanged += Cell_PropertyChanged;
            OnPropertyChanged(nameof(ActiveCellCount));
            return cell;
        }

        public char ReadHeadValue()
        {
            return GetCellWithHead().Value;
        }

        public void WriteHeadValue(char value)
        {
            GetCellWithHead().Value = value;
        }

        public void MoveHead(HeadMoveSymbol moveSymbol)
        {
            if (moveSymbol.Symbol == 'R')
            {
                int index = GetIndexOfHeadCell();
                if (index < Cells.Count - 1)
                {
                    Cells[index + 1].HasHead = true;
                    Cells[index].HasHead = false;
                }
            }
            if (moveSymbol.Symbol == 'L')
            {
                int index = GetIndexOfHeadCell();
                if (index > 0)
                {
                    Cells[index - 1].HasHead = true;
                    Cells[index].HasHead = false;
                }
            }
        }

        public int GetLastValueIndex()
        {
            for (int i = Cells.Count - 1; i >= 0; i--) //Find last non-empty value
            {
                if (Cells[i].Value != EmptySymbol) return i;
            }

            return 0;
        }

        public void MoveHeadTo(int index)
        {
            GetCellWithHead().HasHead = false;
            Cells[0].HasHead = true;
        }

        protected void Cell_PropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == nameof(CellViewModel.Value))
            {
                MaintainEmptyCellBuffer();
                OnPropertyChanged(nameof(ActiveCellCount));
            }
        }

        protected CellViewModel GetCellWithHead()
        {
            foreach (CellViewModel cell in Cells) { if (cell.HasHead) return cell; }
            throw new InvalidOperationException("There should be a head on the tape");
        }

        public int GetIndexOfHeadCell()
        {
            for (int i = 0; i < Cells.Count; i++)
            {
                if (Cells[i].HasHead) return i;
            }
            throw new InvalidOperationException("There should be a head on the tape");
        }

        protected void MaintainEmptyCellBuffer() //Create a buffer of 50 empty cells at the end to keep the appearance of infinity
        {
            int i = GetLastValueIndex();
            int desiredLength = i + _emptyCellBuffer;
            int cellsToAdd = desiredLength - Cells.Count;
            for (int j = 0; j <= cellsToAdd; j++) AddCell(EmptySymbol);
        }
    }
}
