using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;

namespace WpfApp
{
    public static class WindowHelper
    {
        public const int HWND_TOPMOST = -1;

        // 0x0080
        public const int SWP_DRAWFRAME = 32;

        // 0x0040
        public const int SWP_HIDEWINDOW = 128;

        // 0x0004
        public const int SWP_NOACTIVATE = 16;

        // 0x0001
        public const int SWP_NOMOVE = 2;

        // 0xffff
        public const int SWP_NOSIZE = 1;

        // 0x0002
        public const int SWP_NOZORDER = 4;

        // 0x0010
        public const int SWP_SHOWWINDOW = 64;

        private const int RDW_ALLCHILDREN = 0x0080;

        private const int RDW_ERASE = 0x0004;

        private const int RDW_ERASENOW = 0x0200;

        private const int RDW_FRAME = 0x0400;

        private const int RDW_INTERNALPAINT = 0x0002;

        private const int RDW_INVALIDATE = 0x0001;

        private const int RDW_NOCHILDREN = 0x0040;

        private const int RDW_NOERASE = 0x0020;

        private const int RDW_NOFRAME = 0x0800;

        private const int RDW_NOINTERNALPAINT = 0x0010;

        private const int RDW_UPDATENOW = 0x0100;

        private const int RDW_VALIDATE = 0x0008;

        private const int WM_CLOSE = 0x10;

        private const int WS_EX_NOACTIVATE = 0x08000000;

        private const int WS_EX_TOOLWINDOW = 0x00000080;

        private const int WS_EX_TOPMOST = 0x00000008;

        public delegate bool Win32Callback(IntPtr hwnd, IntPtr lParam);

        private const int WM_SETTEXT = 0x000C;

        #region WindowShowStyle enum

        /// <summary>
        /// Enumeration of the different ways of showing a window using ShowWindow
        /// </summary>
        public enum WindowShowStyle : uint
        {
            /// <summary>
            /// Hides the window and activates another window.
            /// </summary>
            /// <remarks>See SW_HIDE</remarks>
            Hide = 0,

            /// <summary>
            /// Activates and displays a window. If the window is minimized or maximized, the system
            /// restores it to its original size and position. An application should specify this
            /// flag when displaying the window for the first time.
            /// </summary>
            /// <remarks>See SW_SHOWNORMAL</remarks>
            ShowNormal = 1,

            /// <summary>
            /// Activates the window and displays it as a minimized window.
            /// </summary>
            /// <remarks>See SW_SHOWMINIMIZED</remarks>
            ShowMinimized = 2,

            /// <summary>
            /// Activates the window and displays it as a maximized window.
            /// </summary>
            /// <remarks>See SW_SHOWMAXIMIZED</remarks>
            ShowMaximized = 3,

            /// <summary>
            /// Maximizes the specified window.
            /// </summary>
            /// <remarks>See SW_MAXIMIZE</remarks>
            Maximize = 3,

            /// <summary>
            /// Displays a window in its most recent size and position. This value is similar to
            /// "ShowNormal", except the window is not actived.
            /// </summary>
            /// <remarks>See SW_SHOWNOACTIVATE</remarks>
            ShowNormalNoActivate = 4,

            /// <summary>
            /// Activates the window and displays it in its current size and position.
            /// </summary>
            /// <remarks>See SW_SHOW</remarks>
            Show = 5,

            /// <summary>
            /// Minimizes the specified window and activates the next top-level window in the Z order.
            /// </summary>
            /// <remarks>See SW_MINIMIZE</remarks>
            Minimize = 6,

            /// <summary>
            /// Displays the window as a minimized window. This value is similar to "ShowMinimized",
            /// except the window is not activated.
            /// </summary>
            /// <remarks>See SW_SHOWMINNOACTIVE</remarks>
            ShowMinNoActivate = 7,

            /// <summary>
            /// Displays the window in its current size and position. This value is similar to
            /// "Show", except the window is not activated.
            /// </summary>
            /// <remarks>See SW_SHOWNA</remarks>
            ShowNoActivate = 8,

