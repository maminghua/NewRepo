using System.Windows;
using 空运系统.Models;

namespace 空运系统
{
    public partial class LoginWindow : Window
    {
        private readonly DatabaseHelper _db = new();

        public LoginWindow()
        {
           InitializeComponent();
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            string user = UsernameBox.Text.Trim();
            string pwd = PasswordBox.Password;

            if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(pwd))
            {
                ErrorText.Text = "请输入用户名和密码。";
                return;
            }

            if (_db.ValidateUser(user, pwd))
            {
                //// 登录成功
               var main = new MainWindow(user);
              main.Show();
               this.Close();
            }
            else
            {
                ErrorText.Text = "用户名或密码错误。";
            }
        }
    }
}
