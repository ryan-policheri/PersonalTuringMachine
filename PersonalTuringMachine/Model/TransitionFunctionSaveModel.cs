namespace PersonalTuringMachine.Model
{
    public class TransitionFunctionSaveModel
    {
        public string InputStateName { get; set; }

        public char[] InputHeadReadArgs { get; set; }

        public string OutputStateName { get; set; }

        public char[] OutputWriteArgs { get; set; }

        public char[] OutputMoveArgs { get; set; }
    }
}