            /// <summary>
            /// Activates and displays the window. If the window is minimized or maximized, the
            /// system restores it to its original size and position. An application should specify
            /// this flag when restoring a minimized window.
            /// </summary>
            /// <remarks>See SW_RESTORE</remarks>
            Restore = 9,

            /// <summary>
            /// Sets the show state based on the SW_ value specified in the STARTUPINFO structure
            /// passed to the CreateProcess function by the program that started the application.
            /// </summary>
            /// <remarks>See SW_SHOWDEFAULT</remarks>
            ShowDefault = 10,

            /// <summary>
            /// Windows 2000/XP: Minimizes a window, even if the thread that owns the window is
            /// hung. This flag should only be used when minimizing windows from a different thread.
            /// </summary>
            /// <remarks>See SW_FORCEMINIMIZE</remarks>
            ForceMinimized = 11
        }

        #endregion WindowShowStyle enum






        public static string GetCaptionOfWindow(IntPtr hwnd)
        {
            var caption = "";
            StringBuilder windowText = null;
            try
            {
                var max_length = GetWindowTextLength(hwnd);
                windowText = new StringBuilder("", max_length + 5);
                GetWindowText(hwnd, windowText, max_length + 2);

                if (!string.IsNullOrEmpty(windowText.ToString()) && !string.IsNullOrWhiteSpace(windowText.ToString()))
                {
                    caption = windowText.ToString();
                }
            }
            catch (Exception ex)
            {
                caption = ex.Message;
            }
            finally
            {
                windowText = null;
            }
            return caption;
        }

        public static List<IntPtr> GetChildWindows(IntPtr parent)
        {
            var result = new List<IntPtr>();
            var listHandle = GCHandle.Alloc(result);
            try
            {
                Win32Callback childProc = EnumWindow;
                EnumChildWindows(parent, childProc, GCHandle.ToIntPtr(listHandle));
            }
            finally
            {
                if (listHandle.IsAllocated)
                {
                    listHandle.Free();
                }
            }
            return result;
        }

        public static string GetClassNameOfWindow(IntPtr hwnd)
        {
            var className = "";
            StringBuilder classText = null;
            try
            {
                var cls_max_length = 1000;
                classText = new StringBuilder("", cls_max_length + 5);
                GetClassName(hwnd, classText, cls_max_length + 2);

                if (!string.IsNullOrEmpty(classText.ToString()) && !string.IsNullOrWhiteSpace(classText.ToString()))
                {
                    className = classText.ToString();
                }
            }
            catch (Exception ex)
            {
                className = ex.Message;
            }
            finally
            {
                classText = null;
            }
            return className;
        }

        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        public static List<IntPtr> GetRootWindowsOfProcess(int pid)
        {
            var rootWindows = GetChildWindows(IntPtr.Zero);
            var dsProcRootWindows = new List<IntPtr>();
            foreach (var hWnd in rootWindows)
            {
                uint lpdwProcessId;
                GetWindowThreadProcessId(hWnd, out lpdwProcessId);
                if (lpdwProcessId == pid)
                {
                    dsProcRootWindows.Add(hWnd);
                }
            }
            return dsProcRootWindows;
        }

        public static string GetTopWindowName(uint processId)
        {
            var text = new StringBuilder(1000);
            var hProcess = OpenProcess(0x0410, false, processId);
            GetModuleFileNameEx(hProcess, IntPtr.Zero, text, text.Capacity);
            CloseHandle(hProcess);

            return text.ToString();
        }

        public static string GetWindowTitle(IntPtr windowHandle)
        {
            uint SMTO_ABORTIFHUNG = 0x0002;
            uint WM_GETTEXT = 0xD;
            var MAX_STRING_SIZE = 32768;
            IntPtr result;
            var title = string.Empty;
            var memoryHandle = Marshal.AllocCoTaskMem(MAX_STRING_SIZE);
            Marshal.Copy(title.ToCharArray(), 0, memoryHandle, title.Length);
            SendMessageTimeout(windowHandle, WM_GETTEXT, (IntPtr)MAX_STRING_SIZE, memoryHandle, SMTO_ABORTIFHUNG, 1000, out result);
            title = Marshal.PtrToStringAuto(memoryHandle);
            Marshal.FreeCoTaskMem(memoryHandle);
            return title;
        }

