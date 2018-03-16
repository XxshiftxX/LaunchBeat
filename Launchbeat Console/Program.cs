using Midi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Launchbeat_Console
{
    class Program
    {
        #region Variables
        private static bool isRendering;
        private static Stopwatch stopwatch = new Stopwatch();
        private static Thread renderer = new Thread(Draw);
        private static Thread processor = new Thread(Calculate);

        private static InputDevice inputDevice;
        private static OutputDevice outputDevice;
        private static Pitch[,] pitches = new Pitch[8, 8];
        private static int[,] isPanelPressed = new int[4, 4] { { 0, 0, 0, 0}, { 0, 0, 0, 0 }, { 0, 0, 0, 0 }, { 0, 0, 0, 0 } };
        private static int[,] panelState = new int[4, 4] { { -1, -1, -1, -1 }, { -1, -1, -1, -1 }, { -1, -1, -1, -1 }, { -1, -1, -1, -1 } };
        private static int[,] preRenderer = new int[8, 8] 
        { 
            { 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0 }, 
            { 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0 }, 
            { 0, 0, 0, 0, 0, 0, 0, 0 }, 
            { 0, 0, 0, 0, 0, 0, 0, 0 }, 
            { 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0 }
        };
        private static Dictionary<Pitch, bool> isNotePressed = new Dictionary<Pitch, bool>();
        private static Queue<int> renderQueue = new Queue<int>();

        private static HitObject? waitingHitObject = null;
        #endregion

        #region Functions
        static void Main(string[] args)
        {
            Initialize();
            Console.ReadKey();
        }

        static void Initialize()
        {
            inputDevice = InputDevice.InstalledDevices[0];
            outputDevice = OutputDevice.InstalledDevices[1];

            inputDevice.Open();
            outputDevice.Open();

            for(int x = 0; x < 8; x++)
                for(int y = 0; y < 8; y++)
                    pitches[x, y] = (Pitch)((8 - y) * 10 + x + 1);

            for (int x = 0; x < 8; x++)
                for (int y = 0; y < 8; y++)
                    isNotePressed[pitches[x, y]] = false;

            inputDevice.NoteOn += NoteInput;

            analyzer a = new analyzer();
            a.readBar(@"D:\Test\Maps\map.txt");
            a = null;

            Clean();
            inputDevice.StartReceiving(null);
            isRendering = true;
            renderer.Start();

            stopwatch.Start();
            processor.Start();
        }

        static void NoteInput(NoteOnMessage msg)
        {
            int noteX = -1, noteY = -1;

            for (int x = 0; x < 8; x++) for (int y = 0; y < 8; y++)
                if (pitches[x, y] == msg.Pitch)
                {
                    noteX = x;
                    noteY = y;
                }

            if (noteX == -1 || noteY == -1)
                return;

            int panelX = noteX / 2;
            int panelY = noteY / 2;
            int panelNum = panelX + panelY * 4;

            isNotePressed[msg.Pitch] = !isNotePressed[msg.Pitch];
            bool isDown = isNotePressed[msg.Pitch];

            if (isNotePressed[msg.Pitch])
                isPanelPressed[panelX, panelY]++;
            else
                isPanelPressed[panelX, panelY]--;

            if(isPanelPressed[panelX, panelY] == 0 && !isDown)
            {
                Console.WriteLine($"PANEL - \t({panelX}, {panelY}) up");
            }
            else if (isPanelPressed[panelX, panelY] == 1 && isDown)
            {
                Console.WriteLine($"PANEL - \t({panelX}, {panelY}) down");
            }
            //Console.WriteLine($"VALUE - \t({panelX}, {panelY}) {isPanelPressed[panelX, panelY]}");
            //Console.WriteLine($"NOTE  - \t({noteX}, {noteY}) {(isNotePressed[msg.Pitch] ? "down" : "up")}");
        }

        static void NoteOn(int x, int y, int velocity) => outputDevice.SendNoteOn(Channel.Channel1, pitches[x,y], velocity);
        static void PanelOn(int x, int y, int status, int velocity)
        {
            int realx = x * 2 + ((status == 0 || status == 3) ? 0 : 1);
            int realy = y * 2 + ((status == 0 || status == 1) ? 0 : 1);
            NoteOn(realx, realy, velocity);
        }
        static void Clean()
        {
            foreach(Pitch p in Enum.GetValues(typeof(Pitch)))
            {
                outputDevice.SendNoteOn(Channel.Channel1, p, 0);
            }
        }

        static void Draw()
        {
            while (isRendering)
            {
                while (renderQueue.Count > 0)
                {
                    int renderPos = renderQueue.Dequeue();
                    NoteOn(renderPos % 8, renderPos / 8, preRenderer[renderPos % 8, renderPos / 8]);
                }
            }
        }
        static void Calculate()
        {
            while (isRendering)
            {
                if (!waitingHitObject.HasValue && HitObject.HitObjects.Count > 0)
                    waitingHitObject = HitObject.HitObjects.Dequeue();
                
                if (stopwatch.ElapsedMilliseconds >= waitingHitObject.Value.Time)
                {
                    foreach (int i in waitingHitObject.Value.Key)
                    {
                        Console.Write($"{i} ");
                    }
                    Console.WriteLine();
                    waitingHitObject = null;
                }
            }
        }
        #endregion
    }
}
