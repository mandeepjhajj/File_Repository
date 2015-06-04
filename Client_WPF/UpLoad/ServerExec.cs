
/////////////////////////////////////////////////////////////////////////
// ServerExec.cs - This package is the entry point for the server      //
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
 * getPort - Get the port from the command line
 * getHost - Get the host from the command line
 * OnNewMessageHandler - Handles the new message received from receiver queue
 * recogAction - Take action on the message
 * readMergedXML - Read the merged file received from the master server
 * readXMLFile - read the file received from cleint about the information about the user selection
 * listentask - creates the receiving channel
 */
/*
 * Build Process
 * =============
 * Required Files: Controller.cs Service1.cs IService1.cs BlockingQueue.cs MessageHandler.cs TakeAction.cs FileH.cs
 *   
 * Build command:
 *   csc /target:library /D:TEST_CONTROL Controller.cs Service1.cs IService1.cs BlockingQueue.cs MessageHandler.cs TakeAction.cs File.cs
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
        //----------Entry point for server-----------
        public static void Main(string[] args)
        {
           List<string> patterns = new List<string>();
           List<string> options = new List<string>();
           string path = null;
           ParseCmdLine cmd = new ParseCmdLine();
           cmd.ParseLine(args, ref patterns, ref options, ref path);
           Controller con = new Controller(patterns, options, path);
           string port = con.getPort();
           string host = con.getHost();
           string url = url = "http://" + host + ":" + port + "/Dependency";
           Task.Run(() => { con.listenTask(url); });
           Console.Write("\n Press any key to exit");
           Console.ReadKey(); 
        }
    }
}
