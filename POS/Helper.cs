using System;
using System.Linq;
using System.Windows.Controls;

namespace POS
{ 
    class Helper
    {
        public static bool CheckInsertItemInputs(Panel selectedPanel)
        {
            foreach (var panel in selectedPanel.Children.OfType<Panel>())
            {
                if (panel is Panel)
                {
                    foreach (var control in panel.Children.OfType<Control>())
                    {
                        if (control is TextBox)
                        {
                            TextBox txtbox = control as TextBox;
                            if (String.IsNullOrWhiteSpace(txtbox.Text))
                            {
                                return false;
                            }
                        }
                    }
                }
            }

            return true;
        }


        public static void ClearInsertItemInputs(Panel selectedPanel)
        {
            foreach (var panel in selectedPanel.Children.OfType<Panel>())
            {
                if (panel is Panel)
                {
                    foreach (var control in panel.Children.OfType<Control>())
                    {
                        if (control is TextBox)
                        {
                            TextBox txtBox = control as TextBox;

                            txtBox.Clear();
                        }
                    }
                }
            }
        }


        public static bool CheckInsertUserInputs(Panel selectedPanel, RadioButton rbUser, RadioButton rbAdmin)
        {
            foreach (var panel in selectedPanel.Children.OfType<Panel>())
            {
                if (panel is Panel)
                {
                    foreach (var control in panel.Children.OfType<Control>())
                    {
                        if (control is TextBox)
                        {
                            TextBox textbox = control as TextBox;
                            if (String.IsNullOrWhiteSpace(textbox.Text))
                            {
                                return false;
                            }
                        }
                        else if (control is PasswordBox)
                        {
                            PasswordBox passbox = control as PasswordBox;
                            if (String.IsNullOrWhiteSpace(passbox.Password))
                            {
                                return false;
                            }
                        }
                    }
                }
            }

            if (rbUser.IsChecked == false && rbAdmin.IsChecked == false)
            {
                return false;
            }

            return true;
        }


        public static void ClearInsertUserInputs(Panel selectedPanel)
        {
            foreach (var panel in selectedPanel.Children.OfType<Panel>())
            {
                if (panel is Panel)
                {
                    foreach (var control in panel.Children.OfType<Control>())
                    {
                        if (control is TextBox)
                        {
                            TextBox textbox = control as TextBox;
                            textbox.Clear();
                        }
                        else if (control is PasswordBox)
                        {
                            PasswordBox passbox = control as PasswordBox;
                            passbox.Clear();
                        }
                        else if (control is RadioButton)
                        {
                            RadioButton radio = control as RadioButton;
                            radio.IsChecked = false;
                        }
                    }
                }
            }
        }
    }
}
