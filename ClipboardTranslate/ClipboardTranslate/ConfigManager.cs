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
        public static bool CreateFile(bool replace = false)
        {
            if (File.Exists(path))
            {
                if (replace == true)
                    File.Delete(path);
                else
                    return false;
            }
            JObject json = new JObject();
            json["example_id1"] = "secret_key";
            json["example_id2"] = "secret_key";
            File.WriteAllText(path, json.ToString());
            return true;
        }
        public static void Load(Translate translater)
        {
            if (!File.Exists(path))
                CreateFile(true);
            translater.Keys.Clear();
            ReadingData = JObject.Parse(File.ReadAllText(path));
            foreach(JProperty property in (JToken)ReadingData)
            {
                if (!property.Name.Contains("example"))
                    translater.Keys.Add(property.Name, (string)property.Value);
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
