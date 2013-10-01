using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Webgame.Utility;
using System.IO;

/// <summary>
/// 配置数据属性.
/// </summary>
public class ConfigAttribute : Attribute
{
    public string Name { get; set; }
    public object DefaultValue { get; set; }
    public Type FieldType { get; set; }
    public object Value { get; set; }
    public string FieldName { get; set; }
}

public class CapsConfig
{
    public int TotalStageCount = 4;

    public UseFilePath useFilePath = UseFilePath.LocalPath;

    //[ConfigAttribute(DefaultValue = 2.0f, Name = "落水等待时间", FieldType = typeof(float))]
    //public float DownInWaterWaitTime = 2.0f;

    //用来计算分数的常量
    public int MaxKQuanlity = 10;
    public int MaxKCombo = 9;
    public int[] KQuanlityTable = { 0, 1, 2, 3, 3, 3, 3, 3, 3 };
    public int[] KComboTable = {0, 1, 2, 3, 4, 5, 6, 6, 6};

    private ConfigOperator _config;

    public string version = "Pre Alpha 0.1";

    public CapsConfig()
    {
        if (Instance == null)
        {
            _config = new ConfigOperator("GameConfig");
            Instance = this;
            InitData();
        }
        else
        {
            throw new Exception("重复的实例");
        }

    }


    private void InitData()
    {
        _config.Read();
        _config.GetValue<int>("TotalStageCount", out TotalStageCount);
        _config.GetValue<string>("version", out version);
    }

    /// <summary>
    /// 获得所有配置数据属性.
    /// </summary>
    /// <returns></returns>
    public List<ConfigAttribute> GetValues()
    {
        List<ConfigAttribute> configValues = new List<ConfigAttribute>();
        System.Reflection.FieldInfo[] fields = Instance.GetType().GetFields();
        foreach (var f in fields)
        {
            ConfigAttribute attribute = f.GetCustomAttributes(false).Where(p => p is ConfigAttribute).FirstOrDefault() as ConfigAttribute;
            if (attribute == null)
                continue;
            attribute.Value = f.GetValue(Instance);
            attribute.FieldName = f.Name;
            configValues.Add(attribute);
        }
        return configValues;
    }

    public void SetValue(string propname, object value)
    {
        System.Reflection.FieldInfo field = Instance.GetType().GetFields().FirstOrDefault(p => p.Name == propname);
        if (field == null)
            return;
        field.SetValue(Instance, value);

    }

    public void Save()
    {
        _config.Write("TotalStageCount", TotalStageCount);
        _config.Write("version", version);
    }

    public void ReadDefault()
    {
        System.Reflection.FieldInfo[] fields = Instance.GetType().GetFields();
        foreach (var f in fields)
        {
            ConfigAttribute attribute = f.GetCustomAttributes(false).Where(p => p is ConfigAttribute).FirstOrDefault() as ConfigAttribute;
            if (attribute == null)
                continue;
            f.SetValue(Instance, attribute.DefaultValue);
        }
    }


    public static CapsConfig Instance;
}

