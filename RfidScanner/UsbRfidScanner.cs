
using RFIDReaderAPI;
using RFIDReaderAPI.Interface;
using RFIDReaderAPI.Models;
using System;
using System.Reflection.Metadata;

namespace Campus_Asset_Management_System.RfidScanner
{
    public class RfidReaderInformaion
    {
        public RfidReaderInformaion(int minPower, int maxPower, int numOfAntenna, int[] bandList, int[] listOfRfidProtocol)
        {
            this.minPower = minPower;
            this.maxPower = maxPower;
            this.numOfAntenna = numOfAntenna;
            this.bandList = bandList;
            this.listOfRfidProtocol = listOfRfidProtocol;
        }
        public int minPower { get; set; }
        public int maxPower { get; set; }
        public int numOfAntenna { get; set; }
        public int[] bandList { get; set; }
        public int[] listOfRfidProtocol { get; set; }
    }
    public class UsbRfidScanner
    {
        private List<String>? usbDeviceList = null;
        private RfidReaderMessage _rfidReaderMessage;
        public RfidReaderMessage rfidReaderMessage
        {
            get { return _rfidReaderMessage; }
        }
        private List<String> ConnectedDeviceList = new List<String>();
        private Dictionary<String, RfidReaderInformaion> DevicesInformation = new Dictionary<String, RfidReaderInformaion>();
        private String password = "12345678";
        private const String defaultPassword = "00000000";
        public List<String> isLooping { get; private set; } = new List<string>();
        public UsbRfidScanner()
        {
            _rfidReaderMessage = new RfidReaderMessage(this);
        }

        // 取得 USB 裝置列表
        public String GetUsbDeviceList()
        {
            usbDeviceList = RFIDReader.GetUsbHidDeviceList();
            return JsonMaker.makeUsbDeviceListJson(usbDeviceList);
        }

        // 連接 RFID Reader
        public String ConnectUsbRfidReader(int indexOfUsbDeviceList)
        {
            try
            {   
                if(usbDeviceList == null)
                {
                    usbDeviceList = RFIDReader.GetUsbHidDeviceList();
                }

                if (indexOfUsbDeviceList < 0 || indexOfUsbDeviceList >= usbDeviceList.Count)
                {
                    throw new Exception("indexOfUsbDeviceList is out of range");
                }
                String ConnID = usbDeviceList[indexOfUsbDeviceList];
                return ConnectUsbRfidReader(ConnID);
            }
            catch (Exception e)
            {
                return JsonMaker.makeErrorJson(e);
            }
        }
        public String ConnectUsbRfidReader(String ConnID)
        {
            try
            {
                if (usbDeviceList == null)
                {
                    throw new Exception("can not find any usb device");
                }
                if (!RFIDReader.GetUsbHidDeviceList().Contains(ConnID))
                {
                    throw new Exception("make sure the device is connected");
                }
                if (ConnectedDeviceList.Contains(ConnID))
                {
                    throw new Exception("the device is already connected");
                }
                //  new nint()只是為了符合方法的參數，不會被使用
                bool success = RFIDReader.CreateUsbConn(ConnID, new nint(), _rfidReaderMessage);
                if (success)
                {
                    ConnectedDeviceList.Add(ConnID);
                }

                return JsonMaker.makeSuccessJson(success);
            }
            catch (Exception e)
            {
                return JsonMaker.makeErrorJson(e);
            }
        }
        // 斷開單個 RFID Reader
        public String DisconnectUsbRfidReader(int indexOfConnectedDeviceList)
        {
            try
            {
                if (ConnectedDeviceList.Count == 0)
            {
                throw new Exception("ConnectedDeviceList is empty");
            }
            if ((indexOfConnectedDeviceList < 0) || (indexOfConnectedDeviceList >= ConnectedDeviceList.Count))
            {
                throw new Exception("indexOfConnectedDeviceList is out of range");
            }
            String ConnID = ConnectedDeviceList[indexOfConnectedDeviceList];
            RFIDReader.CloseConn(ConnID);
            ConnectedDeviceList.RemoveAt(indexOfConnectedDeviceList);
            return JsonMaker.makeDisconnectReaderJson(indexOfConnectedDeviceList, true);
            }
            catch (Exception e)
            {
                return JsonMaker.makeErrorJson(e);
            }
        }
        public String DisconnectUsbRfidReader(String ConnID)
        {
            try
            {
                int indexOfConnectedDeviceList = ConnectedDeviceList.IndexOf(ConnID);
                if (indexOfConnectedDeviceList == -1)
                {
                    throw new Exception("the device is not connected");
                }
                return DisconnectUsbRfidReader(indexOfConnectedDeviceList);
        }
            catch (Exception e)
            {
                return JsonMaker.makeErrorJson(e);
            }
}

