using Bornander.UI.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace TreeViewWithCheckBoxes
{
    public class GroupTreeNodeViewModel<T> : INotifyPropertyChanged where T: IPortfolioInfo
    {
        private readonly ICommand storeInPreviousCommand;

        private readonly ObservableCollection<TreeNodeViewModel<T>> roots = new ObservableCollection<TreeNodeViewModel<T>>();
        private readonly ObservableCollection<string> previousCriteria = new ObservableCollection<string>();
        private string selectedCriteria = String.Empty;
        private string currentCriteria = String.Empty;

        public GroupTreeNodeViewModel(IEnumerable<TreeNodeViewModel<T>> roots)
        {
            foreach (var node in roots)
                this.roots.Add(node);

            storeInPreviousCommand = new Command(StoreInPrevious);
        }

        private void StoreInPrevious(object dummy) {
            if (String.IsNullOrEmpty(CurrentCriteria))
                return;

            if (!previousCriteria.Contains(CurrentCriteria))
                previousCriteria.Add(CurrentCriteria);

            SelectedCriteria = CurrentCriteria;
        }

        private void ApplyFilter() {
            foreach (var node in roots)
                node.ApplyCriteria(CurrentCriteria, new Stack<TreeNodeViewModel<T>>());
        }

        public ICommand StoreInPreviousCommand {
            get { return storeInPreviousCommand; }
        }

        public IEnumerable<TreeNodeViewModel<T>> Roots {
            get { return roots; }
        }

        public IEnumerable<string> PreviousCriteria {
            get { return previousCriteria; }
        }

        public string SelectedCriteria {
            get { return selectedCriteria; }
            set {
                if (value == selectedCriteria)
                    return;

                selectedCriteria = value;
                OnPropertyChanged("SelectedCriteria");
            }
        }

        public string CurrentCriteria {
            get { return currentCriteria; }
            set {
                if (value == currentCriteria)
                    return;

                currentCriteria = value;
                OnPropertyChanged("CurrentCriteria");
                ApplyFilter();                
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