        /// <summary>
        /// Move Window to Monitor
        /// </summary>
        /// <param name="window">Window to Move</param>
        /// <param name="monitorId">Monitor # (1,2,etc)</param>
        /// <param name="maximize">Maximize Window?</param>
        public static void MoveToMonitor(Window window, int monitorId, bool maximize)
        {
            Screen[] screens = Screen.AllScreens;

            int screenId = monitorId - 1;

            if (screens.Length > 1 && screenId < screens.Length)
            {
                var screen = screens[screenId];
                var area = screen.WorkingArea;

                if (maximize)
                {
                    window.Left = area.Left;
                    window.Top = area.Top;
                    window.Width = area.Width;
                    window.Height = area.Height;
                }
                else
                {
                    window.Left = area.Left;
                    window.Top = area.Top;
                }
            }
        }

        /// <summary>
        /// Sets the owner window to the main window of the current process
        /// </summary>
        /// <param name="window">Reference to the current window</param>
        public static void SetOwnerWindow(this Window window)
        {
            // Set new owner window without forcing
            SetOwnerWindow(window, false);
        }

        /// <summary>
        /// Sets the owner window to the main window of the current process
        /// </summary>
        /// <param name="window">Reference to the current window</param>
        /// <param name="forceNewOwner">
        /// If true, the new owner will be forced. Otherwise, if the window currently has an owner,
        /// that owner will be respected (and thus not changed)
        /// </param>
        public static void SetOwnerWindow(this Window window, bool forceNewOwner)
        {
            // Set owner window to process main window without forcing
            SetOwnerWindow(window, Process.GetCurrentProcess().MainWindowHandle, false);
        }

        /// <summary>
        /// Sets the owner window of a specific window via the Window class
        /// </summary>
        /// <param name="window">Reference to the current window</param>
        /// <param name="owner">New owner window</param>
        public static void SetOwnerWindow(this Window window, Window owner)
        {
            // Set owner without forcing
            SetOwnerWindow(window, owner, false);
        }

        /// <summary>
        /// Sets the owner window of a specific window via the window handle
        /// </summary>
        /// <param name="window">Reference to the current window</param>
        /// <param name="owner">New owner window</param>
        public static void SetOwnerWindow(this Window window, IntPtr owner)
        {
            // Set owner without forcing
            SetOwnerWindow(window, owner, false);
        }

        /// <summary>
        /// Sets the owner window of a specific window via the Window class
        /// </summary>
        /// <param name="window">Reference to the current window</param>
        /// <param name="owner">New owner window</param>
        /// <param name="forceNewOwner">
        /// If true, the new owner will be forced. Otherwise, if the window currently has an owner,
        /// that owner will be respected (and thus not changed)
        /// </param>
        public static void SetOwnerWindow(this Window window, Window owner, bool forceNewOwner)
        {
            // Check if this window currently has an owner
            if (!forceNewOwner && HasOwner(window))
            {
                return;
            }

            // Set owner
            window.Owner = owner;
        }

