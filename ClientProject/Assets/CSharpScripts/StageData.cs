﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Webgame.Utility;

public enum GridFlag
{
    GenerateCap = 1,        //是否要在这个格子生成瓶盖

    Jelly = 1 << 1,              //果冻
    
    JellyDouble = 1 << 2,        //双层果冻

    Birth = 1 << 3,              //块的生成点
    
    NotGenerateCap = 1 << 4,     // 标记不生成瓶盖的格子（因为GenerateCap为0时无法清晰表达语义，所以用这个来补充）  

    Stone = 1 << 5,              //石头

    Cage = 1 << 6,               //笼子

    Chocolate = 1 << 7,          //巧克力

    FruitExit = 1 << 8,          //水果的消失点

    Portal = 1 << 9,             //传送门
    PortalStart = 1 << 10,             //传送门
    PortalEnd = 1 << 11,             //传送门

    MoveAble = 1 << 12,             //是否可移动
}

public enum PortalFlag
{
    Normal,             //正常
    Invisible,          //不可见
}


public enum GameTarget
{
	ClearJelly,
	BringFruitDown,
	GetScore,
}

public class Portal
{
	public Portal(){}
	public Portal(Portal p)
	{
		from = new Position(p.from);
		to = new Position(p.to);
		flag = p.flag;
	}
    public Position from;
    public Position to;

    public int flag;           //目前只有两种， 0 不可见  1 可见
}

public class StageData 
{
	public GameTarget Target;           //关卡目标
	public int 		StepLimit;		    //步数限制
	public int  	TimeLimit;          //时间限制（单位秒）
    public int      ColorCount = 7;         //颜色总数
    public int Seed = 0;                    //种子
    public int Nut1Count = 3;
    public int Nut2Count = 3;
    public int[]    StarScore = new int[3];          //获得星星的分数
    public int [, ] GridData = new int[GameLogic.BlockCountX, GameLogic.BlockCountY];                        //关卡初始地块数据


    public Dictionary<int, Portal> PortalMap = new Dictionary<int, Portal>();                                                                //用来储存所有的传送门，键值是传送目标的编码

    public bool CheckFlag(int x, int y, GridFlag flag)
    {
        if ((GridData[x, y] & (int)flag) > 0)
        {
            return true;
        }
        return false;
    }

    public void ClearFlag(int x, int y, GridFlag flag)
    {
        GridData[x, y] &= ~(int)flag;            //清除标记
    }

    public void AddFlag(int x, int y, GridFlag flag)
    {
        GridData[x, y] |= (int)flag;
    }

    public static StageData CreateStageData()     //从资源装载关卡
    {
        StageData data = new StageData();
        for (int i = 0; i < GameLogic.BlockCountX; i++)
            for (int j = 0; j < GameLogic.BlockCountY; j++)
            {
                data.GridData[i, j] = new int();
            }

        return data;
    }

    public int GetJellyCount()
    {
        int count = 0;
        for (int i = 0; i < GameLogic.BlockCountX; i++)				//遍历一行
        {
            for (int j = 0; j < GameLogic.BlockCountY; j++)		//遍历一列
            {
                if (CheckFlag(i, j, GridFlag.Jelly) || CheckFlag(i, j, GridFlag.JellyDouble))
                {
                    ++count;
                }
            }
        }
        return count;
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
		
		int newFormat = 0;
		_config.GetValue<int>("NewFormat", out newFormat);
		if(newFormat == 0)
		{
			LoadOldStageData(levelNum);
			return;
		}
		
        int targetInt = 0;
        _config.GetValue<int>("Target", out targetInt);
        Target = (GameTarget)targetInt;
        _config.GetValue<int>("StepLimit", out StepLimit);
        _config.GetValue<int>("TimeLimit", out TimeLimit);
        _config.GetValue<int>("ColorCount", out ColorCount);
        _config.GetValue<int>("Nut1Count", out Nut1Count);
        _config.GetValue<int>("Nut2Count", out Nut2Count);
        _config.GetValue<int>("Seed", out Seed);

        if (ColorCount == 0)
        {
            ColorCount = 7;
        }

        string temp;
        _config.GetValue<string>("StarScore", out temp);
        string [] scoreTokens = temp.Split(',');
        for (int i = 0; i < 3; ++i)
        {
            StarScore[i] = (int)System.Convert.ChangeType(scoreTokens[i], typeof(int));
        }

        //GridData

        _config.GetValue<string>("GridDataArray", out temp);

        string[] gridDataTokens = temp.Split(',');
        for (int i = 0; i < GameLogic.BlockCountX; ++i )
        {
            for (int j = 0; j < GameLogic.BlockCountY; ++j )
            {
                int number = (int)System.Convert.ChangeType(gridDataTokens[j * GameLogic.BlockCountX + i], typeof(int));
                GridData[i, j] = number;
            }
        }

        PortalMap.Clear();
        //Portals
        _config.GetValue<string>("PortalArray", out temp);
        if (temp != null)
        {
            string[] portalDataTokens = temp.Split(',');
            for (int i = 0; i < portalDataTokens.Length - 4; i += 5)
            {
                Portal portal = new Portal();
                portal.from = new Position((int)System.Convert.ChangeType(portalDataTokens[i], typeof(int)), (int)System.Convert.ChangeType(portalDataTokens[i + 1], typeof(int)));
                portal.to = new Position((int)System.Convert.ChangeType(portalDataTokens[i + 2], typeof(int)), (int)System.Convert.ChangeType(portalDataTokens[i + 3], typeof(int)));
                portal.flag = (int)System.Convert.ChangeType(portalDataTokens[i + 4], typeof(int));
                PortalMap.Add(portal.to.ToInt(), portal);
            }
        }

        Debug.Log("Level " + levelNum + " Loaded");
    }

