using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CorarlERP.SignUps.Dto
{
    public class CreateOrUpdateSignUpInput
    {

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string CompanyOrStoreName { get; set; }
        public string Position { get; set; }
        public string PhoneNumber { get; set; }
        public string SignUpCode { get; set; }
        public bool IsActive { get; set; }
        public Guid? Id { get; set; }
        public string ReferralCode { get; set; }
        [MaxLength(125)]
        public string Description { get; set; }
    }
    public class CreateSignUpOutput
    {
        public Guid Id { get; set; }
        public long? ReferralId { get; set; }
        public string ReferralName { get; set; }
    }
    public class UpdateStatusInput
    {

        public Guid Id { get; set; }
        public SignUp.EnumStatus Status { get; set; }
    }
    public class FindStatusOutput
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

}
