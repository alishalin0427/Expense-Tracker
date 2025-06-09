using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExpenseTracker.Models
{
    public class Expense
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "請輸入描述")]
        [Display(Name = "描述")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "請輸入金額")]
        [Range(0.01, double.MaxValue, ErrorMessage = "金額必須大於0")]
        [Display(Name = "金額")]
        public double Amount { get; set; }

        [Required(ErrorMessage = "請選擇類別")]
        [Display(Name = "類別")]
        public string Category { get; set; } = string.Empty;

        [Required(ErrorMessage = "請選擇日期")]
        [Display(Name = "日期")]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; } = DateTime.Today;

    }
}