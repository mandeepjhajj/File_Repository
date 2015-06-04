/////////////////////////////////////////////////////////////////////////
// MessageHandler.cs - This package help is the entry for masterserver //
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
 * This module defines the following class:
 *   ServerExec
 *   
 * Public Interface
 * ================
 * main- Entry point for the server
 */
/*
 * Build Process
 * =============
 * Required Files: MasterServerExec.cs
 *   
 * Build command:
 *   csc /target:library MasterServerExec.cs
 *   
 * Maintenance History
 * ===================
 * 
 * ver 1.0 : 21 November 2014
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
using System.Threading;
using System.Threading.Tasks;

namespace DependencyAnalyzer
{
    public class ServerExec
    {
        private List<string> servers = new List<string>();

        public ServerExec()
        {
          
        }
        //---Entry point for the Master Server--------------
        public static void Main(string[] args)
        {

            List<string> patterns = new List<string>();
            List<string> options = new List<string>();
            string path = null;
           
           Controller con = new Controller(patterns, options, path);

           string port = con.getPort();
           string host = con.getHost();
           string url = url = "http://localhost:8000/Dependency";
     
            Task.Run(() => { con.listenTask(url); });
            Console.Write("\n Press any key to exit");
            Console.ReadKey();
            

        }
     }
}
