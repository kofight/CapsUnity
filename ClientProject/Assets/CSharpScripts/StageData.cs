using UnityEngine;
using System.Collections;
using Webgame.Utility;

public enum GameTarget
{
	ClearJelly,
	BringFruitDown,
	GetScore,
}

public class StageData 
{
	public GameTarget Target;           //关卡目标
	public int 		StepLimit;		    //步数限制
	public int  	TimeLimit;          //时间限制（单位秒）
    public int[]    StarScore = new int[3];          //获得星星的分数
    public GridData[,] GridDataArray = new GridData[GameLogic.BlockCountX,GameLogic.BlockCountY];      //关卡初始地块数据

    public static StageData CreateStageData()     //从资源装载关卡
    {
        StageData data = new StageData();
        for (int i = 0; i < GameLogic.BlockCountX; i++)
            for (int j = 0; j < GameLogic.BlockCountY; j++)
            {
                data.GridDataArray[i, j] = new GridData();
            }

        return data;
    }

    int GridDataToInt(GridData data)
    {
        return (int)data.grid * 10 + (int)data.gridBlock;
    }

    GridData IntToGridData(int value)
    {
        GridData data = new GridData();
        data.grid = (TGridType)(value / 10);
        data.gridBlock = (TGridBlockType)(value % 10);
        return data;
    }

    private ConfigOperator _config;

    public void LoadStageData(int levelNum)
    {
        _config = new ConfigOperator("Level" + levelNum);
        if (!_config.Read())
        {
            Debug.LogError("Trying to load an file not exist! filename = Level" + levelNum);
            return;
        }
        int targetInt = 0;
        _config.GetValue<int>("Target", out targetInt);
        Target = (GameTarget)targetInt;
        _config.GetValue<int>("StepLimit", out StepLimit);
        _config.GetValue<int>("TimeLimit", out TimeLimit);

        string temp;
        _config.GetValue<string>("StarScore", out temp);
        string [] scoreTokens = temp.Split(',');
        for (int i = 0; i < 3; ++i)
        {
            StarScore[i] = (int)System.Convert.ChangeType(scoreTokens[i], typeof(int));
        }

        _config.GetValue<string>("GridDataArray", out temp);

        string[] gridDataTokens = temp.Split(',');
        for (int i = 0; i < GameLogic.BlockCountX; ++i )
        {
            for (int j = 0; j < GameLogic.BlockCountY; ++j )
            {
                int number = (int)System.Convert.ChangeType(gridDataTokens[j * GameLogic.BlockCountX + i], typeof(int));
                GridDataArray[i, j] = IntToGridData(number);
            }
        }

        Debug.Log("Level " + levelNum + " Loaded");
    }

    public void SaveStageData(int levelNum)              //保存关卡数据到一个文件
    {
        _config = new ConfigOperator("Level" + levelNum);
        _config.Write("Target", (int)Target);
        _config.Write("StepLimit", StepLimit);
        _config.Write("TimeLimit", TimeLimit);
        string temp = string.Empty;
        for (int i = 0; i < 3; ++i )
        {
            temp = temp + StarScore[i] + ",";
        }

        _config.Write("StarScore", temp);

        temp = string.Empty;
        for (int j = 0; j < GameLogic.BlockCountY; ++j)
        {
            for (int i = 0; i < GameLogic.BlockCountX; ++i)
            {
                int number = GridDataToInt(GridDataArray[i, j]);
                temp = temp + number + ",";
            }
        }

        _config.Write("GridDataArray", temp);
        _config.Save();
        Debug.Log("Level " + levelNum + " Saved");
    }
}