        /// <summary>
        /// Sets the owner window of a specific window via the window handle
        /// </summary>
        /// <param name="window">Reference to the current window</param>
        /// <param name="owner">New owner window</param>
        /// <param name="forceNewOwner">
        /// If true, the new owner will be forced. Otherwise, if the window currently has an owner,
        /// that owner will be respected (and thus not changed)
        /// </param>
        public static void SetOwnerWindow(this Window window, IntPtr owner, bool forceNewOwner)
        {
            // Check if this window currently has an owner
            if (!forceNewOwner && HasOwner(window))
            {
                return;
            }

            // Set owner via interop helper
            var interopHelper = new WindowInteropHelper(window);
            interopHelper.Owner = owner;

            // Since this owner type doesn't support WindowStartupLocation.CenterOwner, do it manually
            if (window.WindowStartupLocation == WindowStartupLocation.CenterOwner)
            {
                // Subscribe to the load event
                window.Loaded += delegate
                {
                    // Get the parent window rect
                    RECT ownerRect;
                    if (GetWindowRect(owner, out ownerRect))
                    {
                        // Get some additional information
                        var ownerWidth = ownerRect.Right - ownerRect.Left;
                        var ownerHeight = ownerRect.Bottom - ownerRect.Top;
                        var ownerHorizontalCenter = (ownerWidth / 2) + ownerRect.Left;
                        var ownerVerticalCenter = (ownerHeight / 2) + ownerRect.Top;

                        // Set the location to manual
                        window.WindowStartupLocation = WindowStartupLocation.Manual;

                        // Now we know the location of the parent, center the window
                        window.Left = ownerHorizontalCenter - (window.ActualWidth / 2);
                        window.Top = ownerVerticalCenter - (window.ActualHeight / 2);
                    }
                };
            }
        }

        public static void ShowWindowTopMost(IntPtr handle)
        {
            SetWindowPos(handle, (IntPtr)HWND_TOPMOST, 0, 0, 0, 0, SWP_NOSIZE | SWP_NOMOVE | SWP_NOACTIVATE | SWP_SHOWWINDOW);
        }

        private static bool EnumWindow(IntPtr handle, IntPtr pointer)
        {
            var gch = GCHandle.FromIntPtr(pointer);
            var list = gch.Target as List<IntPtr>;
            if (list == null)
            {
                throw new InvalidCastException("GCHandle Target could not be cast as List<IntPtr>");
            }
            list.Add(handle);
            // You can modify this to check to see if you want to cancel the operation, then return
            // a null here
            return true;
        }

        [DllImport("psapi.dll")]
        private static extern uint GetModuleFileNameEx(IntPtr hWnd, IntPtr hModule, StringBuilder lpFileName, int nSize);

        /// <summary>
        /// Returns whether the window currently has an owner
        /// </summary>
        /// <param name="window">Window to check</param>
        /// <returns>True if the window has an owner, otherwise false</returns>
        private static bool HasOwner(Window window)
        {
            return ((window.Owner != null) || (new WindowInteropHelper(window).Owner != IntPtr.Zero));
        }

        #region Nested type: POINT

        public struct POINT
        {
            public int X;
            public int Y;

            public POINT(Point pt)
            {
                if (pt.X > int.MaxValue)
                {
                    pt.X = int.MaxValue;
                }
                if (pt.X < int.MinValue)
                {
                    pt.X = int.MinValue;
                }
                if (double.IsNaN(pt.X))
                {
                    pt.X = 0;
                }

                if (pt.Y > int.MaxValue)
                {
                    pt.Y = int.MaxValue;
                }
                if (pt.Y < int.MinValue)
                {
                    pt.Y = int.MinValue;
                }
                if (double.IsNaN(pt.Y))
                {
                    pt.Y = 0;
                }

                X = Convert.ToInt32(pt.X);
                Y = Convert.ToInt32(pt.Y);
            }

            public POINT(int x, int y)
            {
                X = x;
                Y = y;
            }
        };

        #endregion Nested type: POINT

        #region User32 API

        public enum GetWindowType : uint
        {
            /// <summary>
            /// The retrieved handle identifies the window of the same type that is highest in the Z order.
            /// <para />
            /// If the specified window is a topmost window, the handle identifies a topmost window.
            /// If the specified window is a top-level window, the handle identifies a top-level
            /// window. If the specified window is a child window, the handle identifies a sibling window.
            /// </summary>
            GW_HWNDFIRST = 0,

            /// <summary>
            /// The retrieved handle identifies the window of the same type that is lowest in the Z order.
            /// <para />
            /// If the specified window is a topmost window, the handle identifies a topmost window.
            /// If the specified window is a top-level window, the handle identifies a top-level
            /// window. If the specified window is a child window, the handle identifies a sibling window.
            /// </summary>
            GW_HWNDLAST = 1,

