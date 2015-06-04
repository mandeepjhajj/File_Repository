/////////////////////////////////////////////////////////////////////////
// MasterTakeAction.cs -This package help the is responsible for merging//
//                      Along with making the file for sending the      //
//                      reply message to cleint or messgage            //
//                      to the server                                  //
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
 *   TakeAction
 *   MakeAndSend
 *   
 * Public Interface
 * ================
 * merger - Call the MasterServerReposiory merger fucntion to merge the tables
 * createClintReply- Creates the reply for the cleint
 * go - send the message and upload the file on channel
 * GenerateXML - generate the XML file
 */
/*
 * Build Process
 * =============
 * Required Files: MasterTakeAction.cs  MasterSeerverrepository.cs
 *   
 * Build command:
 *   csc /target:library /D:TEST_REPOSITORY MasterTakeAction.cs  MasterSeerverrepository.cs
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
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DependencyAnalyzer
{
    public class TakeAction
    {

        static List<string> files = new List<string>();
        static List<string> Projfiles = new List<string>();
        public List<string> getProjects
        {
            get { return files; }
        }
        public List<string> getFls
        {
            get { return Projfiles; }
        }
        
        //----------Calls the function of master repository class which is responsible for merging the table
        public void Merger(List<TempElem> temp)
        {
            bool status;
            MasterServerRepository mst = new MasterServerRepository();
            foreach (TempElem e in temp)
            {
               status=mst.AnalyzeAndMerge(e);
               if(status==false)
               {
                   mst.Merge(e);
               }
            }
        }
        //-------------Test Stub---------------------
#if(TEST_ACTION)
        public static void Main()
        {
            Console.Write("\n  Test TAKEACTION");
            Console.Write("\n =================\n");
            
            List<TempElem> pt = new List<TempElem>();
            TempElem t = new TempElem();
            t.file="abd";
            t.name="merger";
            t.namesp="dependency";
            t.type="function";
            pt.Add(t);
            TakeAction at = new TakeAction();
            at.Merger(pt);
            
        }
#endif
    }
    public class MakeAndSend
    {
        Sender sndr;
        Message msg;
        int mNumber,hash;
        string rep_url,hst_url,client_url;

       
        public MakeAndSend(string reply_url, int msgNbr, string host_url,int hash,string client_url)
        {
            mNumber = msgNbr;
            rep_url = reply_url;
            hst_url = host_url;
            this.hash = hash;
            this.client_url = client_url;
            sndr = new Sender(reply_url);
        }
        
        //--Send the message along with the file on the channel----------
        public void go()
        {
          Message reply = CreateClientReply(rep_url, mNumber,hst_url,hash);
          if(reply.body.text.Contains(".xml"))
                sndr.uploadFile(reply.body.text);
          sndr.SendMessage(reply);
        }
        //-------Creates the client reply------------
        public Message CreateClientReply(string reply_url, int msgNbr, string host_url,int hash)
        {
            msg = new Message();
            MessageBody b2 = new MessageBody();
            MessageHeader h2 = new MessageHeader();
            h2 = MakeHeader(host_url, reply_url, true, false, false, hash, msgNbr);
            string filename = GenerateXml(hash);
            b2.text= filename;
            b2.ClientURL = client_url;
            msg.body = b2;
            msg.header = h2;
            return msg;
        }
        //--------Creates the header for the reply------
        public MessageHeader MakeHeader(string send_url, string reply_url, bool reply, bool quit, bool duplicate, int hash, int msgN)
        {
            MessageHeader header = new MessageHeader();
            header.SenderAddress = send_url;
            header.ReceiverAddress = reply_url;
            header.IsReply = reply;
            header.IsQuit = quit;
            header.IsDulpicate = duplicate;
            header.HashNumber = 10;
            header.MessageNumber = msgN;
            return header;
        }
        //-----Generates the XML file based on the requirement of message-------
        public string GenerateXml(int hash)
        {
            TakeAction act = new TakeAction();
            List<string> projectL = act.getProjects;
            List<string> files = act.getFls;
            char[] seperator = { '\\' };
            switch (hash)
            {
                case 1: XElement Body1 = new XElement("BODY", new XElement("Purpose", "Connection request")); return "connect";
                case 2: return XmlForProject(projectL, seperator);
                case 3: return XmlForType(files);//XmlForFiles(files, seperator); 

                default: Console.Write("\nNo XML generation for this message");  return "default";
            }

        }
        //----Generate the XML containing the projectname------
        private static string XmlForProject(List<string> projectL, char[] seperator)
        {
            try
            {
                XElement Body2 = new XElement("BODY"); XElement project = new XElement("Project");
                foreach (string p in projectL)
                {
                    string a = p.Split(seperator).Last();
                    int index = a.IndexOf('.');
                    a = a.Substring(0, index);
                    XElement pName = new XElement(a);
                    project.Add(pName);
                }
                Body2.Add(project);
                Body2.Save("..\\..\\ToSend\\ProjectList.xml");
            }
            catch (Exception e) { Console.Write("Error encountered {0}", e.Message); }
            return "ProjectList.xml";
        }
        //----Generate the XML containing the files to be analyzed------
        private static string XmlForFiles(List<string> Projfiles, char[] seperator)
        {
            try
            {
                XElement Body2 = new XElement("BODY"); XElement project = new XElement("Files");
                foreach (string p in Projfiles)
                {
                    string a = p.Split(seperator).Last();
                    int index = a.IndexOf('.');
                    a = a.Substring(0, index);
                    XElement pName = new XElement(a);
                    project.Add(pName);
                }
                Body2.Add(project);
                Body2.Save("..\\..\\ToSend\\FileList.xml");
            } catch (Exception e) { Console.Write("Error encountered {0}", e.Message); }
            return "FileList.xml";
        }
        //-----Generate the XML containing the type of type table of all the files-------
        public string XmlForType(List<string> Projfiles)
        {
            List<MElements> fp1 = MasterServerRepository.returnPermanentRep();
            List<string> recvdFile = new List<string>();
            foreach(MElements m in fp1)
            {
                if(!recvdFile.Contains(m.filename))
                         recvdFile.Add(m.filename);
            }
            try
            {
                XElement Body2 = new XElement("BODY"); XElement project = new XElement("Type");
                foreach (string file in recvdFile)
                {
                    XElement f = new XElement("FILE", new XAttribute("Name", file));
                    foreach (MElements e in fp1)
                    {
                        if (file.Equals(e.filename))
                        {
                            XElement a = new XElement("ANALYSIS", new XAttribute("NameSpace", e.namesp),
                            new XAttribute("Type", e.type),
                            new XAttribute("TypeName", e.name),
                            new XAttribute("Begin", e.begin),
                            new XAttribute("End", e.end),
                            new XAttribute("LOC", (e.end - e.begin)));
                            f.Add(a);
                        }
                    }
                    project.Add(f);
                }
                Body2.Add(project);
                Body2.Save("..\\..\\ToSend\\MergedTypeAnalysis.xml");
            } catch (Exception e) { Console.Write("Error encountered {0}", e.Message); }
            return "MergedTypeAnalysis.xml";

        }
    }
}