        // 斷開所有 RFID Reader
        public String DisconnectAllUsbRfidReader()
        {
            foreach (String device in ConnectedDeviceList)
            {
                RFIDReader.CloseConn(device);
            }
            ConnectedDeviceList.Clear();
            return JsonMaker.makeDisconnectReaderJson(-1, true);
        }

        // get conected device list
        public String GetConnectedDeviceList()
        {
            return JsonMaker.makeUsbDeviceListJson(ConnectedDeviceList);
        }

        // check connection status
        public String CheckConnectionStatus(int indexOfConnectedDeviceList)
        {
            return CheckConnectionStatus(ConnectedDeviceList[indexOfConnectedDeviceList]);
        }
        public String CheckConnectionStatus(String ConnID)
        {
            return JsonMaker.makeIsConnectedJson(RFIDReader.CheckConnect(ConnID));
        }

        // get RFID Reader information
        public String GetRfidReaderInformation(int indexOfConnectedDeviceList)
        {
            String ConnID = ConnectedDeviceList[indexOfConnectedDeviceList];
            return GetRfidReaderInformation(ConnID);
        }
        public String GetRfidReaderInformation(String ConnID)
        {
            string[] info = RFIDReader._RFIDConfig.GetReaderProperty(ConnID).Split('|');
            int minPower = Convert.ToInt32(info[0]);
            int maxPower = Convert.ToInt32(info[1]);
            int numOfAntenna = Convert.ToInt32(info[2]);
            int[] bandList = info[3].Split(",").Select(x => Convert.ToInt32(x)).ToArray();
            int[] listOfRfidProtocol = info[4].Split(",").Select(x => Convert.ToInt32(x)).ToArray();
            RfidReaderInformaion ObjInfo = new RfidReaderInformaion(
                minPower,
                maxPower,
                numOfAntenna,
                bandList,
                listOfRfidProtocol
            );
            DevicesInformation[ConnID] = ObjInfo;

            return JsonMaker.makeRfidReaderInformationJson(ObjInfo);
        }

        public RfidReaderInformaion CheckDeviceInformation(int indexOfConnectedDeviceList)
        {
            return CheckDeviceInformation(ConnectedDeviceList[indexOfConnectedDeviceList]);
        }
        private RfidReaderInformaion CheckDeviceInformation(String ConnID)
        {
            if (!DevicesInformation.ContainsKey(ConnID))
            {
                GetRfidReaderInformation(ConnID);
            }
            return DevicesInformation[ConnID];
        }

        // get antenna Enable status
        public String GetAntennaEnableStatus(int indexOfConnectedDeviceList)
        {
            return GetAntennaEnableStatus(ConnectedDeviceList[indexOfConnectedDeviceList]);
        }
        public String GetAntennaEnableStatus(String ConnID)
        {
            return JsonMaker.makeGetAntennaEnableJson(RFIDReader._RFIDConfig.GetReaderANT2(ConnID));
        }

        // set antenna Enable status
        public String SetAntennaEnableStatus(int indexOfConnectedDeviceList, bool[] antennaEnable)
        {
            try
            {
                String ConnID = ConnectedDeviceList[indexOfConnectedDeviceList];
                return SetAntennaEnableStatus(ConnID, antennaEnable);
            }
            catch (Exception e)
            {
                return JsonMaker.makeErrorJson(e);
            }
        }

        public String SetAntennaEnableStatus(String ConnID, bool[] antennaEnable)
        {
            try
            {
                RfidReaderInformaion deviceinfo = CheckDeviceInformation(ConnID);
                int eAntenna = 0;
                if (antennaEnable.Length != deviceinfo.numOfAntenna)
                {
                    throw new Exception("antennaEnable.Length is not equal to numOfAntenna");
                }
                for (int i = 0; i < antennaEnable.Length; i++)
                {
                    if (antennaEnable[i])
                    {
                        eAntenna += (int)Math.Pow(2, i);
                    }
                }

                int result = RFIDReader._RFIDConfig.SetReaderANT(ConnID, (eAntennaNo)eAntenna);
                if (result != 0)
                {
                    throw new Exception("SetReaderANT failed: " + result);
                }
                return JsonMaker.makeSuccessJson(true);
            }
            catch (Exception e)
            {
                return JsonMaker.makeErrorJson(e);
            }
        }

