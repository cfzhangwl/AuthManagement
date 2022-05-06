using System.Collections.Generic;
using System.Linq;
using AuthManagement.DbUtil.Entity;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuthManagement.Pages.Auth
{
    public class LogDetailModel : PageModel
    {
        private readonly AuthDbContext _context;

        //���캯���ж� AuthDbContext ������ע��
        public LogDetailModel(AuthDbContext context)
        {
            _context = context;
        }

        public List<TLog> LogDetails { get; private set; }//���崫�ݸ�ҳ�������

        public void OnGet()
        {
            string batNo = Request.Query["batno"];
            //��lambda���ʽ�� BatchNo Ϊ������ѯ���������ת��Ϊ���� List<TLog>
            LogDetails = _context.TLogs.Where<TLog>(x => x.BatchNo == batNo).ToList<TLog>();
        }
    }
}