using PersonalTuringMachine.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonalTuringMachine.Model
{
    public class MachineSpec
    {
        public char[] Alphabet { get; set; }

        public char EmptySymbol { get; set; }

        public char StartSymbol { get; set; }

        public ICollection<TapeSaveModel> Tapes { get; set; }

        public ICollection<StateSaveModel> States { get; set; }

        public ICollection<TransitionFunctionSaveModel> TransitionFunctions { get; set; }
    }
}
