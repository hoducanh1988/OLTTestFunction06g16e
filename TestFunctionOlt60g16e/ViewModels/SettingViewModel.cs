using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using TestFunctionOlt60g16e.Commands;
using TestFunctionOlt60g16e.Models;
using UtilityPack.IO;

namespace TestFunctionOlt60g16e.ViewModels {
    public class SettingViewModel {

        public SettingViewModel() {
            //load setting file
            if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "setting.xml") == false) _setting = new SettingModel();
            else _setting = XmlHelper<SettingModel>.FromXmlFile(AppDomain.CurrentDomain.BaseDirectory + "setting.xml");

            //binding list combobox
            List<string> listcom = new List<string>();
            for (int i = 1; i < 100; i++) listcom.Add($"COM{i}");
            _collectionComport = new CollectionView(listcom);

            List<string> listbaudrate = new List<string>() { "57600", "115200" };
            _collectionBaudRate = new CollectionView(listbaudrate);


            //set command
            SaveCommand = new SettingSaveCommand(this);
            OpenFirmwareFile = new SettingOpenFirmwareFileCommand(this);
            OpenConfigModeGE = new SettingOpenConfigGEFileCommand(this);
            OpenConfigModeXGE = new SettingOpenConfigXGEFileCommand(this);
            OpenTeleATTFile = new SettingOpenTeleATTFileCommand(this);
            OpenScriptFileTestGE = new SettingOpenScriptFileTestGECommand(this);
            OpenScriptFileTestXGE = new SettingOpenScriptFileTestXGECommand(this);
        }

        //binding setting name
        SettingModel _setting;
        public SettingModel Setting {
            get { return _setting; }
        }

        //list comport
        readonly CollectionView _collectionComport;
        public CollectionView CollectionComport {
            get { return _collectionComport; }
        }

        //list baud rate
        readonly CollectionView _collectionBaudRate;
        public CollectionView CollectionBaudRate {
            get { return _collectionBaudRate; }
        }


        //command
        public ICommand SaveCommand {
            get;
            private set;
        }
        public ICommand OpenFirmwareFile {
            get;
            private set;
        }
        public ICommand OpenConfigModeGE {
            get;
            private set;
        }
        public ICommand OpenConfigModeXGE {
            get;
            private set;
        }
        public ICommand OpenTeleATTFile {
            get;
            private set;
        }
        public ICommand OpenScriptFileTestGE {
            get;
            private set;
        }
        public ICommand OpenScriptFileTestXGE {
            get;
            private set;
        }

    }
}
