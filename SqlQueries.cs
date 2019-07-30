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

        public static bool SqlExecNQInsertTransactionsTable(string query, int cartTotal, int emp_id, string orderType)
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


        public static bool InsertIntoUsersTable(string query, int emp_id, string fname, string lname)
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


        public static bool InsertIntoUserDetailsTable(string query, int role_id, string password)
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

                    if(!DBNull.Value.Equals(cmd.ExecuteScalar()))
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

    }
}
