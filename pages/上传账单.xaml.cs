using System;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using 空运系统.Models;
using 空运系统.pages;
using static 空运系统.Models.DatabaseConfig.DatabaseConfig;
namespace 空运系统.pages
{
    public partial class 上传账单 : Page
    {
        private string currentDefaultMonth = DateTime.Now.ToString("yyyy-MM");
        private DataTable billTable;

        public 上传账单()
        {
            InitializeComponent();
            // 读取固定选项表中的默认月份值
            string savedDefaultMonth = DatabaseHelper.GetOptionValue("月份默认值");
            if (!string.IsNullOrWhiteSpace(savedDefaultMonth))
            {
                currentDefaultMonth = savedDefaultMonth;
            }

            BillTypeLabel.Text = "月份默认值：" + currentDefaultMonth;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            InitializeDataTable();
            UpdateStatistics(); // 初始化时更新统计信息
        }

        // 初始化DataTable和固定列顺序，绑定DataGrid
        private void InitializeDataTable()
        {
            billTable = new DataTable();

            // 按照指定顺序和列名创建列
            billTable.Columns.Add("包裹编号", typeof(string));
            billTable.Columns.Add("编号", typeof(string));
            billTable.Columns.Add("姓名", typeof(string));
            billTable.Columns.Add("品名", typeof(string));
            billTable.Columns.Add("重量", typeof(double));
            billTable.Columns.Add("单价", typeof(double));
            billTable.Columns.Add("金额", typeof(double));
            billTable.Columns.Add("国内付款", typeof(string));
            billTable.Columns.Add("快递单号", typeof(string));
            billTable.Columns.Add("快递公司", typeof(string));
            billTable.Columns.Add("票号", typeof(string));
            billTable.Columns.Add("月份", typeof(string));
            billTable.Columns.Add("备注", typeof(string));

            BillDataGrid.ItemsSource = billTable.DefaultView;

            // 手动定义DataGrid列，保证列顺序和名称
            BillDataGrid.Columns.Clear();

            foreach (DataColumn col in billTable.Columns)
            {
                var dgCol = new DataGridTextColumn()
                {
                    Header = col.ColumnName,
                    Binding = new System.Windows.Data.Binding(col.ColumnName),
                    IsReadOnly = false
                };
                BillDataGrid.Columns.Add(dgCol);
            }
        }

        // 粘贴板数据粘贴事件捕获（示例使用 Ctrl+V 捕获）
        private void BillDataGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if ((Keyboard.Modifiers == ModifierKeys.Control) && (e.Key == Key.V))
            {
                PasteClipboardToDataTable();
                e.Handled = true;
            }
        }

