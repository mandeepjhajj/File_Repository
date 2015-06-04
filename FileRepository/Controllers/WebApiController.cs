using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using FileRepository.Models;
using System.Net.Http.Headers;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Linq;

using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;

namespace FileRepository.Controllers
{
    public class WebApiController : ApiController
    {
        private Entities db = new Entities();

        // GET api/WebApi

        //public IQueryable<Storage> GetStorages()
        //{
        //    return db.Storages;
        //}

        public Dictionary<Storage, List<Storage>> userFilePrivate(int i)
        {
            string path = System.Web.HttpContext.Current.Server.MapPath("~\\App_Data\\UpLoad\\Private");
            IEnumerable<Storage> userPrivateFile = new List<Storage>();

            Dictionary<Storage, List<Storage>> myDict = new Dictionary<Storage, List<Storage>>();
            List<Storage> fList = new List<Storage>();
            List<Storage> sList = new List<Storage>();

            bool addDict = false;
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
            return myDict;
        }

        public Dictionary<Storage, List<Storage>> publicFiles(int i)
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
            return myDict;
        }

        public Dictionary<Storage, List<Storage>> getData(int i)
        {
            Dictionary<Storage, List<Storage>> privateDict = new Dictionary<Storage, List<Storage>>();
            Dictionary<Storage, List<Storage>> publicDict = new Dictionary<Storage, List<Storage>>();
            Dictionary<Storage, List<Storage>> combineDict = new Dictionary<Storage, List<Storage>>();
          
            //private files not working
            //privateDict = userFilePrivate(i);
            publicDict = publicFiles(i);
            return publicDict;
            //if (privateDict.Count != 0 && publicDict.Count == 0)
            //{
            //    combineDict = privateDict;
            //}
            //if (privateDict.Count == 0 && publicDict.Count != 0)
            //{
            //    combineDict = publicDict;
            //}

            //if (privateDict.Count != 0 && publicDict.Count != 0)
            //{
            //    foreach (var c in publicDict)
            //    {
            //        privateDict.Add(c.Key, c.Value);
            //    }
            //}

            //return privateDict;
        }

        //----< GET api/File - get list of available files >---------------
        public IEnumerable<Storage> Get()
        {
            // available files

            List<Storage> folderList = new List<Storage>();
            Dictionary<Storage, List<Storage>> data = getData(1);

            foreach (KeyValuePair<Storage, List<Storage>> item in data)
            {
                folderList.Add(item.Key);
            }
            //HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, folderList);
            //return response;

            return folderList;

            //string path = System.Web.HttpContext.Current.Server.MapPath("~\\App_Data\\DownLoad");
            //string[] files = Directory.GetFiles(path);

            //for (int i = 0; i < files.Length; ++i)
            //    files[i] = Path.GetFileName(files[i]);
            //return files;
        }

        //return the file list
        public IEnumerable<Storage> Get(bool value)
        {
            // available files

            List<Storage> fileList = new List<Storage>();
            Dictionary<Storage, List<Storage>> data = getData(1);

            foreach (KeyValuePair<Storage, List<Storage>> item in data)
            {
                foreach(Storage s in item.Value)
                {
                    if (!s.SType.Equals("Folder"))
                    {
                        fileList.Add(s);
                    }
                }
            }
            return fileList;
        }

