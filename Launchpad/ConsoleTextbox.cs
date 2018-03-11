using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Launchpad
{
    class ConsoleTextbox : TextBlock
    {
        public void Write(object s)
        {
            Text += s.ToString();
        }

        public void WriteLine()
        {
            Text += "\n";
        }

        public void WriteLine(object s)
        {
            Text += s.ToString() + "\n";
        }
    }
}
