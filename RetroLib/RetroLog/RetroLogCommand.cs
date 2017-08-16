using UnityEngine;
using System.Collections;
using System;
using Retro.Log;
/*
public class RetroLogCommand : ConsoleCommand
{
    public RetroLogCommand()
    {
        Name = "RetroLog";
        Description = "Sets log output for logging categories";
        AddAliases(new string[] { "RetroLog", "el", "elog", "edgel" });
    }

    public override string Execute(string name, string[] args)
    {
        if(args.Length == 1) // check for help
        {
            switch(args[0].ToLower())
            {
                case "-h":
                case "-help":
                case "-?":
                case "/h":
                case "/help":
                case "/?":
                    return PrintHelp();
            }
        }
        else if(args.Length == 2) // assume disable
        {
            if(args[0].ToLower() == "disable")
            {
                RetroLog.RetrieveCatagory(args[1]).SetOutputLevel(LogLevel.None);
                return "[RetroLog][System] - Successfully disabled RetroLog category " + args[1] + ".";
            }
        }
        else if(args.Length == 3) // assume enable
        {
            if (args[0].ToLower() == "enable")
            {
                LogLevel level = LogLevel.None;
                switch(args[2].ToLower())
                {
                    case "-v":
                        level = LogLevel.Verbose;
                        break;
                    case "-e":
                        level = LogLevel.Error;
                        break;
                    case "-w":
                        level = LogLevel.Warning;
                        break;
                    case "-d":
                        level = LogLevel.Debug;
                        break;
                    case "-x":
                        level = LogLevel.Exception;
                        break;
                }

                if(level != LogLevel.None)
                {
                    RetroLog.RetrieveCatagory(args[1]).SetOutputLevel(level);
                    return "[RetroLog][System] - Successfully enabled RetroLog category " + args[1] + " for " + level.ToString() + ".";
                }
            }
        }
        
        return "[RetroLog][System] - Invalid arguments!  Run -h for help.";
    }

    private static string PrintHelp()
    {
        return "[RetroLog][System] - Usage : RetroLog [enable/disable] category_name [output level] \n\n\tOutput Levels - Required on Enable only\n\t\t -v\tVerbose\n\t\t -e\tError\n\t\t -w\tWarning\n\t\t -d\tDebug\n\t\t -x\tException\n";
    }

    override public string ToString()
    {
        return PrintHelp();
    }
}
*/