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
            //��Ҫ���µ��ֶμ�������ֵ
            TDept dept = new TDept
            {
                DeptId = deptId,
                IsValid = 0,
                ModifyTime = DateTime.Now
            };
            //����ʵ�� dept ��״̬
            _context.TDepts.Attach(dept);
            //���Ҫ���ĵ������� IsValid �� ModifyTime,������Ǽ�ʹ��ֵ��Ҳ�������
            _context.Entry<TDept>(dept).Property(dept => dept.IsValid).IsModified = true;
            _context.Entry<TDept>(dept).Property(dept => dept.ModifyTime).IsModified = true;
            //�����ı��浽���ݿ�
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
                UserId = 1, //��û�д����¼��������ģ������
                UserName = "����",
                BatchNo = batchNo,
                TableName = "t_dept",
                TableData = "",
                LogTime = DateTime.Now
            }; 
            
            TLog afterLog = new TLog
            {
                UserId = 1,
                UserName = "����",
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
