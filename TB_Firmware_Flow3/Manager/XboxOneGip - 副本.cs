using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using D4XGaming.Devices;
using Windows.Storage;
using System.Threading;
using Windows.Gaming.Input.Custom;
using UpgradeCongroller;
namespace TB_Firmware_Flow3.Manager
{
    class XboxOneGip
    {
        private readonly object myLock = new object();
        private List<D4XDevice> myDevices = new List<D4XDevice>();
        public byte[] payloadB = new byte[0x64];
       
        //Upgraded Controller
        UpdateController updcon;
        private byte[] writeBuffer;
        private byte[][] dataBuffer = new byte[16][];
        bool UpgradeFlag = false;
        bool UpgradeMode = false;
        bool error = true;
        bool upgradeFinish = false;
        uint counttotal = 0;
        uint coutH = 0;
        UInt16 checkNum = 0;
        uint fileSize;
        //byte resetType;
        bool connectStatic = false;

        public string FileName = "ms-appx:///Assets/UpdateFile/LBX1261_V1.0.2(White)_AP(AES).png";
        private D4XDevice device_xbox = null;
        private D4XDevice[] deviceHandle = new D4XDevice[16];
        private bool updateFinished;
        public delegate  void MyDelegate1(bool bl);
        public static event MyDelegate1 IsConnected;
        public bool ConnceNotice = false;//通知设备是否有连接或者断开你
        enum ContrololerType{ VelocityOneFlight=0,ReconWhite,ReconBlack ,BootLoaderMode }


        public void InitxboxGip()
        {
            D4XDevice.D4XDeviceAdded += OnD4XDeviceAdded;
            D4XDevice.D4XDeviceRemoved += OnD4XDeviceRemoved;
            updcon = new UpdateController();
            for (int i = 0; i < 16; i++)
                deviceHandle[i] = null;
            _ = ReadWhiteFileAsync();
            _ = ReadBlackFileAsync();

        }
       public bool Connected()
        {
            for(int i=0;i<16;i++)
            {
                if (deviceHandle[i] != null)
                    return true;
            }
            return false ;
        }
        private void OnD4XDeviceAdded(object sender, D4XDevice device)
        {
            lock (myLock)
            {
                bool deviceInList = myDevices.Contains(device);
                ushort Pid = device.HardwareProductId;
                ushort Vid = device.HardwareVendorId;

                if (!deviceInList && Vid == 0x10f5 && (Pid == 0x7007 || Pid == 0x7008 || Pid == 0x7005))
                {
                    device.MessageBReceived += OnMessageBReceived;
                   // firwmwareVerionos = device.FirmwareVersionInfo.Build;
                    myDevices.Add(device);
                    device_xbox = device;
                    if(Pid==0x7008)
                    {
                        deviceHandle[(int)(ContrololerType.ReconWhite)] = device;
                        upgradeFinish = false;
                    }
                    if(Pid==0x7005)
                    {
                        deviceHandle[(int)(ContrololerType.ReconBlack)] = device;
                        upgradeFinish = false;
                    }
               //     connectStatic = true;
                    IsConnected(true);
                    if (Pid == 0x7007)
                    {
                        deviceHandle[(int)(ContrololerType.BootLoaderMode)] = device;
                        UpgradeMode = true;
                        if (UpgradeFlag)
                        {
                            updcon.IniDeviceXbox(device_xbox);
                            Thread.Sleep(1000);
                            updcon.EnterUpgradeMode(updcon.ConFirmUpgrade);
                        }
                    }
                    else
                        UpgradeMode = false;

                }
            }
        }

        private void OnD4XDeviceRemoved(object sender, D4XDevice device)
        {
            lock (myLock)
            {
                bool deviceInList = myDevices.Contains(device);
                ushort Pid = device.HardwareProductId;
                ushort Vid = device.HardwareVendorId;

                if (deviceInList && Vid == 0x10f5 && (Pid == 0x7007 || Pid == 0x7008 || Pid == 0x7005))
                {
                    // Debug.WriteLine("[" + DateTime.Now.ToString("HH:mm:ss.fff") + "] D4XDevice(" + device.InstanceId + ") Removed");
                    // Debug.WriteLine("[" + DateTime.Now.ToString("HH:mm:ss.fff") + "] D4XDevice.D4XDevices.Count = " + D4XDevice.D4XDevices.Count.ToString());
                    device.MessageBReceived -= OnMessageBReceived;
                    myDevices.Remove(device);
                   // connectStatic = false;
                    device = null;
                    IsConnected(true);
                    if ((Pid == 0x7007) && UpgradeFlag)//中途拔出手柄，升级停止，error为false;
                    {
                        error = true;
                        UpgradeFlag = false;
                        deviceHandle[(int)(ContrololerType.BootLoaderMode)] = device;
                        Debug.WriteLine("Upgrade Error0!");
                    }
                    if (updateFinished)
                    {
                        updateFinished = false;
                    }
                    if (Pid == 0x7008)
                    {
                        deviceHandle[(int)(ContrololerType.ReconWhite)] = device;
                    }
                    if (Pid == 0x7005)
                    {
                        deviceHandle[(int)(ContrololerType.ReconBlack)] = device;
                    }
                    IsConnected(true);
                }
            }
        }
        public void SetError(bool bl)
        {
            error = bl;
        }

