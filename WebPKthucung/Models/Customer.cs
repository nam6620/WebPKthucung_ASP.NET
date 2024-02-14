using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebPKthucung.Models
{
    public class Customer
    {
        QLPKTHUCUNGEntities1 db = new QLPKTHUCUNGEntities1();
        public string MAKH { get; set; }
        public string HOTENKH { get; set; }
        public string DIENTHOAI { get; set; }
        public string DAICHI { get; set; }
        public string TENDNKH { get; set; }
        public string MATKHAUKH { get; set; }
        public string EMAIL { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public System.DateTime NGAYSINH { get; set; }
        public string HINHANH { get; set; }
        public string KHOIPHUCMK { get; set; }
        public string iDIACHI { get; set; }
        public string MAXA { get; set; }
        public int TUOI { get; set; }
    }
}