using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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

public enum TStepReward
{
    Dir,
    Eat,
}

public enum GameTarget
{
	ClearJelly,
	BringFruitDown,
	GetScore,
    Collect,
}

public enum CollectType
{
    Collor1,
    Collor2,
    Collor3,
    Collor4,
    Collor5,
    Collor6,
    Collor7,
    Line0Bomb,
    Line1Bomb,
    Line2Bomb,
    Bomb,
    EatColorBomb,
}

public class FTUEData
{
    public string headImage;
    public string dialog;
    public string pic;
    public Position from;
    public Position to;
    public bool bHighLightBackground;               //是否高亮背景
    public bool bHighLightBlock;                    //是否高亮前景
    public List<Position> highLightPosList;
    public int dialogPos = 0;                           //对话框位置0,1(上下)
    public int picturePos = 9;                          //图片位置
}

public class Portal
{
    public Portal()
    {
        from.MakeItUnAvailable();
        to.MakeItUnAvailable();
    }

	public Portal(Portal p)
	{
		from = p.from;
		to = p.to;
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
    public int NutMaxCount = 3;            //最多同屏数量
    public int NutMinCount = 0;             //同屏最少水果限制，0代表没限制
    public int NutInitCount = 1;            //初始化数量
    public int NutStep = 10;            //步数间隔

    public int PlusMaxCount = 3;            //最多同屏数量
    public int PlusInitCount = 1;            //初始化数量
    public int PlusStepMin = 5;            //步数间隔
    public int PlusStepMax = 10;            //步数间隔
    public int PlusStartTime = 0;           //开始出现+5的时间

    public int ChocolateCount = 0;
    public int StoneCount = 0;

    public int[]    StarScore = new int[3];          //获得星星的分数
    public int [, ] GridData = new int[GameLogic.BlockCountX, GameLogic.BlockCountY];                        //关卡初始地块数据

    public TSpecialBlock[] CollectSpecial = new TSpecialBlock[3];                                            //搜集目标的块类型
    public TBlockColor[] CollectColors = new TBlockColor[3];                                                 //搜集目标的块颜色
    public int [] CollectCount = new int[3];                                                                 //每个搜集项的数量

    public Dictionary<int, List<FTUEData>> FTUEMap = new Dictionary<int, List<FTUEData>>();                             //FTUE地图


    public Dictionary<int, Portal> PortalToMap = new Dictionary<int, Portal>();                                                                //用来储存所有的传送门，键值是传送目标的编码
    public Dictionary<int, Portal> PortalFromMap = new Dictionary<int, Portal>();                                                                //用来储存所有的传送门，键值是传送目标的编码

    public Dictionary<int, int> SpecialBlock = new Dictionary<int, int>();      //关卡开始特殊块

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

    public int GetSingleJellyCount()
    {
        int count = 0;
        for (int i = 0; i < GameLogic.BlockCountX; i++)				//遍历一行
        {
            for (int j = 0; j < GameLogic.BlockCountY; j++)		//遍历一列
            {
                if (CheckFlag(i, j, GridFlag.Jelly))
                {
                    ++count;
                }
            }
        }
        return count;
    }

    public int GetDoubleJellyCount()
    {
        int count = 0;
        for (int i = 0; i < GameLogic.BlockCountX; i++)				//遍历一行
        {
            for (int j = 0; j < GameLogic.BlockCountY; j++)		//遍历一列
            {
                if (CheckFlag(i, j, GridFlag.JellyDouble))
                {
                    ++count;
                }
            }
        }
        return count;
    }

    private ConfigOperator _config;

    public void LoadStageData(int levelNum)
    {
        _config = new ConfigOperator("Level" + levelNum);
        _config.Read();
		
        int targetInt = 0;
        _config.GetValue<int>("Target", out targetInt);
        Target = (GameTarget)targetInt;
        _config.GetValue<int>("StepLimit", out StepLimit);
        _config.GetValue<int>("TimeLimit", out TimeLimit);
        _config.GetValue<int>("ColorCount", out ColorCount);
        _config.GetValue<int>("Nut1Count", out Nut1Count);
        _config.GetValue<int>("Nut2Count", out Nut2Count);
        _config.GetValue<int>("NutInitCount", out NutInitCount);
        _config.GetValue<int>("NutMaxCount", out NutMaxCount);
        _config.GetValue<int>("NutMinCount", out NutMinCount);
        _config.GetValue<int>("NutStep", out NutStep);
        _config.GetValue<int>("PlusInitCount", out PlusInitCount);
        _config.GetValue<int>("PlusMaxCount", out PlusMaxCount);
        _config.GetValue<int>("PlusStartTime", out PlusStartTime);
        if (!_config.GetValue<int>("PlusStepMin", out PlusStepMin))
        {
            PlusStepMin = 5;
        }
        if (!_config.GetValue<int>("PlusStepMax", out PlusStepMax))
        {
            PlusStepMax = 10;
        }
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

        if (_config.GetValue<string>("CollectCount", out temp))
        {
            string[] collectCountTokens = temp.Split(',');
            for (int i = 0; i < 3; ++i)
            {
                CollectCount[i] = System.Convert.ToInt32(collectCountTokens[i]);
            }
        }

        if (_config.GetValue<string>("CollectColors", out temp))
        {
            string[] collectColorTokens = temp.Split(',');
            for (int i = 0; i < 3; ++i)
            {
                CollectColors[i] = (TBlockColor)System.Convert.ToInt32(collectColorTokens[i]);
            }
        }

        if (_config.GetValue<string>("CollectSpecial", out temp))
        {
            string[] collectSpecialTokens = temp.Split(',');
            for (int i = 0; i < 3; ++i)
            {
                CollectSpecial[i] = (TSpecialBlock)System.Convert.ToInt32(collectSpecialTokens[i]);
            }
        }

        //GridData

        _config.GetValue<string>("GridDataArray", out temp);

        string[] gridDataTokens = temp.Split(',');
        for (int i = 0; i < GameLogic.BlockCountX; ++i )
        {
            for (int j = 0; j < GameLogic.BlockCountY; ++j )
            {
                int number = (int)System.Convert.ChangeType(gridDataTokens[j * GameLogic.BlockCountX + i], typeof(int));
                if ((number & (int)GridFlag.Chocolate) > 0)
                {
                    ++ChocolateCount;
                }
                if ((number & (int)GridFlag.Stone) > 0)
                {
                    ++StoneCount;
                }
                GridData[i, j] = number;
            }
        }

        PortalToMap.Clear();
        PortalFromMap.Clear();
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
                PortalToMap.Add(portal.to.ToInt(), portal);
                PortalFromMap.Add(portal.from.ToInt(), portal);
            }
        }

