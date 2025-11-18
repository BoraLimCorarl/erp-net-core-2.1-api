using Abp.Application.Services.Dto;

namespace CorarlERP.Sessions.Dto
{
    public class UserLoginInfoDto : EntityDto<long>
    {
        public string Name { get; set; }

        public string Surname { get; set; }

        public string UserName { get; set; }

        public string EmailAddress { get; set; }

        public string ProfilePictureId { get; set; }
    }

    public class UserLoginSummaryInfoDto : EntityDto<long>
    {
        public string Name { get; set; }

        public string Surname { get; set; }

        public string UserName { get; set; }

        public string FullName { get => Surname + " " + Name; }

        public string EmailAddress { get; set; }

        public string ProfilePictureId { get; set; }
    }
}
