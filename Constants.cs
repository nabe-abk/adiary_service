using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace adiary_service
{
    class Constants
	{
		public readonly string this_exe;
		public readonly string target_exe;
		public readonly string path;
		public readonly string arguments;

		public readonly string service_name;
		public readonly string description;

		public Constants()
        {
			this_exe     = Application.ExecutablePath;
			path         = Regex.Replace(this_exe,   "\\\\[^\\\\]+$", "");
			target_exe   = Regex.Replace(this_exe,   "_service\\.exe$", ".exe",       RegexOptions.IgnoreCase);
			service_name = Regex.Replace(target_exe, ".*\\\\([^\\\\]+)\\.exe$", "$1", RegexOptions.IgnoreCase);
			service_name = Regex.Replace(service_name, "\\W", "");
			arguments    = Regex.Replace(Environment.CommandLine, "^(\"[^\"]*\"|[^ ]*) +", "");
			arguments    = Regex.Replace(arguments, "\"", "\\\"");

			if (this_exe == target_exe) 	target_exe = System.IO.Path.Combine(path, "\adiary.exe");
			if (service_name == target_exe) service_name = "adiary";

			if (service_name == "adiary")   description = "High Performance CMS Deamon";

		}
	}
}
