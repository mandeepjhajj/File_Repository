using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.IO;
using System.Windows.Forms;
using System.Net.Http.Headers;

namespace Client_WPF
{

    #region Model Defination at client side

    public partial class MyUsersStoreLink
    {
        public string UserId { get; set; }
        public int StoreId { get; set; }

        public virtual Storage Storage { get; set; }
    }
    public partial class Storage
    {
        public Storage()
        {
            this.MyUsersStoreLinks = new HashSet<MyUsersStoreLink>();
            this.Storage11 = new HashSet<Storage>();
            this.Storages1 = new HashSet<Storage>();
            this.Storage12 = new HashSet<Storage>();
            this.Storages2 = new HashSet<Storage>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string SType { get; set; }
        public string ClientLocation { get; set; }
        public string ServerLocation { get; set; }
        public string ContainsFile { get; set; }
        public string ContainsFolder { get; set; }
        public string Size { get; set; }
        public string CreatedOn { get; set; }
        public string UpdatedOn { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public string ReadAccess { get; set; }
        public string WriteAccess { get; set; }
        public string IsHidden { get; set; }
        public string IsSharing { get; set; }

        public virtual ICollection<MyUsersStoreLink> MyUsersStoreLinks { get; set; }
        public virtual ICollection<Storage> Storage11 { get; set; }
        public virtual ICollection<Storage> Storages1 { get; set; }
        public virtual ICollection<Storage> Storage12 { get; set; }
        public virtual ICollection<Storage> Storages2 { get; set; }
    }

    #endregion


    #region Data Binding to WPF

    public class Folderlist
    {
        public string relativePath { get; set; }
        public string serverPath { get; set; }
    }

    public class Filelist
    {
        public string relativePath { get; set; }
        public string serverPath { get; set; }
    }

    #endregion

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private HttpClient client = new HttpClient();
        private HttpRequestMessage message;
        private HttpResponseMessage response = new HttpResponseMessage();

        public string status { get; set; }

        private string urlBase = "http://localhost:32010/api/WebApi";

        IAsyncResult cbResult;
        public MainWindow()
        {
            InitializeComponent();
            one.Visibility = Visibility.Visible;
            two.Visibility = Visibility.Hidden;
            three.Visibility = Visibility.Hidden;
        }


        //----< get list of files available for download >---------------------

        string[] getAvailableFiles()
        {
            message = new HttpRequestMessage();
            message.Method = HttpMethod.Get;
            message.RequestUri = new Uri(urlBase);
            Task<HttpResponseMessage> task = client.SendAsync(message);
            HttpResponseMessage response1 = task.Result;
            response = task.Result;
            status = response.ReasonPhrase;
            string[] files = Newtonsoft.Json.JsonConvert.DeserializeObject<string[]>(response1.Content.ReadAsStringAsync().Result);
            return files;
        }


        //----< open file on server for reading >------------------------------

        int openServerDownLoadFile(string fileName)
        {
            message = new HttpRequestMessage();
            message.Method = HttpMethod.Get;
            string urlActn = "?fileName=" + fileName + "&open=download";
            message.RequestUri = new Uri(urlBase + urlActn);
            Task<HttpResponseMessage> task = client.SendAsync(message);
            HttpResponseMessage response = task.Result;
            status = response.ReasonPhrase;
            return (int)response.StatusCode;
        }

        //----< open file on client for writing >------------------------------

        FileStream openClientDownLoadFile(string fileName)
        {
            string path = "../../DownLoad/";

            string[] splitted = fileName.Split('\\');
            fileName = splitted.Last();
            FileStream down;
            try
            {
                down = new FileStream(path + fileName, FileMode.OpenOrCreate);
            }
            catch
            {
                return null;
            }
            return down;
        }

        //----< read block from server file and write to client file >---------

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
        //----< close FileStream on server and FileStream on client >----------

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


        //----< downLoad File >------------------------------------------------
        /*
         *  Open server file for reading
         *  Open client file for writing
         *  Get blocks from server
         *  Write blocks to local file
         *  Close server file
         *  Close client file
         */
        void downLoadFile(string filename)
        {
            FileStream down;
            int status = openServerDownLoadFile(filename);
            if (status >= 400)
                return;
            down = openClientDownLoadFile(filename);
            try
            {
                while (true)
                {
                    int blockSize = 512;
                    byte[] Block = getFileBlock(down, blockSize);
                    if (Block.Length == 0 || blockSize <= 0)
                        break;
                    down.Write(Block, 0, Block.Length);
                    if (Block.Length < blockSize)    // last block
                        break;
                }
            }
            catch (Exception e) { }
            closeServerFile();
            down.Close();
        }

        //----< open file on server for writing >------------------------------

        int openServerUpLoadFile(string fileName)
        {
            //split the fileName in case of Folder Upload
            string[] splitted = fileName.Split('\\');
            string uploadFoldername = "";

            if (splitted.Length > 1)
            {
                uploadFoldername = splitted[splitted.Length - 2]; // client folder name
                if (folderRename.Text != null)
                    uploadFoldername = folderRename.Text;
                fileName = uploadFoldername + "\\" + splitted[splitted.Length - 1]; // filename
            }

            var serverpath = displayFolder.SelectedItem;
            string path = "", server = "";
            if (serverpath is Folderlist)
            {
                Folderlist t = (Folderlist)serverpath;
                path = t.relativePath;
                server = t.serverPath;
                if (splitted.Count() > 1)
                {
                    Directory.CreateDirectory(server + "\\" + uploadFoldername);
                }
            }

            message = new HttpRequestMessage();
            message.Method = HttpMethod.Get;
            string urlActn = "?fileName=" + path + "\\" + fileName + "&open=upload";
            message.RequestUri = new Uri(urlBase + urlActn);
            Task<HttpResponseMessage> task = client.SendAsync(message);
            HttpResponseMessage response = task.Result;
            status = response.ReasonPhrase;
            return (int)response.StatusCode;

        }

        //----< open file on client for Reading >------------------------------

        FileStream openClientUpLoadFile(string fileName)
        {
            //string path = "../../UpLoad/";

            FileStream up;
            try
            {
                up = new FileStream(fileName, FileMode.Open);
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


        //----< upLoad File >--------------------------------------------------
        /*
         *  Open server file for writing
         *  Open client file for reading
         *  Read blocks from local file
         *  Send blocks to server
         *  Close server file
         *  Close client file
         */
        void upLoadFile(string filename, string clientFullPath)
        {
            openServerUpLoadFile(filename);
            FileStream up = openClientUpLoadFile(clientFullPath);
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

        public List<string> getDependentFileList(string filename)
        {
            message = new HttpRequestMessage();
            message.Method = HttpMethod.Get;
            string urlActn = "?filename=" + filename;
            message.RequestUri = new Uri(urlBase + urlActn);
            var files = new List<string>();
            //added this line
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            try
            {
                Task<HttpResponseMessage> task = client.SendAsync(message);
                HttpResponseMessage response1 = task.Result;
                response = task.Result;
            }
            catch (AggregateException e1) { }

            status = response.ReasonPhrase;
            //List<Storage> folders = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Storage>>(response1.Content.ReadAsStringAsync().Result);

            if (response.IsSuccessStatusCode)
            {
                files = response.Content.ReadAsAsync<List<string>>().Result;
            }
            return files;
        }
        private void download_Click(object sender, RoutedEventArgs e)
        {

            var serverpath = displayFile.SelectedItem;
            List<string> depdendentFiles = new List<string>();

            string path = "", server = "";
            if (serverpath is Filelist)
            {
                Filelist t = (Filelist)serverpath;
                path = t.relativePath;
                server = t.serverPath;
                if (DependencyDownload.IsChecked.Value)
                {
                    depdendentFiles = getDependentFileList(server);
                }
            }

            downLoadFile(path);
            if (depdendentFiles.Count != 0)
            {
                foreach (string file in depdendentFiles)
                {
                    downLoadFile(file);
                }
            }
        }


        // this is of no use, need to delete adterword
        void showPath(string path)
        {
            //textbox1.Text = path;
        }

        void search(string path, string pattern)
        {
            /* called on asynch delegate's thread */
            if (Dispatcher.CheckAccess())
                showPath(path);
            else
                Dispatcher.Invoke(
                  new Action<string>(showPath),
                  System.Windows.Threading.DispatcherPriority.Background,
                  new string[] { path }
                );
            string[] files = System.IO.Directory.GetFiles(path, pattern);
            foreach (string file in files)
            {
                if (Dispatcher.CheckAccess())
                    upLoadFile(file, file);// second parameter is added, need to check if it works
                else
                    Dispatcher.Invoke(
                      new Action<string, string>(upLoadFile),
                      System.Windows.Threading.DispatcherPriority.Background,
                      new string[] { file, file }
                    );
            }
            string[] dirs = System.IO.Directory.GetDirectories(path);
            foreach (string dir in dirs)
                search(dir, pattern);
        }

        private void upload_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog dlg;
            OpenFileDialog openFileDialog;
            if (folderUpload.IsChecked.Value)
            {
                dlg = new FolderBrowserDialog();
                string path = AppDomain.CurrentDomain.BaseDirectory;
                dlg.SelectedPath = path;
                DialogResult result = dlg.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    path = dlg.SelectedPath;
                    string pattern = "*.*";
                    Action<string, string> proc = this.search;
                    cbResult = proc.BeginInvoke(path, pattern, null, null);
                }
            }
            else
            {
                openFileDialog = new OpenFileDialog();
                openFileDialog.Multiselect = true;

                openFileDialog.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
                //openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

                if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    foreach (string filename in openFileDialog.FileNames)
                        upLoadFile((System.IO.Path.GetFileName(filename)), filename);
                }
            }

        }

        private IEnumerable<Storage> getFolder()
        {
            IEnumerable<Storage> folders= null;

             message = new HttpRequestMessage();
            message.Method = HttpMethod.Get;
            message.RequestUri = new Uri(urlBase);
            //added this line
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            Task<HttpResponseMessage> task = client.SendAsync(message);
            HttpResponseMessage response1 = task.Result;
            response = task.Result;
            status = response.ReasonPhrase;
            //List<Storage> folders = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Storage>>(response1.Content.ReadAsStringAsync().Result);

            if (response.IsSuccessStatusCode)
            {
                folders = response.Content.ReadAsAsync<IEnumerable<Storage>>().Result;
            }
            return folders;
        }

        private Task<IEnumerable<Storage>> getFolderList()
        {
            return (Task<IEnumerable<Storage>>)Task.Run(() =>
            {
                return getFolder();
            });
        }

        private async void getFolder_Click(object sender, RoutedEventArgs e)
        {
            displayFolder.Items.Clear();
            
            IEnumerable<Storage> folders = await getFolderList();
            
            foreach (Storage f in folders)
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

                    displayFolder.Items.Add(new Folderlist { relativePath = returnString, serverPath = f.ServerLocation });

                }
        }

