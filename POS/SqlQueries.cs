using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;

namespace POS
{
    class SqlQueries
    {
        static DataConnection dc = Globals.DbConnect();
        static SqlCommand cmd = new SqlCommand();
        static SqlDataReader reader;


        // logins user
        public static bool Login(string query, int emp_id, string password)
        {
            int affectedrows = -1;

            cmd.Connection = dc.Connect();

            if(cmd.Connection.State == ConnectionState.Open)
            {
                try
                {
                    cmd.CommandText = query;

                    cmd.Parameters.AddWithValue("@empID", emp_id);
                    cmd.Parameters.AddWithValue("@password", password);

                    reader = cmd.ExecuteReader();

                    if(reader.HasRows == true)
                    {
                        if (reader.Read())
                        {
                            if(reader["STATUS"].ToString() == "Active")
                            {
                                Globals.Emp_ID = Convert.ToInt32(reader["EMP_ID"]);
                                Globals.Role_ID = Convert.ToInt32(reader["ROLE_ID"]);

                                affectedrows = 1;
                            }
                            else
                            {
                                MessageBox.Show("User status is not active");
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("Invalid Credentials");
                    }

                    reader.Close();
                }
                catch(Exception ex)
                {
                    MessageBox.Show("Login Error: " + ex.Message);
                }
            }

            cmd.Parameters.Clear();
            cmd.Dispose();
            cmd.Connection.Close();
            dc.Close();

            if(affectedrows == 1)
            {
                return true;
            }

            return false;
        }


        // inserts into transactions table
        public static bool SqlExecNQInsertTransactionsTable(string query, int cartTotal, int emp_id, string orderType, int amountPaid, int change)
        {
            int affectedrows = -1;

            cmd.Connection = dc.Connect();

            try
            {
                cmd.CommandText = query;

                cmd.Parameters.AddWithValue("@transaction_date", DateTime.Now);
                cmd.Parameters.AddWithValue("@total_price", cartTotal);
                cmd.Parameters.AddWithValue("@emp_id", emp_id);
                cmd.Parameters.AddWithValue("@order_type", orderType);
                cmd.Parameters.AddWithValue("@amount_paid", amountPaid);
                cmd.Parameters.AddWithValue("@change", change);

                affectedrows = cmd.ExecuteNonQuery();

                cmd.Parameters.Clear();
                cmd.Dispose();
            }
            catch(Exception ex)
            {
                MessageBox.Show("Insert Error: " + ex.Message);
            }

            dc.Close();

            if (affectedrows == 1)
            {
                return true;
            }

            return false;
        }


        // inserts into transaction details table
        public static bool SqlExecNQInsertTransactionDetails(string query, List<Item> items)
        {
            int affectedrows = -1;

            cmd.Connection = dc.Connect();

            try
            {
                foreach(Item item in items)
                {
                    cmd.CommandText = query;

                    cmd.Parameters.AddWithValue("@item_code", item.Code);
                    cmd.Parameters.AddWithValue("@total_item_price", item.ItemTotal);
                    cmd.Parameters.AddWithValue("@quantity", item.Quantity);

                    cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();

                    affectedrows += 1;
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("Insert Error: " + ex.Message);
            }

            dc.Close();

            if(affectedrows >= 1)
            {
                return true;
            }

            return false;
        }


        // loads datagrid values from DB
        // single query method for all datagrid controls
        public static DataTable Load_DataGridItems(string query)
        {
            DataTable dtable = new DataTable();
            cmd.Connection = dc.Connect();

            if(cmd.Connection.State == ConnectionState.Open)
            {
                try
                {
                    cmd.CommandText = query;

                    reader = cmd.ExecuteReader();

                    dtable.Load(reader);

                    cmd.Dispose();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Load Table Error: " + ex.Message);
                }
            }

            dc.Close();

            return dtable;
        }


        // gets employee name for MainWindow display
        public static string GetEmployeeName(string query, int emp_id)
        {
            string empName = "";

            cmd.Connection = dc.Connect();
            
            try
            {
                cmd.CommandText = query;

                cmd.Parameters.AddWithValue("@empid", emp_id);

                reader = cmd.ExecuteReader();

                if(Convert.ToInt32(reader.HasRows) == 1)
                {
                    if(reader.Read())
                    {
                        empName = reader["FName"].ToString() + " " + reader["LName"].ToString();
                    }

                    cmd.Parameters.Clear();
                    cmd.Dispose();
                }
                else
                {
                    MessageBox.Show("No employee found with ID " + emp_id);
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }

            dc.Close();

            return empName;
        }


        // inserts new item into DB
        public static bool InsertNewItem(string query, string itemCode, string itemName, int price)
        {
            int affectedrows = -1;

            cmd.Connection = dc.Connect();

            if(cmd.Connection.State == ConnectionState.Open)
            {
                try
                {
                    cmd.CommandText = query;

                    cmd.Parameters.AddWithValue("@item_code", itemCode);
                    cmd.Parameters.AddWithValue("@item_name", itemName);
                    cmd.Parameters.AddWithValue("@item_price", price);

                    affectedrows = cmd.ExecuteNonQuery();

                    cmd.Parameters.Clear();
                    cmd.Dispose();
                }
                catch(Exception ex)
                {
                    MessageBox.Show("Insert item error: " + ex.Message);
                }
             
            }

            if(affectedrows == 1)
            {
                return true;
            }

            return false;
        }


        // inserts new user details into Users Information table
        public static bool InsertIntoUsersInformationTable(string query, int emp_id, string fname, string lname)
        {
            int affectedrows = -1;

            cmd.Connection = dc.Connect();

            try
            {
                cmd.CommandText = query;

                cmd.Parameters.AddWithValue("@empID", emp_id);
                cmd.Parameters.AddWithValue("@fname", fname);
                cmd.Parameters.AddWithValue("@lname", lname);

                affectedrows = cmd.ExecuteNonQuery();

                cmd.Parameters.Clear();
                cmd.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Insert into users error: " + ex.Message);
  
            }

            dc.Close();

            if (affectedrows == 1)
            {
                return true;
            }

            return false;
        }


        // inserts new active user into Users table
        public static bool InsertIntoUsersTable(string query, int role_id, string password)
        {
            int affectedrows = -1;

            cmd.Connection = dc.Connect();

            if(cmd.Connection.State == ConnectionState.Open)
            {
                try
                {
                    cmd.CommandText = query;

                    cmd.Parameters.AddWithValue("@role_id", role_id);
                    cmd.Parameters.AddWithValue("@password", password);

                    affectedrows = cmd.ExecuteNonQuery();

                    cmd.Parameters.Clear();
                    cmd.Dispose();
                }
                catch(Exception ex)
                {
                    MessageBox.Show("Error in inserting into user details: " + ex.Message);
                }
            }

            dc.Close();

            if (affectedrows == 1)
            {
                return true;
            }

            return false;
        }


        // updates item details
        public static bool UpdateItem(string query, string itemName, int itemPrice, string itemCode)
        {
            int affectedrows = -1;

            cmd.Connection = dc.Connect();

            if(cmd.Connection.State == ConnectionState.Open)
            {
                try
                {
                    cmd.CommandText = query;

                    cmd.Parameters.AddWithValue("@item_name", itemName);
                    cmd.Parameters.AddWithValue("@item_price", itemPrice);
                    cmd.Parameters.AddWithValue("@item_code", itemCode);

                    affectedrows = cmd.ExecuteNonQuery();

                    cmd.Parameters.Clear();
                    cmd.Dispose();
                    dc.Close();
                }
                catch(Exception ex)
                {
                    MessageBox.Show("Error in updating item: " + ex.Message);
                    dc.Close();
                }
            }

            dc.Close();

            if (affectedrows == 1)
            {
                return true;
            }

            return false;
        }


        // deletes item data
        public static bool DeleteItem(string query, string itemCode)
        {
            int affectedrows = -1;

            cmd.Connection = dc.Connect();

            if(cmd.Connection.State == ConnectionState.Open)
            {
                try
                {
                    cmd.CommandText = query;

                    cmd.Parameters.AddWithValue("@item_code", itemCode);

                    affectedrows = cmd.ExecuteNonQuery();

                    cmd.Parameters.Clear();
                    cmd.Dispose();
                }
                catch(Exception ex)
                {
                    MessageBox.Show("Error in deleting item: " + ex.Message);

                }
            }

            dc.Close();

            if (affectedrows == 1)
            {
                return true;
            }

            return false;
        }


        // records login and logout of user
        public static bool Logger(string query, int emp_id, string buttonName)
        {
            int affectedrows = -1;

            cmd.Connection = dc.Connect();

            if(cmd.Connection.State == ConnectionState.Open)
            {
                try
                {
                    cmd.CommandText = query;

                    cmd.Parameters.AddWithValue("@emp_id", emp_id);
                    cmd.Parameters.AddWithValue("@log_time", DateTime.Now);

                    if(buttonName == "LOGIN")
                    {
                        cmd.Parameters.AddWithValue("@log_status", "IN");
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@log_status", "OUT");
                    }
             

                    affectedrows = cmd.ExecuteNonQuery();

                    cmd.Parameters.Clear();
                    cmd.Dispose();
                }
                catch(Exception ex)
                {
                    MessageBox.Show("Something went wrong logger: " + ex.Message);
                }
            }

            dc.Close();

            if (affectedrows == 1)
            {
                return true;
            }

            return false;
        }


        // edits user data
        public static bool EditUserData(string query, int emp_id, int role_id, string fname, string lname)
        {
            int affectedrows = -1;

            cmd.Connection = dc.Connect();

            if(cmd.Connection.State == ConnectionState.Open)
            {
                try
                {
                    cmd.CommandText = query;

                    cmd.Parameters.AddWithValue("@emp_id", emp_id);
                    cmd.Parameters.AddWithValue("@role_id", role_id);
                    cmd.Parameters.AddWithValue("@fname", fname);
                    cmd.Parameters.AddWithValue("@lname", lname);

                    affectedrows = cmd.ExecuteNonQuery();

                    cmd.Parameters.Clear();
                    cmd.Dispose();
                }
                catch(Exception ex)
                {
                    MessageBox.Show("Error in editing user data: " + ex.Message);
                }
            }

            dc.Close();

            if(affectedrows == 2)
            {
                return true;
            }

            return false;
        }


        // soft deletes user from active status and prevents login into main application
        public static bool DeactivateUser(string query, int emp_id)
        {
            int affectedrows = -1;

            cmd.Connection = dc.Connect();

            if(cmd.Connection.State == ConnectionState.Open)
            {
                try
                {
                    cmd.CommandText = query;

                    cmd.Parameters.AddWithValue("@emp_id", emp_id);

                    affectedrows = cmd.ExecuteNonQuery();

                    cmd.Parameters.Clear();
                    cmd.Dispose();
                }
                catch(Exception ex)
                {
                    MessageBox.Show("Error in deactivating user: " + ex.Message);
                }
            }

            dc.Close();

            return (affectedrows == 1) ? true : false;
        }


        // computes daily total income
        // adds all transactions for the day
        public static int GetCurrentDailyIncome(string query)
        {
            int total = 0;

            cmd.Connection = dc.Connect();

            if(cmd.Connection.State == ConnectionState.Open)
            {
                try
                {
                    cmd.CommandText = query;

                    cmd.Parameters.AddWithValue("@date", DateTime.Now.ToString("yyyy-MM-dd"));

                    reader = cmd.ExecuteReader();

                    if(Convert.ToInt32(reader.HasRows) == 1)
                    {
                        if(reader.Read())
                        {
                            if(DBNull.Value.Equals(reader["Total_Income"]))
                            {
                                total = 0;
                            }
                            else
                            {
                                total = Convert.ToInt32(reader["Total_Income"]);
                            }
                        }
                    }

                    cmd.Parameters.Clear();
                    cmd.Dispose();
                }
                catch(Exception ex)
                {
                    MessageBox.Show("Error in getting daily income: " + ex.Message);
                }
            }

            dc.Close();

            return total;
        }


        // gets all transaction details for each transaction entry
        public static DataTable Load_GetTransactionDetails(string query, int trans_id)
        {
            DataTable dtable = new DataTable();
            cmd.Connection = dc.Connect();

            if(cmd.Connection.State == ConnectionState.Open)
            {
                try
                {
                    cmd.CommandText = query;

                    cmd.Parameters.AddWithValue("@trans_id", trans_id);

                    reader = cmd.ExecuteReader();

                    dtable.Load(reader);

                    cmd.Parameters.Clear();
                    cmd.Dispose();
                }
                catch(Exception ex)
                {
                    MessageBox.Show("Error in loading transaction details: " + ex.Message);
                }
            }

            dc.Close();

            return dtable;
        }


        // gets all transaction entry for the day
        public static DataTable Load_GetDailyTransactions(string query)
        {
            DataTable dtable = new DataTable();
            cmd.Connection = dc.Connect();

            if (cmd.Connection.State == ConnectionState.Open)
            {
                try
                {
                    cmd.CommandText = query;

                    cmd.Parameters.AddWithValue("@date", DateTime.Now.ToString("yyyy-MM-dd"));

                    reader = cmd.ExecuteReader();

                    dtable.Load(reader);

                    cmd.Parameters.Clear();
                    cmd.Dispose();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Load Table Error: " + ex.Message);
                }
            }

            dc.Close();

            return dtable;
        }


        // checks for daily income record entry 
        // checks whether it needs to create or update an existing entry
        public static bool CheckForExistingDailyIncomeRecord(string query)
        {
            int row = -1;

            cmd.Connection = dc.Connect();

            if(cmd.Connection.State == ConnectionState.Open)
            {
                try
                {
                    cmd.CommandText = query;

                    cmd.Parameters.AddWithValue("@date", DateTime.Now.ToString("yyyy-MM-dd"));

                    if(cmd.ExecuteScalar() != null)
                    {
                        row = 1;
                    }

                    cmd.Parameters.Clear();
                    cmd.Dispose();
                }
                catch(Exception ex)
                {
                    MessageBox.Show("Error in checking for existing record: " + ex.Message);
                }
            }

            dc.Close();

            if(row == 1)
            {
                return true;
            }

            return false;
        }


        // checks if it is not necessary to update existing daily income record entry
        public static bool CheckForExistingTotalIncomeDifference(string query, int total_income)
        {
            cmd.Connection = dc.Connect();

            if(cmd.Connection.State == ConnectionState.Open)
            {
                try
                {
                    cmd.CommandText = query;

                    cmd.Parameters.AddWithValue("@date", DateTime.Now.ToString("yyyy-MM-dd"));

                    if(total_income == Convert.ToInt32(cmd.ExecuteScalar()))
                    {
                        dc.Close();
                        return true;
                    }

                    cmd.Parameters.Clear();
                    cmd.Dispose();
                }
                catch(Exception ex)
                {
                    MessageBox.Show("Error in checking for income difference: " + ex.Message);
                }
            }

            dc.Close();

            return false;
        }


        //  creates a daily income record entry or updates and existing entry
        public static bool SqlExecNQInsertIntoDailyIncomeRecord(string query, int total_income)
        {
            int affectedrows = -1;

            cmd.Connection = dc.Connect();

            if(cmd.Connection.State == ConnectionState.Open)
            {
                try
                {
                    cmd.CommandText = query;

                    cmd.Parameters.AddWithValue("@date", DateTime.Now.ToString("yyyy-MM-dd"));
                    cmd.Parameters.AddWithValue("@total_income", total_income);
                    cmd.Parameters.AddWithValue("@emp_id", Globals.Emp_ID);

                    affectedrows = cmd.ExecuteNonQuery();

                    cmd.Parameters.Clear();
                    cmd.Dispose();
                }
                catch(Exception ex)
                {
                    MessageBox.Show("Error in inserting daily income record: " + ex.Message);
                }
            }

            dc.Close();

            if(affectedrows == 1)
            {
                return true;
            }

            return false;
        }


        // gets the first daily income record entry to set start date
        // for computing income range
        public static string GetFirstIncomeDate(string query)
        {
            string date = null;

            cmd.Connection = dc.Connect();

            if(cmd.Connection.State == ConnectionState.Open)
            {
                try
                {
                    cmd.CommandText = query;

                    date = cmd.ExecuteScalar().ToString();
                }
                catch(Exception ex)
                {
                    MessageBox.Show("Something went wrong in getting start date: " + ex.Message);
                }
            }

            cmd.Dispose();
            dc.Close();

            return date;
        }


        // computes all daily incom record totals 
        // within the specified date range
        public static int ComputeIncomeRange(string query, string _startDate, string _endDate)
        {
            int total = 0;

            DateTime startDate = Convert.ToDateTime(_startDate);
            DateTime endDate = Convert.ToDateTime(_endDate);

            cmd.Connection = dc.Connect();

            if(cmd.Connection.State == ConnectionState.Open)
            {
                try
                {
                    cmd.CommandText = query;

                    cmd.Parameters.AddWithValue("@startDate", startDate.ToString("yyyy-MM-dd"));
                    cmd.Parameters.AddWithValue("@endDate", endDate.ToString("yyyy-MM-dd"));

                    total = Convert.ToInt32(cmd.ExecuteScalar());
                }
                catch(Exception ex)
                {
                    MessageBox.Show("Something went wrong in computing range income: " + ex.Message);
                }
            }

            cmd.Parameters.Clear();
            cmd.Dispose();
            dc.Close();

            return total;
        }

    }
}
