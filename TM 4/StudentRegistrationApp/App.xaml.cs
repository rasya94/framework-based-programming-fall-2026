using System.Configuration;
using System.Data;
using System.Windows;

namespace StudentRegistrationApp
{
    public partial class App : Application
    {
        public App()
        {
            Database.Initialize();
        }
    }
}

