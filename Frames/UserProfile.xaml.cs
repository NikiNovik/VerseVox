using Hardcodet.Wpf.TaskbarNotification;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace VerseVox.Frames
{
    /// <summary>
    /// Логика взаимодействия для UserProfile.xaml
    /// </summary>
    public partial class UserProfile : UserControl
    {
        Scripts.VoiceModule voIP = new Scripts.VoiceModule();
        DispatcherTimer updateStatus = new DispatcherTimer();
        public int MessagesCount = 0;
        public int ChatID = 0;

        public UserProfile()
        {
            InitializeComponent();

            //При инициализации модуля профиля выполняем отправку сигнала о том что мы вошли в сеть
            if (Scripts.GlobalVariables.enableActivity) Scripts.GlobalVariables.activity = Scripts.ProcessListener.GetActiveProcessName();
            Username.Content = Scripts.GlobalVariables.username; Scripts.GlobalVariables.status = 1;
            UserActivity.Content = Scripts.GlobalVariables.activity;
            new Queries.ProfileQueries().updateStatus();
            GetMessagesInfo();
            userStatus();

            updateStatus.Tick += new EventHandler(userStatusUpdater);
            updateStatus.Interval = new TimeSpan(0, 0, 5);
            updateStatus.Start();
        }

        private void userStatusUpdater(object sender, EventArgs e)
        {
            if (Scripts.GlobalVariables.enableActivity) Scripts.GlobalVariables.activity = Scripts.ProcessListener.GetActiveProcessName();
            UserActivity.Content = Scripts.GlobalVariables.activity;
            new Queries.ProfileQueries().updateStatus();
            userStatus();
            CheckMessages();
            CheckCall();


        }
        public void userStatus()
        {
            SolidColorBrush solidColor = new SolidColorBrush();
            switch (Scripts.GlobalVariables.status)
            {
                case 0:
                    solidColor.Color = Colors.Green;
                    break;
                case 1:
                    solidColor.Color = Colors.Green;
                    break;
                case 2:
                    solidColor.Color = Colors.Yellow;
                    break;
            }
            UserStatus.Fill = solidColor;
        }

        public void GetMessagesInfo()
        {
            MessagesCount = new Queries.ProfileQueries().GetMessageNotify().Count;
        }
        public void CheckMessages()
        {
            if (Scripts.GlobalVariables.enableNotifications && Scripts.NonStaticVariables.hiddenWindow)
            {
                if (new Queries.ProfileQueries().GetMessageNotify().Count > MessagesCount)
                {
                    GetMessagesInfo();
                    List<string> messageinfo = new Queries.ProfileQueries().GetMessageNotify();
                    ChatID = int.Parse(messageinfo[0]);
                    NotifyModule.ShowBalloonTip(messageinfo[1], messageinfo[2], BalloonIcon.None);
                }
            }
        }

        private void ActivityRecorder(object sender, MouseButtonEventArgs e)
        {
            SolidColorBrush solidColor = new SolidColorBrush();
            if (Scripts.GlobalVariables.enableActivity)
            {
                solidColor.Color = Colors.Red;
                ActivityRecorderButton.Fill = solidColor;
                Scripts.GlobalVariables.enableActivity = false;
            }
            else
            {
                solidColor.Color = Colors.Green;
                ActivityRecorderButton.Fill = solidColor;
                Scripts.GlobalVariables.enableActivity = true;
            }
        }
        private void Notification(object sender, MouseButtonEventArgs e)
        {
            SolidColorBrush solidColor = new SolidColorBrush();
            if (Scripts.GlobalVariables.enableNotifications)
            {
                solidColor.Color = Colors.Gray;
                notifyIcon.Source = new System.Windows.Media.Imaging.BitmapImage(new Uri("pack://application:,,,/Icons/NotifyOff.png"));
                Scripts.GlobalVariables.status = 2;
                StatusRecorder.Fill = solidColor;
                Scripts.GlobalVariables.enableNotifications = false;
            }
            else
            {
                solidColor.Color = Colors.Yellow;
                notifyIcon.Source = new System.Windows.Media.Imaging.BitmapImage(new Uri("pack://application:,,,/Icons/NotifyOn.png"));
                Scripts.GlobalVariables.status = 1;
                StatusRecorder.Fill = solidColor;
                GetMessagesInfo();
                Scripts.GlobalVariables.enableNotifications = true;
            }
        }

        private void ShowMessage(object sender, RoutedEventArgs e)
        {
            Scripts.NonStaticVariables.leftHalfWindowID = 101;
            Scripts.NonStaticVariables.ChatID = ChatID;
        }

        private void CheckCall()
        {
            if (Scripts.NonStaticVariables.isCalling && !Scripts.NonStaticVariables.isConnected)
            {

                voIP.EnterProtoVoiceChat();
            }
            if (Scripts.NonStaticVariables.clientChannel > 0 && !Scripts.NonStaticVariables.isCalling && Scripts.NonStaticVariables.isConnected)
            {
                MessageBox.Show("Disconnected");
                voIP.ExitProtoVoiceChat();
            }

        }
    }
}
