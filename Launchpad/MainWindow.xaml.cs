using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
//using System.Windows.Input;
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

        InputDevice inputDevice = InputDevice.InstalledDevices[0];
        OutputDevice outputDevice = OutputDevice.InstalledDevices[1];

        public MainWindow()
        {
            ProcessTimer.Tick += ProcessUpdate;
            ProcessTimer.Interval = TimeSpan.FromMilliseconds(10);

            InitializeComponent();

            InputDevicesListbox.ItemsSource = InputDevice.InstalledDevices;
            OutputDevicesListbox.ItemsSource = OutputDevice.InstalledDevices;

            // 테스트용 HitObject 생성
            hitObjects.Enqueue(new HitObject(1875, 2, 6));
            hitObjects.Enqueue(new HitObject(2343, 3));
            hitObjects.Enqueue(new HitObject(2460, 8));
            hitObjects.Enqueue(new HitObject(2695, 1, 5));
            hitObjects.Enqueue(new HitObject(3281, 12));
            hitObjects.Enqueue(new HitObject(3398, 10));
            hitObjects.Enqueue(new HitObject(3515, 11));
            hitObjects.Enqueue(new HitObject(3632, 9));

            ProcessTimer.Start();
            startPoint = DateTime.Now;

            Notes.ItemsSource = hitObjects;

            outputDevice.Open();

            FillPannel(15, 0);
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
                console.WriteLine(WaitingObject.Timing);
                WaitingObject = null;
            }
        }

        private void NoteOn(int x, int y, int color)
        {
            if(x < 0 || x > 7 || y < 0 || y > 7)
            {
                throw new IndexOutOfRangeException();
            }

            int pos = (x + 1) + (8 - y) * 10;

            outputDevice.SendNoteOn(Channel.Channel1, (Pitch)pos, color);
        }

        private int GetPannelPosition(bool xy, int number, int status)
        {
            if (xy)
            {
                switch (status)
                {
                    case 0:
                        return (number % 4) * 2;
                    case 1:
                        return (number % 4) * 2 + 1;
                    case 2:
                        return (number % 4) * 2 + 1;
                    case 3:
                        return (number % 4) * 2;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
            else
            {
                switch (status)
                {
                    case 0:
                        return (number / 4) * 2;
                    case 1:
                        return (number / 4) * 2;
                    case 2:
                        return (number / 4) * 2 + 1;
                    case 3:
                        return (number / 4) * 2 + 1;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
        }

        private void FillPannel(int number, int color)
        {
            NoteOn(GetPannelPosition(true, number, 0), GetPannelPosition(false, number, 0), color);
            NoteOn(GetPannelPosition(true, number, 1), GetPannelPosition(false, number, 1), color);
            NoteOn(GetPannelPosition(true, number, 2), GetPannelPosition(false, number, 2), color);
            NoteOn(GetPannelPosition(true, number, 3), GetPannelPosition(false, number, 3), color);
        }
    }
}
