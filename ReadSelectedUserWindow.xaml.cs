using System;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;


namespace POS
{
    /// <summary>
    /// Interaction logic for ReadSelectedUserWindow.xaml
    /// </summary>
    public partial class ReadSelectedUserWindow : Window
    {
        private DataRowView row;
        private MainWindow main;

        private bool controlsEnabled = false;

        public ReadSelectedUserWindow(DataRowView _row, MainWindow _main)
        {
            InitializeComponent();

            main = _main;

            DataContext = _row;

            row = _row;

            CheckRole();
        }

        public void CheckRole()
        {
            //MessageBox.Show(row["Role"].ToString());

            if(row["Role"].ToString() == "users")
            {
                isRbUser.IsChecked = true;
            }
            else
            {
                isRbAdmin.IsChecked = true;
            }
        }


        public bool EnableControls()
        {
            foreach(var control in spSelectedUserWindow.Children.OfType<Control>())
            {
                if(control.Name != "txtEmpID")
                {
                    control.IsEnabled = true;
                    control.Focusable = true;
                    control.IsHitTestVisible = true;
                }
            }

            return true;
        }


        public bool DisableControls()
        {
            foreach (var control in spSelectedUserWindow.Children.OfType<Control>())
            {
                if (control.Name != "txtEmpID" && control.Name != "btnBack")
                {
                    control.IsEnabled = false;
                    control.Focusable = false;
                    control.IsHitTestVisible = false;
                }
            }
            return false;
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if(controlsEnabled == false)
            {
                controlsEnabled = EnableControls();

                btnEdit.Content = "Save Changes";
                btnBack.Content = "Cancel";
            }
            else
            {
                MessageBoxResult result = MessageBox.Show("Save Changes?", "Confirmation", MessageBoxButton.YesNo);

                if(result == MessageBoxResult.Yes)
                {
                    int role;

                    string queryEditUserData = string.Format(
                                                "BEGIN TRAN; " +
                                                "UPDATE users_information " +
                                                "SET " +
                                                "fname = @fname, " +
                                                "lname = @lname " +
                                                "WHERE " +
                                                "emp_id = @emp_id; " +
                                                "UPDATE users " +
                                                "SET " +
                                                "role_id = @role_id " +
                                                "WHERE " +
                                                "emp_id = @emp_id; " +
                                                "COMMIT;");

                    if (isRbAdmin.IsChecked == true)
                    {
                        role = 1;
                    }
                    else
                    {
                        role = 2;
                    }

                    if (SqlQueries.EditUserData(queryEditUserData, Convert.ToInt32(txtEmpID.Text), role, txtFName.Text, txtLName.Text) == true)
                    {
                        MessageBox.Show("Edit Complete");

                        main.Load_ActiveUsers();

                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Something went wrong");
                    }
                }

            }
    
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure?", "Confirmation", MessageBoxButton.YesNo);

            if(result == MessageBoxResult.Yes)
            {
                string queryDeactivateUser = string.Format("UPDATE users_information " +
                                            "SET status = 'Inactive' " +
                                            "WHERE emp_id = @emp_id");

                if(SqlQueries.DeactivateUser(queryDeactivateUser, Convert.ToInt32(row["Emp_ID"])) == true)
                {
                    MessageBox.Show("User Deactivated");

                    main.Load_ActiveUsers();

                    this.Close();
                }
                else
                {
                    MessageBox.Show("Something went wrong");
                }
            }
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            if (controlsEnabled == true)
            {
                controlsEnabled = DisableControls();
                btnEdit.Content = "Edit User";
            }
            else
            {
                MessageBoxResult result = MessageBox.Show("Changes could have been made", "Confirmation", MessageBoxButton.YesNo);

                if (result == MessageBoxResult.Yes)
                {
                    this.Close();
                }
            }
        }
    }
}
