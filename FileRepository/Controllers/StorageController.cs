using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using FileRepository.Models;
using System.IO;
using System.Xml.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text;


namespace FileRepository.Controllers
{
    public class StorageController : Controller
    {
        private Entities db = new Entities();

        private HttpClient client = new HttpClient();
        private HttpRequestMessage message;
        private HttpResponseMessage response = new HttpResponseMessage();
        private string urlBase;
        public string status { get; set; }

        public string checkIfContains(DirectoryInfo d, bool flag){
            if (flag)
            {
                FileInfo[] containFile = d.GetFiles();
                if (containFile.Length == 0)
                {
                    return "N";
                }
                else
                {
                    return "Y";
                }
            }
            else
            {
                DirectoryInfo[] containDir = d.GetDirectories();
                if (containDir.Length == 0)
                {
                    return "N";
                }
                else
                {
                    return "Y";
                }
            }

            
        }
        public string getFolderSize(DirectoryInfo folder)
        {
            DirectoryInfo di = new DirectoryInfo(folder.FullName);
        
            long sum = di.EnumerateFiles("*.*", SearchOption.AllDirectories).Sum(fi => fi.Length);
            string size = DisplaySize(sum);
            return size;
        }

        public string DisplaySize(long? size)
        {
            if (size == null)
                return string.Empty;
            else
            {
                if (size < 1024)
                    return string.Format("{0:N0} bytes", size.Value);
                else
                    return String.Format("{0:N0} KB", size.Value / 1024);
            }
        }

        private bool IsMatch(string extension, params string[] extensionsToCheck)
        {
            foreach (var str in extensionsToCheck)
                if (string.CompareOrdinal(extension, str) == 0)
                    return true;

            // If we reach here, no match
            return false;
        }

        public string fileType(FileInfo file)
        {
            var extension = Path.GetExtension(file.Name);

            if (IsMatch(extension, ".txt"))
                return "Text file";
            else if (IsMatch(extension, ".pdf"))
                return "PDF file";
            else if (IsMatch(extension, ".doc", ".docx"))
                return "Microsoft Word document";
            else if (IsMatch(extension, ".xls", ".xlsx"))
                return "Microsoft Excel document";
            else if (IsMatch(extension, ".jpg", ".jpeg"))
                return "JPEG image file";
            else if (IsMatch(extension, ".gif"))
                return "GIF image file";
            else if (IsMatch(extension, ".png"))
                return "PNG image file";


            // If we reach here, return the name of the extension
            if (string.IsNullOrEmpty(extension))
                return "Unknown file type";
            else
                return extension.Substring(1).ToUpper() + " file";
        }


        //add the sub folders in Storage model

        public void addSubFolder(Storage root,string path)
        {
            //string path = System.Web.HttpContext.Current.Server.MapPath("~\\App_Data\\UpLoad")+"\\"+root.Name;

            var currentDirInfo = new DirectoryInfo(path);
            var folders = currentDirInfo.GetDirectories();
            var files = currentDirInfo.GetFiles();

            bool updateFolder = false;
            int matchFound = 0;

            //modification
            Storage modifiedFolder = null;

            //store folders inside sub folders
            foreach (var folder in folders)
            {
                foreach (var str in db.Storages)
                {
                    if (folder.FullName.Equals(str.ServerLocation))
                    {
                        matchFound++;
                        //change < to > as date is not matched
                        if (String.Compare(folder.LastWriteTime.ToString(), str.UpdatedOn, true) > 0)
                        {
                            //modified
                            modifiedFolder = str;
                            updateFolder = true;
                        }
                    }
                }
                //modified
                if (updateFolder)
                {
                    addSubFolder(modifiedFolder, folder.FullName);
                }

                if (matchFound == 0)
                {
                    root.ContainsFolder = "Y";
                    Storage store = new Storage();
                    store.Name = folder.Name;
                    store.SType = "Folder";
                    store.ClientLocation = folder.FullName;
                    store.ServerLocation = folder.FullName;
                    store.ContainsFile = checkIfContains(folder, true);
                    store.ContainsFolder = checkIfContains(folder, false);
                    store.Size = getFolderSize(folder);
                    store.CreatedOn = folder.CreationTime.ToString();
                    store.UpdatedOn = folder.LastWriteTime.ToString();

                    db.Storages.Add(store);

                    //entry in HasFolder table
                    root.Storages1.Add(store); 
                    store.Storage11.Add(root);

                    db.SaveChanges();
                    MyUsersStoreLink userFile = new MyUsersStoreLink();
                    userFile.StoreId = store.Id;

                    string id = (from p in db.AspNetUsers
                                 where p.UserName == System.Web.HttpContext.Current.User.Identity.Name
                                 select p.Id).First().ToString();

                    userFile.UserId = id;

                    db.MyUsersStoreLinks.Add(userFile);
                    db.SaveChanges();

                    
                    if (store.ContainsFolder.Equals("Y"))
                    {
                        addSubFolder(store,path+"\\"+store.Name);
                    }

                    if (store.ContainsFile.Equals("Y"))
                    {
                        addSubFiles(store, path + "\\" + store.Name);
                    }
                }
                matchFound = 0;
                updateFolder = false;
            }
            //to check for modification in file and if there are no more folders
            if(files.Length!=0)
                addSubFiles(root,path);
        }

        //add the sub folders in Storage model
        public void addSubFiles(Storage root,string path)
        {
            //string path = System.Web.HttpContext.Current.Server.MapPath("~\\App_Data\\UpLoad\\")+root.Name;

            var currentDirInfo = new DirectoryInfo(path);
            var files = currentDirInfo.GetFiles();
            bool updateFile = false;
            int matchFound = 0;


            //store files in subfolders
            foreach (var file in files)
            {
                foreach (var str in db.Storages)
                {
                    if (file.FullName.Equals(str.ServerLocation))
                    {
                        matchFound++;
                        if (String.Compare(file.LastWriteTime.ToString(), str.UpdatedOn, true) > 0)
                        {

                            updateFile = true;
                        }
                    }
                }
                if (updateFile)
                {
                    //code to delete the file from database and add new object and set link
                }
                if (matchFound == 0)
                {
                    root.ContainsFile = "Y";
                    Storage store = new Storage();
                    store.Name = file.Name;
                    store.SType = fileType(file);
                    store.ClientLocation = file.FullName;
                    store.ServerLocation = file.FullName;
                    store.ContainsFile = "N";
                    store.ContainsFolder = "N";
                    store.Size = DisplaySize(file.Length);
                    store.CreatedOn = file.CreationTime.ToString();
                    store.UpdatedOn = file.LastWriteTime.ToString();

                    db.Storages.Add(store);

                    //entry in HasFile table
                    root.Storages2.Add(store);
                    store.Storage12.Add(root);

                    db.SaveChanges();
                    MyUsersStoreLink userFile = new MyUsersStoreLink();
                    userFile.StoreId = store.Id;

                    string id = (from p in db.AspNetUsers
                                 where p.UserName == System.Web.HttpContext.Current.User.Identity.Name
                                 select p.Id).First().ToString();

                    userFile.UserId = id;

                    db.MyUsersStoreLinks.Add(userFile);
                    db.SaveChanges();
                }
                updateFile = false;
                matchFound = 0;
            }
        }

