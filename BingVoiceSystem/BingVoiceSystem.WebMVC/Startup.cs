using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(BingVoiceSystem.WebMVC.Startup))]
namespace BingVoiceSystem.WebMVC
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
