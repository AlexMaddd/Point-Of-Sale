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
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        public Login()
        {
            InitializeComponent();

            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            string queryCheckCredentials = string.Format("SELECT users.emp_id AS EMP_ID, " +
                                            "users.role_id as ROLE_ID, " +
                                            "users.password AS PASSWORD, " +
                                            "users_information.status AS STATUS " +
                                            "FROM users LEFT JOIN users_information " +
                                            "ON users.emp_id = users_information.emp_id " +
                                            "WHERE users.emp_id = @empID AND users_information.emp_id = @empID " +
                                            "AND users.password = @password");

            string queryLogInRecord = string.Format("INSERT INTO timelog" +
                                            "(emp_id, log_time, log_status) " +
                                            "VALUES" +
                                            "(@emp_id, @log_time, @log_status)");

            try
            {
                if(SqlQueries.Login(queryCheckCredentials, Convert.ToInt32(txtEmpID.Text), txtPassword.Password) == true)
                {
                    if(SqlQueries.Logger(queryLogInRecord, Globals.Emp_ID, btnLogin.Content.ToString()))
                    {
                        MessageBox.Show("Logged in");

                        MainWindow mw = new MainWindow();

                        this.Close();
                        mw.ShowDialog();
                    }
                }
                else
                {
                    txtEmpID.Clear();
                    txtPassword.Clear();
                }
            }
            catch(Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }
    }
}
