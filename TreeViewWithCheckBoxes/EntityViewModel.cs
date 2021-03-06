﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
namespace TreeViewWithCheckBoxes
{
    public class EntityViewModel<T> : INotifyPropertyChanged
    {
        #region Data

        bool? _isChecked = false;
        bool _expanded;
        bool _isMatched = true;
        int _level;
        EntityViewModel<T> _parent;
        T _entity;

        
        #endregion // Data

        #region CreateFoos

        public EntityViewModel(string name)
        {
            this.Name = name;
            this.Children = new List<EntityViewModel<T>>();
        }

        public EntityViewModel(T entity)
        {
            this.Name = ((IPortfolioInfo)entity).Name;
            this._entity = entity;
            this.Children = new List<EntityViewModel<T>>();
        }   

        public void Initialize()
        {
            foreach (var child in this.Children)
            {
                child._parent = this;
                child.Initialize();
            }
        }

        #endregion // CreateFoos

        #region Properties

        public List<EntityViewModel<T>> Children { get; private set; }

      //  public bool IsInitiallySelected { get; private set; }

        public bool IsInitiallySelected { get; set; }

        public string Name { get; private set; }

        public T Entity { get; private set; }

        public int Level { get; set; }

        public bool IsRoot { get { return _parent == null; } }

        public bool IsMatched
        {
            get { return _isMatched; }
            set
            {
                if (value == _isMatched)
                    return;

                _isMatched = value;
                OnPropertyChanged("IsMatched");
            }
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

        public bool IsExpanded
        {
            get { return _expanded; }
            set
            {
                if (value == _expanded)
                    return;

                _expanded = value;
                //if (_expanded)
                //{
                //    foreach (var child in Children)
                //        child.IsMatched = true;
                //}
                OnPropertyChanged("IsExpanded");
            }
        }

        private bool IsCriteriaMatched(string criteria)
        {
            string processedCriteria = criteria.Trim();
            if (String.IsNullOrEmpty(processedCriteria))
                return true;

            var listOfStrings = criteria.Split(new char[] { ';', ',', '-', '\\', '/' }, StringSplitOptions.RemoveEmptyEntries);

            return listOfStrings.Any(s => Name.IndexOf(s, StringComparison.OrdinalIgnoreCase) >= 0);//(name.Contains(criteria);
        }

        private bool VisRule(string criteria)
        {
            var result = !String.IsNullOrEmpty(criteria);
            if (Level == 1)
                result = true;
            return result;
        }

        public bool Visible
        {
            get { return IsMatched || Level == 1; }
        }

        public void ApplyCriteria(string criteria, Stack<EntityViewModel<T>> ancestors)
        {
            if (IsCriteriaMatched(criteria))
            {
                IsMatched = true;
                foreach (var ancestor in ancestors)
                {
                    ancestor.IsMatched = true;
                    ancestor.IsExpanded =  !String.IsNullOrEmpty(criteria);
                }
            }
            else
                IsMatched = false | Level == 1;

            ancestors.Push(this);
            foreach (var child in Children)
                child.ApplyCriteria(criteria, ancestors);

            ancestors.Pop();
        }

        void SetIsChecked(bool? value, bool updateChildren, bool updateParent)
        {
            if (value == _isChecked)
                return;

            _isChecked = value;

            if (updateChildren && _isChecked.HasValue)
                this.Children.ForEach(c => c.SetIsChecked(_isChecked, true, false));

            if (updateParent && _parent != null)
                _parent.VerifyCheckState();

            this.OnPropertyChanged("IsChecked");
        }

        void VerifyCheckState()
        {
            bool? state = null;
            for (int i = 0; i < this.Children.Count; ++i)
            {
                bool? current = this.Children[i].IsChecked;
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

        #endregion // Properties

        #region INotifyPropertyChanged Members

        void OnPropertyChanged(string prop)
        {
            var hanlder = PropertyChanged;
            if (hanlder != null)
                hanlder(this, new PropertyChangedEventArgs(prop));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}