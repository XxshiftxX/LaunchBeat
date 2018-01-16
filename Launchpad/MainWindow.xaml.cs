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

        int[] currentHitObjects;

        HitObject WaitingObject;

        DateTime startPoint;
        TimeSpan nowTiming;

        DispatcherTimer ProcessTimer = new DispatcherTimer();
        DispatcherTimer DrawTimer = new DispatcherTimer();

        public MainWindow()
        {
            ProcessTimer.Tick += ProcessUpdate;
            ProcessTimer.Interval = TimeSpan.FromMilliseconds(10);

            InitializeComponent();

            InputDevicesListbox.ItemsSource = Midi.InputDevice.InstalledDevices;
            OutputDevicesListbox.ItemsSource = OutputDevice.InstalledDevices;

            // 테스트용 HitObject 생성
            hitObjects.Enqueue(new HitObject(1000, ));
            hitObjects.Enqueue(new HitObject(2000));
            hitObjects.Enqueue(new HitObject(2500));
            hitObjects.Enqueue(new HitObject(3000));
            hitObjects.Enqueue(new HitObject(4000));
            hitObjects.Enqueue(new HitObject(4500));
            hitObjects.Enqueue(new HitObject(4800));
            hitObjects.Enqueue(new HitObject(5000));

            ProcessTimer.Start();
            startPoint = DateTime.Now;

            Notes.ItemsSource = hitObjects;
        }

        private void ProcessUpdate(object sender, EventArgs args)
        {
            nowTiming = DateTime.Now - startPoint;

            if (WaitingObject == null)
            {
                if(hitObjects.Count > 0)
                {
                    WaitingObject = hitObjects.Dequeue(); 
                }
                else
                {
                    ProcessTimer.Stop();
                    return;
                }
            }

            if (nowTiming.TotalMilliseconds > WaitingObject.Timing)
            {
                Console.WriteLine(WaitingObject.Timing);
                WaitingObject = null;
            }
        }
    }
}