        private IEnumerable<Storage> getFile()
        {
            message = new HttpRequestMessage();
            message.Method = HttpMethod.Get;
            string urlActn = "?value=" + true;
            IEnumerable<Storage> files = null;
            message.RequestUri = new Uri(urlBase + urlActn);

            //added this line
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            try
            {
                Task<HttpResponseMessage> task = client.SendAsync(message);
                HttpResponseMessage response1 = task.Result;
                response = task.Result;
            }
            catch (AggregateException e1) { }

            status = response.ReasonPhrase;
            //List<Storage> folders = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Storage>>(response1.Content.ReadAsStringAsync().Result);

            if (response.IsSuccessStatusCode)
            {
                files = response.Content.ReadAsAsync<IEnumerable<Storage>>().Result;
            }
            return files;
        }
        private Task<IEnumerable<Storage>> getFileList()
        {
            return (Task<IEnumerable<Storage>>)Task.Run(() =>
            {
                return getFile();
            });
        }

        private async void getFile_Click(object sender, RoutedEventArgs e)
        {
            displayFile.Items.Clear();

            IEnumerable<Storage> files = await getFileList();
            
            foreach (Storage f in files)
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

                displayFile.Items.Add(new Filelist { relativePath = returnString, serverPath = f.ServerLocation });

            }
        }

