using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Management;

namespace DetectAll
{
    public partial class Form1 : Form
    {
        internal static class UsbNotification
        {
            public const int DbtDevicearrival = 0x8000; // system detected a new device        
            public const int DbtDeviceremovecomplete = 0x8004; // device is gone      
            public const int WmDevicechange = 0x0219; // device change event      
            private const int DbtDevtypDeviceinterface = 5;
            private static readonly Guid GuidDevinterfaceUSBDevice = new Guid("50dd5230-ba8a-11d1-bf5d-0000f805f530"); 
            private static IntPtr notificationHandle;
            
            /// <summary>
            /// Registers a window to receive notifications when USB devices are plugged or unplugged.
            /// </summary>
            /// <param name="windowHandle">Handle to the window receiving notifications.</param>
            public static void RegisterUsbDeviceNotification(IntPtr windowHandle)
            {
                
                DevBroadcastDeviceinterface dbi = new DevBroadcastDeviceinterface
                {
                    DeviceType = DbtDevtypDeviceinterface,
                    Reserved = 0,
                    ClassGuid = GuidDevinterfaceUSBDevice,
                    Name = 0
                };

                dbi.Size = Marshal.SizeOf(dbi);
                IntPtr buffer = Marshal.AllocHGlobal(dbi.Size);
                Marshal.StructureToPtr(dbi, buffer, true);

                notificationHandle = RegisterDeviceNotification(windowHandle, buffer, 0);
            }

            /// <summary>
            /// Unregisters the window for USB device notifications
            /// </summary>
            public static void UnregisterUsbDeviceNotification()
            {
                UnregisterDeviceNotification(notificationHandle);
            }

            [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            private static extern IntPtr RegisterDeviceNotification(IntPtr recipient, IntPtr notificationFilter, int flags);

            [DllImport("user32.dll")]
            private static extern bool UnregisterDeviceNotification(IntPtr handle);

            [StructLayout(LayoutKind.Sequential)]
            public struct DevBroadcastDeviceinterface
            {
                internal int Size;
                public int DeviceType;
                internal int Reserved;
                internal Guid ClassGuid;
                internal short Name;
            }

        }
        public Form1()
        {
            InitializeComponent();
            UsbNotification.RegisterUsbDeviceNotification(this.Handle);
        }
        public struct DevBroadcastDeviceinterface
        {
            internal int Size;
            public int DeviceType;
            internal int Reserved;
            internal Guid ClassGuid;
            internal short Name;
        }
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct DISPLAY_DEVICE
        {
            [MarshalAs(UnmanagedType.U4)]
            public int cb;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string DeviceName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string DeviceString;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string DeviceID;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string DeviceKey;
        }
        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            if (m.Msg == UsbNotification.WmDevicechange)
            {
              DevBroadcastDeviceinterface dbh;
                DISPLAY_DEVICE dbj;
                switch ((int)m.WParam)
                {
                    case UsbNotification.DbtDevicearrival:
                        dbj = (DISPLAY_DEVICE)Marshal.PtrToStructure(m.LParam, typeof(DISPLAY_DEVICE));
                      dbh = (DevBroadcastDeviceinterface)Marshal.PtrToStructure(m.LParam, typeof(DevBroadcastDeviceinterface));
                         /*if ((dbh.ClassGuid).ToString() == "50dd5230-ba8a-11d1-bf5d-0000f805f530")
                         {   listBox1.Items.Add(""+dbj.DeviceString.Substring(0,4));                   //Burdan DeviceString'i öğrenebiliriz.
                             listBox1.Items.Add("Akıllı kart okuyucusu takıldı.");
                         }*/
                        if(dbh.ClassGuid.ToString() == "50dd5230-ba8a-11d1-bf5d-0000f805f530")
                        {
                            if (dbj.DeviceString.Substring(0, 4) == "0732")
                            {
                                listBox1.Items.Add("Akıllı kart okuyucusu takıldı.");
                            }
                        }
                        
                        break;
                    case UsbNotification.DbtDeviceremovecomplete:
                      dbh = (DevBroadcastDeviceinterface)Marshal.PtrToStructure(m.LParam, typeof(DevBroadcastDeviceinterface));
                        dbj = (DISPLAY_DEVICE)Marshal.PtrToStructure(m.LParam, typeof(DISPLAY_DEVICE));
                        if (dbh.ClassGuid.ToString() == "50dd5230-ba8a-11d1-bf5d-0000f805f530")
                        {
                            if (dbj.DeviceString.Substring(0, 4) == "0732")
                            {
                                listBox1.Items.Add("Akıllı kart okuyucusu çıkarıldı.");
                            }

                        }
                            
                        break;
                }
            }
        }
    }
}
