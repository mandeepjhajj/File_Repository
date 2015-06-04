/////////////////////////////////////////////////////////////////////////
// CmdP.cs  -  Command line parser                                     //
//             It will parse the command line, given in any order.     //
//                                                                     //
// ver 1.0                                                             //
// Language:    C#, Visual Studio 13.0, .Net Framework 4.5             //
// Platform:    HP Pavilion dv6 , Win 7, SP 1                          //
// Application: Dependency Analyzer                                    //
// Author:      Mandeep Singh, Syracuse University                     //
//              315-751-3413, mjhajj@syr.edu                           //
//                                                                     //
/////////////////////////////////////////////////////////////////////////
/*
 * Module Operations
 * =================
 * This module defines the following class
 *  ParseCmdLine - Parse the arguments
 * 
 * Public Interface
 * ================
 * ParseLine(string[] args)    // Parse the command line
 * seperateArguments(string arg)   // Seperates the arguments
 * main(string[] args)  // for test stub
 * 
 */
/*
 * Build Process
 * =============
 * Required Files:
 *   CmdP.cs
 * 
 * Compiler Command:
 *   csc /target:exe /define:TEST_PARSECMDLINE CmdP.cs
 * 
 * Maintenance History
 * ===================
 * 
 * ver 1.0 : 06 September 2014
 * - First release
 * 
 * Planned Changes:
 * ----------------
 * 
 */
//


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace DependencyAnalyzer
{
    public class ParseCmdLine
    {
        List<string> patterns = new List<string>();
        List<string> options = new List<string>();
        string path;

        /*<-----------It will parse the command line and will call the seperateArgument function---------------->*/

        public void ParseLine(string[] args, ref List<string> pat, ref List<string>  opt, ref string pth)
        {
          
            if (args.Length == 0)
            {
                addPattern("*.sln");
                path= "../";
            }
            else
            {
                foreach (string argument in args)
                {
                    seperateArguments(argument);
                }
                if (patterns.Count == 0)
                {
                    patterns.Add("*.sln");
                }
                if (path == null)
                {
                    path = "../";
                }
            }
            pat = patterns;
            opt = options;
            pth = path;
        }

        /*<----This function will recognize path, option, pattern and will add them to the list------>*/
        public void seperateArguments(string arg)
        {
            try
            {
                if (Directory.Exists(arg))
                {
                    path = arg;
                }
            
               else if (arg[0] == '/')
                {
                    addSwitch(arg);
                }
                else
                {
                    addPattern(arg);
                }
            }
            catch (Exception e)
            {
                Console.Write("Directory does not exists or either you do not have permissions", e.Message);
            }
        }
        public void addSwitch(string option)
        {
            options.Add(option);
        }

        public void addPattern(string pattern)
        {
            patterns.Add(pattern);
        }

#if(TEST_PARSECMDLINE)
        static void Main(string[] args)
        {
            Console.Write("\n  Testing ParseCmdLine Class");
            Console.Write("\n =======================\n");
            List<string> patterns = new List<string>();
            List<string> options = new List<string>();
            string path = null;

            ParseCmdLine cmd = new ParseCmdLine();

            if (args.Length == 0)
            {
                Console.Write("\n  Please enter the command line\n\n");
            }
            cmd.ParseLine(args, ref patterns, ref options, ref path);

            Console.Write("Path is {0}",path);

            Console.Write("\nPatterns are: \n");
            foreach(string pat1 in patterns)
            {
                Console.Write(pat1);
            }

            Console.Write("\nOptions are: \n");
            foreach (string o1 in options)
            {
                Console.Write(o1);
            }
            Console.Write("\n\n");

        }
#endif
    }
}
