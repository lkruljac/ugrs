using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System;
using System.Threading;


namespace SorterSetup
{
    public partial class Form2 : Form
    {

        private Thread _t;
        private ManualResetEvent _eventStop = new ManualResetEvent(false);
        Form1 form1;
        public Form2(Form1 form1local)
        {
            form1 = form1local;
            InitializeComponent();
            for (int i = 0; i < form1.myEnvParm.conteinerNumber; i++)
            {
                this.flowLayoutPanel1.Controls.Add(new Container(i+1));
            }
            _t = new System.Threading.Thread(new ThreadStart(DoSomething));
            _t.Start();
        }

        private void DoSomething()
        {
            loop();
        }

        private void loop()
        {
            string line="";
            while (!_eventStop.WaitOne(1, false))
            {
                line = form1.myPort.ReadLine();
                AppendTextBox(line);
                if (line == "End")
                {
                    //this.Invoke()
                    MessageBox.Show("All objects are sorted");
                    //this.Break.Text = "OK";
                    //this.Break.BackColor = Color.Green;
                    return;
                }
                if (line == "")
                {
                    continue;
                }
                ///MyObject myObj = ToMyObject(line);
                //InsertInTabele(myObj);
            }
        }
        
        public void InsertInTabele(MyObject myObj)
        {
            if (InvokeRequired)
            {
                this.flowLayoutPanel1.Invoke(new Action<MyObject>(InsertInTabele), new object[] { myObj });
                return;
            }
            Container container = (Container) this.flowLayoutPanel1.Controls[myObj.conteiner];
            DataGridViewRow row = (DataGridViewRow) container.dataGridView1.Rows[0].Clone();
            row.Cells[0].Value = container.dataGridView1.RowCount + 1;
            row.Cells[1].Value = myObj.shape;
            row.Cells[2].Value = myObj.color;
            row.Cells[3].Value = myObj.weight.ToString();
            row.Cells[4].Value = myObj.filter.ToString();
            container.dataGridView1.Rows.Add(row);
        }


        public void AppendTextBox(string line)
        {
            if (InvokeRequired)
            {
                this.Invoke(new Action<string>(AppendTextBox), new object[] { line });
                return;
            }
            this.richTextBoxMonitor.Text = line;
        }

        private MyObject ToMyObject(string line)
        {
            MyObject myObj = new MyObject();
            string[] keyWords;
            keyWords = line.Split(';');
            myObj.color = keyWords[0];
            myObj.shape = keyWords[1];
            myObj.weight = float.Parse(keyWords[2]);
            myObj.filter = int.Parse(keyWords[3]);
            myObj.conteiner = int.Parse(keyWords[4]);
            return myObj;
        }

        private void break_Click(object sender, EventArgs e)
        {
            _eventStop.Set();
            this.Close();
            form1.Show();
        }

        private void Form2_Load(object sender, EventArgs e)
        {

        }
    }
}
