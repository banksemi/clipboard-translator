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
using System.Text.RegularExpressions;


namespace ClipboardTranslate
{
    public partial class Form1 : Form
    {
        string data = null;
        int timer = 0;
        NaverTranslate translater = new NaverTranslate();
        int add_Size = 0;
        public Form1()
        {
            InitializeComponent();
            add_Size = Size.Height - label1.Height;
            ConfigManager.Load(translater);
        }
        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(int nLeftRect
                                                      , int nTopRect
                                                      , int nRightRect
                                                      , int nBottomRect
                                                      , int nWidthEllipse
                                                      , int nHeightEllipse);

        private void Form1_Shown(object sender, EventArgs e)
        {
            Visible = false;
            this.Location = new Point(this.Location.X, 5);
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            string cdata = Translate.CleanText(Clipboard.GetText());
            if (cdata != null && cdata != data)
            {

                label1.ForeColor = Color.Black;
                data = cdata;

                if (data.IndexOf("Year:") > 0 && Clipboard.ContainsText(TextDataFormat.Html) && data.Split('\n').Length > 3)
                {
                    string html = Clipboard.GetText(TextDataFormat.Html);
                    int StartOffset = html.IndexOf("<!--StartFragment-->");
                    int EndOffset = html.IndexOf("<!--EndFragment-->");
                    html = html.Substring(StartOffset + 20, EndOffset - StartOffset - 20);

                    string[] lines = data.Split('\n');
                    string title = lines[0];
                    string ac = lines[1].Replace(" ; ", ", ");
                    string publisher = lines[3];
                    string year = lines[4];
                    string url = "";
                    if (year.IndexOf(",") > 0)
                    {
                        publisher += year.Substring(year.IndexOf(','));
                        year = year.Split(',')[0];
                    }
                    Regex regex = new Regex(@"http(s?)://ieeexplore.ieee.org(([.a-z:0-9]+)?)/document/([0-9]+)");
                    Match m = regex.Match(html);
                    url = "https://ieeexplore.ieee.org/document/" + m.Groups[4].Value;
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("<B>" + title + "</B>");
                    sb.Append("<ul>");

                    sb.AppendLine("<li>" + ac + "</li>");
                    sb.AppendLine("<li>" + year + "</li>");
                    sb.AppendLine("<li>" + publisher + "</li>");
                    sb.AppendLine("<li>" + "<A HREF=" + url + ">" + url + "</A>" + "</li>");
                    sb.Append("</ul>");
                    sb.Append("<br>");
                    AddHyperlinkToClipboard(sb.ToString());
                    data = Translate.CleanText(Clipboard.GetText());
                    onMessage("논문 서식 변경\r\n" + title);

                }
                else
                { 
                    onMessage(translater.Start(data));
                }
               // onMessage(Clipboard.GetData("Html").ToString());

                //onMessage(translater.Start(data));

            }
        }
        public void AddHyperlinkToClipboard(string sHtmlFragment)
        {
            const string sContextStart = "<HTML><BODY><!--StartFragment -->";
            const string sContextEnd = "<!--EndFragment --></BODY></HTML>";
            const string m_sDescription = "Version:1.0" + "\r\n" + "StartHTML:aaaaaaaaaa" + "\r\n" + "EndHTML:bbbbbbbbbb" + "\r\n" + "StartFragment:cccccccccc" + "\r\n" + "EndFragment:dddddddddd" + "\r\n";
            string sData = m_sDescription + sContextStart + sHtmlFragment + sContextEnd;
            sData = sData.Replace("aaaaaaaaaa", m_sDescription.Length.ToString().PadLeft(10, '0'));
            sData = sData.Replace("bbbbbbbbbb", sData.Length.ToString().PadLeft(10, '0'));
            sData = sData.Replace("cccccccccc", (m_sDescription + sContextStart).Length.ToString().PadLeft(10, '0'));
            sData = sData.Replace("dddddddddd", (m_sDescription + sContextStart + sHtmlFragment).Length.ToString().PadLeft(10, '0'));
            Clipboard.SetDataObject(new DataObject(DataFormats.Html, sData), true);
        }
        public void onMessage(string message)
        {
            if (message != null)
            {
                label1.Text = message;
              UtilFuncs.SetVisibleNoActivate(this, true);
                //Visible = true;
                timer = 4000;
                timer2.Enabled = true;
                Opacity = 0;
                Height = label1.Height + add_Size;
                Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, this.Width, this.Height, 7, 7));
            }
        }
        public void onHide()
        {

            Visible = false;
            timer2.Enabled = false;
            Opacity = 0;
        }
        private void timer2_Tick(object sender, EventArgs e)
        {
            timer--;
            if (timer == 0)
            {
                onHide();
            }
            if (timer > 500)
            {
                if (Opacity < 1) Opacity = Math.Min(Opacity + 0.15, 1);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, this.Width, this.Height, 7, 7));
        }

        private void notifyIcon1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void label1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                data = Translate.CleanText(label1.Text);
                label1.ForeColor = Color.Blue;
                Clipboard.SetText(data);
            }
            else
            {
                onHide();
            }
        }
    }
}
