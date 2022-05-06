using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthManagement.DbUtil.Entity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuthManagement.Pages.Auth
{
    public class UserListModel : PageModel
    {
        private readonly AuthDbContext _context;
        public UserListModel(AuthDbContext context)
        {
            _context = context;
        }

        public IList<TUser> UserList { get; set; }
        public IList<TDept> DeptList { get; set; }

        public readonly int PageSize = 3; //�����ҳ��С
        public int RecordCount = 0;//��¼��������InitUser()�и��ݲ�ѯ����������ֵ
        public int PageCount = 0;//��ҳ��������InitUser()�и��ݲ�ѯ����������ֵ
        public int PageIndex = 1;//��ǰҳ����InitUser()�и��ݲ�ѯ����������ֵ

        public void OnGet()
        {
            InitPageIndex();
            InitDept();
            InitUser();
        }

        private void InitPageIndex()
        {
            string page = Request.Query["page"];
            int pageIndex = 1;
            if (int.TryParse(page, out int idx))
            {
                pageIndex = idx;
                if (pageIndex < 1) { pageIndex = 1; }
                this.PageIndex = pageIndex;
            }
        }

        private void InitDept()
        {
            DeptList = _context.TDepts.Where<TDept>(dept => dept.IsValid == 1).ToList<TDept>();

        }
        private void InitUser()
        {
            string did = Request.Query["did"];
            string userName = Request.Query["name"];

            IQueryable<TUser> userList = _context.TUsers.Where<TUser>(user => user.UserId > 0);
            if (!string.IsNullOrWhiteSpace(userName))
            {
                userList = userList.Where<TUser>(user => user.UserName.Contains(userName));
            }

            if (int.TryParse(did, out int deptId))
            {
                if (deptId > 0)
                {
                    userList = userList.Where<TUser>(user => user.DeptId == deptId);
                }
            }

            this.RecordCount = userList.Count();
            this.PageCount = (int)Math.Ceiling((double)RecordCount / PageSize);
            if (this.PageIndex > this.PageCount)
            {
                this.PageIndex = this.PageCount;
            }

            this.UserList = userList.Skip<TUser>((this.PageIndex - 1) * this.PageSize).Take<TUser>(this.PageSize).ToList<TUser>();
        }

        //ע��ǰ��ҳ�����������handler����:asp-page-handler="Cancel",
        //���Ӧ�ķ����� OnGetCancel, �� OnGet + Cancel
        public void OnGetCancel()
        {
            string uid = Request.Query["uid"];
            if (int.TryParse(uid, out int userId))
            {
                TUser user = _context.TUsers.Find(userId);
                user.IsValid = 0;
                _context.SaveChanges();
            }

            //������Ҫ���¼������ݣ�����ǰ��ҳ��ȡ����ֵ��
            OnGet();
            Response.Redirect(Request.Path);
        }


        //ע��ǰ��ҳ�����������handler����:asp-page-handler="Delete",
        //���Ӧ�ķ����� OnGetDelete, �� OnGet + Delete
        public void OnGetDelete()
        {
            string uid = Request.Query["uid"];
            if (int.TryParse(uid, out int userId))
            {
                TUser user = _context.TUsers.Find(userId);
                _context.TUsers.Remove(user);
                _context.SaveChanges();
            }

            //���Ϻ�Ҫ���¼������ݣ�����ǰ��ҳ��ȡ����ֵ��
            OnGet();
            Response.Redirect(Request.Path);
        }
    }
}
