using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace VerseVox.Frames
{
    /// <summary>
    /// Логика взаимодействия для FriendList.xaml
    /// </summary>

    public partial class FriendList : UserControl
    {
        public static int ListID = 1;

        Point cursorPos;
        private DateTime clickTime;
        private bool isDoubleClick;

        DispatcherTimer updateStatus = new DispatcherTimer();
        public FriendList()
        {
            InitializeComponent();
            ChangeList();
            updateStatus.Tick += new EventHandler(List_Tick);
            updateStatus.Interval = new TimeSpan(0, 0, 5);
            updateStatus.Start();
        }
        private void List_Tick(object sender, EventArgs e)
        {
            ChangeList();
        }

        public void ChangeList()
        {
            updateStatus.Start();
            searchBar.Visibility = Visibility.Hidden;
            switch (ListID)
            {
                case 1:
                    userSearch.Text = "";
                    PageName.Content = "Список друзей";
                    string Stat = "Gray";
                    listView.ItemsSource = null;
                    ObservableCollection<Friend> frList = new ObservableCollection<Friend>();
                    List<string> frStrList = new Queries.ListQueries().GetFriends();
                    for (int i = 0; i < frStrList.Count - 3; i += 4)
                    {
                        switch (int.Parse(frStrList[i + 2]))
                        {
                            case 0:
                                Stat = "Gray";
                                break;
                            case 1:
                                Stat = "Green";
                                break;
                            case 2:
                                Stat = "Yellow";
                                break;
                        }
                        frList.Add(new Friend { Avatar = "avatar_example.png", Nickname = frStrList[i], Status = Stat, Activity = frStrList[i + 3], Id = frStrList[i + 1] });
                    }
                    listView.ItemsSource = frList;
                    break;
                case 2:
                    userSearch.Text = "";
                    PageName.Content = "Запросы в друзья";
                    listView.ItemsSource = null;
                    ObservableCollection<Friend> invites = new ObservableCollection<Friend>();
                    List<string> userInviteList = new Queries.ListQueries().GetInvites();
                    for (int i = 0; i < userInviteList.Count - 1; i += 2)
                        invites.Add(new Friend { Avatar = "avatar_example.png", Nickname = userInviteList[i], Id = userInviteList[i + 1] });
                    listView.ItemsSource = invites;
                    break;
                case 3:
                    updateStatus.Stop();
                    PageName.Content = "Поиск пользователей";
                    listView.ItemsSource = null;
                    searchBar.Visibility = Visibility.Visible;
                    break;
            }
        }

        private void showInvites(object sender, MouseButtonEventArgs e)
        {
            ListID = 2;
            ChangeList();
        }

        private void CursorWatcher(object sender, MouseEventArgs e)
        {
            cursorPos = e.GetPosition(this);
        }

        private void rightButtonCheck(object sender, MouseButtonEventArgs e)
        {
            if (listView.SelectedItem != null)
            {
                ListViewItem item = (ListViewItem)listView.ItemContainerGenerator.ContainerFromItem(listView.SelectedItem);
                Friend inviteInfo = (Friend)listView.SelectedItem;

                Scripts.NonStaticVariables.selectedUserId = int.Parse(inviteInfo.Id);
                if (item != null)
                {
                    switch (ListID)
                    {
                        case 1:
                            FriendDialog.Visibility = Visibility.Visible;
                            break;
                        case 2:
                            InviteDialog.Visibility = Visibility.Visible;
                            break;
                        case 3:
                            sendDialog.Visibility = Visibility.Visible;
                            break;
                    }
                    InviteDialog.Margin = new Thickness(32, cursorPos.Y - 45, 0, 0);
                }
            }

        }

        private void leftButtonCheck(object sender, MouseButtonEventArgs e)
        {
            updateStatus.Stop(); updateStatus.Start();
            if ((DateTime.Now - clickTime).TotalMilliseconds <= 500)
            {

                isDoubleClick = true;
            }
            else
            {

                isDoubleClick = false;
            }

            clickTime = DateTime.Now;
        }

        private void ApplyRequest(object sender, MouseButtonEventArgs e)
        {
            InviteDialog.Visibility = Visibility.Hidden;
            new Queries.ListQueries().applyRequest();
            Scripts.NonStaticVariables.selectedUserId = 0;
            ChangeList();
        }

        private void ShowFriendList(object sender, MouseButtonEventArgs e)
        {
            ListID = 1;
            ChangeList();
        }

        private void RejectRequest(object sender, MouseButtonEventArgs e)
        {
            InviteDialog.Visibility = Visibility.Hidden;
            new Queries.ListQueries().rejectRequest();
            Scripts.NonStaticVariables.selectedUserId = 0;
            ChangeList();
        }

        private void searchFunc(object sender, MouseButtonEventArgs e)
        {
            ListID = 3;
            ChangeList();
        }

        private void startSearch(object sender, TextChangedEventArgs e)
        {
            listView.ItemsSource = null;
            ObservableCollection<Friend> users = new ObservableCollection<Friend>();
            List<string> usersList = new Queries.ListQueries().SearchFriend(userSearch.Text);
            for (int i = 0; i < usersList.Count - 1; i += 2)
                users.Add(new Friend { Avatar = "avatar_example.png", Nickname = usersList[i], Id = usersList[i + 1] });
            listView.ItemsSource = users;
        }

        private void sendRequest(object sender, MouseButtonEventArgs e)
        {
            sendDialog.Visibility = Visibility.Hidden;
            new Queries.ListQueries().applyRequest();
            Scripts.NonStaticVariables.selectedUserId = 0;
            ChangeList();
        }

        private void deleteFriend(object sender, MouseButtonEventArgs e)
        {
            FriendDialog.Visibility = Visibility.Hidden;
            new Queries.ListQueries().deleteRequest();
            Scripts.NonStaticVariables.selectedUserId = 0;
            ChangeList();
        }

        private void openChatHistory(object sender, MouseButtonEventArgs e)
        {
            FriendDialog.Visibility = Visibility.Hidden;
            Scripts.NonStaticVariables.ChatID = Scripts.NonStaticVariables.selectedUserId;
            Scripts.NonStaticVariables.leftHalfWindowID = 1;
            Scripts.NonStaticVariables.selectedUserId = 0;
            ChangeList();
        }

        private void leftDoubleCheck(object sender, MouseButtonEventArgs e)
        {
            if (isDoubleClick)
            {
                if (listView.SelectedItem != null && ListID == 1)
                {
                    ListViewItem item = (ListViewItem)listView.ItemContainerGenerator.ContainerFromItem(listView.SelectedItem);
                    Friend inviteInfo = (Friend)listView.SelectedItem;
                    if(item != null)
                    {
                        Scripts.NonStaticVariables.ChatID = int.Parse(inviteInfo.Id);
                        sendDialog.Visibility = Visibility.Hidden;
                        FriendDialog.Visibility = Visibility.Hidden;
                        InviteDialog.Visibility = Visibility.Hidden;
                        Scripts.NonStaticVariables.leftHalfWindowID = 1;
                        Scripts.NonStaticVariables.selectedUserId = 0;
                        ChangeList();
                    }


                }

            }
            else
            {
                sendDialog.Visibility = Visibility.Hidden;
                FriendDialog.Visibility = Visibility.Hidden;
                InviteDialog.Visibility = Visibility.Hidden;
                Scripts.NonStaticVariables.selectedUserId = 0;
            }
        }
    }
}