            /// <summary>
            /// The retrieved handle identifies the window below the specified window in the Z order.
            /// <para />
            /// If the specified window is a topmost window, the handle identifies a topmost window.
            /// If the specified window is a top-level window, the handle identifies a top-level
            /// window. If the specified window is a child window, the handle identifies a sibling window.
            /// </summary>
            GW_HWNDNEXT = 2,

            /// <summary>
            /// The retrieved handle identifies the window above the specified window in the Z order.
            /// <para />
            /// If the specified window is a topmost window, the handle identifies a topmost window.
            /// If the specified window is a top-level window, the handle identifies a top-level
            /// window. If the specified window is a child window, the handle identifies a sibling window.
            /// </summary>
            GW_HWNDPREV = 3,

            /// <summary>
            /// The retrieved handle identifies the specified window's owner window, if any.
            /// </summary>
            GW_OWNER = 4,

            /// <summary>
            /// The retrieved handle identifies the child window at the top of the Z order, if the
            /// specified window is a parent window; otherwise, the retrieved handle is NULL. The
            /// function examines only child windows of the specified window. It does not examine
            /// descendant windows.
            /// </summary>
            GW_CHILD = 5,

            /// <summary>
            /// The retrieved handle identifies the enabled popup window owned by the specified
            /// window (the search uses the first such window found using GW_HWNDNEXT); otherwise,
            /// if there are no enabled popup windows, the retrieved handle is that of the specified window.
            /// </summary>
            GW_ENABLEDPOPUP = 6
        }

        [DllImport("user32.dll")]
        public static extern bool AttachThreadInput(uint idAttach, uint idAttachTo, bool fAttach);

        [DllImport("user32.dll")]
        public static extern bool ClientToScreen(IntPtr hWnd, ref POINT lpPoint);

        [DllImport("kernel32.dll")]
        public static extern bool CloseHandle(IntPtr handle);

        [DllImport("user32.Dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EnumChildWindows(IntPtr parentHandle, Win32Callback callback, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        // Find window by Caption only. Note you must pass IntPtr.Zero as the first parameter.
        [DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true)]
        public static extern IntPtr FindWindowByCaption(IntPtr ZeroOnly, string lpWindowName);

        // Find window by ClassName only. Note you must pass IntPtr.Zero as the second parameter.
        [DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true)]
        public static extern IntPtr FindWindowByClassName(string lpWindowName, IntPtr ZeroOnly);

        [DllImport("User32.dll")]
        public static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string strClassName, string strWindowName);

