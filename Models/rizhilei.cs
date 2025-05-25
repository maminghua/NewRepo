// 文件：rizhilei.cs
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using DBConfig = 空运系统.Models.DatabaseConfig.DatabaseConfig;

namespace 空运系统.Models
{
    /// <summary>
    /// 日志表实体类，对应数据库字段
    /// </summary>
    public class 日志表Model
    {
        public string 编号 { get; set; }
        public string 表名 { get; set; }
        public string 作者 { get; set; }
        public string 操作类型 { get; set; }
        public string 上传 { get; set; }
        public string 时间 { get; set; }
        public string 下载 { get; set; }
        public string 字段名称 { get; set; }
        public string 其他 { get; set; }
        public string 字段旧值 { get; set; }
        public string 字段新值 { get; set; }
    }

    /// <summary>
    /// 日志表数据访问类，负责对“日志表”进行增删改查
    /// </summary>
    public class 日志表Repository
    {
        private readonly string _conn = DBConfig.ConnectionString;

        public List<日志表Model> 查询所有()
        {
            var list = new List<日志表Model>();
            using var conn = new SQLiteConnection(_conn);
            conn.Open();
            string sql = $"SELECT * FROM {DBConfig.TableNames.日志表}";
            using var cmd = new SQLiteCommand(sql, conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var m = new 日志表Model();
                foreach (var f in DBConfig.日志表字段.所有字段)
                {
                    var val = reader[f];
                    typeof(日志表Model).GetProperty(f).SetValue(m, val?.ToString());
                }
                list.Add(m);
            }
            return list;
        }

        public bool 新增(日志表Model m)
        {
            using var conn = new SQLiteConnection(_conn);
            conn.Open();
            string cols = string.Join(", ", DBConfig.日志表字段.所有字段);
            string pars = string.Join(", ", DBConfig.日志表字段.所有字段.ConvertAll(f => "@" + f));
            string sql = $"INSERT INTO {DBConfig.TableNames.日志表} ({cols}) VALUES ({pars})";
            using var cmd = new SQLiteCommand(sql, conn);
            foreach (var f in DBConfig.日志表字段.所有字段)
            {
                var val = typeof(日志表Model).GetProperty(f).GetValue(m) ?? (object)DBNull.Value;
                cmd.Parameters.AddWithValue("@" + f, val);
            }
            return cmd.ExecuteNonQuery() > 0;
        }

       
    }
}
