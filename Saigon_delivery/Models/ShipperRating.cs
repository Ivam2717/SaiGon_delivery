using System;
using System.Collections.Generic;

namespace Saigon_delivery.Models;

public partial class ShipperRating
{
    public int Id { get; set; }

    public int OrderId { get; set; }

    public Guid CustomerId { get; set; }

    public Guid ShipperId { get; set; }

    public byte Stars { get; set; }

    public string? Comment { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual User Customer { get; set; } = null!;

    public virtual Order Order { get; set; } = null!;

    public virtual User Shipper { get; set; } = null!;
}
