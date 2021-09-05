using PersonalTuringMachine.Extensions;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace PersonalTuringMachine.ViewModel
{
    public class HeadMoveCommandViewModel : ViewModelBase
    {
        public HeadMoveCommandViewModel(TapeViewModel tape)
        {
            TapeName = tape.ShortName;
            ToolTip = "Direction to Move on " + tape.LongName;
            HeadMoveSymbols = HeadMoveSymbol.GenerateDefaultMoveSymbols().ToObservableCollection();
        }

        public string TapeName { get; }

        public string ToolTip { get; }

        public ObservableCollection<HeadMoveSymbol> HeadMoveSymbols { get; }

        public HeadMoveSymbol SelectedMoveSymbol { get; set; }
    }

    public class HeadMoveSymbol
    {
        public char Symbol { get; set; }

        public string Description { get; set; }

        public static IEnumerable<HeadMoveSymbol> GenerateDefaultMoveSymbols()
        {
            ICollection<HeadMoveSymbol> symbols = new List<HeadMoveSymbol>();
            symbols.Add(new HeadMoveSymbol { Symbol = 'R', Description = "Move Right" });
            symbols.Add(new HeadMoveSymbol { Symbol = 'L', Description = "Move Left" });
            symbols.Add(new HeadMoveSymbol { Symbol = 'S', Description = "Stay Put" });
            return symbols;
        }
    }
}
