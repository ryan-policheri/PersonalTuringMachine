using Microsoft.Win32;
using PersonalTuringMachine.CommandBinding;
using PersonalTuringMachine.Extensions;
using PersonalTuringMachine.Model;
using PersonalTuringMachine.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
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

            Directory.CreateDirectory(_machinesFolder);
            Directory.CreateDirectory(_inputsFolder);
        }

        public PtmViewModel Ptm { get; private set; }

        public ICommand SaveMachine { get; }

        public ICommand SaveMachineAs { get; }

        public ICommand SaveInput { get; }

        public ICommand SaveInputAs { get; }

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
                File.WriteAllText(_currentInputFile, spec.ToJson());
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
            char[] input = Ptm.GetInput();
        }

        private void OnSaveInputAs()
        {
            throw new NotImplementedException();
        }

        private string SaveJsonFileDialog(string folder, string json)
        {
            SaveFileDialog fileDialog = new SaveFileDialog();
            fileDialog.InitialDirectory = folder;
            fileDialog.Filter = "Json files (*.json)|*.json|Text files (*.txt)|*.txt"; ;
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
    }
}
