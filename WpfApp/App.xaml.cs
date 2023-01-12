using System;
using System.Windows;

namespace WpfApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            // Setup Rhino Assemlby Resolver before any Calls to Rhino Happen (DO NOT MOVE THIS)
            RhinoInside.Resolver.Initialize();

            base.OnStartup(e);
        }
    }
}