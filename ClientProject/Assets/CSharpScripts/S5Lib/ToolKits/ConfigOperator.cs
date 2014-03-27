using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Webgame.Utility
{
    public class ConfigOperator
    {

        public ConfigOperator()
        {
            _dictValues = new Dictionary<string, ConfigItem>();
        }

        private string _path;
        public ConfigOperator(string path)
            : this()
        {
            _path = path;
        }

        Dictionary<string, ConfigItem> _dictValues;
        /// <summary>
        /// 向配置文件中写入一条记录.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="values"></param>
        /// <param name="description">注释.</param>
        public void Write(string key, object values, string description = "")
        {
            if (values == null)
                values = "";
            if (_dictValues.ContainsKey(key))
                _dictValues[key].Value = values.ToString();
            else
                _dictValues.Add(key, new ConfigItem { Value = values.ToString() });
        }

        public void Save()
        {
            switch (Application.platform)
            {
                case RuntimePlatform.WindowsEditor:
                case RuntimePlatform.OSXEditor:
                case RuntimePlatform.WindowsPlayer:
                    StreamWriter sw = new StreamWriter("Assets/Config/Resources/" + _path + ".txt", false, Encoding.UTF8);
                    foreach (var key in _dictValues.Keys)
                    {
                        if (key.Contains("//"))
                            sw.WriteLine(_dictValues[key].Description);
                        else
                            sw.WriteLine(key + "\t\t\t" + _dictValues[key].Value + "\t\t\t\t" + _dictValues[key].Description);
                    }
                    sw.Close();
                    //UnityEditor.AssetDatabase.Refresh();
                    break;
            }

        }

        /// <summary>
        /// 将列表格式的文本文件读到内存中.
        /// </summary>
        /// <param name="content"></param>
        /// <param name="split">分隔符：制表符 空格</param>
        /// <returns></returns>
        public List<string[]> ReadTextTable(string content, string[] split)
        {
            StringReader sr = new StringReader(content);
            List<string[]> listdata = new List<string[]>();
            string line = sr.ReadLine();
            while (line != null)
            {
                if (line.Contains("//"))
                {
                    line = sr.ReadLine();
                    continue;
                }
                if (string.IsNullOrEmpty(line))
                {
                    line = sr.ReadLine();
                    continue;
                }
                string[] values = line.Split(split, StringSplitOptions.RemoveEmptyEntries);
                if (values.Length > 0)
                    listdata.Add(values);
                line = sr.ReadLine();

            }
            sr.Close();
            return listdata;
        }

        public bool Read()
        {
            TextAsset text = (TextAsset)Resources.Load(_path);
            if (text != null)
            {
                Read(text.text);
                return true;
            }
            return false;
        }

        public void Read(string content, bool clearExist = true)
        {
            StringReader sr = new StringReader(content);
            if (clearExist)
            {
                _dictValues.Clear();
            }
            string strline = sr.ReadLine();
            int lineindex = -1;
            while (strline != null)
            {
                lineindex++;
                if (string.IsNullOrEmpty(strline))
                {
                    strline = sr.ReadLine();
                    continue;
                }
                int indexofdes = strline.IndexOf(@"//");
                if (indexofdes == 0)
                {
                    _dictValues.Add(@"//" + lineindex, new ConfigItem { Value = string.Empty, Description = strline });
                    strline = sr.ReadLine();
                    continue;
                }
                string description = string.Empty;
                if (indexofdes > 0)
                {
                    description = strline.Substring(indexofdes, strline.Length - indexofdes);
                    strline = strline.Substring(0, indexofdes);
                    if (!description.Contains("//"))
                        description = "//" + description;
                }
                string[] tmps = strline.Split(new string[] { " ", "\t" }, StringSplitOptions.RemoveEmptyEntries);
                if (tmps.Length == 2)
                {
                    string name = tmps[0];
                    string value = tmps[1];
                    if (!_dictValues.ContainsKey(name))
                        _dictValues.Add(name, new ConfigItem { Value = value, Description = description });
                }
                strline = sr.ReadLine();
            }
            sr.Close();
        }

        public int GetIntValue(string key)
        {
            ConfigItem item = null;
            _dictValues.TryGetValue(key, out item);
            if (item == null)
            {
                return 0;
            }
            return Convert.ToInt32(item.Value); ;
        }

        public string GetStringValue(string key)
        {
            ConfigItem item = null;
            _dictValues.TryGetValue(key, out item);
            if (item == null)
            {
                return string.Empty;
            }
            return item.Value; ;
        }

        public float GetFloatValue(string key)
        {
            ConfigItem item = null;
            _dictValues.TryGetValue(key, out item);
            if (item == null)
            {
                return 0;
            }
            return Convert.ToSingle(item.Value);
        }

        public bool GetBoolValue(string key)
        {
            ConfigItem item = null;
            _dictValues.TryGetValue(key, out item);
            if (item == null)
            {
                return false;
            }
            return Convert.ToBoolean(item.Value);
        }

        public bool GetIntValue(string key, out int value)
        {
            ConfigItem item = null;
            _dictValues.TryGetValue(key, out item);
            if (item == null)
            {
                value = 0;
                return false;
            }
            value = Convert.ToInt32(item.Value);
            return true;
        }

        public bool GetStringValue(string key, out string value)
        {
            ConfigItem item = null;
            _dictValues.TryGetValue(key, out item);
            if (item == null)
            {
                value =string.Empty;
                return false;
            }
            value = item.Value;
            return true;
        }

        public bool GetFloatValue(string key, out float value)
        {
            ConfigItem item = null;
            _dictValues.TryGetValue(key, out item);
            if (item == null)
            {
                value = 0;
                return false;
            }
            value = Convert.ToSingle(item.Value);
            return true;
        }

        public bool GetBoolValue(string key, out bool value)
        {
            ConfigItem item = null;
            _dictValues.TryGetValue(key, out item);
            if (item == null)
            {
                value = false;
                return false;
            }
            value = Convert.ToBoolean(item.Value);
            return true;
        }

        private class ConfigItem
        {
            public string Value { get; set; }
            public string Description { get; set; }
        }

        public void Dispose()
        {
            Save();
            _dictValues.Clear();
            _dictValues = null;
        }
    }
}
