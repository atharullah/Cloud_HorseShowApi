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
    
    public partial class Show
    {
        public int ShowId { get; set; }
        public string ShowName { get; set; }
        public Nullable<int> CountryId { get; set; }
        public string Location { get; set; }
        public Nullable<System.DateTime> StartDate { get; set; }
        public Nullable<System.DateTime> EndDate { get; set; }
        public bool IsPointFive { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<int> ShowTypeId { get; set; }
        public Nullable<int> MaxJudgeNumber { get; set; }
        public Nullable<int> MaxJudgePerClass { get; set; }
        public Nullable<int> ScoreSystemId { get; set; }
        public Nullable<int> ChampionshipMethodId { get; set; }
        public Nullable<int> MaxHorseInChampionship { get; set; }
    }
}
