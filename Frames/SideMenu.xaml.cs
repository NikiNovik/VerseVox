using System;
using System.Windows.Controls;
using System.Windows.Threading;

namespace VerseVox.Frames
{
    /// <summary>
    /// Логика взаимодействия для SideMenu.xaml
    /// </summary>
    public partial class SideMenu : UserControl
    {
        public SideMenu()
        {
            InitializeComponent();
            if (Scripts.NonStaticVariables.successLogin)
            {
                Scripts.NonStaticVariables.downWindowID = 1;
                Scripts.NonStaticVariables.HalfWindowID = 1;
            }
            //Нижняя панель
            DispatcherTimer downWindow = new DispatcherTimer();
            downWindow.Tick += new EventHandler(downWindow_tick);
            downWindow.Interval = new TimeSpan(0, 0, 1);
            downWindow.Start();

            //Половинная панель
            DispatcherTimer halfWindow = new DispatcherTimer();
            halfWindow.Tick += new EventHandler(halfWindow_tick);
            halfWindow.Interval = new TimeSpan(0, 0, 1);
            halfWindow.Start();
        }
        private void downWindow_tick(object sender, EventArgs e)
        {
            switch (Scripts.NonStaticVariables.downWindowID)
            {
                case 1:
                    UserProfile obj = new UserProfile();
                    DownSide.Children.Add(obj);
                    Scripts.NonStaticVariables.downWindowID = 0;
                    break;
            }
        }
        private void halfWindow_tick(object sender, EventArgs e)
        {
            switch (Scripts.NonStaticVariables.HalfWindowID)
            {
                case 1:
                    FriendList obj = new FriendList();
                    HalfSide.Children.Add(obj);
                    Scripts.NonStaticVariables.HalfWindowID = 0;
                    break;
            }
        }
    }
}
