/////////////////////////////////////////////////////////////////////////
// PermanentRepository.cs  -This will hold all the information gathered//
//                          in first and second pass                   //
// ver 1.0                                                             //
// Language:    C#, Visual Studio 13.0, .Net Framework 4.5             //
// Platform:    HP Pavilion dv6 , Win 7, SP 1                          //
// Application: Code Analyzer                                          //
// Author:      Mandeep Singh, Syracuse University                     //
//              315-751-3413, mjhajj@syr.edu                           //
//                                                                     // 
/////////////////////////////////////////////////////////////////////////
/*
 * Module Operations
 * =================
 * This module defines the following class
 *  RepositoryPermanemt - This will create the repository which will contain all the types defined in files
 *  Elements - Hold the type information
 *  Relation - Hold the relationship information
 *  storeRel - It will maintain a list of relation objects to store all the detected relations
 *  Test - used for test Stub
 * 
 * Public Interface
 * ================
 *  copyElements(Repository repo, string file, List<Elem> last)  //Copy elements from each file to a application wide repository
 *  returnPermanentRep() // returns the list of element
 *  returnRelation()  // returns the list of relation
 * 
 */
/*
 * Build Process
 * =============
 * Required Files:
 *   PermanentRepository.cs
 *   
 * Build command:
 *   csc /target:library /D:TEST_PERMANENTREPOSITORY PermanentRepository.cs
 * 
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
    public class RepositoryPermanemt
    {

        static List<Elements> PermanentLocation = new List<Elements>();

        /* This function will copy all the elements from local location to a permanent location for each file*/

        public void copyElements(Repository repo, string file, List<Elem> last)
        {
            List<Elem> elem = repo.locations;


            foreach (Elem e in elem)
            {
                Elements elements = new Elements();
                elements.file = file;

                elements.type = e.type;
                elements.name = e.name;
                elements.begin = e.begin;
                elements.end = e.end;
                elements.complex = e.complex;

                /*To keep track of the namespace*/

                foreach (Elem l in last)
                {
                    
                        if (l.type == "namespace")  /*To handle file with no namespaces*/
                        {
                            if ((e.begin >= l.begin) && (e.end <= l.end))
                            {
                                elements.namesp = l.name;
                            }
                            else
                            {
                                continue;
                            }
                         }
                }
                PermanentLocation.Add(elements);
            }
        }

        /*<-- It will return the permanent repository>*/
        public List<Elements> returnPermanentRep()
        {
            return PermanentLocation;
        }     
        public void clear()
        {
            PermanentLocation.Clear();
        }
        
    }

    /*<-- This class contains the field which will hold all the information in first pass-->*/
    public class Elements
    {
        public string file { get; set; }
        public string namesp { get; set; }
        public string type { get; set; }
        public string name { get; set; }
        public int begin { get; set; }
        public int end { get; set; }
        public int complex { get; set; }
    }

    /*<-- This class contains the field which will hold all the relationship information in second pass-->*/
    public class Relation
    {
        public string file { get; set; }
        public string Dependingfile { get; set; }
        public string namespace1 { get; set; }
        public string type1 { get; set; }
        public string name { get; set; }
        public string namespace2 { get; set; }
        public string type2 { get; set; }
        public string objAggregation { get; set; }


    }

    
    public class storeRel
    {
        static List<Relation> relatn = new List<Relation>();

        /*<-- This function will add the relation object to our relation list>*/
        static public void addRelation(Relation r)
        {
            relatn.Add(r);
        }

        /*<-- This function will return the relation list>*/
        static public List<Relation> returnRelation()
        {
            return relatn;
        }
    }

    public class Test
    {

#if(TEST_PERMANENTREPOSITORY)
        static void Main()
        {
            Console.Write("\n  Test Repository");
            Console.Write("\n =================\n");
            Elements e = new Elements();
            Relation r = new Relation();
            List<Elements> perm = new List<Elements>();
            List<Relation> rln = new List<Relation>();
            e.name = "A";
            e.namesp = "Namespace";
            e.type = "Class";
            e.begin = 10;
            e.end = 50;
            perm.Add(e);
            r.namespace1 = "parser";
            r.namespace2 = "analyzer";
            r.name = "Composes";
            r.type1 = "A";
            r.type2 = "B";
            rln.Add(r);
            Console.Write("\n\n Type Analysis of file\n");
            foreach (Elements el in perm)
            {
                Console.Write("\n{0,10} {1,10} {2,10} {3,10} {4,10}", el.namesp, el.type, el.name, el.begin, el.end);
            }
            Console.Write("\n\n Relationship Analysis of file\n");
            foreach (Relation r1 in rln)
            {
                Console.Write("\n{0,10} {1,10} {2,10} {3,10} {4,10}", r1.namespace1, r1.type1, r1.name, r1.namespace2, r1.type2);
            }
            Console.Write("\n\n");
        }
#endif
    }
}

