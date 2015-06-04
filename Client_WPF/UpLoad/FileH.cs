/////////////////////////////////////////////////////////////////////////
// FileH.cs  -  File Manager                                           //
//                It will search for the files present in the given    //
//                directory                                            //
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
 * This module define the following classes
 *   CFileMgr
 * 
 * Public Interface
 * ================
 * 
 * SearchFiles(string path, List<string> patterns, List<string> options) //Searches file
 * returnFiles()  // returns files
 * Main(string[] args) // for test stub
 * 
 */
/*
 * Build Process
 * =============
 * Required Files:
 *   FileH.cs 
 * 
 * Compiler Command:
 * ================
 *   csc /target:library /define:TEST_FILEMGR FileH.cs 
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

namespace DependencyAnalyzer
{
    public class CFileMgr
    {

        List<string> files = new List<string>();
        
        /*<------This function will search for the files in specified path and will search for the given pattern----->*/
        public void SearchFiles(string path, List<string> patterns, List<string> options, ref List<string> file)
        {
            foreach (string patrn in patterns)
            {
                try
                {   string[] temp = Directory.GetFiles(path, patrn);
                    if (temp.Count() == 0)
                        Console.Write("There is no matching file present in {0}", path);
                    else
                    {
                        for (int i = 0; i < temp.Length; ++i)
                            temp[i] = Path.GetFullPath(temp[i]);
                        file.AddRange(temp);
                    }
                }catch (DirectoryNotFoundException e) { Console.WriteLine("Directory not found: " + e.Message); }
                catch (UnauthorizedAccessException e) { Console.Write("\nYou are not authorised to access this directory", e); }
                catch (IOException e) { Console.Write("\nYou are not authorised to access this directory", e); }
                catch (Exception e){Console.Write("{0}", e.Message);}
            }
            /*<-----Check for the recursive search option----->*/
            try
            { if (options.Contains("/S") || options.Contains("/s"))
                {
                    string[] dirs = Directory.GetDirectories(path);
                    foreach (string dir in dirs)
                    {
                        SearchFiles(dir, patterns, options, ref file);    
                    }
                }
            }catch(Exception e){Console.Write("\nNo options were given as input",e);}
        }
        public void FilterTemoraryFile(ref List<string> file)
        {
            List<string> ToRemv = new List<string>();
            foreach(string f in file)
            {
                if(f.Contains("Temporary"))
                {
                    ToRemv.Add(f);
                }
            }
            file.RemoveAll((x)=> ToRemv.Contains(x));
        }

    /*<-----------Test Stub----------------->*/
#if(TEST_FILEMGR)
        static void Main(string[] args)
        {
            Console.Write("\n  Testing FileMgr Class");
            Console.Write("\n =======================\n");

            CFileMgr test = new CFileMgr();
            string path = ".";
            List<string> patrn = new List<string>();
            List<string> optn = new List<string>();
            List<string> fl = new List<string>();

            patrn.Add("*.*");
            test.SearchFiles(path, patrn, optn, ref fl);

            foreach (string file in test.files)
            {
                Console.Write("\n  {0}", file);
            }
        }
#endif
    }
}
