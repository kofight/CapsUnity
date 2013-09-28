using UnityEngine;
using System.Collections;

public enum TEditState
{
    None,
    ChangeColor,
    ChangeSpecial,
    EditStageBlock,
    EditStageGrid,
    Eat,
}

public class GlobalVars {

    public static int AvailabeStageCount = 3;           //当前可用关卡的数量
	public static int TotalStageCount = 4;              //当前可用关卡的数量
    public static bool EditStageMode = false;           //是否关卡编辑模式
    public static int CurStageNum = 1;                  //当前关卡编号

    //编辑模式的变量
    public static TEditState EditState;                        //当前的编辑状态
    public static TBlockColor EditingColor;                    //正在编辑的颜色
    public static TSpecialBlock EditingSpecial;                //正在编辑的颜色
    public static TGridType     EditingGrid;                //正在编辑的颜色
    public static TGridBlockType EditingGridBlock;                //正在编辑的颜色

    public static StageData CurStageData;                                    //当前正在查看或玩的关卡数据
    public static GameLogic CurGameLogic;                       //当前的游戏逻辑

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
