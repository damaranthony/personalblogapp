using System;

namespace BlogData.Data
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("ContentStateToRole")]
    public  class ContentStateToRole
    {
        public int Id { get; set; }

        public int ContentStateId { get; set; }

        [Required]
        [StringLength(128)]
        public string RoleId { get; set; }

        [StringLength(128)]
        public string CreatedBy { get; set; }

        public DateTime? CreatedDate { get; set; }

        [StringLength(128)]
        public string UpdatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public bool? IsDeleted { get; set; }

        public virtual AspNetRole AspNetRole { get; set; }

        public virtual ContentState ContentState { get; set; }
    }
}