        private void OnMessageBReceived(D4XDevice device, byte[] payload)
        {
            ReadUpdateData(payload);
            
        }


        public D4XDevice[] GetD4XDevices()
        {
            return deviceHandle;
        }


        /****************************Upgrade Controller *****************************************/

        public bool StateUpgrade(int type=0)
        {
            if (device_xbox == null)
                return false;
            if (deviceHandle[(int)(ContrololerType.BootLoaderMode)]!=null)
                updcon.IniDeviceXbox(deviceHandle[(int)(ContrololerType.BootLoaderMode)]);
            else 
                updcon.IniDeviceXbox(deviceHandle[type]);
            if(type==(int)ContrololerType.ReconWhite)
            {
                writeBuffer =  dataBuffer[(int)ContrololerType.ReconWhite];
            } 
            else if (type == (int)ContrololerType.ReconBlack)
            {
                writeBuffer = dataBuffer[(int)ContrololerType.ReconBlack];
            } 
            else if (type == (int)ContrololerType.VelocityOneFlight)
            {
                writeBuffer = dataBuffer[(int)ContrololerType.VelocityOneFlight];
            }
                UpgradeFlag = true;
            error = false;
            upgradeFinish = false;
            // quitUpgrade = false;
            counttotal = 0;
            coutH = 0;
            checkNum = 0;
            fileSize = (uint)writeBuffer.Length;
            Debug.WriteLine("MainPage UpgradeMode:" + UpgradeMode);
            if (UpgradeMode)
                updcon.EnterUpgradeMode(updcon.ConFirmUpgrade);
            else
                updcon.EnterUpgradeMode(updcon.EnterUpgade);
            return true;
        }

        private void writeData()//发送文件数据
        {


            byte[] payload = new byte[55];
            Debug.WriteLine("*********size:" + counttotal.ToString() + " " + fileSize.ToString());
            int num = 54;
            for (int i = 0; i < 55; i++)
            {

                if (counttotal >= fileSize) { num = i; break; }
                payload[i] = writeBuffer[counttotal];
                checkNum += writeBuffer[counttotal];
                counttotal++;

            }
            Debug.WriteLine("last num:" + num.ToString() + "  " + counttotal.ToString() + "  " + fileSize.ToString());
            if (num == 54)
                updcon.WritUpgradeFile(payload, (byte)coutH);
            else
            {
                byte[] data = new byte[num];
                for (int i = 0; i < num; i++)
                    data[i] = payload[i];
                updcon.WritUpgradeFile1(data, (byte)coutH, num);

                Debug.WriteLine("*********last num*********:" + num.ToString() + payload[num - 3].ToString() + " " + payload[2].ToString() + " " + payload[1].ToString());
            }

            coutH++;
            
        }

        public async System.Threading.Tasks.Task ReadWhiteFileAsync()//读取文件
        {
#if false
            Windows.Storage.StorageFolder storageFolder =
            Windows.Storage.ApplicationData.Current.LocalFolder;
            Debug.Write("123\n" + storageFolder.Path + "\n321");
            Windows.Storage.StorageFile sampleFile = await storageFolder.GetFileAsync(FileName);


#else
            FileName = "ms-appx:///Assets/UpdateFile/LBX1261_V1.0.3(White)_AP(AES).png";
            StorageFile sampleFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri(FileName));

#endif
            //  return;
            var buffer = await Windows.Storage.FileIO.ReadBufferAsync(sampleFile);
            fileSize = buffer.Length;
            dataBuffer[(int)ContrololerType.ReconWhite] = new byte[fileSize];
            for (uint i = 0; i < buffer.Length; i++)
            {
                dataBuffer[(int)ContrololerType.ReconWhite][i] = buffer.GetByte(i);
            }
            byte by = (byte)buffer.GetByte(1);
            Debug.Write("******" + buffer.GetByte(0).ToString() + buffer.GetByte(1).ToString() + "*U**" + buffer.Length.ToString() + "\n");
            
        }

