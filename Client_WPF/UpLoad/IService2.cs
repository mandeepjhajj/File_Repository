/////////////////////////////////////////////////////////////////////////
// IService.cs - This package decide the service contract on which     //
//                          message should communicate                 //
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
 * This module defines the following interface:
 *   IDependency
 *   
 * Public Interface
 * ================
 * SendMessage- Send the message
 * ReceiveMessage - Receives the message
 * UploadFile - Upload the file on channel
 */
/*
 * Build Process
 * =============
 * Required Files: IService2.cs Service2.cs
 * 
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
using System.Text;

namespace DependencyAnalyzer
{
    [ServiceContract(Namespace = "DependencyAnalyzer")]
    public interface IDependency
    {
        [OperationContract(IsOneWay = true)]
        void SendMessage(Message m);

        [OperationContract(IsOneWay = true)]
        void GetProjects();

        Message ReceiveMessage();

        [OperationContract(IsOneWay = true)]
        void upLoadFile(FileTransferMessage msg);
        [OperationContract]
        Stream downLoadFile(string filename);
    }
    [MessageContract]
    public class FileTransferMessage
    {
        [MessageHeader(MustUnderstand = true)]
        public string filename { get; set; }

        [MessageBodyMember(Order = 1)]
        public Stream transferStream { get; set; }
    }
    [MessageContract]
    public class Message
    {
        [MessageHeader]
        public MessageHeader header;
        [MessageBodyMember]
        public MessageBody body;
        public static Message MakeMessage(bool content)
        {
            Message msg = new Message();
            if (content == true)
                msg.header.IsQuit = content;
            else
                msg.body.text = "Reply";
            return msg;
        }
    }
    [DataContract]
    public class MessageHeader
    {
        [DataMember]
        public string SenderAddress { get; set; }
        [DataMember]
        public string ReceiverAddress { get; set; }
        [DataMember]
        public int MessageNumber { get; set; }
        [DataMember]
        public int HashNumber { get; set; }
        [DataMember]
        public bool IsDulpicate { get; set; }
        [DataMember]
        public bool IsReply { get; set; }
        [DataMember]
        public bool IsQuit { get; set; }

    }
    [DataContract]
    public class MessageBody
    {
        [DataMember]
        private FileVerzDetail fd;
        [DataMember]
        private List<string> ServerList;
        [DataMember]
        public List<string> FileList;
        [DataMember]
        public string ClientURL { get; set; }
        [DataMember]
        public string text { get; set; }
    }
    [DataContract]
    public class FileVerzDetail
    {
        [DataMember]
        public string filename { get; set; }
        [DataMember]
        public int version_number { get; set; }
        [DataMember]
        public string servername { get; set; }
    }

}


