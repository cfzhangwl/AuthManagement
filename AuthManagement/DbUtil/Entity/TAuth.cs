using System;
using System.Collections.Generic;

#nullable disable

namespace AuthManagement.DbUtil.Entity
{
    [Serializable]
    public partial class TAuth
    {
        public int AuthId { get; set; }
        public short? TargetType { get; set; }
        public int? TargetId { get; set; }
        public string FuncCode { get; set; }
        public DateTime? CreateTime { get; set; }
        public DateTime? ModifyTime { get; set; }
    }
}
