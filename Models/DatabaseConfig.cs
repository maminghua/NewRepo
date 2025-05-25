using System.Collections.Generic;

namespace 空运系统.Models.DatabaseConfig
{
    public static class DatabaseConfig
    {
        // 数据库连接字符串
        public static readonly string ConnectionString = @"Data Source=D:\vs空运系统\空运数据库\空运数据库.DB";

        // 表名常量
        public static class TableNames
        {
            public const string 总表 = "总表";
            public const string 用户表 = "用户表";
            public const string 数据权限表 = "数据权限表";
            public const string 权限表 = "权限表";
            public const string 日志表 = "日志表";
            public const string 签收表 = "签收表";
            public const string 固定选项表 = "固定选项表";
        }

        // 各个表字段配置
        public static class 总表字段
        {
            public const string 编号 = "编号";
            public const string 姓名 = "姓名";
            public const string 品名 = "品名";
            public const string 重量 = "重量";
            public const string 单价 = "单价";
            public const string 金额 = "金额";
            public const string 国内付款 = "国内付款";
            public const string 快递公司 = "快递公司";
            public const string 快递单号 = "快递单号";
            public const string 包裹编号 = "包裹编号";
            public const string 票号 = "票号";
            public const string 月份 = "月份";
            public const string 备注 = "备注";

            public static readonly List<string> 所有字段 = new List<string>
            {
                编号, 姓名, 品名, 重量, 单价, 金额,
                国内付款, 快递公司, 快递单号,
                包裹编号, 票号, 月份, 备注
            };
        }

        public static class 用户表字段
        {
            public const string 用户名 = "用户名";
            public const string 密码 = "密码";
        }

        public static class 数据权限表字段
        {
            public const string 用户名 = "用户名";
            public const string 页面名称 = "页面名称";
            public const string 控件名称 = "控件名称";
            public const string 页面权限 = "页面权限";
            public const string 控件权限 = "控件权限";
            public static readonly List<string> 所有字段 = new List<string>
            {
                用户名, 页面名称, 控件名称, 页面权限, 控件权限
            };
        }

        public static class 日志表字段
        {
            public const string 编号 = "编号";
            public const string 表名 = "表名";
            public const string 作者 = "作者";
            public const string 操作类型 = "操作类型";
            public const string 上传 = "上传";
            public const string 时间 = "时间";
            public const string 下载 = "下载";
            public const string 字段名称 = "字段名称";
            public const string 其他 = "其他";
            public const string 字段旧值 = "字段旧值";
            public const string 字段新值 = "字段新值";
            public static readonly List<string> 所有字段 = new List<string>
            {
                编号, 表名, 作者, 操作类型, 上传, 时间, 下载, 字段名称, 其他, 字段旧值, 字段新值
            };
        }

        public static class 权限表字段
        {
            public const string 用户名 = "用户名";
            public const string 页面权限 = "页面权限";
            public const string 页面名称 = "页面名称";
            public const string 控件名称 = "控件名称";
            public const string 控件权限 = "控件权限";
            public static readonly List<string> 所有字段 = new List<string>
            {
                用户名, 页面权限, 页面名称, 控件名称, 控件权限
            };
        }

        public static class 固定选项表字段
        {
            public const string 类别 = "类别";
            public const string 选项值 = "选项值";
            public static readonly List<string> 所有字段 = new List<string>
            {
                类别, 选项值
            };
        }

        public static class 签收表字段
        {
            public const string 编号 = "编号";
            public const string 取货地点 = "取货地点";
            public const string 取货时间 = "取货时间";
            public const string 发货金额 = "发货金额";
            public const string 单价 = "单价";
            public const string 发货批次 = "发货批次";
            public const string 发货时间 = "发货时间";
            public const string 车牌号码 = "车牌号码";
            public const string 实收 = "实收";
            public const string 取货备注 = "取货备注";
            public const string 到达卢本时间 = "到达卢本时间";
            public const string 到达总部时间 = "到达总部时间";
            public const string 公司名称 = "公司名称";
            public const string 还款日期 = "还款日期";
            public const string 签收备注 = "签收备注";
            public const string 货架号 = "货架号";
            public const string 入库时间 = "入库时间";
            public const string 通知与否 = "通知与否";
            public const string 欠款与优惠 = "欠款与优惠";
            public const string 签收批次 = "签收批次";
            public static readonly List<string> 所有字段 = new List<string>
            {
                编号, 取货地点, 取货时间, 发货金额, 单价, 发货批次,
                发货时间, 车牌号码, 实收, 取货备注, 到达卢本时间, 到达总部时间,
                公司名称, 还款日期, 签收备注, 货架号, 入库时间, 通知与否,
                欠款与优惠, 签收批次
            };
        }
    }
}