using System;
using System.Windows.Input;

namespace TreeViewWithCheckBoxes
{
    public class CustomCommand : ICommand {
        public event EventHandler CanExecuteChanged;

        private bool canExecute;
        private readonly Action<object> execute;

        public CustomCommand(bool canExecute, Action<object> execute) {
            this.canExecute = canExecute;
            this.execute = execute;
        }

        public CustomCommand(Action<object> execute)
            : this(true, execute) {
        }

        private void OnCanExecuteChanged() {
            var handler = CanExecuteChanged;
            if (CanExecuteChanged != null)
                CanExecuteChanged(this, EventArgs.Empty);
        }

        public bool CanExecuteCommand {
            get { return canExecute; }
            set {
                if (value == canExecute)
                    return;

                canExecute = value;
                OnCanExecuteChanged();
            }
        }

        public bool CanExecute(object parameter) 
        {
            var key = parameter as KeyEventArgs;
            if (key.Key == Key.Enter)
              return canExecute;
            return false;
        }

        public void Execute(object parameter) {
            if (!CanExecute(parameter))
                throw new InvalidOperationException("Invalid command execution requested");

            execute(parameter);
        }
    }
}
