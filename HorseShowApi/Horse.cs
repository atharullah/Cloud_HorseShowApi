//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace HorseShowAPI
{
    using System;
    using System.Collections.Generic;
    
    public partial class Horse
    {
        public int HorseId { get; set; }
        public string HorseName { get; set; }
        public string HorsePassport { get; set; }
        public string Gender { get; set; }
        public Nullable<System.DateTime> DateOfBirth { get; set; }
        public string OwnerName { get; set; }
        public string OwnerAddress { get; set; }
        public int id { get; set; }
        public Nullable<bool> IsApprove { get; set; }
    }
}
