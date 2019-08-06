using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;

namespace POS
{
    class DataConnection
    {
        private string server;
        private string database;
        private string userID;
        private string password;

        private string connstring;
        private SqlConnection conn = null;
        private SqlConnectionStringBuilder c_sqlb = new SqlConnectionStringBuilder();

        public string ConnectionString { get { return this.connstring; } }
        public string Server { get { return this.server; } set { this.server = value; c_sqlb["Server"] = this.server; connstring = c_sqlb.ConnectionString; } }
        public string Database { get { return this.database; } set { this.database = value; c_sqlb["Database"] = this.database; connstring = c_sqlb.ConnectionString; } }
        public string UserID { get { return this.userID; } set { this.userID = value; c_sqlb["User ID"] = this.userID; connstring = c_sqlb.ConnectionString; } }
        public string Password { get { return this.password; } set { this.Password = value; c_sqlb["Password"] = this.password; connstring = c_sqlb.ConnectionString; } }


        public DataConnection(string server, string database, string userId, string password)
        {
            this.Server = server;
            this.Database = database;
            this.UserID = userId;
            this.password = password;
            c_sqlb.Clear();
            c_sqlb["Server"] = this.Server;
            c_sqlb["Database"] = this.Database;
            c_sqlb["User ID"] = this.UserID;
            c_sqlb["Password"] = this.Password;
            connstring = c_sqlb.ConnectionString;
            conn = new SqlConnection(connstring);
        }


        public SqlConnection Connect()
        {
            try
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.ConnectionString = this.connstring;
                    conn.Open();
                    //this.OnConnected(new ErrMessageArgs("Connection Successful"));
                }
                else
                {
                    conn.Close();
                    conn.ConnectionString = this.connstring;
                    conn.Open();
                    //this.OnConnected(new ErrMessageArgs("Connection Successful"));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in opening Connection: " + ex.Message);
            }

            return conn;
        }


        public void Close()
        {
            try
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in closing Connection: " + ex.Message);
            }
        }

    }
}
