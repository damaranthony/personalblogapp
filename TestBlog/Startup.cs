using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(TestBlog.Startup))]
namespace TestBlog
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
