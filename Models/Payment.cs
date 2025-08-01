using System;
using System.Collections.Generic;

namespace NestLeaf.Models;

public partial class Payment
{
    public int Id { get; set; }

    public int OrderId { get; set; }

    public int UserId { get; set; }

    public decimal Amount { get; set; }

    public string Status { get; set; }

    public string PaymentMethod { get; set; }

    public DateTime? PaymentDate { get; set; }
}