using Midi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Launchbeat_Console
{
    class Program
    {
        #region Variables
        private static InputDevice inputDevice;
        private static OutputDevice outputDevice;
        private static Pitch[,] pitches = new Pitch[8,8];
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

            inputDevice.NoteOn += NoteInput;

            Clean();
            inputDevice.StartReceiving(null);
        }

        static void NoteInput(NoteOnMessage msg)
        {
            for(int x = 0; x < 8; x++)
            {
                for(int y = 0; y < 8; y++)
                {

                }
            }
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

        #endregion
    }
}
