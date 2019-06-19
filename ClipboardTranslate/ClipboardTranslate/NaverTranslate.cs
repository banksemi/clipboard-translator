using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Text;
using System.IO;
using Newtonsoft.Json.Linq;
namespace ClipboardTranslate
{
    class NaverTranslate : Translate
    {
        public int now = 0;
        public NaverTranslate() : base("Naver")
        {
            
        }
        public string SMT(string data)
        {
            string url = "https://openapi.naver.com/v1/language/translate";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Headers.Add("X-Naver-Client-Id", "VEQwGdJYGAaEB79Sxj_G");
            request.Headers.Add("X-Naver-Client-Secret", "FHykKwajk9");
            request.Method = "POST";
            string query = data;
            byte[] byteDataParams = Encoding.UTF8.GetBytes("source=en&target=ko&text=" + query);
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = byteDataParams.Length;
            Stream st = request.GetRequestStream();
            st.Write(byteDataParams, 0, byteDataParams.Length);
            st.Close();
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream stream = response.GetResponseStream();
            StreamReader reader = new StreamReader(stream, Encoding.UTF8);
            string text = reader.ReadToEnd();
            stream.Close();
            response.Close();
            reader.Close();
            JObject json = JObject.Parse(text);
            string jsondata = (string)json["message"]["result"]["translatedText"];
            return jsondata;
        }
        public string NMT(string data)
        {
            if (Keys.Count <= now) return "번역 최대 용량 초과";
            try
            {
                string url = "https://openapi.naver.com/v1/papago/n2mt";
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Timeout = 4000;
                request.Headers.Add("X-Naver-Client-Id", Keys.ToArray()[now].Key);
                request.Headers.Add("X-Naver-Client-Secret", Keys.ToArray()[now].Value);
                request.Method = "POST";
                string query = data;
                byte[] byteDataParams = Encoding.UTF8.GetBytes("source=en&target=ko&text=" + Uri.EscapeDataString(query));
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = byteDataParams.Length;
                Stream st = request.GetRequestStream();
                st.Write(byteDataParams, 0, byteDataParams.Length);
                st.Close();
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream stream = response.GetResponseStream();
                StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                string text = reader.ReadToEnd();
                stream.Close();
                response.Close();
                reader.Close();
                JObject json = JObject.Parse(text);
                string jsondata = (string)json["message"]["result"]["translatedText"];
                return jsondata;
            }
            catch (System.Net.WebException e)
            {
                if (e.Message.IndexOf("429") >= 0)
                {
                    now++;
                    return NMT(data);
                }
                return "[에러] " + e.Message;
            }
        }

        public string NMT2(string data)
        {
            if (Keys.Count <= now) return "번역 최대 용량 초과";
            try
            {
                string url = "https://easyrobot.co.kr/NMT.php";
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Timeout = 4000;
                request.Headers.Add("X-Naver-Client-Id", Keys.ToArray()[now].Key);
                request.Headers.Add("X-Naver-Client-Secret", Keys.ToArray()[now].Value);
                request.Method = "POST";
                string query = data;
                byte[] byteDataParams = Encoding.UTF8.GetBytes("source=en&target=ko&text=" + Uri.EscapeDataString(query));
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = byteDataParams.Length;
                Stream st = request.GetRequestStream();
                st.Write(byteDataParams, 0, byteDataParams.Length);
                st.Close();
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream stream = response.GetResponseStream();
                StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                string text = reader.ReadToEnd();
                stream.Close();
                response.Close();
                reader.Close();
                if (text.IndexOf("{") == 0)
                {
                    JObject json = JObject.Parse(text);
                    if ((string)json["errorCode"] != null)
                        throw new Exception((string)json["errorCode"]);
                }
                return text;
            }
            catch (Exception e)
            {
                if (e.Message.IndexOf("010") >= 0)
                {
                    now++;
                    return NMT2(data);
                }
                return "[에러] " + e.Message;
            }
        }
        public override string Method(string data)
        {
            return NMT2(data);
        }
    }
}
