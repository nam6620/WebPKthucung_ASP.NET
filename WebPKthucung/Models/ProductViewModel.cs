using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebPKthucung.Models
{
    public class ProductViewModel
    {
        QLPKTHUCUNGEntities1 db = new QLPKTHUCUNGEntities1();
        public String MASP { get; set; }
        public string TENSP { get; set; }
        public decimal? DONGIABAN { get; set; }
        public string HINHANH { get; set; }
        public String MATH { get; set; }
        public String MALOAI { get; set; }
        public String TENTH { get; set; }
        public String TENLOAI { get; set; }
        public int? SOLUONG { get; set; }
        public string MOTA { get; set; }
        public String TENMAUSAC { get; set; }
        public int TENKICHTHUOC { get; set; }
        public string HINH1 { get; set; }
        public string LOGO { get; set; }
        public string THANHTOANON { get; set; }
        public bool GIAMGIA { get; set; }
    }
}