        public async System.Threading.Tasks.Task ReadBlackFileAsync()//读取文件
        {
#if false
            Windows.Storage.StorageFolder storageFolder =
            Windows.Storage.ApplicationData.Current.LocalFolder;
            Debug.Write("123\n" + storageFolder.Path + "\n321");
            Windows.Storage.StorageFile sampleFile = await storageFolder.GetFileAsync(FileName);


#else
            FileName = "ms-appx:///Assets/UpdateFile/LBX1261_V1.0.3(Black)_AP(AES).png";
            StorageFile sampleFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri(FileName));

#endif
            //  return;
            var buffer = await Windows.Storage.FileIO.ReadBufferAsync(sampleFile);
            fileSize = buffer.Length;
            dataBuffer[(int)ContrololerType.ReconBlack] = new byte[fileSize];
            for (uint i = 0; i < buffer.Length; i++)
            {
                dataBuffer[(int)ContrololerType.ReconBlack][i] = buffer.GetByte(i);
            }
            byte by = (byte)buffer.GetByte(1);
            Debug.Write("******" + buffer.GetByte(0).ToString() + buffer.GetByte(1).ToString() + "*U**" + buffer.Length.ToString() + "\n");
            //  Debug.Write("\n" + sampleFile.Path + "\n" + buffer.Length.ToString());
        }

        //开始升级，根据返回信息，进行相应升级操作
        private void ReadUpdateData(byte[] payload)
        {
            if (UpgradeFlag)
            {
                Debug.WriteLine("Lentght:" + payload.Length.ToString());
                if (payload.Length == 5)
                {

                    if (UpgradeMode)
                    {
                        Debug.WriteLine("Confirm Enter Upgrade Mode!");
                        if (payload[1] == 0x13 && payload[2] == 4)
                            writeData();
                        else
                        {
                            error = true;
                            UpgradeFlag = false;
                            Debug.WriteLine("Upgrade Error!1");
                        }
                    }
                    else if (payload[1] == 0x13 && payload[2] == 2)
                    {
                        Debug.WriteLine("Enter Upgrade Mode!");
                    }
                    else
                    {
                        Debug.WriteLine("Upgrade Error!4");
                        error = true;
                        UpgradeFlag = false;
                    }


                }
                if (payload.Length == 7)
                {
                    uint countSize = (uint)(payload[3] << 8) + payload[4];
                    Debug.WriteLine("write file byte lenth:" + countSize.ToString());
                    Debug.WriteLine("writeActual number of byte length :" + counttotal.ToString());
                    Debug.WriteLine(payload[3].ToString() + " " + payload[4].ToString());
                    if ((countSize == (counttotal % 65536))&& counttotal != fileSize)
                    {
                        writeData();
                    }
                    else if (counttotal == fileSize)
                    {
                        updcon.RebootDevice(checkNum);
                        counttotal++;


                    }
                    else if ((counttotal - 1) == fileSize)
                    {
                        ///upgradeFinish = true;
                        //UpgradeFlag = false;
                       // Debug.WriteLine("Length:" + payloadB.Length.ToString());
                        if (payload[3] == (byte)(checkNum >> 8) && payload[4] == (byte)(checkNum))
                        {
                            upgradeFinish = true;
                            UpgradeFlag = false;
                            Debug.WriteLine("Upgrade Finished yes!");
                        }
                        else
                        {
                            Debug.WriteLine("Upgrade Error3,quit failed" + counttotal.ToString() + "," + fileSize.ToString());
                            error = true;
                            UpgradeFlag = false;
                        }

                    }
                    else
                    {
                        Debug.WriteLine("Upgrade Error3!"+ counttotal.ToString()+","+fileSize.ToString());
                        error = true;
                        UpgradeFlag = false;
                    }

                }
            }
        }
        //返回更新进度，大小为1k
        public int GetProgressValue()
        {
            return (int)(100 * counttotal / fileSize);
        }
        public bool GetFinishedState()
        {
            return upgradeFinish;
        }

        public bool IsUpdateMode()
        {
            return UpgradeMode;
        }
        //判断控制器设备是否连接（除1183手柄为true，其他均为false）
        public bool GetControllerState()
        {
            if (device_xbox != null)
                return true;
            else
                return false;
        }
        //true是升级标志,false为退出升级.
        public bool GetUpgradeState()
        {
            return UpgradeFlag;
        }

       
    }
}
