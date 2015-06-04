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
using System.Xml.Linq;

namespace DependencyAnalyzer
{
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

        //seperate the port number from the command line and return it to the caller 
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
        //seperate the host from the command line and return it to the caller 
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
        // handles the action on the new received message and call the make message function 
        void OnNewMessageHandler(Message msg)
        {
            Console.Write("\n Received message\n");

            Messagehandler hld = new Messagehandler();
            Controller cl = new Controller(pattern, option, pth);

            Task t = Task.Run(() => { int i = hld.RecogMessage(msg); return i; })
                .ContinueWith(antecedant =>
                {
                    cl.recogAction(antecedant.Result);
                });

            t.Wait();
            string reply_url = recvr.IdentifyClient(msg);
            int msgNumber = msg.header.MessageNumber;
            string host_url = msg.header.ReceiverAddress;
            int hash = msg.header.HashNumber;
            if ((msg.header.HashNumber == 3) && (msg.header.SenderAddress!="http://localhost:8000/Dependency"))
            {
                string master_url = "http://localhost:8000/Dependency";
                MakeAndSend mks = new MakeAndSend(master_url, msgNumber, host_url, hash, reply_url);
                mks.go();
            }
            else
            {
                if (msg.body.ClientURL != null)
                {
                    reply_url = msg.body.ClientURL;
                }
                MakeAndSend mks2 = new MakeAndSend(reply_url, msgNumber, host_url, hash);
                mks2.go();
                
            }
          }
      
        // It decides the action that has to be taken on the message based on unqie message hash number
        public void recogAction(int control)
        {
            TakeAction act = new TakeAction();
            switch (control)
            {
                case 1:break;
                case 2: ExtractProjectect(act);
                        break;
                case 3: Analyse(act);
                        break;
                case 4: break;
                case 10: Dependency(act);
                        break;
                case 11:break;

            }
        }
        //---------get the relationships based on merged table-----------
        private void Dependency(TakeAction act)
        {
            Console.Write("\nMergred file received\n");
            List<Elements> temp = readMergedFile();
            Permanemt pr = new Permanemt();
            pr.clear();
            pr.MakeCopyOfList(temp);
            List<string> file = act.getFls;
            List<string> fileOp = new List<string>();
            fileOp.Add("/S");
            Task<List<string>> t5 = Task.Run(() =>
            {
                Analyzer anal = new Analyzer();
                anal.getRelation(file, fileOp);
                return file;
            });
            t5.Wait();
        }
        //------------Find the realtion-------------
        private void Relatn(TakeAction act)
        {
            List<string> fileToAn = new List<string>();
            List<string> filePatt = new List<string>();
            filePatt.Add("*.cs");
            List<string> fileOpt = new List<string>();
            fileOpt.Add("/S");
            List<string> pthL = new List<string>();
            readXMLFile(ref pthL);
            Task<List<string>> t4 = Task.Run(() =>
            {
                fileToAn = act.GetFiles(fileOpt, filePatt, pthL);
                Analyzer anal = new Analyzer();
                anal.getRelation(fileToAn, fileOpt);
                return fileToAn;
            });
            t4.Wait();
        }
        //---------Make the intial type table------------
        private void Analyse(TakeAction act)
        {
            RepositoryPermanemt pr1 = new RepositoryPermanemt();
            pr1.clear();                                       
            act.clear();
            List<string> fileToAnal = new List<string>();
            List<string> filePattern = new List<string>();
            filePattern.Add("*.cs");
            List<string> fileOption = new List<string>();
            fileOption.Add("/S");
            List<string> pthList = new List<string>();
            readXMLFile(ref pthList);
            Task<List<string>> t3 = Task.Run(() =>
            {
                fileToAnal = act.GetFiles(fileOption, filePattern, pthList);
                Analyzer anal = new Analyzer();
                anal.getFile(fileToAnal, fileOption);
                return fileToAnal;
            });
            t3.Wait();
        }
        //------Extracts the project list and send it to client---------------
        private void ExtractProjectect(TakeAction act)
        {
            act.clear();
            Task t2 = Task.Run(() => { act.GetProjects(option, pattern, pth); });
            t2.Wait();
        }

        // Read the type table information from the merged file received from the master server and save it to list
        public List<Elements> readMergedFile()
        {
            List<Elements> temp = new List<Elements>();
            try
            {
                XElement frmF = XElement.Load(@"../../SavedFiles/MergedTypeAnalysis.xml");
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
                        Elements newE = new Elements();
                        newE.namesp = nmsp;
                        newE.type = typ;
                        newE.name = tynm;
                        newE.begin = bgn;
                        newE.end = ed;
                        newE.complex = lc;
                        newE.file = filenm;
                        temp.Add(newE);
                    }
                }
            } catch (Exception e) { Console.Write("Error encountered {0}", e.Message); }
            return temp;
         }

        // Read the XML file received from the cleint which contains the information about the user selection from the list view
        public void readXMLFile(ref List<string> pth)
        {
            try
            {
                char[] seperator = { ' ' };
                XElement fromFile = XElement.Load(@"../../SavedFiles/RequestForFiles.xml");
                var xElem = fromFile.Element("ListProjects");
                foreach (var child in xElem.Elements())
                {
                    Console.WriteLine(child.Name.LocalName);
                    foreach (var childElement in child.Elements())
                    {
                        pth.Add(childElement.Name.LocalName);
                    }
                }
            }catch (Exception e) { Console.Write("Error encountered {0}", e.Message); }
        }

        // Creates the receiving channel and start the thread which waits at blocking queue for messages
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


        //---------------Test Stub---------------------
#if(TEST_CONTROL)
        public static void Main()
        {
            Console.Write("\n  Test Controller");
            Console.Write("\n =================\n");
            string add = "http://localhost:4005/Dependency";
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
