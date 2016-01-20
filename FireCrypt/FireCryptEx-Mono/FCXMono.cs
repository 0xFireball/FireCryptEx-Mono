﻿
using System;
using System.Collections.Generic;
using NDesk.Options;

namespace FireCryptEx
{
	class FCXMono
	{
		static int verbosity = 0;
		static string path;
		static int action; //2 create, 1 unlock, 0 lock
		public static void Main(string[] args)
		{
			Console.WriteLine("FireCryptEx - Mono Edition");
			bool show_help = false;
		    List<string> names = new List<string> ();
		
		    var p = new OptionSet () {
		        { "p|path=", "The path to the volume.",
		          v => names.Add (v) },
		        { "c|create", "Create a volume at the specified path.",
		          (v) => action=2 },
		        { "u|unlock", "Unlock a volume.",
		    		v => action=1},
		    	{ "l|lock", "Lock a volume.",
		    		v => action=0},
		    	{ "v|verbose", "Enable debug messages and verbose mode.",
		          v => { if (v != null) ++verbosity; } },
		        { "h|help",  "show this message and exit", 
		          v => show_help = v != null },
		    };
		
		    List<string> extra;
		    try {
		        extra = p.Parse (args);
		    }
		    catch (OptionException e) {
		        Console.Write ("firecryptm: ");
		        Console.WriteLine (e.Message);
		        Console.WriteLine ("Try firecryptm --help' for more information.");
		        return;
		    }
		
		    if (show_help) {
		        ShowHelp (p);
		        return;
		    }
			
		}
		
		static void ShowHelp (OptionSet p)
		{
		    Console.WriteLine ("Usage: firecryptm [OPTIONS]");
		    Console.WriteLine ("A command-line, Mono-compatiable version of FireCryptEx");
		    Console.WriteLine ();
		    Console.WriteLine ("Options:");
		    p.WriteOptionDescriptions (Console.Out);
		}
		
		static void Debug (string format, params object[] args)
		{
		    if (verbosity > 0) {
				var c = Console.ForegroundColor;
				Console.ForegroundColor = ConsoleColor.Yellow;
		        Console.Write ("[DEBUG] ");
		        Console.ForegroundColor = c;
		        Console.WriteLine (format, args);
		    }
		}
	}
}