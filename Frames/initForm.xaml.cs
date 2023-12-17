using System.Windows.Controls;

namespace VerseVox.Frames
{
    /// <summary>
    /// Логика взаимодействия для initForm.xaml
    /// </summary>
    public partial class initForm : UserControl
    {
        public initForm()
        {
            InitializeComponent();
        }

        private void CheckAuth(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Queries.LoginQueries loginQueries = new Queries.LoginQueries();
            loginQueries.CheckLogin(LoginBox.Text, PasswordBox.Password);
            if(Scripts.NonStaticVariables.successLogin)
            {
                (this.Parent as DockPanel).Children.Remove(this);
            }
        }

        private void openRegWindow(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Scripts.NonStaticVariables.windowId = 2;
            (this.Parent as DockPanel).Children.Remove(this);
        }
    }
}
