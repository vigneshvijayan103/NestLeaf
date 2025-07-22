
using System;
using System.Collections.Generic;

namespace NestLeaf.Models;

public partial class Address
{
    public int Id { get; set; }

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

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}