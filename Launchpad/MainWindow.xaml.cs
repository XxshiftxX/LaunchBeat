using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
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
        InputDevice inputDevice;
        OutputDevice outputDevice;

        Queue<HitObject> hitObjects = new Queue<HitObject>();

        int[] currentHitObjects = new int[16]
            { -1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1 };

        HitObject WaitingObject;

        DateTime startPoint;
        TimeSpan nowTiming;

        DispatcherTimer ProcessTimer = new DispatcherTimer();
        DispatcherTimer DrawTimer = new DispatcherTimer();

        int fadeinAnimationSpeed = 4;
        int fadeoutAnimationSpeed = 2;
        int idleAnimationSpeed = 10;

        int[] fadeColors = new int[4] { 71, 1, 2, 3 };
        int[] fadeColors2 = new int[4] { 83, 11, 10, 9 };

        public MainWindow()
        {
            ProcessTimer.Tick += ProcessUpdate;
            ProcessTimer.Interval = TimeSpan.FromMilliseconds(1);
            DrawTimer.Tick += DrawUpdate;
            DrawTimer.Interval = TimeSpan.FromMilliseconds(0.5);

            InitializeComponent();

            InputDevicesListbox.ItemsSource = Midi.InputDevice.InstalledDevices;
            OutputDevicesListbox.ItemsSource = OutputDevice.InstalledDevices;

            Notes.ItemsSource = hitObjects;

            inputDevice = InputDevice.InstalledDevices[0];
            outputDevice = OutputDevice.InstalledDevices[1];

            outputDevice.Open();

            int interval = 100;
            hitObjects.Enqueue(new HitObject(1000 + interval * 1, 0));
            //hitObjects.Enqueue(new HitObject(1000 + interval * 2, 1));
            hitObjects.Enqueue(new HitObject(1000 + interval * 3, 2));
            //hitObjects.Enqueue(new HitObject(1000 + interval * 4, 3));
            
            hitObjects.Enqueue(new HitObject(1000 + interval * 5, 7));
            //hitObjects.Enqueue(new HitObject(1000 + interval * 6, 6));
            hitObjects.Enqueue(new HitObject(1000 + interval * 7, 5));
            //hitObjects.Enqueue(new HitObject(1000 + interval * 8, 4));
            
            hitObjects.Enqueue(new HitObject(1000 + interval * 9, 8));
            //hitObjects.Enqueue(new HitObject(1000 + interval * 10, 9));
            hitObjects.Enqueue(new HitObject(1000 + interval * 11, 10));
            //hitObjects.Enqueue(new HitObject(1000 + interval * 12, 11));
            hitObjects.Enqueue(new HitObject(1000 + interval * 13, 15));
            //hitObjects.Enqueue(new HitObject(1000 + interval * 14, 14));
            hitObjects.Enqueue(new HitObject(1000 + interval * 15, 13));
            //hitObjects.Enqueue(new HitObject(1000 + interval * 16, 12));
            

            ProcessTimer.Start();
            DrawTimer.Start();
            startPoint = DateTime.Now;
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
                foreach(int key in WaitingObject.Note)
                {
                    currentHitObjects[key] = 0;
                    ConsoleBlock.Text += key + "\n";
                }
                WaitingObject = null;
            }
        }

        private void DrawUpdate(object sender, EventArgs args)
        {
            for(int key = 0; key < 16; key++)
            {
                int status = currentHitObjects[key];

                if (status < 0)
                    continue;

                // Animation End
                if(status >= fadeinAnimationSpeed * 4 + idleAnimationSpeed + fadeoutAnimationSpeed * 4)
                {
                    SendByNote(key, 0, 0);
                    SendByNote(key, 1, 0);
                    SendByNote(key, 2, 0);
                    SendByNote(key, 3, 0);

                    currentHitObjects[key] = -1;
                    continue;
                }
                // Animation Fade Out
                else if(status >= fadeinAnimationSpeed * 4 + idleAnimationSpeed && 
                    (status - (fadeinAnimationSpeed * 4 + idleAnimationSpeed)) % fadeoutAnimationSpeed == 0)
                {
                    switch (key)
                    {
                        case 0:
                        case 2:
                        case 5:
                        case 7:
                        case 8:
                        case 10:
                        case 13:
                        case 15:
                            SendByNote(key, 0, fadeColors2[3 - (status - (fadeinAnimationSpeed * 4 + idleAnimationSpeed)) / fadeoutAnimationSpeed]);
                            SendByNote(key, 1, fadeColors2[3 - (status - (fadeinAnimationSpeed * 4 + idleAnimationSpeed)) / fadeoutAnimationSpeed]);
                            SendByNote(key, 2, fadeColors2[3 - (status - (fadeinAnimationSpeed * 4 + idleAnimationSpeed)) / fadeoutAnimationSpeed]);
                            SendByNote(key, 3, fadeColors2[3 - (status - (fadeinAnimationSpeed * 4 + idleAnimationSpeed)) / fadeoutAnimationSpeed]);
                            break;
                        default:
                            SendByNote(key, 0, fadeColors[3 - (status - (fadeinAnimationSpeed * 4 + idleAnimationSpeed)) / fadeoutAnimationSpeed]);
                            SendByNote(key, 1, fadeColors[3 - (status - (fadeinAnimationSpeed * 4 + idleAnimationSpeed)) / fadeoutAnimationSpeed]);
                            SendByNote(key, 2, fadeColors[3 - (status - (fadeinAnimationSpeed * 4 + idleAnimationSpeed)) / fadeoutAnimationSpeed]);
                            SendByNote(key, 3, fadeColors[3 - (status - (fadeinAnimationSpeed * 4 + idleAnimationSpeed)) / fadeoutAnimationSpeed]);
                            break;
                    }
                }
                // Animation Idle
                else if(status >= fadeinAnimationSpeed * 4)
                {

                }
                // Animation Fade in
                else
                {
                    switch(key)
                    {
                        case 0:
                        case 2:
                        case 5:
                        case 7:
                        case 8:
                        case 10:
                        case 13:
                        case 15:
                            SendByNote(key, 0, fadeColors2[status / fadeinAnimationSpeed]);
                            SendByNote(key, 1, fadeColors2[status / fadeinAnimationSpeed]);
                            SendByNote(key, 2, fadeColors2[status / fadeinAnimationSpeed]);
                            SendByNote(key, 3, fadeColors2[status / fadeinAnimationSpeed]);
                            break;
                        default:
                            SendByNote(key, 0, fadeColors[status / fadeinAnimationSpeed]);
                            SendByNote(key, 1, fadeColors[status / fadeinAnimationSpeed]);
                            SendByNote(key, 2, fadeColors[status / fadeinAnimationSpeed]);
                            SendByNote(key, 3, fadeColors[status / fadeinAnimationSpeed]);
                            break;
                    }
                }

                currentHitObjects[key] += 1;
            }
        }

        private Pitch GetPitch(int x, int y)
        {
            return (Pitch)((8 - y) * 10 + x+1);
        }

        private void SendByNote(int note, int value, int color)
        {
            int x, y;

            x = note % 4 * 2;
            if (value == 1 || value == 2)
                x++;

            y = note / 4 * 2;
            if (value == 2 || value == 3)
                y++;

            outputDevice.SendNoteOn(Channel.Channel1, GetPitch(x, y), color);
        }

        private void CleanDevice()
        {
            var pitchList = Enum.GetValues(typeof(Pitch)).Cast<Pitch>();
            foreach(Pitch pitch in pitchList)
            {
                outputDevice.SendNoteOn(Channel.Channel1, pitch, 0);
            }
        }
    }
}