        [DllImport("kernel32.dll")]
        public static extern uint GetCurrentThreadId();

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr GetWindow(IntPtr hWnd, GetWindowType uCmd);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [DllImport("user32.dll")]
        public static extern int GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        // 0x0020
        /// <summary>
        /// The MoveWindow function changes the position and dimensions of the specified window. For
        /// a top-level window, the position and dimensions are relative to the upper-left corner of
        /// the screen. For a child window, they are relative to the upper-left corner of the parent
        /// window's client area.
        /// </summary>
        /// <param name="hWnd">Handle to the window.</param>
        /// <param name="X">Specifies the new position of the left side of the window.</param>
        /// <param name="Y">Specifies the new position of the top of the window.</param>
        /// <param name="nWidth">Specifies the new width of the window.</param>
        /// <param name="nHeight">Specifies the new height of the window.</param>
        /// <param name="bRepaint">
        /// Specifies whether the window is to be repainted. If this parameter is TRUE, the window
        /// receives a message. If the parameter is FALSE, no repainting of any kind occurs. This
        /// applies to the client area, the nonclient area (including the title bar and scroll
        /// bars), and any part of the parent window uncovered as a result of moving a child window.
        /// </param>
        /// <returns>
        /// If the function succeeds, the return value is nonzero.
        /// <para>
        /// If the function fails, the return value is zero. To get extended error information, call GetLastError.
        /// </para>
        /// </returns>
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);
        

        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(uint dwDesiredAccess, bool bInheritHandle, uint dwProcessId);

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool PostMessage(HandleRef hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        public static void RedrawWindow(IntPtr hWnd)
        {
            RedrawWindow(hWnd, IntPtr.Zero, IntPtr.Zero, RDW_INTERNALPAINT | RDW_UPDATENOW | RDW_INVALIDATE | RDW_FRAME | RDW_ERASE | RDW_ALLCHILDREN);
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr SendMessageTimeout(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam, uint fuFlags, uint uTimeout, out IntPtr lpdwResult);

        [DllImport("user32.dll")]
        public static extern IntPtr SetActiveWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern IntPtr SetFocus(IntPtr hWnd);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, int uFlags);

        /// <summary>
        /// Shows a Window
        /// </summary>
        /// <remarks>
        /// <para>To perform certain special effects when showing or hiding a window, use AnimateWindow.</para>
        /// <para>
        /// The first time an application calls ShowWindow, it should use the WinMain function's
        /// nCmdShow parameter as its nCmdShow parameter. Subsequent calls to ShowWindow must use
        /// one of the values in the given list, instead of the one specified by the WinMain
        /// function's nCmdShow parameter.
        /// </para>
        /// <para>
        /// As noted in the discussion of the nCmdShow parameter, the nCmdShow value is ignored in
        /// the first call to ShowWindow if the program that launched the application specifies
        /// startup information in the structure. In this case, ShowWindow uses the information
        /// specified in the STARTUPINFO structure to show the window. On subsequent calls, the
        /// application must call ShowWindow with nCmdShow set to SW_SHOWDEFAULT to use the startup
        /// information provided by the program that launched the application. This behavior is
        /// designed for the following situations:
        /// </para>
        /// <list type="">
        /// <item>
        /// Applications create their main window by calling CreateWindow with the WS_VISIBLE flag set.
        /// </item>
        /// <item>
        /// Applications create their main window by calling CreateWindow with the WS_VISIBLE flag
        /// cleared, and later call ShowWindow with the SW_SHOW flag set to make it visible.
        /// </item>
        /// </list>
        /// </remarks>
        /// <param name="hWnd">Handle to the window.</param>
        /// <param name="nCmdShow">
        /// Specifies how the window is to be shown. This parameter is ignored the first time an
        /// application calls ShowWindow, if the program that launched the application provides a
        /// STARTUPINFO structure. Otherwise, the first time ShowWindow is called, the value should
        /// be the value obtained by the WinMain function in its nCmdShow parameter. In subsequent
        /// calls, this parameter can be one of the WindowShowStyle members.
        /// </param>
        /// <returns>
        /// If the window was previously visible, the return value is nonzero. If the window was
        /// previously hidden, the return value is zero.
        /// </returns>
        [DllImport("user32.dll")]
        public static extern bool ShowWindow(IntPtr hWnd, WindowShowStyle cmdShow);

        [DllImport("user32.dll")]
        public static extern bool ShowWindowAsync(IntPtr hWnd, WindowShowStyle cmdShow);

        [DllImport("User32.dll")]
        private static extern IntPtr ChildWindowFromPoint(IntPtr hWndParent, POINT p);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

        [DllImport("User32.dll")]
        private static extern IntPtr GetParent(IntPtr hwnd);

        [DllImport("User32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern long GetWindowText(IntPtr hwnd, StringBuilder lpString, long cch);

        [DllImport("user32.dll")]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);


        [DllImport("user32.dll")]
        public static extern bool SetWindowText(IntPtr hWnd, string text);



        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool RedrawWindow(IntPtr hWnd, [In] ref RECT lprcUpdate, IntPtr hrgnUpdate, uint flags);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool RedrawWindow(IntPtr hWnd, IntPtr lprcUpdate, IntPtr hrgnUpdate, uint flags);

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Bottom;
            public int Left;
            public int Right;
            public int Top;
        }

        #endregion User32 API
    }
}