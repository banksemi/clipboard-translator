using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace ClipboardTranslate
{
    public class RoundView : Form
    {
        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(int nLeftRect
                                                 , int nTopRect
                                                 , int nRightRect
                                                 , int nBottomRect
                                                 , int nWidthEllipse
                                                 , int nHeightEllipse);
        public RoundView()
        {
            this.Resize += new System.EventHandler(this.RoundView_Resize);
        }

        private void RoundView_Resize(object sender, EventArgs e)
        {
            Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 7, 7));
        }
    }
}