        // get antenna power
        public String GetAntennaPower(int indexOfConnectedDeviceList)
        {
            String ConnID = ConnectedDeviceList[indexOfConnectedDeviceList];
            return GetAntennaPower(ConnID);
        }
        public String GetAntennaPower(String ConnID)
        {
            int[] power = RFIDReader._RFIDConfig.GetANTPowerParam(ConnID).Values.ToArray();
            return JsonMaker.makeGetAntennaPowerJson(power);
        }

        // set antenna power
        public String SetAntennaPower(int indexOfConnectedDeviceList, int[] power)
        {
            try
            {
                String ConnID = ConnectedDeviceList[indexOfConnectedDeviceList];
                return SetAntennaPower(ConnID, power);
            }
            catch (Exception e)
            {
                return JsonMaker.makeErrorJson(e);
            }
        }
        public String SetAntennaPower(String ConnID, int[] power)
        {
            try
            {
                RfidReaderInformaion deviceinfo = CheckDeviceInformation(ConnID);
                if (power.Length != deviceinfo.numOfAntenna)
                {
                    throw new Exception("power.Length is not equal to numOfAntenna");
                }
                Dictionary<int, int> powerDic = new Dictionary<int, int>();
                for (int i = 0; i < power.Length; i++)
                {
                    if (power[i] < deviceinfo.minPower || power[i] > deviceinfo.maxPower)
                    {
                        throw new Exception($"the power of antenna{i + 1} is out of range, minPower is {deviceinfo.minPower}, maxPower is {deviceinfo.maxPower}");
                    }
                    powerDic[i+1] = power[i];
                }
                int result = RFIDReader._RFIDConfig.SetANTPowerParam(ConnID, powerDic);

                if (result != 0)
                {
                    throw new Exception("SetReaderPower failed: " + result);
                }
                return JsonMaker.makeSuccessJson(true);
            }
            catch (Exception e)
            {
                return JsonMaker.makeErrorJson(e);
            }
        }

        // 單次讀取
        public String SingleRead(int indexOfConnectedDeviceList)
        {
            try
            {
                string ConnID = ConnectedDeviceList[indexOfConnectedDeviceList];
                return SingleRead(ConnID);
            }
            catch (Exception e)
            {
                return JsonMaker.makeErrorJson(e);
            }
        }
        public String SingleRead(String ConnID)
        {
            try
            {
                int result = RFIDReader._Tag6C.GetEPC_TID_UserData(ConnID,
                    RFIDReaderAPI.RFIDReader._RFIDConfig.GetReaderANT(ConnID),
                    eReadType.Single,
                    0,
                    2
                    );
                if (result != 0)
                {
                    throw new Exception("GetEPC_TID_UserData failed: " + result);
                }
                return JsonMaker.makeSuccessJson(true);
            }
            catch (Exception e)
            {
                return JsonMaker.makeErrorJson(e);
            }
        }

        // 開始循環讀取
        public String StartLoopRead(int indexOfConnectedDeviceList)
        {
            try
            {
                string ConnID = ConnectedDeviceList[indexOfConnectedDeviceList];
                return StartLoopRead(ConnID);
            }
            catch (Exception e)
            {
                return JsonMaker.makeErrorJson(e);
            }
        }
        public String StartLoopRead(String ConnID)
        {
            try
            {
                if (isLooping.Contains(ConnID))
                {
                    throw new Exception("the device is already looping");
                }
                int result = RFIDReader._Tag6C.GetEPC_TID_UserData(ConnID,
                    RFIDReaderAPI.RFIDReader._RFIDConfig.GetReaderANT(ConnID),
                    eReadType.Inventory,
                    0,
                    2
                    );
                if (result != 0)
                {
                    throw new Exception("GetEPC_TID_UserData failed: " + result);
                }
                isLooping.Add(ConnID);
                return JsonMaker.makeSuccessJson(true);
            }
            catch (Exception e)
            {
                return JsonMaker.makeErrorJson(e);
            }
        }

