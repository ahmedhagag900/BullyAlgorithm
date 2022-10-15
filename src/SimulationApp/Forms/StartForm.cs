using System.Diagnostics;

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



            StartCluster();



        }

        private void StartCluster()
        {
            using(Process cluster =new Process())
            {
                cluster.StartInfo.FileName = "ClusterServer.exe";
                cluster.StartInfo.UseShellExecute = true;
                cluster.StartInfo.WindowStyle = ProcessWindowStyle.Maximized;
                cluster.Start();
            }
        }


        private void label2_Click(object sender, EventArgs e)
        {

        }
    }
}
