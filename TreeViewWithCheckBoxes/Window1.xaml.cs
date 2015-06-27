using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace TreeViewWithCheckBoxes
{
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();

            //if (!DesignerProperties.GetIsInDesignMode(this))
            //{
            //    this.checkboxList.DataSource = GetList(Portfolios);
            //}
            InitialControl();
        }

        public void InitialControl()
        {
            TreeViewCheckBoxListView view = new TreeViewCheckBoxListView(null);
            view.PortfolioList = Portfolios;
            gridPanel.Children.Add(view);
        }

        public IEnumerable<IPortfolioInfo> Portfolios
        {
            get
            {
                return new List<IPortfolioInfo>
                {
                    new PortfolioInfo {Name="Asia Adam Fund", Aum=100000000000, PortfolioId=1},
                    new PortfolioInfo {Name="Asia Stacy Fund", Aum=1000000000, PortfolioId=2},
                    new PortfolioInfo {Name="Asia Christine Fund", Aum=10000000, PortfolioId=3},
                    new PortfolioInfo {Name="ICPA Adam Fund", Aum=1000000000, PortfolioId=4},
                    new PortfolioInfo {Name="ICPA Stacy Fund", Aum=1000000000, PortfolioId=5},
                    new PortfolioInfo {Name="ICPA Christine Fund", Aum=1000000000, PortfolioId=6},
                    new PortfolioInfo {Name="EMA Adam Fund", Aum=1000000000, PortfolioId=7},
                    new PortfolioInfo {Name="EMA Stacy Fund", Aum=10000000, PortfolioId=8},
                    new PortfolioInfo {Name="EMA Christine Fund", Aum=10000000, PortfolioId=9}
                };
            }
        }

        
    }
}