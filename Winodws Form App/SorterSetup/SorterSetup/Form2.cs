using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SorterSetup
{
    public partial class Form2 : Form
    {
        public Form2(Form1 form1)
        {
            InitializeComponent();
        }

        public void loop(Form1 form1)
        {
            while (true)
            {
                this.richTextBoxMonitor.Text = form1.myPort.ReadLine();
            }
        }
    }
}
