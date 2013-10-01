using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum TGameState
{
    EGameState_Waitting,
    EGameState_Playing,
    EGameState_EndWaitingDropingDown,			//游戏已经满足结束条件，等待下落结束
    EGameState_PlayingEndAni,
    EGameState_End,
};

enum TDirection
{
    EDir_Up,
    EDir_UpRight,
    EDir_DownRight,
    EDir_Down,
    EDir_LeftDown,
    EDir_LeftUp,
};

struct Paticle
{
    int xSpeed;
    int ySpeed;
    int startX;
    int startY;
    long startTime;
    float gravity;			//重力值
    float lifeTime;			//生命期
    bool changeAlphaByLifeTime;		//是否改变Alpha
    TBlockColor color;
    UISprite pBlockSprite;
};

public class Position{
	public int x;
	public int y;
	public Position()
	{
        x = 0;
        y = 0;
	}
	public Position(int xPos, int yPos)
    {
        x = xPos;
        y = yPos;
    }

	public static bool operator!=(Position lhs, Position rhs)
	{
		if (lhs.x != rhs.x || lhs.y != rhs.y)
		{
			return true;
		}
		return false;
	}

    public static bool operator==(Position lhs, Position rhs)
	{
		if (lhs.x == rhs.x && lhs.y == rhs.y)
		{
			return true;
		}
		return false;
	}

    public void Set(int xPos, int yPos)
    {
        x = xPos;
        y = yPos;
    }
};

public enum TGridType
{
    Normal,         //普通
    None,           //空格子
    Jelly,          //果冻
    JellyDouble,    //两层果冻
}

public enum TGridBlockType     //固定障碍
{
    None,               //没有
    Stone,              //石头
    Chocolate,          //巧克力
    Cage,               //笼子
}

public class GridData
{
    public TGridType grid;
    public TGridBlockType gridBlock;
    public bool bBirth;                          //是否为出生点
}

public class GameLogic {
    public static readonly int BlockCountX = 7;	//游戏区有几列
    public static readonly int BlockCountY = 7;	//游戏区有几行
    public static readonly int ColorCount = 7;     //有几种颜色
    public static readonly int BLOCKWIDTH = 60;
    public static readonly int gameAreaX = 0;		//游戏区域左上角坐标
    public static readonly int gameAreaY = 100;		//游戏区域左上角坐标
    public static readonly int gameAreaWidth = BLOCKWIDTH * BlockCountX;	//游戏区域宽度
    public static readonly int gameAreaHeight = BLOCKWIDTH * BlockCountY + BlockCountY / 2;//游戏区域高度
    
    public static readonly bool CanMoveWhenDroping = true;			//是否支持下落的同时移动
    public static readonly int PROGRESSTOWIN = 2000;
    public static readonly int DROP_TIME = 120;			//下落的时间
    public static readonly int MOVE_TIME = 250;    		//移动的时间
    public static readonly int EATBLOCK_TIME = 200;		//消块时间
    public static readonly int GAMETIME = 6000000;		//游戏时间

    public StageData PlayingStageData;                      //当前的关卡数据

    ///游戏逻辑变量/////////////////////////////////////////////////////////////////
	TDirection m_moveDirection;							                //选择的块1向块2移动的方向
	Position [] m_selectedPos = new Position[2];		                //记录两次点击选择的方块
    CapBlock[,] m_blocks = new CapBlock[BlockCountX, BlockCountY];		//屏幕上方块的数组
	int m_progress;										//当前进度
	TGameState m_gameState;								//游戏状态
	int m_comboCount;				//记录当前连击数
	bool m_changeBack;		//在交换方块动画中标志是否为换回动画
    System.Random m_random;
    long m_gameStartTime = 0;                              //游戏开始时间

    ///统计数据///////////////////////////////////////////////////////////////////////
    long m_lastClickTime;				//上次点击时间
    long m_rhythmMark;				//节奏感得分
    long m_gameTakeTime;				//一局游戏所用时间
    int m_totalClickCount;		//总共点击次数
    int m_workedClickCount;	//有效点击次数
    int m_maxComboCount;		//最大combo数
    int m_totalComboCount;			//本局总连击数

    LinkedList<long> m_perClickTakeTime = new LinkedList<long>();				//每次点击所用时间记录
    int m_rpMark;								//人品得分

    //计时器
    Timer timerMoveBlock = new Timer();
    Timer timerEatBlock = new Timer();
    Timer timerDropDown = new Timer();

    	//资源物件
	//CCAnimation *m_blockMoveAni[6][6];			//六种方块向六个方向移动的动画
	//CCSprite * m_selectAni;						//选中动画
	//BlockSprite * m_pComboSprite[5];			//Combo粒子的精灵
	Dictionary<int, LinkedList<Transform> >   m_availableSprite = new Dictionary<int, LinkedList<Transform>>();				//用来存放可用的Sprite
	Dictionary<int, string> m_soundEffectMap = new Dictionary<int, string>();								    //声音

    LinkedList<Paticle> m_paticleList;			//粒子列表
	//std::vector<NumberDrawer *> m_endScoreBoardNumsVec;		//结束面板上的一堆数字

	//NumberDrawer * m_comboNumDrawer;		//用来绘制Combo数字
	NumberDrawer m_progressNumDrawer = new NumberDrawer();		//用来绘制进度数字
	NumberDrawer m_pSecNum = new NumberDrawer();				//用来绘制秒数
	NumberDrawer m_pMicroSecNum = new NumberDrawer();			//用来绘制微秒数

    GameObject m_freePool;
    GameObject m_capsPool;

    public GameLogic()
    {
        m_freePool = GameObject.Find("FreePool");
        m_capsPool = GameObject.Find("CapsPool");
    }

    public void Init()
    {
        //初始化瓶盖图片池
        string name;
        for (int i = 0; i < ColorCount; ++i )
        {
            name = "Item" + (i + 1);
            m_availableSprite[i] = new LinkedList<Transform>();
            GameObject capObj = GameObject.Find("CapInstance" + (i+1));
            m_availableSprite[i].AddFirst(capObj.transform);
            for (int j = 0; j < 40; ++j )
            {
                GameObject newObj = GameObject.Instantiate(capObj) as GameObject;
                MakeSpriteFree(TBlockColor.EColor_White + i, newObj.transform);
                newObj.transform.localScale = new Vector3(58.0f, 58.0f, 1.0f);
                newObj.transform.localPosition = Vector3.zero;
            }
        }

        for (int i = 0; i < BlockCountX; i++)
            for (int j = 0; j < BlockCountY; j++)
            {
                m_blocks[i, j] = new CapBlock();
            }

        m_selectedPos[0] = new Position();
        m_selectedPos[1] = new Position();
    }

