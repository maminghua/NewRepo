using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using 空运系统.pages;
using 空运系统.Models;
namespace 空运系统
{
    public partial class MainWindow : Window
    {
        private readonly DatabaseHelper _db = new DatabaseHelper();
        private readonly string _currentUser;

        public MainWindow(string 用户名)
        {
            InitializeComponent();
            _currentUser = 用户名;
            UserInfoBlock.Text = $"当前用户：{_currentUser}";
            InitializeUI();
        }

        /// <summary>
        /// 初始化界面和权限控制
        /// </summary>
        private void InitializeUI()
        {
            // 欢迎界面
            MainContentFrame.Content = new TextBlock
            {
                Text = "欢迎使用空运物流管理系统",
                FontSize = 24,
                Foreground = Brushes.Gray,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                FontWeight = FontWeights.Bold
            };

            // 折叠所有菜单项
            foreach (var child in MainMenuPanel.Children)
            {
                if (child is Expander exp)
                    exp.IsExpanded = false;
            }

            // 仅对系统设置菜单应用权限控制
            ApplySystemSettingsPermission();
            ApplyYueduduizhangPermission();
        }

        /// <summary>
        /// 系统设置菜单的权限控制
        /// </summary>
        private void ApplySystemSettingsPermission()
        {
            try
            {
                // 从数据库查询权限
                string perm = _db.GetControlPermission(
                    _currentUser,
                    "MainWindow",               // 对应权限表中的页面名称
                    "SystemSettingsExpander"   // 对应权限表中的控件名称
                    
                );

                // 判断权限（默认无权限）
                bool hasPermission = perm == "有权限";

                // 设置Expander状态
                SystemSettingsExpander.IsEnabled = hasPermission;
                SystemSettingsExpander.Foreground = hasPermission ? Brushes.White : Brushes.Gray;

                // 无权限时添加提示
                if (!hasPermission)
                {
                    SystemSettingsExpander.ToolTip = "当前用户无权限访问此功能";
                    SystemSettingsExpander.IsExpanded = false; // 强制折叠
                }
            }
            catch (Exception ex)
            {
                // 数据库异常时默认禁用
                SystemSettingsExpander.IsEnabled = false;
                SystemSettingsExpander.Foreground = Brushes.Gray;
                SystemSettingsExpander.ToolTip = $"权限验证失败: {ex.Message}";
            }
        }
        private void ApplyYueduduizhangPermission()
        {
            try
            {
                string perm = _db.GetControlPermission(_currentUser, "MainWindow", "yueduduizhang");
                bool hasPermission = perm == "有权限";

                yueduduizhang.IsEnabled = hasPermission;
                yueduduizhang.Foreground = hasPermission ? Brushes.White : Brushes.Gray;

                if (!hasPermission)
                {
                    yueduduizhang.ToolTip = "当前用户无权限访问此功能";
                }
            }
            catch (Exception ex)
            {
                yueduduizhang.IsEnabled = false;
                yueduduizhang.Foreground = Brushes.Gray;
                yueduduizhang.ToolTip = $"权限验证失败: {ex.Message}";
            }
        }


        // ==================== 菜单交互事件 ====================
        private void Expander_Expanded(object sender, RoutedEventArgs e)
        {
            // 保持同一时间只有一个Expander展开
            if (sender is Expander activatedExpander)
            {
                foreach (var child in MainMenuPanel.Children)
                {
                    if (child is Expander exp && exp != activatedExpander)
                        exp.IsExpanded = false;
                }
            }
        }

        // ==================== 系统操作事件 ====================
        private void CloseSystem_Click(object sender, RoutedEventArgs e)
        {
            System.Media.SystemSounds.Hand.Play();
            var result = MessageBox.Show(
                "确定要关闭系统吗？",
                "系统关闭确认",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
                Application.Current.Shutdown();
        }

        // ==================== 页面导航事件 ====================
        private void NavigateToHomePage(object sender, RoutedEventArgs e)
        {
            MainContentFrame.Content = new TextBlock
            {
                Text = "欢迎使用空运物流管理系统",
                FontSize = 24,
                Foreground = Brushes.Gray,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                FontWeight = FontWeights.Bold
            };

            // 折叠所有菜单
            foreach (var child in MainMenuPanel.Children)
            {
                if (child is Expander exp)
                    exp.IsExpanded = false;
            }
        }

        // 以下是示例导航方法（需自行实现页面）
        

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MainContentFrame.Navigate(new 用户管理());
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("功能未实现", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            MainContentFrame.Navigate(new 页面权限());
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            MainContentFrame.Navigate(new 上传账单());
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            MainContentFrame.Navigate(new 更新页面());
        }
    }
}