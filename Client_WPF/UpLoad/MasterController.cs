/////////////////////////////////////////////////////////////////////////
// Controller.cs - This package is used for making a queue thread safe //
//                 It does so by creating lock.                        //
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
 *   Controller
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
 * Required Files: MasterController.cs Service1.cs IService1.cs BlockingQueue.cs MasterMessageHandler.cs MasterTakeAction.cs 
 *   
 * Build command:
 *   csc /target:library /D:TEST_CONTROL MasterController.cs Service1.cs IService1.cs BlockingQueue.cs MasterMessageHandler.cs MasterTakeAction.cs 
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
using System.Xml.Linq;

namespace DependencyAnalyzer
{
    //Hold the information retrived from the file
    public class TempElem
    {
        public string file { get; set; }
        public string servnm { get; set; }
        public string namesp { get; set; }
        public string type { get; set; }
        public string name { get; set; }
        public int begin { get; set; }
        public int end { get; set; }
        public int complex { get; set; }
    }
    public class Controller
    {
        Receiver recvr;
        Sender sndr;
        Message rcvdMsg = null;
        Thread rcvThrd = null;
        delegate void NewMessage(Message msg);
        event NewMessage OnNewMessage;

        List<string> pattern = new List<string>();
        List<string> option = new List<string>();
        string pth = null;
        public Controller(List<string> pat, List<string> opt, string path)
        {
            pattern = pat;
            option = opt;
            pth = path;

            OnNewMessage += new NewMessage(OnNewMessageHandler);
        }

        //----Extracts the port------------
        public string getPort()
        {
            char[] seperator = { 'p', 'P' };
            string p = null;
            foreach (string s in option)
            {
                if (s.Contains("/p") || s.Contains("/P"))
                {
                    p = s.Split(seperator).Last();
                }
            }
            return p;
        }

        //---Extracts the host-------
        public string getHost()
        {
            char[] seperator = { 'd', 'D' };
            string h = null;
            foreach (string s in option)
            {
                if (s.Contains("/d") || s.Contains("/D"))
                {
                    h = s.Split(seperator).Last();
                }
            }
            return h;
        }
        //----Receives the message and them check whether a XML file is transfered or not. It waits until file is transfered
        void ThreadProc()
        {
            while (true)
            {
                // get message out of receive queue - will block if queue is empty
                rcvdMsg = recvr.ReceiveMessage();

                if (rcvdMsg.body.text.Contains(".xml"))
                {
                    while (true)
                    {
                        if (Receiver.status.ContainsKey(rcvdMsg.body.text))
                        {
                            if (Receiver.status[rcvdMsg.body.text] == 1)
                                break;
                        }
                    }
                }
                this.OnNewMessage(rcvdMsg);
            }
        }
        //---Decide the action associated with new message-------
        void OnNewMessageHandler(Message msg)
        {
            Console.Write("\n Received message\n");

            Messagehandler hld = new Messagehandler();
            Controller cl = new Controller(pattern, option, pth);
             
            Task t = Task.Run(() => { int i = hld.RecogMessage(msg); return i; })
                .ContinueWith(antecedant =>
                {
                    cl.recogAction(antecedant.Result,msg.header.SenderAddress);
                });

            t.Wait();
            string reply_url = recvr.IdentifyClient(msg);
            int msgNumber = msg.header.MessageNumber;
            string host_url = msg.header.ReceiverAddress;
            
            int hash = msg.header.HashNumber;
            MakeAndSend mks = new MakeAndSend(reply_url, msgNumber, host_url, hash,msg.body.ClientURL);
            mks.go();

        }
        
        //--------Defines the action associated with the message that is just received------------
        public void recogAction(int control, string servnm)
        {
            TakeAction act = new TakeAction();
            switch (control)
            {
                case 3: List<TempElem> temp = ReadAndStoreXML(servnm);
                    Task t3 = Task.Run(() =>
                    { act.Merger(temp); });
                    t3.Wait();
                    break;
                default: Console.Write("\n No action specified");break;

            }
        }

        //--------Read the information associated with the type table of sent by the server------------
        public List<TempElem> ReadAndStoreXML(string servernm)
        {
            List<TempElem> temp = new List<TempElem>();
            try
            {
                XElement frmF = XElement.Load(@"../../SavedFiles/TypeAnalysis.xml");
                var query1 = from c in frmF.Elements("Type").Elements()
                             select c;
                foreach (var e in query1)
                {
                    string filenm = e.Attribute("Name").Value;
                    var query2 = from cust in e.Elements("ANALYSIS")
                                 select cust.Attributes().ToList();
                    foreach (var e2 in query2)
                    {
                        string nmsp = (string)e2[0];
                        string typ = (string)e2[1];
                        string tynm = (string)e2[2];
                        int bgn = (int)e2[3];
                        int ed = (int)e2[4];
                        int lc = (int)e2[5];
                        TempElem newE = new TempElem();
                        newE.namesp = nmsp;
                        newE.type = typ;
                        newE.name = tynm;
                        newE.begin = bgn;
                        newE.end = ed;
                        newE.complex = lc;
                        newE.servnm = servernm;
                        newE.file = filenm;
                        temp.Add(newE);
                    }
                }
            }  catch (Exception e) { Console.Write("Erroe occured {0}",e.Message); }
            return temp;
        }

        //-------Creates the receiving channel-----------
        public void listenTask(string url)
        {

            try
            {
                recvr = new Receiver();
                recvr.CreateRecvChannel(url);
                Console.Write("\n=============Server started=========\n");

                rcvThrd = new Thread(new ThreadStart(this.ThreadProc));
                rcvThrd.IsBackground = true;
                rcvThrd.Start();

            }
            catch (Exception ex)
            {
                Console.Write("\n Exception occured while listening {0}\n", ex);
            }
        }

#if(TEST_CONTROL)
        public static void Main()
        {
            Console.Write("\n  Test Controller");
            Console.Write("\n =================\n");
            string add = "http://localhost:8000/Dependency";
            List<string> pattern = new List<string>();
            List<string> option = new List<string>();

            string pth = null;
            Controller con = new Controller(pattern,option,pth);
            con.listenTask(add);
            Message m = new Message();
            m.header.HashNumber = 1;
            m.body.text = "dummy message";
            con.OnNewMessage(m);
        }
#endif
    }

}
