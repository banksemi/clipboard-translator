using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Windows.Forms;
namespace ClipboardTranslate
{
    public static class ConfigManager
    {
        private const string path = "Config.json";
        private static JObject ReadingData = null;
        private static JObject InitSetting
        {
            get
            {
                JObject json = new JObject();
                json["select"] = "naver";
                json["naver"] = new JObject();
                json["naver"]["key"] = new JObject();
                json["naver"]["setting"] = new JObject();
                json["naver"]["key"]["example_id1"] = "secret_key";
                json["naver"]["key"]["example_id2"] = "secret_key";
                json["naver"]["setting"]["type"] = "NMT";
                return json;
            }
        }
        public static bool CreateFile(bool replace = false)
        {
            if (File.Exists(path))
            {
                if (replace == true)
                    File.Delete(path);
                else
                    return false;
            }
            File.WriteAllText(path, InitSetting.ToString());
            return true;
        }
        public static void Load(Translate translater)
        {
            if (!File.Exists(path))
                CreateFile(true);
            translater.Keys.Clear();
            ReadingData = JObject.Parse(File.ReadAllText(path));
            foreach (JProperty property in ReadingData[translater.Name]["key"])
            {
                if (!property.Name.Contains("example"))
                    translater.Keys.Add(property.Name, (string)property.Value);
            }
            foreach (JProperty property in ReadingData[translater.Name]["setting"])
            {
                if (!property.Name.Contains("example"))
                    translater.Settings.Add(property.Name, (string)property.Value);
            }
            if (translater.Keys.Count == 0)
            {
                MessageBox.Show("다음 열리는 파일에 네이버 API를 입력해주세요.", "API 없음");
                ProcessStartInfo info = new ProcessStartInfo();
                info.FileName = path;
                info.WorkingDirectory = Application.StartupPath;
                Process.Start(path);
            }
        }
    }
}
