using BullyAlgorithm.Interfaces;
using BullyAlgorithm.Services;
using System;
using System.Collections.Concurrent;
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
        private readonly List<IProcess> _processess;
        public SimulationForm(int numberOfProcesses)
        {
            InitializeComponent();
            _numberOfProcesses = numberOfProcesses;
            _processess = new List<IProcess>();
        }

        private void SimulationForm_Load(object sender, EventArgs e)
        {
            ICommunicator communicator = new Communicator();
            IMessageWritter messageWritter = new ListWritter(this,this._logBox);
            int x = 10;
            int y = 0;
            var rand = new Random();
            var processIDs = Enumerable.Range(1, _numberOfProcesses).OrderBy(i => rand.Next()).ToList();
            
            foreach(int processId in processIDs)
            {
                var process = new Process(processId, communicator, messageWritter);
                var processPanel = new ProcessPanel(new Point(x, y), process);
                _processess.Add(process);
                processPanel.Name.Text = $"Process No [{processId}]";
                x +=130;
                this.Controls.Add(processPanel.Display);
            }
        }

        private void SimulationForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            foreach (var p in _processess)
                p.Dispose();
        }
    }

    public class ListWritter : IMessageWritter
    {
        private readonly Form _form;
        private readonly ListBox _listBox;
        public ListWritter(Form form,ListBox listBox)
        {
            _form = form;
            _listBox = listBox;
        }
        public void Write(string data)
        {
            _form.Invoke((MethodInvoker)(() =>
            {
                _listBox.Items.Add(data + "\n");
                _listBox.SelectedIndex = _listBox.Items.Count - 1;
            }
            ));

        }

    }
}
