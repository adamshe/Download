using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace TreeViewWithCheckBoxes
{
    /// <summary>
    /// Interaction logic for TreeViewCheckBoxList.xaml
    /// </summary>
    public partial class TreeViewCheckBoxListView : UserControl, INotifyPropertyChanged
    {
        #region Search
        
        private readonly ICommand _storeInPreviousCommand;
        private string selectedCriteria = String.Empty;
        private string currentCriteria = String.Empty;
        private string _groupMember;
        private readonly ObservableCollection<string> _previousCriteria = new ObservableCollection<string>();
        #endregion

        ObservableCollection<EntityViewModel<object>> _root;
        Func<object, string> _filter;
        IEnumerable _portfolioList;
        public static readonly DependencyProperty ItemsSourceProperty =
        DependencyProperty.Register("ItemsSource", typeof(IEnumerable),
        typeof(TreeViewCheckBoxListView), new FrameworkPropertyMetadata(null));

        public IEnumerable ItemsSource
        {
            get { return (IEnumerable) GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public TreeViewCheckBoxListView(IEnumerable  portlist)
        {
            _portfolioList = portlist;
            _filter = port => { var propVal = port.GetType().GetProperty(_groupMember).GetValue(port, null).ToString(); 
                return propVal.Substring(0, propVal.IndexOf(' ')); };
            this.Loaded += TreeViewCheckBoxListView_Loaded;
            _storeInPreviousCommand = new CustomCommand(StoreInPrevious);
            InitializeComponent();

        }

        private void StoreInPrevious(object dummy)
        {
            if (String.IsNullOrEmpty(CurrentCriteria))
                return;

            if (!_previousCriteria.Contains(CurrentCriteria))
                _previousCriteria.Add(CurrentCriteria);

            SelectedCriteria = CurrentCriteria;
        }

        public string SelectedCriteria
        {
            get { return selectedCriteria; }
            set
            {
                if (value == selectedCriteria)
                    return;

                selectedCriteria = value;
                OnPropertyChanged("SelectedCriteria");
            }
        }

        public string GroupMember { get { return _groupMember; } set { _groupMember = value; } }


        public IEnumerable<string> PreviousCriteria
        {
            get { return _previousCriteria; }
        }

        public string CurrentCriteria
        {
            get { return currentCriteria; }
            set
            {
                if (value == currentCriteria)
                    return;

                currentCriteria = value;
                OnPropertyChanged("CurrentCriteria");
                ApplyFilter();
            }
        }

        private void ApplyFilter()
        {
            foreach (var node in _root)
                node.ApplyCriteria(CurrentCriteria, new Stack<EntityViewModel<object>>());
        }

        void TreeViewCheckBoxListView_Loaded(object sender, RoutedEventArgs e)
        {
           // WireUpToggleButton();
        }

        public IEnumerable PortfolioList
        {
            get { return _portfolioList; }
            set
            {
                _portfolioList = value as IEnumerable;
                _root = GetList;
                tree.GetBindingExpression(TreeView.ItemsSourceProperty).UpdateTarget();
                CommandManager.InvalidateRequerySuggested();
            }
        }

        public ICommand StoreInPreviousCommand
        {
            get { return _storeInPreviousCommand; }
        }

        public IEnumerable<EntityViewModel<object>> Root
        {
            get { return _root; }
        }

        public ObservableCollection<EntityViewModel<object>> GetList
        {
            get
            {
                if (PortfolioList == null)
                    return null;
                IEnumerable<IGrouping<string, object>> query = PortfolioList.OfType<object>().GroupBy(_filter);
                var root = new EntityViewModel<object>("Portfolios") { IsInitiallySelected = true, IsExpanded = true, Level = 1 };
                foreach (var group in query)
                {
                    var key = group.Key;
                    var portGroup = new EntityViewModel<object>(key) { IsExpanded = false, Level = 2 };
                    root.Children.Add(portGroup);
                    foreach (var portName in group)
                    {
                        var port = new EntityViewModel<object>(portName) { Level = 3 };
                        portGroup.Children.Add(port);
                    }
                }

                root.Initialize();
                _root = new ObservableCollection<EntityViewModel<object>> { root };
                return _root;
            }           
        }

        private IEnumerable<object> SelectedPortfolios
        {
            get
            {
                var list= _root.Where ( box => (box.IsChecked.HasValue && box.IsChecked.Value == true));
                return list.Select(box => box.Entity);
            }           
        }

        private void WireUpToggleButton ()
        {
            var root = this.tree.Items[0] as EntityViewModel<IPortfolioInfo>;

            base.CommandBindings.Add(
                new CommandBinding(
                    ApplicationCommands.Undo,
                    (sender, e) => // Execute
                    {
                        e.Handled = true;
                        if (toggleButton.Content.ToString() == "Select All")
                        {
                            root.IsChecked = true;
                            toggleButton.Content = "Uncheck All";

                        }
                        else
                        {
                            root.IsChecked = false;
                            toggleButton.Content = "Select All";
                        }

                        this.tree.Focus();
                    },
                    (sender, e) => // CanExecute
                    {
                        e.Handled = true;
                        e.CanExecute = true;
                    }));

            this.tree.Focus();
        }

        public List<EntityViewModel<IPortfolioInfo>> DataSource { get; set; }

        private void tree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            e.Handled = true;
            IInputElement element = sender as IInputElement;
            if (e.OldValue != null && e.OldValue != e.NewValue)
            {
                var source = ((TreeView)e.Source).SelectedItem as EntityViewModel<IPortfolioInfo>;
                if ((Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
                {
                    if (Keyboard.IsKeyDown(Key.Up) || Keyboard.IsKeyDown(Key.Down))
                    {
                        source.IsChecked = !source.IsChecked;
                    }
                }
            }
        }       

        
        private void TreeViewItemClick(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            if (!(e.OriginalSource is TextBlock))            
                return;

            if (e.Source is ContentPresenter)
            {
                var source = ((ContentPresenter)e.Source).Content as EntityViewModel<IPortfolioInfo>;
                if (source != null)
                {                    

                    if (source.IsChecked == null)
                        source.IsChecked = false;
                    else
                        source.IsChecked = !source.IsChecked;                   
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }
            //public bool? ToggleState (bool? state)
            //{
            //    if (state == null)
            //        return false;
            //    if (state.HasValue)
            //        return !state.Value;
            //    return null;
            //}
    }
}
