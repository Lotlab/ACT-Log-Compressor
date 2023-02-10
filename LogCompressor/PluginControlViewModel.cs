using Lotlab.PluginCommon;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace LogCompressor
{
    class PluginControlViewModel : PropertyNotifier
    {
        ObservableCollection<LogItem> _logs { get; set; } = null;
        public ObservableCollection<LogItem> Logs { get => _logs; set { _logs = value; OnPropertyChanged(); } }

        ObservableCollection<LogFileItem> _files { get; set; } = null;
        public ObservableCollection<LogFileItem> Files { get => _files; set { _files = value; OnPropertyChanged(); } }

        public SimpleCommand CompressSelected { get; } = new SimpleCommand();

        public SimpleCommand DecompressSelected { get; } = new SimpleCommand();
        public SimpleCommand ApplyConfig { get; } = new SimpleCommand();

        Config _config = null;
        public Config Config { get => _config; set { _config = value; OnPropertyChanged(); } }

        public PluginControlViewModel()
        {

        }
    }

    class SimpleCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        public event Action<object> OnExecute;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            OnExecute?.Invoke(parameter);
        }
    }
}
