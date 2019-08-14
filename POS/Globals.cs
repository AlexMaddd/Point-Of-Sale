using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS
{
    class Globals
    {
        private static int emp_id;
        private static int role_id;
        private static string emp_name;

        private static readonly string server = "localhost";
        private static readonly string database = "pos";
        private static readonly string userID = "root";
        private static readonly string password = "password";

        public static int Emp_ID
        {
            get
            {
                return emp_id;
            }
            set
            {
                emp_id = value;
            }
        }

        public static int Role_ID
        {
            get
            {
                return role_id;
            }
            set
            {
                role_id = value;
            }
        }


        public static string Emp_Name
        {
            get
            {
                return emp_name;
            }
            set
            {
                emp_name = value;
            }
        }


        // returns SqlConnection instance
        // credentials passed into constructor creates ConnectionString 
        public static DataConnection DbConnect()
        {
            DataConnection dbConnect = new DataConnection(server, database, userID, password);
            return dbConnect;
        }

    }
}
