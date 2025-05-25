using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using 空运系统.Models;

namespace 空运系统.pages
{
    public partial class 用户管理 : Page
    {
        private readonly DatabaseHelper _db = new DatabaseHelper();
        private DataRowView _selectedUser;
        private bool _isEditing = false;

        public 用户管理()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            RefreshUserList();
            ClearInputs();
        }

        private void RefreshUserList()
        {
            UsersDataGrid.ItemsSource = _db.GetAllUsers().DefaultView;
          ClearSelection();
        }

        private void ClearSelection()
        {
            UsersDataGrid.UnselectAll();
            DeleteButton.IsEnabled = false;
            _selectedUser = null;
        }

        private void ClearInputs()
        {
            UsernameTextBox.Text = "";
            PasswordBox.Password = "";
            _isEditing = false;
            SaveButton.Content = "保存";
        }

        private void UsersDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedUser = UsersDataGrid.SelectedItem as DataRowView;
            DeleteButton.IsEnabled = _selectedUser != null;

            if (_selectedUser != null)
            {
                UsernameTextBox.Text = _selectedUser["用户名"].ToString();
                PasswordBox.Password = _selectedUser["密码"].ToString();
                _isEditing = true;
                SaveButton.Content = "更新";
            }
        }

        private void NewButton_Click(object sender, RoutedEventArgs e)
        {
            ClearInputs();
            UsernameTextBox.Focus();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text.Trim();
            string password = PasswordBox.Password;

            if (string.IsNullOrWhiteSpace(username))
            {
                MessageBox.Show("请输入用户名", "错误", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("请输入密码", "错误", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            bool success;
            if (_isEditing && _selectedUser != null)
            {
                string originalUsername = _selectedUser["用户名"].ToString();
                success = _db.UpdateUser(originalUsername, username, password);
            }
            else
            {
                success = _db.AddUser(username, password);
            }

            if (success)
            {
                MessageBox.Show("操作成功", "成功", MessageBoxButton.OK, MessageBoxImage.Information);
                RefreshUserList();
                ClearInputs();
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedUser == null) return;

            string username = _selectedUser["用户名"].ToString();
            var result = MessageBox.Show($"确定要删除用户 '{username}' 吗?", "确认删除",
                MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                if (_db.DeleteUser(username))
                {
                    MessageBox.Show("用户删除成功", "成功", MessageBoxButton.OK, MessageBoxImage.Information);
                    RefreshUserList();
                    ClearInputs();
                }
            }
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            RefreshUserList();
            ClearInputs();
        }
    }
}