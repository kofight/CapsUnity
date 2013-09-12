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
    /// <summary>
    /// 是否网络版
    /// </summary>
    public bool IsNetVersion = false;

    [ConfigAttribute(DefaultValue = 1.5f, Name = "球正常落地等待时间", FieldType = typeof(float))]
    public float BallStopWaitTime = 1.0f;

    [ConfigAttribute(DefaultValue = 2.0f, Name = "落水等待时间", FieldType = typeof(float))]
    public float DownInWaterWaitTime = 2.0f;

    [ConfigAttribute(DefaultValue = 3.0f, Name = "进洞等待时间", FieldType = typeof(float))]
    public float EnterHoleWaitTime = 3.0f;

    [ConfigAttribute(DefaultValue = 5.0f, Name = "所有玩家进洞等待时间", FieldType = typeof(float))]
    public float AllPlayerEnterHoleWaitTime = 5.0f;

    [ConfigAttribute(DefaultValue = 3f, Name = "击球数据显示时间", FieldType = typeof(float))]
    public float ShowPlayerHidDataWaitTime = 2.5f;

    private ConfigOperator _config;

    public UseFilePath useFilePath = UseFilePath.LocalPath;

    public bool RapidStart;

    [ConfigAttribute(DefaultValue = 20.0f, Name = "鼠标移动速度", FieldType = typeof(float))]
    public float MouseMoveSpeed = 20.0f;

    [ConfigAttribute(DefaultValue = 1, Name = "摄像机初始高度", FieldType = typeof(float))]
    public float InitHeightToFloor = 1.0f;

    [ConfigAttribute(DefaultValue = 2, Name = "摄像机到球的距离", FieldType = typeof(float))]
    public float InitCameraDistanceToBall = 2;

    [ConfigAttribute(DefaultValue = true, Name = "开启3d云", FieldType = typeof(Boolean))]
    public bool Use3DCloud = true;

    [ConfigAttribute(DefaultValue = 100.0f, Name = "风的方向(0到360)", FieldType = typeof(float))]
    public float WindEqualAnglesY = 100.0f;

    [ConfigAttribute(DefaultValue = 0.5f, Name = "风的强度", FieldType = typeof(float))]
    public float WindStrength = 0.5f;

    [ConfigAttribute(DefaultValue = 0.0f, Name = "雨的强度(0.0f到1.0f)", FieldType = typeof(float))]
    public float RainStrength = 0.0f;

    [ConfigAttribute(DefaultValue = 14.0f, Name = "太阳时间", FieldType = typeof(float))]
    public float SunTime = 14.0f;

    [ConfigAttribute(DefaultValue = 0.1f, Name = "色彩饱和度", FieldType = typeof(float))]
    public float ColorSaturation = 0.1f;

    public string version;
    public string sn;

    //不同地表的滚动摩擦系数.
    [ConfigAttribute(DefaultValue = 0.8f, Name = "球道上的减速系数", FieldType = typeof(float))]
    public float fractionFactorOnFairWay = 0.8f;

    [ConfigAttribute(DefaultValue = 0.85f, Name = "果岭上的减速系数", FieldType = typeof(float))]
    public float fractionFactorOnGreen = 0.85f;

    [ConfigAttribute(DefaultValue = 0.7f, Name = "短草上的减速系数", FieldType = typeof(float))]
    public float fractionFactorOnShortGrass = 0.7f;

    [ConfigAttribute(DefaultValue = 0.6f, Name = "长草上的减速系数", FieldType = typeof(float))]
    public float fractionFactorOnLongGrass = 0.6f;

    [ConfigAttribute(DefaultValue = 0.5f, Name = "沙坑上的减速系数", FieldType = typeof(float))]
    public float fractionFactorOSand = 0.5f;

    [ConfigAttribute(DefaultValue = -9.8f, Name = "空中的重力", FieldType = typeof(float))]
    public float gravityOnAir = -9.8f;

    [ConfigAttribute(DefaultValue = -0.1f, Name = "球道的下坡重力", FieldType = typeof(float))]
    public float gravityOnFairWay = -0.1f;

    [ConfigAttribute(DefaultValue = -0.1f, Name = "果岭的下坡重力", FieldType = typeof(float))]
    public float gravityOnGreen = -0.1f;

    [ConfigAttribute(DefaultValue = -9.8f, Name = "其他表面的下坡重力", FieldType = typeof(float))]
    public float gravityOnSurface = -9.8f;

    [ConfigAttribute(DefaultValue = 0.06f, Name = "在果岭上的停止速度", FieldType = typeof(float))]
    public float stopSpeedOnGreen = 0.06f;

    [ConfigAttribute(DefaultValue = 0.1f, Name = "在球道上的停止速度", FieldType = typeof(float))]
    public float stopSpeedOnFairWay = 0.1f;

    [ConfigAttribute(DefaultValue = 0.2f, Name = "一般情况的停止速度", FieldType = typeof(float))]
    public float stopSpeed = 0.2f;

    [ConfigAttribute(DefaultValue = 2000, Name = "击球消息挂起时间(毫秒)", FieldType = typeof(int))]
    public int hitBallHoldMessageTime = 2000;

    [ConfigAttribute(DefaultValue = 5.0f, Name = "球飞行时的风力系数", FieldType = typeof(float))]
    public float windFactorWhenFlying = 5.0f;

    [ConfigAttribute(DefaultValue = 0.5f, Name = "球滚动时的风力系数", FieldType = typeof(float))]
    public float windFactorWhenRolling = 0.5f;

    [ConfigAttribute(DefaultValue = "COM5", Name = "COM接口名字", FieldType = typeof(string))]
    public string ComName = "COM5";

    [ConfigAttribute(DefaultValue = 115200, Name = "波特率", FieldType = typeof(int))]
    public int BaudRate = 115200;

    /// <summary>
    /// 球的半径.
    /// </summary>
    public float RadiusProp = 0.035f;


    public string LoginServerIp;
    public int LoginServerPort;

    public string GameServerIp;
    public int GameServerPort;

    public float windFactor = 5.0f;



    public CapsConfig()
    {
        if (Instance == null)
        {
            //_config = new ConfigOperator(ResManager.Singleton.GetBasePath() + "Config/golfconfig.txt");
            Instance = this;
            //InitData();
        }
        else
        {
            throw new Exception("重复的实例");
        }

    }


    private void InitData()
    {
        _config.Read();
        _config.GetValue<float>("BallStopWaitTime", out BallStopWaitTime);
        _config.GetValue<float>("DownInWaterWaitTime", out DownInWaterWaitTime);
        _config.GetValue<float>("EnterHoleWaitTime", out EnterHoleWaitTime);
        _config.GetValue<float>("AllPlayerEnterHoleWaitTime", out AllPlayerEnterHoleWaitTime);
        _config.GetValue<float>("ShowPlayerHidDataWaitTime", out ShowPlayerHidDataWaitTime);

        int filepath = 0;
        _config.GetValue<int>("useFilePath", out filepath);
        useFilePath = (UseFilePath)filepath;
        _config.GetValue<bool>("RapidStart", out RapidStart);
        _config.GetValue<float>("MouseMoveSpeed", out MouseMoveSpeed);
        _config.GetValue<float>("InitHeightToFloor", out InitHeightToFloor);
        _config.GetValue<bool>("Use3DCloud", out Use3DCloud);
        _config.GetValue<float>("WindEqualAnglesY", out WindEqualAnglesY);
        _config.GetValue<float>("WindStrength", out WindStrength);
        _config.GetValue<float>("RainStrength", out RainStrength);
        _config.GetValue<float>("SunTime", out SunTime);
        _config.GetValue<float>("ColorSaturation", out ColorSaturation);
        _config.GetValue<string>("version", out version);
        _config.GetValue<string>("sn", out sn);
        _config.GetValue<float>("fractionFactorOnFairWay", out fractionFactorOnFairWay);
        _config.GetValue<float>("fractionFactorOnGreen", out fractionFactorOnGreen);
        _config.GetValue<float>("fractionFactorOnShortGrass", out fractionFactorOnShortGrass);
        _config.GetValue<float>("fractionFactorOnLongGrass", out fractionFactorOnLongGrass);
        _config.GetValue<float>("fractionFactorOSand", out fractionFactorOSand);
        _config.GetValue<float>("gravityOnAir", out gravityOnAir);
        _config.GetValue<float>("gravityOnFairWay", out gravityOnFairWay);
        _config.GetValue<float>("gravityOnGreen", out gravityOnGreen);
        _config.GetValue<float>("gravityOnSurface", out gravityOnSurface);
        _config.GetValue<float>("stopSpeedOnGreen", out stopSpeedOnGreen);
        _config.GetValue<float>("stopSpeedOnFairWay", out stopSpeedOnFairWay);
        _config.GetValue<float>("stopSpeed", out stopSpeed);
        _config.GetValue<string>("LoginServerIp", out LoginServerIp);
        _config.GetValue<int>("LoginServerPort", out LoginServerPort);
        _config.GetValue<string>("GameServerIp", out GameServerIp);
        _config.GetValue<int>("GameServerPort", out GameServerPort);
        _config.GetValue<int>("hitBallHoldMessageTime", out hitBallHoldMessageTime);
        _config.GetValue<float>("windFactorWhenRolling", out windFactorWhenRolling);
        _config.GetValue<float>("RadiusProp", out RadiusProp);
        _config.GetValue<string>("ComName", out ComName);
        _config.GetValue<int>("BaudRate", out BaudRate);
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
        _config.Write("BallStopWaitTime", BallStopWaitTime);
        _config.Write("DownInWaterWaitTime", DownInWaterWaitTime);
        _config.Write("EnterHoleWaitTime", EnterHoleWaitTime);
        _config.Write("AllPlayerEnterHoleWaitTime", AllPlayerEnterHoleWaitTime);
        _config.Write("ShowPlayerHidDataWaitTime", ShowPlayerHidDataWaitTime);
        _config.Write("filepath", (int)useFilePath);
        _config.Write("RapidStart", RapidStart);
        _config.Write("MouseMoveSpeed", MouseMoveSpeed);
        _config.Write("InitHeightToFloor", InitHeightToFloor);
        _config.Write("Use3DCloud", Use3DCloud);
        _config.Write("WindEqualAnglesY", WindEqualAnglesY);
        _config.Write("WindStrength", WindStrength);
        _config.Write("RainStrength", RainStrength);
        _config.Write("SunTime", SunTime);
        _config.Write("ColorSaturation", ColorSaturation);
        _config.Write("fractionFactorOnFairWay", fractionFactorOnFairWay);
        _config.Write("fractionFactorOnGreen", fractionFactorOnGreen);
        _config.Write("fractionFactorOnShortGrass", fractionFactorOnShortGrass);
        _config.Write("fractionFactorOnLongGrass", fractionFactorOnLongGrass);
        _config.Write("fractionFactorOSand", fractionFactorOSand);
        _config.Write("gravityOnAir", gravityOnAir);
        _config.Write("gravityOnFairWay", gravityOnFairWay);
        _config.Write("gravityOnGreen", gravityOnGreen);
        _config.Write("stopSpeedOnFairWay", stopSpeedOnFairWay);
        _config.Write("stopSpeed", stopSpeed);
        _config.Write("version", version);
        _config.Write("sn", sn);
        _config.Write("hitBallHoldMessageTime", hitBallHoldMessageTime);
        _config.Write("windFactorWhenRolling", windFactorWhenRolling);
        _config.Write("RadiusProp", RadiusProp);
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

