using RFIDReaderAPI.Models;
using System.Text.Json;

namespace Campus_Asset_Management_System.RfidScanner
{
    public static class JsonMaker
    {
        public static string makeErrorJson(Exception e)
        {
            string error = e.Message;
            Console.WriteLine("Error: " + error);
            Console.WriteLine("==========================");
            Console.WriteLine(e.StackTrace);
            return JsonSerializer.Serialize(new { error });
        }
        public static string makeTagJson(Tag_Model tag)
        {
            return JsonSerializer.Serialize(tag);
        }

        public static string makeUsbDeviceListJson(List<string> devices)
        {
            return JsonSerializer.Serialize(new
            {
                devices
            });
        }

        public static string makeSuccessJson(bool success)
        {
            return JsonSerializer.Serialize(new
            {
                success
            });
        }

        public static string makeDisconnectReaderJson(int indexOfConnectedDevice, bool success)
        {
            return JsonSerializer.Serialize(new
            {
                indexOfConnectedDevice,
                success
            });
        }

        public static string makeIsConnectedJson(bool isConnected)
        {
            return JsonSerializer.Serialize(new
            {
                isConnected
            });
        }

        public static string makeRfidReaderInformationJson(RfidReaderInformaion information)
        {
            return JsonSerializer.Serialize(information);
        }

        public static string makeGetAntennaEnableJson(string antenna)
        {
            int[] antennaEnable = antenna.Split(",").Select(x => Convert.ToInt32(x)).ToArray();
            return JsonSerializer.Serialize(new
            {
                antennaEnable
            });
        }
        public static string makeGetAntennaPowerJson(int[] power)
        {
            return JsonSerializer.Serialize(new
            {
                power
            });
        }

        public static string makeIsLoopingJson(bool isLooping)
        {
            return JsonSerializer.Serialize(new
            {
                isLooping
            });
        }

        public static string makeIndexOfConnectedDeviceJson(int indexOfConnectedDevice)
        {
            return JsonSerializer.Serialize(new
            {
                indexOfConnectedDevice
            });
        }

    }
}
