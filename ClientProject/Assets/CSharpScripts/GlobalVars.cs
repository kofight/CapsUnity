using UnityEngine;
using System.Collections;

public enum TEditState
{
    None,
    ChangeColor,
    ChangeSpecial,
    EditStageGrid,
    Eat,
    EditPortal,
}

public enum PurchasedItem
{
	Item_Hammer,
	Item_PlusStep,
    None,
}

public class GlobalVars {

    public static int AvailabeStageCount = 3;           //当前可用关卡的数量
    public static int[] StageStarArray;                 //存每关得了几颗星
    public static int[] StageScoreArray;                 //存每关得了几颗星
	public static int[] StageFailedArray;               //how many times every stage failed
	public static int TotalStageCount = 4;              //当前可用关卡的数量
    public static bool EditStageMode = false;           //是否关卡编辑模式
    public static int CurStageNum = 1;                  //当前关卡编号
    public static bool DeveloperMode = false;           //开发者模式
    public static int HeartCount = 5;                   //爱心数量
    public static System.DateTime GetHeartTime;         //获得爱心的时间

    //编辑模式的变量
    public static TEditState EditState;                        //当前的编辑状态
    public static Portal EditingPortal;                     //当前正编辑的传送门
    public static string EditingPortalTip;                      //编辑传送门的提示
    public static TBlockColor EditingColor;                    //正在编辑的颜色
    public static TSpecialBlock EditingSpecial;                //正在编辑的颜色
    public static int     EditingGrid;                          //正在编辑的块

    public static StageData CurStageData;                       //当前正在查看或玩的关卡数据
    public static GameLogic CurGameLogic;                       //当前的游戏逻辑
	
	public static int Coins = 0;								//
	public static int [] PurchasedItemArray;					//
    public static int LastStage = 1;                            //当前在玩的关

    public static bool UseSFX;                                  //开启音效
    public static bool UseMusic;                                //开启音乐
}
