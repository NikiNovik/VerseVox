using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace VerseVox
{

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            //Временная проверка
            DispatcherTimer mwc = new DispatcherTimer();
            mwc.Tick += new EventHandler(mainwindow_check);
            mwc.Interval = new TimeSpan(0, 0, 1);
            mwc.Start();

            DispatcherTimer leftside = new DispatcherTimer();
            leftside.Tick += new EventHandler(leftside_check);
            leftside.Interval = new TimeSpan(0, 0, 1);
            leftside.Start();
        }

        private void mainwindow_check(object sender, EventArgs e)
        {

            switch (Scripts.NonStaticVariables.windowId)
            {
                case 1:
                    Frames.initForm obj = new Frames.initForm();
                    DeskPanel.Children.Add(obj);
                    Scripts.NonStaticVariables.windowId = 0;
                    break;
                case 2:
                    Frames.regForm obj2 = new Frames.regForm();
                    DeskPanel.Children.Add(obj2);
                    Scripts.NonStaticVariables.windowId = 0;
                    break;
                case 50:
                    SideBar.Visibility = Visibility.Visible;
                    Frames.SideMenu obj50 = new Frames.SideMenu();
                    RightSide.Children.Add(obj50);
                    Scripts.NonStaticVariables.windowId = 0;
                    break;
                case 404:
                    Scripts.NonStaticVariables.errorMessage = "Ошибка подключения к серверу";
                    Frames.FatalError obj404 = new Frames.FatalError();
                    DeskPanel.Children.Add(obj404);
                    Scripts.NonStaticVariables.windowId = 0;
                    break;
                case 1000:
                    Queries.ConnectionQueries connectionQueries = new Queries.ConnectionQueries();
                    connectionQueries.CheckConnectionState();
                    break;
            }

            if (WindowState == WindowState.Minimized)
                Scripts.NonStaticVariables.hiddenWindow = true;
            else Scripts.NonStaticVariables.hiddenWindow = false;
        }

        private void leftside_check(object sender, EventArgs e)
        {

            switch (Scripts.NonStaticVariables.leftHalfWindowID)
            {
                case 1:
                    Frames.ChatPanel objchat = new Frames.ChatPanel();
                    LeftSide.Children.Add(objchat);
                    Scripts.NonStaticVariables.leftHalfWindowID = 0;
                    break;
                case 101:
                    WindowState = WindowState.Normal;
                    this.Activate();
                    Frames.ChatPanel objchatNotify = new Frames.ChatPanel();
                    LeftSide.Children.Add(objchatNotify);
                    Scripts.NonStaticVariables.leftHalfWindowID = 0;
                    break;
            }
        }

        private void CloseApplication(object sender, MouseButtonEventArgs e)
        {
            WindowState = WindowState.Minimized;
            Scripts.GlobalVariables.activity = "";
            Scripts.GlobalVariables.status = 0;
            Queries.ProfileQueries profileQueries = new Queries.ProfileQueries();
            profileQueries.updateStatus();
            Environment.Exit(0);
        }

        private void MaximizeApplication(object sender, MouseButtonEventArgs e)
        {
            if (WindowState != WindowState.Maximized)
                WindowState = WindowState.Maximized;
            else WindowState = WindowState.Normal;
        }

        private void MinimizeApplication(object sender, MouseButtonEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void DragWindow(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
            e.Handled = true;
        }
    }
}
