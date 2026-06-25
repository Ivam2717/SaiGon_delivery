using System.ComponentModel.DataAnnotations;

namespace Saigon_delivery.Models.ViewModels
{
    public class OrderViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập tên người nhận")]
        [Display(Name = "Tên người nhận")]
        public string ReceiverName { get; set; } = null!;

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại người nhận")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        [Display(Name = "Số điện thoại người nhận")]
        public string ReceiverPhone { get; set; } = null!;

        [Required(ErrorMessage = "Vui lòng nhập địa chỉ lấy hàng")]
        [Display(Name = "Địa chỉ lấy hàng")]
        public string PickupAddress { get; set; } = null!;

        [Required(ErrorMessage = "Vui lòng nhập địa chỉ giao hàng")]
        [Display(Name = "Địa chỉ giao hàng")]
        public string DeliveryAddress { get; set; } = null!;

        [Required(ErrorMessage = "Vui lòng nhập khoảng cách")]
        [Range(0.1, 50, ErrorMessage = "Khoảng cách từ 0.1 đến 50 km")]
        [Display(Name = "Khoảng cách ước tính (km)")]
        public decimal Distance { get; set; }
    }
}