using Abp.AutoMapper;
using CorarlERP.Authorization.Users;
using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.Dto
{
    [AutoMapFrom(typeof(User))]
    public class UserDto
    {
        public long Id { get; set; }
        public string UserName { get; set; }
    }
}
