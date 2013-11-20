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
    public float DropAcc = 5.0f;
    public float DropSpeed = 3.0f;
    public float SlideSpeed = 3.0f;
    public float GameSpeed = 1.0f;
    public float EatTime = 0.2f;
    public int MoveTime = 200;
    public int GetHeartInterval = 300;          //获得心的时间间隔，目前是5分钟，单位是秒
    public UseFilePath useFilePath = UseFilePath.LocalPath;

    //[ConfigAttribute(DefaultValue = 2.0f, Name = "落水等待时间", FieldType = typeof(float))]
    //public float DownInWaterWaitTime = 2.0f;

    //用来计算分数的常量
    public int MaxKQuanlity = 10;
    public int MaxKCombo = 9;
    public int[] KQuanlityTable = { 1, 2, 3, 3, 3, 3, 3, 3, 3 };
    public int[] KComboTable = {0, 1, 2, 3, 4, 5, 6, 6, 6};
    public static readonly int Plus5Point = 1500;
    public static readonly int SugarCrushStepReward = 500;
    public static readonly int BombPoint = 600;
    public static readonly int EatAColorPoint = 2000;
    public static readonly int FruitDropDown = 1000;
    public static readonly int EatJelly = 200;
    public static readonly int EatJellyDouble = 200;
    public static readonly int EatCagePoint = 400;
    public static readonly int EatStonePoint = 400;
    public static readonly int EatChocolate = 400;

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
        _config.GetValue<float>("DropAccelarate", out DropAcc);
        _config.GetValue<float>("DropSpeed", out DropSpeed);
        _config.GetValue<float>("SlideSpeed", out SlideSpeed);
        _config.GetValue<float>("GameSpeed", out GameSpeed);
        _config.GetValue<float>("EatTime", out EatTime);
        _config.GetValue<int>("MoveTime", out MoveTime);
        _config.GetValue<int>("GetHeartInterval", out GetHeartInterval);
        _config.GetValue<string>("version", out version);

        _config.GetValue<float>("BLOCKWIDTH", out GameLogic.BLOCKWIDTH);
        _config.GetValue<float>("BLOCKHEIGHT", out GameLogic.BLOCKHEIGHT);
        _config.GetValue<float>("BLOCKSCALE", out GameLogic.BlockScale);
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
        _config.Write("DropAccelarate", DropAcc);
        _config.Write("DropSpeed", DropSpeed);
        _config.Write("SlideSpeed", SlideSpeed);
        _config.Write("GameSpeed", GameSpeed);
        _config.Write("EatTime", EatTime);
        _config.Write("MoveTime", MoveTime);
        _config.Write("version", version);
        _config.Write("GetHeartInterval", GetHeartInterval);

        _config.Write("BLOCKWIDTH", GameLogic.BLOCKWIDTH);
        _config.Write("BLOCKHEIGHT", GameLogic.BLOCKHEIGHT);
        _config.Write("BLOCKSCALE", GameLogic.BlockScale);
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

