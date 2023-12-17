using System.Windows.Controls;

namespace VerseVox.Frames
{
    /// <summary>
    /// Логика взаимодействия для FatalError.xaml
    /// </summary>
    public partial class FatalError : UserControl
    {
        public FatalError()
        {
            InitializeComponent();
            ErrorDialog.Content = Scripts.NonStaticVariables.errorMessage;
        }

        private void ReconnectDB(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            (this.Parent as DockPanel).Children.Remove(this);
            Scripts.NonStaticVariables.windowId = 1000;
        }
    }
}
