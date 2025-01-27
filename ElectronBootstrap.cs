using ElectronNET.API.Entities;
using ElectronNET.API;
using System.Drawing;

namespace Campus_Asset_Management_System
{
    public class ElectronBootstrap
    {
        public static async Task InitAsync()
        {
            if (HybridSupport.IsElectronActive)
            {
                var windowOptions = new BrowserWindowOptions
                {
                    MinWidth = 800,
                    MinHeight = 600,
                    Title = "Campus Asset Management System",
                    Icon = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "favicon.ico"),
                    WebPreferences = new WebPreferences
                    {
                        NodeIntegration = true,
                        ContextIsolation = false,
                        DevTools = true // TODO: turn off in production
                    },
                    AutoHideMenuBar = true,
                };

                var window = await Electron.WindowManager.CreateWindowAsync(windowOptions);
                window.OnClosed += () => { Electron.App.Exit(); };

                MenuItem[] menuitem = new MenuItem[] { };
                Electron.Menu.SetApplicationMenu(menuitem);
            }
        }
    }
}
