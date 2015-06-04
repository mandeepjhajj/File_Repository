/////////////////////////////////////////////////////////////////////////
// MasterServerRepository.cs  -This will hold all the information gathered//
//                          in first and second pass                   //
// ver 1.0                                                             //
// Language:    C#, Visual Studio 13.0, .Net Framework 4.5             //
// Platform:    HP Pavilion dv6 , Win 7, SP 1                          //
// Application: dependency Analyzer                                    //
// Author:      Mandeep Singh, Syracuse University                     //
//              315-751-3413, mjhajj@syr.edu                           //
//                                                                     // 
/////////////////////////////////////////////////////////////////////////
/*
 * Module Operations
 * =================
 * This module defines the following class
 *  MasterServerRepository 
 * 
 * Public Interface
 * ================
 *  AnalyzeAndMerge()
 *  returnPermanentRep() // returns the list of element
 *  returnRelation()  // returns the list of relation
 * 
 */
/*
 * Build Process
 * =============
 * Required Files:
 *   MasterServerRepository.cs
 *   
 * Build command:
 *   csc /target:library /D:TEST_REPOSITORY MasterServerRepository.cs
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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DependencyAnalyzer
{
    public class MElements  // holds scope information
    {
        public string filename { get; set; }
        public string servernm { get; set; }
        public string namesp { get; set; }
        public string type { get; set; }
        public string name { get; set; }
        public int begin { get; set; }
        public int end { get; set; }

        public int complex { get; set; }
    }
        public class MasterServerRepository
        {
            static List<MElements> merged = new List<MElements>();
            int flag = 0;
            public void Merge(TempElem t)
            {
                MElements elements = new MElements();
                elements.filename = t.file;
                elements.servernm = t.servnm;
                elements.type = t.type;
                elements.name = t.name;
                elements.namesp = t.namesp;
                elements.begin = t.begin;
                elements.end = t.end;
                elements.complex = t.complex;
                merged.Add(elements);
            }
            //Check for the type already present in repository, if not then add it 
            public bool AnalyzeAndMerge(TempElem e)
            {
                foreach(MElements m in merged)
                {
                       if(e.file.Equals(m.filename))
                       {
                           if(e.namesp.Equals(m.namesp))
                           {
                               if(e.type.Equals(m.type))
                               {
                                   if(e.name.Equals(m.name))
                                   {
                                       return true;
                                   }
                               }
                           }
                       }
                 }
                return false;
                        
            }
            //---------returns the instance of repository
            static public List<MElements> returnPermanentRep()
            {
                return merged;
            }
#if(TEST_REPOSITORY)
        static void Main()
        {
            Console.Write("\n  Test Repository");
            Console.Write("\n =================\n");
            MElements e = new MElements();
            
            List<MElements> perm = new List<MElements>();
            
            e.name = "A";
            e.namesp = "Namespace";
            e.type = "Class";
            e.begin = 10;
            e.end = 50;
            perm.Add(e);

            e.name = "B";
            e.namesp = "CodeAnalysis";
            e.type = "function";
            e.begin = 150;
            e.end = 200;
            perm.Add(e);
            Console.Write("\n\n Type Analysis of file\n");
            Console.Write("\n ----------------------------\n\n");
            foreach (MElements el in perm)
            {
                Console.Write("\n{0,10} {1,10} {2,10} {3,10} {4,10}", el.namesp, el.type, el.name, el.begin, el.end);
            }
            
        }
#endif
        }
}
