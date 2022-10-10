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
    public partial class StartForm : Form
    {
        public StartForm()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void _strartSimulateBtn_Click(object sender, EventArgs e)
        {
            int numberOfProcess=-1;
            bool ok = int.TryParse(_noOfProcess.Text, out numberOfProcess);
            if(!ok)
            {
                MessageBox.Show("Enter valid number");
                return;
            }
            if(numberOfProcess>7)
            {
                MessageBox.Show("Maximum number of process is 7 ");
                return;
            }

            var simulationForm=new SimulationForm(numberOfProcess);

            simulationForm.Show();



        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
    }
}