        public Dictionary<Storage, List<Storage>> userFilePrivate()
        {
            string path = System.Web.HttpContext.Current.Server.MapPath("~\\App_Data\\UpLoad\\Private");
            IEnumerable<Storage> userPrivateFile = new List<Storage>();

            Dictionary<Storage, List<Storage>> myDict = new Dictionary<Storage, List<Storage>>();
            List<Storage> fList = new List<Storage>();
            List<Storage> sList = new List<Storage>();

            bool addDict = false;

            try
            {
                string id = (from p in db.AspNetUsers
                             where p.UserName == System.Web.HttpContext.Current.User.Identity.Name
                             select p.Id).First().ToString();

                userPrivateFile = from p in db.Storages
                                  where p.MyUsersStoreLinks.Any(l => l.UserId == id) || p.ServerLocation.Equals(path)
                                  select p;

                foreach (var s in userPrivateFile)
                {
                    if (s.ServerLocation.Equals(path))
                    {
                        List<Storage> checkChildFolder = s.Storages1.ToList();
                        List<Storage> checkChildFile = s.Storages2.ToList();
                        checkChildFolder.AddRange(checkChildFile);

                        List<Storage> toAdd = new List<Storage>();
                        foreach (Storage pStore in checkChildFolder)
                        {

                            string currentId = (from p in db.MyUsersStoreLinks
                                                where p.StoreId == pStore.Id
                                                select p.UserId).First().ToString();
                            if (currentId.Equals(id))
                            {
                                toAdd.Add(pStore);
                            }
                        }

                        myDict.Add(s, toAdd);

                    }
                    else
                    {
                        if (s.ContainsFolder.Equals("Y") && s.ServerLocation.Contains(path))
                        {
                            addDict = true;
                            fList = s.Storages1.ToList();

                        }
                        if (s.ContainsFile.Equals("Y") && s.ServerLocation.Contains(path))
                        {
                            addDict = true;
                            sList = s.Storages2.ToList();
                        }

                        if (fList.Count != 0 && sList.Count != 0 && addDict)
                        {
                            fList.AddRange(sList);
                        }
                        else if (fList.Count == 0 && addDict)
                        {
                            fList = sList;
                        }
                    }
                    if (addDict)
                    {
                        if (s.ServerLocation.Equals(path) && !s.ContainsFile.Equals("Y") && !s.ContainsFolder.Equals("Y"))
                        {
                            List<Storage> rootList = myDict[s];
                            rootList.AddRange(fList.ToList());
                        }
                        else
                        {
                            myDict.Add(s, fList.ToList());
                        }

                        fList.Clear();
                        sList.Clear();
                        addDict = false;
                    }
                }
            }
            catch (Exception e) { }
            return myDict;
        }

        public Dictionary<Storage,List<Storage>> publicFiles()
        {
            string path = System.Web.HttpContext.Current.Server.MapPath("~\\App_Data\\UpLoad\\Public");
            IEnumerable<Storage> userPublicFile = new List<Storage>();

            Dictionary<Storage, List<Storage>> myDict = new Dictionary<Storage, List<Storage>>();
            List<Storage> fList = new List<Storage>();
            List<Storage> sList = new List<Storage>();

            bool addDict = false;
            //string id = (from p in db.AspNetUsers
            //             where p.UserName == System.Web.HttpContext.Current.User.Identity.Name
            //             select p.Id).First().ToString();

            try
            {

                userPublicFile = from p in db.Storages
                                 //where p.MyUsersStoreLinks.Any(l => l.UserId == id)
                                 select p;

                foreach (var s in userPublicFile)
                {

                    if (s.ServerLocation.Equals(path))
                    {
                        myDict.Add(s, new List<Storage>());
                    }
                    if (s.ContainsFolder.Equals("Y") && s.ServerLocation.Contains(path))
                    {
                        addDict = true;
                        fList = s.Storages1.ToList();

                    }
                    if (s.ContainsFile.Equals("Y") && s.ServerLocation.Contains(path))
                    {
                        addDict = true;
                        sList = s.Storages2.ToList();
                    }

                    if (fList.Count != 0 && sList.Count != 0 && addDict)
                    {
                        fList.AddRange(sList);
                    }
                    else if (fList.Count == 0 && addDict)
                    {
                        fList = sList;
                    }

                    if (addDict)
                    {
                        if (s.ServerLocation.Equals(path))
                        {
                            List<Storage> rootList = myDict[s];
                            rootList.AddRange(fList.ToList());
                        }
                        else
                        {
                            myDict.Add(s, fList.ToList());
                        }

                        fList.Clear();
                        sList.Clear();
                        addDict = false;
                    }
                }
            }
            catch (Exception e) { }
            return myDict;
        }

