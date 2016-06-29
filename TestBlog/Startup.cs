using System;
using System.Linq;
using BlogData.Data;
using BlogData.DAL;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Owin;
using TestBlog.Models;

[assembly: OwinStartupAttribute(typeof(TestBlog.Startup))]
namespace TestBlog
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            CreateRoles();
        }

        private void CreateRoles()
        {
            var context = new ApplicationDbContext();
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            
            var unitOfWork = new UnitOfWork();

            //check if admin exist on roles
            if (!roleManager.RoleExists("Admin"))
            {
                var role = new IdentityRole
                {
                    Name = "Admin"
                };
                roleManager.Create(role);
            }
            //check if editor role exist
            if (roleManager.RoleExists("Editor")) return;
            {
                var role = new IdentityRole { Name = "Editor"};
                roleManager.Create(role);
            }

            if (unitOfWork.ContentStateRepository.GetAll().Any()) return;

            unitOfWork.ContentStateRepository.Insert(new ContentState { Title = "Draft", CreatedDate = DateTime.Now, IsDeleted = false });
            unitOfWork.ContentStateRepository.Insert(new ContentState { Title = "Ready to publish", CreatedDate = DateTime.Now, IsDeleted = false });
            unitOfWork.ContentStateRepository.Insert(new ContentState { Title = "Reject", CreatedDate = DateTime.Now, IsDeleted = false });
            unitOfWork.ContentStateRepository.Insert(new ContentState { Title = "Published", CreatedDate = DateTime.Now, IsDeleted = false });
            unitOfWork.ContentStateRepository.Insert(new ContentState { Title = "Archived", CreatedDate = DateTime.Now, IsDeleted = false });

            unitOfWork.Save();
        }
    }
}
