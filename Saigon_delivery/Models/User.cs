using System;
using System.Collections.Generic;

namespace Saigon_delivery.Models;

public partial class User
{
    public Guid Id { get; set; }

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string FullName { get; set; } = null!;

    public string? Phone { get; set; }

    public string Role { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<Order> OrderCustomers { get; set; } = new List<Order>();

    public virtual ICollection<Order> OrderShippers { get; set; } = new List<Order>();

    public virtual ICollection<ShipperRating> ShipperRatingCustomers { get; set; } = new List<ShipperRating>();

    public virtual ICollection<ShipperRating> ShipperRatingShippers { get; set; } = new List<ShipperRating>();
}
