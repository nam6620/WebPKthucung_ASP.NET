using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebPKthucung.Models
{
    public class LienheModel
    {
        QLPKTHUCUNGEntities1 db = new QLPKTHUCUNGEntities1();

        [Display(Name = "Họ và tên:")]
        [Required(ErrorMessage = " Họ tên không được để trống. ")]
        public string Name { get; set; }

        [Display(Name = "Email:")]
        [Required(ErrorMessage = " Email không được để trống. ")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = " Tiêu đề không được để trống. ")]
        [Display(Name = "Tiêu đề:")]
        public string Subject { get; set; }

        [Required(ErrorMessage = " Nội dung không được để trống. ")]
        [Display(Name = "Nội dung:")]
        public string Message { get; set; }
    }
}