using BullyAlgorithm.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SimulationApp.Forms
{
    internal class ProcessPanel
    {
        public Panel Display;
        public Label Name;
        public Button Run;
        public Button ShutDown;
        private Label Status;
        private readonly IProcess _process;
        public ProcessPanel(Point location,IProcess process)
        {
            Display = new Panel();
            Name = new Label();
            Run = new Button();
            ShutDown = new Button();
            Status = new Label();
            _process = process;

            Display.Location = location;
            Name.Location = new Point(location.X + 50, location.Y + 10);
            Status.Location = new Point(Name.Location.X+15, Name.Location.Y + 75);
            Run.Location = new Point(location.X+10, Name.Location.Y + 160);
            ShutDown.Location = new Point(Run.Location.X + 100, Run.Location.Y);
            
            Run.Text = "Run Process";
            ShutDown.Text = "ShutDown Process";
            Status.Text = "Running Process";

            Name.Width = 150;
            Run.Width = 100;
            Run.Height = 30;
            ShutDown.Width = 150;
            ShutDown.Height = 30;

            Display.Controls.Add(Name);
            Display.Controls.Add(Run);
            Display.Controls.Add(ShutDown);
            Display.Controls.Add(Status);
            Display.BorderStyle = BorderStyle.FixedSingle;
            Display.AutoSize=true;
            _process.Run();

            Run.Click += OnRun;
            ShutDown.Click += OnShutDown;
            Run.Enabled = false;
        }

        private void OnShutDown(object sender,EventArgs args)
        {
            _process.ShutDown();
            Status.Text = "ShutDown";
            Run.Enabled = true;
            ShutDown.Enabled = false;
        }

        private void OnRun(object sender, EventArgs args)
        {

            _process.Run();
            Status.Text = "Running";
            Run.Enabled = false;
            ShutDown.Enabled = true;
        }
    }
}
