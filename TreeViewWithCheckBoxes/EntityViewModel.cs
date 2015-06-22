﻿using System.Collections.Generic;
using System.ComponentModel;

namespace TreeViewWithCheckBoxes
{
    public class EntityViewModel<T> : INotifyPropertyChanged
    {
        #region Data

        bool? _isChecked = false;
        EntityViewModel<T> _parent;
        T _entity;
       // List<EntityViewModel<T>> _list;
        
        #endregion // Data

        #region CreateFoos

        public static List<EntityViewModel<T>> CreateFoos()
        {
            EntityViewModel<T> root = new EntityViewModel<T>("Weapons")
            {
                IsInitiallySelected = true,
                Children =
                {
                    new EntityViewModel<T>("Blades")
                    {
                        Children =
                        {
                            new EntityViewModel<T>("Dagger"),
                            new EntityViewModel<T>("Machete"),
                            new EntityViewModel<T>("Sword"),
                        }
                    },
                    new EntityViewModel<T>("Vehicles")
                    {
                        Children =
                        {
                            new EntityViewModel<T>("Apache Helicopter"),
                            new EntityViewModel<T>("Submarine"),
                            new EntityViewModel<T>("Tank"),                            
                        }
                    },
                    new EntityViewModel<T>("Guns")
                    {
                        Children =
                        {
                            new EntityViewModel<T>("AK 47"),
                            new EntityViewModel<T>("Beretta"),
                            new EntityViewModel<T>("Uzi"),
                        }
                    },
                }
            };

            root.Initialize();
            var list = new List<EntityViewModel<T>> { root };
            return list;
        }

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
        private string p;

        #endregion
    }
}