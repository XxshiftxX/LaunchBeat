using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
using Midi;

namespace Launchpad
{
    public partial class MainWindow : Window
    {
        Queue<HitObject> hitObjects = new Queue<HitObject>();

        HitObject WaitingObject;

        DateTime startPoint;

        DispatcherTimer ProcessTimer = new DispatcherTimer();
        DispatcherTimer DrawTimer = new DispatcherTimer();

        public MainWindow()
        {
            ProcessTimer.Tick += ProcessUpdate;
            ProcessTimer.Interval = TimeSpan.FromMilliseconds(10);

            InitializeComponent();

            InputDevicesListbox.ItemsSource = Midi.InputDevice.InstalledDevices;
            OutputDevicesListbox.ItemsSource = OutputDevice.InstalledDevices;
        }
    }
}
