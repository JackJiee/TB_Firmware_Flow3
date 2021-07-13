using System;
using System.Diagnostics;
using System.Threading;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;
using Windows.Web.Http;

namespace TB_Firmware_Flow3.Manager
{
     class httpClientLoadFile
    {
        private CancellationTokenSource cts;
        //private HttpClient httpClient;
        private ushort clearVersiosn = 0;
        private ushort blackVersiosn = 0;
        private ushort whiteVersiosn = 0;
        private ushort []controllerVersiosn = new ushort [16]; 
        private bool[] loadVersiosnFinished = new bool[16];
        private bool[] versionFlg = new bool[16];
        public byte[] blackByteArray;
        public byte[] whiteByteArray;
        public byte [][]loadByteArray=new byte[16][];
        public bool blackFileLoadFinished;
        bool whiteFlleLoadFinished;
        bool[] fileLoadFinished = new bool[16];
        //bool[] loadVersionFinished = new bool[16];
        string clearMD5;
        string blackMD5;
        string whiteMD5;
        const string generalWhiteURLFirmware = "https://t.veterinerburada.com/LBX1261_White.bin";
        const string generalBlackURLFirmware = "https://t.veterinerburada.com/LBX1261_Black.bin";
        const string generalUrbanCamoURLFirmware = "https://ftp.nacon.fr/firmwareurbancamocompact";
        const string generalForestCamoURLFirmware = "https://ftp.nacon.fr/firmwareforestcamocompact";
        const string generalBluefrontURLFirmware = "https://ftp.nacon.fr/firmwarebluecompact";
        const string generalRedfrontURLFirmware  = "https://ftp.nacon.fr/firmwareredcompact";

        const string generalWhiteURLMD5 = "https://t.veterinerburada.com/LBX1261_WhiteMD5.bin";
        const string generalBlackURLMD5 = "https://t.veterinerburada.com/LBX1261_BlackMD5.bin";
        const string generalUrbanCamoURLMD5 = "https://ftp.nacon.fr/crcmd5urbancamocompact";
        const string generalForestCamoURLMD5 = "https://ftp.nacon.fr/crcmd5forestcamocompact";
        const string generalBluefrontURLMD5 = "https://ftp.nacon.fr/crcmd5bluecompact";
        const string generalRedfrontURLMD5 = "https://ftp.nacon.fr/crcmd5redcompact";

        const string generalWhiteURLVersion = "https://t.veterinerburada.com/LBX1261_White_Version.bin";
        const string generalBlackURLVersion = "https://t.veterinerburada.com/LBX1261_Black_Version.bin";
        const string generalUrbanCamoURLVersion = "https://ftp.nacon.fr/versionurbancamocompact";
        const string generalForestCamoURLVersion = "https://ftp.nacon.fr/versionforestcamocompact";
        const string generalBluefrontURLVersion = "https://ftp.nacon.fr/versionbluecompact";
        const string generalRedfrontURLVersion = "https://ftp.nacon.fr/versionredcompact";

        const string clearWhiteURLFirmware = "https://ftp.nacon.fr/firmwareclearwhitecompact";
        const string clearGreenURLFirmware = "https://ftp.nacon.fr/firmwarecleargreencompact";
        const string clearBlueURLFirmware = "https://ftp.nacon.fr/firmwareclearbluecompact";
        const string clearRedURLFirmware = "https://ftp.nacon.fr/firmwareclearredcompact";

        const string clearWhiteURLMD5 = "https://ftp.nacon.fr/crcmd5clearwhitecompact";
        const string clearGreenURLMD5 = "https://ftp.nacon.fr/crcmd5cleargreencompact";
        const string clearBlueURLMD5 = "https://ftp.nacon.fr/crcmd5clearbluecompact";
        const string clearRedURLMD5 = "https://ftp.nacon.fr/crcmd5clearredcompact";