        //Portals
        _config.GetValue<string>("SpecialArray", out temp);
        if (temp != null)
        {
            string[] specialDataTokens = temp.Split(',');
            for (int i = 0; i < specialDataTokens.Length - 2; i += 3)
            {
                Position pos = new Position((int)System.Convert.ChangeType(specialDataTokens[i], typeof(int)), (int)System.Convert.ChangeType(specialDataTokens[i + 1], typeof(int)));
                int special = (int)System.Convert.ChangeType(specialDataTokens[i + 2], typeof(int));
                SpecialBlock.Add(pos.ToInt(), special);
            }
        }

        Debug.Log("Level " + levelNum + " Loaded");

        FTUEMap.Clear();

        string ftue = ResourceManager.Singleton.LoadTextFile("FTUE" + levelNum);
        if (ftue != string.Empty)
        {
            List<FTUEData> curFTUEGroup = new List<FTUEData>();
            int step = -1;
            StringReader sr = new StringReader(ftue);
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
                string[] values = line.Split(new string[] { "\t", " " }, System.StringSplitOptions.RemoveEmptyEntries);
                if (values.Length > 0)
                {
                    int num = System.Convert.ToInt32(values[0]);

                    if (step == -1)
                    {
                        step = num;
                        FTUEMap.Add(step, curFTUEGroup);                               //添加一个组
                    }
                    else if(num != step)
                    {
                        curFTUEGroup = new List<FTUEData>();
                        step = num;
                        FTUEMap.Add(step, curFTUEGroup);                               //添加一个组
                    }
                    

                    FTUEData data = new FTUEData();
                    data.highLightPosList = new List<Position>();
                    data.headImage = values[1];
                    data.dialog = values[2];
                    string[] posArray;
                    if (values[3] != "None")
                    {
                        posArray = values[3].Split(',');
                        data.from.x = System.Convert.ToInt32(posArray[0]);
                        data.from.y = System.Convert.ToInt32(posArray[1]);
                    }
                    else
                    {
                        data.from.MakeItUnAvailable();
                    }


                    if (values[4] != "None")
                    {
                        posArray = values[4].Split(',');
                        data.to.x = System.Convert.ToInt32(posArray[0]);
                        data.to.y = System.Convert.ToInt32(posArray[1]);
                    }
                    else
                    {
                        data.to.MakeItUnAvailable();
                    }

                    if (values[5] != "None")
                    {
                        posArray = values[5].Split(',');
                        for (int i = 0; i < posArray.Length / 2; ++i)
                        {
                            int x = System.Convert.ToInt32(posArray[i * 2]);
                            int y = System.Convert.ToInt32(posArray[i * 2 + 1]);
                            data.highLightPosList.Add(new Position(x, y));
                        }
                    }

                    if (values.Length > 6)
                    {
                        data.pic = values[6];
                    }
                    else
                    {
                        data.pic = "None";
                    }

                    if (values.Length > 7)
                    {
                        if (System.Convert.ToInt32(values[7]) == 1)
                        {
                            data.bHighLightBackground = true;
                            data.bHighLightBlock = false;
                        }
                        else if (System.Convert.ToInt32(values[7]) == 2)
                        {
                            data.bHighLightBackground = true;
                            data.bHighLightBlock = true;
                        }
						else if (System.Convert.ToInt32(values[7]) == 0)
						{
							data.bHighLightBackground = false;
                        	data.bHighLightBlock = true;
						}
                    }
                    else
                    {
                        data.bHighLightBackground = false;
                        data.bHighLightBlock = true;
                    }

                    if (values.Length > 8)
                    {
                        posArray = values[8].Split(',');

                        data.dialogPos = System.Convert.ToInt32(posArray[0]);
                        data.picturePos = System.Convert.ToInt32(posArray[1]);
                    }

                    curFTUEGroup.Add(data);                               //添加对话数据
                }

                line = sr.ReadLine();
            }
            sr.Close();
        }
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
		_config.Write("NutInitCount", NutInitCount);
        _config.Write("NutMaxCount", NutMaxCount);
        _config.Write("NutMinCount", NutMinCount);
		_config.Write("NutStep", NutStep);

        _config.Write("PlusInitCount", PlusInitCount);
        _config.Write("PlusMaxCount", PlusMaxCount);
        _config.Write("PlusStartTime", PlusStartTime);
        _config.Write("PlusStepMin", PlusStepMin);
        _config.Write("PlusStepMax", PlusStepMax);

        _config.Write("Seed", Seed);
        _config.Write("NewFormat", 1);                  //新格式标记
        string temp = string.Empty;
        for (int i = 0; i < 3; ++i )
        {
            temp = temp + StarScore[i] + ",";
        }

        _config.Write("StarScore", temp);

        temp = string.Empty;
        for (int i = 0; i < 3; ++i)
        {
            temp = temp + (int)CollectColors[i] + ",";
        }

        _config.Write("CollectColors", temp);

        temp = string.Empty;
        for (int i = 0; i < 3; ++i)
        {
            temp = temp + (int)CollectSpecial[i] + ",";
        }

        _config.Write("CollectSpecial", temp);

        temp = string.Empty;
        for (int i = 0; i < 3; ++i)
        {
            temp = temp + CollectCount[i] + ",";
        }

        _config.Write("CollectCount", temp);

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

        foreach(KeyValuePair<int, Portal> pair in PortalToMap)
        {
            temp = temp + pair.Value.from.x + ",";
            temp = temp + pair.Value.from.y + ",";
            temp = temp + pair.Value.to.x + ",";
            temp = temp + pair.Value.to.y + ",";
            temp = temp + pair.Value.flag + ",";
        }

        _config.Write("PortalArray", temp);

        //保存画面上的特殊块
        temp = string.Empty;
        for (int j = 0; j < GameLogic.BlockCountY; ++j)
        {
            for (int i = 0; i < GameLogic.BlockCountX; ++i)
            {
                CapBlock block = GameLogic.Singleton.GetBlock(new Position(i, j));
                if (block != null && block.special != TSpecialBlock.ESpecial_Normal)
                {
                    temp = temp + i + ",";
                    temp = temp + j + ",";
                    temp = temp + (int)block.special+ ",";
                }
            }
        }
        _config.Write("SpecialArray", temp);

        _config.Save();
        Debug.Log("Level " + levelNum + " Saved");
    }
}
