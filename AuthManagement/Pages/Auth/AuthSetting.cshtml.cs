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

        //构造函数中对 AuthDbContext 做依赖注入
        public AuthSettingModel(AuthDbContext context)
        {
            _context = context;
        }


        public List<TDept> DeptList { get; private set; }
        public List<TUser> UserList { get; private set; }
        public TargetInfo TargetInfo { get; private set; }
        public string[] AuthArray { get; private set; }

        private void InitDeptList() //初始化页面的部门列表
        {
            DeptList = _context.TDepts.Where<TDept>(dept => dept.IsValid == 1).ToList<TDept>();
        }

        private void InitUesrList() //初始化用户列表，如果没有点部门的时候为空
        {
            string deptId = Request.Query["deptid"];
            int did = Convert.ToInt32(deptId);
            UserList = _context.TUsers.Where<TUser>(x => x.DeptId == did).ToList<TUser>();
        }

        private void InitTargetInfo() //初始化权限设置对象信息
        {
            TargetInfo = new TargetInfo { TargetId = 0, TargetName = "请选择部门或用户" };
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

        private void InitAuthArray() //初始化权限数组,把该部门或用户的权限code都取出来(授权类型 1:部门，2:用户)
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
                //用LINQ把查询出来的权限集合中的funcCode找出来并转化成字符数据，
                //前端页面可以判断单个权限是否存在，决定checkbox是否要勾选。
                AuthArray = (from TAuth auth in authList select auth.FuncCode).ToArray<string>();
            }
        }


        public void OnGet() //第一次加载页面
        {
            InitDeptList();
            InitUesrList();
            InitTargetInfo();
            InitAuthArray();
        }


        public void OnPost() //点保存按钮的时候
        {
            string deptId = Request.Query["deptid"];
            string userId = Request.Query["userid"];

            string allFuncCode = Request.Form["funccode"];
            if (string.IsNullOrWhiteSpace(allFuncCode)) return;


            string[] arrCode = allFuncCode.Split(",");
            if (!string.IsNullOrWhiteSpace(userId)) //优先处理用户的权限
            {
                int uid = Convert.ToInt32(userId);
                SaveUserAuth(arrCode, uid); //保存用户权限
            }
            else if (!string.IsNullOrWhiteSpace(deptId))
            {
                int did = Convert.ToInt32(deptId);
                SaveDeptAuth(arrCode, did); //保存部门权限
            }
            //初始化页面数据
            OnGet();
            Response.Redirect("/Auth/AuthSetting?deptid=" + deptId + "&userid=" + userId);
        }

        private void SaveUserAuth(string[] arrFuncCode, int userId) //保存用户权限
        {
            //先将该用户已有的权限全部删除，然后再批量插入(授权类型 1:部门，2:用户)            
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
            _context.TAuths.RemoveRange(existList);//批量删除
            _context.TAuths.AddRange(newList);//批量增加
            _context.SaveChanges();//执行数据库操作
        }

        private void SaveDeptAuth(string[] arrFuncCode, int deptId) //保存部门权限
        {
            //先将该部门已有的权限全部删除，然后再批量插入(授权类型 1:部门，2:用户)
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
            _context.TAuths.RemoveRange(existList);//批量删除
            _context.TAuths.AddRange(newList);//批量增加
            _context.SaveChanges();//执行数据库操作
        }
    }


    //为了页面处理数据方便，定义一个TargetInfo对象来记录用户选择的目标对象信息
    public class TargetInfo
    {
        public int TargetId { get; set; } //部门或用户编号
        public string TargetName { get; set; } //部门或用户名称
    }
}