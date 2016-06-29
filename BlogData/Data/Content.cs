namespace BlogData.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Content
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Content()
        {
            ContentHistories = new HashSet<ContentHistory>();
        }

        public int Id { get; set; }

        [StringLength(250)]
        [Required]
        public string Title { get; set; }

        [Required]
        [Display(Name = "Blog Post")]
        public string MainContent { get; set; }

        [Display(Name = "Publish Date")]
        public DateTime? PublishDate { get; set; }

        [StringLength(128)]
        public string PublishedBy { get; set; }

        [StringLength(128)]
        public string UpdatedBy { get; set; }

        [StringLength(128)]
        public string Author { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public bool? IsDeleted { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ContentHistory> ContentHistories { get; set; }
    }
}
