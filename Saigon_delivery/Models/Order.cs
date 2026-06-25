using System;
using System.Collections.Generic;

namespace Saigon_delivery.Models;

public partial class Order
{
    public int Id { get; set; }

    public Guid CustomerId { get; set; }

    public Guid? ShipperId { get; set; }

    public string ReceiverName { get; set; } = null!;

    public string ReceiverPhone { get; set; } = null!;

    public string DeliveryAddress { get; set; } = null!;

    public decimal Distance { get; set; }

    public decimal ShipFee { get; set; }

    public string Status { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public string PickupAddress { get; set; } = null!;

    public virtual User Customer { get; set; } = null!;

    public virtual User? Shipper { get; set; }

    public virtual ShipperRating? ShipperRating { get; set; }
}
