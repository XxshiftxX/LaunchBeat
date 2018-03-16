using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Launchbeat_Console
{
    public struct HitObject
    {
        public static int BPM;
        public static Queue<HitObject> HitObjects = new Queue<HitObject>();
        public readonly int[] Key;
        public readonly double Time;

        public HitObject(int[] key, double time)
        {
            Key = key;
            Time = time;
        }
    }
}
