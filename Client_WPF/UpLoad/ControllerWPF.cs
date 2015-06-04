/////////////////////////////////////////////////////////////////////////
// ControllerWPF.cs - This package will make a message and send it to  //
//                      the server. This is also responsible to display//
//                      the output to GUI                              //
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
 *   Serverlist
 *   TypeList
 *   WPFController
 *   
 * Public Interface
 * ================
 * OnNewMessageHandler - This will decide action to do with new message
 * listenTask- This will create a receiving channel
 * connectTask - This will create a sending channel
 * MakeMessage - This will make message as header and body
 * Makeheader - create a header for the message
 * makeXML- create a XML file for the message transfer
 * 
 * 
 */
/*
 * Build Process
 * =============
 * Required Files: ControllerWPF.cs, BlockingQueue.cs,IService1.cs,Service1.cs
 *   
 *   
 * Build command:
 *   csc /target:library /D:TEST_CONTROLLER ControllerWPF.cs BlockingQueue.cs IService1.cs Service1.cs
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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace DependencyAnalyzer
{
    //To display the data in WPF with binding
    public class Serverlist
    {
        public string servername { get; set; }
        public string projectname { get; set; }
    }

    //To display the data in WPF with binding
    public class TypeList
    {
        public string namesp1 { get; set; }
        public string type1 { get; set; }
        public string typenm { get; set; }
        public string namesp2 { get; set; }
        public string type2 { get; set; }
        public string filenm { get; set; }
        public string dependnm { get; set; }
    }
     
    //Act as the entry point for the application in WPF
    public class WPFController : Window
    {
        Sender sndr;
        Receiver recvr;
        Thread rcvThrd = null;

        Message rcvdMsg = null;
        delegate void NewMessage(Message msg);
        event NewMessage OnNewMessage;
        private List<string> servers = new List<string>();
        string url, send_url;

        public WPFController()
        {
            OnNewMessage += new NewMessage(OnNewMessageHandler);
        }

        //get message out of receive queue - will block if queue is empty
        void ThreadProc()
        {
            while (true)
            {
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
                // call window functions on UI thread
                this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, OnNewMessage, rcvdMsg);
            }
        }

        //Decide the action which wil be taken after message is received, action is decided with help of hash number in header  
        public void OnNewMessageHandler(Message msg)
        {
            if (msg.body.text.Contains("Reply OK"))
            {            }
            if (!(msg.body.text.Contains("Reply OK")))
            {
                try
                {
                    switch (msg.header.HashNumber)
                    {
                        case 1: ((MainWindow)System.Windows.Application.Current.MainWindow).displayServer.Items.Add(msg.header.SenderAddress); break;
                        case 2: try
                               {
                                XElement fromFile = XElement.Load(@"../../SavedFiles/ProjectList.xml");
                                var query = from c in fromFile.Elements("Project").DescendantNodes()
                                            select c;
                                foreach (var e in query)
                                {
                                    string e1 = e.ToString();
                                    int index = e1.IndexOf("<"); int index1 = e1.IndexOf("/") - 1;
                                    string proc = e1.Substring(index + 1, index1);
                                    ((MainWindow)System.Windows.Application.Current.MainWindow).displayProjects.Items.Add(new Serverlist { servername = msg.header.SenderAddress, projectname = proc });
                                }
                            }
                            catch (Exception e) { Console.Write("Error occured {0}", e.Message); };
                            break;
                        case 3: try
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
                                        string bgn = (string)e2[3];
                                        string ed = (string)e2[4];
                                        string lc = (string)e2[5];
                                        ((MainWindow)System.Windows.Application.Current.MainWindow).displayTypeAnalysis.Items.Add(new TypeList { namesp1 = nmsp, type1 = typ, typenm = tynm, filenm = filenm });
                                    }
                                }
                            }
                            catch (Exception e) { Console.Write("Error occured {0}", e.Message); }
                            break;
                        case 10: DisplayMergedType();
                            break;
                        case 4: XMLMove();
                            DisplayPackageDependency();
                            break;
                        default: break;
                    }
                }
                catch (Exception e) { Console.Write("Error occured {0}", e.Message); }
            }
        }
        //-----Copy the file with different name-----------
        public void XMLMove()
        {
            string source = "..\\..\\SavedFiles\\relationsAnalysis.xml";
            string destination = "..\\..\\SavedFiles\\packageDependency.xml";
            System.IO.File.Copy(source, destination, true);
        }
        //Display the merged type table frmo the file received from the server
        private static void DisplayMergedType()
        {
            try
            {
                XElement merge = XElement.Load(@"../../SavedFiles/relationsAnalysis.xml");
                var query3 = from c in merge.Elements("Relation").Elements()
                             select c;
                foreach (var e in query3)
                {
                    string filenm = e.Attribute("Name").Value;
                    var query4 = from cust in e.Elements("ANALYSIS")
                                 select cust.Attributes().ToList();
                    foreach (var e2 in query4)
                    {
                        string nmsp1 = (string)e2[0];
                        string typ1 = (string)e2[1];
                        string tynm = (string)e2[4];
                        string nmsp2 = (string)e2[2];
                        string typ2 = (string)e2[3];
                        ((MainWindow)System.Windows.Application.Current.MainWindow).displayTypeAnalysis.Items.Add(new TypeList { namesp1 = nmsp1, type1 = typ1, typenm = tynm, namesp2 = nmsp2, type2 = typ2, filenm = filenm });
                    }
               }
            }
            catch (Exception e) { Console.Write("Error occured {0}", e.Message); }
        }
        
        //Display the package dependency
        public void DisplayPackageDependency()
        {
            try
            {
                XElement merge = XElement.Load(@"../../SavedFiles/packageDependency.xml");
                var query3 = from c in merge.Elements("Relation").Elements()
                             select c;

                foreach (var e in query3)
                {
                    string filenm = e.Attribute("Name").Value;
                    var query4 = from cust in e.Elements("ANALYSIS")
                                 select cust.Attributes().ToList();
                    List<string> chk = new List<string>();
                    foreach (var e2 in query4)
                    {
                        string file2 = (string)e2[5];
                        if (!chk.Contains(file2))
                        {
                            ((MainWindow)System.Windows.Application.Current.MainWindow).displayPackage.Items.Add(new TypeList { filenm = filenm, dependnm = file2 });
                            chk.Add(file2);
                        }
                    }
                }
            }
            catch (Exception e) { Console.Write("Error occured {0}", e.Message); }
        }

        //Display the type table of the single server
        //private static void DisplayTypeTable()
        //{
        //    try
        //    {
        //        XElement frmF = XElement.Load(@"../../SavedFiles/TypeAnalysis.xml");
        //        var query1 = from c in frmF.Elements("Type").Elements()
        //                     select c;
        //        foreach (var e in query1)
        //        {
        //            string filenm = e.Attribute("Name").ToString();
        //            var query2 = from cust in e.Elements("ANALYSIS")
        //                         select cust.Attributes().ToList();
        //            foreach (var e2 in query2)
        //            {
        //                string nmsp = (string)e2[0];
        //                string typ = (string)e2[1];
        //                string tynm = (string)e2[2];
        //                string bgn = (string)e2[3];
        //                string ed = (string)e2[4];
        //                string lc = (string)e2[5];
        //                ((MainWindow)System.Windows.Application.Current.MainWindow).displayTypeAnalysis.Items.Add(new TypeList { namesp1 = nmsp, type1 = typ, typenm = tynm, filenm = filenm });
        //            }
        //        }
        //    }
        //    catch (Exception e) { Console.Write("Error occured {0}", e.Message); }
        //}

        //Read the xml file received to display the projects name contained in that server
        //private static void GetProjectList(Message msg)
        //{
        //    try { 
        //    XElement fromFile = XElement.Load(@"../../SavedFiles/ProjectList.xml");
        //    var query = from c in fromFile.Elements("Project").DescendantNodes()
        //                select c;
        //        foreach (var e in query)
        //        {
        //            string e1 = e.ToString();
        //            int index = e1.IndexOf("<"); int index1 = e1.IndexOf("/") - 1;
        //            string proc = e1.Substring(index + 1, index1);
        //            ((MainWindow)System.Windows.Application.Current.MainWindow).displayProjects.Items.Add(new Serverlist { servername = msg.header.SenderAddress, projectname = proc });
        //        }
        //    }
        //    catch (Exception e) { Console.Write("Error occured {0}",e.Message); }
        //}
        
        //Used to create the receiving channel and create a thread that will keep waiting at blocking queue for message
        public void listenTask(string hostvalue, string portvalue)
        {
            send_url = "http://" + hostvalue + ":" + portvalue + "/Dependency";
           try
            {
                recvr = new Receiver();
                recvr.CreateRecvChannel(send_url);
                // create receive thread which calls rcvBlockingQ.deQ() (see ThreadProc above)
                rcvThrd = new Thread(new ThreadStart(this.ThreadProc));
                rcvThrd.IsBackground = true;
                rcvThrd.Start();
            }
            catch (Exception ex)
            {
                Window temp = new Window();
                StringBuilder msg = new StringBuilder(ex.Message);
                msg.Append("\nport = ");
                msg.Append(url.ToString());
                temp.Content = msg.ToString();
                temp.Height = 100;
                temp.Width = 500;
                temp.Show();
            }

        }

        //Creates the sending channel
        public void connectTask(string hostvalue, string portvalue)
        {
            url = "http://" + hostvalue + ":" + portvalue + "/Dependency";

            sndr = new Sender(url);

        }

        //Create the message based on the user selection of buttons
        public void MakeMessage(int a)
        {
            try
            {   Message msg = new Message();
                MessageBody b2 = new MessageBody();
                MessageHeader h2 = new MessageHeader();
                int i = 1;
                switch (a)
                {
                    case 0:
                        msg.header.IsQuit = true;
                        sndr.SendMessage(msg); sndr.Close(); recvr.Close(); break;
                    case 1:
                        h2 = MakeHeader(send_url, url, false, false, false, 1, i);
                        b2.text = "DisplayTheServername";  break;
                    case 2:
                        h2 = MakeHeader(send_url, url, false, false, false, 2, i);
                        b2.text = "GetTheProjectsOfServer"; break;
                    case 3:
                        h2 = MakeHeader(send_url, url, false, false, false, 3, i);
                        b2.text = MakeXML(); break;
                    case 4:
                        h2 = MakeHeader(send_url, url, false, false, false, 4, i);
                        b2.text = "Dependency"; break;
                    default:break;
                }
                i++;
                msg.header = h2;
                msg.body=b2;
                if (msg.body.text.Contains(".xml"))
                    sndr.uploadFile(msg.body.text);
                //if(msg.header.HashNumber==4)
                //     //OnNewMessageHandler(msg);
                //else
                    sndr.SendMessage(msg);
                }
            catch (Exception ex)
            {
                Window temp = new Window();
                temp.Content = ex.Message;
                temp.Height = 100;
                temp.Width = 500;
            }
        }
        //Creates the header for the message
        public MessageHeader MakeHeader(string send_url, string url,bool reply, bool quit, bool duplicate, int hash, int msgN)
        {
            MessageHeader header = new MessageHeader();
            header.SenderAddress = send_url;
            header.ReceiverAddress = url;
            header.IsReply = reply;
            header.IsQuit = quit;
            header.IsDulpicate = duplicate;
            header.HashNumber = hash;
            header.MessageNumber = msgN;
            return header;
        }
        //Creates the XML file for the selection of servername and project name
        public string MakeXML()
        {
            try { 
            List<string> servernm = new List<string>();
            System.Collections.IList items = (System.Collections.IList)((MainWindow)System.Windows.Application.Current.MainWindow).displayProjects.SelectedItems;
            var lits = items.Cast<Serverlist>();
            foreach(Serverlist s in lits)
            {
                if(!servernm.Contains(s.servername))
                    servernm.Add(s.servername);
            }
            XElement Body1 = new XElement("BODY"); XElement project = new XElement("ListProjects");
                foreach (var p in servernm)
                {
                    int index = p.IndexOf(":");
                    string woHttp = p.Substring(index + 18);
                    XNamespace aw = p.Substring(0, 21);
                    XElement sName = new XElement(aw + woHttp);
                    project.Add(sName);
                    foreach (Serverlist s in lits)
                    {
                        if (p.Equals(s.servername))
                        {
                            XElement pname = new XElement(s.projectname.Trim());
                            sName.Add(pname);
                        }
                    }
                }
                Body1.Add(project);
                Body1.Save("..\\..\\ToSend\\RequestForFiles.xml");
             }catch (Exception e) { Console.Write("Error occured {0}", e.Message); }
                       return "RequestForFiles.xml";
        }

        //--------------Test Stub------------------//

#if(TEST_CONTROLLER)
        public static void Main()
        {
            Console.Write("\n  Test Controller");
            Console.Write("\n =================\n");
            WPFController con = new WPFController();
            string host = "localhost";
            string port = "1234";
            string port1 = "4005";
            con.listenTask(host, port);
            con.connectTask(host, port1);
            con.MakeMessage(1);
        }
#endif
    }
}
