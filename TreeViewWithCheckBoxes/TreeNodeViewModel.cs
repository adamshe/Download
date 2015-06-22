using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using TreeViewWithCheckBoxes;

namespace Bornander.UI.ViewModels
{
    public class TreeNodeViewModel<T> : INotifyPropertyChanged where T: IPortfolioInfo
    {
        private readonly ObservableCollection<TreeNodeViewModel<T>> _children;
        private readonly string _name;

        private bool _expanded;
        private bool _match = true;
        bool? _isChecked = false;
        TreeNodeViewModel<T> _parent;
        public TreeNodeViewModel(string name, IEnumerable<TreeNodeViewModel<T>> children)
        {
            this._name = name;
            this._children = new ObservableCollection<TreeNodeViewModel<T>>(children);
        }

        public TreeNodeViewModel(string name)
            : this(name, Enumerable.Empty<TreeNodeViewModel<T>>())
        {
        }

        void Initialize()
        {
            foreach (TreeNodeViewModel<T> child in this.Children)
            {
                child._parent = this;
                child.Initialize();
            }
        }

        public override string ToString()
        {
            return _name;
        }

        private bool IsCriteriaMatched(string criteria)
        {
            return String.IsNullOrEmpty(criteria) || _name.Contains(criteria);
        }

        public void ApplyCriteria(string criteria, Stack<TreeNodeViewModel<T>> ancestors)
        {
            if (IsCriteriaMatched(criteria))
            {
                IsMatch = true;
                foreach (var ancestor in ancestors)
                {
                    ancestor.IsMatch = true;
                    ancestor.IsExpanded = !String.IsNullOrEmpty(criteria);
                }
            }
            else
                IsMatch = false;

            ancestors.Push(this);
            foreach (var child in Children)
                child.ApplyCriteria(criteria, ancestors);

            ancestors.Pop();
        }

        public IEnumerable<TreeNodeViewModel<T>> Children
        {
            get { return _children; }
        }

        public string Name
        {
            get { return _name; }
        }

        public bool IsExpanded
        {
            get { return _expanded; }
            set
            {
                if (value == _expanded)
                    return;

                _expanded = value;
                if (_expanded)
                {
                    foreach (var child in Children)
                        child.IsMatch = true;
                }
                OnPropertyChanged("IsExpanded");
            }
        }

        public bool IsMatch
        {
            get { return _match; }
            set
            {
                if (value == _match)
                    return;

                _match = value;
                OnPropertyChanged("IsMatch");
            }
        }

        public bool IsLeaf
        {
            get { return !Children.Any(); }
        }

        #region IsChecked

        /// <summary>
        /// Gets/sets the state of the associated UI toggle (ex. CheckBox).
        /// The return value is calculated based on the check state of all
        /// child FooViewModels.  Setting this property to true or false
        /// will set all children to the same check state, and setting it 
        /// to any value will cause the parent to verify its check state.
        /// </summary>
        public bool? IsChecked
        {
            get { return _isChecked; }
            set { this.SetIsChecked(value, true, true); }
        }

        void SetIsChecked(bool? value, bool updateChildren, bool updateParent)
        {
            if (value == _isChecked)
                return;

            _isChecked = value;

            if (updateChildren && _isChecked.HasValue)
                this.Children.ToList().ForEach(c => c.SetIsChecked(_isChecked, true, false));

            if (updateParent && _parent != null)
                _parent.VerifyCheckState();

            this.OnPropertyChanged("IsChecked");
        }

        void VerifyCheckState()
        {
            bool? state = null;
            var list = this.Children.ToList();
            for (int i = 0; i < list.Count; ++i)
            {
                bool? current = list[i].IsChecked;
                if (i == 0)
                {
                    state = current;
                }
                else if (state != current)
                {
                    state = null;
                    break;
                }
            }
            this.SetIsChecked(state, false, true);
        }

        #endregion // IsChecked

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
