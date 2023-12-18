using System;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Input;

namespace VerseVox.Frames
{
    /// <summary>
    /// Логика взаимодействия для regForm.xaml
    /// </summary>
    public partial class regForm : UserControl
    {
        public regForm()
        {
            InitializeComponent();
        }

        private void sendRegRequest(object sender, MouseButtonEventArgs e)
        {
            if (IsValidEmail())
            {
                if (IsValidPassword())
                {
                    new Queries.registrationQueries().sendRegistationRequest(username.Text, usermail.Text, userpass.Password);
                    if(Scripts.NonStaticVariables.successRegister)
                    {
                        System.Windows.MessageBox.Show("Регистрация успешна!");
                        Scripts.NonStaticVariables.windowId = 1;
                        (this.Parent as DockPanel).Children.Remove(this);
                    }
                }
                else System.Windows.MessageBox.Show("Неверно указан пароль или несоответствие паролей");
            }
            else System.Windows.MessageBox.Show("Неверно указана почта");
        }


        public bool IsValidEmail()
        {
            if (string.IsNullOrWhiteSpace(usermail.Text))
                return false;

            try
            {
                // Простая проверка на соответствие общему шаблону
                return Regex.IsMatch(usermail.Text,
                    @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
                    RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
        }

        public bool IsValidPassword()
        {
            if (string.IsNullOrWhiteSpace(userpass.Password))
                return false;

            if (userpass_confirm.Password == userpass.Password)
                return true;
            else
                return false;
        }
    }
}
