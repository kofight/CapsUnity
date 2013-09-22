using UnityEngine;
using System.Collections;

public enum TEditState
{
    None,
    ChangeColor,
    ChangeSpecial,
    EditStageBlock,
    EditStageGrid,
}

public class GlobalVars {

    public static int AvailabeStageCount = 3;           //当前可用关卡的数量
	public static int TotalStageCount = 4;              //当前可用关卡的数量
    public static bool EditStageMode = false;           //是否关卡编辑模式

    //编辑模式的变量
    public static TEditState EditState;                        //当前的编辑状态
    public static TBlockColor EditingColor;                    //正在编辑的颜色
    public static TSpecialBlock EditingSpecial;                //正在编辑的颜色

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
