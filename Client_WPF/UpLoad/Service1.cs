/////////////////////////////////////////////////////////////////////////
// Servcice1.cs - This package implement the service contract and defines //
//                  classes for sending and receiving the message     //
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
 *   Receiver
 *   Sender
 *   
 * Public Interface
 * ================
 * SendMessage - This method in sender class is used to send post the message at channel
 * SendMessage - This method in receiver class is used to receive the message at channel
 * ReceiveMessage - It received the messages and deQueue it from blocking queue
 * Uploadfile - Upload the file on channel
 * Downloadfile - Download the file form channel
 * CreateRecvChannel - Creates the receivng channel
 * CreateSendChannel - Creates the sending channel
 * 
 * 
 */
/*
 * Build Process
 * =============
 * Required Files: Service1.cs, BlockingQueue.cs,IService1.cs
 *   
 * Build command:
 *   csc /target:library /D:TEST_SERVICE BlockingQueue.cs IService1.cs Service1.cs
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
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.Text;
using System.Threading;

namespace DependencyAnalyzer
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession)]
    public class Receiver : IDependency
    {
        string filename;
        string savePath = "..\\..\\SavedFiles";
        string ToSendPath = "..\\..\\ToSend";
        int BlockSize = 1024;
        byte[] block;
        delegate Message SendReply(string reply_url, int msgNbr);
        event SendReply CreateReply;

        static BlockingQueue<Message> BlockingQ = null;
        ServiceHost service = null;

        public static int downloaded;
        public static Dictionary<string, int> status = new Dictionary<string, int>();
        delegate void NewFile(string filename);
        event NewFile OnNewFile;

        public Receiver()
        {
            if (BlockingQ == null)
                BlockingQ = new BlockingQueue<Message>();

            CreateReply += new SendReply(MakeReply);
            block = new byte[BlockSize];
        }
        //  Create ServiceHost for Communication service
        public void CreateRecvChannel(string address)
        {
            BasicHttpBinding binding = new BasicHttpBinding();
            binding.MaxReceivedMessageSize = 500000000;
            Uri baseAddress = new Uri(address);
            service = new ServiceHost(typeof(Receiver), baseAddress);
            service.AddServiceEndpoint(typeof(IDependency), binding, baseAddress);
            service.Open();
        }
        public void Close()
        {
            service.Close();
        }
        public void SendMessage(Message msg)
        {
            if(!msg.header.SenderAddress.Equals("http://localhost:8000/Dependency"))
            {
                string reply_url = IdentifyClient(msg);
                Sender sndr = new Sender(reply_url);
                BlockingQ.enQ(msg);
                int msgNumber = msg.header.MessageNumber;
                Message reply = CreateReply(reply_url, msgNumber);  //Ok reply that receiver has received the message
                sndr.SendMessage(reply);
            }
            else
            {
                BlockingQ.enQ(msg);
            }
                       
           
        }
        public Message ReceiveMessage()
        {
            return BlockingQ.deQ();
        }
        public string IdentifyClient(Message msg)
        {
            string msg_senderAdd = msg.header.SenderAddress;
            return msg_senderAdd;
        }

        public Message MakeReply(string reply_url, int msgNbr)
        {
            Message mees = new Message();
            MessageBody b2 = new MessageBody();
            MessageHeader h2 = new MessageHeader();
            b2.text = "Reply OK message";
            h2.ReceiverAddress = reply_url;
            h2.IsReply = true;
            h2.MessageNumber = msgNbr;
            mees.body = b2;
            mees.header = h2;
            return mees;
        }
        public void GetProjects()
        {
            Console.Write("This is testing phase");
        }
        public void upLoadFile(FileTransferMessage msg)
        {
            filename = msg.filename;
            string rfilename = Path.Combine(savePath, filename);
            if (!Directory.Exists(savePath))
                Directory.CreateDirectory(savePath);
            using (var outputStream = new FileStream(rfilename, FileMode.Create))
            {
                while (true)
                {
                    int bytesRead = msg.transferStream.Read(block, 0, BlockSize);
                    if (bytesRead > 0)
                        outputStream.Write(block, 0, bytesRead);
                    else
                        break;
                }
            }
            OnNewFileHandler(filename);
            Console.Write("\n  Received file \"{0}\"", filename);
        }

        public Stream downLoadFile(string filename)
        {
            string sfilename = Path.Combine(ToSendPath, filename);
            FileStream outStream = null;
            if (File.Exists(sfilename))
            {
                outStream = new FileStream(sfilename, FileMode.Open);
                Console.Write("\n  Sending File \"{0}\"", filename);
            }
            else
                throw new Exception("open failed for \"" + filename + "\"");
            return outStream;
        }
        public void OnNewFileHandler(string filename)
        {
            downloaded = 1;
            try
            {
                status.Add(filename, 1);
            }
            catch(Exception e)
            {
                Console.Write("Same key already added",e.Message);
            }
        }
        //------------Test Stub----------------
#if(TEST_SERVICE)
        public static void Main()
        {
            Console.Write("\n  Test Service");
            Console.Write("\n =================\n");
            Receiver rev = new Receiver();
            Sender sen = new Sender("http://localhost:4005/Dependency");
            string address = "http://localhost:1234/Dependency";
            rev.CreateRecvChannel(address);
            Message m = new Message();
            MessageBody b = new MessageBody();
            b.text = "Dummy message";
            MessageHeader h = new MessageHeader();
            m.body.text = "Dummy message";
            sen.SendMessage(m);
            Message rep = rev.ReceiveMessage();
        }
#endif

    }
    public class Sender
    {
        IDependency channel;
        string lastError = "";
        BlockingQueue<Message> sndBlockingQ = null;
        Thread sndThrd = null;
        int tryCount = 0, MaxCount = 10;

        string ToSendPath = "..\\..\\ToSend";
        string SavePath = "..\\..\\SavedFiles";
        int BlockSize = 1024;
        byte[] block;
        // Processing for sndThrd to pull msgs out of sndBlockingQ
        // and post them to another Peer's Communication service

        void ThreadProc()
        {
            while (true)
            {
                Message msg = sndBlockingQ.deQ();
                
                channel.SendMessage(msg);
                if (msg.body.text == "break")
                    break;
            }
        }

        // Create Communication channel proxy, sndBlockingQ, and
        // start sndThrd to send messages that client enqueues

        public Sender(string url)
        {
            block = new byte[BlockSize];
            sndBlockingQ = new BlockingQueue<Message>();
            while (true)
            {
                try
                {
                    CreateSendChannel(url);
                    tryCount = 0;
                    break;
                }
                catch (Exception ex)
                {
                    if (++tryCount < MaxCount)
                        Thread.Sleep(100);
                    else
                    {
                        lastError = ex.Message;
                        break;
                    }
                }
            }
            sndThrd = new Thread(ThreadProc);
            sndThrd.IsBackground = true;
            sndThrd.Start();
        }

        // Create proxy to another Peer's Communicator

        public void CreateSendChannel(string address)
        {
            EndpointAddress baseAddress = new EndpointAddress(address);
            BasicHttpBinding binding = new BasicHttpBinding();
            binding.MaxReceivedMessageSize = 500000000;
            ChannelFactory<IDependency> factory = new ChannelFactory<IDependency>(binding, address);
            channel = factory.CreateChannel();
            
        }

        // Sender posts message to another Peer's queue using
        // Communication service hosted by receipient via sndThrd

        public void SendMessage(Message msg)
        {
            sndBlockingQ.enQ(msg);
        }

        public string GetLastError()
        {
            string temp = lastError;
            lastError = "";
            return temp;
        }

        public void Close()
        {
            ChannelFactory<IDependency> temp = (ChannelFactory<IDependency>)channel;
            temp.Close();
        }
        public void uploadFile(string filename)
        {
            Console.Write("\n  sending file \"{0}\"", filename);
            string fqname = Path.Combine(ToSendPath, filename);
            using (var inputStream = new FileStream(fqname, FileMode.Open))
            {
                FileTransferMessage msg = new FileTransferMessage();
                msg.filename = filename;
                msg.transferStream = inputStream;
                channel.upLoadFile(msg);
            }
        }

        public void download(string filename)
        {
            try
            {
                Stream strm = channel.downLoadFile(filename);
                string rfilename = Path.Combine(SavePath, filename);
                if (!Directory.Exists(SavePath))
                    Directory.CreateDirectory(SavePath);
                using (var outputStream = new FileStream(rfilename, FileMode.Create))
                {
                    while (true)
                    {
                        int bytesRead = strm.Read(block, 0, BlockSize);
                        if (bytesRead > 0)
                            outputStream.Write(block, 0, bytesRead);
                        else
                            break;
                    }
                }
                Console.Write("\n  Received file \"{0}\"", filename);
            }
            catch (Exception ex)
            {
                Console.Write("\n  {0}\n", ex.Message);
            }
        }

    }
}