
using System;
using System.Collections.Generic;
using NestLeaf.Enum;

namespace NestLeaf.Models;

public partial class Order
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public DateTime OrderDate { get; set; }

    public decimal TotalAmount { get; set; }

    public OrderStatus Status { get; set; }

    public bool IsPaid { get; set; }

    public int ShippingAddressId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? CancelledAt { get; set; }

    public string CancelledBy {  get; set; }

  

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual Address ShippingAddress { get; set; }
}