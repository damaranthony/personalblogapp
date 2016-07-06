namespace BlogData.Data
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("ContentHistory")]
    public partial class ContentHistory
    {
        public int Id { get; set; }

        public int ContentId { get; set; }

        public int ContentStateId { get; set; }

        [StringLength(250)]
        public string Title { get; set; }

        public string MainContent { get; set; }

        public string Comment { get; set; }

        [StringLength(128)]
        public string CreatedBy { get; set; }

        public DateTime? CreatedDate { get; set; }

        public virtual Content Content { get; set; }

        public virtual ContentState ContentState { get; set; }
    }
}
