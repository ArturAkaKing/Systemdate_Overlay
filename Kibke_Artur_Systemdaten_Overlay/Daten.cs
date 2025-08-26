using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Management.Instrumentation;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Forms;
using System.IO;
using System.Net.Http;



namespace Kibke_Artur_Systemdaten_Overlay
{
    internal class Daten
    {
        //Variable für den eingebauten RAM
        public int Memory;

        //Hier mit kriegt man den eingebauten RAM
        [DllImport("kernel32.dll")]
        public static extern bool GetPhysicallyInstalledSystemMemory(out long totalMemoryInKilobytes);

        
        //Das setzt die Instance cpuCounter und ramCounter um die Verschiedenen Werte zu holen
        private PerformanceCounter cpuCounter;
        private PerformanceCounter ramCounter;

        public Daten()
        {
            //Die Klammer gibt die CPU Daten wieder (Da kann auch was anderes stehen um Ram z.B. zu ziehen)
            cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");

            //RAM ziehen
            ramCounter = new PerformanceCounter("Memory","Available MBytes");
            
            //Erste Abfrage immer leer
            cpuCounter.NextValue(); 
            ramCounter.NextValue();
            

            
            //Warten
            Thread.Sleep(500);
            
            //Liefert den Wert der Eingebauten RAM Platine
           if (GetPhysicallyInstalledSystemMemory(out long totalMemoryKB))
           {
                //Hier wird der von einem long in ein int Convertiert und ausgerechnet
                Memory = (int)(totalMemoryKB / 1024 / 1024);
           } 
        }

        //Holt sich die IPAdresse
        public string GetLocalIPAddress()
        {
            //Hier wird die IP vom host gespeichert in dem er den Name des host kennt
            var host = Dns.GetHostEntry(Dns.GetHostName());

            //Check der IPv4
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }
        
        //Holt sich den Ping
        public int Ping()
        {
            //Neue ping Instance
            Ping ping1 = new Ping();

            //Hier wird der ping gesendet zu 8.8.8.8(Google.de kann man auch hinschreiben)
            //und mit Reply wird der vorgang gespeichert
            PingReply ping = ping1.Send("8.8.8.8");
            
            //Hier werden die Millisekunden gezählt
            return (int)ping.RoundtripTime;
        }

        public float CpuNutzung()
        {    
            //Liefert CPU Wert
            return cpuCounter.NextValue(); 
        }

        public float RamNutzung()
        {
            //Liefert RAM Wert
            return ramCounter.NextValue();
        }

        //WMI (Funktioniert nur bei neuern GPUs)
        public float GpuNutzung()
        {
            //Ausgegebener wert am ende
            float lastValue = 0;

            //Suche nach der GPU 
            var searcher = new ManagementObjectSearcher("root\\CIMV2",
                "SELECT * FROM Win32_PerfFormattedData_GPUPerformanceCounters_GPUEngine");

            //Hier sucht er und zieht sich den Wert
            foreach (ManagementObject obj in searcher.Get())
            {
                string name = obj["Name"]?.ToString() ?? "";
                if (name.Contains("engtype_3D")) //Die 3D-Engine der GPU
                {
                    lastValue += Convert.ToSingle(obj["UtilizationPercentage"]);
                }
            }
            return lastValue;
        }

        public int AkkuProzent()
        {
            //Das ist der Status für Akku aber ein PC hat kein weshalb da eigentlich
            //immer 0 wäre aber mal 100 sind es 100% und in int umgewandelt
            return (int)(SystemInformation.PowerStatus.BatteryLifePercent * 100);
        }
    }
}
