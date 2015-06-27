using System;
using System.Collections;
using System.Collections.Generic;
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
    public partial class TreeViewCheckBoxListView : UserControl
    {
        List<EntityViewModel<IPortfolioInfo>> _list;
        Func<IPortfolioInfo, string> _filter;
        IEnumerable<IPortfolioInfo> _portfolioList;
        public static readonly DependencyProperty ItemsSourceProperty =
        DependencyProperty.Register("ItemsSource", typeof(IEnumerable),
        typeof(TreeViewCheckBoxListView), new FrameworkPropertyMetadata(null));

        public IEnumerable ItemsSource
        {
            get { return (IEnumerable) GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public TreeViewCheckBoxListView(IEnumerable<IPortfolioInfo> portlist)
        {
            _portfolioList = portlist; 
            _filter = port => port.Name.Substring(0, port.Name.IndexOf(' '));
            this.Loaded += TreeViewCheckBoxListView_Loaded;
            InitializeComponent();
           
           
           // DataContext = GetList(portList, port => port.Name.Substring(0, port.Name.IndexOf(' ')));
        }

        void TreeViewCheckBoxListView_Loaded(object sender, RoutedEventArgs e)
        {
           // WireUpToggleButton();
        }

        public IEnumerable<IPortfolioInfo> PortfolioList
        {
            get { return _portfolioList; }
            set { _portfolioList = value;
                ItemsSource = GetList;
                tree.GetBindingExpression(TreeView.ItemsSourceProperty).UpdateTarget();
            }
        }

        public IEnumerable<EntityViewModel<IPortfolioInfo>> GetList
        {
            get
            {
                if (PortfolioList == null)
                    return null;
                IEnumerable<IGrouping<string, IPortfolioInfo>> query = PortfolioList.GroupBy(_filter);
                var root = new EntityViewModel<IPortfolioInfo>("Portfolios") { IsInitiallySelected = true };
                foreach (var group in query)
                {
                    var key = group.Key;
                    var portGroup = new EntityViewModel<IPortfolioInfo>(key);
                    root.Children.Add(portGroup);
                    foreach (var portName in group)
                    {
                        var port = new EntityViewModel<IPortfolioInfo>(portName);
                        portGroup.Children.Add(port);
                    }
                }

                root.Initialize();
                _list = new List<EntityViewModel<IPortfolioInfo>> { root };
                return _list;
            }           
        }

        private IEnumerable<IPortfolioInfo> SelectedPortfolios
        {
            get
            {
                var list= _list.Where ( box => (box.IsChecked.HasValue && box.IsChecked.Value == true));
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
            IInputElement element = sender as IInputElement;
            if (e.OldValue != null && e.OldValue != e.NewValue)
            {
                var source = ((TreeView)e.Source).SelectedItem as EntityViewModel<IPortfolioInfo>;
                if ((Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
                {
                   
                    source.IsChecked = !source.IsChecked;
                    return;
                }
                if (Keyboard.IsKeyDown(Key.Up) || Keyboard.IsKeyDown(Key.Down))
                {
                    return;
                }
                source.IsChecked = !source.IsChecked;
            }
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
