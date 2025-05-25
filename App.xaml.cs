using System;
using System.Windows;

namespace 空运系统
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            // 注册未捕获的后台线程异常
            AppDomain.CurrentDomain.UnhandledException += (s, args) =>
            {
                var ex = args.ExceptionObject as Exception;
                MessageBox.Show(
                    $"[未捕获异常]\n{ex?.Message}\n\n{ex?.StackTrace}",
                    "Fatal Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            };

            // 注册UI线程未处理异常
            this.DispatcherUnhandledException += (s, args) =>
            {
                MessageBox.Show(
                    $"[UI 线程异常]\n{args.Exception.Message}\n\n{args.Exception.StackTrace}",
                    "Fatal Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                args.Handled = true;
            };

            base.OnStartup(e);

            // 如果你有登录窗口，先显示登录窗口，然后再显示主窗体
            // var loginWindow = new LoginWindow();
            // loginWindow.Show();
        }
    }
}
