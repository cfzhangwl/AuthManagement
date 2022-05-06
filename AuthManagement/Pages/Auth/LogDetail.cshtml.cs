using System.Collections.Generic;
using System.Linq;
using AuthManagement.DbUtil.Entity;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuthManagement.Pages.Auth
{
    public class LogDetailModel : PageModel
    {
        private readonly AuthDbContext _context;

        //构造函数中对 AuthDbContext 做依赖注入
        public LogDetailModel(AuthDbContext context)
        {
            _context = context;
        }

        public List<TLog> LogDetails { get; private set; }//定义传递给页面的属性

        public void OnGet()
        {
            string batNo = Request.Query["batno"];
            //用lambda表达式以 BatchNo 为条件查询，并将结果转化为泛型 List<TLog>
            LogDetails = _context.TLogs.Where<TLog>(x => x.BatchNo == batNo).ToList<TLog>();
        }
    }
}