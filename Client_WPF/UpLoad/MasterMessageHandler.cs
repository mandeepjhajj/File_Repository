/////////////////////////////////////////////////////////////////////////
// MessageHandler.cs - This package help the controller to decide the  //
//                      action associated with message                 //
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
 *   Messagehandler
 *   
 * Public Interface
 * ================
 * recogmessage - This returns the unique value which help the controller to decide the course of action associated with message
 */
/*
 * Build Process
 * =============
 * Required Files: Messagehandler.cs
 *   
 * Build command:
 *   csc /target:library /D:TEST_HANDLER MessageHandler.cs
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

namespace DependencyAnalyzer
{
    public class Messagehandler
    {
        // Returns the int value which help the controller to take an appropriate action on message
        public int RecogMessage(Message msg)
        {
            int recog_para=0 ;
            switch(msg.header.HashNumber)
            {
                case 3:
                    Console.Write("\nMerge the type tables\n");
                    recog_para = 3;
                    break;

                default :
                    break;
            }
            return recog_para;
        }
        //--------------Test stub--------------------
#if(TEST_HANDLER)
        public static void Main()
        {
            Console.Write("\n  Test Handler");
            Console.Write("\n =================\n");
            
            Messagehandler hld = new Messagehandler();
            Message m = new Message();
            m.header.HashNumber = 1;
            m.body.text = "dummy message";
            int i= hld.RecogMessage(m);
            Console.Write("\n{0}\n",i);
        }
#endif
    }
}
