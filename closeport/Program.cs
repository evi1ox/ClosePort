using Microsoft.Win32;
using System;
using System.Linq;
using System.ServiceProcess;

namespace service
{
    class Program
    {

        static public bool DisableSc(string ScName)
        {
            try
            {
                RegistryKey NetBT = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Services\" + ScName, true);
                NetBT.SetValue("Start", 4, RegistryValueKind.DWord);
                NetBT.Close();
            }
            catch (Exception err)
            {
                Console.WriteLine("Error: {0}", err);
                return false;
            }

            return true;
        }
        static void Main(string[] args)
        {
            var serviceControllers = ServiceController.GetServices();
            string[] ScName = { "MSDTC", "lmhosts", "SessionEnv", "Browser", "LanmanServer", "LanmanWorkstation" };
            foreach (var ServiceName in ScName)
            {
                var server = serviceControllers.FirstOrDefault(service => service.ServiceName == ServiceName);
                if (server != null)
                {
                    if (server.Status == ServiceControllerStatus.Running)
                    {
                        server.Stop();
                    }

                    if (ServiceName == "SessionEnv")
                    {
                        continue;
                    }
                    DisableSc(ServiceName);
                }
            }

            //禁用135
            try
            {
                RegistryKey NetBT = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Ole", true);
                NetBT.SetValue("EnableDCOM", "N", RegistryValueKind.String);
                NetBT.Close();
            }
            catch (Exception err)
            {
                Console.WriteLine("Error: {0}", err);
            }

            try
            {
                RegistryKey NetBT = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Rpc", true);
                NetBT.SetValue("DCOM Protocols", new string[] { }, RegistryValueKind.MultiString);
                NetBT.CreateSubKey("Internet");
                NetBT.Close();
            }
            catch (Exception err)
            {
                Console.WriteLine("Error: {0}", err);
            }


            //禁用NetBIOS 139
            try
            {
                RegistryKey NetBT = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Services\NetBT", true);
                NetBT.SetValue("Start", 4, RegistryValueKind.DWord);
                NetBT.Close();
            }
            catch (Exception err)
            {
                Console.WriteLine("Error: {0}", err);
            }

            // 登录时无须按 Ctrl+Alt+Del
            try
            {
                RegistryKey NetBT = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\Currentversion\Policies\System", true);
                NetBT.SetValue("disablecad", 0, RegistryValueKind.DWord);
                NetBT.Close();
            }
            catch (Exception err)
            {
                Console.WriteLine("Error: {0}", err);
            }


            // 关闭445端口
            try
            {
                RegistryKey NetBT = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\services\NetBT\Parameters", true);
                NetBT.SetValue("SMBDeviceEnabled", 0, RegistryValueKind.DWord);
                NetBT.Close();
            }
            catch (Exception err)
            {
                Console.WriteLine("Error: {0}", err);
            }

            Console.WriteLine("Success, Please restart the computer to completely close the port! ");

        }
    }
}
