using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using 空运系统.Models.DatabaseConfig;
using static 空运系统.Models.DatabaseConfig.DatabaseConfig;

namespace 空运系统.Models
{
    /// <summary>
    /// 总表实体类，对应数据库字段
    /// </summary>
    public class 总表Model
    {
        public string 编号 { get; set; }
        public string 姓名 { get; set; }
        public string 品名 { get; set; }
        public double 重量 { get; set; }
        public double 单价 { get; set; }
        public double 金额 { get; set; }
        public string 国内付款 { get; set; }
        public string 快递公司 { get; set; }
        public string 快递单号 { get; set; }
        public string 包裹编号 { get; set; }
        public string 票号 { get; set; }
        public string 月份 { get; set; }
        public string 备注 { get; set; }
    }

    /// <summary>
    /// 总表数据访问类，负责对“总表”进行增删改查
    /// </summary>
    public class 总表Repository
    {
        private readonly string _connectionString = DatabaseConfig.DatabaseConfig.ConnectionString;

        /// <summary>
        /// 查询所有总表数据
        /// </summary>
        public List<总表Model> 查询所有()
        {
            var list = new List<总表Model>();
            using (var conn = new SQLiteConnection(_connectionString))
            {
                conn.Open();
                var sql = $"SELECT * FROM {TableNames.总表}";
                using (var cmd = new SQLiteCommand(sql, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                        list.Add(ReadModel(reader));
                }
            }
            return list;
        }

        /// <summary>
        /// 带条件查询总表数据，空参数则忽略相应条件
        /// </summary>
        public List<总表Model> 查询(string 编号 = null, string 姓名 = null, string 月份开始 = null, string 月份结束 = null)
        {
            var list = new List<总表Model>();
            using (var conn = new SQLiteConnection(_connectionString))
            {
                conn.Open();
                var sb = new System.Text.StringBuilder($"SELECT * FROM {TableNames.总表} WHERE 1=1");
                using var cmd = new SQLiteCommand { Connection = conn };

                if (!string.IsNullOrWhiteSpace(编号))
                {
                    sb.Append($" AND {总表字段.编号} = @编号");
                    cmd.Parameters.AddWithValue("@编号", 编号);
                }
                if (!string.IsNullOrWhiteSpace(姓名))
                {
                    sb.Append($" AND {总表字段.姓名} LIKE @姓名");
                    cmd.Parameters.AddWithValue("@姓名", $"%{姓名}%");
                }
                if (!string.IsNullOrWhiteSpace(月份开始))
                {
                    sb.Append($" AND {总表字段.月份} >= @月份开始");
                    cmd.Parameters.AddWithValue("@月份开始", 月份开始);
                }
                if (!string.IsNullOrWhiteSpace(月份结束))
                {
                    sb.Append($" AND {总表字段.月份} <= @月份结束");
                    cmd.Parameters.AddWithValue("@月份结束", 月份结束);
                }

                cmd.CommandText = sb.ToString();
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                    list.Add(ReadModel(reader));
            }
            return list;
        }

        /// <summary>
        /// 插入一条新记录
        /// </summary>
        public bool 新增(总表Model model)
        {
            using (var conn = new SQLiteConnection(_connectionString))
            {
                conn.Open();
                var sql = $@"
                INSERT INTO {TableNames.总表}
                (
                    {总表字段.编号}, {总表字段.姓名}, {总表字段.品名}, {总表字段.重量},
                    {总表字段.单价}, {总表字段.金额}, {总表字段.国内付款}, {总表字段.快递公司},
                    {总表字段.快递单号}, {总表字段.包裹编号}, {总表字段.票号}, {总表字段.月份},
                    {总表字段.备注}
                )
                VALUES
                (
                    @编号, @姓名, @品名, @重量,
                    @单价, @金额, @国内付款, @快递公司,
                    @快递单号, @包裹编号, @票号, @月份,
                    @备注
                )";
                using var cmd = new SQLiteCommand(sql, conn);
                AddParameters(cmd, model);
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        /// <summary>
        /// 更新一条记录（按编号）
        /// </summary>
        public bool 更新(总表Model model)
        {
            using (var conn = new SQLiteConnection(_connectionString))
            {
                conn.Open();
                var sql = $@"
                    UPDATE {TableNames.总表} SET
                        {总表字段.姓名}       = @姓名,
                        {总表字段.品名}       = @品名,
                        {总表字段.重量}       = @重量,
                        {总表字段.单价}       = @单价,
                        {总表字段.金额}       = @金额,
                        {总表字段.国内付款}   = @国内付款,
                        {总表字段.快递公司}   = @快递公司,
                        {总表字段.快递单号}   = @快递单号,
                        {总表字段.包裹编号}   = @包裹编号,
                        {总表字段.票号}       = @票号,
                        {总表字段.月份}       = @月份,
                        {总表字段.备注}       = @备注
                    WHERE {总表字段.编号} = @编号";
                using var cmd = new SQLiteCommand(sql, conn);
                AddParameters(cmd, model);
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        /// <summary>
        /// 删除一条记录（按编号）
        /// </summary>
       
        /// <summary>
        /// 将模型参数添加到命令
        /// </summary>
        private void AddParameters(SQLiteCommand cmd, 总表Model m)
        {
            cmd.Parameters.AddWithValue("@编号", m.编号);
            cmd.Parameters.AddWithValue("@姓名", m.姓名);
            cmd.Parameters.AddWithValue("@品名", m.品名);
            cmd.Parameters.AddWithValue("@重量", m.重量);
            cmd.Parameters.AddWithValue("@单价", m.单价);
            cmd.Parameters.AddWithValue("@金额", m.金额);
            cmd.Parameters.AddWithValue("@国内付款", m.国内付款);
            cmd.Parameters.AddWithValue("@快递公司", m.快递公司);
            cmd.Parameters.AddWithValue("@快递单号", m.快递单号);
            cmd.Parameters.AddWithValue("@包裹编号", m.包裹编号);
            cmd.Parameters.AddWithValue("@票号", m.票号);
            cmd.Parameters.AddWithValue("@月份", m.月份);
            cmd.Parameters.AddWithValue("@备注", m.备注);
        }

        /// <summary>
        /// 从读取器中构建一个实体
        /// </summary>
        private 总表Model ReadModel(SQLiteDataReader reader)
        {
            return new 总表Model
            {
                编号 = reader[总表字段.编号]?.ToString(),
                姓名 = reader[总表字段.姓名]?.ToString(),
                品名 = reader[总表字段.品名]?.ToString(),
                重量 = reader[总表字段.重量] != DBNull.Value ? Convert.ToDouble(reader[总表字段.重量]) : 0,
                单价 = reader[总表字段.单价] != DBNull.Value ? Convert.ToDouble(reader[总表字段.单价]) : 0,
                金额 = reader[总表字段.金额] != DBNull.Value ? Convert.ToDouble(reader[总表字段.金额]) : 0,
                国内付款 = reader[总表字段.国内付款]?.ToString(),
                快递公司 = reader[总表字段.快递公司]?.ToString(),
                快递单号 = reader[总表字段.快递单号]?.ToString(),
                包裹编号 = reader[总表字段.包裹编号]?.ToString(),
                票号 = reader[总表字段.票号]?.ToString(),
                月份 = reader[总表字段.月份]?.ToString(),
                备注 = reader[总表字段.备注]?.ToString()
            };
        }
    }
}
