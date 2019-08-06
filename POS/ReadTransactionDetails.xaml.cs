using System;
using System.Data;
using System.Windows;

namespace POS
{
    /// <summary>
    /// Interaction logic for ReadTransactionDetails.xaml
    /// </summary>
    public partial class ReadTransactionDetails : Window
    {
        private DataRowView row;

        public ReadTransactionDetails(DataRowView _row)
        {
            InitializeComponent();

            row = _row;

            tbTransID.Text = row["TransID"].ToString();

            Load_GetTransactionDetails();
        }


        public void Load_GetTransactionDetails()
        {
            string queryViewTransDetails = string.Format("SELECT " +
                                            "items.item_name AS ItemName, " +
                                            "items.item_price AS ItemPrice, " +
                                            "transaction_details.quantity AS Quantity " +
                                            "FROM items LEFT JOIN transaction_details " +
                                            "ON items.item_code = transaction_details.item_code " +
                                            "LEFT JOIN transactions " +
                                            "ON transactions.transaction_id = transaction_details.transaction_id " +
                                            "WHERE transactions.transaction_id = @trans_id " +
                                            "AND " +
                                            "transaction_details.transaction_id = @trans_id");

            int trans_id = Convert.ToInt32(row["TransID"]);

            dgTransDetails.ItemsSource = SqlQueries.Load_GetTransactionDetails(queryViewTransDetails, trans_id).DefaultView;

        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
