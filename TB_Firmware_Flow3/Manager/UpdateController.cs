using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using D4XGaming.Devices;
namespace UpgradeCongroller
{
    class UpdateController
    {
        enum GipMessage
        {
            Command = 0,
            LowLatency,
            StandardLatency

        };
        public byte EnterUpgade = 0x1;
        public byte ConFirmUpgrade = 0x3;
        private readonly byte[] payloadB;
        D4XDevice device_xbox;

        public void IniDeviceXbox(D4XDevice device_xbox)
        {
            if (device_xbox != null)
                this.device_xbox = device_xbox;

        }
        /*****CRC 校验*****************/
        private uint CRC_SEED = 0;

        private uint calcFast(uint seed, byte c)
        {
            uint h1, h2, h3, h4;
            h1 = (seed ^ (uint)c) & 0xFF;
            h2 = h1 & 0x0F;
            h3 = (h2 << 4) ^ h1;
            h4 = h3 >> 4;
            UInt16 dd = 0;
            UInt16 ff = (UInt16)(dd ^ 44);
            return (((((h3 << 1) ^ h4) << 4) ^ h2) << 3) ^ h4 ^ (seed >> 8);
        }
        private uint CRC_Check(byte[] src, Int16 len)
        {
            uint i;
            uint crc;
            crc = calcFast(CRC_SEED, src[0]);
            for (i = 1; i < len; i++)
            {
                crc = calcFast((byte)crc, src[i]);

            }
            return crc;
        }
        public void EnterUpgradeMode(byte com)
        {

            byte[] payload = new byte[0x5];
            payload[0] = 0x0;
            payload[1] = 0x13;
            payload[2] = com;
            payload[3] = (byte)CRC_Check(payload, 3);
            payload[4] = (byte)(CRC_Check(payload, 3) >> 8);
            device_xbox.SendMessageA((int)GipMessage.LowLatency, 0x1, payload);
            //  uint length;
            //  device_xbox.GetMessageB(payloadB, out length);
            return;
        }

        public void RebootDevice(UInt16 checkNum)
        {
            byte[] payload = new byte[0x7];
            uint crc = CRC_Check(payload, 5);
            payload[0] = 0x0;
            payload[1] = 0x13;
            payload[2] = 0x05;
            payload[3] = (byte)(checkNum >> 8);
            payload[4] = (byte)(checkNum);
            payload[5] = (byte)CRC_Check(payload, 5);
            payload[6] = (byte)(CRC_Check(payload, 5));
            device_xbox.SendMessageA((int)GipMessage.LowLatency, 0x1, payload);
        }
        /*
         * 功能发送文件数据
         * 
         * data 升级文件数据
         * 
         * count 升级计数
         * 
         */
        public bool WritUpgradeFile(byte[] data, byte count)
        {
            byte[] payload = new byte[60];
            //payload.CopyTo(payload, 0);
            payload[0] = count;
            payload[1] = 0x13;
            payload[2] = 0x12;
            data.CopyTo(payload, 3);
            payload[58] = (byte)CRC_Check(payload, 58);
            payload[59] = (byte)(CRC_Check(payload, 58) >> 16);
            device_xbox.SendMessageA((int)GipMessage.LowLatency, 0x1, payload);


            return false;
        }
        public bool WritUpgradeFile1(byte[] data, byte count, int num = 55)
        {

            byte[] payload = new byte[5 + num];
            //payload.CopyTo(payload, 0);
            payload[0] = count;
            payload[1] = 0x13;
            payload[2] = 0x12;
            data.CopyTo(payload, 3);
            payload[5 + num - 2] = (byte)CRC_Check(payload, (short)(5 + num - 2));
            payload[5 + num - 1] = (byte)(CRC_Check(payload, (short)(5 + num - 2)) >> 16);
            device_xbox.SendMessageA((int)GipMessage.LowLatency, 0x1, payload);


            return false;
        }
    }

}