        // 粘贴剪贴板数据到 DataTable
        private void PasteClipboardToDataTable()
        {
            string clipboardText = Clipboard.GetText();
            if (string.IsNullOrWhiteSpace(clipboardText)) return;

            var lines = clipboardText.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var line in lines)
            {
                var fields = line.Split('\t');

                var rowFields = new string[billTable.Columns.Count];
                for (int i = 0; i < billTable.Columns.Count; i++)
                {
                    // 先填字段数据，月份字段留空或填默认值
                    if (billTable.Columns[i].ColumnName == "月份")
                    {
                        rowFields[i] = currentDefaultMonth; // 自动填默认月份
                    }
                    else
                    {
                        rowFields[i] = i < fields.Length ? fields[i] : string.Empty;
                    }
                }

                var newRow = billTable.NewRow();

                for (int i = 0; i < billTable.Columns.Count; i++)
                {
                    var col = billTable.Columns[i];
                    try
                    {
                        if (col.DataType == typeof(double))
                        {
                            if (double.TryParse(rowFields[i], out double val))
                                newRow[i] = val;
                            else
                                newRow[i] = 0;
                        }
                        else
                        {
                            newRow[i] = rowFields[i];
                        }
                    }
                    catch
                    {
                        newRow[i] = DBNull.Value;
                    }
                }

                billTable.Rows.Add(newRow);
            }
            UpdateStatistics();
        }


        // 点击 添加新账单 按钮事件，调用插入数据库等逻辑
        private async void AddBillButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (BillDataGrid.Items.Count == 0)
                {
                    MessageBox.Show("表格中无数据，请先粘贴数据！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                var 总表Repo = new 总表Repository();
                var 签收表Repo = new 签收表Repository();
                var 日志Repo = new 日志表Repository();

                var exist编号List = new List<string>();
                var 编号ListToCheck = new List<string>();

                // 1) 收集所有编号
                foreach (var item in BillDataGrid.Items)
                {
                    if (item is DataRowView rowView)
                    {
                        string 编号 = (rowView["编号"] ?? "").ToString().Trim();
                        if (string.IsNullOrWhiteSpace(编号))
                        {
                            MessageBox.Show("编号不能为空，请检查数据。", "错误", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }
                        编号ListToCheck.Add(编号);
                    }
                }

                // 2) 检查哪些编号在总表已经存在
                foreach (var 编号 in 编号ListToCheck)
                {
                    if (总表Repo.查询(编号).Any())
                        exist编号List.Add(编号);
                }
                if (exist编号List.Any())
                {
                    MessageBox.Show($"以下编号已存在，未插入：{string.Join(", ", exist编号List)}", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // 3) 主窗体用户名
                //    假设 MainWindow 实例保存在 Application.Current.Windows 中
                var main = Application.Current.Windows
                               .OfType<MainWindow>()
                               .FirstOrDefault();
                string currentUser = main?.UserInfoBlock.Text ?? "Unknown";

                // 4) 插入到总表、签收表、日志表
                int insertedCount = 0;
                foreach (var item in BillDataGrid.Items)
                {
                    if (item is DataRowView rowView)
                    {
                        var model = new 总表Model
                        {
                            包裹编号 = rowView["包裹编号"]?.ToString(),
                            编号 = rowView["编号"]?.ToString().Trim(),
                            姓名 = rowView["姓名"]?.ToString(),
                            品名 = rowView["品名"]?.ToString(),
                            重量 = double.TryParse(rowView["重量"]?.ToString(), out var w) ? w : 0,
                            单价 = double.TryParse(rowView["单价"]?.ToString(), out var p) ? p : 0,
                            金额 = double.TryParse(rowView["金额"]?.ToString(), out var a) ? a : 0,
                            国内付款 = rowView["国内付款"]?.ToString(),
                            快递单号 = rowView["快递单号"]?.ToString(),
                            快递公司 = rowView["快递公司"]?.ToString(),
                            票号 = rowView["票号"]?.ToString(),
                            月份 = string.IsNullOrEmpty(rowView["月份"]?.ToString())
                                           ? currentDefaultMonth
                                           : rowView["月份"].ToString(),
                            备注 = rowView["备注"]?.ToString()
                        };

                        // (1) 插入总表
                        if (总表Repo.新增(model))
                        {
                            insertedCount++;

                            // (2) 同时只把“编号”插入签收表
                            签收表Repo.新增编号(model.编号);

                            // (3) 写日志：编号字段使用 model.编号
                            var log总表 = new 日志表Model
                            {
                                编号 = model.编号,
                                表名 = TableNames.总表,
                                作者 = currentUser,
                                操作类型 = "新增记录",
                                上传 = "是",
                                时间 = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                                字段名称 = "编号",
                                字段旧值 = null,
                                字段新值 = model.编号,
                                下载 = null,
                                其他 = null
                            };
                            日志Repo.新增(log总表);

                            // 3) 写日志：针对签收表
                            var log签收表 = new 日志表Model
                            {
                                // 使用相同的编号字段值
                                编号 = model.编号,
                                表名 = TableNames.签收表,
                                作者 = currentUser,
                                操作类型 = "新增记录",
                                上传 = "是",
                                时间 = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                                字段名称 = "编号",
                                字段旧值 = null,
                                字段新值 = model.编号,
                                下载 = null,
                                其他 = null
                            };
                            日志Repo.新增(log签收表);
                        }
                    }
                }

                MessageBox.Show($"成功插入 {insertedCount} 条总表记录并同步写入签收表和日志。", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                billTable.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"发生错误：{ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // 点击 清空表格 按钮事件
        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            billTable.Clear();
        }

        // 示例标签双击修改（如果需要实现）
        private void BillTypeLabel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                string input = Microsoft.VisualBasic.Interaction.InputBox(
                    "请输入默认月份，例如 2025-05", "设置默认月份", currentDefaultMonth);

                if (!string.IsNullOrWhiteSpace(input))
                {
                    DatabaseHelper.SaveOptionValue("月份默认值", input);
                    currentDefaultMonth = input;  // 同步更新变量
                    BillTypeLabel.Text = "月份默认值：" + input;
                }
            }
        }

        private void UpdateStatistics()
        {
            int rowCount = billTable?.Rows.Count ?? 0;
            double totalAmount = 0;

            if (billTable != null && billTable.Columns.Contains("金额"))
            {
                foreach (DataRow row in billTable.Rows)
                {
                    if (double.TryParse(row["金额"]?.ToString(), out double val))
                    {
                        totalAmount += val;
                    }
                }
            }

            RowCountTextBlock.Text = $"行数：{rowCount}";
            TotalAmountTextBlock.Text = $"金额合计：{totalAmount:N2}";
        }

    }
}
