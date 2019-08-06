    using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
    /// Interaction logic for CheckOut.xaml
    /// </summary>
    public partial class CheckOut : Window
    {
        private MainWindow main;
        private List<Item> items;

        private int cartTotal;
        private string cbResult;

        public CheckOut(List<Item> _items, int _cartTotal,  MainWindow _main)
        {
            InitializeComponent();
             
            items = _items;
            main = _main;
            cartTotal = _cartTotal;

            ShowDetails(items);
        }

        public void ShowDetails(List<Item> items)
        {
            dgDetails.ItemsSource = items;

            lblTotal.Content = "PHP " + cartTotal.ToString();
        }

        private void BtnDone_Click(object sender, RoutedEventArgs e)
        {
            if(cbDineIn.IsSelected = false && cbTakeOut.IsSelected == false && cbDriveThru.IsSelected == false)
            {
                MessageBox.Show("Select Order Type!");
            }
            else
            {
                MessageBoxResult result = MessageBox.Show("Finish transaction?", "Confrim", MessageBoxButton.YesNo);

                if(result == MessageBoxResult.Yes)
                {
                    string queryInsertIntoTransactionsTable = string.Format(
                                                   "INSERT INTO transactions" +
                                                   "(transaction_date, total_price, emp_id, order_type) " +
                                                   "VALUES(" +
                                                   "@transaction_date, @total_price, @emp_id, " +
                                                   "(SELECT orderType_id " +
                                                   "FROM order_type " +
                                                   "WHERE orderType_description = @order_type)" +
                                                   ")"
                                                   );

                    string queryInsertIntoTransactionDetails = string.Format("INSERT INTO transaction_details" +
                                                           "(transaction_id, item_code, total_item_price, quantity) " +
                                                           "VALUES(" +
                                                           "(SELECT TOP 1 transaction_id " +
                                                           "FROM transactions " +
                                                           "ORDER BY transaction_id DESC), " +
                                                           "@item_code, @total_item_price, @quantity" +
                                                           ")"
                                                           );

                    cbResult = cborderType.SelectionBoxItem.ToString();

                    if (SqlQueries.SqlExecNQInsertTransactionsTable(queryInsertIntoTransactionsTable, cartTotal, Globals.Emp_ID, cbResult) == true)
                    {
                        if (SqlQueries.SqlExecNQInsertTransactionDetails(queryInsertIntoTransactionDetails, items) == true)
                        {
                            MessageBox.Show("Transaction Complete!");
                            main.ClearOrderList();
                            main.ClearOrder();
                            main.GetCurrentDailyIncome();
                            main.Load_DailyTransactions();
                            this.Close();
                        }
                        else
                        {
                            MessageBox.Show("Something went wrong");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Something went wrong asadsad");
                    }
                }
            }
        }


        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
