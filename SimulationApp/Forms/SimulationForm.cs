using BullyAlgorithm.Interfaces;
using BullyAlgorithm.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SimulationApp.Forms
{
    public partial class SimulationForm : Form
    {
        private readonly int _numberOfProcesses;
        public SimulationForm(int numberOfProcesses)
        {
            InitializeComponent();
            _numberOfProcesses = numberOfProcesses; 
        }

        private void SimulationForm_Load(object sender, EventArgs e)
        {
            ICommunicator communicator = new Communicator();
            IMessageWritter messageWritter = new ConsoleWriter();
            int x = 10;
            int y = 0;
            for(int i=0; i < _numberOfProcesses; i++)
            {
                var process = new Process(i, communicator, messageWritter);
                var processPanel = new ProcessPanel(new Point(x, y), process);
                processPanel.Name.Text = $"Process No [{i}]";
                x +=130;
                this.Controls.Add(processPanel.Display);
            }

        }
    }
}
