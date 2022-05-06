using System;
using System.Collections.Generic;
using System.Linq;
using AuthManagement.DbUtil.Entity;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuthManagement.Pages.Auth
{
    public class AuthSettingModel : PageModel
    {
        private readonly AuthDbContext _context;

        //���캯���ж� AuthDbContext ������ע��
        public AuthSettingModel(AuthDbContext context)
        {
            _context = context;
        }


        public List<TDept> DeptList { get; private set; }
        public List<TUser> UserList { get; private set; }
        public TargetInfo TargetInfo { get; private set; }
        public string[] AuthArray { get; private set; }

        private void InitDeptList() //��ʼ��ҳ��Ĳ����б�
        {
            DeptList = _context.TDepts.Where<TDept>(dept => dept.IsValid == 1).ToList<TDept>();
        }

        private void InitUesrList() //��ʼ���û��б����û�е㲿�ŵ�ʱ��Ϊ��
        {
            string deptId = Request.Query["deptid"];
            int did = Convert.ToInt32(deptId);
            UserList = _context.TUsers.Where<TUser>(x => x.DeptId == did).ToList<TUser>();
        }

        private void InitTargetInfo() //��ʼ��Ȩ�����ö�����Ϣ
        {
            TargetInfo = new TargetInfo { TargetId = 0, TargetName = "��ѡ���Ż��û�" };
            string deptId = Request.Query["deptid"];
            string userId = Request.Query["userid"];
            if (!string.IsNullOrWhiteSpace(userId))
            {
                TUser user = _context.TUsers.Find(Convert.ToInt32(userId));
                TargetInfo.TargetId = Convert.ToInt32(userId);
                TargetInfo.TargetName = user.UserName;
            }
            else if (!string.IsNullOrWhiteSpace(deptId))
            {
                TDept dept = _context.TDepts.Find(Convert.ToInt32(deptId));
                TargetInfo.TargetId = Convert.ToInt32(deptId);
                TargetInfo.TargetName = dept.DeptName;
            }
        }

        private void InitAuthArray() //��ʼ��Ȩ������,�Ѹò��Ż��û���Ȩ��code��ȡ����(��Ȩ���� 1:���ţ�2:�û�)
        {
            IQueryable<TAuth> authList = null;
            string deptId = Request.Query["deptid"];
            string userId = Request.Query["userid"];
            if (!string.IsNullOrWhiteSpace(userId))
            {
                authList = _context.TAuths.Where<TAuth>(x => x.TargetType == 2 && x.TargetId == Convert.ToInt32(userId));
            }
            else if (!string.IsNullOrWhiteSpace(deptId))
            {
                authList = _context.TAuths.Where<TAuth>(x => x.TargetType == 1 && x.TargetId == Convert.ToInt32(deptId));
            }
            if (authList == null || authList.Count() < 1)
            {
                AuthArray = new string[1] { "" };
            }
            else
            {
                //��LINQ�Ѳ�ѯ������Ȩ�޼����е�funcCode�ҳ�����ת�����ַ����ݣ�
                //ǰ��ҳ������жϵ���Ȩ���Ƿ���ڣ�����checkbox�Ƿ�Ҫ��ѡ��
                AuthArray = (from TAuth auth in authList select auth.FuncCode).ToArray<string>();
            }
        }


        public void OnGet() //��һ�μ���ҳ��
        {
            InitDeptList();
            InitUesrList();
            InitTargetInfo();
            InitAuthArray();
        }


        public void OnPost() //�㱣�水ť��ʱ��
        {
            string deptId = Request.Query["deptid"];
            string userId = Request.Query["userid"];

            string allFuncCode = Request.Form["funccode"];
            if (string.IsNullOrWhiteSpace(allFuncCode)) return;


            string[] arrCode = allFuncCode.Split(",");
            if (!string.IsNullOrWhiteSpace(userId)) //���ȴ����û���Ȩ��
            {
                int uid = Convert.ToInt32(userId);
                SaveUserAuth(arrCode, uid); //�����û�Ȩ��
            }
            else if (!string.IsNullOrWhiteSpace(deptId))
            {
                int did = Convert.ToInt32(deptId);
                SaveDeptAuth(arrCode, did); //���沿��Ȩ��
            }
            //��ʼ��ҳ������
            OnGet();
            Response.Redirect("/Auth/AuthSetting?deptid=" + deptId + "&userid=" + userId);
        }

        private void SaveUserAuth(string[] arrFuncCode, int userId) //�����û�Ȩ��
        {
            //�Ƚ����û����е�Ȩ��ȫ��ɾ����Ȼ������������(��Ȩ���� 1:���ţ�2:�û�)            
            List<TAuth> newList = new List<TAuth>();
            foreach (string str in arrFuncCode)
            {
                TAuth tauth = new TAuth
                {
                    CreateTime = DateTime.Now,
                    FuncCode = str,
                    TargetId = userId,
                    TargetType = 2
                };
                newList.Add(tauth);
            }
            IQueryable<TAuth> existList = _context.TAuths.Where<TAuth>(x => x.TargetType == 2 && x.TargetId == userId);
            _context.TAuths.RemoveRange(existList);//����ɾ��
            _context.TAuths.AddRange(newList);//��������
            _context.SaveChanges();//ִ�����ݿ����
        }

        private void SaveDeptAuth(string[] arrFuncCode, int deptId) //���沿��Ȩ��
        {
            //�Ƚ��ò������е�Ȩ��ȫ��ɾ����Ȼ������������(��Ȩ���� 1:���ţ�2:�û�)
            List<TAuth> newList = new List<TAuth>();
            foreach (string str in arrFuncCode)
            {
                TAuth tauth = new TAuth
                {
                    CreateTime = DateTime.Now,
                    FuncCode = str,
                    TargetId = deptId,
                    TargetType = 1
                };
                newList.Add(tauth);
            }
            IQueryable<TAuth> existList = _context.TAuths.Where<TAuth>(x => x.TargetType == 1 && x.TargetId == deptId);
            _context.TAuths.RemoveRange(existList);//����ɾ��
            _context.TAuths.AddRange(newList);//��������
            _context.SaveChanges();//ִ�����ݿ����
        }
    }


    //Ϊ��ҳ�洦�����ݷ��㣬����һ��TargetInfo��������¼�û�ѡ���Ŀ�������Ϣ
    public class TargetInfo
    {
        public int TargetId { get; set; } //���Ż��û����
        public string TargetName { get; set; } //���Ż��û�����
    }
}