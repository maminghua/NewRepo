using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace 空运系统.pages
{
    public partial class 更新页面 : Page
    {
        private DataTable updateTable;

        public 更新页面()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            InitializeDataTable();
        }

        private void InitializeDataTable()
        {
            updateTable = new DataTable();

            // 创建与上传账单页面完全相同的列结构
           
            updateTable.Columns.Add("编号", typeof(string));
            updateTable.Columns.Add("姓名", typeof(string));
            updateTable.Columns.Add("品名", typeof(string));
            updateTable.Columns.Add("重量", typeof(double));
            updateTable.Columns.Add("单价", typeof(double));
            updateTable.Columns.Add("金额", typeof(double));
            updateTable.Columns.Add("国内付款", typeof(string));
            updateTable.Columns.Add("快递单号", typeof(string));
            updateTable.Columns.Add("快递公司", typeof(string));
            updateTable.Columns.Add("票号", typeof(string));
            updateTable.Columns.Add("月份", typeof(string));
            updateTable.Columns.Add("包裹编号", typeof(string));
            updateTable.Columns.Add("备注", typeof(string));

            UpdateDataGrid.ItemsSource = updateTable.DefaultView;
            UpdateStatistics();
        }

        private void UpdateDataGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if ((Keyboard.Modifiers == ModifierKeys.Control) && (e.Key == Key.V))
            {
                PasteClipboardToDataTable();
                e.Handled = true;
            }
        }

        private void PasteClipboardToDataTable()
        {
            try
            {
                string clipboardText = Clipboard.GetText();
                if (string.IsNullOrWhiteSpace(clipboardText)) return;

                var lines = clipboardText.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);

                foreach (var line in lines)
                {
                    var fields = line.Split('\t');
                    var newRow = updateTable.NewRow();

                    for (int i = 0; i < Math.Min(fields.Length, updateTable.Columns.Count); i++)
                    {
                        try
                        {
                            if (updateTable.Columns[i].DataType == typeof(double))
                            {
                                if (double.TryParse(fields[i], out double val))
                                    newRow[i] = val;
                                else
                                    newRow[i] = 0;
                            }
                            else
                            {
                                newRow[i] = fields[i];
                            }
                        }
                        catch
                        {
                            newRow[i] = DBNull.Value;
                        }
                    }

                    updateTable.Rows.Add(newRow);
                }
                UpdateStatistics();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"粘贴数据时出错: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateStatistics()
        {
            int rowCount = updateTable?.Rows.Count ?? 0;
            double totalAmount = 0;

            if (updateTable != null && updateTable.Columns.Contains("金额"))
            {
                foreach (DataRow row in updateTable.Rows)
                {
                    if (double.TryParse(row["金额"]?.ToString(), out double val))
                    {
                        totalAmount += val;
                    }
                }
            }

            RowCountTextBlock.Text = $"行数: {rowCount}";
            TotalAmountTextBlock.Text = $"金额合计: {totalAmount:N2}";
        }

        private void QueryButton_Click(object sender, RoutedEventArgs e)
        {
            // 实现查询逻辑
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            // 实现更新逻辑
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            updateTable.Clear();
            UpdateStatistics();
        }
    }
}