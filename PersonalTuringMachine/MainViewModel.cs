using Microsoft.Win32;
using PersonalTuringMachine.CommandBinding;
using PersonalTuringMachine.Extensions;
using PersonalTuringMachine.Model;
using PersonalTuringMachine.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Input;

namespace PersonalTuringMachine
{
    public class MainViewModel
    {
        private readonly string _appFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "PersonalTuringMachine");
        private string _machinesFolder => Path.Combine(_appFolderPath, "Machines");
        private string _inputsFolder => Path.Combine(_appFolderPath, "Inputs");


        private string _currentMachineFile;
        private string _currentInputFile;

        public MainViewModel()
        {
            SaveMachine = new DelegateCommand(OnSaveMachine);
            SaveMachineAs = new DelegateCommand(OnSaveMachineAs);
            SaveInput = new DelegateCommand(OnSaveInput);
            SaveInputAs = new DelegateCommand(OnSaveInputAs);
            LoadMachine = new DelegateCommand(OnLoadMachine);
            LoadInput = new DelegateCommand(OnLoadInput);

            Directory.CreateDirectory(_machinesFolder);
            Directory.CreateDirectory(_inputsFolder);
        }

        public PtmViewModel Ptm { get; private set; }

        public ICommand SaveMachine { get; }

        public ICommand SaveMachineAs { get; }

        public ICommand SaveInput { get; }

        public ICommand SaveInputAs { get; }

        public ICommand LoadMachine { get; }

        public ICommand LoadInput { get; }

        public void Load()
        {
            char[] alphabet = { '>', '1', '0', '□' };
            IEnumerable<TapeViewModel> tapes = new List<TapeViewModel> {
                new TapeViewModel(1, TapeType.ReadOnly, alphabet, alphabet[3], new List<CellViewModel>{ new CellViewModel(alphabet, alphabet[0]) }),
                new TapeViewModel(2, TapeType.ReadWrite, alphabet, alphabet[3], new List<CellViewModel>{ new CellViewModel(alphabet, alphabet[0]) }),
                new TapeViewModel(3, TapeType.ReadWrite, alphabet, alphabet[3], new List<CellViewModel>{ new CellViewModel(alphabet, alphabet[0]) })
            };

            IEnumerable<StateViewModel> states = new List<StateViewModel> {
                new StateViewModel("Qstart", false),
                new StateViewModel("Qhalt", false)
            };

            PtmViewModel ptmViewModel = new PtmViewModel(alphabet, alphabet[0], alphabet[3], tapes, states, null);
            Ptm = ptmViewModel;
        }

        private void OnSaveMachine()
        {
            if (!String.IsNullOrWhiteSpace(_currentInputFile))
            {
                MachineSpec spec = Ptm.GetMachineSpec();
                File.WriteAllText(_currentMachineFile, spec.ToJson());
            }
            else OnSaveMachineAs();
        }

        private void OnSaveMachineAs()
        {
            MachineSpec spec = Ptm.GetMachineSpec();
            _currentMachineFile = SaveJsonFileDialog(_machinesFolder, spec.ToJson());
        }

        private void OnSaveInput()
        {
            if (!String.IsNullOrWhiteSpace(_currentInputFile))
            {
                char[] input = Ptm.GetInput();
                File.WriteAllText(_currentInputFile, input.ToJson());
            }
            else OnSaveInputAs();
        }

        private void OnSaveInputAs()
        {
            char[] input = Ptm.GetInput();
            _currentInputFile = SaveJsonFileDialog(_inputsFolder, input.ToJson());
        }

        private string SaveJsonFileDialog(string folder, string json)
        {
            SaveFileDialog fileDialog = new SaveFileDialog();
            fileDialog.InitialDirectory = folder;
            fileDialog.Filter = "Json files (*.json)|*.json|Text files (*.txt)|*.txt";
            fileDialog.AddExtension = true;
            fileDialog.DefaultExt = ".json";
            fileDialog.ShowDialog();

            if (String.IsNullOrWhiteSpace(fileDialog.FileName)) return null;
            else
            {
                File.WriteAllText(fileDialog.FileName, json);
                return fileDialog.FileName;
            }
        }

        private void OnLoadMachine()
        {
            string file = GetFilePathDialog(_machinesFolder);
            if (!String.IsNullOrWhiteSpace(file))
            {
                string json = File.ReadAllText(file);
                MachineSpec spec = json.ConvertJsonToObject<MachineSpec>();

                ICollection<TapeViewModel> tapes = new List<TapeViewModel>();
                foreach (TapeSaveModel tapeSaveModel in spec.Tapes)
                {
                    tapes.Add(new TapeViewModel(tapeSaveModel.Number, tapeSaveModel.Type, spec.Alphabet, spec.EmptySymbol, null));
                }

                ICollection<StateViewModel> states = new List<StateViewModel>();
                foreach (StateSaveModel state in spec.States)
                {
                    states.Add(new StateViewModel(state.Name, !state.IsReadOnly));
                }

                ICollection<TransitionFunctionViewModel> transitionFunctions = new List<TransitionFunctionViewModel>();
                foreach (TransitionFunctionSaveModel func in spec.TransitionFunctions)
                {
                    TransitionFunctionViewModel transitionFunctionViewModel = new TransitionFunctionViewModel(spec.Alphabet, tapes.ToObservableCollection(), states.ToObservableCollection());
                    transitionFunctionViewModel.SelectedInputState = transitionFunctionViewModel.States.Where(x => x.Name == func.InputStateName).FirstOrDefault();

                    for (int i = 0; i < transitionFunctionViewModel.InputHeadReadArgs.Count; i++)
                    {
                        transitionFunctionViewModel.InputHeadReadArgs[i].ReadWriteValue = func.InputHeadReadArgs[i];
                    }
                }

                //PtmViewModel viewModel = new PtmViewModel(spec.Alphabet, spec.StartSymbol, spec.EmptySymbol, tapes, states)
            }
        }

        private void OnLoadInput()
        {
            string file = GetFilePathDialog(_inputsFolder);
            if (!String.IsNullOrWhiteSpace(file))
            {
                string json = File.ReadAllText(file);
                char[] input = json.ConvertJsonToObject<char[]>();
                Ptm.LoadInputOntoTape(input);
            }
        }

        private string GetFilePathDialog(string folder)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = folder;
            openFileDialog.Filter = "Json files (*.json)|*.json|Text files (*.txt)|*.txt";
            openFileDialog.AddExtension = true;
            openFileDialog.DefaultExt = ".json";
            openFileDialog.ShowDialog();
            return openFileDialog.FileName;
        }
    }
}