        const string clearWhiteURLVersion = "https://ftp.nacon.fr/versionclearwhitecompact";
        const string clearGreenURLVersion = "https://ftp.nacon.fr/versioncleargreencompact";
        const string clearBlueURLVersion = "https://ftp.nacon.fr/versionclearbluecompact";
        const string clearRedURLVersion = "https://ftp.nacon.fr/versionclearredcompact";

       enum ControllerType { VelocityOneFlight = 0, ReconWhite, ReconBlack, BootLoaderMode };

        public httpClientLoadFile()
        {
            //Debug.Write("\n************************http*****************************************\n");
            //CancellationTokenSource cts;
            //cts = new CancellationTokenSource();
            //  Load_black_UpgradeFile();
            for (int i = 0; i < 16; i++)
            {
                fileLoadFinished[i] = false;
                loadVersiosnFinished[i] = false;
                versionFlg[i] = false;
            }


        }



        private async void Load_UpgradeFile(string controllerURLType, string clearMD5,int cleartypeNum)
        {

            CancellationTokenSource cts;
            cts = new CancellationTokenSource();
            //Debug.WriteLine(sampleFile + Windows.Storage.ApplicationData.Current.LocalFolder.Path);

            var uri = new System.Uri(controllerURLType);

            using (var httpClient = new Windows.Web.Http.HttpClient())
            {
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, uri);
                try
                {
                    IBuffer result = await httpClient.GetBufferAsync(uri);
                    var alg = HashAlgorithmProvider.OpenAlgorithm(HashAlgorithmNames.Md5);
                    var hashed = alg.HashData(result);
                    var md5Value = CryptographicBuffer.EncodeToHexString(hashed);

                    if (clearMD5 == md5Value)
                    {
                        CryptographicBuffer.CopyToByteArray(result, out loadByteArray[cleartypeNum]);
                        Debug.WriteLine(result + "\nlength:" + result.Length + " loadByteArray.Length:  " + loadByteArray.Length);
                        fileLoadFinished[cleartypeNum] = true;
                    }
                    else
                        Debug.WriteLine("\nMD5 ERROR!" + md5Value + "        " + clearMD5);

                }
                catch (Exception ex)
                {
                    Load_UpgradeFile(controllerURLType,clearMD5,cleartypeNum);
                    Debug.WriteLine(" Load_UpgradeFile Failed");// Details in ex.Message and ex.HResult.
                }
            }



            //Demo byte[] change IBuff ;IBuff chang byte[]
            void ByteArrayCopy()
            {
                // Initialize a byte array.
                byte[] bytes = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

                // Create a buffer from the byte array.
                IBuffer buffer = CryptographicBuffer.CreateFromByteArray(bytes);

                // Encode the buffer into a hexadecimal string (for display);
                string hex = CryptographicBuffer.EncodeToHexString(buffer);

                // Copy the buffer back into a new byte array.
                byte[] newByteArray;
                CryptographicBuffer.CopyToByteArray(buffer, out newByteArray);
            }
        }

