/////////////////////////////////////////////////////////////////////////
// Parsing.cs - Parser detects code constructs defined by rules        //
//                                                                     //
// ver 1.0                                                             //
// Language:    C#, Visual Studio 13.0, .Net Framework 4.5             //
// Platform:    HP Pavilion dv6 , Win 7, SP 1                          //
// Application: Code Analyzer                                          //
// Author:      Mandeep Singh, Syracuse University                     //
//              315-751-3413, mjhajj@syr.edu                           //
// Source:      Jim Fawcett, CST 4-187, Syracuse University            //
//              (315) 443-3948, jfawcett@twcny.rr.com                  //
//                                                                     //
/////////////////////////////////////////////////////////////////////////
/*
 * Module Operations
 * =================
 * This module defines the following class:
 *   Parser  - a collection of IRules
 *   TestParser  - used for test stub
 *   
 * Public Interface
 * ================
 * add(IRule rule)                      // This will add the rules to the List Rules of IRule type.
 * parse(SemiEx.CSemiExp semi, string file) // This will test each rule for type analysis on the current semi expression.
 * relations(SemiEx.CSemiExp semi, string file) // This will test each rule for relationship analysis on current semi expression.
 * 
 */
/*
 * Build Process
 * =============
 * Required Files:
 *   IRulesAndActions.cs, RulesAndActions.cs, Parsing.cs, Semiexpresion.cs, Tokenizer.cs
 *   
 * Build command:
 *   csc /target:library /D:TEST_PARSER Parsing.cs IRulesAndActions.cs RulesAndActions.cs Semiexpresion.cs Tokenizer.cs
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
using System.IO;
using System.Collections;

namespace DependencyAnalyzer
{
    public class Parser
    {
        private List<IRule> Rules;

        public Parser()
        {
            Rules = new List<IRule>();
        }

        /*<------This function will add each rule while configuring the parser-------->*/
        public void add(IRule rule)
        {
            Rules.Add(rule);
        }

        /*<----------This function will apply rules for current semi expression and will stop if it detects some rule---->*/
        public void parse(SemiEx.CSemiExp semi, string file)
        {
            // Note: rule returns true to tell parser to stop
            //       processing the current semiExp

            foreach (IRule rule in Rules)
            {

                if (rule.test(semi, file))
                    break;
            }
        }

        /*<---------To find relationship we need to apply all rules----------->*/
        public void relations(SemiEx.CSemiExp semi, string file)
        {

            foreach (IRule rule in Rules)
            {
                if (rule.test(semi, file))
                    continue;
            }
        }
    }
   public class TestParser
  {
    //----< process commandline to get file references >-----------------

    static List<string> ProcessCommandline(string[] args)
    {
      List<string> files = new List<string>();
      if (args.Length == 0)
      {
        Console.Write("\n  Please enter file(s) to analyze\n\n");
        return files;
      }
      string path = args[0];
      path = Path.GetFullPath(path);
      for (int i = 1; i < args.Length; ++i)
      {
        string filename = Path.GetFileName(args[i]);
        files.AddRange(Directory.GetFiles(path, filename));
      }
      return files;
    }

    static void ShowCommandLine(string[] args)
    {
      Console.Write("\n  Commandline args are:\n");
      foreach (string arg in args)
      {
        Console.Write("  {0}", arg);
      }
      Console.Write("\n\n  current directory: {0}", System.IO.Directory.GetCurrentDirectory());
      Console.Write("\n\n");
    }


        /*<---------------Test Stub >--------------------->*/

#if(TEST_PARSER)

    static void Main(string[] args)
    {
      Console.Write("\n  Demonstrating Parser");
      Console.Write("\n ======================\n");

      ShowCommandLine(args);

      List<string> files = TestParser.ProcessCommandline(args);
      foreach (string file in files)
      {
        Console.Write("\n  Processing file {0}\n", file as string);

        SemiEx.CSemiExp semi = new SemiEx.CSemiExp();
        semi.displayNewLines = false;
        if (!semi.open(file as string))
        {
          Console.Write("\n  Can't open {0}\n\n", args[0]);
          return;
        }

        Console.Write("\n  Type and Function Analysis");
        Console.Write("\n ----------------------------\n");

        BuildCodeAnalyzer builder = new BuildCodeAnalyzer(semi);
        Parser parser = builder.build();

        try
        {
          while (semi.getSemi())
            parser.parse(semi, file);
          Console.Write("\n\n  locations table contains:");
        }
        catch (Exception ex)
        {
          Console.Write("\n\n  {0}\n", ex.Message);
        }
        Repository rep = Repository.getInstance();
        List<Elem> table = rep.locations;
        foreach (Elem e in table)
        {
          Console.Write("\n  {0,10}, {1,25}, {2,5}, {3,5}", e.type, e.name, e.begin, e.end);
        }
        Console.WriteLine();
        Console.Write("\n\n  That's all folks!\n\n");
        semi.close();
      }
    }
#endif

    }
}