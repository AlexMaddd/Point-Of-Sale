using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Windows;

namespace POS
{

    public partial class MainWindow : Window
    {
        // List for transaction items
        public List<Item> items = new List<Item>();

        // List for storing price * quantity total 
        // for List<Item> items transactions
        public List<int> total = new List<int>();

        private int cartTotal = 0;

        public int orderCounter = 1;

        private string empName;
        private bool logout = false;

        public MainWindow()
        {
            InitializeComponent();

            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            CheckRole();

            GetEmployeeName();

            GetCurrentDailyIncome();

            Load_POSItems();
            Load_DailyTransactions();
            Load_ActiveUsers();
            Load_Items();

            SetCalendarConstraints();

            // listens for .Close() event and checks whether logout = true 
            // to properly close program without using default window close btn
            this.Closing += new CancelEventHandler(OnClosing);

        }

        //computes and outputs total purchase
        public void Cart_Total()
        {
            cartTotal = 0;

            foreach (var price in total)
            {
                cartTotal += Convert.ToInt32(price);
            }

            labelCartTotal.Content = "PHP " + cartTotal.ToString();

            UsersTab.IsEnabled = false;
            ItemsTab.IsEnabled = false;
            RecordsTab.IsEnabled = false;
        }


        //load available item menu
        public void Load_POSItems()
        {
            string queryLoadAvailableItems = string.Format("SELECT item_code AS Code, " +
                                                "item_name AS Item, " +
                                                "item_price AS Price " +
                                                "FROM items");

            try
            {
                this.dgPOSItems.ItemsSource = SqlQueries.Load_DataGridItems(queryLoadAvailableItems).DefaultView;
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }


        //proceeds to CashOut window after finishing transaction
        private void BtnCashOut_Click(object sender, RoutedEventArgs e)
        {
            if (cartTotal != 0)
            {
                CheckOut co = new CheckOut(items, cartTotal, this);
                co.ShowDialog();
            }
            else
            {
                btnCashOut.IsEnabled = false;
                MessageBox.Show("No Purchase");
            }
        }

        
        //loads ordered items on datagrid from List<Item> items
        public void Load_dgItemList()
        {
            dgOrderList.ItemsSource = items;
        }


        public void ClearOrderList()
        {
            items.Clear();
        }

        //clears DataGrid and labelCartTotal after cashing out
        public void ClearOrder()
        {
            dgOrderList.ItemsSource = null;
            cartTotal = 0;
            labelCartTotal.Content = cartTotal.ToString();

            CheckRole();
        }


        //gets Employee Name for lblEmployee and lblEmpID
        public void GetEmployeeName()
        {
            string queryGetEmpName = string.Format("SELECT fname AS FName, " +
                                      "lname AS LName " +
                                      "FROM users_information " +
                                      "WHERE emp_id = @empid");

            try
            {
                empName = SqlQueries.GetEmployeeName(queryGetEmpName, Globals.Emp_ID);

                if (!String.IsNullOrWhiteSpace(empName)) 
                {
                    lblEmployee.Content = empName;
                    lblEmpID.Content = Globals.Emp_ID;
                }
                else
                {
                    Login log = new Login();
                    this.Close();
                    log.ShowDialog();
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }


        // checks role to determine access to tabs
        public void CheckRole()
        {
            if (Globals.Role_ID == 1)
            {
                UsersTab.IsEnabled = true;
                ItemsTab.IsEnabled = true;
                RecordsTab.IsEnabled = true;
            }
            else
            {
                UsersTab.IsEnabled = false;
                ItemsTab.IsEnabled = false;
                RecordsTab.IsEnabled = false;
            }
        }


        // accesses interface to create user
        private void BtnAddUserTab_Click(object sender, RoutedEventArgs e)
        {
            spAddUserTab.Visibility = Visibility.Visible;
            cEditUserTab.Visibility = Visibility.Hidden;
        }


        private void BtnEditUserTab_Click(object sender, RoutedEventArgs e)
        {
            cEditUserTab.Visibility = Visibility.Visible;
            spAddUserTab.Visibility = Visibility.Hidden;
        }


        // enter button to create new user
        private void BtnNewAddUser_Click(object sender, RoutedEventArgs e)
        {
            int role_id;

            string queryAddNewUser = string.Format("INSERT INTO users_information" +
                                        "(emp_id, fname, lname) " +
                                        "VALUES" +
                                        "(@empID, @fname, @lname)");

            string queryInsertIntoUsersTable = string.Format("INSERT INTO users" +
                                                "(emp_id, role_id, password) " +
                                                "VALUES" +
                                                "((SELECT TOP 1 emp_id " +
                                                "FROM users_information " +
                                                "ORDER BY emp_id DESC" +
                                                "), @role_id, @password)");

            try
            {
                if (Helper.CheckInsertUserInputs(spAddUserTab, rbUser, rbAdmin))
                {
                    if (txtPassword.Password == txtPasswordConfirm.Password)
                    {
                        if(SqlQueries.InsertIntoUsersInformationTable(queryAddNewUser, Convert.ToInt32(txtEmpID.Text), txtFName.Text, txtLName.Text) == true)
                        {
                            if (rbUser.IsChecked == true)
                            {
                                role_id = 2;
                            }
                            else
                            {
                                role_id = 1;
                            }

                            if (SqlQueries.InsertIntoUsersTable(queryInsertIntoUsersTable, role_id, txtPassword.Password) == true)
                            {
                                MessageBox.Show("New User Added");
                                Helper.ClearInsertUserInputs(spAddUserTab);
                                Load_ActiveUsers();
                                cEditUserTab.Visibility = Visibility.Visible;
                            }
                        }

                        Helper.ClearInsertUserInputs(spAddUserTab);
                    }
                    else
                    {
                        MessageBox.Show("Passwords dont match");
                    }
                }
                else
                {
                    MessageBox.Show("Value cannot be empty or null");
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }


        // loads all active users in DB
        public void Load_ActiveUsers()
        {
            string querySelectAllActiveUsers = string.Format("SELECT " +
                                            "users_information.emp_id AS Emp_ID," +
                                            "users_information.fname AS FName," +
                                            "users_information.lname AS LName," +
                                            "roles.description AS Role " +
                                            "FROM users_information LEFT JOIN users " +
                                            "ON users_information.emp_id = users.emp_id " +
                                            "LEFT JOIN roles " +
                                            "ON users.role_id = roles.role_id " +
                                            "WHERE users_information.status = 'Active'");

            try
            {

                this.dgUserList.ItemsSource = SqlQueries.Load_DataGridItems(querySelectAllActiveUsers).DefaultView;
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }


        // reads active user data
        private void BtnOpenUser_Click(object sender, RoutedEventArgs e)
        {
            DataRowView row = dgUserList.SelectedItem as DataRowView;

            ReadSelectedUserWindow suw = new ReadSelectedUserWindow(row, this);
            suw.ShowDialog();
        }


        private void BtnEditOrder_Click(object sender, RoutedEventArgs e)
        {
            int orderNumber = Convert.ToInt32(dgOrderList.SelectedIndex) + 1;
            try
            {
                OpenOrderItemWindow oiw = new OpenOrderItemWindow(orderNumber, this);
                oiw.ShowDialog();
            }
            catch (Exception err)
            {
                MessageBox.Show(err.StackTrace + "           " + err.TargetSite);
            }
        }

        //checks if an item is selected from dgORderList
        //not used in POS_V2
        public bool IsOrderRowSelected()
        {
            if (dgOrderList.SelectedIndex == -1)
            {
                return false;
            }

            return true;
        }

        public void OrderCounterReset()
        {
            orderCounter = 0;
        }


        // loads all available menu utems
        public void Load_Items()
        {
            string querySelectAllItems = string.Format("SELECT item_code AS ItemCode, " +
                                            "item_name AS ItemName," +
                                            "item_price AS ItemPrice " +
                                            "FROM items");

            try
            {
                dgItemsList.ItemsSource = SqlQueries.Load_DataGridItems(querySelectAllItems).DefaultView;
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }


        // accesses interface to add new item
        private void BtnAddItemTab_Click(object sender, RoutedEventArgs e)
        {
            spAddItemTab.Visibility = Visibility.Visible;
            spEditItemTab.Visibility = Visibility.Hidden;
        }


        private void BtnEditItemTab_Click(object sender, RoutedEventArgs e)
        {
            spAddItemTab.Visibility = Visibility.Hidden;
            spEditItemTab.Visibility = Visibility.Visible;
        }


        // enter button to add new item
        private void BtnNewAddItem_Click(object sender, RoutedEventArgs e)
        {
            string queryAddNewItem = string.Format("INSERT INTO items " +
                                        "(item_code, item_name, item_price) " +
                                        "VALUES(@item_code, @item_name, @item_price)");

            try
            {
                if(Helper.CheckInsertItemInputs(spAddItemTab) == true)
                {
                   if(SqlQueries.InsertNewItem(queryAddNewItem, txtItemCode.Text, txtItemName.Text, Convert.ToInt32(txtItemPrice.Text)) == true)
                    {
                        MessageBox.Show("New Item Added");
                        Helper.ClearInsertItemInputs(spAddItemTab);
                        Load_Items();
                        spEditItemTab.Visibility = Visibility.Visible;
                    }
                   else
                    {
                        MessageBox.Show("Something went wrong");
                    }
                }
                else
                {
                    MessageBox.Show("Value cannot be empty or null");
                 
                }
            }
            catch(Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        
        // opens window to view item data
        private void BtnOpenItem_Click(object sender, RoutedEventArgs e)
        {
            DataRowView row = dgItemsList.SelectedItem as DataRowView;
            ReadSelectedItemWindow rsiw = new ReadSelectedItemWindow(row, this);
            rsiw.ShowDialog();
        }


        // button to logout
        // sets logout data to true to close application
        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            string queryLoggerOut = string.Format("INSERT INTO timelog" +
                                        "(emp_id, log_time, log_status) " +
                                        "VALUES(@emp_id, @log_time, @log_status)");

            if(cartTotal == 0)
            {
                if (SqlQueries.Logger(queryLoggerOut, Globals.Emp_ID, btnLogout.Content.ToString()) == true)
                {
                    MessageBox.Show("Logging Out");

                    lblEmpID.Content = " ";
                    lblEmployee.Content = " ";

                    Login login = new Login();

                    logout = true;

                    if(logout == true)
                    {
                        this.Close();
                    }
                 

                    login.Show();
                }
                else
                {
                    MessageBox.Show("Something went wrong logging out");
                }
            }
            else
            {
                MessageBox.Show("Transaction Ongoing");
            }

        }


        // selects and opens new window to set item quantity
        private void BtnSelectItem_Click(object sender, RoutedEventArgs e)
        {
            DataRowView row = dgPOSItems.SelectedItem as DataRowView;
            SelectedOrderWindow ow = new SelectedOrderWindow(row, this);

            ow.ShowDialog();
        }

        
        // clears all orders 
        private void BtnClearOrders_Click(object sender, RoutedEventArgs e)
        {
            if(cartTotal != 0)
            {
                MessageBoxResult result = MessageBox.Show("Are you sure?", "Confirmation", MessageBoxButton.YesNo);

                if (result == MessageBoxResult.Yes)
                {
                    ClearOrderList();
                    ClearOrder();
                    CheckRole();
                }
            }
            else
            {
                MessageBox.Show("Currently no orders");
            }
        }


        // loads list of transactions for current date
        public void Load_DailyTransactions()
        {
            string queryLoadDailyTransactions = string.Format("SELECT " +
                                                    "transaction_id AS TransID, " +
                                                    "emp_id AS EmpID, " +
                                                    "transaction_date AS Date, " +
                                                    "total_price AS Price " +
                                                    "FROM transactions " +
                                                    "WHERE CAST(transaction_date AS Date) = @date");

            dgDailyTransactions.ItemsSource = SqlQueries.Load_GetDailyTransactions(queryLoadDailyTransactions).DefaultView;
        }


        // computes income for today and displays as text
        public void GetCurrentDailyIncome()
        {
            string queryGetCurrentDailyIncome = string.Format("SELECT SUM(CAST(total_price AS Int)) AS Total_Income " +
                                                    "FROM transactions " +
                                                    "WHERE CAST(transaction_date as Date) = @date");

            int daily_total = SqlQueries.GetCurrentDailyIncome(queryGetCurrentDailyIncome);

            tbDateToday.Text = DateTime.Now.ToString("yyyy-MM-dd");
            txtCurrentEarnings.Text = daily_total.ToString();
        }


        // Records tab button
        // shows daily income interface
        private void BtnDailyIncome_Click(object sender, RoutedEventArgs e)
        {
            cDailyIncomeTab.Visibility = Visibility.Visible;
            cIncomeRecords.Visibility = Visibility.Hidden;
        }


        // Records tab button
        // shows range income interface
        private void BtnDailyIncomeRecords_Click(object sender, RoutedEventArgs e)
        {
            cDailyIncomeTab.Visibility = Visibility.Hidden;
            cIncomeRecords.Visibility = Visibility.Visible;
        }


        // opens window to view transaction details for specific transaction
        private void BtnViewTransaction_Click(object sender, RoutedEventArgs e)
        {
            DataRowView row = dgDailyTransactions.SelectedItem as DataRowView;
            ReadTransactionDetails rtd = new ReadTransactionDetails(row);
            rtd.Show();
        }


        // creates or updates DB data for current daily income
        private void BtnEnterDailyIncome_Click(object sender, RoutedEventArgs e)
        {
            int dailyIncome = Convert.ToInt32(txtCurrentEarnings.Text);

            string queryCheckForExistingRecord = string.Format("SELECT CAST(date AS Date) " +
                                                    "FROM daily_IncomeRecord " +
                                                    "WHERE date = @date");

            string queryEnterDailyIncome = string.Format("INSERT INTO daily_IncomeRecord " +
                                                "(date, total_income, recorded_by_emp_id) " +
                                                "VALUES" +
                                                "(@date, @total_income, @emp_id)");

            string queryUpdateDailyIncome = string.Format("UPDATE daily_IncomeRecord " +
                                                "SET total_income = @total_income " +
                                                "WHERE date = @date");

            string queryCheckForExistingTotalIncomeDifference = string.Format("SELECT total_income " +
                                                "FROM daily_IncomeRecord " +
                                                "WHERE date = @date");


            if(SqlQueries.CheckForExistingDailyIncomeRecord(queryCheckForExistingRecord) == false)
            {
                if(SqlQueries.SqlExecNQInsertIntoDailyIncomeRecord(queryEnterDailyIncome, dailyIncome) == true)
                {
                    MessageBox.Show("Daily Income Recorded Inserted!");
                }
            }
            else
            {
                if(SqlQueries.CheckForExistingTotalIncomeDifference(queryCheckForExistingTotalIncomeDifference, dailyIncome) == false)
                {
                    if (SqlQueries.SqlExecNQInsertIntoDailyIncomeRecord(queryUpdateDailyIncome, dailyIncome) == true)
                    {
                        MessageBox.Show("Record Updated!");
                    }
                }
                else
                {
                    MessageBox.Show("Previous record is unchanged. Nothing to update");
                }
            }
        }


        // listens for .Close() method and checks whether logout value is true or false
        // forces user to use logout button to close MainWindow Window and return to login form
        private void OnClosing(object sender, CancelEventArgs e)
        {
            if(logout == false)
            {
                e.Cancel = true;
                MessageBox.Show("LogOut Required");
            }
        }


        // Computes income from specified date ranges
        private void BtnComputeIncomeRange_Click(object sender, RoutedEventArgs e)
        {
            if(dateStart.SelectedDate != null && dateEnd.SelectedDate != null)
            {
                string queryComputeIncomeRange = string.Format("SELECT SUM(CAST(total_income AS INT)) AS Total_Income " +
                                               "FROM daily_IncomeRecord " +
                                               "WHERE date " +
                                               "BETWEEN @startDate AND @endDate");

                string startDate = dateStart.Text;
                string endDate = dateEnd.Text;

                int total = SqlQueries.ComputeIncomeRange(queryComputeIncomeRange, startDate, endDate);

                if (total > 0)
                {
                    txtRangeIncomeTotal.Text = total.ToString();
                }
                else
                {
                    MessageBox.Show("asdsadas");
                }
            }
            else
            {
                MessageBox.Show("Select range");
            }
           
        }


        // sets constraints for start date and end date
        private void SetCalendarConstraints()
        {
            string queryFirstIncomeDate = string.Format("SELECT TOP 1 date " +
                                            "FROM daily_IncomeRecord " +
                                            "ORDER BY date ASC");

            string resultDate = SqlQueries.GetFirstIncomeDate(queryFirstIncomeDate);

            if(resultDate != null)
            {
                DateTime startingDate = Convert.ToDateTime(resultDate);
     
                dateStart.DisplayDateStart = startingDate;
                dateStart.DisplayDateEnd = DateTime.Now;

                dateEnd.DisplayDateEnd = DateTime.Now;
                dateEnd.DisplayDateStart = startingDate;
            }
            else
            {
                MessageBox.Show("No starting date detected");
            }
        }

    }
}
