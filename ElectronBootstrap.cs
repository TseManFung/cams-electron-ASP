using ElectronNET.API.Entities;
using ElectronNET.API;

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
                    Width = 800,
                    Height = 600,
                    WebPreferences = new WebPreferences
                    {
                        NodeIntegration = true,
                        ContextIsolation = false
                    }
                };

                var window = await Electron.WindowManager.CreateWindowAsync(windowOptions);
                window.OnClosed += () => { Electron.App.Exit(); };
            }
        }
    }
}
