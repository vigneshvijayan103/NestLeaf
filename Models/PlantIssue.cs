using System;
using System.Collections.Generic;

namespace NestLeaf.Models;

public partial class PlantIssue
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int ProductId { get; set; }

    public string Title { get; set; }

    public string Description { get; set; }

    public string Status { get; set; }

    public string Resolution { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? ResolvedAt { get; set; }

    public DateTime? DeletedAt { get; set; }
}