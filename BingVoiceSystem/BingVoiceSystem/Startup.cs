using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(BingVoiceSystem.Startup))]
namespace BingVoiceSystem
{
    public partial class Startup {
        public void Configuration(IAppBuilder app) {
            ConfigureAuth(app);
        }
    }
}
