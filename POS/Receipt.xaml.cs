using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace POS
{
    /// <summary>
    /// Interaction logic for Receipt.xaml
    /// </summary>
    public partial class Receipt : Window
    {
        private List<Item> items;
        private MainWindow main;

        private int amountPaid;
        private int change;
        private int cartTotal;
        //private int transID = 0;

        private string orderType;

        public Receipt(List<Item> _items, int _cartTotal, int _amountPaid, int _change, string _orderType, MainWindow _main)
        {
            InitializeComponent();

            main = _main;

            items = _items;
            cartTotal = _cartTotal;
            amountPaid = _amountPaid;
            change = _change;
            orderType = _orderType;

            txtUser.Text = Globals.Emp_Name;
            txtDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
            txtOrderType.Text = orderType;

            txtDue.Text = cartTotal.ToString();
            txtPaid.Text = amountPaid.ToString();
            txtChange.Text = change.ToString();

            ShowTransactionList();
        }

        public void ShowTransactionList()
        {
            foreach(Item item in items)
            {
                Grid grid = new Grid();

                ColumnDefinition gridCol1 = new ColumnDefinition
                {
                    Width = new GridLength(3, GridUnitType.Star)
                };

                ColumnDefinition gridCol2 = new ColumnDefinition
                {
                    Width = new GridLength(1, GridUnitType.Star)
                };

                ColumnDefinition gridCol3 = new ColumnDefinition
                {
                    Width = new GridLength(1, GridUnitType.Star)
                };

                ColumnDefinition gridCol4 = new ColumnDefinition
                {
                    Width = new GridLength(1, GridUnitType.Star)
                };

                grid.ColumnDefinitions.Add(gridCol1);
                grid.ColumnDefinitions.Add(gridCol2);
                grid.ColumnDefinitions.Add(gridCol3);
                grid.ColumnDefinitions.Add(gridCol4);


                TextBlock textblock1 = new TextBlock
                {
                    Text = item.ItemName.ToString(),
                    FontSize = 20
                };

                TextBlock textblock2 = new TextBlock
                {
                    Text = item.Price.ToString(),
                    FontSize = 20
                };

                TextBlock textblock3 = new TextBlock
                {
                    Text = item.Quantity.ToString(),
                    FontSize = 20
                };

                TextBlock textblock4 = new TextBlock
                {
                    Text = item.ItemTotal.ToString(),
                    FontSize = 20
                };

                Grid.SetColumn(textblock1, 0);
                Grid.SetColumn(textblock2, 1);
                Grid.SetColumn(textblock3, 2);
                Grid.SetColumn(textblock4, 3);
         
                grid.Children.Add(textblock1);
                grid.Children.Add(textblock2);
                grid.Children.Add(textblock3);
                grid.Children.Add(textblock4);

                sp.Children.Add(grid);
            }
        }

        private void BtnPrint_Click(object sender, RoutedEventArgs e)
        {
            PrintDialog print = new PrintDialog();

            if(print.ShowDialog() == true)
            {
                print.PrintVisual(spMain, "Invoice Test");

                main.ClearOrderList();
                main.ClearOrder();
                main.GetCurrentDailyIncome();
                main.Load_DailyTransactions();

                MessageBox.Show("Invoice Printed!");

                this.Close();
            }

        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            main.ClearOrderList();
            main.ClearOrder();
            main.GetCurrentDailyIncome();
            main.Load_DailyTransactions();
          
            this.Close();
        }


      
    }
}
