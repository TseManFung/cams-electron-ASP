using Campus_Asset_Management_System.Models;
using ElectronNET.API;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using RFIDReaderAPI;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Campus_Asset_Management_System.RfidScanner;
using RFIDReaderAPI.Interface;
using RFIDReaderAPI.Models;

namespace Campus_Asset_Management_System.Controllers
{
    public class RfidReaderController : Controller
    {
        private readonly ILogger<RfidReaderController> _logger;
        private readonly UsbRfidScanner _rfidScanner;
        

        public RfidReaderController(ILogger<RfidReaderController> logger)
        {
            _logger = logger;
            _rfidScanner = new UsbRfidScanner();
            _rfidScanner.rfidReaderMessage.funcGetMainWindow = getMainWindow;
        }

        private BrowserWindow getMainWindow()
        {
            return Electron.WindowManager.BrowserWindows.First();
        }
        public IActionResult Index()
        {
            // 取得 USB 裝置列表
            Electron.IpcMain.On("getUsbDeviceList", fun =>
            {
                try
                {
                    Electron.IpcMain.Send(getMainWindow(), "replyGetUsbDeviceList", _rfidScanner.GetUsbDeviceList());
                }
                catch (Exception e)
                {
                    Electron.IpcMain.Send(getMainWindow(), "replyGetUsbDeviceList", JsonMaker.makeErrorJson(e));
                }
            }
            );

            // 連接 RFID Reader
            Electron.IpcMain.On("connectUsbRfidReader", (indexOfUsbDeviceList) =>
            {
                try
                {
                    int i = Convert.ToInt32(indexOfUsbDeviceList);
                    Electron.IpcMain.Send(getMainWindow(), "replyConnectUsbRfidReader", _rfidScanner.ConnectUsbRfidReader(i));
                }
                catch (Exception e)
                {
                    Electron.IpcMain.Send(getMainWindow(), "replyConnectUsbRfidReader", JsonMaker.makeErrorJson(e));
                }
            }
            );
            // get conected device list
            Electron.IpcMain.On("getConnectedDeviceList", fun =>
            {
                try
                {
                    Electron.IpcMain.Send(getMainWindow(), "replyGetConnectedDeviceList", _rfidScanner.GetConnectedDeviceList());
                }
                catch (Exception e)
                {
                    Electron.IpcMain.Send(getMainWindow(), "replyGetConnectedDeviceList", JsonMaker.makeErrorJson(e));
                }
            }
            );
            // 斷開單個 RFID Reader
            Electron.IpcMain.On("disconnectUsbRfidReader", (indexOfConnectedDeviceList) =>
            {
                try
                {
                    int i = Convert.ToInt32(indexOfConnectedDeviceList);
                    Electron.IpcMain.Send(getMainWindow(), "replyDisconnectUsbRfidReader", _rfidScanner.DisconnectUsbRfidReader(i));
                }
                catch (Exception e)
                {
                    Electron.IpcMain.Send(getMainWindow(), "replyDisconnectUsbRfidReader", JsonMaker.makeErrorJson(e));
                }
            }
            );
            // 斷開所有 RFID Reader
            Electron.IpcMain.On("disconnectAllUsbRfidReader", fun =>
            {
                try
                {
                    Electron.IpcMain.Send(getMainWindow(), "replyDisconnectAllUsbRfidReader", _rfidScanner.DisconnectAllUsbRfidReader());
                }
                catch (Exception e)
                {
                    Electron.IpcMain.Send(getMainWindow(), "replyDisconnectAllUsbRfidReader", JsonMaker.makeErrorJson(e));
                }
            }
            );
            // check connection status
            Electron.IpcMain.On("checkConnectionStatus", (indexOfConnectedDeviceList) =>
            {
                try
                {
                    int i = Convert.ToInt32(indexOfConnectedDeviceList);
                    Electron.IpcMain.Send(getMainWindow(), "replyCheckConnectionStatus", _rfidScanner.CheckConnectionStatus(i));
                }
                catch (Exception e)
                {
                    Electron.IpcMain.Send(getMainWindow(), "replyCheckConnectionStatus", JsonMaker.makeErrorJson(e));
                }
            }
            );
            // get RFID Reader information
            Electron.IpcMain.On("getRfidReaderInformation", (indexOfConnectedDeviceList) =>
            {
                try
                {
                    int i = Convert.ToInt32(indexOfConnectedDeviceList);
                    Electron.IpcMain.Send(getMainWindow(), "replyGetRfidReaderInformation", _rfidScanner.GetRfidReaderInformation(i));
                }
                catch (Exception e)
                {
                    Electron.IpcMain.Send(getMainWindow(), "replyGetRfidReaderInformation", JsonMaker.makeErrorJson(e));
                }
            }
            );

            // get antenna Enable status
            Electron.IpcMain.On("getAntennaEnable", (args) =>
            {
                try
                {
                    if (args == null)
                    {
                        throw new Exception("args is null");
                    }
                    int[] arguments = args as int[];
                    if (arguments == null)
                    {
                        throw new Exception("args is not an int[]");
                    }
                    if (arguments.Length == 2 &&
                        int.TryParse(arguments[0].ToString(), out int indexOfConnectedDeviceList) &&
                        int.TryParse(arguments[1].ToString(), out int antennaIndex))
                    {
                        Electron.IpcMain.Send(getMainWindow(), "replyGetAntennaEnable", _rfidScanner.GetAntennaEnableStatus(indexOfConnectedDeviceList));
                    }
                }
                catch (Exception e)
                {
                    Electron.IpcMain.Send(getMainWindow(), "replyGetAntennaEnable", JsonMaker.makeErrorJson(e));
                }
            });

            // set antenna Enable status
            Electron.IpcMain.On("setAntennaEnable", (args) =>
            {
                try
                {
                    if (args == null)
                    {
                        throw new Exception("args is null");
                    }
                    object[] arguments = args as object[];
                    if (arguments == null)
                    {
                        throw new Exception("args is not an object[]");
                    }
                    if (arguments.Length == 2 &&
                        int.TryParse(arguments[0].ToString(), out int indexOfConnectedDeviceList) &&
                        arguments[1] is bool[] antennaEnable)
                    {
                        Electron.IpcMain.Send(getMainWindow(), "replySetAntennaEnable", _rfidScanner.SetAntennaEnableStatus(indexOfConnectedDeviceList, antennaEnable));
                    }
                }
                catch (Exception e)
                {
                    Electron.IpcMain.Send(getMainWindow(), "replySetAntennaEnable", JsonMaker.makeErrorJson(e));
                }
            });

            // get antenna power
            Electron.IpcMain.On("getAntennaPower", (indexOfConnectedDeviceList) =>
            {
                try
                {
                    int i = Convert.ToInt32(indexOfConnectedDeviceList);
                    Electron.IpcMain.Send(getMainWindow(), "replyGetAntennaPower", _rfidScanner.GetAntennaPower(i));
                }
                catch (Exception e)
                {
                    Electron.IpcMain.Send(getMainWindow(), "replyGetAntennaPower", JsonMaker.makeErrorJson(e));
                }
            }
            );

            // set antenna power
            Electron.IpcMain.On("setAntennaPower", (args) =>
            {
                try
                {
                    if (args == null)
                    {
                        throw new Exception("args is null");
                    }
                    object[] arguments = args as object[];
                    if (arguments == null)
                    {
                        throw new Exception("args is not an object[]");
                    }
                    if (arguments.Length == 2 &&
                        int.TryParse(arguments[0].ToString(), out int indexOfConnectedDeviceList) &&
                        arguments[1] is int[] power)
                    {
                        Electron.IpcMain.Send(getMainWindow(), "replySetAntennaPower", _rfidScanner.SetAntennaPower(indexOfConnectedDeviceList, power));
                    }
                }
                catch (Exception e)
                {
                    Electron.IpcMain.Send(getMainWindow(), "replySetAntennaPower", JsonMaker.makeErrorJson(e));
                }
            });

            // 單次讀取
            Electron.IpcMain.On("singleRead", (indexOfConnectedDeviceList) =>
            {
                try
                {
                    int i = Convert.ToInt32(indexOfConnectedDeviceList);
                    Electron.IpcMain.Send(getMainWindow(), "replySingleRead", _rfidScanner.SingleRead(i));
                }
                catch (Exception e)
                {
                    Electron.IpcMain.Send(getMainWindow(), "replySingleRead", JsonMaker.makeErrorJson(e));
                }
            }
            );


            // 開始循環讀取
            Electron.IpcMain.On("startLoopRead", (indexOfConnectedDeviceList) =>
            {
                try
                {
                    int i = Convert.ToInt32(indexOfConnectedDeviceList);
                    Electron.IpcMain.Send(getMainWindow(), "replyStartLoopRead", _rfidScanner.StartLoopRead(i));
                }
                catch (Exception e)
                {
                    Electron.IpcMain.Send(getMainWindow(), "replyStartLoopRead", JsonMaker.makeErrorJson(e));
                }
            }
            );

            // 停止循環讀取
            Electron.IpcMain.On("stopLoopRead", (indexOfConnectedDeviceList) =>
            {
                try
                {
                    int i = Convert.ToInt32(indexOfConnectedDeviceList);
                    Electron.IpcMain.Send(getMainWindow(), "replyStopLoopRead", _rfidScanner.StopLoopRead(i));
                }
                catch (Exception e)
                {
                    Electron.IpcMain.Send(getMainWindow(), "replyStopLoopRead", JsonMaker.makeErrorJson(e));
                }
            }
            );

            // set password
            Electron.IpcMain.On("setPassword", (indexOfConnectedDeviceList) =>
            {
                try
                {
                    int i = Convert.ToInt32(indexOfConnectedDeviceList);
                    Electron.IpcMain.Send(getMainWindow(), "replySetPassword", _rfidScanner.SetTagPassword(i));
                }
                catch (Exception e)
                {
                    Electron.IpcMain.Send(getMainWindow(), "replySetPassword", JsonMaker.makeErrorJson(e));
                }
            });





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
