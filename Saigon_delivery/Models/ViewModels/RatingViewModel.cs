using System.ComponentModel.DataAnnotations;

namespace Saigon_delivery.Models.ViewModels
{
    public class RatingViewModel
    {
        public int OrderId { get; set; }
        public Guid ShipperId { get; set; }
        public string ShipperName { get; set; } = null!;

        [Required(ErrorMessage = "Vui lòng chọn số sao")]
        [Range(1, 5, ErrorMessage = "Số sao từ 1 đến 5")]
        [Display(Name = "Đánh giá")]
        public byte Stars { get; set; }

        [MaxLength(500, ErrorMessage = "Nhận xét tối đa 500 ký tự")]
        [Display(Name = "Nhận xét")]
        public string? Comment { get; set; }
    }
}