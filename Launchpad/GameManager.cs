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
        private readonly InputDevice inputDevice;
        private readonly OutputDevice outputDevice;

        public GameManager(InputDevice inputDevice, OutputDevice outputDevice)
        {
            this.inputDevice = inputDevice;
            this.outputDevice = outputDevice;
        }


    }
}
