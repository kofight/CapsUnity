using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Webgame.Utility;
using System.IO;
using UnityEngine;

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
    public float SlideSpeed = 8.0f;
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
    public static readonly int Plus5Point = 500;
    public static readonly int SugarCrushStepReward = 1500;
    public static readonly int SugarCrushStepIncrease = 100;
    public static readonly int BombPoint = 600;                //Invalid
    public static readonly int EatAColorPoint = 2000;          //Invalid
    public static readonly int FruitDropDown = 1000;
    public static readonly int EatJelly = 200;
    public static readonly int EatJellyDouble = 200;
    public static readonly int EatCagePoint = 400;
    public static readonly int EatStonePoint = 400;
    public static readonly int EatChocolate = 400;

    public static readonly int EffectAllDirTime = 1500;
    public static readonly int EffectAllDirBigTime = 1800;
    public static readonly int EffectEatAColorTime = 2000;
    public static readonly int EffectEatAllColorTime = 2200;
    public static readonly int EffectBigBombTime = 2000;
    public static readonly int EffectEatAColorNDBombTime = 2000;
    public static readonly int EffectResortTime = 600;                            //重拍特效中间的时间
    public static readonly int EffectResortInterval = 15;                         //重拍特效的时间间隔

    public static readonly string EatEffect = "EatEffect";                      //吃块的目标特效
    public static readonly string BombEatEffect = "BombEatEffect";              //炸弹吃块的目标特效
    public static readonly string LineEatEffect = "LineEatEffect";              //条状吃块的目标特效
    public static readonly string RainbowEatEffect = "RainbowEatEffect";        //彩虹吃块的目标特效
    public static readonly string ResortInEffect = "EatEffect";                      //重排开始特效
    public static readonly string ResortOutEffect = "EatEffect";                      //重排结束特效
    public static readonly string AddSpecialEffect = "AddSpecialEffect";       //添加
	
	public static readonly string EatAnim = "EatAnim";                          //条状炸弹的动画
    public static readonly string LineEatAnim = "LineEatAnim";             //条状炸弹的动画
    public static readonly string BombEatAnim = "BombEatAnim";                  //炸弹的动画
    public static readonly string RainbowEatAnim = "RainbowEatAnim";            //彩虹炸弹的动画

    public static readonly string ResortInAnim = "ResortInAnim";                  //重排特效开始动画
    public static readonly string ResortOutAnim = "ResortOutAnim";                //重排特效结束动画

    public static readonly string Bomb_Bomb_EatAnim = "Bomb_Bomb_EatAnim";      //两条状交换的动画
    public static readonly string Line_Line_EatAnim = "Line_Line_EatAnim";      //两条状交换的动画
    public static readonly string Line_Bomb_EatAnim = "Line_Bomb_EatAnim";      //条状炸弹交换的动画
    public static readonly string Line_Rainbow_EatAnim = "Line_Rainbow_EatAnim";   //条状和彩虹交换的动画
    public static readonly string Rainbow_Bomb_EatAnim = "Rainbow_Bomb_EatAnim";   //彩虹和炸弹交换的动画
    public static readonly string Rainbow_Rainbow_EatAnim = "Rainbow_Rainbow_EatAnim";   //彩虹和彩虹交换的动画

    public static readonly int EatAllDirBigAnimTime = 1800;                         //条状和彩虹交换的动画的时长

    public static float EatLineEffectInterval = 0.05f;               //消行特效 吃块的间隔
    public static float EatLineEffectStartInterval = 0.17f;          //消行特效 吃块的开始时间

    public static float Line_Line_EffectStartDelay = 0.0f;          //消行合消行 吃块的开始时间
    public static float Line_Line_EffectInterval = 0.05f;            //消行合消行 吃块的间隔

    public static float Line_Bomb_EffectStartDelay = 0.0f;          //消行合炸弹 吃块的开始时间
    public static float Line_Bomb_EffectInterval = 0.05f;            //消行合炸弹 吃块的间隔

    public static float Line_Rainbow_EffectStartDelay = 0.0f;       //消行合彩虹 吃块的开始时间
    public static float Line_Rainbow_EffectInterval = 0.05f;         //消行合彩虹 吃块的间隔

    public static float BombEffectInterval = 0.0f;                   //炸弹特效 吃块的间隔
    public static float EatBombEffectStartInterval = 0.9f;           //炸弹特效 吃块的开始时间

    public static float BigBombEffectInterval = 0.0f;                //炸弹合炸弹 特效吃块的间隔
    public static float EatBigBombEffectStartInterval = 0.9f;        //炸弹合炸弹 特效吃块的开始时间

    public static float Rainbow_EffectInterval = 0.1f;                //彩虹特效 吃块的间隔
    public static float Rainbow_EffectStartDelay = 0.3f;              //彩虹特效 吃块的开始前等待时间
    public static float Rainbow_EffectFlyDuration = 0.3f;             //彩虹子弹特效 飞行的时间

    public static float Rainbow_Rainbow_EffectInterval = 0.1f;        //彩虹合彩虹特效 吃块的间隔
    public static float Rainbow_Rainbow_StartDelay = 0.3f;            //彩虹合彩虹特效 吃块的开始前等待时间
    public static float Rainbow_Rainbow_EffectFlyDuration = 0.3f;     //彩虹合彩虹子弹特效 飞行的时间
	
	public static float Rainbow_Bomb_EffectAddItemInterval = 0.2f;    //


    public static AudioList CurAudioList;
	public static bool EnableGA = false;
	public static bool EnableTalkingData = false;

    public static int[] StageTypeArray;                             //存每关的类型
    public static int[] ItemPriceArray;                             //道具的价格
    public static int[] ItemUnLockLevelArray;                       //道具的解锁关卡

    private ConfigOperator _config;

    public string version = "Pre Alpha 0.1";

    public CapsConfig()
    {
        if (Instance == null)
        {
            _config = new ConfigOperator("GameConfig");
            Instance = this;
            InitData();

            GameObject obj = GameObject.Find("GlobalObject");
            CurAudioList = obj.GetComponent<AudioList>();
        }
        else
        {
            throw new Exception("重复的实例");
        }

    }

    public static int GetItemPrice(PurchasedItem item)
    {
        return ItemPriceArray[(int)item];
    }

    private void InitData()
    {
        _config.Read();
        TotalStageCount = _config.GetIntValue("TotalStageCount");
        DropAcc = _config.GetFloatValue("DropAccelarate");
        DropSpeed = _config.GetFloatValue("DropSpeed");
        SlideSpeed = _config.GetFloatValue("SlideSpeed");
        GameSpeed = _config.GetFloatValue("GameSpeed");
        EatTime = _config.GetFloatValue("EatTime");
        MoveTime = _config.GetIntValue("MoveTime");
        GetHeartInterval = _config.GetIntValue("GetHeartInterval");
        version = _config.GetStringValue("version");

        ///读取StageTypeArray///////////////////////////////////////////////////////////////////////
        string stageTypeStr = _config.GetStringValue("StageTypeArray");
        if (stageTypeStr != null)
        {
            string[] stringArray = stageTypeStr.Split("|"[0]);
            StageTypeArray = new int[stringArray.Length];
            for (int i = 0; i < stringArray.Length; i++)
                StageTypeArray[i] = Convert.ToInt32(stringArray[i]);
        }
        else
        {
            StageTypeArray = new int[50];
        }

        string itemPriceArrayStr = _config.GetStringValue("ItemPriceArray");
        if (itemPriceArrayStr != null)
        {
            string[] stringArray = itemPriceArrayStr.Split("|"[0]);
            ItemPriceArray = new int[stringArray.Length];
            for (int i = 0; i < stringArray.Length; i++)
                ItemPriceArray[i] = Convert.ToInt32(stringArray[i]);
        }
        else
        {
            ItemPriceArray = new int[10];
        }

        string itemUnLockArrayStr = _config.GetStringValue("ItemUnLockArray");
        if (itemUnLockArrayStr != null)
        {
            string[] stringArray = itemUnLockArrayStr.Split("|"[0]);
            ItemUnLockLevelArray = new int[stringArray.Length];
            for (int i = 0; i < stringArray.Length; i++)
                ItemUnLockLevelArray[i] = Convert.ToInt32(stringArray[i]);
        }
        else
        {
            ItemUnLockLevelArray = new int[10];
        }

        GameLogic.BLOCKWIDTH = _config.GetFloatValue("BLOCKWIDTH");
        GameLogic.BLOCKHEIGHT = _config.GetFloatValue("BLOCKHEIGHT");
		
		GameLogic.gameAreaWidth = GameLogic.BLOCKWIDTH * GameLogic.BlockCountX;	//游戏区域宽度
     	GameLogic.gameAreaHeight = GameLogic.BLOCKHEIGHT * GameLogic.BlockCountY + GameLogic.BLOCKHEIGHT / 2;//游戏区域高度
		
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

        //保存StageTypeArray////////////////////////////////////////////////////////////////////////
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        for (int i = 0; i < StageTypeArray.Length - 1; i++)
            sb.Append(StageTypeArray[i]).Append("|");
        sb.Append(StageTypeArray[StageTypeArray.Length - 1]);
        _config.Write("StageTypeArray", sb.ToString());


        _config.Write("BLOCKWIDTH", GameLogic.BLOCKWIDTH);
        _config.Write("BLOCKHEIGHT", GameLogic.BLOCKHEIGHT);
        _config.Save();
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

