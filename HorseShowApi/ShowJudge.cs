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
    
    public partial class ShowJudge
    {
        public int ShowJudgesId { get; set; }
        public string JudgeName { get; set; }
        public string JudgeCode { get; set; }
        public string IpadUdid { get; set; }
        public Nullable<int> ShowId { get; set; }
        public Nullable<int> JudgeId { get; set; }
        public Nullable<int> ShowJudgeCodeId { get; set; }
        public Nullable<int> ShowIpadUdidId { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public string JudgeAbvr { get; set; }
    }
}
