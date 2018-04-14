using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(BingVoiceSystem.Startup))]
namespace BingVoiceSystem
{
    public partial class Startup {
        public void Configuration(IAppBuilder app) {
            ConfigureAuth(app);
            //Rules ruleslist = new Rules();
            //ruleslist.AddRule("I am pending approval", "Or am I?", "PendingRules");
        }
    }
}
