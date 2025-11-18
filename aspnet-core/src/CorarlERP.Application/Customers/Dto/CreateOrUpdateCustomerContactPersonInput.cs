using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.Customers.Dto
{
  public  class CreateOrUpdateCustomerContactPersonInput
    {
        public Guid? Id { get; set; }
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DisplayNameAs { get; set; }
        public string PhoneNumber { get; set; }    
        public bool IsPrimary { get; set; }
        public string Email { get; set; }
       // public Guid CustomerId { get; set; }
    }

    public class CreateOrUpdateCustomerContactPersonExprotInput
    {
        public CustomerDetailOutput  Customer { get; set; }
        public Guid? Id { get; set; }
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DisplayNameAs { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsPrimary { get; set; }
        public string Email { get; set; }
    }
}
