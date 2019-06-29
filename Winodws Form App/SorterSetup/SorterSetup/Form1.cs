﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;
using System.IO;

namespace SorterSetup
{
    public partial class Form1 : Form
    {


        //actual COM3
        private int counter = 0;
        private string portName;
        private int baudRate;
        public SerialPort myPort = new SerialPort();
        public SorterEnvironmentParameters myEnvParm = new SorterEnvironmentParameters();
        public string outStr;
        public Form1()
        {
            InitializeComponent();
            SetComboBoxPorts();
            myEnvParm.LoadParm(out myEnvParm);
            //myEnvParm.makeXml();
            flowLayoutPanelFilters.Controls.Add(new Filter(flowLayoutPanelFilters.Controls.Count, myEnvParm));
        }
         /*
        private void button1_Click(object sender, EventArgs e)
        {
            if(myPort.IsOpen == false)
            {
                MessageBox.Show("No connection");
                return;
            }
            if (counter % 2 == 0)
            {
                myPort.WriteLine(textBox1.Text.ToString());
            }
            else
            {
                myPort.WriteLine(textBox1.Text.ToString());
            }
            counter++;
        }
           */
        private void SetComboBoxPorts()
        {

            // Get a list of serial port names.
            string[] ports = SerialPort.GetPortNames();
            // Display each port name to the console.
            comboBoxPorts.Items.Add("Select port");
            comboBoxPorts.SelectedItem = "Select port";
            foreach (string port in ports)
            {
                comboBoxPorts.Items.Add(port);
            }
        }

        private void OpenSerialPort(string portName, int baudRate)
        {
            myPort.BaudRate = baudRate;
            myPort.PortName = portName;
            try
            {
               myPort.Open();
               labelConnectedFlag.Text = "Connected on\nPort: " + myPort.PortName + "\nBaud Rate: " + myPort.BaudRate;


                //MessageBox.Show(this, "Successfully connected to port " + myPort.PortName + " with " + myPort.BaudRate + " baud rate",
                //                   "Successfully connected ", MessageBoxButtons.OK,
                //                   MessageBoxIcon.Information,
                //                   MessageBoxDefaultButton.Button1);

            }
            catch
            {
                DialogResult result;
                result = MessageBox.Show(this, "Error in try to connect on port " + myPort.PortName + " with " + myPort.BaudRate + " baud rate",
                                        "Connection error ", MessageBoxButtons.RetryCancel,
                                        MessageBoxIcon.Error,MessageBoxDefaultButton.Button1);
                labelConnectedFlag.Text = "Not Connected";
                if ( string.Compare(result.ToString(), "Retry") == 0)
                {
                    OpenSerialPort(portName, baudRate);
                }
            }
        }

        private void buttonConnect_Click(object sender, EventArgs e)
        {

            //get baud rate
            try
            {
                Int32.TryParse(textBoxBaudRate.Text.ToString(), out baudRate);
                if (baudRate <= 0 || baudRate >= 11250)
                {
                    MessageBox.Show("Wrong baud rate");
                    return;
                }
            }
            catch
            {
                MessageBox.Show("Wrong baud rate");
            }

            //get port name
            portName = comboBoxPorts.SelectedItem.ToString();
            if(string.Compare(portName, myPort.PortName) != 0 || myPort.BaudRate != baudRate)
            {
                myPort.Close();
            }
            if (string.Compare(portName, "Select port") == 0)
            {
                MessageBox.Show("Wrong port");
                return;
            }

            //connect
            OpenSerialPort(portName, baudRate);
        }

        private void buttonRefreshPorts_Click(object sender, EventArgs e)
        {
            comboBoxPorts.Items.Clear();
            SetComboBoxPorts();
        }

        private void button2Disconnect_Click(object sender, EventArgs e)
        {
            myPort.Close();
            textBoxBaudRate.Text = "";
            comboBoxPorts.SelectedItem = "Select port";
            labelConnectedFlag.Text = "Not Connected";
        }

