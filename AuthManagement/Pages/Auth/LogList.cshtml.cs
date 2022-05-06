using AuthManagement.DbUtil.Entity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AuthManagement.Pages.Auth
{
    public class LogListModel : PageModel
    {
        private readonly AuthDbContext _context;

        //���캯���ж� AuthDbContext ������ע��
        public LogListModel(AuthDbContext context)
        {
            _context = context;
        }

        public List<TLog> LogList { get; private set; }//���崫�ݸ�ҳ�������

        private void InitLogList() //�����Ը�ֵ
        {
            //ȡ�����3������ݣ��б�ҳ����ʾ table_data �ֶε����ݣ���Ϊ����̫���ˣ�
            //������ÿ��ĩβ��һ����������Ӵ򿪵�����ҳ��ȥ����
            //�� Select ȡ TLog �е� UserName,TableName,LogTime,BatchNo ��3���ֶβ���ֵ,Ȼ���� Distinct()ȥ���ظ�ֵ
            LogList = _context.TLogs.Where<TLog>(x => x.LogTime > DateTime.Now.AddDays(-3).Date)
                .Select(x => new TLog { UserName = x.UserName, TableName = x.TableName, LogTime = x.LogTime, BatchNo = x.BatchNo })
                .Distinct().ToList<TLog>();
        }

        public void OnGet()
        {
            InitLogList();
        }
    }
}