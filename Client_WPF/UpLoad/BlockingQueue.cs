/////////////////////////////////////////////////////////////////////////
// BlockingQueue.cs - This package is used for making a queue thread safe//
//                 It does so by creating lock.                        //
//                                                                     //
// ver 1.0                                                             //
// Language:    C#, Visual Studio 13.0, .Net Framework 4.5             //
// Platform:    HP Pavilion dv6 , Win 7, SP 1                          //
// Application: Dependency Analyzer                                    //
// Author:      Jim Fawcett, CST 4-187, Syracuse University            //
//              (315) 443-3948, jfawcett@twcny.rr.com                  //
//                                                                     //
/////////////////////////////////////////////////////////////////////////
/*
 * Module Operations
 * =================
 * This module defines the following class:
 *   BlockingQueue<T>
 *   
 * Public Interface
 * ================
 * enQ - Enueue the message in blocking queue
 * deQ - Dequeue the message from  blocking queue
 * size- get the size of queue
 * clear - clear the queue
 * 
 */
/*
 * Build Process
 * =============
 * Required Files: BlockingQueue.cs
 *   
 * Build command:
 *   csc /target:library /D:TEST_BLOCK BlockingQueue.cs 
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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DependencyAnalyzer
{
    public class BlockingQueue<T>
    {
        private Queue blockingQ;
        ManualResetEvent ev;

        //----< constructor >--------------------------------------------

        public BlockingQueue()
        {
            Queue Q = new Queue();
            blockingQ = Q;
            ev = new ManualResetEvent(false);
        }
        //----< enqueue a string >---------------------------------------

        public void enQ(T msg)
        {
            lock (blockingQ)
            {
                blockingQ.Enqueue(msg);
                ev.Set();
            }
        }
        //
        //----< dequeue a T >---------------------------------------
        //
        //  This looks more complicated than you might think it needs
        //  to be; however without the second count check:
        //    If a single item is in the queue and a thread
        //    moves toward the deQ but finishes its time allocation
        //    before deQ'ing another thread may get throught the locks
        //    and deQ.  Then the first thread wakes up and since its
        //    waitFlag is false, attempts to deQ the empty queue.
        //  This is the reason for the second count check.

        public T deQ()
        {
            T msg = default(T);
            while (true)
            {
                if (this.size() == 0)
                {
                    ev.Reset();
                    ev.WaitOne();
                }
                lock (blockingQ)
                {
                    if (blockingQ.Count != 0)
                    {
                        msg = (T)blockingQ.Dequeue();
                        break;
                    }
                }
            }
            return msg;
        }
        //----< return number of elements in queue >---------------------

        public int size()
        {
            int count;
            lock (blockingQ) { count = blockingQ.Count; }
            return count;
        }
        //----< purge elements from queue >------------------------------

        public void clear()
        {
            lock (blockingQ) { blockingQ.Clear(); }
        }
        //--------------Test Stub------------------//
#if(TEST_BLOCK)
        public static void Main()
        {
            Console.Write("\n  Test Blocking Queue");
            Console.Write("\n =================\n");
            
            BlockingQueue<string> q = new BlockingQueue<string>();
            q.enQ("test");
            string a = q.deQ();
        }
#endif
    }
}
