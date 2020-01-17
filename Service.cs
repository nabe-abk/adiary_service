using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.ServiceProcess;
using System.IO;
using System.Runtime.InteropServices;

namespace adiary_service
{
	public partial class adiary : ServiceBase
	{
		private Constants constants = new Constants();
		private Process proc;
		private Boolean stopping = false;
		private string debug_file;

		public adiary()
		{
			InitializeComponent();

			ServiceName = constants.service_name;
			//debug_file  = "d:\\debug_log.txt";
		}

		public Boolean CheckRunasService()
		{
			if (constants.path == System.Environment.CurrentDirectory) return false;
			return true;
		}

		///////////////////////////////////////////////////////////////////////
		// start service 
		///////////////////////////////////////////////////////////////////////
		protected override void OnStart(string[] args)
		{
			debug("Start service");

			ProcessStartInfo psi = new ProcessStartInfo();
			psi.FileName  = constants.target_exe;
			psi.Arguments = constants.arguments;
			psi.UseShellExecute = false;
			psi.CreateNoWindow  = true;

			if (debug_file != "") {
				psi.FileName = "d:\\adiary\\adiary.exe";
				psi.WorkingDirectory = "d:\\adiary";
			}

			debug("execute file  : " + psi.FileName);
			debug("execute arg   : " + psi.Arguments);

			proc = new Process();
			proc.StartInfo = psi;
			proc.EnableRaisingEvents = true;
			proc.Exited += new EventHandler(process_exited);
			proc.Start();
		}

		///////////////////////////////////////////////////////////////////////
		// stop service 
		///////////////////////////////////////////////////////////////////////
		protected override void OnStop()
		{
			stopping = true;
			debug("Stop service");
			if (proc.HasExited) return;

			try {
				debug("Send CTRL-C");
				send_ctrl_c(proc);
				proc.WaitForExit(1000);
			}
			catch (Exception e)
			{
				;
			}
			if (proc.HasExited) return;

			debug("proc kill()");
			proc.Kill();
			proc.WaitForExit(1000);
		}

		///////////////////////////////////////////////////////////////////////
		// process exit 
		///////////////////////////////////////////////////////////////////////
		private void process_exited(object sender, System.EventArgs e)
		{
			if (stopping) return;
			debug("process_exited()");
			Stop();
		}

		///////////////////////////////////////////////////////////////////////
		// debug log 
		///////////////////////////////////////////////////////////////////////
		private void debug(string msg)
		{
			if (debug_file == "") return;

			string date = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
			File.AppendAllText(debug_file, "[" + this.ServiceName + "] " + date + " " + msg + "\n");
		}

		///////////////////////////////////////////////////////////////////////
		// send CTRL-C 
		///////////////////////////////////////////////////////////////////////
		[DllImport("kernel32.dll", SetLastError = true)]
		static extern bool AttachConsole(uint dwProcessId);

		[DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
		static extern bool FreeConsole();

		[DllImport("kernel32.dll")]
		static extern bool SetConsoleCtrlHandler(ConsoleCtrlDelegate HandlerRoutine, bool Add);

		delegate bool ConsoleCtrlDelegate(CtrlTypes CtrlType);

		// Enumerated type for the control messages sent to the handler routine
		enum CtrlTypes : uint
		{
			CTRL_C_EVENT = 0,
			CTRL_BREAK_EVENT,
			CTRL_CLOSE_EVENT,
			CTRL_LOGOFF_EVENT = 5,
			CTRL_SHUTDOWN_EVENT
		}

		[DllImport("kernel32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool GenerateConsoleCtrlEvent(CtrlTypes dwCtrlEvent, uint dwProcessGroupId);

		public void send_ctrl_c(Process proc)
		{
			//This does not require the console window to be visible.
			if (AttachConsole((uint)proc.Id))
			{
				// Disable Ctrl-C handling for our program
				SetConsoleCtrlHandler(null, true);
				GenerateConsoleCtrlEvent(CtrlTypes.CTRL_C_EVENT, 0);

				//Moved this command up on suggestion from Timothy Jannace (see comments below)
				FreeConsole();

				//Re-enable Ctrl-C handling or any subsequently started
				//programs will inherit the disabled state.
				SetConsoleCtrlHandler(null, false);
			}
		}
	}
}