        // 停止循環讀取
        public String StopLoopRead(int indexOfConnectedDeviceList)
        {
            try
            {
                string ConnID = ConnectedDeviceList[indexOfConnectedDeviceList];
                return StopLoopRead(ConnID);
            }
            catch (Exception e)
            {
                return JsonMaker.makeErrorJson(e);
            }
        }
        public String StopLoopRead(String ConnID)
        {
            try
            {
                if (!isLooping.Contains(ConnID))
                {
                    throw new Exception("the device is not looping");
                }
                RFIDReader._Tag6C.Stop(ConnID);
                isLooping.Remove(ConnID);
                return JsonMaker.makeSuccessJson(true);
            }
            catch (Exception e)
            {
                return JsonMaker.makeErrorJson(e);
            }
        }

        // 寫入data to 標籤
        // i do not want to write

        // lock 標籤
        //private String LockTag(int indexOfConnectedDeviceList)
        //{
        //    try
        //    {
        //        string ConnID = ConnectedDeviceList[indexOfConnectedDeviceList];
        //        return LockTag(ConnID);
        //    }
        //    catch (Exception e)
        //    {
        //        return JsonMaker.makeErrorJson(e);
        //    }
        //}

        //private String LockTag(String ConnID)
        //{
        //    try
        //    {
        //        int result = RFIDReader._Tag6C.Lock(ConnID, RFIDReaderAPI.RFIDReader._RFIDConfig.GetReaderANT(ConnID),eLockArea.epc|eLockArea.AccessPassword|eLockArea.tid|eLockArea.userdata|eLockArea.DestroyPassword,eLockType.Lock);
        //        if (result != 0)
        //        {
        //            throw new Exception("LockTag failed: " + result);
        //        }
        //        return JsonMaker.makeSuccessJson(true);
        //    }
        //    catch (Exception e)
        //    {
        //        return JsonMaker.makeErrorJson(e);
        //    }
        //}

        // set password
        public String SetTagPassword(int indexOfConnectedDeviceList)
        {
            try
            {
                String ConnID = ConnectedDeviceList[indexOfConnectedDeviceList];
                return SetTagPassword(ConnID);
            }
            catch (Exception e)
            {
                return JsonMaker.makeErrorJson(e);
            }
        }

        public String SetTagPassword(String ConnID)
        {
            try
            {
                string result = RFIDReader._Tag6C.WriteDestroyPassWord(ConnID, RFIDReaderAPI.RFIDReader._RFIDConfig.GetReaderANT(ConnID), password, accessPassword: password);
                if (result == "8")
                {
                    string result1 = RFIDReader._Tag6C.WriteDestroyPassWord(ConnID, RFIDReaderAPI.RFIDReader._RFIDConfig.GetReaderANT(ConnID), password, accessPassword: defaultPassword);
                    if (result1 != "0")
                    {
                        throw new Exception("WriteDestroyPassWord failed: " + result);
                    }
                    string result2 = RFIDReader._Tag6C.WriteAccessPassWord(ConnID, RFIDReaderAPI.RFIDReader._RFIDConfig.GetReaderANT(ConnID), password, accessPassword: defaultPassword);
                    if (result2 != "0")
                    {
                        throw new Exception("WriteAccessPassWord failed: " + result);
                    }
                }
                else if (result != "0")
                {
                    throw new Exception("WriteDestroyPassWord failed: " + result);
                }
                return JsonMaker.makeSuccessJson(true);
            }
            catch (Exception e)
            {
                return JsonMaker.makeErrorJson(e);
            }
        }


        // check loop status
        public String CheckLoopStatus(int indexOfConnectedDeviceList)
        {
            return CheckLoopStatus(ConnectedDeviceList[indexOfConnectedDeviceList]);
        }
        public String CheckLoopStatus(String ConnID)
        {
            return JsonMaker.makeIsLoopingJson(isLooping.Contains(ConnID));
        }

        // get index of connected device
        public String GetIndexOfConnectedDevice(String ConnID)
        {
            try {
                if (ConnectedDeviceList.Contains(ConnID))
                {
                    return JsonMaker.makeIndexOfConnectedDeviceJson(ConnectedDeviceList.IndexOf(ConnID));
                }
                else
                {
                    throw new Exception("the device is not connected");
                }
            }catch (Exception e) {
                return JsonMaker.makeErrorJson(e);
            }

        }


    }
}