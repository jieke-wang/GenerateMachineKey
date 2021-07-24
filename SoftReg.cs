using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management;
using System.Text;
using System.Security;

namespace GenerateMachineKey
{
    internal class SoftReg
    {
        /// <summary>
        /// 取得设备硬盘的卷标号
        /// </summary>
        /// <returns></returns>
        public string GetDiskVolumeSerialNumber()
        {
            //ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
            ManagementObject disk = new ManagementObject("win32_logicaldisk.deviceid=\"c:\"");
            disk.Get();
            return disk.GetPropertyValue("VolumeSerialNumber").ToString();
        }

        /// <summary>
        /// 获取硬盘信息
        /// </summary>
        /// <returns></returns>
        public string GetHardDiskInfos()
        {
            System.Text.StringBuilder hardDiskInfos = new System.Text.StringBuilder();

            ManagementClass myHardDisks = new ManagementClass("Win32_DiskDrive");
            ManagementObjectCollection hardDiskCollection = myHardDisks.GetInstances();

            foreach (ManagementObject hardDiskObject in hardDiskCollection)
            {
                hardDiskInfos.Append(hardDiskObject.Properties["Model"].Value.ToString());
            }

            return hardDiskInfos.ToString();
        }

        /// <summary>
        /// 获得CPU信息
        /// </summary>
        /// <returns></returns>
        public string getCpuInfos()
        {
            System.Text.StringBuilder cpuInfos = new System.Text.StringBuilder();

            ManagementClass myCpus = new ManagementClass("win32_Processor");
            ManagementObjectCollection cpuConnection = myCpus.GetInstances();

            foreach (ManagementObject cpuObject in cpuConnection)
            {
                cpuInfos.Append(cpuObject.Properties["Processorid"].Value.ToString());
            }
            return cpuInfos.ToString();
        }

        public string GetNetworkAdapterInfos()
        {
            System.Text.StringBuilder networkAdapterInfos = new System.Text.StringBuilder();

            ManagementClass myNetworkAdapters = new ManagementClass("Win32_NetworkAdapterConfiguration");
            ManagementObjectCollection networkAdapterCollection = myNetworkAdapters.GetInstances();

            foreach (ManagementObject networkAdapterObject in networkAdapterCollection)
            {
                if ((bool)networkAdapterObject["IPEnabled"] == true)
                    networkAdapterInfos.Append(networkAdapterObject["MacAddress"].ToString());
            }

            return CalculateMd5Hash(networkAdapterInfos.ToString());
        }

        public string CalculateMd5Hash(string input)
        {
            System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
            byte[] hash = md5.ComputeHash(inputBytes);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }
            return sb.ToString();
        }

        /// <summary>
        /// 生成机器码
        /// </summary>
        /// <returns></returns>
        public string getMNum()
        {
            StringBuilder sbNumber = new System.Text.StringBuilder();
            sbNumber.Append(getCpuInfos());
            sbNumber.Append(GetHardDiskInfos());
            sbNumber.Append(GetNetworkAdapterInfos());
            return sbNumber.ToString();
        }
        public int[] intCode = new int[127];//存储密钥
        public int[] intNumber = new int[25];//存机器码的Ascii值
        public char[] Charcode = new char[25];//存储机器码字
        public void setIntCode()//给数组赋值小于10的数
        {
            for (int i = 1; i < intCode.Length; i++)
            {
                intCode[i] = i % 9;
            }
        }

        /// <summary>
        /// 生成注册码
        /// </summary>
        /// <returns></returns>
        public string getRNum()
        {
            setIntCode();//初始化127位数组
            string MNum = this.getMNum();//获取注册码
            for (int i = 1; i < Charcode.Length; i++)//把机器码存入数组中
            {
                Charcode[i] = Convert.ToChar(MNum.Substring(i - 1, 1));
            }
            for (int j = 1; j < intNumber.Length; j++)//把字符的ASCII值存入一个整数组中。
            {
                intNumber[j] = intCode[Convert.ToInt32(Charcode[j])] + Convert.ToInt32(Charcode[j]);
            }
            StringBuilder strAsciiName = new System.Text.StringBuilder();//用于存储注册码
            for (int j = 1; j < intNumber.Length; j++)
            {
                if (intNumber[j] >= 48 && intNumber[j] <= 57)//判断字符ASCII值是否0－9之间
                {
                    strAsciiName.Append(Convert.ToChar(intNumber[j]).ToString());
                }
                else if (intNumber[j] >= 65 && intNumber[j] <= 90)//判断字符ASCII值是否A－Z之间
                {
                    strAsciiName.Append(Convert.ToChar(intNumber[j]).ToString());
                }
                else if (intNumber[j] >= 97 && intNumber[j] <= 122)//判断字符ASCII值是否a－z之间
                {
                    strAsciiName.Append(Convert.ToChar(intNumber[j]).ToString());
                }
                else//判断字符ASCII值不在以上范围内
                {
                    if (intNumber[j] > 122)//判断字符ASCII值是否大于z
                    {
                        strAsciiName.Append(Convert.ToChar(intNumber[j] - 10).ToString());
                    }
                    else
                    {
                        strAsciiName.Append(Convert.ToChar(intNumber[j] - 9).ToString());
                    }
                }
            }
            return strAsciiName.ToString();//返回注册码
        }
    }
}

//http://www.blue1000.com/bkhtml/c17/2010-12/70231.htm
//http://bbs.csdn.net/topics/370066824

//http://blog.csdn.net/dq9005/article/details/8874604
//http://www.cnblogs.com/jx270/archive/2013/01/21/2869116.html

//http://bbs.csdn.net/topics/360042162