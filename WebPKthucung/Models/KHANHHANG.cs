//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WebPKthucung.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class KHANHHANG
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public KHANHHANG()
        {
            this.DONDATHANGs = new HashSet<DONDATHANG>();
        }
    
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
        public string MAXA { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DONDATHANG> DONDATHANGs { get; set; }
        public virtual XA XA { get; set; }
    }
}