        public Dictionary<Storage, List<Storage>> getData()
        {
            Dictionary<Storage, List<Storage>> privateDict = new Dictionary<Storage, List<Storage>>();
            Dictionary<Storage, List<Storage>> publicDict = new Dictionary<Storage, List<Storage>>();
            Dictionary<Storage, List<Storage>> combineDict = new Dictionary<Storage, List<Storage>>();
            try
            {
                privateDict = userFilePrivate();
                publicDict = publicFiles();

                if (privateDict.Count != 0 && publicDict.Count == 0)
                {
                    combineDict = privateDict;
                }
                if (privateDict.Count == 0 && publicDict.Count != 0)
                {
                    combineDict = publicDict;
                }

                if (privateDict.Count != 0 && publicDict.Count != 0)
                {
                    foreach (var c in publicDict)
                    {
                        privateDict.Add(c.Key, c.Value);
                    }
                }
            }
            catch (Exception e) { }
            return privateDict;
           // IEnumerable<Storage> userFile = new List<Storage>();

           // Dictionary<Storage, List<Storage>> myDict = new Dictionary<Storage, List<Storage>>();
           // List<Storage> fList = new List<Storage>();
           // List<Storage> sList = new List<Storage>();

           // bool addDict = false;
           // string id = (from p in db.AspNetUsers
           //              where p.UserName == System.Web.HttpContext.Current.User.Identity.Name
           //              select p.Id).First().ToString();

           // userFile = from p in db.Storages
           //            where p.MyUsersStoreLinks.Any( l => l.UserId == id)
           //            select p;

           // //nomal join
           // //var userFile = from s in db.Storages
           // //               join l in db.MyUsersStoreLinks
           // //               on s.Id equals l.StoreId
           // //               where l.UserId.Equals(id)
           // //               select s;

           // foreach (var s in userFile)
           // {
                
           //     if (s.ContainsFolder.Equals("Y"))
           //     {
           //         addDict = true;
           //         fList = s.Storages1.ToList();
                    
           //     }
           //     if (s.ContainsFile.Equals("Y"))
           //     {
           //         addDict = true;
           //         sList = s.Storages2.ToList();
                    
            
           //             //sList =from p in db.Storages
           //             //                where p.Storages2.Any(q => q.Id == p.Id) //&& p.Storages2.Any(r => r.Id == p.Id)
           //             //                select p;
           //     }

           //     if (fList.Count != 0 && sList.Count!=0 && addDict)
           //     {
           //         fList.AddRange(sList);
           //     }
           //     else if (fList.Count == 0 && addDict)
           //     {
           //         fList = sList;
           //     }

           //     if (addDict)
           //     {
           //         myDict.Add(s, fList.ToList());
           //         fList.Clear();
           //         sList.Clear();
           //         addDict = false;
           //     }
                

           //         //tList =from p in db.Storages
           //         //                   where p.Storage11.Any(q => q.Id == p.Id)
           //         //                   select p;

           //         //lList = from p in db.Storages
           //         //                where p.Storages1.Any(q => q.Id == p.Id)
           //         //                select p;

           //         //appendList.AddRange((from p in db.Storages
           //         //             where p.Storage11.Equals(s.Id)
           //         //            select p).ToList());
           //     }

           // return myDict;
           // //List<Storage> finalList = userFile.Concat(appendList).ToList();
           //// List<Storage> T1List = fList.Concat(sList).ToList();


        }
        
        
        // GET: /Storage/
        [Authorize(Roles = "admin,developer,naiveUser")]
        public ActionResult Index()
        {
            string path = System.Web.HttpContext.Current.Server.MapPath("~\\App_Data\\UpLoad");
           
            var currentDirInfo = new DirectoryInfo(path);
            var folders = currentDirInfo.GetDirectories();
            var files = currentDirInfo.GetFiles();

            bool updateFolder = false, updateFile=false;
            int matchFound=0;
           
            //to copy contents of folder is it has been modifed
             Storage modifiedFolder = null;

            //store folders
            foreach (var folder in folders)
            {
                foreach (var str in db.Storages)
                {
                    if (folder.Name.Equals(str.Name))
                    {
                        matchFound++;
                        
                        if (String.Compare(folder.LastWriteTime.ToString(), str.UpdatedOn, true) > 0)
                        {
                            modifiedFolder = str;
                            updateFolder = true;
                        }
                    }
                }

                if (updateFolder)
                {
                    addSubFolder(modifiedFolder, folder.FullName);
                }
                if (matchFound == 0)
                {
                    
                    Storage store = new Storage();
                    store.Name = folder.Name;
                    store.SType = "Folder";
                    store.ClientLocation = folder.FullName;
                    store.ServerLocation = folder.FullName;
                    store.ContainsFile = checkIfContains(folder, true);
                    store.ContainsFolder = checkIfContains(folder, false);
                    store.Size = getFolderSize(folder);
                    store.CreatedOn = folder.CreationTime.ToString();
                    store.UpdatedOn = folder.LastWriteTime.ToString();

                    db.Storages.Add(store);
                    db.SaveChanges();

                    // commented to check if private works

                    //MyUsersStoreLink userFile = new MyUsersStoreLink();
                    //userFile.StoreId = store.Id;

                    //string id = (from p in db.AspNetUsers
                    //             where p.UserName == System.Web.HttpContext.Current.User.Identity.Name
                    //             select p.Id).First().ToString();

                    //userFile.UserId = id;

                    //db.MyUsersStoreLinks.Add(userFile);
                    //db.SaveChanges();

                    if (store.ContainsFolder.Equals("Y"))
                    {
                        addSubFolder(store,path+"\\"+store.Name);
                    }

                    if (store.ContainsFile.Equals("Y"))
                    {
                        addSubFiles(store, path +"\\"+ store.Name);
                    }
                }
                matchFound = 0;
                updateFolder = false;
            }

            #region
            //store files
            //foreach (var file in files)
            //{
            //    foreach (var str in db.Storages)
            //    {
            //        if (file.Name.Equals(str.Name))
            //        {
            //            matchFound++;
            //            if (String.Compare(file.LastWriteTime.ToString(), str.UpdatedOn, true) > 0)
            //            {
            //                updateFile = true;
            //            }
            //        }
            //    }
            //    if (updateFile || matchFound == 0)
            //    {
            //        Storage store = new Storage();
            //        store.Name = file.Name;
            //        store.SType = fileType(file);
            //        store.ClientLocation = file.FullName;
            //        store.ServerLocation = file.FullName;
            //        store.ContainsFile = "N";
            //        store.ContainsFolder = "N";
            //        store.Size = DisplaySize(file.Length);
            //        store.CreatedOn = file.CreationTime.ToString();
            //        store.UpdatedOn = file.LastWriteTime.ToString();

            //        db.Storages.Add(store);

            //        db.SaveChanges();
            //        MyUsersStoreLink userFile = new MyUsersStoreLink();
            //        userFile.StoreId = store.Id;

            //        string id = (from p in db.AspNetUsers
            //                     where p.UserName == System.Web.HttpContext.Current.User.Identity.Name
            //                     select p.Id).First().ToString();

            //        userFile.UserId = id;

            //        db.MyUsersStoreLinks.Add(userFile);
            //        db.SaveChanges();
            //    }
            //    updateFile = false;
            //    matchFound = 0;
            //}
            #endregion

            Dictionary<Storage, List<Storage>> displayData =getData();

            var viewModel = new DisplayModel();
            viewModel.data = displayData;

            return View("FileManager", viewModel);
           
        }


        // GET: /Storage/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Storage storage = db.Storages.Find(id);
            if (storage == null)
            {
                return HttpNotFound();
            }
            return View(storage);
        }

        
        [HttpPost]
        [Authorize(Roles = "admin,developer,naiveUser")]
        //read the file contents
        public void ReadFile()
        {
            string filepath = Request.Form["path"];
            Dictionary<Storage, List<Storage>> displayData = getData();

            try
            {
                string fileContent = System.IO.File.ReadAllText(filepath);
                Response.Write(fileContent);
            }
            catch (Exception e){ }
            //return View("Display",viewModel);
            //return View("Content",viewModel);
        }

