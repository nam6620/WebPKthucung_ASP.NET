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
    
    public partial class XA
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public XA()
        {
            this.KHANHHANGs = new HashSet<KHANHHANG>();
        }
    
        public string MAXA { get; set; }
        public string TENTINH { get; set; }
        public string MAHUYEN { get; set; }
    
        public virtual HUYEN HUYEN { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<KHANHHANG> KHANHHANGs { get; set; }
    }
}