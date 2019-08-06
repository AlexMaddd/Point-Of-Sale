using System;
using System.Data;
using System.Windows;


namespace POS
{
    /// <summary>
    /// Interaction logic for ReadSelectedItemWindow.xaml
    /// </summary>
    public partial class ReadSelectedItemWindow : Window
    {
        MainWindow main;

        private bool controlsEnabled = false;

        public ReadSelectedItemWindow(DataRowView _row, MainWindow _mw)
        {
            InitializeComponent();

            DataContext = _row;

            main = _mw;
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if(controlsEnabled == false)
            {
                controlsEnabled = EnableControls();
                btnBack.Content = "Cancel";
                btnEdit.Content = "Save Changes";
            }
            else
            {
                MessageBoxResult result = MessageBox.Show("Save Changes?", "Confirmation", MessageBoxButton.YesNo);

                if(result == MessageBoxResult.Yes)
                {
                    string queryUpdateItem = string.Format("UPDATE items " +
                                         "SET " +
                                         "item_name=@item_name, " +
                                         "item_price=@item_price " +
                                         "WHERE " +
                                         "item_code=@item_code");

                    try
                    {
                        if (SqlQueries.UpdateItem(queryUpdateItem, txtItemName.Text, Convert.ToInt32(txtItemPrice.Text), lblItemCode.Content.ToString()) == true)
                        {
                            MessageBox.Show("Item Edited");

                            main.Load_Items();

                            this.Close();
                        }
                        else
                        {
                            MessageBox.Show("Something went wrong");
                        }
                    }
                    catch (Exception err)
                    {
                        MessageBox.Show(err.Message);
                    }
                }
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult mbResult = MessageBox.Show("Are you sure?", "Delete Confirmation", MessageBoxButton.YesNo);

            if(mbResult == MessageBoxResult.Yes)
            {
                string queryDeleteItem = string.Format("DELETE FROM items " +
                                            "WHERE " +
                                            "item_code=@item_code");

                try
                {
                    if(SqlQueries.DeleteItem(queryDeleteItem, lblItemCode.Content.ToString()) == true)
                    {
                        MessageBox.Show("Item Deleted");

                        main.Load_Items();
                    }
                    else
                    {
                        MessageBox.Show("Something went wrong");
                    }

                    this.Close();
                }
                catch(Exception err)
                {
                    MessageBox.Show(err.Message);
                }
            }
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            if(controlsEnabled == true)
            {
                controlsEnabled = DisableControls();
                btnBack.Content = "Back";
                btnEdit.Content = "Edit Item";
            }
            else
            {
                MessageBoxResult result = MessageBox.Show("Changes could have been made", "Confirmation", MessageBoxButton.YesNo);

                if(result == MessageBoxResult.Yes)
                    this.Close();
            }
        }


        public bool EnableControls()
        {
            txtItemName.IsEnabled = true;
            txtItemPrice.IsEnabled = true;

            return true;
        }

        public bool DisableControls()
        {
            txtItemName.IsEnabled = false;
            txtItemPrice.IsEnabled = false;

            return false;
        }
    }
}