        [HttpPost]
        [Authorize(Roles = "admin,developer,naiveUser")]
        public ActionResult MultipleRead(FormCollection form)
        {
            Dictionary<string, string> fileContents = new Dictionary<string, string>();
            List<string> files = new List<string>();

            for (int i = 0; i < form.Count; i++)
            {
                var value = form.GetKey(i);
                files.Add(value);
                string fileContent = System.IO.File.ReadAllText(value);
                fileContents.Add(value, fileContent);
                
            }

            var displayFile = new MultiFileContent();
            displayFile.fileContent = fileContents;
            displayFile.fileList = files;
            displayFile.filenumber = 0;
            return View(displayFile);
        }

       
        [Authorize(Roles = "admin")]
        public ActionResult DeleteFile()
        {

            //changed for displaying user files and public file. Hiding the other private files

            Dictionary<Storage, List<Storage>> displayData = getData();

            List<Storage> fileList = new List<Storage>();
            List<RelativePathModel> relativeDisplay = new List<RelativePathModel>();

            foreach(KeyValuePair<Storage,List<Storage>> item in displayData)
            {
                foreach (Storage s in item.Value)
                {
                    if (!s.SType.Equals("Folder"))
                    {
                        fileList.Add(s);
                    }
                }

            }
            foreach (Storage f in fileList)
            {
                string returnString = "";
                string[] file = f.ServerLocation.Split('\\');
                string relativePath = "";
                int count = 0;
                foreach (string content in file)
                {
                    if (content.Contains("UpLoad"))
                    {
                        while (count < file.Length - 1)
                        {
                            count++;
                            relativePath = relativePath + "\\" + file[count];
                        }
                        break;
                    }
                    count++;
                }
                returnString = returnString + relativePath;
                RelativePathModel myObject = new RelativePathModel();
                myObject.RelativePath = returnString;
                myObject.ServerLocation = f.ServerLocation;
                relativeDisplay.Add(myObject);
            }

            return View("DeleteRelative",relativeDisplay);

            //foreach (Storage s in db.Storages)
            //{
            //    if (s.SType.Contains("file"))
            //    {
            //        fileList.Add(s);
            //    }
            //}
            //return View("Delete",fileList);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public ActionResult DeleteFile(string pat)
        {
            string toDeletePath = Request.Form["Files"];
            try
            {
                if (toDeletePath != null)
                {
                    int deleteId = (from p in db.Storages
                                    where p.ServerLocation == toDeletePath
                                    select p.Id).First();

                    Storage deleteObject = (from p in db.Storages
                                            where p.Id.Equals(deleteId)
                                            select p).First();

                    List<Storage> parentObj = deleteObject.Storage12.ToList();
                    Storage parentObject = null;
                    if (parentObj.Count != 0)
                    {
                        foreach (Storage s in parentObj)
                        {
                            if (s.Storages2.Contains(deleteObject))
                            {
                                parentObject = s;
                                parentObject.Storages2.Remove(deleteObject);
                                db.SaveChanges();
                            }
                        }

                        if (parentObject.Storages2.Count() == 0)
                        {
                            parentObject.ContainsFile = "N";
                        }

                    }
                    MyUsersStoreLink userLink = (from p in db.MyUsersStoreLinks
                                                 where p.StoreId.Equals(deleteId)
                                                 select p).First();

                    db.MyUsersStoreLinks.Remove(userLink);
                    db.Storages.Remove(deleteObject);
                    db.SaveChanges();
                    System.IO.File.Delete(toDeletePath);
                }
            }
            catch (Exception e) { }
            return RedirectToAction("Index");
        }


        [HttpPost]
        [Authorize(Roles = "admin,developer,naiveUser")]
        public ActionResult MultipleFoder(FormCollection form)
        {
            List<Storage> myList = new List<Storage>();

            Dictionary<Storage, List<Storage>> displayData = getData();

            foreach(KeyValuePair<Storage, List<Storage>> item in displayData)
            {
                 var checkBox = form[item.Key.ServerLocation];
                 if (checkBox != null)
                 {
                     foreach (Storage s in item.Value)
                     {
                         if (s.SType.Contains("file"))
                         {
                             myList.Add(s);
                         }
                     }
                 }
            }

            return View(myList);
        }

        [Authorize(Roles = "admin,developer")]
        // GET: /Storage/Create
        public ActionResult Create()
        {

            //changed for displaying user files and public file. Hiding the other private files

            Dictionary<Storage, List<Storage>> displayData = getData();

            List<Storage> folderList = new List<Storage>();

            List<RelativePathModel> relativeDisplay = new List<RelativePathModel>();

            foreach (KeyValuePair<Storage, List<Storage>> item in displayData)
            {
                if (item.Key.SType.Equals("Folder"))
                {
                    if (!folderList.Contains(item.Key))
                    {
                        folderList.Add(item.Key);
                    }
                }
                foreach (Storage s in item.Value)
                {
                    if (s.SType.Equals("Folder"))
                    {
                        folderList.Add(s);
                    }
                }

            }
            foreach (Storage f in folderList)
            {
                string returnString = "";
                string[] file = f.ServerLocation.Split('\\');
                string relativePath = "";
                int count = 0;
                foreach (string content in file)
                {
                    if (content.Contains("UpLoad"))
                    {
                        while (count < file.Length - 1)
                        {
                            count++;
                            relativePath = relativePath + "\\" + file[count];
                        }
                        break;
                    }
                    count++;
                }
                returnString = returnString + relativePath;
                RelativePathModel myObject = new RelativePathModel();
                myObject.RelativePath = returnString;
                myObject.ServerLocation = f.ServerLocation;
                relativeDisplay.Add(myObject);
            }

            return View("CreateRelative",relativeDisplay);


            //List<Storage> folderList = new List<Storage>();
            //foreach( Storage s in db.Storages)
            //{
            //    if(s.SType.Equals("Folder"))
            //    {                 
            //    folderList.Add(s);
            //    }
            //}
            //return View(folderList);
        }

        // POST: /Storage/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin,developer")]
        public ActionResult Create([Bind(Include="Name")] Storage newFolder)
        {
            string selectedValue = Request.Form["Folders"];
            string newName = Request.Form["textbox"];
            if (ModelState.IsValid)
            {
                Storage userFile = (from p in db.Storages
                                    where p.ServerLocation.Equals(selectedValue)
                                    select p).First();

                //modify parent details, so that getData can get new folders from list
                userFile.ContainsFolder = "Y";

                newFolder.Name = newName;
                newFolder.SType = "Folder";
                newFolder.ClientLocation = userFile.ClientLocation+"\\"+newName;
                newFolder.ServerLocation = userFile.ServerLocation + "\\" + newName;
                newFolder.ContainsFile = "N";
                newFolder.ContainsFolder = "N";
                newFolder.Size = "0 bytes";
                newFolder.CreatedOn = DateTime.Now.ToString();
                newFolder.UpdatedOn = DateTime.Now.ToString();
                
                DirectoryInfo di = Directory.CreateDirectory(newFolder.ServerLocation);

                db.Storages.Add(newFolder);
                db.SaveChanges();

                //add link for that user in link table
                MyUsersStoreLink newLink = new MyUsersStoreLink();
                newLink.StoreId = newFolder.Id;

                string id = (from p in db.AspNetUsers
                             where p.UserName == System.Web.HttpContext.Current.User.Identity.Name
                             select p.Id).First().ToString();

                newLink.UserId = id;

                db.MyUsersStoreLinks.Add(newLink);
                db.SaveChanges();

                //set the parent pointer
                userFile.Storages1.Add(newFolder);
                newFolder.Storage11.Add(userFile);
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(newFolder);
        }

        [Authorize(Roles = "admin,developer,naiveUser")]
        public ActionResult Refresh()
        {
            return RedirectToAction("Index");

        }

        // GET: /Storage/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Storage storage = db.Storages.Find(id);
            if (storage == null)
            {
                return HttpNotFound();
            }
            return View(storage);
        }

        // POST: /Storage/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="Id,Name,SType,ClientLocation,ServerLocation,ContainsFile,ContainsFolder,Size,CreatedOn,UpdatedOn,CreatedBy,UpdatedBy,ReadAccess,WriteAccess,IsHidden,IsSharing")] Storage storage)
        {
            if (ModelState.IsValid)
            {
                db.Entry(storage).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(storage);
        }

        // GET: /Storage/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Storage storage = db.Storages.Find(id);
            if (storage == null)
            {
                return HttpNotFound();
            }
            return View(storage);
        }

        // POST: /Storage/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Storage storage = db.Storages.Find(id);
            db.Storages.Remove(storage);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "admin,developer")]
        public ActionResult Upload()
        {
            Dictionary<Storage, List<Storage>> displayData = getData();

            List<Storage> folderList = new List<Storage>();
            List<Storage> fileList = new List<Storage>();
            List<RelativePathModel> relativeDisplayFolder = new List<RelativePathModel>();
            List<RelativePathModel> relativeDisplayFile = new List<RelativePathModel>();

            foreach (KeyValuePair<Storage, List<Storage>> item in displayData)
            {
                if (item.Key.SType.Equals("Folder"))
                {
                    if (!folderList.Contains(item.Key))
                    {
                        folderList.Add(item.Key);
                    }
                }
                foreach (Storage s in item.Value)
                {
                    if (s.SType.Equals("Folder"))
                    {
                        folderList.Add(s);
                    }
                    else
                    {
                        fileList.Add(s);
                    }
                }

            }
            foreach (Storage f in fileList)
            {
                string returnString = "";
                string[] file = f.ServerLocation.Split('\\');
                string relativePath = "";
                int count = 0;
                foreach (string content in file)
                {
                    if (content.Contains("UpLoad"))
                    {
                        while (count < file.Length - 1)
                        {
                            count++;
                            relativePath = relativePath + "\\" + file[count];
                        }
                        break;
                    }
                    count++;
                }
                returnString = returnString + relativePath;
                RelativePathModel myObject = new RelativePathModel();
                myObject.RelativePath = returnString;
                myObject.ServerLocation = f.ServerLocation;
                relativeDisplayFile.Add(myObject);
            }

            foreach (Storage f in folderList)
            {
                string returnString = "";
                string[] file = f.ServerLocation.Split('\\');
                string relativePath = "";
                int count = 0;
                foreach (string content in file)
                {
                    if (content.Contains("UpLoad"))
                    {
                        while (count < file.Length - 1)
                        {
                            count++;
                            relativePath = relativePath + "\\" + file[count];
                        }
                        break;
                    }
                    count++;
                }
                returnString = returnString + relativePath;
                RelativePathModel myObject = new RelativePathModel();
                myObject.RelativePath = returnString;
                myObject.ServerLocation = f.ServerLocation;
                relativeDisplayFolder.Add(myObject);
            }
            ViewBag.fileList = new MultiSelectList(relativeDisplayFile, "ServerLocation", "RelativePath");
            
            return View("UploadRelative", relativeDisplayFolder);

            
            //foreach (Storage s in db.Storages)
            //{
            //    if (s.SType.Equals("Folder"))
            //    {
            //        folderList.Add(s);
            //    }
            //    else
            //    {
            //        fileList.Add(s);
            //    }
            //}
            //ViewBag.fileList = new MultiSelectList(fileList, "ServerLocation", "Name");
            ////ViewBag.fileList = fileList;
            //return View(folderList);
        }

        [HttpPost]
        [Authorize(Roles = "admin,developer")]
        public ActionResult Upload(FormCollection f)
        {
            string folderSelection = Request.Form["Folder"];
            string fileSelection = Request.Form["Files"];
            string clientFile = Request.Form["selection"]; //try to get client absolute path

            string clientPath = "../../Client_WPF/UpLoad/"+clientFile;
            string serverPath = folderSelection + "\\"+clientFile;
            string path = System.Web.HttpContext.Current.Server.MapPath("~\\App_Data\\");

            if (fileSelection != null)
            {
                string[] dependentFiles = fileSelection.Split(',');
                
                try
                {
                    XElement Body = null;
                    if (System.IO.File.Exists(path +"\\dependencies.xml"))
                    {
                        XElement present = XElement.Load(path +"\\dependencies.xml");
                        Body = present;
                    }
                    else
                    {
                        Body = new XElement("BODY");
                    }
                    string pathForXML = System.Web.HttpContext.Current.Server.MapPath("~\\App_Data");
                    XElement file = new XElement("FILE", new XAttribute("ClientPath", clientFile), new XAttribute("ServerPath", serverPath));

                    foreach (string depend in dependentFiles)
                    {
                        XElement dp = new XElement("DEPENDENT", new XAttribute("ServerPath", depend));
                        file.Add(dp);
                    }
                    Body.Add(file);
                    Body.Save(path + "\\dependencies.xml");
                }
                catch (Exception e)
                {
                    
                }
            }

                serverPath = getRelativePath(serverPath);

                urlBase = "http://localhost:32010/api/WebApi";
                openServerUpLoadFile(serverPath);// server path 

                try
                {
                    FileStream up = openClientUpLoadFile(clientFile);

                    const int upBlockSize = 512;
                    byte[] upBlock = new byte[upBlockSize];
                    int bytesRead = upBlockSize;
                    while (bytesRead == upBlockSize)
                    {
                        bytesRead = up.Read(upBlock, 0, upBlockSize);
                        if (bytesRead < upBlockSize)
                        {
                            byte[] temp = new byte[bytesRead];
                            for (int i = 0; i < bytesRead; ++i)
                                temp[i] = upBlock[i];
                            upBlock = temp;
                        }
                        putBlock(upBlock);
                    }
                    closeServerFile();

                    up.Close();
                }
                catch (Exception e) { }

            return RedirectToAction("Index");
        }
        int openServerUpLoadFile(string fileName)
        {
            message = new HttpRequestMessage();
            message.Method = HttpMethod.Get;
            HttpResponseMessage response=null;
            string urlActn = "?fileName=" + fileName + "&open=upload";
            try
            {

                message.RequestUri = new Uri(urlBase + urlActn);
                Task<HttpResponseMessage> task = client.SendAsync(message);
                response= task.Result;
                status = response.ReasonPhrase;
                
            }
            catch (Exception e) { }
            return (int)response.StatusCode;
        }
        FileStream openClientUpLoadFile(string fileName)
        {
            string path = @"C:\Users\Mandeep\Documents\Visual Studio 2013\Projects\FileRepository_Copy\Client_WPF\UpLoad\";

            FileStream up;
            try
            {
                up = new FileStream(path + fileName, FileMode.Open);
            }
            catch
            {
                return null;
            }
            return up;
        }
        //----< post blocks to server >----------------------------------------
        void putBlock(byte[] Block)
        {
            message = new HttpRequestMessage();
            message.Method = HttpMethod.Post;
            message.Content = new ByteArrayContent(Block);
            message.Content.Headers.Add("Content-Type", "application/http;msgtype=request");
            string urlActn = "?blockSize=" + Block.Count().ToString();
            message.RequestUri = new Uri(urlBase + urlActn);
            Task<HttpResponseMessage> task = client.SendAsync(message);
            HttpResponseMessage response = task.Result;
            status = response.ReasonPhrase;
        }
        void closeServerFile()
        {
            message = new HttpRequestMessage();
            message.Method = HttpMethod.Get;
            string urlActn = "?fileName=dontCare.txt&open=close";
            message.RequestUri = new Uri(urlBase + urlActn);
            Task<HttpResponseMessage> task = client.SendAsync(message);
            HttpResponseMessage response = task.Result;
            status = response.ReasonPhrase;
        }

        [Authorize(Roles = "admin,developer,naiveUser")]    
        public ActionResult Download()
        {
            //changed for displaying user files and public file. Hiding the other private files

            Dictionary<Storage, List<Storage>> displayData = getData();

            List<Storage> fileList = new List<Storage>();
            List<RelativePathModel> relativeDisplay = new List<RelativePathModel>();

            foreach (KeyValuePair<Storage, List<Storage>> item in displayData)
            {
                foreach (Storage s in item.Value)
                {
                    if (!s.SType.Equals("Folder"))
                    {
                        fileList.Add(s);
                    }
                }

            }
            foreach (Storage f in fileList)
            {
                string returnString = "";
                string[] file = f.ServerLocation.Split('\\');
                string relativePath = "";
                int count = 0;
                foreach (string content in file)
                {
                    if (content.Contains("UpLoad"))
                    {
                        while (count < file.Length - 1)
                        {
                            count++;
                            relativePath = relativePath + "\\" + file[count];
                        }
                        break;
                    }
                    count++;
                }
                returnString = returnString + relativePath;
                RelativePathModel myObject = new RelativePathModel();
                myObject.RelativePath = returnString;
                myObject.ServerLocation = f.ServerLocation;
                relativeDisplay.Add(myObject);
            }

            return View("DownloadRelative", relativeDisplay);

            //List<Storage> fileList = new List<Storage>();
            //foreach (Storage s in db.Storages)
            //{
            //    if (s.SType.Contains("file"))
            //    {
            //        fileList.Add(s);
            //    }
            //}
            //return View(fileList);
        }

        public string getRelativePath(string fullPath)
        {
            
            string[] file = fullPath.Split('\\');
            string relativePath = "";
            int count = 0;

            foreach (string content in file)
            {
                if (content.Contains("UpLoad"))
                {
                    while (count < file.Length - 1)
                    {
                        count++;
                        relativePath = relativePath + "\\" + file[count];
                    }
                    break;
                }
                count++;
            }
                
            return relativePath;
        }
        
        public List<string> getRelativePath(List<string> files)
        {
            List<string> returnRelPath = new List<string>();

            foreach (string f in files)
            {
                string returnString = "";
                string[] file = f.Split('\\');
                string relativePath = "";
                int count = 0;
                foreach (string content in file)
                {
                    if (content.Contains("UpLoad"))
                    {
                        while (count < file.Length - 1)
                        {
                            count++;
                            relativePath = relativePath + "\\" + file[count];
                        }
                        break;
                    }
                    count++;
                }
                returnString = returnString + relativePath;
                returnRelPath.Add(returnString);
            }
            return returnRelPath;
        }
        [HttpPost]
        public ActionResult Download(FormCollection form)
        {
            List<string> donwloadFiles = new List<string>();
           
            string selectedFile = Request.Form["Files"];
            string checkBox = Request.Form["depend"];
           
            string path = System.Web.HttpContext.Current.Server.MapPath("~\\App_Data\\");
            urlBase = "http://localhost:32010/api/WebApi";

            donwloadFiles.Add(selectedFile);
            if (checkBox != null)
            {
                try
                {
                    XElement readFile = XElement.Load(path + "\\dependencies.xml");

                    var query1 = from c in readFile.Elements("FILE")
                                 select c;

                    foreach (var e2 in query1)
                    {
                        string filenm = e2.Attribute("ServerPath").Value;

                        if (filenm.Equals(selectedFile))
                        {
                            var query4 = from c in e2.Elements("DEPENDENT")
                                         select c;
                            foreach (var s in query4)
                            {
                                string dependFile = s.Attribute("ServerPath").Value;
                                donwloadFiles.Add(dependFile);
                            }
                        }
                    }
                }
                catch (Exception e) { return View("Error"); }
            }

            donwloadFiles = getRelativePath(donwloadFiles);

            foreach (string file in donwloadFiles)
            {
                FileStream down;
                int status = openServerDownLoadFile(file);
                if (status >= 400)
                    return View("Error");
                try
                {
                    down = openClientDownLoadFile(file);
                    while (true)
                    {
                        int blockSize = 512;
                        byte[] Block = getFileBlock(down, blockSize);
                        Console.Write("\n  Response status = {0}", status);
                        Console.Write("\n  received block of size {0} bytes\n", Block.Length);
                        if (Block.Length == 0 || blockSize <= 0)
                            break;
                        down.Write(Block, 0, Block.Length);
                        if (Block.Length < blockSize)    // last block
                            break;
                    }
                    closeServerFile();
                    down.Close();
                }
                catch (Exception e) { return View("Error"); }
            }

            return RedirectToAction("Index");
        }
        byte[] getFileBlock(FileStream down, int blockSize)
        {
            message = new HttpRequestMessage();
            message.Method = HttpMethod.Get;
            string urlActn = "?blockSize=" + blockSize.ToString();
            message.RequestUri = new Uri(urlBase + urlActn);
            Task<HttpResponseMessage> task = client.SendAsync(message);
            HttpResponseMessage response = task.Result;
            Task<byte[]> taskb = response.Content.ReadAsByteArrayAsync();
            byte[] Block = taskb.Result;
            status = response.ReasonPhrase;
            return Block;
        }
        FileStream openClientDownLoadFile(string fileName)
        {
            int count = 0;
            //string path = "../../DownLoad/";
            string path = @"C:\Users\Mandeep\Documents\Visual Studio 2013\Projects\FileRepository_Copy\Client_WPF\DownLoad\";

            string[] file = fileName.Split('\\');
            string test = file.Last();
            foreach (string f in file)
            {
                if (count == file.Length-1)
                {
                    string name = f;
                }
                count++;
            }

            FileStream down;
            try
            {
                down = new FileStream(path+test, FileMode.OpenOrCreate);
            }
            catch
            {
                return null;
            }
            return down;
        }
        int openServerDownLoadFile(string fileName)
        {
            HttpResponseMessage response = null;
            try
            {
                message = new HttpRequestMessage();
                message.Method = HttpMethod.Get;
                string urlActn = "?fileName=" + fileName + "&open=download";
                message.RequestUri = new Uri(urlBase + urlActn);
                Task<HttpResponseMessage> task = client.SendAsync(message);
                response = task.Result;
                status = response.ReasonPhrase;
            }
            catch (Exception e) { }
            return (int)response.StatusCode;
        }

        [Authorize(Roles = "admin,developer")]
        public ActionResult ModifyDependencies()
        {
            //changed for displaying user files and public file. Hiding the other private files

            Dictionary<Storage, List<Storage>> displayData = getData();

            List<Storage> fileList = new List<Storage>();
            List<RelativePathModel> relativeDisplay = new List<RelativePathModel>();

            foreach (KeyValuePair<Storage, List<Storage>> item in displayData)
            {
                foreach (Storage s in item.Value)
                {
                    if (!s.SType.Equals("Folder"))
                    {
                        fileList.Add(s);
                    }
                }

            }
            foreach (Storage f in fileList)
            {
                string returnString = "";
                string[] file = f.ServerLocation.Split('\\');
                string relativePath = "";
                int count = 0;
                foreach (string content in file)
                {
                    if (content.Contains("UpLoad"))
                    {
                        while (count < file.Length - 1)
                        {
                            count++;
                            relativePath = relativePath + "\\" + file[count];
                        }
                        break;
                    }
                    count++;
                }
                returnString = returnString + relativePath;
                RelativePathModel myObject = new RelativePathModel();
                myObject.RelativePath = returnString;
                myObject.ServerLocation = f.ServerLocation;
                relativeDisplay.Add(myObject);
            }

            return View("ModifyDependenciesRelative", relativeDisplay);

            //List<Storage> fileList = new List<Storage>();
            //foreach (Storage s in db.Storages)
            //{
            //    if (s.SType.Contains("file"))
            //    {
            //        fileList.Add(s);
            //    }
            //}

            //return View(fileList);
        }

        public void ShowDependencies()
        {
            string selectedFile = Request.Form["path"];
            string path = System.Web.HttpContext.Current.Server.MapPath("~\\App_Data\\");
            List<string> fileList = new List<string>();

            string returnString="";
            try
            {
                XElement readFile = XElement.Load(path + "\\dependencies.xml");

                var query1 = from c in readFile.Elements("FILE")
                             select c;

                foreach (var e2 in query1)
                {
                    string filenm = e2.Attribute("ServerPath").Value;

                    if (filenm.Equals(selectedFile))
                    {
                        var query4 = from c in e2.Elements("DEPENDENT")
                                     select c;
                        foreach (var s in query4)
                        {
                            string dependFile = s.Attribute("ServerPath").Value;
                            string[] file = dependFile.Split('\\');
                            string test = file.Last();
                            string relativePath = ""; 
                            int count = 0;
                            foreach (string f in file)
                            {
                                if (f.Contains("UpLoad"))
                                {
                                    while (count < file.Length-1)
                                    {
                                        count++;
                                        relativePath = relativePath +"/"+ file[count];
                                    }
                                    break;
                                }
                                
                                count++;
                            }
                            returnString = returnString + relativePath + Environment.NewLine;
                            fileList.Add(dependFile);
                        }
                    }
                }
            }
            catch (Exception e) { }
            Response.Write(returnString);
            //ViewBag.fileList = new MultiSelectList(returnString, "ServerLocation", "Name");
        }

        [Authorize(Roles = "admin,developer")]
        public ActionResult AddDependencies()
        {
            //changed for displaying user files and public file. Hiding the other private files

            Dictionary<Storage, List<Storage>> displayData = getData();

            List<Storage> fileList = new List<Storage>();
            List<RelativePathModel> relativeDisplay = new List<RelativePathModel>();

            foreach (KeyValuePair<Storage, List<Storage>> item in displayData)
            {
                foreach (Storage s in item.Value)
                {
                    if (!s.SType.Equals("Folder"))
                    {
                        fileList.Add(s);
                    }
                }

            }
            foreach (Storage f in fileList)
            {
                string returnString = "";
                string[] file = f.ServerLocation.Split('\\');
                string relativePath = "";
                int count = 0;
                foreach (string content in file)
                {
                    if (content.Contains("UpLoad"))
                    {
                        while (count < file.Length - 1)
                        {
                            count++;
                            relativePath = relativePath + "\\" + file[count];
                        }
                        break;
                    }
                    count++;
                }
                returnString = returnString + relativePath;
                RelativePathModel myObject = new RelativePathModel();
                myObject.RelativePath = returnString;
                myObject.ServerLocation = f.ServerLocation;
                relativeDisplay.Add(myObject);
            }

            ViewBag.fileList = new MultiSelectList(relativeDisplay, "ServerLocation", "RelativePath");

            return View("AddDepdendenciesRelative", relativeDisplay);

            //List<Storage> fileList = new List<Storage>();
            //foreach (Storage s in db.Storages)
            //{
            //    if (s.SType.Contains("file"))
            //    {
            //        fileList.Add(s);
            //    }
            //}
            //ViewBag.fileList = new MultiSelectList(fileList, "ServerLocation", "Name");
            //return View(fileList);

        }
        
        [HttpPost]
        [Authorize(Roles = "admin,developer")]
        public ActionResult AddDependencies(FormCollection form)
        {
            string fileSelection = Request.Form["AllFiles"];
            string parentFile = Request.Form["Files"];
            bool toAdd=true;
            string path = System.Web.HttpContext.Current.Server.MapPath("~\\App_Data\\");

            if (fileSelection != null)
            {
                string[] dependentFiles = fileSelection.Split(',');

                try
                {
                    XElement Body = null;
                    XElement file = null;
                    if (System.IO.File.Exists(path + "\\dependencies.xml"))
                    {
                        XElement present = XElement.Load(path + "\\dependencies.xml");
                        Body = present;
                    }
                    else
                    {
                        Body = new XElement("BODY");
                    }

                    var query1 = from c in Body.Elements("FILE")
                                 select c;

                    foreach (var e2 in query1)
                    {
                        string filenm = e2.Attribute("ServerPath").Value;

                        if (filenm.Equals(parentFile))
                        {
                            toAdd = false;
                            file = e2;
                        }
                    }
                     
                    if(toAdd)
                        file = new XElement("FILE", new XAttribute("ServerPath", parentFile));

                    foreach (string depend in dependentFiles)
                    {
                        XElement dp = new XElement("DEPENDENT", new XAttribute("ServerPath", depend));
                        file.Add(dp);
                    }
                    if(toAdd)
                        Body.Add(file);
                    Body.Save(path + "\\dependencies.xml");
                }
                catch (Exception e){}   
            }
            return RedirectToAction("Index");
        }
        [Authorize(Roles = "admin,developer")]
        public ActionResult RemoveDependencies()
        {
            //changed for displaying user files and public file. Hiding the other private files

            Dictionary<Storage, List<Storage>> displayData = getData();

            List<Storage> fileList = new List<Storage>();
            List<RelativePathModel> relativeDisplay = new List<RelativePathModel>();
            List<Storage> emptyList = new List<Storage>();

            foreach (KeyValuePair<Storage, List<Storage>> item in displayData)
            {
                foreach (Storage s in item.Value)
                {
                    if (!s.SType.Equals("Folder"))
                    {
                        fileList.Add(s);
                    }
                }

            }
            foreach (Storage f in fileList)
            {
                string returnString = "";
                string[] file = f.ServerLocation.Split('\\');
                string relativePath = "";
                int count = 0;
                foreach (string content in file)
                {
                    if (content.Contains("UpLoad"))
                    {
                        while (count < file.Length - 1)
                        {
                            count++;
                            relativePath = relativePath + "\\" + file[count];
                        }
                        break;
                    }
                    count++;
                }
                returnString = returnString + relativePath;
                RelativePathModel myObject = new RelativePathModel();
                myObject.RelativePath = returnString;
                myObject.ServerLocation = f.ServerLocation;
                relativeDisplay.Add(myObject);
            }

            ViewBag.fileList = new MultiSelectList(emptyList, "ServerLocation", "RelativePath");

            return View("RemoveDependenciesRelative", relativeDisplay);

            //List<Storage> fileList = new List<Storage>();
            //List<Storage> emptyList = new List<Storage>();
            //foreach (Storage s in db.Storages)
            //{
            //    if (s.SType.Contains("file"))
            //    {
            //        fileList.Add(s);
            //    }
            //}
            //ViewBag.fileList = new MultiSelectList(emptyList, "ServerLocation", "Name");
            //return View(fileList);

        }

        [HttpPost]
        [Authorize(Roles = "admin,developer")]
        public ActionResult RemoveDependencies(FormCollection form)
        {
            string fileSelection = Request.Form["DependingFile"];
            string parentFile = Request.Form["ParentFile"];
            string path = System.Web.HttpContext.Current.Server.MapPath("~\\App_Data\\");

            if (fileSelection != null)
            {
                string[] dependentFiles = fileSelection.Split(',');

                try
                {
                    XElement readFile = null;
                    if (System.IO.File.Exists(path + "\\dependencies.xml"))
                    {
                        XElement present = XElement.Load(path + "\\dependencies.xml");
                        readFile = present;
                    }
                    else
                    {
                        return View("NoDependentFile");
                    }
                    var query1 = from c in readFile.Elements("FILE")
                                 select c;

                    foreach (var e2 in query1)
                    {
                        string filenm = e2.Attribute("ServerPath").Value;

                        if (filenm.Equals(parentFile))
                        {
                            e2.Elements("DEPENDENT").
                                Where(y => dependentFiles.Contains(y.Attribute("ServerPath").Value)).Remove();

                            //var query4 = from c in e2.Elements("DEPENDENT")
                            //             select c;

                            //foreach (var s in query4)
                            //{
                            //    string dependFile = s.Attribute("ServerPath").Value;
                            //    foreach (string d in dependentFiles)
                            //    {
                            //        if (d.Equals(dependFile))
                            //        {
                            //            s.Remove();
                            //        }
                            //    }
                            //}
                        }
                    }
                    readFile.Save(path + "\\dependencies.xml");
                }
                catch (Exception e)
                {
                    return View("Error");
                }
            }

            return RedirectToAction("Index");
        }

        //to show the parent file withe the dependent files
        [HttpPost]
        [Authorize(Roles = "admin,developer")]
        public ActionResult CustomShowDepend()
        {
            string selectedFile = Request.Form["CustomFile"];
            string path = System.Web.HttpContext.Current.Server.MapPath("~\\App_Data\\");
            List<Storage> fileList = new List<Storage>();
            List<Storage> fileListCustom = new List<Storage>();
            List<RelativePathModel> relativeDisplay = new List<RelativePathModel>();
            List<RelativePathModel> fileListCustomRelative = new List<RelativePathModel>();
            
            try
            {
                XElement readFile = XElement.Load(path + "\\dependencies.xml");

                var query1 = from c in readFile.Elements("FILE")
                             select c;

                foreach (var e2 in query1)
                {
                    string filenm = e2.Attribute("ServerPath").Value;

                    if (filenm.Equals(selectedFile))
                    {
                        var query4 = from c in e2.Elements("DEPENDENT")
                                     select c;
                        foreach (var s in query4)
                        {
                            
                            string dependFile = s.Attribute("ServerPath").Value;

                            Storage storeObject = (from p in db.Storages
                                                    where p.ServerLocation.Equals(dependFile)
                                                   select p).First();
                                                       
                            fileList.Add(storeObject);
                        }
                    }
                }
            }
            catch (Exception e) { return View("Error"); }
            try
            {

                Storage selectedObject = (from p in db.Storages
                                          where p.ServerLocation.Equals(selectedFile)
                                          select p).First();
                fileListCustom.Add(selectedObject);
            }
            catch (Exception e){ }

            foreach (Storage f in fileListCustom)
            {
                string returnString = "";
                string[] file = f.ServerLocation.Split('\\');
                string relativePath = "";
                int count = 0;
                foreach (string content in file)
                {
                    if (content.Contains("UpLoad"))
                    {
                        while (count < file.Length - 1)
                        {
                            count++;
                            relativePath = relativePath + "\\" + file[count];
                        }
                        break;
                    }
                    count++;
                }
                returnString = returnString + relativePath;
                RelativePathModel myObject = new RelativePathModel();
                myObject.RelativePath = returnString;
                myObject.ServerLocation = f.ServerLocation;
                fileListCustomRelative.Add(myObject);
            }
            foreach (Storage f in fileList)
            {
                string returnString = "";
                string[] file = f.ServerLocation.Split('\\');
                string relativePath = "";
                int count = 0;
                foreach (string content in file)
                {
                    if (content.Contains("UpLoad"))
                    {
                        while (count < file.Length - 1)
                        {
                            count++;
                            relativePath = relativePath + "\\" + file[count];
                        }
                        break;
                    }
                    count++;
                }
                returnString = returnString + relativePath;
                RelativePathModel myObject = new RelativePathModel();
                myObject.RelativePath = returnString;
                myObject.ServerLocation = f.ServerLocation;
                relativeDisplay.Add(myObject);
            }

            ViewBag.fileList = new MultiSelectList(relativeDisplay, "ServerLocation", "RelativePath");
           
            return View("RemoveDependenciesRelative", fileListCustomRelative);
        }

        [Authorize(Roles = "admin")]
        public ActionResult Rename()
        {
            //changed for displaying user files and public file. Hiding the other private files

            string pub=System.Web.HttpContext.Current.Server.MapPath("~\\App_Data\\UpLoad\\Public");
            string priv = System.Web.HttpContext.Current.Server.MapPath("~\\App_Data\\UpLoad\\Private");
            Dictionary<Storage, List<Storage>> displayData = getData();

            List<Storage> folderList = new List<Storage>();

            List<RelativePathModel> relativeDisplay = new List<RelativePathModel>();

            foreach (KeyValuePair<Storage, List<Storage>> item in displayData)
            {
                if (item.Key.SType.Equals("Folder"))
                {
                    if (!folderList.Contains(item.Key) && !item.Key.ServerLocation.Equals(pub) && !item.Key.ServerLocation.Equals(priv))
                    {
                        folderList.Add(item.Key);
                    }
                }
                foreach (Storage s in item.Value)
                {
                    if (s.SType.Equals("Folder"))
                    {
                        folderList.Add(s);
                    }
                }

            }
            foreach (Storage f in folderList)
            {
                string returnString = "";
                string[] file = f.ServerLocation.Split('\\');
                string relativePath = "";
                int count = 0;
                foreach (string content in file)
                {
                    if (content.Contains("UpLoad"))
                    {
                        while (count < file.Length - 1)
                        {
                            count++;
                            relativePath = relativePath + "\\" + file[count];
                        }
                        break;
                    }
                    count++;
                }
                returnString = returnString + relativePath;
                RelativePathModel myObject = new RelativePathModel();
                myObject.RelativePath = returnString;
                myObject.ServerLocation = f.ServerLocation;
                relativeDisplay.Add(myObject);
            }

            return View("RenameRelative", relativeDisplay);

            //List<Storage> folderList = new List<Storage>();
           
            //foreach (Storage s in db.Storages)
            //{
            //    if (s.SType.Equals("Folder"))
            //    {
            //        folderList.Add(s);
            //    }
            //}
         
            //return View(folderList);
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public ActionResult Rename(FormCollection form)
        {
            string folderPath = Request.Form["selectFolder"];
            string newName = Request.Form["newName"];

            try
            {
                Storage renameObject = (from p in db.Storages
                                        where p.ServerLocation.Equals(folderPath)
                                        select p).First();

                renameRecurse(renameObject, folderPath, newName);

                int lastIndexOfSep = renameObject.ServerLocation.LastIndexOf('\\');

                string unmodifiedPath = renameObject.ServerLocation.Substring(0, lastIndexOfSep);

                string newPath = unmodifiedPath + "\\" + newName;
                renameObject.ServerLocation = newPath;
                renameObject.ClientLocation = newPath;
                renameObject.Name = newName;
                db.SaveChanges();

                Directory.Move(folderPath, newPath);
            }
            catch (Exception e) { return View("Error"); }

            return RedirectToAction("Index");
            
        }

        //rename the subfiles and sub folders path to newName
        public void renameRecurse(Storage renameObject, string folderPath, string newName)
        {
            List<Storage> folders = renameObject.Storages1.ToList();
            List<Storage> files = renameObject.Storages2.ToList();

            folders.AddRange(files);

            int length = folderPath.Length;

            foreach (Storage s in folders)
            {
                if (s.SType.Equals("Folder"))
                {
                    renameRecurse(s, folderPath, newName);
                }

                if (s.ServerLocation.Contains(folderPath))
                {
                    string firstPath = s.ServerLocation.Substring(0, length);
                    int lastIndexOfSep = firstPath.LastIndexOf('\\');

                    string unmodifiedPath = firstPath.Substring(0, lastIndexOfSep);

                    string remainingPath = s.ServerLocation.Substring(length+1);

                    string newPath = unmodifiedPath + "\\"+newName +"\\"+ remainingPath;
                    s.ServerLocation = newPath;
                    s.ClientLocation = newPath;
                    db.SaveChanges();

                }
            }
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
