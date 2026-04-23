using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using Econova.Infrastructure;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Econova
{
    /// <summary>
    /// Lógica de interacción para App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            SqliteDataService.Instance.InitializeDatabase();
            base.OnStartup(e);
        }
    }
}
