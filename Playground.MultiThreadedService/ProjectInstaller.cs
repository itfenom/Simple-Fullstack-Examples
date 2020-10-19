using System.ComponentModel;
using System.Configuration;

namespace Playground.MultiThreadedService
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : System.Configuration.Install.Installer
    {
        public ProjectInstaller()
        {
            InitializeComponent();

            string configFile = System.Reflection.Assembly.GetExecutingAssembly().Location;
            Configuration config = ConfigurationManager.OpenExeConfiguration(configFile);

            serviceInstaller1.ServiceName = config.AppSettings.Settings["Service.Name"].Value;
            serviceInstaller1.Description = config.AppSettings.Settings["Program.Name"].Value;
            serviceInstaller1.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            serviceProcessInstaller1.Account = System.ServiceProcess.ServiceAccount.User;
            //serviceProcessInstaller1.Username = "";
            //serviceProcessInstaller1.Password = "";
        }
    }
}
