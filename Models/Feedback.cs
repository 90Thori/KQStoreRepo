//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace KQStore.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Feedback
    {
        public int FbId { get; set; }
        public int ProductId { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }
        public string FbImg { get; set; }
        public System.DateTime CreatedAt { get; set; }
        public int UserId { get; set; }
    
        public virtual Product Product { get; set; }
        public virtual User User { get; set; }
    }
}