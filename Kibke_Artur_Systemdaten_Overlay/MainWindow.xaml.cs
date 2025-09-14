using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.IO;
using System.Net.Http;
using System.Windows.Forms;


namespace Kibke_Artur_Systemdaten_Overlay
{

    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window 
    {
        //Objekten Liste wird gesetzt
        ObservableCollection<Daten> systemDaten = new ObservableCollection<Daten>();

        //neuen Timer gesetzt
        DispatcherTimer timer = new DispatcherTimer();

        public MainWindow()
        {
            InitializeComponent();

            // Fensterposition: oben rechts mit 10px Abstand
            this.Left = SystemParameters.WorkArea.Width - this.Width - 10;
            this.Top = 10;

            if (!HatAkku())
            {
                //Hier wird das Akku aus dem Fester infernt und angepasst
                AkkuRow.Visibility = Visibility.Collapsed;
                this.Height -= 20;
            }
            
            //Funktion zur Aktualisierung der CPU daten ablauf eines timers
            StartTimer();
        }

        void StartTimer()
        {
            //Neues liste daten
            Daten daten = new Daten();

            //Timer wird gesetzt
            timer.Interval = TimeSpan.FromSeconds(1);

            //Wird nach ablauf des Timers Benutzt
            timer.Tick += (s, e) =>
            {
                VerschiebeFensterAufZweitenMonitor();
                
                //Klassen Funtkion wird auf gerufen und in TextBlock verfasst
                CpuTextBlock.Text = $"{daten.CpuNutzung():F1}%";
                GpuTextBlock.Text = $"{daten.GpuNutzung():F1}%";
                RamTextBlock.Text = $"{daten.RamNutzung()*0.001:F1} / {daten.Memory:F1} GB";
                AkkuTextBlock.Text = $"{daten.AkkuProzent()}%";
                IpTextBlock.Text = $"{daten.GetLocalIPAddress()}";
                PingTextBlock.Text = $"{daten.Ping()} ms";
                LaufzeitTextBlock.Text = daten.Laufzeit();

            };

            //Timer wird wiederholt
            timer.Start();
        }

        private void VerschiebeFensterAufZweitenMonitor()
        {
            var screens = Screen.AllScreens;

            if (screens.Length > 1)
            {
                // Zweiten Monitor auswählen
                var monitor = screens[1];

                // Fensterposition setzen
                this.Left = monitor.WorkingArea.Right - this.Width - 10;
                this.Top = monitor.WorkingArea.Top + 10;
            }
            else
            {
                System.Windows.MessageBox.Show("Nur ein Monitor erkannt.");
            }
        }

        private bool HatAkku()
        {
            //Abfrage ob das gerät eine Batterie hat.
            var status = System.Windows.Forms.SystemInformation.PowerStatus.BatteryChargeStatus;
            return status != System.Windows.Forms.BatteryChargeStatus.NoSystemBattery;
        }
    }
}
