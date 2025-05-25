using System;
using System.Data;
using System.Data.SQLite;
using System.Windows;
using DBConfig = 空运系统.Models.DatabaseConfig.DatabaseConfig;

namespace 空运系统.Models
{
    /// <summary>
    /// 通用数据库操作辅助类，针对“用户表”、“权限表”、“固定选项表”等表提供增删改查
    /// </summary>
    public class DatabaseHelper
    {
        private readonly string _connectionString = DBConfig.ConnectionString;

        /// <summary>
        /// 用户登录验证
        /// </summary>
        public bool ValidateUser(string username, string password)
        {
            using var conn = new SQLiteConnection(_connectionString);
            conn.Open();
            string sql =
                $"SELECT COUNT(1) FROM {DBConfig.TableNames.用户表} " +
                $"WHERE {DBConfig.用户表字段.用户名} = @u AND {DBConfig.用户表字段.密码} = @p";
            using var cmd = new SQLiteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@u", username);
            cmd.Parameters.AddWithValue("@p", password);
            return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
        }

        /// <summary>
        /// 添加新用户
        /// </summary>
        public bool AddUser(string username, string password)
        {
            try
            {
                using var conn = new SQLiteConnection(_connectionString);
                conn.Open();
                string sql =
                    $"INSERT INTO {DBConfig.TableNames.用户表} " +
                    $"({DBConfig.用户表字段.用户名}, {DBConfig.用户表字段.密码}) " +
                    "VALUES (@u, @p)";
                using var cmd = new SQLiteCommand(sql, conn);
                cmd.Parameters.AddWithValue("@u", username);
                cmd.Parameters.AddWithValue("@p", password);
                return cmd.ExecuteNonQuery() > 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"添加用户失败: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 更新用户信息
        /// </summary>
        public bool UpdateUser(string originalUsername, string newUsername, string newPassword)
        {
            try
            {
                using var conn = new SQLiteConnection(_connectionString);
                conn.Open();
                string sql =
                    $"UPDATE {DBConfig.TableNames.用户表} SET " +
                    $"{DBConfig.用户表字段.用户名} = @newU, " +
                    $"{DBConfig.用户表字段.密码} = @newP " +
                    $"WHERE {DBConfig.用户表字段.用户名} = @orig";
                using var cmd = new SQLiteCommand(sql, conn);
                cmd.Parameters.AddWithValue("@newU", newUsername);
                cmd.Parameters.AddWithValue("@newP", newPassword);
                cmd.Parameters.AddWithValue("@orig", originalUsername);
                return cmd.ExecuteNonQuery() > 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"更新用户失败: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 删除用户
        /// </summary>
        public bool DeleteUser(string username)
        {
            try
            {
                using var conn = new SQLiteConnection(_connectionString);
                conn.Open();
                string sql =
                    $"DELETE FROM {DBConfig.TableNames.用户表} " +
                    $"WHERE {DBConfig.用户表字段.用户名} = @u";
                using var cmd = new SQLiteCommand(sql, conn);
                cmd.Parameters.AddWithValue("@u", username);
                return cmd.ExecuteNonQuery() > 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"删除用户失败: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 获取所有用户
        /// </summary>
        public DataTable GetAllUsers()
        {
            string sql =
                $"SELECT {DBConfig.用户表字段.用户名}, {DBConfig.用户表字段.密码} " +
                $"FROM {DBConfig.TableNames.用户表}";
            return ExecuteQuery(sql);
        }

        /// <summary>
        /// 检查页面级权限
        /// </summary>
        public bool HasPagePermission(string username, string pageName)
        {
            using var conn = new SQLiteConnection(_connectionString);
            conn.Open();
            string sql =
                $"SELECT {DBConfig.权限表字段.页面权限} FROM {DBConfig.TableNames.权限表} " +
                $"WHERE {DBConfig.权限表字段.用户名} = @u " +
                $"AND {DBConfig.权限表字段.页面名称} = @p " +
                "AND 控件名称 IS NULL";
            using var cmd = new SQLiteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@u", username);
            cmd.Parameters.AddWithValue("@p", pageName);
            var result = cmd.ExecuteScalar();
            return result?.ToString() == "有权限";
        }

        /// <summary>
        /// 获取控件级权限
        /// </summary>
        public string GetControlPermission(string username, string pageName, string controlName)
        {
            using var conn = new SQLiteConnection(_connectionString);
            conn.Open();
            string sql =
                $"SELECT {DBConfig.权限表字段.控件权限} FROM {DBConfig.TableNames.权限表} " +
                $"WHERE {DBConfig.权限表字段.用户名} = @u " +
                $"AND {DBConfig.权限表字段.页面名称} = @p " +
                $"AND {DBConfig.权限表字段.控件名称} = @c";
            using var cmd = new SQLiteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@u", username);
            cmd.Parameters.AddWithValue("@p", pageName);
            cmd.Parameters.AddWithValue("@c", controlName);
            var result = cmd.ExecuteScalar();
            return result?.ToString() ?? "只读";
        }

        /// <summary>
        /// 获取所有权限记录
        /// </summary>
        public DataTable GetAllPermissions()
        {
            string sql =
                $"SELECT {DBConfig.权限表字段.用户名}, {DBConfig.权限表字段.页面名称}, " +
                $"{DBConfig.权限表字段.页面权限}, {DBConfig.权限表字段.控件名称}, {DBConfig.权限表字段.控件权限} " +
                $"FROM {DBConfig.TableNames.权限表} ORDER BY {DBConfig.权限表字段.用户名}, {DBConfig.权限表字段.页面名称}";
            return ExecuteQuery(sql);
        }

        /// <summary>
        /// 更新权限记录（按ID）
        /// </summary>
        public bool UpdatePermission(int id, string newUsername, string newPageName, string newPagePermission,
                                     string newControlName, string newControlPermission)
        {
            using var conn = new SQLiteConnection(_connectionString);
            conn.Open();
            string sql =
                $"UPDATE {DBConfig.TableNames.权限表} SET " +
                $"{DBConfig.权限表字段.用户名} = @u, " +
                $"{DBConfig.权限表字段.页面名称} = @p, " +
                $"{DBConfig.权限表字段.页面权限} = @pp, " +
                $"{DBConfig.权限表字段.控件名称} = @cn, " +
                $"{DBConfig.权限表字段.控件权限} = @cp " +
                "WHERE ID = @id";
            using var cmd = new SQLiteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@u", newUsername);
            cmd.Parameters.AddWithValue("@p", newPageName);
            cmd.Parameters.AddWithValue("@pp", newPagePermission);
            cmd.Parameters.AddWithValue("@cn", newControlName);
            cmd.Parameters.AddWithValue("@cp", newControlPermission);
            cmd.Parameters.AddWithValue("@id", id);
            return cmd.ExecuteNonQuery() > 0;
        }

        /// <summary>
        /// 添加权限记录
        /// </summary>
        public bool AddPermission(string username, string pageName, string pagePermission, string controlName, string controlPermission)
        {
            using var conn = new SQLiteConnection(_connectionString);
            conn.Open();
            string sql =
                $"INSERT INTO {DBConfig.TableNames.权限表} " +
                $"({DBConfig.权限表字段.用户名}, {DBConfig.权限表字段.页面名称}, {DBConfig.权限表字段.页面权限}, " +
                $"{DBConfig.权限表字段.控件名称}, {DBConfig.权限表字段.控件权限}) " +
                "VALUES (@u, @p, @pp, @cn, @cp)";
            using var cmd = new SQLiteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@u", username);
            cmd.Parameters.AddWithValue("@p", pageName);
            cmd.Parameters.AddWithValue("@pp", pagePermission);
            cmd.Parameters.AddWithValue("@cn", controlName);
            cmd.Parameters.AddWithValue("@cp", controlPermission);
            return cmd.ExecuteNonQuery() > 0;
        }

        /// <summary>
        /// 删除权限记录
        /// </summary>
        public bool DeletePermission(string username, string pageName, string controlName)
        {
            using var conn = new SQLiteConnection(_connectionString);
            conn.Open();
            string sql =
                $"DELETE FROM {DBConfig.TableNames.权限表} " +
                $"WHERE {DBConfig.权限表字段.用户名} = @u AND {DBConfig.权限表字段.页面名称} = @p AND {DBConfig.权限表字段.控件名称} = @c";
            using var cmd = new SQLiteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@u", username);
            cmd.Parameters.AddWithValue("@p", pageName);
            cmd.Parameters.AddWithValue("@c", controlName);
            return cmd.ExecuteNonQuery() > 0;
        }

        /// <summary>
        /// 读取固定选项表的值
        /// </summary>
        public static string GetOptionValue(string category)
        {
            string sql =
                $"SELECT {DBConfig.固定选项表字段.选项值} FROM {DBConfig.TableNames.固定选项表} " +
                $"WHERE {DBConfig.固定选项表字段.类别} = @cat LIMIT 1";
            using var conn = new SQLiteConnection(DBConfig.ConnectionString);
            conn.Open();
            using var cmd = new SQLiteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@cat", category);
            var result = cmd.ExecuteScalar();
            return result?.ToString();
        }

        /// <summary>
        /// 更新固定选项表的值
        /// </summary>
        public static void SaveOptionValue(string category, string value)
        {
            string sql =
                $"UPDATE {DBConfig.TableNames.固定选项表} " +
                $"SET {DBConfig.固定选项表字段.选项值} = @v " +
                $"WHERE {DBConfig.固定选项表字段.类别} = @c";
            using var conn = new SQLiteConnection(DBConfig.ConnectionString);
            conn.Open();
            using var cmd = new SQLiteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@v", value);
            cmd.Parameters.AddWithValue("@c", category);
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// 执行 SELECT 并返回 DataTable
        /// </summary>
        private DataTable ExecuteQuery(string sql, params SQLiteParameter[] ps)
        {
            using var conn = new SQLiteConnection(_connectionString);
            conn.Open();
            using var cmd = new SQLiteCommand(sql, conn);
            if (ps.Length > 0) cmd.Parameters.AddRange(ps);
            using var adapter = new SQLiteDataAdapter(cmd);
            var dt = new DataTable();
            adapter.Fill(dt);
            return dt;
        }
    }
}
