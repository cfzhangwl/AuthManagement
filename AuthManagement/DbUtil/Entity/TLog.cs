using System;
using System.Collections.Generic;

#nullable disable

namespace AuthManagement.DbUtil.Entity
{
    [Serializable]
    public partial class TLog
    {
        public int LogId { get; set; }
        public int? UserId { get; set; }
        public string UserName { get; set; }
        public string BatchNo { get; set; }
        public string TableName { get; set; }
        public string TableData { get; set; }
        public DateTime LogTime { get; set; }
    }
}
