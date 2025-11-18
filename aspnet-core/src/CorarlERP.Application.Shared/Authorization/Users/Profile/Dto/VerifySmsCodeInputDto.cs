using System;

namespace CorarlERP.Authorization.Users.Profile.Dto
{
    public class VerifySmsCodeInputDto
    {
        public string Code { get; set; }
    }
    public class UpdateUserProfileImageInput {
    
        public Guid? UserImgId { get; set; }
       
    }

}