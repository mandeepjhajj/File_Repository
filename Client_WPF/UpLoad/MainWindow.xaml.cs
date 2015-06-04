/////////////////////////////////////////////////////////////////////////
// mainWindow.xaml.cs - This package is entry point for the GUI        //
//                      It will call the controller package to decide  //
//                      the action                                     //
//                                                                     //
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
 * This module defines the following class:
 *   MainWindow
 *   
 * Public Interface
 * ================
 * Provides the Button click events, of WPF form
 * 
 */
/*
 * Build Process
 * =============
 * Required Files: MainWindow.xaml.cs ControllerWPF.cs, BlockingQueue.cs,IService1.cs,Service1.cs
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
using System.Threading;
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

namespace DependencyAnalyzer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        int MaxMsgCount = 100;
        WPFController flow;
        public MainWindow()
        {
            InitializeComponent();
            Title = "Dependency Analyzer";
            flow  = new WPFController();
            AServer.IsEnabled = true;
        }
        public string portvalue
        {
            get;
            set;
        }
        public string hostvalue
        {
            get;
            set;
        }   

        //Action when used will select the connect button
        private void AddServer(object sender, RoutedEventArgs e)
        {
           
            portvalue = port.Text;
            hostvalue = hostname.Text;
            Task.Run(() => { flow.connectTask(hostvalue,portvalue); });                  
        }
        //Action when used will select the listen button
        private void ListenIT(object sender, RoutedEventArgs e)
        {
            portvalue = port.Text;
            hostvalue = hostname.Text;
            Task.Run(() => { flow.listenTask(hostvalue, portvalue); });
        }
        //Action when used will select the Getprojects button
        private void GetProjects(object sender, RoutedEventArgs e)
        {
            flow.MakeMessage(2);
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            flow.MakeMessage(0);
        }
        //Action when used will select the Add to list button
        private void ServerList(object sender, RoutedEventArgs e)
        {
            flow.MakeMessage(1);
        }
        //Action when used will select the Get type button
        private void GetDependencies(object sender, RoutedEventArgs e)
        {
            
            flow.MakeMessage(3);
        }

        //Action when used will select the get dependencies button
        private void GetDepend(object sender, RoutedEventArgs e)
        {
            flow.MakeMessage(4);
        }                
    }
}
