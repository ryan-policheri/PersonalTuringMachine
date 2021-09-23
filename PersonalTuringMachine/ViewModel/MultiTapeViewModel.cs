using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using PersonalTuringMachine.Model;
using PersonalTuringMachine.View;

namespace PersonalTuringMachine.ViewModel
{
    public class MultiTapeViewModel : TapeViewModel
    {
        private readonly IEnumerable<TapeViewModel> _tapes;

        public MultiTapeViewModel(int number, TapeType type, char[] alphabet, char emptySymbol, IEnumerable<TapeViewModel> initialTapes)
            : base(number, type, alphabet, emptySymbol, null, false)
        {
            _tapes = initialTapes;
            TapeCount = initialTapes.Count();

            AddInititalCells(initialTapes);
            MaintainEmptyCellBuffer();
            Cells.First().HasHead = true;
        }

        public int TapeCount { get; }

        public new string LongName => $"Multiplexed Tape {Number} ({TypeDescription})";

        public override CellViewModel AddCell(char value, bool hasHead = false)
        {
            int nextCellIndex = Cells.Count;
            int owningTapeIndex;
            owningTapeIndex = nextCellIndex % TapeCount;
            TapeViewModel tape = _tapes.ElementAt(owningTapeIndex);

            CellViewModel cell = new CellViewModel(Alphabet, value, hasHead, tape);
            Cells.Add(cell);
            cell.PropertyChanged += Cell_PropertyChanged;
            OnPropertyChanged(nameof(ActiveCellCount));
            return cell;
        }

        public IEnumerable<TapeViewModel> GetSourceTapes() => _tapes;

        public bool PhysicalHeadIsOnVirtualHead()
        {
            return GetCellWithHead().HasVirtualHead;
        }

        public string GetCurrentTapeName()
        {
            return GetCellWithHead().OwningTapeShortName;
        }

        public bool MoveHeadRight(int maxIndex) //returns true if head is at max index, false otherwise
        {
            int headCellIndex = GetIndexOfHeadCell();
            if (headCellIndex == maxIndex) return true;
            if (headCellIndex + 1 == Cells.Count) { AddCell(EmptySymbol, false); }
            Cells[headCellIndex].HasHead = false;
            Cells[++headCellIndex].HasHead = true;
            return headCellIndex >= maxIndex;
        }

        public bool MoveHeadLeft() //returns true if head is at start, false otherwise
        {
            int headCellIndex = GetIndexOfHeadCell();
            if (headCellIndex == 0) return true;
            Cells[headCellIndex].HasHead = false;
            Cells[--headCellIndex].HasHead = true;
            return headCellIndex == 0;
        }

        public void ToggleVirtualHead()
        {
            GetCellWithHead().HasVirtualHead = !GetCellWithHead().HasVirtualHead;
        }

        private void AddInititalCells(IEnumerable<TapeViewModel> tapes)
        {
            Cells.Clear();
            int longestTape = tapes.OrderByDescending(t => t.ActiveCellCount).First().ActiveCellCount;

            for (int i = 0; i < longestTape; i++)
            {
                foreach (TapeViewModel tape in tapes)
                {
                    if (i < tape.ActiveCellCount)
                    {
                        CellViewModel cell = tape.Cells[i];
                        CellViewModel newCell = AddCell(cell.Value, false);
                        newCell.HasVirtualHead = cell.HasHead;
                    }
                    else { AddCell(EmptySymbol, false); }
                }
            }
        }
    }
}
