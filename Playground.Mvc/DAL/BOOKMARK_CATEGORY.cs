//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Playground.Mvc.DAL
{
    using System;
    using System.Collections.Generic;
    
    public partial class BOOKMARK_CATEGORY
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public BOOKMARK_CATEGORY()
        {
            this.BOOKMARKs = new HashSet<BOOKMARK>();
        }
    
        public int CATEGORY_ID { get; set; }
        public string CATEGORY_NAME { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BOOKMARK> BOOKMARKs { get; set; }
    }
}