//------------------------------------------------------------------------------
// <auto-generated>
//     此代码已从模板生成。
//
//     手动更改此文件可能导致应用程序出现意外的行为。
//     如果重新生成代码，将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace AvariCapitalCRM.Models.Data
{
    using System;
    using System.Collections.Generic;
    
    public partial class Account
    {
        public long Id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string PasswordSalt { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string AvatarFileName { get; set; }
        public int RoleType { get; set; }
        public int RoleId { get; set; }
        public int State { get; set; }
        public string AccessIds { get; set; }
        public System.DateTime AddTime { get; set; }
    }
}