        private async void Load_VersionFile(string controllerURLType, int cleartypeNum)
        {
            CancellationTokenSource cts;
            cts = new CancellationTokenSource();


            var uri = new System.Uri(controllerURLType);

            using (var httpClient = new Windows.Web.Http.HttpClient())
            {
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, uri);
                // Always catch network exceptions for async methods
                try
                {
                    var result = await httpClient.GetStringAsync(uri);

                    controllerVersiosn[cleartypeNum] = Convert.ToByte(result);
                    Debug.Write("\n************************http*****************************************Clear*******Version:\n" + 
                        cleartypeNum.ToString()+":"+ controllerVersiosn[cleartypeNum].ToString());
                    loadVersiosnFinished[cleartypeNum] = true;
                    Debug.Write("\nFinished:*********Version:\n" + controllerVersiosn[4].ToString() + controllerVersiosn[5].ToString() + controllerVersiosn[6].ToString() + controllerVersiosn[7].ToString());
                    return;

                }
                catch (Exception ex)
                {
                    if(ex.Message!= "未找到(404)。\r\n\r\n响应状态代码未指示成功: 404 (Not Found)。")
                    Load_VersionFile(controllerURLType, cleartypeNum);
                  
                    Debug.WriteLine(" Load_VersionFile Failed:" + ex.Message+"\n");// Details in ex.Message and ex.HResult.
                }
            }
            clearVersiosn = 0;

        }

        private async void Load_MD5File(string controllerMD5URLType,string controllerURLType, int cleartypeNum)
        {
            CancellationTokenSource cts;
            cts = new CancellationTokenSource();

            var uri = new System.Uri(controllerMD5URLType);

            using (var httpClient = new Windows.Web.Http.HttpClient())
            {
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, uri);
                // Always catch network exceptions for async methods
                try
                {
                    clearMD5 = await httpClient.GetStringAsync(uri);
                    Debug.Write("\n************************http*****************************************clearMD5:\n" + clearMD5);
                    Load_UpgradeFile(controllerURLType, clearMD5,cleartypeNum);
                }
                catch (Exception ex)
                {
                    Load_MD5File(controllerMD5URLType, controllerURLType, cleartypeNum);
                    Debug.WriteLine(" Load_MD5File Failed"); // Details in ex.Message and ex.HResult.
                }
            }

        }

        public void UpgradeClearController(int controllertype)
        {
            switch (controllertype)
            {

                case (int)ControllerType.ReconBlack:
                    Load_MD5File(generalBlackURLMD5, generalBlackURLFirmware, controllertype);
                    break;
                case (int)ControllerType.ReconWhite:
                    Load_MD5File(generalWhiteURLMD5, generalWhiteURLFirmware, controllertype);
                    break;
                //case (int)ControllerType.clearWhite:
                //    Load_MD5File(clearWhiteURLMD5, clearWhiteURLFirmware, controllertype);
                //    break;
                //case (int)ControllerType.clearGreen:
                //    Load_MD5File(clearGreenURLMD5, clearGreenURLFirmware, controllertype);
                //    break;
                //case (int)ControllerType.clearBlue:
                //    Load_MD5File(clearBlueURLMD5, clearBlueURLFirmware, controllertype);
                //    break;
                //case (int)ControllerType.clearRed:
                //    Load_MD5File(clearRedURLMD5, clearRedURLFirmware, controllertype);
                //    break;
                //case (int)ControllerType.generalUrbanCamo:
                //    Load_MD5File(generalUrbanCamoURLMD5, generalUrbanCamoURLFirmware, controllertype);
                //    break;
                //case (int)ControllerType.generalForestCamo:
                //    Load_MD5File(generalForestCamoURLMD5, generalForestCamoURLFirmware, controllertype);
                //    break;
                //case (int)ControllerType.generalBluefront:
                //    Load_MD5File(generalBluefrontURLMD5, generalBluefrontURLFirmware, controllertype);
                //    break;
                //case (int)ControllerType.generalRedfront:
                //    Load_MD5File(generalRedfrontURLMD5, generalRedfrontURLFirmware, controllertype);
                //    break;
                default:
                    break;
            }
            
        }

        public void LoadVerionController(int controllertype)
        {
            switch (controllertype)
            {

                case (int)ControllerType.ReconBlack:
                    Load_VersionFile(generalBlackURLVersion, controllertype);
                    break;
                case (int)ControllerType.ReconWhite:
                    Load_VersionFile(generalWhiteURLVersion, controllertype);
                    break;
                //case (int)ControllerType.clearWhite:
                //    Load_VersionFile(clearWhiteURLVersion, controllertype);
                //    break;
                //case (int)ControllerType.clearGreen:
                //    Load_VersionFile(clearGreenURLVersion, controllertype);
                //    break;
                //case (int)ControllerType.clearBlue:
                //    Load_VersionFile(clearBlueURLVersion, controllertype);
                //    break;
                //case (int)ControllerType.clearRed:
                //    Load_VersionFile(clearRedURLVersion, controllertype);
                //    break;
                //case (int)ControllerType.generalUrbanCamo:
                //    Load_VersionFile(generalUrbanCamoURLVersion, controllertype);
                //    break;
                //case (int)ControllerType.generalForestCamo:
                //    Load_VersionFile(generalForestCamoURLVersion, controllertype);
                //    break;
                //case (int)ControllerType.generalBluefront:
                //    Load_VersionFile(generalBluefrontURLVersion, controllertype);
                //    break;
                //case (int)ControllerType.generalRedfront:
                //    Load_VersionFile(generalRedfrontURLVersion, controllertype);
                //    break;
                default:
                    break;
            }

        }


        /******************************************不调用*****************************************************/

        private async void Load_black_UpgradeFile()
        {

            //Windows.Storage.StorageFolder storageFolder =
            //Windows.Storage.ApplicationData.Current.LocalFolder;
            //Windows.Storage.StorageFile sampleFile =
            //await storageFolder.CreateFileAsync("update_black_compact.bin",
            //Windows.Storage.CreationCollisionOption.ReplaceExisting);


            CancellationTokenSource cts;
            cts = new CancellationTokenSource();
            //Debug.WriteLine(sampleFile+ Windows.Storage.ApplicationData.Current.LocalFolder.Path);

            var uri = new System.Uri("https://ftp.nacon.fr/firmwareblackxboxcompact");

            using (var httpClient = new Windows.Web.Http.HttpClient())
            {
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, uri);
                // Always catch network exceptions for async methods
                try
                {
                    IBuffer result = await httpClient.GetBufferAsync(uri);

                    //await Windows.Storage.FileIO.WriteBufferAsync(sampleFile, result);
                    //HttpRequestResult result1 = await httpClient.TrySendRequestAsync(
                    //request,
                    //HttpCompletionOption.ResponseHeadersRead).AsTask(cts.Token);

                    //Get File MD5 Value
                    var alg = HashAlgorithmProvider.OpenAlgorithm(HashAlgorithmNames.Md5);
                    var hashed = alg.HashData(result);
                    var md5Value = CryptographicBuffer.EncodeToHexString(hashed);

                    if (md5Value == blackMD5)
                    {
                        CryptographicBuffer.CopyToByteArray(result, out blackByteArray);
                        Debug.WriteLine(result + "\nlength:" + result.Length + " blackByteArray:  " + blackByteArray.Length);
                        blackFileLoadFinished = true;
                    }

                }
                catch (Exception ex)
                {
                    // Details in ex.Message and ex.HResult.
                    Load_black_UpgradeFile();
                    Debug.WriteLine(" Load_black_UpgradeFile Failed");
                }
            }
            //Demo byte[] change IBuff ;IBuff chang byte[]
            //void ByteArrayCopy()
            //{
            //    // Initialize a byte array.
            //    byte[] bytes = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

            //    // Create a buffer from the byte array.
            //    IBuffer buffer = CryptographicBuffer.CreateFromByteArray(bytes);

            //    // Encode the buffer into a hexadecimal string (for display);
            //    string hex = CryptographicBuffer.EncodeToHexString(buffer);

            //    // Copy the buffer back into a new byte array.
            //    byte[] newByteArray;
            //    CryptographicBuffer.CopyToByteArray(buffer, out newByteArray);
            //}
            //Load_black_MD5File();
        }

        private async void Load_white_UpgradeFile()
        {

            //Windows.Storage.StorageFolder storageFolder =
            //Windows.Storage.ApplicationData.Current.LocalFolder;
            //Windows.Storage.StorageFile sampleFile =
            //await storageFolder.CreateFileAsync("update_white_compact.bin",
            //Windows.Storage.CreationCollisionOption.ReplaceExisting);

            CancellationTokenSource cts;
            cts = new CancellationTokenSource();
            //Debug.WriteLine(sampleFile + Windows.Storage.ApplicationData.Current.LocalFolder.Path);

            var uri = new System.Uri("https://ftp.nacon.fr/firmwarewhitexboxcompact");

            using (var httpClient = new Windows.Web.Http.HttpClient())
            {
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, uri);
                // Always catch network exceptions for async methods
                try
                {
                    IBuffer result = await httpClient.GetBufferAsync(uri);

                 //   await Windows.Storage.FileIO.WriteBufferAsync(sampleFile, result);
                 //   HttpRequestResult result1 = await httpClient.TrySendRequestAsync(
                 //request,
                 //HttpCompletionOption.ResponseHeadersRead).AsTask(cts.Token);
                 //   string strresult = result.ToString();

                    //Get File MD5 Value
                    var alg = HashAlgorithmProvider.OpenAlgorithm(HashAlgorithmNames.Md5);
                    var hashed = alg.HashData(result);
                    var md5Value = CryptographicBuffer.EncodeToHexString(hashed);

                    if (whiteMD5 == md5Value)
                    {
                        CryptographicBuffer.CopyToByteArray(result, out whiteByteArray);
                        Debug.WriteLine(result + "\nlength:" + result.Length + " whiteByteArray.Length:  " + whiteByteArray.Length);
                        whiteFlleLoadFinished = true;
                    }
                    else
                        Debug.WriteLine("000\n\nMD5 ERROR!"+ md5Value+"        "+ whiteMD5);

                }
                catch (Exception ex)
                {
                    Load_white_UpgradeFile();
                    Debug.WriteLine(" Load_white_UpgradeFile Failed");// Details in ex.Message and ex.HResult.
                }
            }



            //Demo byte[] change IBuff ;IBuff chang byte[]
            void ByteArrayCopy()
            {
                // Initialize a byte array.
                byte[] bytes = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

                // Create a buffer from the byte array.
                IBuffer buffer = CryptographicBuffer.CreateFromByteArray(bytes);

                // Encode the buffer into a hexadecimal string (for display);
                string hex = CryptographicBuffer.EncodeToHexString(buffer);

                // Copy the buffer back into a new byte array.
                byte[] newByteArray;
                CryptographicBuffer.CopyToByteArray(buffer, out newByteArray);
            }
        }

        public async void Load_black_VersionFile()
        {
            CancellationTokenSource cts;
            cts = new CancellationTokenSource();


            var uri = new System.Uri("https://ftp.nacon.fr/versionblackxboxcompact");

            using (var httpClient = new Windows.Web.Http.HttpClient())
            {
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, uri);
                // Always catch network exceptions for async methods
                try
                {
                    var result = await httpClient.GetStringAsync(uri);

                    blackVersiosn = Convert.ToByte(result);
                    Debug.Write("\n************************http*****************************************blackVersion:\n" + blackVersiosn.ToString());
                    Load_white_VersionFile();

                }
                catch (Exception ex)
                {
                     Load_black_VersionFile();
                    Debug.WriteLine(" Load_black_VersionFile Failed");// Details in ex.Message and ex.HResult.
                }
            }






            //Demo byte[] change IBuff ;IBuff chang byte[]
            void ByteArrayCopy()
            {
                // Initialize a byte array.
                byte[] bytes = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

                // Create a buffer from the byte array.
                IBuffer buffer = CryptographicBuffer.CreateFromByteArray(bytes);

                // Encode the buffer into a hexadecimal string (for display);
                string hex = CryptographicBuffer.EncodeToHexString(buffer);

                // Copy the buffer back into a new byte array.
                byte[] newByteArray;
                CryptographicBuffer.CopyToByteArray(buffer, out newByteArray);
            }
        }

        public async void Load_white_VersionFile()
        {
            CancellationTokenSource cts;
            cts = new CancellationTokenSource();
            var uri = new System.Uri("https://ftp.nacon.fr/versionwhitexboxcompact");

            using (var httpClient = new Windows.Web.Http.HttpClient())
            {
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, uri);
                // Always catch network exceptions for async methods
                try
                {
                    var result = await httpClient.GetStringAsync(uri);

                    whiteVersiosn = Convert.ToByte(result);
                    
                    Debug.Write("\n************************http*****************************************whiteVersion:\n" + whiteVersiosn.ToString());
                   // Load_VersionFile(clearWhiteURLVersion);

                }
                catch (Exception ex)
                {
                    Load_white_VersionFile();
                    Debug.WriteLine(" Load_white_VersionFile Failed");  // Details in ex.Message and ex.HResult.
                }
            }











            //Demo byte[] change IBuff ;IBuff chang byte[]
            void ByteArrayCopy()
            {
                // Initialize a byte array.
                byte[] bytes = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

                // Create a buffer from the byte array.
                IBuffer buffer = CryptographicBuffer.CreateFromByteArray(bytes);

                // Encode the buffer into a hexadecimal string (for display);
                string hex = CryptographicBuffer.EncodeToHexString(buffer);

                // Copy the buffer back into a new byte array.
                byte[] newByteArray;
                CryptographicBuffer.CopyToByteArray(buffer, out newByteArray);
            }
        }

        public async void Load_black_MD5File()
        {
            CancellationTokenSource cts;
            cts = new CancellationTokenSource();

            var uri = new System.Uri("https://ftp.nacon.fr/crcmd5blackxboxcompact");

            using (var httpClient = new Windows.Web.Http.HttpClient())
            {
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, uri);
                // Always catch network exceptions for async methods
                try
                {
                    blackMD5 = await httpClient.GetStringAsync(uri);
                    Debug.Write("\n************************http*****************************************blackMD5:\n" + blackMD5);
                    Load_black_UpgradeFile();
                }
                catch (Exception ex)
                {
                    Load_black_MD5File();
                    Debug.WriteLine(" Load_black_MD5File Failed"); // Details in ex.Message and ex.HResult.
                }
            }

        }

        public async void Load_white_MD5File()
        {
            CancellationTokenSource cts;
            cts = new CancellationTokenSource();

            var uri = new System.Uri("https://ftp.nacon.fr/crcmd5whitexboxcompact");

            using (var httpClient = new Windows.Web.Http.HttpClient())
            {
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, uri);
                // Always catch network exceptions for async methods
                try
                {
                    whiteMD5 = await httpClient.GetStringAsync(uri);
                    Debug.Write("\n************************http*****************************************whiteMD5:\n" + whiteMD5);
                    Load_white_UpgradeFile();

                }
                catch (Exception ex)
                {
                    Load_white_MD5File();
                    Debug.WriteLine("Load_white_MD5File Failed!");// Details in ex.Message and ex.HResult.
                }
            }
        }

        /******************************************不调用 结束*****************************************************/


        public bool IsBlackFileLoadFinished()
        {
            return blackFileLoadFinished;
        }
        public bool IsWhiteFileLoadFinished()
        {
            return whiteFlleLoadFinished;
        }
        public bool IsClearFileLoadFinished(int cleartype)
        {
            return fileLoadFinished[cleartype];
        }

        public ushort GetBlackControllerVersion()
        {
            return blackVersiosn;
        }
        public ushort GetWhiteControllerVersion()
        {
            return whiteVersiosn;
        }

        public bool IsLoadVersionFinished(int cleartype)
        {
            if (!versionFlg[cleartype])
            {
                versionFlg[cleartype] = true;
                LoadVerionController(cleartype);
            }

            return loadVersiosnFinished[cleartype];
        }
        public ushort GetClearControllerVersion(int controllertype)
        {
            return controllerVersiosn[controllertype];
        }
    }
}
