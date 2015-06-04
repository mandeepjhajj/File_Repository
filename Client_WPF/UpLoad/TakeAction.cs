/////////////////////////////////////////////////////////////////////////
// TakeAction.cs - This package help the is responsible for fetching   //
//                  files and project list. Along with making the file //
//                  for sending the reply message to cleint or messgage//
//                  to the server                                      //
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
 * recogmessage - This returns the unique value which help the controller to decide the course of action associated with message
 */
/*
 * Build Process
 * =============
 * Required Files: TakeAction.cs FileH.cs
 *   
 * Build command:
 *   csc /target:library /D:TEST_ACTION TakeAction.cs FileH.cs
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
        static List<string> permfiles = new List<string>();
        public List<string> getPermFil
        {
            get { return permfiles; }
        }
        public List<string> getProjects
        {
            get { return files; }
        }
        public List<string> getFls
        {
            get { return Projfiles; }
        }
        //--------get the project list from server directory-------
        public List<string> GetProjects(List<string> option, List<string> pattern, string path)
        {
            CFileMgr fm = new CFileMgr();
            fm.SearchFiles(path, pattern, option, ref files);
            foreach (string f in files)
            {
                permfiles.Add(f);
            }
            return files;
        }
        //------Find the files for the matching user selection---------
        public List<string> GetFiles(List<string> option, List<string> pattern, List<string> path)
        {
            int index = 0;
            string pth = null;
            List<string> newPath = new List<string>();
            foreach (string s in path)
            {
                newPath.Add(string.Concat(s, ".sln"));
            }
            foreach (string p in newPath)
            {
                foreach (string file in permfiles)
                {
                    if (file.Contains(p))
                    {
                        index = file.IndexOf(p);
                        pth = file.Substring(0, index - 1);
                        break;
                    }
                }
                CFileMgr fm = new CFileMgr();
                fm.SearchFiles(pth, pattern, option, ref Projfiles);
                fm.FilterTemoraryFile(ref Projfiles);
            }
            return Projfiles;
        }
        public void clear()
        {
            files.Clear();
            Projfiles.Clear();
        }

        //-------------Test Stub---------------------
#if(TEST_ACTION)
        public static void Main()
        {
            Console.Write("\n  Test TAKEACTION");
            Console.Write("\n =================\n");
            string path = ".";
            List<string> patrn = new List<string>();
            List<string> optn = new List<string>();
            List<string> pt = new List<string>();
            TakeAction t = new TakeAction();
            patrn.Add("*.sln");
            t.GetProjects(optn,patrn,path);
            patrn.Remove("*.sln");
            patrn.Add("*.cs");
            t.GetFiles(optn,patrn,pt);
            List<string> projL = t.getProjects;
            List<string> fil = t.getFls;
            foreach(string s in projL)
            {
                Console.Write(s);
            }
        }
#endif
    }
    public class MakeAndSend
    {
        Sender sndr;
        Message msg;
        int mNumber,hash;
        string rep_url,hst_url,client_url;
        RepositoryPermanemt pr = new RepositoryPermanemt();
        Permanemt fp = new Permanemt();
        public MakeAndSend(string reply_url, int msgNbr, string host_url,int hash)
        {
            mNumber = msgNbr;
            rep_url = reply_url;
            hst_url = host_url;
            this.hash = hash;
            sndr = new Sender(reply_url);
        }
        public MakeAndSend(string reply_url, int msgNbr, string host_url, int hash, string client_url)
        {
            mNumber = msgNbr;
            rep_url = reply_url;
            hst_url = host_url;
            this.hash = hash;
            this.client_url= client_url;
            sndr = new Sender(reply_url);
        }
        //-----------Sends the message to the receiver----------------
        public void go()
        {
          Message reply = CreateClientReply(rep_url, mNumber,hst_url,hash);
            
          if(reply.body.text.Contains(".xml"))
                sndr.uploadFile(reply.body.text);
          sndr.SendMessage(reply);
        }
        public Message CreateClientReply(string reply_url, int msgNbr, string host_url,int hash)
        {
            msg = new Message();
            MessageBody b2 = new MessageBody();
            MessageHeader h2 = new MessageHeader();
            h2 = MakeHeader(host_url, reply_url, true, false, false, hash, msgNbr);
            string filename = GenerateXml(h2.HashNumber);
            b2.text= filename;
            b2.ClientURL = client_url;
            msg.body = b2;
            msg.header = h2;
            return msg;
        }
        public MessageHeader MakeHeader(string send_url, string reply_url, bool reply, bool quit, bool duplicate, int hash, int msgN)
        {
            MessageHeader header = new MessageHeader();
            header.SenderAddress = send_url;
            header.ReceiverAddress = reply_url;
            header.IsReply = reply;
            header.IsQuit = quit;
            header.IsDulpicate = duplicate;
            header.HashNumber = hash;
            header.MessageNumber = msgN;
            return header;
        }
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
                case 3: return XmlForType(files);
                case 10: return XMLForRelation(files);
                default: Console.Write("\nNo XML generation for this message");  return "default";
            }
         }
        //---Copies the file from Saved to ToSend folder on server
        public string XMLMove()
        {
            string source = "..\\..\\SavedFiles\\MergedTypeAnalysis.xml";
            string destination = "..\\..\\ToSend\\MergedTypeAnalysis.xml";
            System.IO.File.Copy(source, destination, true);
            return "MergedTypeAnalysis.xml";
        }
        //----Generate the XML file for the list of project-----------
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
            }  catch (Exception e) { Console.Write("Error encountered {0}", e.Message); }
            return "ProjectList.xml";
        }
        //-------Generate the XML file for list of files-----
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
        //-----Generate the XML file for Type analysis-------
        public string XmlForType(List<string> Projfiles)
        {
            List<Elements> fp1 = pr.returnPermanentRep();
            try
            {
                XElement Body2 = new XElement("BODY"); XElement project = new XElement("Type");
                foreach (string file in Projfiles)
                {
                    XElement f = new XElement("FILE", new XAttribute("Name", file));
                    foreach (Elements e in fp1)
                    {
                        if (file.Equals(e.file))
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
                Body2.Save("..\\..\\ToSend\\TypeAnalysis.xml");
            } catch (Exception e) { Console.Write("Error encountered {0}", e.Message); }
            return "TypeAnalysis.xml";

        }
        public string XMLForRelation(List<string> Projfiles)
        {
            List<Elements> fp1 = fp.returnRep();
            List<Relation> sp3 = storeRel.returnRelation();
            try
            {
                for (int i = 0; i < sp3.Count; i++)
                {
                    if (sp3.ElementAt(i).Dependingfile == null)
                        sp3.ElementAt(i).Dependingfile = "Defined in Using Statements, not caputed";
                }
            }
            catch (Exception e) { Console.Write("error occured {0}", e.Message); }
            try
            {
                XElement Body2 = new XElement("BODY"); XElement project = new XElement("Relation");
                foreach (string file in Projfiles)
                {
                    XElement f = new XElement("FILE", new XAttribute("Name", file));
                    foreach (Relation e in sp3)
                    {
                        if (file.Equals(e.file) && e.Dependingfile != null && e.namespace1 != null && e.namespace2 != null)
                        {
                            XElement a = new XElement("ANALYSIS", new XAttribute("NameSpace1", e.namespace1),
                            new XAttribute("Type1", e.type1),
                            new XAttribute("NameSpace2", e.namespace2),
                            new XAttribute("Tpe2", e.type2),
                            new XAttribute("Relation", e.name),
                            new XAttribute("DependingFile", e.Dependingfile));
                            f.Add(a);
                        }
                    }
                    project.Add(f);
                }
                Body2.Add(project);
                Body2.Save("..\\..\\ToSend\\relationsAnalysis.xml");
            } 
            catch (Exception e) { Console.Write("Error encountered {0}", e.Message); }
            return "relationsAnalysis.xml";
        }
        public string XmlForDependency(List<string> Projfiles)
        {
            List<Elements> fp1 = pr.returnPermanentRep();
            try
            {
                XElement Body2 = new XElement("BODY"); XElement project = new XElement("Type");
                foreach (string file in Projfiles)
                {
                    XElement f = new XElement("FILE", new XAttribute("Name", file));
                    foreach (Elements e in fp1)
                    {
                        if (file.Equals(e.file))
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
                Body2.Save("..\\..\\ToSend\\DependencyAnalysis.xml");
            } catch (Exception e) { Console.Write("Error encountered {0}", e.Message); }
            return "DependencyAnalysis.xml";
        }

    }
    public class Permanemt
    {

        static List<Elements> Permanent = new List<Elements>();
        static Permanemt instance;

        ScopeStack<Elements> stack_ = new ScopeStack<Elements>();
        static List<Elements> locations_ = new List<Elements>();
        
        /* This function will copy all the elements from local location to a permanent location for each file*/
        public Permanemt()
        {
            instance = this;
        }
        public void MakeCopyOfList(List<Elements> last)
        {
            
            foreach (Elements e in last)
            {
                Elements elements = new Elements();
                elements.file = e.file;
                elements.type = e.type;
                elements.name = e.name;
                elements.begin = e.begin;
                elements.end = e.end;
                elements.complex = e.complex;
                elements.namesp = e.namesp;
                Permanent.Add(elements);
            }
        }

        /*<-- It will return the permanent repository>*/
        public List<Elements> returnRep()
        {
            return Permanent;
        }
        public static Permanemt getInstance()
        {
            return instance;
        }
        public SemiEx.CSemiExp semi
        {
            get;
            set;
        }
        public ScopeStack<Elements> stack  // pushed and popped by scope rule's action
        {
            get { return stack_; }
        }
        // the locations table is the result returned by parser's actions
        // in this demo

        public List<Elements> locations
        {
            get { return locations_; }
        }
        public void clear()
        {
            Permanent.Clear();
        }


    }

}
