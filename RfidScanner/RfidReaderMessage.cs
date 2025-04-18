﻿using ElectronNET.API;
using RFIDReaderAPI.Interface;
using RFIDReaderAPI.Models;
using System.Text.Json;

namespace Campus_Asset_Management_System.RfidScanner
{
    public class RfidReaderMessage : IAsynchronousMessage
    {

        public Func<BrowserWindow> funcGetMainWindow { get; set; }
        private UsbRfidScanner _rfidScanner;
        public RfidReaderMessage(UsbRfidScanner rfidScanner)
        {
            _rfidScanner = rfidScanner;
        }


        #region implement IAsynchronousMessage function

        /// <summary>
        /// 當 GPI 觸發參數開啟且有 GPI 觸發事件時，該函數將回調當前事件所在的 GPI 端口號以及級別狀態信息。
        /// </summary>
        /// <param name="gpi_model">表示 GPI 事件的數據模型對象。</param>
        public void GPIControlMsg(GPI_Model gpi_model)
        {
            Console.WriteLine("GPIControlMs: We may not use GPIControlMsg, if need to use please call me");
        }

        /// <summary>
        /// 輸出標籤信息的回調，無論是單次讀取、盤點（循環）讀取，還是從緩存中獲取標籤數據，都是相同的回調介面。
        /// </summary>
        /// <param name="tag">表示已讀取的標籤數據的模型對象。</param>
        public void OutPutTags(Tag_Model tag)
        {
            try
            {
                if (tag == null || tag.Result != 0x00) { 
                    throw new Exception("Tag is null");
                };
                Electron.IpcMain.Send(funcGetMainWindow(), "newScannedTag", JsonMaker.makeTagJson(tag));
            }
            catch (Exception e)
            {
                Electron.IpcMain.Send(funcGetMainWindow(), "newScannedTag", JsonMaker.makeErrorJson(e));
            }
        }

        /// <summary>
        /// 在最後一個標籤上傳後，上傳同步結束信號，指示當前讀取標籤操作結束。
        /// </summary>
        public void OutPutTagsOver()
        {
            Electron.IpcMain.Send(funcGetMainWindow(), "scanningOver", null);
        }

        /// <summary>
        /// 當設備斷開連接時，API 回調連接 ID，表示當前連接 ID 的設備已斷開。
        /// </summary>
        /// <param name="connID">表示當前連接的 ID。</param>
        public void PortClosing(string connID)
        {
            Electron.IpcMain.Send(funcGetMainWindow(), "deviceDisconnected", connID);
            _rfidScanner.DisconnectUsbRfidReader(connID);
        }

        /// <summary>
        /// 在 TCP 伺服器模式下處理客戶端的連接回調，當獲得連接 ID 時，可通過該 ID 讀寫設備。
        /// </summary>
        /// <param name="connID">表示當前客戶端的連接 ID。</param>
        public void PortConnecting(string connID)
        {
            Console.WriteLine(connID + " is connecting");
        }

        /// <summary>
        /// 輸出調試消息。
        /// </summary>
        /// <param name="msg">表示要輸出的調試消息字符串。</param>
        public void WriteDebugMsg(string msg)
        {
            Console.WriteLine("WriteDebugMsg: "+msg);
        }

        /// <summary>
        /// 輸出日誌消息。
        /// </summary>
        /// <param name="msg">表示要記錄的日誌消息字符串。</param>
        public void WriteLog(string msg)
        {
            Console.WriteLine("WriteLog: " + msg);
        }

        #endregion
    }
}
