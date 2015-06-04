/////////////////////////////////////////////////////////////////////////
// RulesAndActions.cs  - Contains the rules and actions                //
//                       for building the parser for type analysis and //
//                       relationship analysis                         //
// ver 1.0                                                             //
// Language:    C#, Visual Studio 13.0, .Net Framework 4.5             //
// Platform:    HP Pavilion dv6 , Win 7, SP 1                          //
// Application: Code Analyzer                                          //
// Author:      Mandeep Singh, Syracuse University                     //
//              315-751-3413, mjhajj@syr.edu                           //
//                                                                     //
// Source:      Jim Fawcett, CST 2-187, Syracuse University            //
//              (315) 443-3948, jfawcett@twcny.rr.com                  //
/////////////////////////////////////////////////////////////////////////
/*
 * Module Operations
 * =================
 * This module defines the following class
 *  Elem
 *  Relation
 *  Repository
 *  PushStack
 *  PopStack
 *  SearchInherit
 *  SearchAggregation
 *  SearchComposition
 *  SearchUsing
 *  PrintFunction
 *  Print
 *  DetectNamespace
 *  DetectClass
 *  DetectFunction
 *  DetectStruct
 *  DetectEnum
 *  DetectDelegates
 *  DetectBracelessScope
 *  DetectAnonymousScope
 *  DetectLeavingScope
 *  BuildCodeAnalyzer
 *  DetectInherit
 *  DetectComposition
 *  DetectAggregate
 *  DetectUsing
 *  RelationshipAnalyzer
 * 
 * 
 * 
 */
