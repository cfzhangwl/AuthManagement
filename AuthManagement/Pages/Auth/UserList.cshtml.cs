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

        public readonly int PageSize = 3; //定义分页大小
        public int RecordCount = 0;//记录总数，在InitUser()中根据查询结果赋具体的值
        public int PageCount = 0;//分页总数，在InitUser()中根据查询结果赋具体的值
        public int PageIndex = 1;//当前页，在InitUser()中根据查询结果赋具体的值

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

        //注：前端页面如果设置了handler属性:asp-page-handler="Cancel",
        //则对应的方法是 OnGetCancel, 即 OnGet + Cancel
        public void OnGetCancel()
        {
            string uid = Request.Query["uid"];
            if (int.TryParse(uid, out int userId))
            {
                TUser user = _context.TUsers.Find(userId);
                user.IsValid = 0;
                _context.SaveChanges();
            }

            //锁定后要重新加载数据，否则前端页面取不到值。
            OnGet();
            Response.Redirect(Request.Path);
        }


        //注：前端页面如果设置了handler属性:asp-page-handler="Delete",
        //则对应的方法是 OnGetDelete, 即 OnGet + Delete
        public void OnGetDelete()
        {
            string uid = Request.Query["uid"];
            if (int.TryParse(uid, out int userId))
            {
                TUser user = _context.TUsers.Find(userId);
                _context.TUsers.Remove(user);
                _context.SaveChanges();
            }

            //作废后要重新加载数据，否则前端页面取不到值。
            OnGet();
            Response.Redirect(Request.Path);
        }
    }
}
