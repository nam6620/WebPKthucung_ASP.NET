using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebPKthucung.Models
{
    public class BaoCaoThang
    {
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public System.DateTime ThangBan { get; set; }
        public Nullable<decimal> TongTien { get; set; }
    }
}