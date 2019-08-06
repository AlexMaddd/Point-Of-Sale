using System;
using System.Data;
using System.Windows;

namespace POS
{
    /// <summary>
    /// Interaction logic for SelectedOrderWindow.xaml
    /// </summary>
    public partial class SelectedOrderWindow : Window
    {
        private MainWindow main;
        private DataRowView row;

        private int quantity;
        private int itemTotal;

        public SelectedOrderWindow(DataRowView _row, MainWindow _main)
        {
            InitializeComponent();

            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            
            quantity = Convert.ToInt32(txtQuantity.Text);

            DataContext = _row;
            main = _main;
            row = _row;
        }

        private void BtnIncrement_Click(object sender, RoutedEventArgs e)
        {
            quantity = ++quantity;
            txtQuantity.Text = quantity.ToString();
        }

        private void BtnDecrement_Click(object sender, RoutedEventArgs e)
        {
            if(quantity >= 1)
            {
                quantity = --quantity;
                txtQuantity.Text = quantity.ToString();
            }
        }

        private void BtnEnter_Click(object sender, RoutedEventArgs e)
        {
            ComputeItemTotal();

            main.items.Add(new Item()
            {
                OrderNumber = main.orderCounter,
                Quantity = quantity,
                ItemName = row["Item"].ToString(),
                ItemTotal = itemTotal,
                Code = row["Code"].ToString(),
                Price = Convert.ToInt32(row["Price"])
            });

            main.total.Add(itemTotal);
            main.orderCounter++;
            main.ClearOrder();
            main.Load_dgItemList();
            main.Cart_Total();
            this.Close();
        }

        private void ComputeItemTotal()
        {
            itemTotal = quantity * Convert.ToInt32(row["Price"]);
        }
    }
}
