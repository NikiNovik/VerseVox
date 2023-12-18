using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace VerseVox.Frames
{
    /// <summary>
    /// Логика взаимодействия для ChatPanel.xaml
    /// </summary>
    public partial class ChatPanel : UserControl
    {
        DispatcherTimer MessageTick = new DispatcherTimer();
        private int MessagesCount = 0;
        private bool callEnabled = false;
        public ChatPanel()
        {
            InitializeComponent();
            LoadMessageHistory();
            GetUserChatStatus();
            MessageTick.Tick += new EventHandler(MessageList_Tick);
            MessageTick.Interval = new TimeSpan(0, 0, 5);
            MessageTick.Start();
        }
        private void MessageList_Tick(object sender, EventArgs e)
        {
            if(new Queries.ChatQueries().CheckFriendStatus().Count > 0)
            {
                LoadMessageHistory();
                GetUserChatStatus();
            }
            else
            {
                try
                {
                    (this.Parent as DockPanel).Children.Remove(this);
                    Scripts.NonStaticVariables.leftHalfWindowID = 0;
                }
                catch (Exception ex) { }
            }

        }
        public void LoadMessageHistory()
        {
            listView.ItemsSource = null;
            ObservableCollection<Message> messages = new ObservableCollection<Message>();
            List<string> messagelist = new Queries.ChatQueries().GetMessageHistory();
            for (int i = 0; i < messagelist.Count - 1; i += 2)
                messages.Add(new Message { Avatar = "avatar_example.png", Nickname = messagelist[i], MessageInfo = messagelist[i + 1] });
            listView.ItemsSource = messages;
            if (listView.Items.Count > MessagesCount)
            {
                MessagesCount = listView.Items.Count;
                object item = listView.Items[listView.Items.Count - 1];
                listView.ScrollIntoView(item);
            }
        }

        public void GetUserChatStatus()
        {
            SolidColorBrush solidColor = new SolidColorBrush();
            List<string> chatinfo = new Queries.ChatQueries().GetChatStatus();
            Username.Content = chatinfo[0];
            solidColor.Color = StatusColor(int.Parse(chatinfo[1]));
            Status.Fill = solidColor;
            checkCallStatus(int.Parse(chatinfo[1]));
            Activity.Content = chatinfo[2];
        }
        public void checkCallStatus(int CallStatus)
        {
            SolidColorBrush solidColor = new SolidColorBrush();
            switch (CallStatus)
            {
                
                case 0:
                    if (Scripts.NonStaticVariables.isCalling)
                    {
                        new Queries.ChatQueries().EndCallRequest();
                        Scripts.NonStaticVariables.isCalling = false;

                    }
                    callEnabled = false;
                    solidColor.Color = Colors.Gray;
                    callButton.Fill = solidColor;
                    break;
                case 1:
                    if(new Queries.ChatQueries().GetCallStatus() != 2)
                    {
                        Scripts.NonStaticVariables.isCalling = false;
                    }
                    callEnabled = true;
                    solidColor.Color = Colors.Green;
                    callButton.Fill = solidColor;
                    if(new Queries.ChatQueries().GetCallStatus() == 2)
                    {
                        callEnabled = false;
                        solidColor.Color = Colors.Cyan;
                        callButton.Fill = solidColor;
                        if(!Scripts.NonStaticVariables.isCalling && !Scripts.NonStaticVariables.isConnected)
                        {
                            Scripts.NonStaticVariables.clientChannel = new Queries.ChatQueries().getChannelID();
                            Scripts.NonStaticVariables.isCalling = true;
                        }
                    }
                    else if (new Queries.ChatQueries().GetCallStatus() == 1)
                    {
                        callEnabled = false;
                        solidColor.Color = Colors.Yellow;
                        callButton.Fill = solidColor;
                    }
                    break;
            }
        }
        public Color StatusColor(int status)
        {
            switch (status)
            {
                case 0:
                    return Colors.Gray;
                case 1:
                    return Colors.Green;
                case 2:
                    return Colors.Yellow;
                default:
                    return Colors.Transparent;
            }
        }

        private void CallToUser(object sender, MouseButtonEventArgs e)
        {
            if (callEnabled)
            {
                if (!Scripts.NonStaticVariables.isConnected)
                    new Queries.ChatQueries().SendCallRequest();
                else
                    System.Windows.MessageBox.Show("Вы уже подключены к другому пользователю");
            }
            else
            {
                if(new Queries.ChatQueries().GetCallStatus() == 1)
                {
                    if(new Queries.ChatQueries().checkWhoSender())
                    {
                        new Queries.ChatQueries().EndCallRequest();
                        Scripts.NonStaticVariables.isCalling = false;
                    }
                    else
                    {
                        new Queries.ChatQueries().GetCallRequest();
                    }
                }
                else if(new Queries.ChatQueries().GetCallStatus() == 2)
                {
                    new Queries.ChatQueries().EndCallRequest();
                    Scripts.NonStaticVariables.isCalling = false;
                }
            }
        }

        private void sendMessage(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (MessageField.Text.Length > 0)
                {
                    new Queries.ChatQueries().SendMessageToUser(MessageField.Text);
                    MessageField.Text = "";
                    LoadMessageHistory();
                }
            }
        }
    }
}
