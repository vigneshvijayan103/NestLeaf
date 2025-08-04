﻿namespace NestLeaf.Dto
{
    public class AddressDto
    {

     
        public int UserId { get; set; }
        public string FullName { get; set; }

        public string PhoneNumber { get; set; }

        public string AddressLine1 { get; set; }

        public string AddressLine2 { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string Pincode { get; set; }

        public string Country { get; set; }

        public bool IsDefault { get; set; }


    }
}
