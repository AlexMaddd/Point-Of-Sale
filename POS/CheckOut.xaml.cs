using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

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
        private int change;
        private int amountPaid = 0;

        private string cbResult;

        private bool hasPaid = false;

        public CheckOut(List<Item> _items, int _cartTotal,  MainWindow _main)
        {
            InitializeComponent();
             
            items = _items;
            main = _main;
            cartTotal = _cartTotal;

            lblTotal.Content = "PHP " + cartTotal.ToString();
        }


        // records transaction into Transactions table and Transaction Details table
        private void BtnDone_Click(object sender, RoutedEventArgs e)
        {
            if(cbDineIn.IsSelected = false && cbTakeOut.IsSelected == false && cbDriveThru.IsSelected == false)
            {
                MessageBox.Show("Select Order Type!");
            }
            else if(hasPaid == false)
            {
                MessageBox.Show("Enter amount paid!");
            }
            else
            {
                MessageBoxResult result = MessageBox.Show("Finish transaction?", "Confrim", MessageBoxButton.YesNo);

                if(result == MessageBoxResult.Yes)
                {
                    string queryInsertIntoTransactionsTable = string.Format(
                                                   "INSERT INTO transactions" +
                                                   "(transaction_date, total_price, emp_id, order_type, amount_paid, change) " +
                                                   "VALUES(" +
                                                   "@transaction_date, @total_price, @emp_id, " +
                                                   "(SELECT orderType_id " +
                                                   "FROM order_type " +
                                                   "WHERE orderType_description = @order_type), " +
                                                   "@amount_paid, @change" +
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

                    if (SqlQueries.SqlExecNQInsertTransactionsTable(queryInsertIntoTransactionsTable, cartTotal, Globals.Emp_ID, cbResult, amountPaid, change) == true)
                    {
                        if (SqlQueries.SqlExecNQInsertTransactionDetails(queryInsertIntoTransactionDetails, items) == true)
                        {
                            MessageBox.Show("Transaction Complete!");
                            MessageBox.Show(cbResult.ToString());
                            Receipt receipt = new Receipt(items, cartTotal, amountPaid, change, cbResult, main);
                            receipt.ShowDialog();

                            //main.ClearOrderList();
                            //main.ClearOrder();
                            //main.GetCurrentDailyIncome();
                            //main.Load_DailyTransactions();

                            this.Close();
                        }
                        else
                        {
                            MessageBox.Show("Something went wrongggg");
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


        private void BtnCompute_Click(object sender, RoutedEventArgs e)
        {
            amountPaid = Convert.ToInt32(tbAmountPaid.Text);

            if(amountPaid < cartTotal)
            {
                MessageBox.Show("Paid amount less than amount payable");
            }
            else
            {
                change = amountPaid - cartTotal;
                txtChange.Text = change.ToString();

                hasPaid = true;
            }
         
        }
    }
}
