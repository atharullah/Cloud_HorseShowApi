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
    
    public partial class Rating
    {
        public int RatingId { get; set; }
        public Nullable<int> ShowClassJudgesId { get; set; }
        public Nullable<int> ShowHorsesId { get; set; }
        public Nullable<decimal> Type { get; set; }
        public Nullable<decimal> HandN { get; set; }
        public Nullable<decimal> BandT { get; set; }
        public Nullable<decimal> Legs { get; set; }
        public Nullable<decimal> Move { get; set; }
        public Nullable<System.DateTime> Date { get; set; }
        public string HorseCatalog { get; set; }
        public string IpadUdid { get; set; }
        public Nullable<int> ShowId { get; set; }
        public Nullable<int> ShowClassId { get; set; }
        public Nullable<decimal> SumScore { get; set; }
        public string JudgeCode { get; set; }
        public string JudgeName { get; set; }
    }
}