    public void LoadOldStageData(int levelNum)
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
        _config.GetValue<int>("ColorCount", out ColorCount);
        _config.GetValue<int>("Nut1Count", out Nut1Count);
        _config.GetValue<int>("Nut2Count", out Nut2Count);
        int newFormat = 0;
        _config.GetValue<int>("NewFormat", out newFormat);
        if (ColorCount == 0)
        {
            ColorCount = 7;
        }

        string temp;
        _config.GetValue<string>("StarScore", out temp);
        string[] scoreTokens = temp.Split(',');
        for (int i = 0; i < 3; ++i)
        {
            StarScore[i] = (int)System.Convert.ChangeType(scoreTokens[i], typeof(int));
        }

        _config.GetValue<string>("GridDataArray", out temp);

        string[] gridDataTokens = temp.Split(',');
        for (int i = 0; i < GameLogic.BlockCountX; ++i)
        {
            for (int j = 0; j < GameLogic.BlockCountY; ++j)
            {
                int number = (int)System.Convert.ChangeType(gridDataTokens[j * GameLogic.BlockCountX + i], typeof(int));

                if (newFormat > 0)
                {
                    GridData[i, j] = number;
                    continue;
                }

                GridData data = IntToGridData(number);

                int flags = 0;

                if (data.grid == TGridType.Jelly)
                {
                    flags |= (int)GridFlag.Jelly;
                    flags |= (int)GridFlag.GenerateCap;
                }
                if (data.grid == TGridType.JellyDouble)
                {
                    flags |= (int)GridFlag.JellyDouble;
                    flags |= (int)GridFlag.GenerateCap;
                }

                if (data.gridBlock == TGridBlockType.Cage)
                {
                    flags |= (int)GridFlag.GenerateCap;
                    flags |= (int)GridFlag.Cage;
                }

                if (data.gridBlock == TGridBlockType.Chocolate)
                {
                    flags |= (int)GridFlag.NotGenerateCap;
                    flags |= (int)GridFlag.Chocolate;
                }

                if (data.gridBlock == TGridBlockType.Stone)
                {
                    flags |= (int)GridFlag.Stone;
                    flags |= (int)GridFlag.NotGenerateCap;
                }

                if (data.gridBlock == TGridBlockType.None)
                {
                    if (data.grid == TGridType.Normal)
                    {
                        flags |= (int)GridFlag.GenerateCap;
                    }
                }

                GridData[i, j] = flags;
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
        _config.Write("ColorCount", ColorCount);
        _config.Write("Nut1Count", Nut1Count);
        _config.Write("Nut2Count", Nut2Count);
        _config.Write("Seed", Seed);
        _config.Write("NewFormat", 1);                  //新格式标记
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
                temp = temp + GridData[i, j] + ",";
            }
        }

        _config.Write("GridDataArray", temp);

        temp = string.Empty;

        foreach(KeyValuePair<int, Portal> pair in PortalMap)
        {
            temp = temp + pair.Value.from.x + ",";
            temp = temp + pair.Value.from.y + ",";
            temp = temp + pair.Value.to.x + ",";
            temp = temp + pair.Value.to.y + ",";
            temp = temp + pair.Value.flag + ",";
        }

        _config.Write("PortalArray", temp);

        _config.Save();
        Debug.Log("Level " + levelNum + " Saved");
    }
}