        private async void login_Click(object sender, RoutedEventArgs e)
        {
            string username = userName.Text;
            string pass = password.Password;
            
            string id = await login_Async(username, pass);
                if (id.Equals("1"))
                {
                        one.IsEnabled = false;
                        //one.Visibility = Visibility.Visible;
                        two.Visibility = Visibility.Visible;
                        three.Visibility = Visibility.Visible;
                        three.Focus();
                }
                else if(id.Equals("2"))
                {
                        //one.Visibility = Visibility.Visible;

                    one.IsEnabled = false; 
                    two.Visibility = Visibility.Visible;
                    three.Visibility = Visibility.Visible;
                    three.Focus();
                }
                else if (id.Equals("3"))
                {
                    //one.Visibility = Visibility.Visible;
                    one.IsEnabled = false; 
                    two.Visibility = Visibility.Hidden;
                    three.Visibility = Visibility.Visible;
                    three.Focus();
                }
                else
                {
                    one.IsEnabled = true;
                    one.Focus();
                    two.Visibility = Visibility.Hidden;
                    three.Visibility = Visibility.Hidden;
                    userName.Clear();
                    password.Clear();
                    errorMessage.Content = "Invalid username or password";
                }
        }

        private Task<string> login_Async(string username, string pass)
        {
            return (Task<string>)Task.Run(() =>
            {
                return doLogin(username,pass);
            });
        }

        private string doLogin(string username, string pass)
        {
            message = new HttpRequestMessage();
            message.Method = HttpMethod.Get;
            string urlActn = "?userName=" + username + "&password=" + pass + "&value=true";
            message.RequestUri = new Uri(urlBase + urlActn);
            string id = "";
            //added this line
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            try
            {
                Task<HttpResponseMessage> task = client.SendAsync(message);
                HttpResponseMessage response1 = task.Result;
                response = task.Result;
            }
            catch (AggregateException e1) { }

            status = response.ReasonPhrase;
            //List<Storage> folders = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Storage>>(response1.Content.ReadAsStringAsync().Result);

            if (response.IsSuccessStatusCode)
            {
                id = response.Content.ReadAsAsync<string>().Result;
            }
            return id;
        }
        private void logout_Click(object sender, RoutedEventArgs e)
        {
            one.IsEnabled = true;
            one.Focus();
            two.Visibility = Visibility.Hidden;
            three.Visibility = Visibility.Hidden;
            errorMessage.Content = "Logged Out";
            displayFile.Items.Clear();
            displayFolder.Items.Clear();
            userName.Clear();
            password.Clear();
        }
    }
}

