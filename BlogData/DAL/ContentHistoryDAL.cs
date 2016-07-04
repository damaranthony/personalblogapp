using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlogData.Data;

namespace BlogData.DAL
{
    public class ContentHistoryDal : GenericRepository<ContentHistory>
    {
        internal new BlogContext Context;
        internal new DbSet<ContentHistory> DbSet;

        public ContentHistoryDal(BlogContext c) : base(c)
        {
            Context = c;
            DbSet = c.Set<ContentHistory>();
        }

        public ContentHistory GetLatestContentHistory(int contentId)
        {
            //get the latest content history record by content ID
            return DbSet.OrderByDescending(c => c.CreatedDate).FirstOrDefault(c => c.ContentId == contentId);
        }

        public IEnumerable<ContentHistory> GetPublishedPosts(IEnumerable<Content> blogContents)
        {
            //get contents from history by content ID
            //check content if active
            //check publish date if later than or equal to current date and time
            //get all contents with published content state
            var contentHistory = DbSet.Where(b => b.ContentStateId.Equals(4)).ToList();

            return contentHistory.Where(b => blogContents.Select(c => c.Id).Contains(b.ContentId));
        }

    }
}
