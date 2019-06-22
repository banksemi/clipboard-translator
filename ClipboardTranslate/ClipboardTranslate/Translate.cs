using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClipboardTranslate
{
    public abstract class Translate
    {
        public Dictionary<string, string> Keys = new Dictionary<string, string>();
        public Dictionary<string, string> Settings = new Dictionary<string, string>();
        private Dictionary<string, string> cache = new Dictionary<string, string>();
        public string Name = null;
        public Translate(string Name)
        {
            this.Name = Name;
        }

        public abstract string Method(string data);
        public string Start(string data)
        {
            // 입력 데이터 정리
            data = CleanText(data);
            if (string.IsNullOrEmpty(data))
                return null;

            if (cache.ContainsKey(data))
                return cache[data];
            else
            {
                string tdata = Method(data);
                if (data.IndexOf("[에러]") != 0)
                   cache.Add(data, tdata);
                return tdata;
            }
        }
        public static string CleanText(string data)
        {
            data = data.Replace("\r", "");


            // 문장 합치기 번역
            //  data = data.Replace("\n", " ");
            //   data = data.Replace(".", ".\n");

            data = data.Replace("”", "\"");


            data = data.Replace("\t", " ");
            data = data.Replace("", " ");
            data = data.Replace("", " "); // 두개 다른거
            data = data.Replace("  ", " ");
            data = data.Replace("  ", " ");
            data = data.Replace("  ", " ");
            data = data.Trim();

            // 문장을 이쁘게 분리한다.

            List<List<string>> lines = new List<List<string>>();
            string[] ori_data = data.Split('\n');
            foreach (string d in ori_data)
            {
                List<string> temp = new List<string>();
                temp.AddRange(d.Trim().Split(' '));
                lines.Add(temp);
            }
            string result = "";
            foreach (var c in lines)
            {
                if (c[0] == "")
                {
                    result += "\n";
                    continue;
                }
                bool enter = false;
                if (result.Length > 0)
                {
                    if (c[0][0] >= '0' && c[0][0] <= '9')
                    {
                        enter = true;
                    }
                    if (c[0][0] >= 'A' && c[0][0] <= 'Z')
                    {
                        enter = true;
                    }
                    if (c[0][0] == '(')
                    {
                        enter = true;
                    }

                    if (c[0][0] == '-')
                    {
                        enter = true;
                    }
                    if (result[result.Length - 1] == '.')
                    {
                        enter = true;
                    }
                    if (result[result.Length - 1] == ':')
                    {
                        enter = true;
                    }

                    if (result[result.Length - 1] == ')')
                    {
                        enter = true;
                    }

                    if (result[result.Length - 1] == '?')
                    {
                        enter = true;
                    }
                    if (result[result.Length - 1] == ',')
                    {
                        enter = false;
                        result += ' ';
                    }
                }
                if (enter) result += "\n";
                else if (result.Length > 0)
                    result += " ";
                bool f = true;
                if (c[0].ToLower() == "is" || c[0].ToLower() == "do") c.Add("?");


                for(int i = 0; i < c.Count; i++)
                {
                    if (i != 0) result += " ";
                    result += c[i];
                }
            }

            if (result.Length > 1000) result = "원활한 번역을 위해 1000자 이하로 복사해주세요!!";
            if (result.Equals("\n")) result = null;
            return result;
        }
    }
}
