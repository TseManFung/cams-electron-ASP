using Campus_Asset_Management_System.Models;
using ElectronNET.API;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using RFIDReaderAPI;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;

namespace Campus_Asset_Management_System.Controllers
{
    public class RfidReaderController : Controller
    {
        private readonly ILogger<RfidReaderController> _logger;

        public RfidReaderController(ILogger<RfidReaderController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            // 取得 USB 裝置列表
            Electron.IpcMain.On("getUsbDeviceList", fun => 
            {
                var mainWindow = Electron.WindowManager.BrowserWindows.First();
                var devices = RFIDReader.GetUsbHidDeviceList();
                
                
                var cleanedDevices = devices.Select(device =>
                {
                    var parts = device.Split(new[] { '&' }, StringSplitOptions.RemoveEmptyEntries);
                    return parts.Length > 0 ? parts[0] : device;
                }).ToArray();

                var result = new
                {
                    devices = devices
                };
                Console.WriteLine("USB Devices: " + result);
                Electron.IpcMain.Send(mainWindow, "returnUsbDeviceList", JsonConvert.SerializeObject(result));
            }
            );

            // 連接 RFID Reader

            // 斷開 RFID Reader

            // 單次讀取

            // 開始循環讀取

            // 停止循環讀取

            // 寫入data to 標籤

            // lock 標籤

            // set RFID Reader power



            return View();
        }

        // TODO: will be deleted
        public IActionResult web()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
