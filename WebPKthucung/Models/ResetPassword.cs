using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace WebPKthucung.Models
{
    public class ResetPassword
    {
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }

        [Required]
        public String Resetcode { get; set; }
    }
}