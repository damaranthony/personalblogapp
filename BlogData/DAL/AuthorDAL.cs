using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlogData.Data;

namespace BlogData.DAL
{
    public class AuthorDal : GenericRepository<Author>
    {
        internal new BlogContext Context;
        internal new DbSet<Author> DbSet;
        public AuthorDal(BlogContext c) : base(c)
        {
            Context = c;
            DbSet = c.Set<Author>();
        }

        public string GetAuthorName(string userId)
        {
            var firstOrDefault = DbSet.FirstOrDefault(a => a.UserId == userId);
            return firstOrDefault != null ? firstOrDefault.Name : string.Empty;
        }
    }
}
