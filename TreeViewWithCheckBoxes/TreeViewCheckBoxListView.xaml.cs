using System;
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
        public TreeViewCheckBoxListView(IEnumerable<IPortfolioInfo> portList)
        {
            this.Loaded += TreeViewCheckBoxListView_Loaded;
            InitializeComponent();
            DataContext = GetList(portList, port => port.Name.Substring(0, port.Name.IndexOf(' ')));
        }

        void TreeViewCheckBoxListView_Loaded(object sender, RoutedEventArgs e)
        {
            WireUpToggleButton();
        }

        private List<EntityViewModel<IPortfolioInfo>> GetList(IEnumerable<IPortfolioInfo> portList, Func<IPortfolioInfo, string> filter)
        {
            IEnumerable<IGrouping<string, IPortfolioInfo>> query = portList.GroupBy(filter);
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
