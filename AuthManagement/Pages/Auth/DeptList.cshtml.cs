using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using System.Threading.Tasks;
using AuthManagement.DbUtil.Entity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuthManagement.Pages.Auth
{
    public class DeptListModel : PageModel
    {
        private readonly AuthDbContext _context;
        public DeptListModel(AuthDbContext context)
        {
            _context = context;
        }

        public IList<TDept> DeptList { get; set; }

        public void OnGet()
        {
            string deptId = Request.Query["deptid"];
            if (int.TryParse(deptId, out int did))
            {
                TUser user = _context.TUsers.Where(user => user.DeptId == did && user.IsValid == 1).FirstOrDefault<TUser>();
                if (user == null)
                {
                    UpdateByFind(did);
                }

            }
            DeptList = _context.TDepts.Where<TDept>(dept => dept.IsValid == 1).ToList<TDept>();
        }
        private void UpdateDirectly(int deptId)
        {
            //将要更新的字段及主键赋值
            TDept dept = new TDept
            {
                DeptId = deptId,
                IsValid = 0,
                ModifyTime = DateTime.Now
            };
            //跟踪实体 dept 的状态
            _context.TDepts.Attach(dept);
            //标记要更改的属性是 IsValid 和 ModifyTime,不做标记即使赋值了也不会更改
            _context.Entry<TDept>(dept).Property(dept => dept.IsValid).IsModified = true;
            _context.Entry<TDept>(dept).Property(dept => dept.ModifyTime).IsModified = true;
            //将更改保存到数据库
            _context.SaveChanges();
        }

        private void UpdateByFind(int deptId)
        {
            JsonSerializerOptions options = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
            };

            List<TLog> logList = GenerateLog();
            TDept dept = _context.TDepts.Find(deptId);

            logList[0].TableData = JsonSerializer.Serialize<TDept>(dept, options);
            dept.IsValid = 0;
            dept.ModifyTime = DateTime.Now;

            logList[1].TableData = JsonSerializer.Serialize<TDept>(dept, options);

            _context.TLogs.AddRange(logList);
            _context.SaveChanges();

        }

        private List<TLog> GenerateLog()
        {
            string batchNo = Guid.NewGuid().ToString();
            TLog beforeLog = new TLog
            {
                UserId = 1, //因没有处理登录，这里用模拟数据
                UserName = "张三",
                BatchNo = batchNo,
                TableName = "t_dept",
                TableData = "",
                LogTime = DateTime.Now
            }; 
            
            TLog afterLog = new TLog
            {
                UserId = 1,
                UserName = "张三",
                BatchNo = batchNo,
                TableName = "t_dept",
                TableData = "",
                LogTime = DateTime.Now
            };

            List<TLog> logList = new List<TLog>();
            logList.Add(beforeLog);
            logList.Add(afterLog);
            return logList;
        }


    }
}
