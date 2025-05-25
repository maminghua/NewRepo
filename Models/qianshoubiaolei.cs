using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using DBConfig = 空运系统.Models.DatabaseConfig.DatabaseConfig;



// 文件：qianshoubiaolei.cs
/// <summary>
/// 签收表实体类，对应数据库字段
/// </summary>
public class 签收表Model
{
    public string 编号 { get; set; }
    public string 取货地点 { get; set; }
    public string 取货时间 { get; set; }
    public double 发货金额 { get; set; }
    public double 单价 { get; set; }
    public int 发货批次 { get; set; }
    public string 发货时间 { get; set; }
    public string 车牌号码 { get; set; }
    public double 实收 { get; set; }
    public string 取货备注 { get; set; }
    public string 到达卢本时间 { get; set; }
    public string 到达总部时间 { get; set; }
    public string 公司名称 { get; set; }
    public string 还款日期 { get; set; }
    public string 签收备注 { get; set; }
    public string 货架号 { get; set; }
    public string 入库时间 { get; set; }
    public string 通知与否 { get; set; }
    public string 欠款与优惠 { get; set; }
    public string 签收批次 { get; set; }
}

/// <summary>
/// 签收表数据访问类，负责对“签收表”进行增删改查
/// </summary>
public class 签收表Repository
{
    private readonly string _conn = DBConfig.ConnectionString;

    public List<签收表Model> 查询所有()
    {
        var list = new List<签收表Model>();
        using var conn = new SQLiteConnection(_conn);
        conn.Open();
        string sql = $"SELECT * FROM {DBConfig.TableNames.签收表}";
        using var cmd = new SQLiteCommand(sql, conn);
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            var m = new 签收表Model();
            foreach (var f in DBConfig.签收表字段.所有字段)
            {
                var val = reader[f];
                var prop = typeof(签收表Model).GetProperty(f);
                if (prop.PropertyType == typeof(double))
                    prop.SetValue(m, val != DBNull.Value ? Convert.ToDouble(val) : 0);
                else if (prop.PropertyType == typeof(int))
                    prop.SetValue(m, val != DBNull.Value ? Convert.ToInt32(val) : 0);
                else
                    prop.SetValue(m, val?.ToString());
            }
            list.Add(m);
        }
        return list;
    }

    public bool 新增编号(string 编号)
    {
        using var conn = new SQLiteConnection(_conn);
        conn.Open();

        string sql = $"INSERT INTO {DBConfig.TableNames.签收表} (编号) VALUES (@编号)";
        using var cmd = new SQLiteCommand(sql, conn);
        cmd.Parameters.AddWithValue("@编号", 编号);

        return cmd.ExecuteNonQuery() > 0;
    }
    public bool 更新字段(string 编号, Dictionary<string, object> 字段值字典)
    {
        using var conn = new SQLiteConnection(_conn);
        conn.Open();

        var sets = string.Join(", ", 字段值字典.Keys.Select(k => $"{k} = @{k}"));
        string sql = $"UPDATE {DBConfig.TableNames.签收表} SET {sets} WHERE 编号 = @编号";

        using var cmd = new SQLiteCommand(sql, conn);
        cmd.Parameters.AddWithValue("@编号", 编号);
        foreach (var kvp in 字段值字典)
        {
            cmd.Parameters.AddWithValue("@" + kvp.Key, kvp.Value ?? DBNull.Value);
        }

        return cmd.ExecuteNonQuery() > 0;
    }



}
