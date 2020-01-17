using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Diagnostics;
using System.ServiceProcess;

namespace adiary_service
{
	public class main
	{
		private Constants constants = new Constants();

		public main()
		{
			string name = constants.service_name;
			string method;

			if (! require_admin()) return;

			if (search_service(name) == null)
			{
				method = "Install and Start";
				string opt = "create " + name + " start= auto binPath= " + "\"\\\"" + constants.this_exe + "\\\"";
				if (constants.arguments != "") opt += " " + constants.arguments;
				opt += "\"";

				if (sc_command(opt)) {  // regist
					if (constants.description != "") {
						sc_command("description " + name + " " + "\"" + constants.description + "\"");
					}
					start_service(name);    // start
				}
				else {
					method = "[ERROR] Install failed";
				}
			}
			else
			{
				stop_service(name);

				method = "Uninstall";
				if (!sc_command("delete " + name))
					method = "Uninstall failed";
			}

			MessageBox.Show(method + " \"" + name + "\" Service", "Service Wrapper");
		}

		///////////////////////////////////////////////////////////////////////
		// search service
		///////////////////////////////////////////////////////////////////////
		private ServiceController search_service(string name)
		{
			ServiceController[] services = ServiceController.GetServices();

			foreach (ServiceController sc in services)
				if (sc.ServiceName == name) return sc;

			return null;
		}
		///////////////////////////////////////////////////////////////////////
		// start/stop service
		///////////////////////////////////////////////////////////////////////
		private void start_service(string name)
		{
			ServiceController sc = search_service(name);
			if (sc == null) return;
			try
			{
				sc.Start();
			}
			catch (Exception e) {
				;
			}
		}
		private void stop_service(string name)
		{
			ServiceController sc = search_service(name);
			if (sc == null) return;
			try
			{
				sc.Stop();
			}
			catch (Exception e)	{
				;
			}
		}

		///////////////////////////////////////////////////////////////////////
		// Run as admin
		///////////////////////////////////////////////////////////////////////
		private Boolean require_admin()
		{
			System.Security.Principal.WindowsIdentity identity = System.Security.Principal.WindowsIdentity.GetCurrent();
			System.Security.Principal.WindowsPrincipal wp      = new System.Security.Principal.WindowsPrincipal(identity);
			Boolean is_admin = wp.IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator);

			if (is_admin) return true;

			ProcessStartInfo psi = new ProcessStartInfo();
			psi.FileName  = constants.this_exe;
			psi.Arguments = constants.arguments;
			psi.Verb = "runas";

			try
			{
				Process.Start(psi);
			}
			catch (Exception e)	{
				;
			}
			return false;
		}

		///////////////////////////////////////////////////////////////////////
		// Run sc command
		///////////////////////////////////////////////////////////////////////
		private Boolean sc_command(string arg)
		{
			ProcessStartInfo psi = new ProcessStartInfo();
			psi.FileName  = "sc";
			psi.Arguments = arg;
			psi.UseShellExecute = false;
			psi.CreateNoWindow = true;

			Process p = Process.Start(psi);
			p.WaitForExit();

			return (p.ExitCode == 0) ? true : false;
		}
	}
}