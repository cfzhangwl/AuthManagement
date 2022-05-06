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
            EditType = "�����û�";
            if (userId > 0)
            {
                EditType = "�޸��û�";
            }
        }

        private void InitDept()
        {
            //��ʼ��ҳ��Ĳ��ţ������б��е�ѡ�
            DeptList = _context.TDepts.Where<TDept>(dept => dept.IsValid == 1).ToList<TDept>();
        }

        private void InitUser(int userId)
        {
            Tuser = new TUser();
            if (userId > 0)
            {
                Tuser = _context.TUsers.Find(userId);//������޸ģ���ʼ�����û�����Ϣ
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

        //�û��㱣�水ťʱִ�У���form�в���Ҫָ��action��
        //��Լ����Post��ʽ�ύ�ı���Ӧ�ķ�����OnPost
        //����ķ��������� [FromForm] ���κ�����Զ������пؼ���ֵ����TUser������ͬ�������ԣ������ִ�Сд��
        //���ֶαȽ϶��ʱ�������Զ�ӳ����ؼ���ֵ�����������ϵķ�ʽ�ñ���ǳ����㡣
        public void OnPost([FromForm] TUser user)
        {
            string userId = Request.Query["userid"];
            if (int.TryParse(userId, out int uid))
            {
                if (uid > 0)
                {
                    user.ModifyTime = DateTime.Now;//����û���޸�ʱ������ؼ�������Ҫ�ֶ���ֵ��
                    ModifyUser(user); //user�����Ѿ��Զ��õ��˱���ͬ���ؼ���ֵ�ˣ�����ʱҲ��һ���ģ���
                }
                else
                {
                    int newUserId = AddUser(user);
                    userId = newUserId.ToString();
                }
            }

            OnGet();//���»�ȡ����
            //ִ����֮����ҳ�����¼���һ�±����û�ˢ�µ�ʱ�������ٴλش�
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
            //�������л�ʱ�Ķ����ĵı��뷽ʽ��Ϊ����ϵ�л�TUser����д��TLog����׼��
            JsonSerializerOptions options = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
            };

            List<TLog> logList = GenerateLog(); //��ʼ������2����־��Ϣ���б�

            //���ҳ�Ҫ���ĵ�ʵ��
            TUser oldUser = _context.TUsers.Find(user.UserId);
            //������ǰ���������л���json���¼����
            logList[0].TableData = JsonSerializer.Serialize<TUser>(oldUser, options);

            //�����޸ĺ��ֵ��TUser�����У����︳ֵ��ִ��_context.SaveChanges()�����ͻὫ�޸ĺ��ֵд�����ݿ��ˡ�
            oldUser.UserName = user.UserName;//�� [FromForm] ���κ������д Request.From["username"] ȡֵ���ǳ����㡣
            oldUser.Mobile = user.Mobile;
            oldUser.ModifyTime = user.ModifyTime;
            oldUser.SigninAcc = user.SigninAcc;
            oldUser.SigninPwd = user.SigninPwd;
            oldUser.DeptId = user.DeptId;
            oldUser.DeptName = user.DeptName;
            //�����ĺ���������л���json���¼����
            logList[1].TableData = JsonSerializer.Serialize<TUser>(oldUser, options);

            //�������ݵ�t_log��
            _context.TLogs.AddRange(logList);

            //�����ı��浽���ݿ�
            _context.SaveChanges();
        }


        /// <summary>
        /// ������ǰ�͸��ĺ�����ݱ��浽t_log��
        /// </summary>
        /// <returns></returns>
        private List<TLog> GenerateLog()
        {
            int userId = 0;
            string userName = "δ֪";

            //ȡ����¼ʱ���õ��û���Ϣ
            ClaimsPrincipal cp = HttpContext.User;
            if (cp.Identity.IsAuthenticated)
            {
                //ȡ����¼ʱ���õ������û���Ϣ
                List<Claim> claims = cp.Claims.ToList<Claim>();

                //ͨ������Lambda���ʽ�ҳ���¼ʱ���õ� UserId ֵ
                string uid = claims.Single<Claim>(option => option.Type == "UserId").Value;
                userId = Convert.ToInt32(uid);

                //ͨ������Lambda���ʽ�ҳ���¼ʱ���õ� UserName ֵ
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
