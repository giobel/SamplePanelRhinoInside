using Rhino;
using Rhino.Runtime.InProcess;
using System;
using System.Windows;
using System.Windows.Interop;

namespace WpfApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private WindowInteropHelper _interop;
        private RhinoCore _rhino_core;
        private IntPtr _windowHandle;

        public MainWindow()
        {
            InitializeComponent();

            ContentRendered += MainWindow_ContentRendered;

            LocationChanged += new EventHandler(Window_LocationChanged);

            Closing += MainWindow_Closing;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Rhino.Geometry.Point3d signalBBoxCenter = new Rhino.Geometry.Point3d(0, 0, 0);
            double radius = 4;
            Rhino.Geometry.Sphere sphere = new Rhino.Geometry.Sphere(signalBBoxCenter, radius);

            Rhino.RhinoDoc.ActiveDoc.Objects.AddSphere(sphere);
            Rhino.RhinoDoc.ActiveDoc.Views.Redraw();
            //RhinoApp.RunScript("!_Layer", "test", true);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            RhinoApp.RunScript("!_Options", "test", true);
        }

        private void HideSkin()
        {
            Rhino.ApplicationSettings.AppearanceSettings.MenuVisible = false;
            Rhino.ApplicationSettings.AppearanceSettings.ShowSideBar = false;
            Rhino.ApplicationSettings.AppearanceSettings.ShowOsnapBar = false;
            Rhino.ApplicationSettings.AppearanceSettings.ShowStatusBar = false;
            Rhino.ApplicationSettings.AppearanceSettings.ShowFullPathInTitleBar = false;

            for (int i = 0; i < RhinoApp.ToolbarFiles.Count; i++)
            {
                RhinoApp.ToolbarFiles[i].Close(false);
            }

            foreach (var panel in Rhino.UI.Panels.GetOpenPanelIds())
            {
                Rhino.UI.Panels.ClosePanel(panel);
            }


            var view = Rhino.RhinoDoc.ActiveDoc.Views.ActiveView;

            int index = Rhino.RhinoDoc.ActiveDoc.NamedViews.FindByName("Perspective");

            Rhino.RhinoDoc.ActiveDoc.NamedViews.Restore(index, view.ActiveViewport);

            view.Maximized = true;
            view.Redraw();

            RhinoApp.RunScript("-_CommandPrompt _Show _No _Enter", false);
            //Rhino.ApplicationSettings.AppearanceSettings.CommandPromptPosition = Rhino.ApplicationSettings.CommandPromptPosition.Hidden;
        }

        private void Button_Show_Rhino(object sender, RoutedEventArgs e)
        {





        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            WindowHelper.ShowWindow(Rhino.RhinoApp.MainWindowHandle(), WindowHelper.WindowShowStyle.Hide);
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

            
            Rhino.ApplicationSettings.AppearanceSettings.MenuVisible = true;
            Rhino.ApplicationSettings.AppearanceSettings.ShowSideBar = true;
            Rhino.ApplicationSettings.AppearanceSettings.ShowOsnapBar = true;
            Rhino.ApplicationSettings.AppearanceSettings.ShowStatusBar = true;
            Rhino.ApplicationSettings.AppearanceSettings.ShowFullPathInTitleBar = true;

            //Rhino.ApplicationSettings.AppearanceSettings.GetDefaultState();
            //Rhino.ApplicationSettings.AppearanceSettings.RestoreDefaults();

            Rhino.ApplicationSettings.AppearanceSettings.CommandPromptPosition = Rhino.ApplicationSettings.CommandPromptPosition.Top;

            


            RhinoApp.Exit();
            _rhino_core?.Dispose();
        }

        private void MainWindow_ContentRendered(object sender, EventArgs e)
        {
            _interop = new WindowInteropHelper(this);
            _windowHandle = _interop.Handle;
            _rhino_core = new RhinoCore(new[] { "/nosplash" },
                Rhino.Runtime.InProcess.WindowStyle.Hidden, _windowHandle);

            

            HideSkin();

            WindowHelper.ShowWindow(Rhino.RhinoApp.MainWindowHandle(), WindowHelper.WindowShowStyle.ShowNormal);

            WindowHelper.MoveWindow(Rhino.RhinoApp.MainWindowHandle(), 435, 100, 500, 500, false);

            string title = WindowHelper.GetCaptionOfWindow(Rhino.RhinoApp.MainWindowHandle());

            WindowHelper.SetWindowText(RhinoApp.MainWindowHandle(), "Ciao belli");

            RhinoApp.RunScript("-_CommandPrompt _Show _No _Enter", false);
            
        }

        private void Window_LocationChanged(object sender, EventArgs e)
        {
            int top = Convert.ToInt32(this.Top);
            int left = Convert.ToInt32(this.Left);

            try
            {
                //WindowHelper.SendMessage(RhinoApp.MainWindowHandle(), WindowHelper.sette WM_SETTEXT, 0, (LPARAM)"C:\\Documents and Settings\\Blah\\Desktop\\myText.txt");

                

                WindowHelper.MoveWindow(Rhino.RhinoApp.MainWindowHandle(), 435+left-100, top, 500, 500, false);
            }
            catch { }

        }

    }
}