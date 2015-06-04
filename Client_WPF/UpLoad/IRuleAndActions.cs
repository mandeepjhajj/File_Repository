/////////////////////////////////////////////////////////////////////////
//IRuleAndAction.cs - Interfaces & abstract bases for rules and actions//
//                                                                     //
// ver 1.0                                                             //
// Language:    C#, Visual Studio 13.0, .Net Framework 4.5             //
// Platform:    HP Pavilion dv6 , Win 7, SP 1                          //
// Application: Code Analyzer                                          //
// Author:      Mandeep Singh, Syracuse University                     //
//              315-751-3413, mjhajj@syr.edu                           //
//                                                                     //
// Source:      Jim Fawcett, CST 4-187, Syracuse University            //
//              (315) 443-3948, jfawcett@twcny.rr.com                  //
/////////////////////////////////////////////////////////////////////////
/*
 * Module Operations:
 * ------------------
 * This module defines the following classes:
 *   IRule   - interface contract for Rules
 *   ARule   - abstract base class for Rules that defines some common ops
 *   IAction - interface contract for rule actions
 *   AAction - abstract base class for actions that defines common ops
 */
/* Required Files:
 *   IRuleAndAction.cs
 *   
 * Build command:
 *   Interfaces and abstract base classes only so no build
 *   
 * Maintenance History
 * ===================
 * 
 * ver 1.0 : 06 September 2014
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
    public interface IAction
    {
        void doAction(SemiEx.CSemiExp semi, string file);
        
    }
    /////////////////////////////////////////////////////////
    // abstract action base supplying common functions

    public abstract class AAction : IAction
    {
        static bool displaySemi_ = false;   // default
        static bool displayStack_ = false;  // default

        public abstract void doAction(SemiEx.CSemiExp semi, string file);
        
        public static bool displaySemi
        {
            get { return displaySemi_; }
            set { displaySemi_ = value; }
        }
        public static bool displayStack
        {
            get { return displayStack_; }
            set { displayStack_ = value; }
        }

        public virtual void display(SemiEx.CSemiExp semi)
        {
            if (displaySemi)
                for (int i = 0; i < semi.count; ++i)
                    Console.Write("{0} ", semi[i]);
        }
    }
    /////////////////////////////////////////////////////////
    // contract for parser rules

    public interface IRule
    {
        bool test(SemiEx.CSemiExp semi, string file);
        void add(IAction action);
    }
    /////////////////////////////////////////////////////////
    // abstract rule base implementing common functions

    public abstract class ARule : IRule
    {
        private List<IAction> actions;
        public ARule()
        {
            actions = new List<IAction>();
        }
        public void add(IAction action)
        {
            actions.Add(action);
        }
        abstract public bool test(SemiEx.CSemiExp semi, string file);
        public void doActions(SemiEx.CSemiExp semi, string file)
        {
            foreach (IAction action in actions)
                action.doAction(semi, file);
        }
        public int indexOfType(SemiEx.CSemiExp semi)
        {
            int indexCL = semi.Contains("class");
            int indexIF = semi.Contains("interface");
            int indexST = semi.Contains("struct");
            int indexEN = semi.Contains("enum");
            int indexDE = semi.Contains("delegate");

            int index = Math.Max(indexCL, indexIF);
            index = Math.Max(index, indexST);
            index = Math.Max(index, indexEN);
            index = Math.Max(index, indexDE);
            return index;
        }
    }
}
