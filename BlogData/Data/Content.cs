namespace BlogData.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

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
        [Display(Name = "Post Title")]
        public string Title { get; set; }

        [Required]
        [Display(Name = "Main Content")]
        public string MainContent { get; set; }

        public DateTime? PublishDate { get; set; }

        [StringLength(128)]
        public string PublishedBy { get; set; }

        [StringLength(128)]
        public string UpdatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public bool? IsDeleted { get; set; }

        [StringLength(250)]
        public string Author { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ContentHistory> ContentHistories { get; set; }
    }
}
