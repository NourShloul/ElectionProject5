//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Project5.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class User
    {
        public long id { get; set; }
        public Nullable<long> national_id { get; set; }
        public string name { get; set; }
        public string electionarea { get; set; }
        public Nullable<long> voteLocal { get; set; }
        public Nullable<long> voteParty { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public string gender { get; set; }
        public string religion { get; set; }
        public string birthdate { get; set; }
    }
}