        private void buttonConnectionTest_Click(object sender, EventArgs e)
        {
            string buffer;
            if (myPort.IsOpen == false)
            {
                MessageBox.Show("No connection");
                return;
            }
            else
            {
                myPort.WriteLine("TestReq");
                buffer = myPort.ReadLine();
                MessageBox.Show(buffer);
                /*
                if (string.Compare(buffer, "Hi, here is sorter") == 0)
                {
                  
                }
                */
            }
        }

        private void buttonAddAnotherFilter_Click(object sender, EventArgs e)
        {
            flowLayoutPanelFilters.Controls.Add(new Filter(flowLayoutPanelFilters.Controls.Count, myEnvParm));
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            UserCriterium output = new UserCriterium();
            FilterData myFilter = new FilterData();
            output.eleseConteiner = (int)this.numericUpDownElseConteiner.Value;
            foreach (Filter filter in flowLayoutPanelFilters.Controls)
            {
                myFilter.Fill(filter);
                //ova linija ispod ne radi, pregazi sve elemente s novim, ne kužim, 
                //bez nje i metoda UserCtiteriumToSTMString(UserCriterium output) pada u vodu
                
                //output.filters.Add(myFilter);

                //obilazim to ovako:
                outStr = outStr + myFilter.colors.Count.ToString() + ",";
                foreach (string color in myFilter.colors)
                {
                    outStr = outStr + color + ",";
                }
                outStr = outStr + myFilter.shapes.Count.ToString() + ",";
                foreach (string shape in myFilter.shapes)
                {
                    outStr = outStr + shape + ",";
                }
                outStr = outStr + myFilter.conteinerNumber.ToString() + ",";
                outStr = outStr + myFilter.minWeight.ToString() + ",";
                outStr = outStr + myFilter.maxWeight.ToString() + "\r\n";
            }

            outStr = outStr + "ElseCont:" + output.eleseConteiner.ToString() + "\r\n";
           

          

            /*
            for ( int i = 0; i< flowLayoutPanelFilters.Controls.Count; i++)
            {
                myFilter.Fill(f(Filter)lowLayoutPanelFilters.Controls[i]);
            }
            */
        
            //XML Verzija
            /*
            System.Xml.Serialization.XmlSerializer x = new System.Xml.Serialization.XmlSerializer(output.GetType());
            using (System.IO.StringWriter textWriter = new StringWriter())
            {
                x.Serialize(textWriter, output);
                outputString = textWriter.ToString();
                try
                {
                    myPort.WriteLine(outputString);
                }
                catch
                {
                    MessageBox.Show("Serial device not connected");
                    return;
                }
            }
            */



            //String verzija
                                  
            using (System.IO.StringWriter textWriter = new StringWriter())
            {

                //ovdje objket "UserCriterium output" pretvoriti u string(vjerojatno
                //neki multiline string, i pohraniti ga u "outputSting 
                //outputString = UserCtiteriumToSTMString(output);
                try
                {
                    //UOCI OVO
                    //###########################
                    //####### VIDI ME ###########
                    //###########################

                    //uncoment line below when using UART
                    //myPort.WriteLine(outStr);

                    //when debuging without STEM use this:
                    MessageBox.Show(outStr);
                }
                catch
                {
                    MessageBox.Show("Serial device not connected");
                    return;
                }
            }
            Form2 form2 = new Form2(this);
            this.Hide();

            form2.Show();
            //form2.loop(this);
        }

        //Metoda koja pretvara poklikano u string za STM
        //Ipak ne koristim, jer otuput.filters lista ne ispadne dobro..
        public string UserCtiteriumToSTMString(UserCriterium output)
        {
            string outStr="";
            foreach(FilterData filter in output.filters)
            {
                outStr = outStr + filter.colors.Count.ToString()+",";
                foreach(string color in filter.colors)
                {
                    outStr = outStr + color + ",";
                }
                outStr = outStr + filter.shapes.Count.ToString() + ",";
                foreach (string shape in filter.shapes)
                {
                    outStr = outStr + shape + ",";
                }
                outStr = outStr + filter.conteinerNumber.ToString() + ",";
                outStr = outStr + filter.minWeight.ToString() + ",";
                outStr = outStr + filter.maxWeight.ToString() + "\r\n";
            }
            outStr = outStr + "ElseCont:" + output.eleseConteiner.ToString() + "\r\n";
            return outStr;
        }
    }
}
