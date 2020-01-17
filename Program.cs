using System;
using System.Collections.Generic;
using System.ServiceProcess;
using System.Text;
using System.Windows.Forms;

namespace adiary_service
{
	static class Program
	{
		static void Main()
		{
			adiary aService = new adiary();
			if (aService.CheckRunasService()) {
				ServiceBase.Run(aService);
				return;
			}
			// RunAs application
			new main();
		}
	}
}
