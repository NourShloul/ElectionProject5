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
    
    public partial class PartyCandidate
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public PartyCandidate()
        {
            this.PartyLists = new HashSet<PartyList>();
        }
    
        public long id { get; set; }
        public string partyname { get; set; }
        public string electionarea { get; set; }
        public string email { get; set; }
        public Nullable<long> national_id { get; set; }
        public string gender { get; set; }
        public string birthdate { get; set; }
        public string religion { get; set; }
        public Nullable<long> ordercandidate { get; set; }
        public Nullable<long> fk_counter { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PartyList> PartyLists { get; set; }
        public virtual TempPartyCandidate TempPartyCandidate { get; set; }
    }
}
