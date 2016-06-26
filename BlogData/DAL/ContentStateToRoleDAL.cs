using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlogData.Data;

namespace BlogData.DAL
{
 
    public class ContentStateToRoleDal : GenericRepository<ContentStateToRole>
    {
        internal BlogContext Context;
        internal DbSet<ContentStateToRole> DbSet;
        public ContentStateToRoleDal(BlogContext c) : base(c)
        {
            Context = c;
            DbSet = c.Set<ContentStateToRole>();
        }
        
        public ContentStateToRole GetByStateType(ContentStateType cType)
        {
            
            switch (cType)
            {
                case ContentStateType.Draft:
                    return DbSet.FirstOrDefault(c => c.ContentState.Title.ToLower().Equals("draft"));
                case ContentStateType.ReadyToPublish:
                    return DbSet.FirstOrDefault(c => c.ContentState.Title.ToLower().Equals("ready to publish"));
                case ContentStateType.Reject:
                    return DbSet.FirstOrDefault(c => c.ContentState.Title.ToLower().Equals("Reject"));
                case ContentStateType.Published:
                    return DbSet.FirstOrDefault(c => c.ContentState.Title.ToLower().Equals("Published"));
                case ContentStateType.Archived:
                    return DbSet.FirstOrDefault(c => c.ContentState.Title.ToLower().Equals("Archived"));
                default:
                    return new ContentStateToRole();
            }
        }

        public List<ContentStateToRole> GetContentStateRolesByRoleId(string roleId)
        {
            return DbSet.Where(c => c.RoleId == roleId).ToList();
        }
    }
    
}
