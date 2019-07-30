using System;
using System.Data;
using System.Linq;
using System.Windows;

namespace POS
{
    /// <summary>
    /// Interaction logic for OpenOrderItemWindow.xaml
    /// </summary>
    public partial class OpenOrderItemWindow : Window
    {
        //private Item orderItem;
        private MainWindow mw;

        private int index;
        private int itemPrice;
        private int itemTotal;
        private int quantity;
        private int counter = 1;

        private int quantityCounter;

        public OpenOrderItemWindow(int _orderNumber, MainWindow _mw)
        {
            InitializeComponent();

            index = _orderNumber - 1;

            mw = _mw;

            lblItemName.Content = mw.items[index].ItemName;
            txtQuantity.Text = mw.items[index].Quantity.ToString();

            itemPrice = mw.items[index].Price;

            quantityCounter = Convert.ToInt32(txtQuantity.Text);
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult mbResult = MessageBox.Show("Delete Confirmation", "Are you sure?", MessageBoxButton.YesNo);

            if(mbResult == MessageBoxResult.Yes)
            {
                mw.total.RemoveAt(index);
                mw.items.RemoveAt(index);

                OrderCounter();

                mw.ClearOrder();
                mw.Load_dgItemList();
                mw.Cart_Total();

                this.Close();
            }
         
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (btnDecrement.IsEnabled == false && btnIncrement.IsEnabled == false)
            {
                btnDecrement.IsEnabled = true;
                btnIncrement.IsEnabled = true;

                btnEdit.Content = "ENTER";
            }
            else
            {
                quantity = Convert.ToInt32(txtQuantity.Text);

                itemTotal = itemPrice * quantity;

                mw.items[index].Quantity = quantity;
                mw.items[index].ItemTotal = itemTotal;

                mw.total[index] = itemTotal;

                mw.ClearOrder();
                mw.Load_dgItemList();
                mw.Cart_Total();

                this.Close();
            }
        }


        public void OrderCounter()
        {
            mw.OrderCounterReset();

            for (int ctr = 0; ctr < mw.items.Count(); ctr++)
            {
                mw.items[ctr].OrderNumber = counter;
                counter++;
            }
        }


        private void BtnIncrement_Click(object sender, RoutedEventArgs e)
        {
            ++quantityCounter;
            txtQuantity.Text = quantityCounter.ToString();
        }


        private void BtnDecrement_Click(object sender, RoutedEventArgs e)
        {
            --quantityCounter;
            txtQuantity.Text = quantityCounter.ToString();
        }


        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
    
}
