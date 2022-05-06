using System;
using System.Collections.Generic;

#nullable disable

namespace AuthManagement.DbUtil.Entity
{
    [Serializable]
    public partial class TDept
    {
        public int DeptId { get; set; }
        public string DeptName { get; set; }
        public short? IsValid { get; set; }
        public DateTime? CreateTime { get; set; }
        public DateTime? ModifyTime { get; set; }
    }
}