    public void StartGame()     //开始游戏（及重新开始游戏）
    {
        Timer.s_currentTime = Time.realtimeSinceStartup;        //更新一次时间
        long time = Timer.millisecondNow();
        m_random = new System.Random((int)(Time.realtimeSinceStartup * 1000));
        m_lastClickTime = time;
        m_gameStartTime = time;
        //srand(time);

        PlayingStageData = StageData.CreateStageData();
        PlayingStageData.LoadStageData(GlobalVars.CurStageNum);

        for (int i = 0; i < BlockCountX; i++)
        {
            bool bHasBirth = false;
            for (int j = 0; j < BlockCountY; j++)
            {
                if (PlayingStageData.GridDataArray[i, j].grid == TGridType.None)
                {
                    continue;
                }
                if (!bHasBirth)
                {
                    PlayingStageData.GridDataArray[i, j].bBirth = true;             //每列的第一个位置为出生点
                    bHasBirth = true;
                }
                m_blocks[i, j].color = GetRandomColor();
                while (IsHaveLine(new Position(i, j))) m_blocks[i, j].color = GetNextColor(m_blocks[i, j].color);		//若新生成瓶盖的造成消行，换一个颜色
                m_blocks[i, j].Reset();
                m_blocks[i, j].m_blockTransform = GetFreeBlockSprite(m_blocks[i, j].color);
                m_blocks[i, j].m_blockSprite = m_blocks[i, j].m_blockTransform.GetComponent<UISprite>();
            }
        }

        UpdateProgress();
    }

    public void ClearGame()
    {
        m_progress = 0;
        for (int i = 0; i < BlockCountX; i++)
            for (int j = 0; j < BlockCountY; j++)
            {
                if (PlayingStageData.GridDataArray[i, j].grid == TGridType.None)
                {
                    continue;
                }
                MakeSpriteFree(m_blocks[i, j].color, m_blocks[i, j].m_blockTransform);
                CreateSpecialBlock(TSpecialBlock.ESpecial_Normal, new Position(i, j));
                m_blocks[i, j].color = TBlockColor.EColor_None;
                m_blocks[i, j].m_blockTransform = null;
                m_blocks[i, j].m_blockSprite = null;
                m_blocks[i, j].Reset();
            }
    }

    public void Update()
    {
        Timer.s_currentTime = Time.realtimeSinceStartup;

        if (m_gameStartTime == 0)           //游戏没到开始状态
        {
            return;
        }
		//Timer.s_currentTime = Timer.s_currentTime + 0.02f;		//

        TimerWork();

        Color curColor = new Color(1.0f, 1.0f, 1.0f, 1.0f - Mathf.Clamp01((float)timerEatBlock.GetTime() / EATBLOCK_TIME));
		Color defaultColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);

        //根据数据绘制Sprite
        for (int i = 0; i < BlockCountX; i++)
        {
            for (int j = 0; j < BlockCountY; j++)
            {
                if (PlayingStageData.GridDataArray[i, j].grid == TGridType.None)
                {
                    continue;
                }
                if (m_blocks[i,j].color != TBlockColor.EColor_None)
                {
                    m_blocks[i, j].m_blockTransform.localPosition = new Vector3(gameAreaX + i * BLOCKWIDTH + m_blocks[i, j].x_move + BLOCKWIDTH / 2, -(gameAreaY + j * BLOCKWIDTH + m_blocks[i,j].y_move + (i + 1) % 2 * BLOCKWIDTH / 2 + BLOCKWIDTH / 2), -105);
                    if ( m_blocks[i, j].m_blockSprite != null)
                    {
                        if (m_blocks[i, j].IsEating())
                        {
                            m_blocks[i, j].m_blockSprite.color = curColor;
                            UIDrawer.Singleton.DrawNumber("Score" + i + "," + j, (int)m_blocks[i, j].m_blockTransform.localPosition.x, -(int)m_blocks[i, j].m_blockTransform.localPosition.y, 60, "HighDown", 15);
                        }
                        else
                        {
                            m_blocks[i, j].m_blockSprite.color = defaultColor;
                        }
                    }
                }

                if (PlayingStageData.GridDataArray[i, j].grid == TGridType.Normal)
                {
                    UIDrawer.Singleton.DrawSprite("Grid" + i + "," + j, gameAreaX + i * BLOCKWIDTH + BLOCKWIDTH / 2, gameAreaY + j * BLOCKWIDTH + (i + 1) % 2 * BLOCKWIDTH / 2 + BLOCKWIDTH / 2, "Grid0");
                }
                if (PlayingStageData.GridDataArray[i, j].grid == TGridType.Jelly)
                {
                    UIDrawer.Singleton.DrawSprite("Jelly" + i + "," + j, gameAreaX + i * BLOCKWIDTH + BLOCKWIDTH / 2, gameAreaY + j * BLOCKWIDTH + (i + 1) % 2 * BLOCKWIDTH / 2 + BLOCKWIDTH / 2, "Grid1");
                }
                if (PlayingStageData.GridDataArray[i, j].grid == TGridType.JellyDouble)
                {
                    UIDrawer.Singleton.DrawSprite("Jelly2" + i + "," + j, gameAreaX + i * BLOCKWIDTH + BLOCKWIDTH / 2, gameAreaY + j * BLOCKWIDTH + (i + 1) % 2 * BLOCKWIDTH / 2 + BLOCKWIDTH / 2, "Grid2");
                }
            }
        }
        if (Time.deltaTime > 0.02f)
        {
            Debug.Log("DeltaTime = " + Time.deltaTime);
        }

