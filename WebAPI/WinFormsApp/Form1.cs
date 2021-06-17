using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WebSocketSharp;

namespace WinFormsApp
{
    public partial class CloudProject : Form
    {
        public CloudProject()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (WebSocket webSocket = new WebSocket("ws://127.0.0.1:4649/Synchronize"))
            {
                webSocket.Connect();
                bool ping = webSocket.Ping();
                webSocket.Send("1");
            }
        }

        private void CloudProject_SizeChanged(object sender, EventArgs e)
        {
            bool MousePointerNotOnTaskBar = Screen.GetWorkingArea(this).Contains(Cursor.Position);

            if (this.WindowState == FormWindowState.Minimized && MousePointerNotOnTaskBar)
            {
                notifyIcon1.Icon = SystemIcons.Application;
                this.ShowInTaskbar = false;
                notifyIcon1.Visible = true;
                notifyIcon1.Text = "CloudProject";

                // context menu
                notifyIcon1.ContextMenuStrip = new ContextMenuStrip();
                notifyIcon1.ContextMenuStrip.Items.Add("Zakończ", null, this.menuExit_Click);
            }       
        }
        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.WindowState = FormWindowState.Normal;
            if(this.WindowState == FormWindowState.Normal)
            {
                this.ShowInTaskbar = true;
                notifyIcon1.Visible = false;
            }
        }
        private void menuExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