        // get the dependent files of given file
        public IEnumerable<string> Get(string filename)
        {
            
            string path = System.Web.HttpContext.Current.Server.MapPath("~\\App_Data\\");
            List<string> fileList = new List<string>();

            string returnString = "";
            try
            {
                XElement readFile = XElement.Load(path + "\\dependencies.xml");

                var query1 = from c in readFile.Elements("FILE")
                             select c;

                foreach (var e2 in query1)
                {
                    string filenm = e2.Attribute("ServerPath").Value;

                    if (filenm.Equals(filename))
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
                                    while (count < file.Length - 1)
                                    {
                                        count++;
                                        relativePath = relativePath + "\\" + file[count];
                                    }
                                    break;
                                }

                                count++;
                            }
                            fileList.Add(relativePath);
                        }
                    }
                }
            }
            catch (Exception e) { }
            return fileList;
            
        }

        // GET api/WebApi/5
        //[ResponseType(typeof(Storage))]
        //public IHttpActionResult GetStorage(int id)
        //{
        //    Storage storage = db.Storages.Find(id);
        //    if (storage == null)
        //    {
        //        return NotFound();
        //    }

        //    return Ok(storage);
        //}


        //----< GET api/File?fileName=foobar.txt&open=true >---------------
        //----< attempt to open or close FileStream >----------------------

        public HttpResponseMessage Get(string fileName, string open)
        {
            string sessionId;
            var response = new HttpResponseMessage();
            Models.Session session = new Models.Session();

            CookieHeaderValue cookie = Request.Headers.GetCookies("session-id").FirstOrDefault();
            if (cookie == null)
            {
                sessionId = session.incrSessionId();
                cookie = new CookieHeaderValue("session-id", sessionId);
                cookie.Expires = DateTimeOffset.Now.AddDays(1);
                cookie.Domain = Request.RequestUri.Host;
                cookie.Path = "/";
            }
            else
            {
                sessionId = cookie["session-id"].Value;
            }
            try
            {
                FileStream fs;
                string path = System.Web.HttpContext.Current.Server.MapPath("~\\App_Data\\");
                if (open == "download")  // attempt to open requested fileName
                {
                    path = path + "UpLoad";
                    string currentFileSpec = path + fileName;

                    fs = new FileStream(currentFileSpec, FileMode.Open); //This is for WPF download
                    //fs = new FileStream(fileName, FileMode.Open); //This is for website
                    session.saveStream(fs, sessionId);
                }
                else if (open == "upload")
                {
                    path = path + "UpLoad";
                    string currentFileSpec = path + fileName;

                    fs = new FileStream(currentFileSpec, FileMode.OpenOrCreate); // This is for upload from WPF
                    //fs = new FileStream(fileName, FileMode.OpenOrCreate);   This is for upload from website
                    session.saveStream(fs, sessionId);
                }
                else  // close FileStream
                {
                    fs = session.getStream(sessionId);
                    session.removeStream(sessionId);
                    fs.Close();
                }
                response.StatusCode = (HttpStatusCode)200;
            }
            catch
            {
                response.StatusCode = (HttpStatusCode)400;
            }
          finally  // return cookie to save current sessionId
            {
                
                response.Headers.AddCookies(new CookieHeaderValue[] { cookie });
            }
            return response;
        }


        //----< GET api/File?blockSize=2048 - get a block of bytes >-------

        public HttpResponseMessage Get(int blockSize)
        {
            // get FileStream and read block

            Models.Session session = new Models.Session();
            CookieHeaderValue cookie = Request.Headers.GetCookies("session-id").FirstOrDefault();
            string sessionId = cookie["session-id"].Value;
            FileStream down = session.getStream(sessionId);
            byte[] Block = new byte[blockSize];
            int bytesRead = down.Read(Block, 0, blockSize);
            if (bytesRead < blockSize)  // compress block
            {
                byte[] returnBlock = new byte[bytesRead];
                for (int i = 0; i < bytesRead; ++i)
                    returnBlock[i] = Block[i];
                Block = returnBlock;
            }
            // make response message containing block and cookie

            HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.OK);
            message.Headers.AddCookies(new CookieHeaderValue[] { cookie });
            message.Content = new ByteArrayContent(Block);
            return message;
        }

         // POST api/file
        public HttpResponseMessage Post(int blockSize)
        {
          Task<byte[]> task = Request.Content.ReadAsByteArrayAsync();
          byte[] Block = task.Result;
          Models.Session session = new Models.Session();
          CookieHeaderValue cookie = Request.Headers.GetCookies("session-id").FirstOrDefault();
          string sessionId = cookie["session-id"].Value;
          FileStream up = session.getStream(sessionId);
          up.Write(Block, 0, Block.Count());
          HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.OK);
          message.Headers.AddCookies(new CookieHeaderValue[] { cookie });
          return message;
        }

        // PUT api/file/5
        public void Put(int id, [FromBody]string value)
        {
          string debug = "debug";
        }

        // DELETE api/file/5
        public void Delete(int id)
        {
          string debug = "debug";
        }
    
        //// PUT api/WebApi/5
        //public IHttpActionResult PutStorage(int id, Storage storage)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    if (id != storage.Id)
        //    {
        //        return BadRequest();
        //    }

        //    db.Entry(storage).State = EntityState.Modified;

        //    try
        //    {
        //        db.SaveChanges();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!StorageExists(id))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return StatusCode(HttpStatusCode.NoContent);
        //}

        // POST api/WebApi
        //[ResponseType(typeof(Storage))]
        //public IHttpActionResult PostStorage(Storage storage)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    db.Storages.Add(storage);
        //    db.SaveChanges();

        //    return CreatedAtRoute("DefaultApi", new { id = storage.Id }, storage);
        //}

        //// DELETE api/WebApi/5
        //[ResponseType(typeof(Storage))]
        //public IHttpActionResult DeleteStorage(int id)
        //{
        //    Storage storage = db.Storages.Find(id);
        //    if (storage == null)
        //    {
        //        return NotFound();
        //    }

        //    db.Storages.Remove(storage);
        //    db.SaveChanges();

        //    return Ok(storage);
        //}

        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing)
        //    {
        //        db.Dispose();
        //    }
        //    base.Dispose(disposing);
        //}

        //private bool StorageExists(int id)
        //{
        //    return db.Storages.Count(e => e.Id == id) > 0;
        //}


        public string Get(string userName, string password,bool value)
        {

            UserManager<ApplicationUser> userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            string role="";
            var user =  userManager.FindAsync(userName, password);
            if (user.Result != null)
            {
                try
                {
                    var roleId = (from p in db.AspNetUsers
                                  where p.Id == user.Result.Id
                                  select p.AspNetRoles).ToList();
                    foreach (var i in roleId)
                    {
                        role = i.First().Id;
                    }
                }
                catch (Exception e) { }
                
            }
            else
            {
                role = "0";
            }
            
             return role;
            
        }
    }
}