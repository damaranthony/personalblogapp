namespace BlogData.Data
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Author")]
    public partial class Author
    {
        public int Id { get; set; }

        [StringLength(128)]
        public string UserId { get; set; }

        [StringLength(250)]
        public string Name { get; set; }

        public bool? IsDeleted { get; set; }

        public virtual AspNetUser AspNetUser { get; set; }
    }
}
