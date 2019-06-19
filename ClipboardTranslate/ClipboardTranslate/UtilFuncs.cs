using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ClipboardTranslate
{
    public static class UtilFuncs

    {

        [DllImport("user32.dll")]

        extern public static bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter,
                                                                 int x, int y, int cx, int cy, int uFlags);


        public const int HWND_TOPMOST = -1;     // 0xffff 

        public const int SWP_NOSIZE = 1;            // 0x0001 

        public const int SWP_NOMOVE = 2;          // 0x0002 

        public const int SWP_NOACTIVATE = 16;   // 0x0010 

        public const int SWP_SHOWWINDOW = 64; // 0x0040 


        public static void ShowWindowTopMost(IntPtr handle)

        {

            SetWindowPos(handle, (IntPtr)HWND_TOPMOST, 0, 0, 0, 0, SWP_NOSIZE |
                                                                                                 SWP_NOMOVE |
                                                                                                 SWP_NOACTIVATE |
                                                                                                 SWP_SHOWWINDOW);

        }



        public static void SetVisibleNoActivate(Control control, bool visible)

        {

            if (visible)

                ShowWindowTopMost(control.Handle);



                 control.Visible = visible;

        }
    }
}
