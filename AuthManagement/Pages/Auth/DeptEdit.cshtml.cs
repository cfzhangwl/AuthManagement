using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using System.Threading.Tasks;
using AuthManagement.DbUtil.Entity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuthManagement.Pages.Auth
{
    public class DeptEditModel : PageModel
    {
        private readonly AuthDbContext _context;
        public DeptEditModel(AuthDbContext context)
        {
            _context = context;
        }

        public string SubjectName { get; set; }
        public string DeptName { get; set; }

        public void OnGet()
        {
            SubjectName = "��������";
            DeptName = "";
            string deptId = Request.Query["deptId"];
            if (int.TryParse(deptId, out int did))
            {
                if (did > 0)
                {
                    SubjectName = "�޸Ĳ���";
                    TDept dept = _context.TDepts.Find(did);
                    DeptName = dept.DeptName;
                }
            }
        }

        public void OnPost()
        {
            string deptName = Request.Form["deptname"];
            string deptId = Request.Query["deptid"];
            if (int.TryParse(deptId, out int did))
            {
                if (did > 0)
                {
                    ModifyDept(did, deptName);
                }
                else
                {
                    int newDeptId = AddDept(deptName);
                    deptId = newDeptId.ToString();
                }
            }
            Response.Redirect("/Auth/DeptEdit?deptid=" + deptId);
        }
        //��������
        private int AddDept(string deptName)
        {
            TDept dept = new TDept
            {
                DeptName = deptName,
                IsValid = 1,
                CreateTime = DateTime.Now
            };
            _context.TDepts.Add(dept);
            _context.SaveChanges();
            return dept.DeptId;
        }
        private void ModifyDept(int deptId, string deptName)
        {
            //�������л�ʱ�Ķ����ĵı��뷽ʽ
            JsonSerializerOptions options = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
            };
            List<TLog> logList = GenerateLog();
            TDept dept = _context.TDepts.Find(deptId);
            logList[0].TableData = JsonSerializer.Serialize<TDept>(dept, options);
            dept.DeptName = deptName;
            dept.ModifyTime = DateTime.Now;
            logList[1].TableData = JsonSerializer.Serialize<TDept>(dept, options);
            _context.TLogs.AddRange(logList);
            _context.SaveChanges();
        }

        private List<TLog> GenerateLog()
        {
            int userId = 0;
            string userName = "δ֪";
            ClaimsPrincipal cp = HttpContext.User;
            if (cp.Identity.IsAuthenticated)
            {
                string userAcc = cp.Identity.Name;
                List<Claim> claims = cp.Claims.ToList<Claim>();
                string uid = claims.Single<Claim>(option => option.Type == "UserId").Value;
                userId = Convert.ToInt32(uid);

                userName = claims.Single<Claim>(option => option.Type == "UserName").Value;

                string deptId = claims.Single<Claim>(option => option.Type == "DeptId").Value;
                string deptName = claims.Single<Claim>(option => option.Type == "DeptName").Value;

            }


            string batchNo = Guid.NewGuid().ToString();
            TLog beforeLog = new TLog
            {
                UserId = userId,//��Ϊû������¼�����޷���ȡ UserID �� UserName����д�����������ӵ�¼���ܺ����޸����
                UserName = userName,
                BatchNo = batchNo,
                TableName = "t_dept",
                TableData = "",
                LogTime = DateTime.Now
            };
            TLog afterLog = new TLog
            {
                UserId = userId,
                UserName = userName,
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
