using AuthManagement.DbUtil.Entity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AuthManagement.Pages.Auth
{
    public class LogListModel : PageModel
    {
        private readonly AuthDbContext _context;

        //构造函数中对 AuthDbContext 做依赖注入
        public LogListModel(AuthDbContext context)
        {
            _context = context;
        }

        public List<TLog> LogList { get; private set; }//定义传递给页面的属性

        private void InitLogList() //给属性赋值
        {
            //取出最近3天的数据，列表页不显示 table_data 字段的数据，因为数据太长了，
            //我们在每行末尾加一个详情的连接打开单独的页面去看。
            //用 Select 取 TLog 中的 UserName,TableName,LogTime,BatchNo 这3个字段并赋值,然后用 Distinct()去掉重复值
            LogList = _context.TLogs.Where<TLog>(x => x.LogTime > DateTime.Now.AddDays(-3).Date)
                .Select(x => new TLog { UserName = x.UserName, TableName = x.TableName, LogTime = x.LogTime, BatchNo = x.BatchNo })
                .Distinct().ToList<TLog>();
        }

        public void OnGet()
        {
            InitLogList();
        }
    }
}