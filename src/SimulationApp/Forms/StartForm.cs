using System.Diagnostics;

namespace SimulationApp.Forms
{
    public partial class StartForm : Form
    {
        private List<Process> _processList = new List<Process>();
        private Process  _clusterProcess=new Process();
        public StartForm()
        {
            InitializeComponent();
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
            //if(numberOfProcess>5)
            //{
            //    MessageBox.Show("Maximum number of process is 5 ");
            //    return;
            //}



            StartCluster();
            var rand = new Random();
            var processesIds = Enumerable.Range(1, numberOfProcess).OrderBy(x => rand.Next()).ToList();
            foreach(int id in processesIds)
            {
                StartProcess(id);
                Thread.Sleep(200);
            }



        }

        private void StartCluster()
        {
            using(Process process =new Process())
            {
                process.StartInfo.FileName = "ClusterServer.exe";
                process.StartInfo.UseShellExecute = true;
                process.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
                process.Start();
                _processList.Add(process);  
            }
        }

        private void StartProcess(int id)
        {
            if (id <= 0)
                return;
            using (Process cluster = new Process())
            {
                cluster.StartInfo.FileName = "ProcessConsole.exe";
                cluster.StartInfo.UseShellExecute = true;
                cluster.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
                cluster.StartInfo.ArgumentList.Add(id.ToString());
                cluster.Start();
                _clusterProcess=cluster;
            }
        }



        private void StartForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            foreach (var p in _processList)
                p?.Close();

            _clusterProcess?.Close();
        }
    }
}