/*
 * Note:
 * This package does not have a test stub since it cannot execute
 * without requests from Parser.
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
    
    public class Elem  // holds scope information
    {
        public string type { get; set; }
        public string name { get; set; }
        public int begin { get; set; }
        public int end { get; set; }

        public int complex { get; set; }
        public override string ToString()
        {
            StringBuilder temp = new StringBuilder();
            temp.Append("{");
            temp.Append(String.Format("{0,-10}", type)).Append(" : ");
            temp.Append(String.Format("{0,-10}", name)).Append(" : ");
            temp.Append(String.Format("{0,-5}", begin.ToString()));  // line of scope start
            temp.Append(String.Format("{0,-5}", end.ToString()));    // line of scope end
            temp.Append("}");
            return temp.ToString();
        }
    }

    
    public class Repository
    {
        ScopeStack<Elem> stack_ = new ScopeStack<Elem>();
        static List<Elem> locations_ = new List<Elem>();
        
        static Repository instance;

        public Repository()
        {
            instance = this;
        }

        public static Repository getInstance()
        {
            return instance;
        }
        // provides all actions access to current semiExp

        public SemiEx.CSemiExp semi
        {
            get;
            set;
        }

        // semi gets line count from toker who counts lines
        // while reading from its source

        public int lineCount  // saved by newline rule's action
        {
            get { return semi.lineCount; }
        }
        public int prevLineCount  // not used in this demo
        {
            get;
            set;
        }
        // enables recursively tracking entry and exit from scopes

        public ScopeStack<Elem> stack  // pushed and popped by scope rule's action
        {
            get { return stack_; }
        }
        // the locations table is the result returned by parser's actions
        // in this demo

        public List<Elem> locations
        {
            get { return locations_; }
        }

    }

    /////////////////////////////////////////////////////////
    // pushes scope info on stack when entering new scope

    public class PushStack : AAction
    {
        Repository repo_;

        public PushStack(Repository repo)
        {
            repo_ = repo;
        }
        public override void doAction(SemiEx.CSemiExp semi, string file)
        {
            Elem elem = new Elem();
            elem.type = semi[0];  // expects type
            elem.name = semi[1];  // expects name
            elem.begin = repo_.semi.lineCount - 1;
            elem.end = 0;
            if(elem.type.Equals("function"))
                elem.complex = 1;
            int index = semi.Contains("delegate");
            if (index == -1)
               repo_.stack.push(elem);
            if (index != -1)
            {
                int end = repo_.semi.lineCount - 1;
                elem.end = end;
            }
            /*<--To increment the complexity counter if we have braceless, anonymus and function type with end=0 -->*/
            if (elem.type == "control" || elem.name == "anonymous")
            {
                for (int i = 0; i < repo_.locations.Count; ++i)
                {
                    Elem temp = repo_.locations[i];
                    if (temp.type == "function")
                    {
                            if ((repo_.locations[i]).end == 0)
                            {
                                (repo_.locations[i]).complex = (repo_.locations[i]).complex + 1;
                                break;
                            }
                     }
                }
               return;
            }
            repo_.locations.Add(elem);
            if (AAction.displaySemi)
            {
                Console.Write("\n  line# {0,-5}", repo_.semi.lineCount - 1);
                Console.Write("entering ");
                string indent = new string(' ', 2 * repo_.stack.count);
                Console.Write("{0}", indent);
                this.display(semi); // defined in abstract action
            }
            if (AAction.displayStack)
                repo_.stack.display();
        }
    }
    /////////////////////////////////////////////////////////
    // pops scope info from stack when leaving scope

    public class PopStack : AAction
    {
        Repository repo_;

        public PopStack(Repository repo)
        {
            repo_ = repo;
        }
        public override void doAction(SemiEx.CSemiExp semi, string file)
        {
            Elem elem;

            try
            {
                /*<---To set the end of the type--->*/
                elem = repo_.stack.pop();
                for (int i = 0; i < repo_.locations.Count; ++i)
                {
                    Elem temp = repo_.locations[i];
                    if (elem.type == temp.type)
                    {
                        if (elem.name == temp.name)
                        {
                            if ((repo_.locations[i]).end == 0)
                            {
                                (repo_.locations[i]).end = repo_.semi.lineCount;
                                break;
                            }
                        }
                    }
                }
            }
            catch
            {
                Console.Write("popped empty stack on semiExp: ");
                semi.display();
                return;
            }
            SemiEx.CSemiExp local = new SemiEx.CSemiExp();
            local.Add(elem.type).Add(elem.name);
            if (local[0] == "control")
                return;

            if (AAction.displaySemi)
            {
                Console.Write("\n  line# {0,-5}", repo_.semi.lineCount);
                Console.Write("leaving  ");
                string indent = new string(' ', 2 * (repo_.stack.count + 1));
                Console.Write("{0}", indent);
                this.display(local); // defined in abstract action
            }
        }
    }

    public class SearchInherit : AAction
    {
            Permanemt repo_, rp1;

            
            public SearchInherit(Permanemt repo, Permanemt rp)
            {
                repo_ = repo;
                rp1 = rp;
            }
            public override void doAction(SemiEx.CSemiExp semi, string file)
            {
                Relation rln = new Relation();
                List<Elements> nm = repo_.stack.getList();
                List<Elements> loc = repo_.locations;
                Permanemt pr = new Permanemt();
                List<Elements> perm = pr.returnRep();
                     
                /*<----To find the namespace of the type 1 by scanning the repository---->*/
                foreach (Elements elm in perm)
                    {
                        if (elm.file == file)
                        {
                            if (elm.type == "class")
                            {
                                if (elm.name == semi[0])
                                {
                                    if ((elm.begin <= repo_.semi.lineCount) && (elm.end >= repo_.semi.lineCount))
                                    {
                                        rln.namespace1 = elm.namesp;
                                    }
                                }
                            }
                        }
                    }
               /*<-- If semi count is 3 then it's not a special case but we count is more than 3 then we have namespace appended with class name-->*/
                if (semi.count == 3)
                {
                    rln.type1 = semi[0];
                    rln.namespace2 = rln.namespace1;                    
                    rln.type2 = semi[2];
                    rln.name = "Inherit";
                    rln.file = file;
                }
                else
                {
                    rln.type1 = semi[0];
                    rln.type2 = semi[3];
                    rln.name = "Inherit";
                    rln.file = file;
                    rln.namespace2 = semi[2];
                }
                foreach (Elements elm in perm)
                {
                    if((elm.name.Equals(rln.type2)) && (elm.namesp.Equals(rln.namespace2)))
                    {
                        rln.Dependingfile = elm.file;
                    }
                }
                storeRel.addRelation(rln);            
        }
    }

    public class SearchAggregation : AAction
    {
        Permanemt repo_, rp1;

        public SearchAggregation(Permanemt repo, Permanemt rp)
        {
            repo_ = repo;
            rp1 = rp;
        }
        public override void doAction(SemiEx.CSemiExp semi, string file)
        {
            Relation rln = new Relation();
            List<Elements> nm = repo_.stack.getList();
            Permanemt pr = new Permanemt();
            List<Elements> perm = pr.returnRep();
            /*<--To find th namespace and class name in which object is getting created-->*/          
            foreach (Elements elm in perm)
            {
                if (elm.file == file)
                {
                    if (elm.type == "class")
                    {
                        if ((elm.begin <= repo_.semi.lineCount) && (elm.end >= repo_.semi.lineCount))
                        {
                            rln.namespace1 = elm.namesp;
                            rln.type1 = elm.name;
                        }
                    }
                }
            } 
            /*<--If my DetectAggregation returns a semi with more than 3 in cout, then it's a case of Namespace.Class -->*/        
            if (semi.count == 3)
            {
                rln.namespace2 = rln.namespace1;
                rln.type2 = semi[1];
                rln.name = "Aggregate";
                rln.file = file;
                rln.objAggregation = semi[2];
            }
            else
            {
                rln.type2 = semi[2];
                rln.name = "Aggregate";
                rln.file = file;
                rln.namespace2 = semi[1];
                rln.objAggregation = semi[3];
            }
            foreach (Elements elm in perm)
            {
                if ((elm.name.Equals(rln.type2)) && (elm.namesp.Equals(rln.namespace2)))
                {
                    rln.Dependingfile = elm.file;
                }
            }
            storeRel.addRelation(rln);
         }
    }

    public class SearchComposition : AAction
    {
        Permanemt repo_, rp1;

        public SearchComposition(Permanemt repo, Permanemt rp)
        {
            repo_ = repo;
            rp1 = rp;
        }
        public override void doAction(SemiEx.CSemiExp semi, string file)
        {
            List<Elements> nm = repo_.stack.getList();
            List<Elements> loc = repo_.locations;
            
            int defination = semi.Contains("Declared");
            /*<--Capturing the two  cases where, struct/enum is defined and we have instance of struct/enum -->*/
            if (defination == -1)
            {
                addToRelation(semi, file);
            }
            else
            {
                definedRelation(semi, file);
            }
        }

        /*<--------This function will store data if we use struct variable in our code---------->*/
        public void addToRelation(SemiEx.CSemiExp semi, string file)
            {
                Relation rln = new Relation();
                Permanemt pr = new Permanemt();
                List<Elements> perm = pr.returnRep();

                rln.type2 = semi[0];
                rln.name = "Composing";
                rln.file = file;
               /*<--To find the namespace in which the struct was detected, added this as one struct can be present in many namespaces-->*/
                for (int i = 1; i < semi.count; i++)
                {
                    foreach (Elements e in perm)
                    {
                        if (e.file == file && e.type.Equals("namespace"))
                        {
                            if (e.begin <= repo_.semi.lineCount && e.end >= rp1.semi.lineCount)  //1 file with multiple namespaces
                            {
                                rln.namespace2 = e.name;
                                rln.namespace1 = e.name;
                            }
                        }
                    }
                }
                foreach (Elements e in perm)
                {
                    if (e.file == file && e.type.Equals("class") && e.namesp == rln.namespace1)
                    {
                        if (e.begin <= repo_.semi.lineCount && e.end >= repo_.semi.lineCount)
                        {
                            rln.type1 = e.name; 
                        }
                    }
                }
                foreach (Elements elm in perm)
                {
                    if ((elm.name.Equals(rln.type2)) && (elm.namesp.Equals(rln.namespace2)))
                    {
                        rln.Dependingfile = elm.file;
                    }
                }
                storeRel.addRelation(rln);
            }

        /*<----Below function will store the class name and namespace in which struct/enum is defined---->*/

        public void definedRelation(SemiEx.CSemiExp semi, string file)
        {
            Relation rln = new Relation();
            Permanemt pr = new Permanemt();
            List<Elements> perm = pr.returnRep();

            rln.type2 = semi[1];
            rln.name = "Has Defined";
            rln.file = file;
            for (int i = 1; i < semi.count - 1; i++)
            {
                foreach (Elements e in perm)
                {
                    if (e.file == file && e.type.Equals("namespace"))
                    {
                        if (e.begin <= repo_.semi.lineCount && e.end >= rp1.semi.lineCount)  //1 file with multiple namespaces
                        {
                            rln.namespace2 = e.name;
                            rln.namespace1 = e.name;
                        }
                    }
                }
            }
            foreach (Elements e in perm)
            {
                if (e.file == file && e.type.Equals("class") && e.namesp == rln.namespace1)
                {
                    if (e.begin <= repo_.semi.lineCount && e.end >= repo_.semi.lineCount)
                    {
                        rln.type1 = e.name;
                    }
                }
            }
            foreach (Elements elm in perm)
            {
                if ((elm.name.Equals(rln.type2)) && (elm.namesp.Equals(rln.namespace2)))
                {
                    rln.Dependingfile = elm.file;
                }
            }
            storeRel.addRelation(rln);

        }
    }

    public class SearchUsing : AAction
    {
        Permanemt repo_, rp1;
        List<Relation> temp = storeRel.returnRelation();
        public SearchUsing(Permanemt repo, Permanemt rp)
        {
            repo_ = repo;
            rp1 = rp;
        }
        public override void doAction(SemiEx.CSemiExp semi, string file)
        {
            /*<--To distinguish between the two case of using relation>*/
            int i = 0;
            foreach (Relation e in temp)
            {
                if (e.objAggregation == semi[0])
                {
                    i = 1;
                }
            }

            /*<-------To find the using relations in object.function(); cases------>*/

            if (i == 1)
            {
                usingObjectFunction(semi, file);
            }

            /*<---------To find the relationships for function(Class obj1) types------->*/

            else if (i == 0)
            {
                usingInFunction(semi, file);
            }
        }

        /*<----To find using relation when fucntion is called by object of that class----->*/
        //Example - Parser p = builder.build();
        public void usingObjectFunction(SemiEx.CSemiExp semi, string file)
        {
            Relation rln = new Relation();
            Permanemt pr = new Permanemt();
            List<Elements> perm = pr.returnRep();
            foreach (Relation e in temp)
            {
                if (e.objAggregation == semi[0])
                {
                    rln.type2 = e.type2;
                    rln.namespace2 = e.namespace2;
                }
            }
            List<string> tempNS = new List<string>();
                        
            foreach (Elements l in perm)
            {
                     if (l.type == "class" && l.file == file)
                     {
                            tempNS.Add(l.name);  
                     }
            }
            /*<--To find the class name for type1-->*/
            foreach (string tempns in tempNS)
            {
                 foreach (Elements ns in perm)
                 {
                     if (ns.name == tempns && ns.file == file)
                     {
                         if (ns.begin <= repo_.semi.lineCount && ns.end >= rp1.semi.lineCount)
                         {
                                rln.type1 = ns.name;
                                rln.namespace1 = ns.namesp;
                                break;
                         }
                      }
                  }
             }
             rln.file = file;
             rln.name = "Using";
             foreach (Elements elm in perm)
             {
                 if ((elm.name.Equals(rln.type2)) && (elm.namesp.Equals(rln.namespace2)))
                 {
                     rln.Dependingfile = elm.file;
                 }
             }
             storeRel.addRelation(rln);
        }

        /*<------This function will determine and store the relationships for function(ClassA a, ClassB b) types>*/
        public void usingInFunction(SemiEx.CSemiExp semi, string file)
        {
            Permanemt pr = new Permanemt();
            List<Elements> perm = pr.returnRep();
            string namespace1= null, namespace2=null, type1=null;
            int begin_ = 0, end_ = 0;
            foreach (Elements l in perm)
            {
                    if (l.file == file)
                    {
                        if (l.type == "function" && l.name == semi[0] && l.begin <= repo_.semi.lineCount && l.end >= repo_.semi.lineCount)
                        {
                            namespace1 = l.namesp;
                            namespace2 = l.namesp;
                            begin_ = l.begin;
                            end_ = l.end;
                            break;
                        }
                    }
                }
                foreach (Elements lo in perm)
                {
                    if (lo.file == file)
                    {
                        if (lo.type == "class" && lo.begin <= begin_ && lo.end >= end_) // Check it once more to verify and might use perm
                            type1 = lo.name;
                    }
                }
                for (int j = 1; j < semi.count; j++)
                {
                    foreach (Elements lm in perm)
                    {
                        if (namespace1 == lm.namesp)
                        {
                            if (lm.name == (semi[j]) && lm.type =="class")
                            {
                                Relation rln = new Relation();
                                rln.type1 = type1;
                                rln.namespace1 = namespace1;
                                rln.namespace2 = namespace2;
                                rln.type2 = semi[j];
                                rln.file = file;
                                rln.name = "Using";
                                foreach (Elements elm in perm)
                                {
                                    if ((elm.name.Equals(rln.type2)) && (elm.namesp.Equals(rln.namespace2)))
                                    {
                                        rln.Dependingfile = elm.file;
                                    }
                                }
                                storeRel.addRelation(rln);
                                break;
                            }
                        }
                    }
                }
        }
    }
    
    ///////////////////////////////////////////////////////////
    // action to print function signatures - not used in demo

    public class PrintFunction : AAction
    {
        Repository repo_;

        public PrintFunction(Repository repo)
        {
            repo_ = repo;
        }
        public override void display(SemiEx.CSemiExp semi)
        {
            Console.Write("\n    line# {0}", repo_.semi.lineCount - 1);
            Console.Write("\n    ");
            for (int i = 0; i < semi.count; ++i)
                if (semi[i] != "\n" && !semi.isComment(semi[i]))
                    Console.Write("{0} ", semi[i]);
        }
        public override void doAction(SemiEx.CSemiExp semi, string file)
        {
            this.display(semi);
        }
    }
    /////////////////////////////////////////////////////////
    // concrete printing action, useful for debugging

    public class Print : AAction
    {
        Repository repo_;

        public Print(Repository repo)
        {
            repo_ = repo;
        }
        public override void doAction(SemiEx.CSemiExp semi, string file)
        {
            Console.Write("\n  line# {0}", repo_.semi.lineCount - 1);
            this.display(semi);
        }
    }
    /////////////////////////////////////////////////////////
    // rule to detect namespace declarations

    public class DetectNamespace : ARule
    {
        public override bool test(SemiEx.CSemiExp semi, string file)
        {
            int index = semi.Contains("namespace");
            if (index != -1)
            {
                SemiEx.CSemiExp local = new SemiEx.CSemiExp();
                // create local semiExp with tokens for type and name
                local.displayNewLines = false;
                local.Add(semi[index]).Add(semi[index + 1]);
                doActions(local, file);
                return true;
            }
            return false;
        }
    }
    /////////////////////////////////////////////////////////
    // rule to dectect class definitions

    public class DetectClass : ARule
    {
        public override bool test(SemiEx.CSemiExp semi, string file)
        {
            int indexCL = semi.Contains("class");
            int indexIF = semi.Contains("interface");
            int index = Math.Max(indexCL, indexIF);
            
            if (index != -1)
            {
                SemiEx.CSemiExp local = new SemiEx.CSemiExp();
                // local semiExp with tokens for type and name
                local.displayNewLines = false;
                local.Add(semi[index]).Add(semi[index + 1]);
                doActions(local, file);
                return true;
            }
            return false;
        }
    }
    /////////////////////////////////////////////////////////
    // rule to dectect function definitions

    public class DetectFunction : ARule
    {
        public static bool isSpecialToken(string token)
        {
            string[] SpecialToken = { "if", "for", "foreach", "while", "catch", "using","else","else if","try" };
            foreach (string stoken in SpecialToken)
                if (stoken == token)
                    return true;
            return false;
        }
        public override bool test(SemiEx.CSemiExp semi, string file)
        {
            if (semi[semi.count - 1] != "{")
                return false;

            int index = semi.FindFirst("(");
            if (index > 0 && !isSpecialToken(semi[index - 1]))
            {
                SemiEx.CSemiExp local = new SemiEx.CSemiExp();
                local.Add("function").Add(semi[index - 1]);
                doActions(local, file);
                return true;
            }
            return false;
        }
    }
    /*<-- Rule to detect struct -->*/
    public class DetectStruct : ARule
    {
        public override bool test(SemiEx.CSemiExp semi, string file)
        {
            int index = semi.Contains("struct");
            if (index != -1)
            {
                SemiEx.CSemiExp local = new SemiEx.CSemiExp();
                local.Add(semi[index]);
                local.Add(semi[index + 1]);
                doActions(local,file);
                return true;
            }
            return false;
        }
    }
    /*<-- Rule to detect enum -->*/
    public class DetectEnum : ARule
    {
        public override bool test(SemiEx.CSemiExp semi, string file)
        {
            int index = semi.Contains("enum");
            if (index != -1)
            {
                SemiEx.CSemiExp local = new SemiEx.CSemiExp();
                local.Add(semi[index]);
                local.Add(semi[index + 1]);
                doActions(local, file);
                return true;
            }
            return false;
        }
    }
    /*<-- Rule to detect delegates -->*/
    public class DetectDelegates : ARule
    {
        public override bool test(SemiEx.CSemiExp semi, string file)
        {
            int indexDE = semi.Contains("delegate");
            if (indexDE != -1)
            {
                SemiEx.CSemiExp local = new SemiEx.CSemiExp();

                int index = semi.FindFirst("(");
                if (semi[index - 1] == ">")
                {
                    index = semi.FindFirst("<");
                    local.Add(semi[indexDE]);
                    local.Add(semi[index - 1]);
                    doActions(local, file);
                }
                else
                {
                    local.Add(semi[indexDE]);
                    local.Add(semi[indexDE + 2]);
                    doActions(local, file);
                    return true;
                }

            }
            return false;
        }
    }

    /////////////////////////////////////////////////////////
    // detect entering anonymous scope
    // - expects namespace, class, and function scopes
    //   already handled, so put this rule after those
    public class DetectAnonymousScope : ARule
    {
        
        public override bool test(SemiEx.CSemiExp semi, string file)
        {
            int index = semi.Contains("{");

            if (index != -1)
            {
                SemiEx.CSemiExp local = new SemiEx.CSemiExp();
                // create local semiExp with tokens for type and name
                local.displayNewLines = false;
                local.Add("control").Add("anonymous");
                doActions(local, file);
                return true;
            }
            return false;
        }
    }
    /////////////////////////////////////////////////////////
    // detect leaving scope

    public class DetectLeavingScope : ARule
    {
        public override bool test(SemiEx.CSemiExp semi, string file)
        {
            int index = semi.Contains("}");
            if (index != -1)
            {
                doActions(semi, file);
                return true;
            }
            return false;
        }
    }
    /*<-- Rule to detect braceless scope for calculating complexity -->*/
    public class DetectBracelessScope : ARule
    {
        public static bool isSpecialToken(SemiEx.CSemiExp semi)
        {
            string[] SpecialToken = { "if", "for", "foreach", "while", "catch", "else", "else if", "try" };
            for (int i = 0; i < semi.count; i++)
            {
                foreach (string stoken in SpecialToken)
                    if (stoken == semi[i])
                        return true;
            }

            return false;
        }
        public override bool test(SemiEx.CSemiExp semi, string file)
        {
            if(isSpecialToken(semi))
            {
                SemiEx.CSemiExp local = new SemiEx.CSemiExp();
                // create local semiExp with tokens for type and name
                local.displayNewLines = false;
                local.Add("control").Add("anonymous");
                doActions(local, file);
                return true;
            }
            return false;
        }
     }
    
   
    public class BuildCodeAnalyzer
    {
        Repository repo = new Repository();
        
        public BuildCodeAnalyzer(SemiEx.CSemiExp semi)
        {
            repo.semi = semi;
            repo.locations.Clear();
        }

        /*<-- This function will build our parser for the type analysis. 
         * It will detect the types based on the rules defined for the detection
         * Appropriate action is also configured with each detection of type*/

        public virtual Parser build()
        {
            Parser parser = new Parser();
            // decide what to show
            AAction.displaySemi = false;
            AAction.displayStack = false;  // this is default so redundant
            PushStack push = new PushStack(repo);

            DetectNamespace detectNS = new DetectNamespace();
            detectNS.add(push);
            parser.add(detectNS);

            DetectClass detectCl = new DetectClass();
            detectCl.add(push);
            parser.add(detectCl);

            DetectFunction detectFN = new DetectFunction();
            detectFN.add(push);
            parser.add(detectFN);
            
            DetectStruct istruct = new DetectStruct();
            istruct.add(push);
            parser.add(istruct);

            DetectEnum ienum = new DetectEnum();
            ienum.add(push);
            parser.add(ienum);

            DetectDelegates del = new DetectDelegates();
            del.add(push);
            parser.add(del);
            
            // handle entering anonymous scopes, e.g., if, while, etc.
            DetectAnonymousScope anon = new DetectAnonymousScope();
            anon.add(push);
            parser.add(anon);

            PopStack pop = new PopStack(repo);
            DetectBracelessScope brace = new DetectBracelessScope();
            brace.add(push);
            brace.add(pop);
            parser.add(brace);
            // handle leaving scopes
            DetectLeavingScope leave = new DetectLeavingScope();
            leave.add(pop);
            parser.add(leave);
            // parser configured
            return parser;
        }
    }


    public class DetectInherit : ARule
    {
        /*<---- This function will detect the inheritance in our semi
         *  expression and will build local semi for SearchInheritance-->*/
        public override bool test(SemiEx.CSemiExp semi, string file)
        {
            int indexC = semi.Contains(":");
            int indexCL = semi.Contains("class");
            if ((indexC != -1) && (indexCL != -1))
            {
                SemiEx.CSemiExp local = new SemiEx.CSemiExp();
                local.Add(semi[indexCL + 1]);
                local.Add("Inherits");
                int indexSplit = semi.Contains(".");
                /*<--To check if we have inehritance of class which is present in different namespace-->*/
                if (indexSplit != -1 && indexC < indexSplit)
                {
                    int pos = semi.FindFirst(".");
                    local.Add(semi[pos - 1]);
                    local.Add(semi[pos + 1]);
                }
                else
                {
                    local.Add(semi[indexC + 1]);
                }
                doActions(local, file);
                return true;
            }
            return false;
        }
    }

    public class str
    {
        public string ns { get; set; }
        public string nm { get; set; }
    }
    public class DetectComposition : ARule
    {
        List<str> temp = new List<str>();
        /*<---- This function will detect the composition in our semi expression and will build/call functions for building local semi expression-->*/
        public override bool test(SemiEx.CSemiExp semi, string file)
        {
            int indexS = semi.Contains("struct");
            int indexE = semi.Contains("enum");
            int indexN = semi.Contains("new");
            int indexEq = semi.Contains("=");
            if((indexS != -1 || indexE != -1) && (semi[semi.count -1] == "{"))
            {
                if (indexS != -1)
                {
                    constructStruct();
                    constructLocal(semi, file,indexS);
                }
                else
                {      
                    constructStruct();
                    constructLocal(semi, file, indexE);
                }
             }
            else if(indexN == -1 && semi[semi.count -1] == ";")
            {
                   constructStruct();
                   int indexO = semi.FindFirst("<");
                   int indexC = semi.FindFirst(">");
                   SemiEx.CSemiExp local = new SemiEx.CSemiExp();
                   for(int i=0; i<semi.count;i++)
                   {
                       foreach (str sname in temp)
                       {
                           if (sname.nm.Equals(semi[i]))
                           {
                               if (!(indexO < i && indexC > i))
                               {
                                   if (local.count == 0)
                                       local.Add(semi[i]);

                                   local.Add(sname.ns);
                               }
                           }
                       }
                   }
                   if(local.count >=1 )
                   {
                      doActions(local,file);
                      temp.Clear();
                   }
            }
            return true;
        }
        /*<-- This function will create a list of all structs which are present in all the files-->*/
        public void constructStruct()
        {
            Permanemt pr = new Permanemt();
            List<Elements> perm = pr.returnRep();
            temp.Clear();
            foreach (Elements e in perm)
            {
                  str structNS = new str();

                  if (e.type.Equals("struct") || e.type.Equals("enum"))
                  {
                       structNS.nm = e.name;
                       structNS.ns = e.namesp;
                       temp.Add(structNS);
                  }    
            }
        }

        /*<-- This function will build local semi for SearchComposition-->*/
        public void constructLocal(SemiEx.CSemiExp semi, string file, int index)
        {
            SemiEx.CSemiExp local = new SemiEx.CSemiExp();
            foreach (str sname in temp)
            {
                if (sname.nm.Equals(semi[index + 1]))
                {
                    if (local.count == 0)
                    {
                        local.Add("Declared");
                        local.Add(semi[index + 1]);
                    }
                    local.Add(sname.ns);
                }
            }
            if (local.count >= 1)
            {
                doActions(local, file);
                temp.Clear();
            }
        }
}
          
    
    public class DetectAggregate : ARule
    {
        /*<-- This function will detect aggregation and will build the local semi expression for SearchAggregate-->*/
        public override bool test(SemiEx.CSemiExp semi, string file)
        {
            int indexN = semi.Contains("new");
            int indexE = semi.FindFirst("=");
            Permanemt pr = new Permanemt();
            List<Elements> perm = pr.returnRep();

            if ((indexN != -1) &&(indexE != -1))
            {
                int indexP = semi.Contains("(");
                int indexS = semi.Contains("<");
                if(indexS != -1)
                {
                    indexP = Math.Min(indexS, indexP);
                }
                SemiEx.CSemiExp local = new SemiEx.CSemiExp();
                local.Add("Aggregate");
                int indexSplit = semi.FindFirst(".");
                if(indexSplit != -1)
                {
                    local.Add(semi[indexSplit - 1]);
                    local.Add(semi[indexSplit + 1]);
                    local.Add(semi[indexE - 1]);
                }
                else
                {
                    local.Add(semi[indexP - 1]);
                    local.Add(semi[indexE - 1]);
                }
                /*<-- To detect the user defined types and will exclude collections like, List<string> s = new List<string>();>*/
                foreach( Elements e in perm)
                {
                        int i= local.Contains(e.name);
                        if(i != -1 && e.type.Equals("class"))
                        {
                            doActions(local, file);
                            break;
                        }
                }
                return true;
            }
            return false;
        }
    }

    public class DetectUsing : ARule
    {
        
             public static bool isSpecialToken(string token)
             {
                 string[] SpecialToken = { "if", "for", "foreach", "while", "catch", "using","elseif" };
                 foreach (string stoken in SpecialToken)
                    if (stoken == token)
                        return true;
                 return false;
             }
            /*<-- This function will detect the using relationship and will call actions of SearchUsing>*/
            public override bool test(SemiEx.CSemiExp semi, string file)
            {
                int indexE = semi.FindFirst("=");
                int indexP = semi.FindFirst("(");
                int indexD = semi.FindFirst(".");
                int indexN = semi.Contains("new");

                int index = semi.FindFirst("(");
                int indexO = semi.Contains(")");
                int difference = indexO - index;
                int indexC = semi.FindFirst(",");
                int compare;
                /*<-- To detect the function call or defination-->*/
                if(indexC == -1)
                {
                     compare = index;    //modified from indexO to index
                }
                else
                {
                    compare = indexC - index;
                }
                /*<-- It will detect using relationship in object.Function(); types>*/
                if ((semi[semi.count - 1] != "{") && (semi[semi.count - 1] != "}") && (indexN == -1) && (indexP > 1) && (indexD > 1) && (indexE > 1))
                {
                    constructLocal(semi, file, indexD, indexP, indexE);
                }
                /*<----It will check if there is using relationship in function---->*/
                else if (semi[semi.count - 1] == "{" && difference > 2 && compare > 2 && index > 0 && !isSpecialToken(semi[index - 1]))
                {   
                    if(!(isObject(semi[index-1])))
                    {
                        SemiEx.CSemiExp local = new SemiEx.CSemiExp();
                        local.Add(semi[index - 1]);   //function name
                        local.Add(semi[index + 1]);   //Class name
                        while (indexC != -1)
                        {
                            local.Add(semi[indexC + 1]);
                            semi.remove(indexC);
                            indexC = semi.FindFirst(",");
                        }
                        if (local.count >= 1)
                             doActions(local, file);
                        return true;   
                    }
                }          
                return false;
            }
            /*<-- This will create the local semi for using relatioship-->*/
            public void constructLocal(SemiEx.CSemiExp semi, string file,int indexD, int indexP, int indexE)
            {
                SemiEx.CSemiExp local = new SemiEx.CSemiExp();
                while (indexD != -1)
                {
                        if ((indexD > indexE) && (indexD < indexP))
                        {
                            local.Add(semi[indexD - 1]);  //object name
                            local.Add(semi[indexD + 1]);  //function name
                            break;
                         }
                         else
                         {
                            indexD = semi.findNext(indexD, ".");
                         }
                }
                if (local.count >= 1)
                {
                    doActions(local, file);
                }
            }
            public bool isObject(string semi)
            {
                List<Relation> temp = storeRel.returnRelation();
                foreach (Relation e in temp)
                {
                    if (e.objAggregation == semi)
                    {
                        return true;
                    }
                }
                return false;
             }
    }
    public class RelationshipAnalyzer
    {
        Permanemt repo;
        Permanemt rp1 = new Permanemt();   

        public RelationshipAnalyzer(SemiEx.CSemiExp semi)
        {
            repo = Permanemt.getInstance();   //It might give only the last file of pass 1 instance
            rp1.semi = semi;
        }
        
        /*<-- This function will build our parser for the relationship analysis. 
         * It will detect the relations based on the rules defined for the detection
         * Appropriate action is also configured with each detection of type*/

        public virtual Parser build()
        {
            Parser parser = new Parser();

            // decide what to show
            AAction.displaySemi = false;
            AAction.displayStack = false;  // this is default so redundant

            SearchInherit searchI = new SearchInherit(repo,rp1);
            SearchComposition searchC = new SearchComposition(repo, rp1);
            SearchAggregation searchA = new SearchAggregation(repo, rp1);
            SearchUsing searchU = new SearchUsing(repo, rp1);

            //Detect Inheritance
            DetectInherit inherit = new DetectInherit();
            inherit.add(searchI);
            parser.add(inherit);

            //Detect Compositon
            DetectComposition compose = new DetectComposition();
            compose.add(searchC);
            parser.add(compose);

            //Detect Aggregate
            DetectAggregate aggregate = new DetectAggregate();
            aggregate.add(searchA);
            parser.add(aggregate);

            //Detect Using
            DetectUsing usng = new DetectUsing();
            usng.add(searchU);
            parser.add(usng);

            // parser configured
            return parser;
        }
    }
}