using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(FileRepository.Startup))]
namespace FileRepository
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
