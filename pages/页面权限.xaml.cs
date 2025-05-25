using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using 空运系统.Models; // 假设有一个DatabaseHelper类用于数据库操作
namespace 空运系统.pages
{
    public partial class 页面权限 : Page
    {
        private readonly DatabaseHelper _db = new DatabaseHelper();
        private DataRowView _selectedPermission;
        private bool _isEditing = false;

        public 页面权限()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            RefreshPermissionList();
            ClearInputs();
        }

        private void RefreshPermissionList()
        {
            PermissionDataGrid.ItemsSource = _db.GetAllPermissions().DefaultView;
            ClearSelection();
        }

        private void ClearSelection()
        {
            PermissionDataGrid.UnselectAll();
            DeleteButton.IsEnabled = false;
            _selectedPermission = null;
        }

        private void ClearInputs()
        {
            UsernameTextBox.Text = "";
            PageNameTextBox.Text = "";
            PagePermissionTextBox.Text = "";
            ControlNameTextBox.Text = "";
            ControlPermissionTextBox.Text = "";
            _isEditing = false;
            SaveButton.Content = "保存";
        }

        private void PermissionDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedPermission = PermissionDataGrid.SelectedItem as DataRowView;
            DeleteButton.IsEnabled = _selectedPermission != null;

            if (_selectedPermission != null)
            {
                UsernameTextBox.Text = _selectedPermission["用户名"].ToString();
                PageNameTextBox.Text = _selectedPermission["页面名称"].ToString();
                PagePermissionTextBox.Text = _selectedPermission["页面权限"].ToString();
                ControlNameTextBox.Text = _selectedPermission["控件名称"].ToString();
                ControlPermissionTextBox.Text = _selectedPermission["控件权限"].ToString();
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
            string page = PageNameTextBox.Text.Trim();
            string pagePerm = PagePermissionTextBox.Text.Trim();
            string control = ControlNameTextBox.Text.Trim();
            string controlPerm = ControlPermissionTextBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(page))
            {
                MessageBox.Show("用户名和页面名称不能为空", "错误", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            bool success;
            if (_isEditing && _selectedPermission != null)
            {
                int id = Convert.ToInt32(_selectedPermission["ID"]); // 需在表中有唯一ID列
                success = _db.UpdatePermission(id, username, page, pagePerm, control, controlPerm);
            }
            else
            {
                success = _db.AddPermission(username, page, pagePerm, control, controlPerm);
            }

            if (success)
            {
                MessageBox.Show("操作成功", "成功", MessageBoxButton.OK, MessageBoxImage.Information);
                RefreshPermissionList();
                ClearInputs();
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedPermission == null) return;

            string username = _selectedPermission["用户名"].ToString();
            string pageName = _selectedPermission["页面名称"].ToString();
            string controlName = _selectedPermission["控件名称"].ToString();

            var result = MessageBox.Show(
                $"确定要删除用户“{username}”在页面“{pageName}”上对控件“{controlName}”的权限吗？",
                "确认删除",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning
            );

            if (result == MessageBoxResult.Yes)
            {
                if (_db.DeletePermission(username, pageName, controlName))
                {
                    MessageBox.Show("删除成功", "成功", MessageBoxButton.OK, MessageBoxImage.Information);
                    RefreshPermissionList();
                    ClearInputs();
                }
                else
                {
                    MessageBox.Show("删除失败，记录可能不存在。", "失败", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }



        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            RefreshPermissionList();
            ClearInputs();
        }
    }
}
