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
    public class UserEditModel : PageModel
    {
        private readonly AuthDbContext _context;
        public UserEditModel(AuthDbContext context)
        {
            _context = context;
        }

        public string EditType { get; set; }
        public List<TDept> DeptList { get; set; }
        public TUser Tuser { get; set; }

        private void InitEditType(int userId)
        {
            EditType = "新增用户";
            if (userId > 0)
            {
                EditType = "修改用户";
            }
        }

        private void InitDept()
        {
            //初始化页面的部门（下拉列表中的选项）
            DeptList = _context.TDepts.Where<TDept>(dept => dept.IsValid == 1).ToList<TDept>();
        }

        private void InitUser(int userId)
        {
            Tuser = new TUser();
            if (userId > 0)
            {
                Tuser = _context.TUsers.Find(userId);//如果是修改，初始化该用户的信息
            }
        }

        public void OnGet()
        {
            string userId = Request.Query["userid"];
            int uid = Convert.ToInt32(userId);
            InitEditType(uid);
            InitDept();
            InitUser(uid);
        }

        //用户点保存按钮时执行，在form中不需要指定action，
        //按约定以Post方式提交的表单对应的方法是OnPost
        //这里的方法参数用 [FromForm] 修饰后可以自动将表单中控件的值赋给TUser对象中同名的属性，不区分大小写，
        //当字段比较多的时候，这种自动映射表单控件的值到对象属性上的方式让编码非常方便。
        public void OnPost([FromForm] TUser user)
        {
            string userId = Request.Query["userid"];
            if (int.TryParse(userId, out int uid))
            {
                if (uid > 0)
                {
                    user.ModifyTime = DateTime.Now;//表单中没有修改时间这个控件，所以要手动赋值。
                    ModifyUser(user); //user对象已经自动得到了表单中同名控件的值了（新增时也是一样的）。
                }
                else
                {
                    int newUserId = AddUser(user);
                    userId = newUserId.ToString();
                }
            }

            OnGet();//重新获取数据
            //执行完之后让页面重新加载一下避免用户刷新的时候数据再次回传
            Response.Redirect("/Auth/UserEdit?userid=" + userId);
        }


        private int AddUser(TUser user)
        {
            user.CreateTime = DateTime.Now;
            user.IsValid = 1;
            _context.TUsers.Add(user);
            _context.SaveChanges();

            return user.UserId;
        }


        private void ModifyUser(TUser user)
        {
            //设置序列化时的对中文的编码方式，为后续系列化TUser对象写入TLog表做准备
            JsonSerializerOptions options = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
            };

            List<TLog> logList = GenerateLog(); //初始化包含2条日志信息的列表

            //先找出要更改的实体
            TUser oldUser = _context.TUsers.Find(user.UserId);
            //将更改前的数据序列化成json后记录下来
            logList[0].TableData = JsonSerializer.Serialize<TUser>(oldUser, options);

            //保存修改后的值到TUser对象中，这里赋值后执行_context.SaveChanges()方法就会将修改后的值写入数据库了。
            oldUser.UserName = user.UserName;//用 [FromForm] 修饰后避免手写 Request.From["username"] 取值，非常方便。
            oldUser.Mobile = user.Mobile;
            oldUser.ModifyTime = user.ModifyTime;
            oldUser.SigninAcc = user.SigninAcc;
            oldUser.SigninPwd = user.SigninPwd;
            oldUser.DeptId = user.DeptId;
            oldUser.DeptName = user.DeptName;
            //将更改后的数据序列化成json后记录下来
            logList[1].TableData = JsonSerializer.Serialize<TUser>(oldUser, options);

            //保存数据到t_log表
            _context.TLogs.AddRange(logList);

            //将更改保存到数据库
            _context.SaveChanges();
        }


        /// <summary>
        /// 将更改前和更改后的数据保存到t_log表
        /// </summary>
        /// <returns></returns>
        private List<TLog> GenerateLog()
        {
            int userId = 0;
            string userName = "未知";

            //取出登录时设置的用户信息
            ClaimsPrincipal cp = HttpContext.User;
            if (cp.Identity.IsAuthenticated)
            {
                //取出登录时设置的所有用户信息
                List<Claim> claims = cp.Claims.ToList<Claim>();

                //通过传入Lambda表达式找出登录时设置的 UserId 值
                string uid = claims.Single<Claim>(option => option.Type == "UserId").Value;
                userId = Convert.ToInt32(uid);

                //通过传入Lambda表达式找出登录时设置的 UserName 值
                userName = claims.Single<Claim>(option => option.Type == "UserName").Value;
            }
            string batchNo = Guid.NewGuid().ToString();
            TLog beforeLog = new TLog
            {
                UserId = userId,
                UserName = userName,
                BatchNo = batchNo,
                TableName = "t_user",
                TableData = "",
                LogTime = DateTime.Now
            };
            TLog afterLog = new TLog
            {
                UserId = userId,
                UserName = userName,
                BatchNo = batchNo,
                TableName = "t_user",
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
