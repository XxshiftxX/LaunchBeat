using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Launchpad
{
    class HitObject
    {
        public readonly int Timing;
        public readonly int[] Note;

        public HitObject(int timing, int first, params int[] note)
        {
            Timing = timing;

            var list = note.ToList();
            list.Insert(0, first);
            Note = list.ToArray();
        }
    }
}