        if (GlobalVars.CurStageData.StepLimit > 0)          //限制步数的关卡
        {
            UIDrawer.Singleton.DrawText("StepLimitText", 100, 575, "Steps:");
            UIDrawer.Singleton.DrawNumber("SetpLimit", 140, 586, PlayingStageData.StepLimit, "HighDown", 14);
            if (PlayingStageData.StepLimit == 0)            //
            {
                if (timerDropDown.GetState() == TimerEnum.EStop && timerEatBlock.GetState() == TimerEnum.EStop && timerMoveBlock.GetState() == TimerEnum.EStop)   //若没步数了，触发游戏结束
                {
                    UIWindowManager.Singleton.GetUIWindow<UIGameEnd>().ShowWindow();
                    m_gameStartTime = 0;
                }
            }
        }
        if (GlobalVars.CurStageData.TimeLimit > 0)          //限制时间的关卡
        {
            UIDrawer.Singleton.DrawText("TimeLimitText", 100, 575, "Time:");
            float timeRemain = GlobalVars.CurStageData.TimeLimit - (Timer.millisecondNow() - m_gameStartTime) / 1000.0f;

            if (timeRemain < 0)     
            {
                timeRemain = 0;
                if (timerDropDown.GetState() == TimerEnum.EStop && timerEatBlock.GetState() == TimerEnum.EStop && timerMoveBlock.GetState() == TimerEnum.EStop)   //若没时间了，触发游戏结束
                {
                    UIWindowManager.Singleton.GetUIWindow<UIGameEnd>().ShowWindow();
                    m_gameStartTime = 0;
                }
            }

            UIDrawer.Singleton.DrawNumber("TimeLimit", 140, 586, timeRemain, "HighDown", 14, 2, 2);
        }
    }

    void TimerWork()
    {
        /*------------------处理timerEatBlock------------------*/
        if (timerEatBlock.GetState() == TimerEnum.ERunning)
        {			//如果消块计时器状态为开启
            if (timerEatBlock.GetTime() > EATBLOCK_TIME)
            {		//消块计时器到400毫秒
                timerEatBlock.Stop();

                //消块逻辑，把正在消失的块变成粒子，原块置空
                for (int i = 0; i < BlockCountX; i++)
                    for (int j = 0; j < BlockCountY; j++)
                    {
                        if (PlayingStageData.GridDataArray[i, j].grid == TGridType.None)
                        {
                            continue;
                        }
                        if (m_blocks[i, j].IsEating())
                        {
                            //PlayAni(i, j, 0, true);
                            //CreateCapParticle(i, j);
                            //清空block信息
                            MakeSpriteFree(m_blocks[i, j].color, m_blocks[i, j].m_blockTransform);
                            CreateSpecialBlock(TSpecialBlock.ESpecial_Normal, new Position(i, j));
                            m_blocks[i, j].color = TBlockColor.EColor_None;
                            m_blocks[i, j].m_blockTransform = null;
                            m_blocks[i, j].m_blockSprite = null;
                            m_blocks[i, j].Reset();
                        }
                    }

                //AddCombo();

                if (timerDropDown.GetState() == TimerEnum.EStop)		//若没在下落状态，调用下落。（若在下落状态，等下落计时器到了自然会处理）
                {
                    DropDown();										//下落逻辑
                }
            }
        }

        /*------------------处理timerDropDown------------------*/
        if (timerDropDown.GetState() == TimerEnum.ERunning)
        {
            if (timerDropDown.GetTime() > DROP_TIME)	//如果下落计时器到120毫秒
            {
                timerDropDown.Stop();

                //检查那些块需要检查是否能消
                for (int i = 0; i < BlockCountX; i++)
                {
                    for (int j = BlockCountY - 1; j >= 0; j--)			//从下往上遍历
                    {
                        if (PlayingStageData.GridDataArray[i, j].grid == TGridType.None)
                        {
                            continue;
                        }
                        if (m_blocks[i, j].color == TBlockColor.EColor_None)			//若发现空格，中断，证明这列上面所有的块都没落到底
                        {
                            break;
                        }
                        if (m_blocks[i, j].isDropping)						//若正在下降且
                        {
                            m_blocks[i, j].m_bNeedCheckEatLine = true;		//完成了下落，等待检查消行
                            m_blocks[i, j].isCanMove = true;				//清除不可移动状态
                        }
                    }
                }

                //清空方块的下落标志和偏移量
                for (int i = 0; i < BlockCountX; i++)
                {
                    for (int j = 0; j < BlockCountY; j++)
                    {
                        if (PlayingStageData.GridDataArray[i, j].grid == TGridType.None)
                        {
                            continue;
                        }
                        m_blocks[i, j].isDropping = false;
                        m_blocks[i, j].x_move = 0;
                        m_blocks[i, j].y_move = 0;
                    }
                }

                if (!DropDown())		//尝试下落，若不能
                {
                    //PlaySound(dropdown);				//先播个落地声音
                    for (int i = 0; i < BlockCountX; i++)		//清空数据
                    {
                        for (int j = 0; j < BlockCountY; j++)
                        {
                            if (PlayingStageData.GridDataArray[i, j].grid == TGridType.None)
                            {
                                continue;
                            }
                            m_blocks[i, j].isCanMove = true;
                        }
                    }
                    if (!EatAllLine())					//若没有能消了的
                    {
                        m_comboCount = 0;
                        OnDropEnd();
                    }
                    else								//若有能直接消除的块
                    {
                        timerEatBlock.Play();
                        timerEatBlock.Adjust(5);		//Bug,不知道为什么这里必须停一下，才能消所有行
                    }
                }

            }
            //如果未到120毫秒，更新各方快的位置
            for (int i = 0; i < BlockCountX; i++)
            {
                for (int j = 0; j < BlockCountY; j++)
                {
                    if (PlayingStageData.GridDataArray[i, j].grid == TGridType.None)
                    {
                        continue;
                    }
                    if (m_blocks[i, j].isDropping)
                    {
                        m_blocks[i, j].y_move = (int)timerDropDown.GetTime() * BLOCKWIDTH / DROP_TIME - BLOCKWIDTH;
                    }
                }
            }
        }

        /*------------------处理timerMoveBlock------------------*/
        if (timerMoveBlock.GetState() == TimerEnum.ERunning)		//如果交换方块计时器状态为开启
        {
            int passTime = (int)timerMoveBlock.GetTime();
            if (passTime > MOVE_TIME)	//交换方块计时器到了MOVE_TIME
            {
                timerMoveBlock.Stop();				//停止计时器
                //清空方块的偏移值
                m_blocks[m_selectedPos[0].x, m_selectedPos[0].y].x_move = 0;
                m_blocks[m_selectedPos[0].x, m_selectedPos[0].y].y_move = 0;
                m_blocks[m_selectedPos[1].x, m_selectedPos[1].y].x_move = 0;
                m_blocks[m_selectedPos[1].x, m_selectedPos[1].y].y_move = 0; ;
                if (m_changeBack)//如果处于换回状态
                {
                    m_changeBack = false;	//清除换回标志
                    if (m_selectedPos[0].x == -1 || m_selectedPos[1].x == -1) return;

                    bool hasEatLine1 = EatLine(m_selectedPos[0]);
                    bool hasEatLine2 = EatLine(m_selectedPos[1]);

                    if (hasEatLine1 || hasEatLine2)				//若有能消的
                    {
                        //PlaySound(eat);

                        if (m_selectedPos[0].x == -1 || m_selectedPos[1].x == -1) return;			//若什么都没选这里返回

                        timerEatBlock.Play();												//开启消块计时器
                    }

                    //清空选择的方块
                    ClearSelected();
                }
                else
                {
                    bool hasEatLine1 = EatLine(m_selectedPos[0]);
                    bool hasEatLine2 = EatLine(m_selectedPos[1]);
                    if (!hasEatLine1 && !hasEatLine2)//如果交换不成功,播放交换回来的动画
                    {
                        ++PlayingStageData.StepLimit;           //步数恢复
                        ExchangeBlock(m_selectedPos[0], m_selectedPos[1]);
                        timerMoveBlock.Play();
                        //PlayAni(m_selectedPos[1].x, m_selectedPos[1].y,GetOtherDirection(m_moveDirection));
                        //PlayAni(m_selectedPos[0].x, m_selectedPos[0].y,m_moveDirection);
                        m_changeBack = true;
                    }
                    else
                    {					//如果交换成功
                        m_workedClickCount++;
                        //PlaySound(eat);

                        if (m_selectedPos[0].x == -1 || m_selectedPos[1].x == -1) return;

                        ClearSelected();
                        timerEatBlock.Play();												//开启消块计时器
                    }
                }
            }

            passTime = (int)timerMoveBlock.GetTime();
            int moveTime = MOVE_TIME - passTime;		//重新获取一次passtime，因为前面有可能被刷新过
            if (m_selectedPos[0].x == -1 || m_selectedPos[1].x == -1) return;
            if (m_selectedPos[0].x != m_selectedPos[1].x)		//若x方向上的值不一样，就有x方向上的移动
            {
                m_blocks[m_selectedPos[0].x, m_selectedPos[0].y].x_move = (m_selectedPos[1].x - m_selectedPos[0].x) * moveTime * BLOCKWIDTH / MOVE_TIME;
                m_blocks[m_selectedPos[1].x, m_selectedPos[1].y].x_move = (m_selectedPos[0].x - m_selectedPos[1].x) * moveTime * BLOCKWIDTH / MOVE_TIME;
            }
            if (m_selectedPos[0].x - m_selectedPos[1].x == 0)
            {
                m_blocks[m_selectedPos[0].x, m_selectedPos[0].y].y_move = (m_selectedPos[1].y - m_selectedPos[0].y) * moveTime * BLOCKWIDTH / MOVE_TIME;
                m_blocks[m_selectedPos[1].x, m_selectedPos[1].y].y_move = (m_selectedPos[0].y - m_selectedPos[1].y) * moveTime * BLOCKWIDTH / MOVE_TIME;
            }
            else
            {
                if (m_selectedPos[0].y != m_selectedPos[1].y)
                {
                    m_blocks[m_selectedPos[0].x, m_selectedPos[0].y].y_move = (m_selectedPos[1].y - m_selectedPos[0].y) * moveTime * (BLOCKWIDTH / 2) / MOVE_TIME;
                    m_blocks[m_selectedPos[1].x, m_selectedPos[1].y].y_move = (m_selectedPos[0].y - m_selectedPos[1].y) * moveTime * (BLOCKWIDTH / 2) / MOVE_TIME;
                }
                else
                {
                    if (m_selectedPos[0].x % 2 == 0)
                    {
                        m_blocks[m_selectedPos[0].x, m_selectedPos[0].y].y_move = 0 - moveTime * (BLOCKWIDTH / 2) / MOVE_TIME;
                        m_blocks[m_selectedPos[1].x, m_selectedPos[1].y].y_move = moveTime * (BLOCKWIDTH / 2) / MOVE_TIME;
                    }
                    else
                    {
                        m_blocks[m_selectedPos[0].x, m_selectedPos[0].y].y_move = moveTime * (BLOCKWIDTH / 2) / MOVE_TIME;
                        m_blocks[m_selectedPos[1].x, m_selectedPos[1].y].y_move = 0 - moveTime * (BLOCKWIDTH / 2) / MOVE_TIME;
                    }
                }
            }
        }
    }

    bool DropDown()
    {
        //下落块，所有可下落区域下落一行////////////////////////////////////////////////////////////////////////
        bool tag = false;
        Position dropDest = new Position();          //掉落的目标点
        Position dropFrom = new Position();          //从哪里开始掉落 
        for (int j = BlockCountY - 1; j >= 0; j--)		//从最下面开始遍历
        {
            for (int i = 0; i < BlockCountX; i++)				//一次遍历一行
            {
                if (PlayingStageData.GridDataArray[i, j].grid == TGridType.None)
                {
                    continue;
                }
                if (!m_blocks[i,j].IsEmpty())			//若有块，看它下方是否为空格
                {
                    dropFrom.Set(i, j);         //设立一个掉落目标

                    dropDest.Set(i, j + 1);     //设立一个基本的掉落目标点

                    //一直向下找到不是镂空的有效点
                    while (CheckPosAvailable(dropDest) && 
                        PlayingStageData.GridDataArray[dropDest.x, dropDest.y].grid == TGridType.None)
                    {
                        ++dropDest.y;
                    }

                    if (CheckPosAvailable(dropDest) && m_blocks[dropDest.x, dropDest.y].color == TBlockColor.EColor_None)        //若是有效点，形成掉落
                    {
                        m_blocks[dropDest.x, dropDest.y].Assign(m_blocks[dropFrom.x, dropFrom.y]);
                        m_blocks[dropDest.x, dropDest.y].isDropping = true;



                        //若是出生点，生成新块////////////////////////////////////////////////////////////////////////
                        if (PlayingStageData.GridDataArray[dropFrom.x, dropFrom.y].bBirth)
                        {
                            //补充新的方块
                            m_blocks[dropFrom.x, dropFrom.y].color = GetRandomColor();		//最上方获取新的方块
                            m_blocks[dropFrom.x, dropFrom.y].Reset();
                            m_blocks[dropFrom.x, dropFrom.y].isDropping = true;
                            m_blocks[dropFrom.x, dropFrom.y].m_blockTransform = GetFreeBlockSprite(m_blocks[dropFrom.x, dropFrom.y].color);
                            m_blocks[dropFrom.x, dropFrom.y].m_blockSprite = m_blocks[dropFrom.x, dropFrom.y].m_blockTransform.GetComponent<UISprite>();
                        }
                        else
                        {
                            m_blocks[dropFrom.x, dropFrom.y].color = TBlockColor.EColor_None;
                            m_blocks[dropFrom.x, dropFrom.y].m_blockSprite = null;
                            m_blocks[dropFrom.x, dropFrom.y].m_blockTransform = null;
                        }

                        tag = true;
                    }

                    ////先看下面的格子是否为镂空，若为镂空，一直向下查找不镂空的
                    //if (PlayingStageData.GridDataArray[dropFrom.x, dropFrom.y + 1].grid == TGridType.None)      //若为镂空
                    //{

                    //}
                    //if (DropBlockFrom(dropFrom, TDirection.EDir_Down, 1))     //尝试往正下方掉落
                    //{
                    //    tag = true;
                    //}
                    /*else if (DropBlockTo(dropDest, TDirection.EDir_UpRight))
                    {
                        tag = true;
                    }
                    else if (DropBlockTo(dropDest, TDirection.EDir_LeftDown))
                    {
                        tag = true;
                    }*/

                    ////补充新的方块
                    //m_blocks[i,0].color = GetRandomColor();		//最上方获取新的方块
                    //m_blocks[i,0].Reset();
                    //m_blocks[i,0].isDropping = true;
                    //m_blocks[i,0].m_blockTransform = GetFreeBlockSprite(m_blocks[i,0].color);
                    //m_blocks[i,0].m_blockSprite = m_blocks[i,0].m_blockTransform.GetComponent<UISprite>();
                    //break;
                }
            }
        }

        //需要补充遍历所有出生点
        for (int j = BlockCountY - 1; j >= 0; j--)		//从最下面开始遍历
        {
            for (int i = 0; i < BlockCountX; i++)				//一次遍历一行
            {
                if (PlayingStageData.GridDataArray[i,j].bBirth && m_blocks[i, j].IsEmpty())     //若为出生点
                {
                    //补充新的方块
                    m_blocks[i,j].color = GetRandomColor();		//最上方获取新的方块
                    m_blocks[i, j].Reset();
                    m_blocks[i, j].isDropping = true;
                    m_blocks[i, j].m_blockTransform = GetFreeBlockSprite(m_blocks[i, j].color);
                    m_blocks[i, j].m_blockSprite = m_blocks[i, j].m_blockTransform.GetComponent<UISprite>();
                    tag = true;
                }
            }
        }

        if (tag)
            timerDropDown.Play();								//开启自由下落计时器

        return tag;			//返回是否发生了掉落
    }

    bool EatAllLine()
    {
        bool tag = false;
        CapBlock pBlock = null;
        for (int i = 0; i < BlockCountX; i++)
        {
            for (int j = 0; j < BlockCountY; j++)
            {
                if (PlayingStageData.GridDataArray[i, j].grid == TGridType.None)
                {
                    continue;
                }
                pBlock = GetBlock(new Position(i, j));
                if (pBlock.m_bNeedCheckEatLine && !pBlock.IsEating())		//只有没在消除状态的块才遍历，防止重复记分
                {
                    if (EatLine(new Position(i, j)))
                    {
                        tag = true;
                    }
                }
            }
        }
        return tag;
    }

    bool EatLine(Position position)
    {
        int countInSameLine = 1;					//在同一条线上相同的颜色的数量
        int totalSameCount = 1;						//总共相同颜色的数量
        int maxCountInSameDir = 1;					//最大的在同一个方向上相同颜色的数量
        int step = 1;
        Position[] eatBlockPos = new Position[10];
        eatBlockPos[0] = position;

        TBlockColor color = GetBlockColor(position);
        Position curPos;
        for (TDirection dir = TDirection.EDir_Up; dir <= TDirection.EDir_DownRight; dir = (TDirection)(dir + 1))		//遍历3个方向
        {
            countInSameLine = 1;
            curPos = position;
            while (true)
            {
                curPos = GoTo(curPos, dir, 1);									//沿着Dir方向走一步
                if (!CheckPosAvailable(curPos))
                {
                    break;
                }
                if (GetBlockColor(curPos) != color)								//若碰到不一样的颜色就停下来
                {
                    break;
                }
                eatBlockPos[countInSameLine] = curPos;								//把Block存起来（用来后面消除）
                ++countInSameLine;
            }
            curPos = position;														//重置位置
            while (true)
            {
                curPos = GoTo(curPos, GetOtherDirection(dir), 1);					//沿着Dir反方向走
                if (!CheckPosAvailable(curPos))
                {
                    break;
                }
                if (GetBlockColor(curPos) != color)								//若碰到不一样的颜色就停下来
                {
                    break;
                }
                eatBlockPos[countInSameLine] = curPos;
                ++countInSameLine;
            }
            //一条线处理一次消除
            if (countInSameLine >= 3)
            {
                for (int i = 0; i < countInSameLine; ++i)
                {
                    if (eatBlockPos[i] != position)										//起始块放在后面处理
                    {
                        EatBlock(eatBlockPos[i]);										//吃掉
                    }
                }
                totalSameCount += (countInSameLine - 1);							//记录总的消除数量，减1是因为起始块是各条线公用的
            }
            if (countInSameLine > maxCountInSameDir)
            {
                maxCountInSameDir = countInSameLine;								//记录在单行中的最大消除数量
            }
        }

        if (maxCountInSameDir < 3)		//若没产生消除，返回
        {
            return false;
        }

        if (totalSameCount == 3)		//总共就消了3个
        {
            EatBlock(position);
        }
        //TODO 生成道具
        //根据结果来生成道具////////////////////////////////////////////////////////////////////////
		else if (maxCountInSameDir >= 5)		//若最大每行消了5个
        {
            CreateSpecialBlock(TSpecialBlock.ESpecial_EatAColor, position);
        }
		else if (totalSameCount >= 6)			//若总共消除大于等于6个（3,4消除或者多个3消）
        {
            CreateSpecialBlock(TSpecialBlock.ESpecial_Bomb, position);
        }
        else if (maxCountInSameDir == 4)		//若最大每行消了4个
        {
            if (m_moveDirection == TDirection.EDir_Up || m_moveDirection == TDirection.EDir_Down)
            {
                CreateSpecialBlock(TSpecialBlock.ESpecial_EatLineDir0, position);
            }
            if (m_moveDirection == TDirection.EDir_UpRight || m_moveDirection == TDirection.EDir_LeftDown)
            {
                CreateSpecialBlock(TSpecialBlock.ESpecial_EatLineDir1, position);
            }
            if (m_moveDirection == TDirection.EDir_DownRight || m_moveDirection == TDirection.EDir_LeftUp)
            {
                CreateSpecialBlock(TSpecialBlock.ESpecial_EatLineDir2, position);
            }
        }
        else if (totalSameCount > 4)			//若总共消除大于4个
        {
            CreateSpecialBlock(TSpecialBlock.ESpecial_Painter, position);
        }

        //TODO 记分
        ////根据结果来记分
        //float kQuantity = 1;
        //float kCombo = 1;
        //if ((totalSameCount - 3) >= MaxKQuanlity)
        //{
        //    kQuantity = KQuanlityTable[MaxKQuanlity-1];
        //}
        //else
        //{
        //    kQuantity = KQuanlityTable[totalSameCount-3];
        //}

        //if (m_comboCount >= MaxKCombo)
        //{
        //    kCombo = KComboTable[MaxKCombo-1];
        //}
        //else
        //{
        //    kCombo = KComboTable[m_comboCount];
        //}
        //m_progress += 50 * kQuantity * kCombo;
        UpdateProgress();
        return true;
    }

    Position FindRandomPos(TBlockColor excludeColor, Position [] excludePos)       //找到某个颜色的随机一个块, 简易算法，性能不好
    {
        int ranNum = m_random.Next()%(BlockCountX*BlockCountY);
        int count = 0;
        for (int i = 0; i < BlockCountX; i++)				//遍历一行
        {
            for (int j = 0; j < BlockCountY; j++)		//遍历一列
            {
                if (PlayingStageData.GridDataArray[i, j].grid == TGridType.None)
                {
                    continue;
                }
                if (count < ranNum)
                {
					++count;
                    continue;
                }
                if (m_blocks[i, j].color == TBlockColor.EColor_None)
                {
                    continue;
                }
                if (m_blocks[i, j].color == excludeColor)
                {
                    continue;
                }
                if (m_blocks[i,j].special == TSpecialBlock.ESpecial_EatAColor)
                {
                    continue;
                }
                Position pos = new Position(i, j);
                bool bFind = false;
                for (int k = 0; k < excludePos.Length; ++k )
                {
                    if (excludePos[k] == pos)
                    {
                        bFind = true;
                        break;
                    }
                }
                if (!bFind)
                {
                    return pos;
                }
                if(i == BlockCountX -1 && j == BlockCountY -1)//Repeat the loop till get a result
				{
					i=0;
					j=0;
				}
            }
        }
        return new Position(0, 0);
    }

    void ChangeColor(Position pos, TBlockColor color)
    {
        //更改颜色的操作
        MakeSpriteFree(m_blocks[pos.x, pos.y].color, m_blocks[pos.x, pos.y].m_blockTransform);
        m_blocks[pos.x, pos.y].m_blockTransform = GetFreeBlockSprite(color);
        m_blocks[pos.x, pos.y].m_blockSprite = m_blocks[pos.x, pos.y].m_blockTransform.GetComponent<UISprite>();
        m_blocks[pos.x, pos.y].color = color;
    }

    void EatBlock(Position position)
    {
        if (position.x >= BlockCountX || position.y >= BlockCountY || position.x < 0 || position.y < 0)
            return;
        if (m_blocks[position.x, position.y].x_move > 0 || m_blocks[position.x, position.y].y_move > 0)
            return;
        if (m_blocks[position.x, position.y].IsEating())        //不重复消除
        {
            return;
        }
        for (int i = 0; i <= position.y; i++)
        {
            m_blocks[position.x, i].isCanMove = false;      //上方所有块都不能移动
        }
        m_blocks[position.x, position.y].Eat();			//吃掉当前块

        if (PlayingStageData.GridDataArray[position.x, position.y].grid == TGridType.Jelly)
        {
            PlayingStageData.GridDataArray[position.x, position.y].grid = TGridType.Normal;
        }
        else if (PlayingStageData.GridDataArray[position.x, position.y].grid == TGridType.JellyDouble)
        {
            PlayingStageData.GridDataArray[position.x, position.y].grid = TGridType.Jelly;
        }

        switch (m_blocks[position.x, position.y].special)
        {
            case TSpecialBlock.ESpecial_Bomb:
                {
                    for (TDirection dir = TDirection.EDir_Up; dir <= TDirection.EDir_LeftUp; ++dir )
                    {
                        //TODO 这里要放特效
                        Position newPos = GoTo(position, dir, 1);
                        EatBlock(newPos);
                    }
                }
                break;
            case TSpecialBlock.ESpecial_Painter:
                {
                    Position[] excludePos = new Position[4];
                    for (int i = 0; i < 4; ++i )
                    {
                        excludePos[i] = position;           //先用当前位置初始化排除的位置数组
                    }
                    for (int i = 0; i < 3; ++i )            //取得随机点
                    {
                        excludePos[i + 1] = FindRandomPos(m_blocks[position.x, position.y].color, excludePos);
                    }
                    //TODO 这里要放特效
                    for (int i = 1; i < 4; ++i )
                    {
                        ChangeColor(excludePos[i], m_blocks[position.x, position.y].color);
                    }
                }
                break;
            case TSpecialBlock.ESpecial_EatLineDir0:
                {
                    for (int i = 0; i < BlockCountX; ++i )
                    {
                        EatBlock(GoTo(position, TDirection.EDir_Down, i));
                        EatBlock(GoTo(position, TDirection.EDir_Up, i));
                    }
                }
                break;
            case TSpecialBlock.ESpecial_EatLineDir1:
                {
                    for (int i = 1; i < BlockCountX - 1; ++i)
                    {
                        EatBlock(GoTo(position, TDirection.EDir_UpRight, i));
                        EatBlock(GoTo(position, TDirection.EDir_LeftDown, i));
                    }
                }
                break;
            case TSpecialBlock.ESpecial_EatLineDir2:
                {
                    for (int i = 0; i < BlockCountX; ++i)
                    {
                        EatBlock(GoTo(position, TDirection.EDir_LeftUp, i));
                        EatBlock(GoTo(position, TDirection.EDir_DownRight, i));
                    }
                }
                break;
            case TSpecialBlock.ESpecial_EatAColor:
                {

                }
                break;
        }
    }

    int GetJellyCount()
    {
        int count = 0;
        for (int i = 0; i < BlockCountX; i++)				//遍历一行
        {
            for (int j = 0; j < BlockCountY; j++)		//遍历一列
            {
                if (PlayingStageData.GridDataArray[i, j].grid == TGridType.Jelly || PlayingStageData.GridDataArray[i, j].grid == TGridType.JellyDouble)
                {
                    ++count;
                }
            }
        }
        return count;
    }

    public bool CheckStageFinish()
    {
        if (PlayingStageData.Target == GameTarget.ClearJelly)       //若目标为清果冻，计算果冻数量
        {
            if (GetJellyCount() == 0)       //若完成目标
            {
                return true;
            }
        }
        else if (PlayingStageData.Target == GameTarget.BringFruitDown)      //看看水果有没有都落地
        {

        }
        else if (PlayingStageData.Target == GameTarget.GetScore)
        {
            if (m_progress > PlayingStageData.StarScore[0])                 //大于1星就算完成
            {
                return true;
            }
        }
        return false;
    }

    void OnDropEnd()            //所有下落和移动结束时被调用
    {
        if (CheckStageFinish())
        {
            UIWindowManager.Singleton.GetUIWindow<UIRetry>().ShowWindow();
        }
    }


    void CreateSpecialBlock(TSpecialBlock specailType, Position pos)
    {
        m_blocks[pos.x, pos.y].special = specailType;
        switch (specailType)
        {
            case TSpecialBlock.ESpecial_Normal:
                {
                    m_blocks[pos.x, pos.y].m_blockSprite.spriteName = "Item" + (int)(m_blocks[pos.x, pos.y].color - TBlockColor.EColor_None);
                }
                break;
            case TSpecialBlock.ESpecial_EatLineDir0:
                m_blocks[pos.x, pos.y].m_blockSprite.spriteName = "Line" + (int)(m_blocks[pos.x, pos.y].color - TBlockColor.EColor_None) + "_3";
                break;
            case TSpecialBlock.ESpecial_EatLineDir1:
                m_blocks[pos.x, pos.y].m_blockSprite.spriteName = "Line" + (int)(m_blocks[pos.x, pos.y].color - TBlockColor.EColor_None) + "_1";
                break;
            case TSpecialBlock.ESpecial_EatLineDir2:
                m_blocks[pos.x, pos.y].m_blockSprite.spriteName = "Line" + (int)(m_blocks[pos.x, pos.y].color - TBlockColor.EColor_None) + "_2";
                break;
            case TSpecialBlock.ESpecial_Bomb:
                m_blocks[pos.x, pos.y].m_blockSprite.spriteName = "Bomb" + (int)(m_blocks[pos.x, pos.y].color - TBlockColor.EColor_None);
                break;
            case TSpecialBlock.ESpecial_Painter:
                {
                    m_blocks[pos.x, pos.y].m_blockSprite.spriteName = "Painter" + (int)(m_blocks[pos.x, pos.y].color - TBlockColor.EColor_None);
                }
                break;
            case TSpecialBlock.ESpecial_EatAColor:
                m_blocks[pos.x, pos.y].m_blockSprite.spriteName = "Rainbow";
                break;
            default:
                break;
        }
    }

    public void OnTap(int x, int y)
    {
        //不在游戏区，不处理
        if (x < gameAreaX || y < gameAreaY || x > gameAreaX + gameAreaWidth || y > gameAreaY + gameAreaHeight)
        {
            return;
        }

        Position p = new Position();
        p.x = (x - gameAreaX) / BLOCKWIDTH;
        if (p.x % 2 == 0)
            p.y = (y - gameAreaY - BLOCKWIDTH / 2) / BLOCKWIDTH;
        else
            p.y = (y - gameAreaY) / BLOCKWIDTH;
        if (p.y > BlockCountY) p.y = BlockCountY;

        if (GlobalVars.EditState == TEditState.ChangeColor)
        {
            ChangeColor(p, GlobalVars.EditingColor);
        }

        if (GlobalVars.EditState == TEditState.ChangeSpecial)
        {
            CreateSpecialBlock(GlobalVars.EditingSpecial, p);
        }

        if (GlobalVars.EditState == TEditState.EditStageBlock)
        {
            PlayingStageData.GridDataArray[p.x, p.y].gridBlock = GlobalVars.EditingGridBlock;
        }

        if (GlobalVars.EditState == TEditState.EditStageGrid)
        {
            PlayingStageData.GridDataArray[p.x, p.y].grid = GlobalVars.EditingGrid;
        }

        if (GlobalVars.EditState == TEditState.Eat)
        {
            EatBlock(p);
            timerEatBlock.Play();												//开启消块计时器
        }
    }

    public void OnTouchBegin(int x, int y)
    {
	    //不在游戏区，先不处理
	    if(x < gameAreaX || y < gameAreaY || x > gameAreaX+gameAreaWidth || y > gameAreaY+gameAreaHeight)
	    {
		    return;
	    }

        if (timerMoveBlock.GetState() == TimerEnum.ERunning) return;

	    Position p = new Position();
	    p.x=(x-gameAreaX)/BLOCKWIDTH;
	    if(p.x%2==0)
		    p.y=(y-gameAreaY-BLOCKWIDTH/2)/BLOCKWIDTH;
	    else
		    p.y=(y-gameAreaY)/BLOCKWIDTH;
	    if(p.y>BlockCountY)p.y=BlockCountY;

	    //若不可选中，不处理
	    if(!m_blocks[p.x, p.y].SelectAble())
		    return;

	    //如果选中一个状态处于不可移动的块，或者一个特殊块，置选中标志为空，返回
	    if( ! m_blocks[p.x,p.y].SelectAble() )
	    {
		    ClearSelected();
		    return;
	    }
        m_selectedPos[0] = p;
        if (m_selectedPos[0].x == -1) return;
        m_totalClickCount++;
        m_selectedPos[1].x = -1;
        long clickTime = Timer.millisecondNow();
        m_perClickTakeTime.AddLast(clickTime - m_lastClickTime);
        m_lastClickTime = clickTime;
        //SetSelectAni(p.x, p.y);
    }

    public void OnTouchMove(int x, int y)
    {
        if (!CanMoveWhenDroping && timerDropDown.GetState() != TimerEnum.EStop)		//屏蔽下落时移动的代码
        {
            return;
        }

        //不在游戏区，先不处理
        if (x < gameAreaX || y < gameAreaY || x > gameAreaX + gameAreaWidth || y > gameAreaY + gameAreaHeight)
        {
            return;
        }

        if (timerMoveBlock.GetState() == TimerEnum.ERunning) return;

        if (m_selectedPos[0].x == -1)		//若没选好第一个块，不处理
        {
            return;
        }

        //计算位置
        Position p = new Position();
        p.x = (x - gameAreaX) / BLOCKWIDTH;
        if (p.x % 2 == 0)
            p.y = (y - gameAreaY - BLOCKWIDTH / 2) / BLOCKWIDTH;
        else
            p.y = (y - gameAreaY) / BLOCKWIDTH;
        if (p.y > BlockCountY - 1) p.y = BlockCountY - 1;

        if (m_selectedPos[0].x == p.x && m_selectedPos[0].y == p.y)		//若没移出第一个方块
        {
            return;
        }

        if (!GetBlock(p).SelectAble())
        {
            return;
        }

        //如果移到了连接方块
        if (!CheckTwoPosLinked(p, m_selectedPos[0]))
        {
            return;
        }

        //如果在选第二个块时，第一次选中的块已经变为不可移动，清空选中状态，返回
        if (!m_blocks[m_selectedPos[0].x, m_selectedPos[0].y].SelectAble())
        {
            ClearSelected();
            return;
        }

        //选中第二个
        m_selectedPos[1] = p;
        ProcessMove();
    }

    void ProcessMove()
    {
        if (PlayingStageData.StepLimit > 0)
        {
            TSpecialBlock special0 = m_blocks[m_selectedPos[0].x, m_selectedPos[0].y].special;
            TSpecialBlock special1 = m_blocks[m_selectedPos[1].x, m_selectedPos[1].y].special;
            if (special0 == TSpecialBlock.ESpecial_Normal
                && special1 == TSpecialBlock.ESpecial_Normal)
            {
                MoveBlockPair(m_selectedPos[0], m_selectedPos[1]);
            }
            else
            {
                //处理五彩块
                if (special0 == TSpecialBlock.ESpecial_EatAColor)
                {
                    if (special1 == TSpecialBlock.ESpecial_Normal)
                    {
                        EatAColor(m_blocks[m_selectedPos[1].x, m_selectedPos[1].y].color);
                    }
                    if (special1 == TSpecialBlock.ESpecial_EatLineDir0 || special1 == TSpecialBlock.ESpecial_EatLineDir2 ||
                        special1 == TSpecialBlock.ESpecial_EatLineDir1)
                    {
                        ChangeColorToLine(m_blocks[m_selectedPos[1].x, m_selectedPos[1].y].color);
                    }
                    if (special1 == TSpecialBlock.ESpecial_EatAColor)
                    {
                        EatAColor(TBlockColor.EColor_None);         //消全部
                    }
                }
                else if (special1 == TSpecialBlock.ESpecial_EatAColor)
                {
                    if (special0 == TSpecialBlock.ESpecial_Normal)
                    {
                        EatAColor(m_blocks[m_selectedPos[0].x, m_selectedPos[0].y].color);
                    }
                    if (special0 == TSpecialBlock.ESpecial_EatLineDir0 || special0 == TSpecialBlock.ESpecial_EatLineDir2 ||
                        special0 == TSpecialBlock.ESpecial_EatLineDir1)
                    {
                        ChangeColorToLine(m_blocks[m_selectedPos[0].x, m_selectedPos[0].y].color);
                    }
                }

                //处理条状块
                if (special0 == TSpecialBlock.ESpecial_EatLineDir0 || special0 == TSpecialBlock.ESpecial_EatLineDir1 || special0 == TSpecialBlock.ESpecial_EatLineDir2)
                {
                    if (special1 == TSpecialBlock.ESpecial_EatLineDir0 || special1 == TSpecialBlock.ESpecial_EatLineDir1 || special1 == TSpecialBlock.ESpecial_EatLineDir2)
                    {

                    }
                }
            }
        }
    }

    void EatAColor(TBlockColor color)
    {

    }

    void ChangeColorToLine(TBlockColor color)
    {

    }

    void MoveBlockPair(Position position1, Position position2)
    {
        --PlayingStageData.StepLimit;
        ExchangeBlock(position1, position2);			//交换方块

        //PlaySound(capsmove);							//移动
        timerMoveBlock.Play();							//开启计时器

        //计算方向
        if (position1.y > position2.y)
        {
            if (position1.x == position2.x)
            {
                m_moveDirection = TDirection.EDir_Up;
            }
            else if (position1.x > position2.x)
            {
                m_moveDirection = TDirection.EDir_DownRight;
            }
            else
            {
                m_moveDirection = TDirection.EDir_UpRight;
            }
        }
        else if (position1.y < position2.y)
        {
            if (position1.x == position2.x)
            {
                m_moveDirection = TDirection.EDir_Down;
            }
            else if (position1.x > position2.x)
            {
                m_moveDirection = TDirection.EDir_LeftDown;
            }
            else
            {
                m_moveDirection = TDirection.EDir_LeftUp;
            }
        }
        else
        {
            if (position1.x > position2.x)
            {
                if (position1.x % 2 == 0)
                {
                    m_moveDirection = TDirection.EDir_DownRight;
                }
                else
                {
                    m_moveDirection = TDirection.EDir_LeftDown;
                }
            }
            else
            {
                if (position1.x % 2 == 0)
                {
                    m_moveDirection = TDirection.EDir_UpRight;
                }
                else
                {
                    m_moveDirection = TDirection.EDir_LeftUp;
                }
            }
        }
        //播放动画
        //PlayAni(position1.x, position1.y,m_moveDirection);
        //PlayAni(position2.x, position2.y,GetOtherDirection(m_moveDirection));

        //SetSelectAni(-1, -1);
    }

    void ExchangeBlock(Position position1, Position position2)
    {
        CapBlock tempBlock = m_blocks[position1.x, position1.y];
        m_blocks[position1.x, position1.y] = m_blocks[position2.x, position2.y];
        m_blocks[position2.x, position2.y] = tempBlock;
    }

    Transform GetFreeBlockSprite(TBlockColor color)
    {
	    int type = (int)(color - TBlockColor.EColor_White);
        LinkedList<Transform> list = m_availableSprite[type];
	    Transform trans = list.Last.Value;
        trans.parent = m_capsPool.transform;
        trans.gameObject.SetActive(true);
        list.RemoveLast();
        return trans;
    }

    void MakeSpriteFree(TBlockColor color, Transform trans)
    {
        int type = (int)(color - TBlockColor.EColor_White);
        m_availableSprite[type].AddLast(trans);
        trans.parent = m_freePool.transform;
        trans.gameObject.SetActive(false);
    }

    void UpdateProgress()
    {

    }

    void ClearSelected()
    {
        m_selectedPos[0].x = -1;
        m_selectedPos[1].x = -1;
    }

    TBlockColor GetRandomColor()
    {
        return TBlockColor.EColor_White + m_random.Next() % ColorCount;
    }

    TBlockColor GetNextColor(TBlockColor color)
    {
        int index = color - TBlockColor.EColor_White;
        return (TBlockColor)((index + 1) % ColorCount + TBlockColor.EColor_White);
    }

    bool IsHaveLine(Position position)
    {
	    int countInSameLine = 1;					//在同一条线上相同的颜色的数量
	    int totalSameCount = 1;						//总共相同颜色的数量
	    int step = 1;
	    TBlockColor color = GetBlockColor(position);
	    Position curPos;
        for (TDirection dir = TDirection.EDir_Up; dir <= TDirection.EDir_DownRight; dir = (TDirection)(dir + 1))		//遍历3个方向
	    {
		    countInSameLine = 1;
		    curPos = position;
		    while (true)			
		    {
			    curPos = GoTo(curPos, dir, 1);									//沿着Dir方向走一步
			    if (GetBlockColor(curPos) != color)								//若碰到不一样的颜色就停下来
			    {
				    break;
			    }
			    ++countInSameLine;
			    if (countInSameLine >= 3)
			    {
				    return true;
			    }
		    }
		    curPos = position;														//重置位置
		    while (true)			
		    {
			    curPos = GoTo(curPos, GetOtherDirection(dir), 1);					//沿着Dir反方向走
			    if (GetBlockColor(curPos) != color)								//若碰到不一样的颜色就停下来
			    {
				    break;
			    }
			    ++countInSameLine;
			    if (countInSameLine >= 3)
			    {
				    return true;
			    }
		    }
	    }
	    return false;
    }

    TDirection GetOtherDirection(TDirection dir)					//从一个方向获得相反的方向
	{
		return (TDirection)((int)(dir + 3) % 6);
	}

	CapBlock GetBlock(Position p)							//获得某位置的块对象
	{
		return m_blocks[p.x, p.y];
	}

	bool CheckPosAvailable(Position p)							//获得某位置是否可用
	{
		if (p.x < 0 || p.x >=BlockCountX || p.y < 0 || p.y >= BlockCountY)
		{
			return false;
		}
		return true;
	}

	TBlockColor GetBlockColor(Position p)						//获得位置的颜色
	{
		if (!CheckPosAvailable(p))
		{
            return TBlockColor.EColor_None;
		}
		return m_blocks[p.x, p.y].color;
	}

    Position GoTo(Position pos,TDirection direction,int step)
    {
        if (step == 0) return pos;
        int i = pos.x;
        int j = pos.y;
        Position p = new Position();
        if ((i + 10) % 2 == 0)
        {
            switch (direction)
            {
                case TDirection.EDir_Up:
                    {
                        j--;
                    }
                    break;
                case TDirection.EDir_UpRight:
                    {
                        i++;
                    }
                    break;
                case TDirection.EDir_DownRight:
                    {
                        i++;
                        j++;
                    }
                    break;
                case TDirection.EDir_Down:
                    {
                        j++;
                    }
                    break;
                case TDirection.EDir_LeftDown:
                    {
                        i--;
                        j++;
                    }
                    break;
                case TDirection.EDir_LeftUp:
                    {
                        i--;
                    }
                    break;
                default:
                    break;
            }
        }
        else
        {
            switch (direction)
            {
                case TDirection.EDir_Up:
                    {
                        j--;
                    }
                    break;
                case TDirection.EDir_UpRight:
                    {
                        i++;
                        j--;
                    }
                    break;
                case TDirection.EDir_DownRight:
                    {
                        i++;
                    }
                    break;
                case TDirection.EDir_Down:
                    {
                        j++;
                    }
                    break;
                case TDirection.EDir_LeftDown:
                    {
                        i--;
                    }
                    break;
                case TDirection.EDir_LeftUp:
                    {
                        j--;
                        i--;
                    }
                    break;
                default:
                    break;
            }
        }
        p.x = i;
        p.y = j;

        if (step - 1 > 0)
        {
            return GoTo(p, direction, step - 1);
        }
        else
        {
            return p;
        }
    }

    bool CheckTwoPosLinked(Position position1, Position position2)
{
    if (System.Math.Abs(position2.x - position1.x) + System.Math.Abs(position2.y - position1.y) == 1
        || (position1.x % 2 == 1 && position1.y - position2.y == 1 && System.Math.Abs(position2.x - position1.x) == 1)
        || (position1.x % 2 == 0 && position2.y - position1.y == 1 && System.Math.Abs(position2.x - position1.x) == 1))
		return true;
	return false;
}

}
