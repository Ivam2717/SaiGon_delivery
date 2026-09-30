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
        [Display(Name = "Số điện thoại")]
        public string ReceiverPhone { get; set; } = null!;

        [Required(ErrorMessage = "Vui lòng chọn địa chỉ lấy hàng")]
        [Display(Name = "Địa chỉ lấy hàng")]
        public string PickupAddress { get; set; } = null!;

        [Required(ErrorMessage = "Vui lòng chọn địa chỉ giao hàng")]
        [Display(Name = "Địa chỉ giao hàng")]
        public string DeliveryAddress { get; set; } = null!;

        [Required]
        [Range(0.01, 200, ErrorMessage = "Không thể tính khoảng cách, vui lòng thử lại")]
        public decimal Distance { get; set; }
    }
}