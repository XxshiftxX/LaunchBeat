using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Midi;

namespace Launchpad
{
    public class GameManager
    {
        public readonly InputDevice inputDevice;
        public readonly OutputDevice outputDevice;

        public GameManager(InputDevice inputDevice, OutputDevice outputDevice)
        {
            this.inputDevice = inputDevice;
            this.outputDevice = outputDevice;
        }


    }
}
