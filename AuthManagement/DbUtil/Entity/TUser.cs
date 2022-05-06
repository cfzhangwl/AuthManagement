using System;
using System.Collections.Generic;

#nullable disable

namespace AuthManagement.DbUtil.Entity
{
    [Serializable]
    public partial class TUser
    {
        public int UserId { get; set; }
        public string SigninAcc { get; set; }
        public string SigninPwd { get; set; }
        public string UserName { get; set; }
        public string Mobile { get; set; }
        public int? DeptId { get; set; }
        public string DeptName { get; set; }
        public short? IsValid { get; set; }
        public DateTime? CreateTime { get; set; }
        public DateTime? ModifyTime { get; set; }
    }
}
