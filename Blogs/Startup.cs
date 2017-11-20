using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Blogs.Startup))]
namespace Blogs
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
