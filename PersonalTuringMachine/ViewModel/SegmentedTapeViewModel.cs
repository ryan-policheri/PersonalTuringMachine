using PersonalTuringMachine.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonalTuringMachine.ViewModel
{
    public class SegmentedTapeViewModel : TapeViewModel
    {
        private int _segmentLength;
        private int _segmentIterator;
        private int _tapeCounterForColoring;

        public SegmentedTapeViewModel(int number, TapeType type, char[] alphabet, char emptySymbol, IEnumerable<CellViewModel> initialCells, bool inititialzeWithBuffer = true)
            : base(number, type, alphabet, emptySymbol, initialCells, inititialzeWithBuffer)
        {
            _segmentLength = 4;
            _segmentIterator = 0;
            _tapeCounterForColoring = 2;
            AddInititalCells(initialCells);
            MaintainEmptyCellBuffer();
        }

        private void AddInititalCells(IEnumerable<CellViewModel> cells)
        {
            Cells.Clear();
            foreach (CellViewModel cell in cells)
            {
                AddCell(cell.Value, cell.HasHead);
            }
        }

        public override CellViewModel AddCell(char value, bool hasHead = false)
        {
            TapeViewModel pseudoTapeForColoring = new TapeViewModel(_tapeCounterForColoring, TapeType.ReadWrite, Alphabet, EmptySymbol, null, false);
            CellViewModel cell = new CellViewModel(Alphabet, value, hasHead, pseudoTapeForColoring);
            Cells.Add(cell);
            cell.PropertyChanged += Cell_PropertyChanged;
            OnPropertyChanged(nameof(ActiveCellCount));

            if (_segmentIterator == _segmentLength - 1)
            {
                _segmentIterator = 0;
                _tapeCounterForColoring++;
            }
            else { _segmentIterator++; }
            if (_tapeCounterForColoring == 6) _tapeCounterForColoring = 2;
            return cell;
        }
    }
}
