using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum TGameFlow
{
    EGameState_StartGameAnim,                   //开始游戏动画
    EGameState_ResortAnim,                      //重排动画
    EGameState_Playing,                         //游戏中
    EGameState_EffectTime,                      //特效时间
    EGameState_SugarCrushAnim,                  //进入特殊奖励前的动画
    EGameState_EndEatingSpecial,                //结束后开始逐个吃屏幕上的特殊块
    EGameState_EndStepRewarding,                //结束后根据步数奖励
    EGameState_End,
    EGameState_Clear,                           //过了结束画面进入这个状态，清掉画面显示
    EGameState_FTUE,                            //FTUE状态
};

//特殊效果(一般是两个特殊块交换得到的，需要根据不同特效单独写代码)
public enum TSpecialEffect
{
    ENone,
    EAllDir,
    EAllDirBig,
    EEatAColor,
    EEatAllColor,
    EBigBomb,
    EEatAColorNDBomb,
}

enum TDirection
{
    EDir_Up,
    EDir_UpRight,
    EDir_DownRight,
    EDir_Down,
    EDir_LeftDown,
    EDir_LeftUp,
};

public struct Position
{
    public int x;
    public int y;

    public Position(int xPos, int yPos)
    {
        x = xPos;
        y = yPos;
    }

    public void Set(int xPos, int yPos)
    {
        x = xPos;
        y = yPos;
    }

    public int ToInt()
    {
        return y * 10 + x;
    }

    public void FromInt(int val)
    {
        x = val % 10;
        y = val / 10;
    }

    public bool IsAvailable()
    {
        return x > -99999;
    }

    public void MakeItUnAvailable()
    {
        x = -99999;
    }

    public static Position UnAvailablePos = new Position(-99999, -99999);
};

public class GridSprites
{
    public UISprite layer0;            //普通块/果冻
    public UISprite layer1;            //石头/笼子/巧克力
    public UISprite layer2;            //出生点
    public UISprite layer3;            //结束点
    public bool hasProcessAngle = false;       //是否已经处理了角(用来处理角的中间变量)
}

struct ShowingNumberEffect
{
    Transform trans;
    UISprite[] sprites;     //最多6位
    Position position;                           //位置
    long startTime;          //开始时间
    static long effectTime = 1000;              //特效显示时间
    int numberCount;

    public void Init(GameObject numInstance)
    {
        sprites = new UISprite[5];
        GameObject newObj = GameObject.Instantiate(numInstance) as GameObject;
        trans = newObj.transform;
        trans.parent = numInstance.transform.parent;
        trans.localScale = numInstance.transform.localScale;
        newObj.SetActive(false);

        for (int i = 0; i < trans.childCount; ++i)
        {
            sprites[i] = trans.GetChild(i).GetComponent<UISprite>();
        }
    }

    public void SetNumber(int num, int x, int y)
    {
        trans.gameObject.SetActive(true);
        numberCount = 0;
        while (num > 0)
        {
            sprites[numberCount].spriteName = "Score" + (num % 10);
            sprites[numberCount].gameObject.SetActive(true);
            num /= 10;
            ++numberCount;
        }

        for (int i = numberCount; i < 5; ++i)
        {
            sprites[i].gameObject.SetActive(false);
        }

        position = new Position(x, y);
        trans.localPosition = new Vector3(x + numberCount * 27 / 2, -y, -120);
        startTime = Timer.millisecondNow();
    }

    public void SetFree()
    {
        for (int i = 0; i < 5; ++i)
        {
            sprites[i].alpha = 1.0f;
        }
        trans.gameObject.SetActive(false);
    }

    public void Update()
    {
        if (!IsEnd())
        {
            long yOffset = (Timer.millisecondNow() - startTime) * 50 / effectTime;
            trans.localPosition = new Vector3(position.x + numberCount * 27 / 2, -(position.y - yOffset), -120);

            for (int i = 0; i < 5; ++i)
            {
                sprites[i].alpha = (1.0f - (Timer.millisecondNow() - startTime) * 1.0f / effectTime);
            }
        }
    }

    public bool IsEnd()
    {
        return Timer.millisecondNow() - startTime > effectTime;
    }
}

public enum ProcessGridType
{
    NormalEat,          //普通消除，需要触发周围块的消除
    Cage,               //消笼子
    Stone,
    Chocolate,          
}

class DelayProceedGrid
{
    public int x;
    public int y;
    public float startTime;                   //什么时间处理
    public bool bProceeStoneAround;           //是否处理周围的石头
    public bool bProceeChocolateAround;           //是否处理周围的巧克力
    public CapBlock block;                    //block的引用
}

class DelayParticle
{
    public ParticleSystem par;          //粒子系统
    public float startTime;             //开始时间0代表已开始
    public AudioEnum audio;             //声音
}

class FlyParticle
{
    public ParticleSystem par;          //粒子系统
    public Vector3 start;              //开始位置
    public Vector3 end;                //结束位置
    public float duration;              //飞行时间
    public float startTime;             //起飞时间
    public AudioEnum audio;             //声音
    public string name;
}

public enum AudioEnum
{
    Audio_None,
    Audio_Bomb,
    Audio_Drop,
    Audio_Eat,
    Audio_EatColor,
    Audio_MoveFailed,
    Audio_itemBirth,
    Audio_Jelly,
    Audio_Line1,
    Audio_Stone,
    Audio_Cage,
    Audio_Chocolate,

    Audio_Combo1,
    Audio_Combo2,
    Audio_Combo3,
    Audio_Combo4,
    Audio_Combo5,
    Audio_Combo6,
    Audio_Combo7,
    Audio_Combo8,
}

public class GameLogic
{
    #region Singleton
    public static GameLogic Singleton { get; private set; }
    public GameLogic()
    {
        if (Singleton == null)
        {
            Singleton = this;
        }
        else
        {
            throw new System.Exception();			//if singleton exist, throw a Exception
        }
        InitSingleton();
    }
    #endregion

    public static int BlockCountX = 9;	//游戏区有几列
    public static int BlockCountY = 9;	//游戏区有几行

    public static float BLOCKWIDTH = 65;      //宽度
    public static float BLOCKHEIGHT = 75.05553f;     //高度

    public static int gameAreaX = 16;		//游戏区域左上角坐标
    public static int gameAreaY = 90;		//游戏区域左上角坐标
    public static float gameAreaWidth = BLOCKWIDTH * BlockCountX;	//游戏区域宽度
    public static float gameAreaHeight = BLOCKHEIGHT * BlockCountY + BLOCKHEIGHT / 2;//游戏区域高度
    public static int TotalColorCount = 7;
    public static int PROGRESSTOWIN = 2000;
    public static float DROP_ACC = 5.0f;        //下落加速度
    public static float DROP_SPEED = 3.0f;        //下落初速度
    public static float SLIDE_SPEED = 3.0f;        //下落初速度
    public static int MOVE_TIME = 250;    		//移动的时间
    public static float EATBLOCK_TIME = 0.2f;		//消块时间
    public static int GAMETIME = 6000000;		//游戏时间
    public static float CheckAvailableTimeInterval = 1.0f;       //1秒钟后尝试找是否有可消块
    public static float ShowHelpTimeInterval = 5.0f;       //5秒钟后显示可消块
    public static float ShowNoPossibleExhangeTextTime = 1.0f;      //没有可交换的块显示，持续1秒钟
    public static int StepRewardInterval = 300;             //步数奖励的时间间隔
    public static int SugarCrushAnimTime = 1200;            //SugarCrush动画的时间长度
    public static int StartAnimTime = 1200;            //开始动画的时间长度

    public PurchasedItem UsingItem = PurchasedItem.None;                         //当前正在使用的道具
    UIGameBottom m_gameBottomUI;


    ///游戏逻辑变量/////////////////////////////////////////////////////////////////
    TDirection m_moveDirection;							                //选择的块1向块2移动的方向
    Position[] m_selectedPos = new Position[2];		                //记录两次点击选择的方块
    CapBlock[,] m_blocks = new CapBlock[BlockCountX, BlockCountY];		//屏幕上方块的数组
    GridSprites[,] m_gridBackImage = new GridSprites[BlockCountX, BlockCountY];        //用来记录背景块
    int[,] m_scoreToShow = new int[BlockCountX, BlockCountY];
    int[] m_slopeDropLock = new int[BlockCountX];         				//斜下落的锁定，值是锁在什么位置，锁和锁以下的位置都不能产生斜下落

    List<Position> m_cageCheckList = new List<Position>();              //笼子消除时，消除结束后还有再额外检查一次是否有消除的可能，中间要检查的位置就放在这个里面

    LinkedList<DelayProceedGrid> m_delayProcessGrid = new LinkedList<DelayProceedGrid>();           //延迟处理消除对场景的影响

    Position [] m_saveHelpBlocks = new Position[3];        //用来保存帮助找到的可消块

    ///特殊游戏状态///////////////////////////////////////////////////////////////////////
    bool m_gettingExtraScore = false;                                           //特殊状态，获得额外的分数
    int m_stoppingChocoGrowStepLeft;                                            //停止巧克力生长的剩余步数

    public bool IsStopingChocoGrow { get; set; }                                //特殊状态，停止巧克力生长
    public bool IsStoppingTime{get; set;}                                      //特殊状态，时间暂停
    
    int m_stoppingTimeStepLeft;                                                 //停止时间的剩余步数

    //用这4个数来记录每关实际块的范围，用来计算游戏区位置及优化性能
    int BlockXStart = 0;
    int BlockXEnd = 0;
    int BlockYStart = 0;
    int BlockYEnd = 0;

    int m_progress;										//当前进度
    TGameFlow m_gameFlow;								//游戏状态
    long m_curStateStartTime = 0;                       //当前状态的开始时间
    float m_lastPlayDropSoundTime = 0;                       //上一次播放Drop声音的时间


    TSpecialEffect m_curSpecialEffect;                  //当前的特殊效果
    Position m_curSpecialEffectPos;                     //当前特殊效果的开始位置
    int m_effectStep;                                   //当前特殊效果进行到第几步了
    int m_effectStateDuration;                          //特效演出时长
    TBlockColor m_colorToBomb;                          //要变成Bomb的颜色
    float m_colorToBombLastTime;                        //同颜色变炸弹，上次发生时间

    float m_endingAccelarateStartTime;                  //游戏结束时的额外加速的开始时间

    int m_comboCount;				//记录当前连击数
	
	int m_comboSoundCount;			//
	float m_comboSoundPlayTime;		//
	
    bool m_changeBack;		//在交换方块动画中标志是否为换回动画
    System.Random m_random;
    long m_gameStartTime = 0;                              //游戏开始时间
	float m_gameStartTimeReal = 0;						   //the game start time to calculate stage duration
    long m_lastStepRewardTime = 0;                         //上次生成StepReward的时间
    public StageData PlayingStageData;                      //当前的关卡数据
    bool m_bDropFromLeft = true;                            //用来控制左右斜下落的开关
    bool m_bHidingHelp = false;                             //弹其他界面时隐藏Help
	public int m_stepCountWhenReachTarget;							//save it for submit data
	public float GetStagePlayTime(){ return CapsApplication.Singleton.GetPlayTime() - m_gameStartTimeReal; }
    public bool m_bNeedCheckEatAllLineAgain = false;        //是否需要在下落都完成都额外检查一次消除的可能(当发生笼子消除时)

    public int m_chocolateCount = 0;                        //当前关卡里巧克力块的数量（用来优化性能）
    public int m_stoneCount = 0;                            //当前关卡里石头块的数量（用来优化性能）

    //计时器
    Timer timerMoveBlock = new Timer();

    float m_lastHelpTime;
    float m_lastGCTime;                                     //在操作间隔进行GC
    
    Position touchBeginPos;                                 //触控开始的位置
	Position touchBeginGrid;                                 //触控开始的位置

    //物件池
    LinkedList<CapBlock> m_capBlockFreeList = new LinkedList<CapBlock>();				//用来存放可用的Sprite
    LinkedList<UISprite> m_freeShadowSpriteList = new LinkedList<UISprite>();           //用来存放可用的ShadowSprite
    Dictionary<string, LinkedList<DelayParticle>> m_particleMap = new Dictionary<string, LinkedList<DelayParticle>>();
    Dictionary<string, LinkedList<ParticleSystem>> m_freeParticleMap = new Dictionary<string, LinkedList<ParticleSystem>>();
    LinkedList<ShowingNumberEffect> m_freeNumberList = new LinkedList<ShowingNumberEffect>();                 //用来存放数字图片的池
    LinkedList<ShowingNumberEffect> m_showingNumberEffectList = new LinkedList<ShowingNumberEffect>();      //用来管理正在播放的数字
    LinkedList<FlyParticle> m_flyParticleList = new LinkedList<FlyParticle>();                                  //飞行粒子特效

    GameObject m_gameArea;                  //游戏区域
    GameObject m_capsPool;                  //瓶盖池
    GameObject TopLeftAnchor;               //左上角
    GameObject CenterAnchor;               //中心
    GameObject m_gridInstance;              //把Grid的实例存起来，用来优化性能
    GameObject m_numInstance;              //把数字的实例存起来，用来优化性能
    GameObject m_shadowSpriteInstance;      //影子图片的实例

    GameObject m_angleInstance;             //凸角的实例
    GameObject m_angle2Instance;            //凹角1的实例
    GameObject m_angle3Instance;            //凹角2的实例
    

    List<GameObject> m_gridAngles = new List<GameObject>();            //背景块的圆角

    int m_nut1Count;                        //当前屏幕上的坚果数量
    int m_nut2Count;                        //当前屏幕上的坚果数量

    bool m_chocolateNeedGrow;               //是否需要巧克力生长
    bool m_bStopFTUE = false;               //若在关卡中跳过了FTUE,这个就为true了，进关卡时清回false

    public void StopFTUE()
    {
        m_bStopFTUE = true;
    }

    HashSet<AudioEnum> m_playSoundNextFrame = new HashSet<AudioEnum>();     //有声音要播放时，放在这个容器里，下一帧播放,这样做是为了避免重复播放

    void PlaySound(AudioEnum audio)
    {
        if (!GlobalVars.UseSFX)
        {
            return;
        }
        AudioClip clip = null;
        if (audio >= AudioEnum.Audio_Combo1)
        {
            clip = CapsConfig.CurAudioList.ComboClip[(int)audio - (int)AudioEnum.Audio_Combo1];
        }
        else
        {
            switch (audio)
            {
                case AudioEnum.Audio_Bomb:
                    {
                        clip = CapsConfig.CurAudioList.BombAudioClip;
                    }
                    break;
                case AudioEnum.Audio_Drop:
                    {
                        clip = CapsConfig.CurAudioList.DropClip;
                    }
                    break;
                case AudioEnum.Audio_Eat:
                    {
                        clip = CapsConfig.CurAudioList.EatClip;
                    }
                    break;
                case AudioEnum.Audio_EatColor:
                    {
                        clip = CapsConfig.CurAudioList.EatColorClip;
                    }
                    break;
                case AudioEnum.Audio_itemBirth:
                    {
                        clip = CapsConfig.CurAudioList.ItemBirthClip;
                    }
                    break;
                case AudioEnum.Audio_Jelly:
                    {
                        clip = CapsConfig.CurAudioList.JellyClip;
                    }
                    break;
                case AudioEnum.Audio_Line1:
                    {
                        clip = CapsConfig.CurAudioList.Line1Clip;
                    }
                    break;
                case AudioEnum.Audio_MoveFailed:
                    {
                        clip = CapsConfig.CurAudioList.MoveFailedClip;
                    }
                    break;
                case AudioEnum.Audio_Stone:
                    {
                        clip = CapsConfig.CurAudioList.StoneClip;
                    }
                    break;
                case AudioEnum.Audio_Cage:
                    {
                        clip = CapsConfig.CurAudioList.CageClip;
                    }
                    break;
                case AudioEnum.Audio_Chocolate:
                    {
                        clip = CapsConfig.CurAudioList.ChocolateClip;
                    }
                    break;
            }
        }
        
		NGUITools.PlaySound(clip);
    }

    void PlaySoundNextFrame(AudioEnum audio)                            //为了同一声音在一帧内播放多次，把要播放的声音存起来下一帧播放
    {
        if (audio == AudioEnum.Audio_Drop)       //对于下落声音，若没到固定时间间隔不播放，防止声音太密集
        {
            if (Timer.s_currentTime < m_lastPlayDropSoundTime + 0.2f)
            {
                return;
            }
            m_lastPlayDropSoundTime = Timer.s_currentTime;
        }
        m_playSoundNextFrame.Add(audio);
    }

    public TGameFlow GetGameFlow() { return m_gameFlow; }
    public void SetGameFlow(TGameFlow flow) { m_gameFlow = flow; }

    public int GetProgress() { return m_progress; }
    public void AddProgress(int progress, int x, int y)     //增加分数，同时在某位置显示一个得分的数字特效
    {
        if (m_gettingExtraScore)
        {
            progress = (int)(progress * 1.2f);
        }
        m_progress += progress;
        m_gameBottomUI.OnChangeProgress(m_progress);
        AddNumber(progress, GetXPos(x), GetYPos(x, y));
    }

    public void AddGameTime(int seconds)
    {
        m_gameStartTime += seconds * 1000;
    }
	
	public void SetGameTime(int seconds)
    {
        m_gameStartTime = Timer.millisecondNow() - (GlobalVars.CurStageData.TimeLimit - seconds) * 1000;
    }

    //暂停游戏
    public void PauseGame()
    {
        HideHelp();
        CapsConfig.Instance.GameSpeed = 0.0f;
    }

    //恢复游戏
    public void ResumeGame()
    {
        ShowHelpAnim();
        CapsConfig.Instance.GameSpeed = 1.0f;
    }

    void AddNumber(int number, int x, int y)                //添加一个数字特效
    {
        ShowingNumberEffect numEffect;
        if (m_freeNumberList.Count == 0)
        {
            numEffect = new ShowingNumberEffect();      //创建个新的
            numEffect.Init(m_numInstance);
        }
        else
        {
            numEffect = m_freeNumberList.Last.Value;
            m_freeNumberList.RemoveLast();
        }
        
        numEffect.SetNumber(number, x, y);
        m_showingNumberEffectList.AddLast(numEffect);
    }

    public float GetTimeRemain()                            //获得剩余时间
    {
        float timeRemain = GlobalVars.CurStageData.TimeLimit - (Timer.millisecondNow() - m_gameStartTime) / 1000.0f;
        timeRemain = Mathf.Max(0, timeRemain);
        return timeRemain;
    }

    public void InitSingleton()
    {
        m_capsPool = GameObject.Find("CapsPool");
        m_gameArea = GameObject.Find("GameArea");
        TopLeftAnchor = GameObject.Find("TopLeftAnchor");
        CenterAnchor = GameObject.Find("CenterAnchor");

        m_gridInstance = GameObject.Find("GridInstance");
        m_angleInstance = GameObject.Find("Angle");
        m_angle2Instance = GameObject.Find("Angle2");
        m_angle3Instance = GameObject.Find("Angle3");
        m_numInstance = GameObject.Find("NumberInstance");
        m_shadowSpriteInstance = GameObject.Find("ShadowSprite");

        m_gameBottomUI = UIWindowManager.Singleton.GetUIWindow<UIGameBottom>();

        //初始化瓶盖图片池
        for (int j = 0; j < 100; ++j)              //一百个够了
        {
            CapBlock capBlock = new CapBlock();
            m_capBlockFreeList.AddLast(capBlock);
        }

        for (int j = 0; j < 10; ++j)
        {
            GameObject newObj = GameObject.Instantiate(m_shadowSpriteInstance) as GameObject;
            newObj.transform.parent = m_shadowSpriteInstance.transform.parent;
            newObj.transform.localScale = m_shadowSpriteInstance.transform.localScale;
            UISprite sprite = newObj.GetComponent<UISprite>();
            m_freeShadowSpriteList.AddLast(sprite);
            newObj.SetActive(false);
        }
    }

    public void Init(int seed = -1)         //seed > -1时，指定seed
    {
        for (int i = 0; i < 81; ++i)
        {
            ShowingNumberEffect numEffect = new ShowingNumberEffect();
            numEffect.Init(m_numInstance);
            m_freeNumberList.AddLast(numEffect);
        }

        PlayingStageData = StageData.CreateStageData();
        PlayingStageData.LoadStageData(GlobalVars.CurStageNum);
        if (seed > -1)
        {
            PlayingStageData.Seed = seed;
        }

        //计算游戏区位置////////////////////////////////////////////////////////////////////////
        int BlockXStart = 999;
        int BlockXEnd = -1;
        int BlockYStart = 999;
        int BlockYEnd = -1;
        for (int i = 0; i < BlockCountX; ++i)
        {
            for (int j = 0; j < BlockCountY; ++j)
            {
                if (PlayingStageData.GridData[i, j] != 0)
                {
                    if (i < BlockXStart)
                    {
                        BlockXStart = i;
                    }
                    if (i > BlockXEnd)
                    {
                        BlockXEnd = i;
                    }
                    if (j < BlockYStart)
                    {
                        BlockYStart = j;
                    }
                    if (j > BlockYEnd)
                    {
                        BlockYEnd = j;
                    }
                }
            }
        }

        gameAreaX = (int)((CapsApplication.Singleton.Width - (BlockXEnd + 1 - BlockXStart) * BLOCKWIDTH) / 2 - BlockXStart * BLOCKWIDTH);
        gameAreaY = (int)((CapsApplication.Singleton.Height - (BlockYEnd + 1 - BlockYStart) * BLOCKHEIGHT) / 2 - BlockYStart * BLOCKHEIGHT - BLOCKHEIGHT /2) - 10;

        //绘制底图
        for (int i = 0; i < BlockCountX; ++i)
        {
            for (int j = 0; j < BlockCountY; ++j)
            {
                if (PlayingStageData.GridData[i, j] == 0)
                {
                    continue;
                }
                m_gridBackImage[i, j] = new GridSprites();
                ProcessGridSprites(i, j);
            }
        }

        for (int i = 0; i < 3; ++i )
        {
            m_saveHelpBlocks[i].MakeItUnAvailable();
        }

        EasyTouch.On_SimpleTap += OnTap;
        EasyTouch.On_Swipe += OnTouchMove;
        EasyTouch.On_TouchStart += OnTouchBegin;
        EasyTouch.On_TouchUp += OnTouchEnd;

        if (PlayingStageData.Seed > 0)
        {
            m_random = new System.Random(PlayingStageData.Seed);
        }
        else
        {
            m_random = new System.Random((int)Time.timeSinceLevelLoad * 1000);
        }

        //初始化坚果数量
        if (PlayingStageData.Target == GameTarget.BringFruitDown)
        {
            PlayingStageData.Nut1Count = 0;
            PlayingStageData.Nut2Count = 0;
        }

        //从随机位置开始
        int randomPos = m_random.Next() % BlockCountX;

        bool startOver = true;
        while (startOver)
        {
            startOver = false;
            for (int i = 0; i < BlockCountX; i++)
            {
                int xPos = (randomPos + i) % BlockCountX;
                for (int j = 0; j < BlockCountY; j++)
                {
                    if (PlayingStageData.CheckFlag(xPos, j, GridFlag.GenerateCap))
                    {
                        if (!CreateBlock(xPos, j, true))
                        {
                            startOver = true;       //重新生成所有块 
                            break;
                        }
                    }
                }
                if (startOver)
                {
                    FreeAllBlocks();                //清理
                    break;
                }
            }
        }

        //处理预置特殊块
        foreach (KeyValuePair<int, int> pair in PlayingStageData.SpecialBlock)
        {
            int x = pair.Key % 10;
            int y = pair.Key / 10;
            m_blocks[x, y].special = (TSpecialBlock)pair.Value;
            m_blocks[x, y].RefreshBlockSprite(PlayingStageData.GridData[x, y]);
        }

        UIWindowManager.Singleton.GetUIWindow<UIGameBottom>().Reset();

        ClearSelected();

		if (GlobalVars.StageStarArray[GlobalVars.CurStageNum] == 0)	//Only send start stage data when first time enters
		{
			if(CapsConfig.EnableGA)
				GA.API.Design.NewEvent("Stage" + GlobalVars.CurStageNum + ":Start");        //记录当前开始的关卡的数据
#if UNITY_ANDROID || UNITY_IPHONE
            if (CapsConfig.EnableTalkingData)
				TalkingDataPlugin.TrackEvent("Stage" + GlobalVars.CurStageNum + ":Start");  //记录当前开始的关卡的数据
#endif
		}       
		
		UIFTUE ftue = UIWindowManager.Singleton.GetUIWindow<UIFTUE>();
        if (ftue == null)
        {
            ftue = UIWindowManager.Singleton.CreateWindow<UIFTUE>();
        }
		ftue.ResetFTUEStep();

        ///处理游戏开始使用的道具///////////////////////////////////////////////////////////////////////
        for (int i = 0; i < 3; ++i )
        {
            if (GlobalVars.StartStageItem[i] == PurchasedItem.ItemPreGame_AddEatColor)
            {
                Position pos = FindRandomPos(TBlockColor.EColor_None, null, true);
                m_blocks[pos.x, pos.y].special = TSpecialBlock.ESpecial_EatAColor;
                m_blocks[pos.x, pos.y].RefreshBlockSprite(PlayingStageData.GridData[pos.x, pos.y]);
            }
            else if (GlobalVars.StartStageItem[i] == PurchasedItem.ItemPreGame_ExtraScore)
            {
                m_gettingExtraScore = true;
            }
            else if (GlobalVars.StartStageItem[i] == PurchasedItem.ItemPreGame_PlusStep)
            {
                GlobalVars.CurStageData.StepLimit += 7;
                GameLogic.Singleton.PlayingStageData.StepLimit += 7;        //步数加7
                UIWindowManager.Singleton.GetUIWindow<UIGameBottom>().OnChangeStep(GameLogic.Singleton.PlayingStageData.StepLimit);
            }
            else if (GlobalVars.StartStageItem[i] == PurchasedItem.ItemPreGame_PlusTime)
            {
                GlobalVars.CurStageData.TimeLimit += 20;                    //增加20秒限制
                GameLogic.Singleton.PlayingStageData.TimeLimit += 20;       //增加20秒限制
            }

            GlobalVars.StartStageItem[i] = PurchasedItem.None;
        }

        if (PlayingStageData.Target == GameTarget.Collect)
        {
            for (int i = 0; i < 3; ++i)
            {
                PlayingStageData.CollectCount[i] = 0;           //先把已搜集数量清零
            }
        }
    }

    void ProcessGridSprites(int x, int y)
    {
        //处理第一层////////////////////////////////////////////////////////////////////////
        GameObject newObj = GameObject.Instantiate(m_gridInstance) as GameObject;
        m_gridBackImage[x, y].layer0 = newObj.GetComponent<UISprite>();
        m_gridBackImage[x, y].layer0.transform.parent = m_gridInstance.transform.parent;
        m_gridBackImage[x, y].layer0.transform.localScale = m_gridInstance.transform.localScale;
        if (PlayingStageData.CheckFlag(x, y, GridFlag.Jelly))
        {
            m_gridBackImage[x, y].layer0.spriteName = "Jelly" + ((y + (x % 2)) % 3);
        }
        else if (PlayingStageData.CheckFlag(x, y, GridFlag.JellyDouble))
        {
            m_gridBackImage[x, y].layer0.spriteName = "JellyDouble" + ((y + (x % 2)) % 3); ;
        }
        else
        {
            m_gridBackImage[x, y].layer0.spriteName = "Grid" + ((y + (x % 2)) % 3);
        }

        m_gridBackImage[x, y].layer0.transform.localPosition = new Vector3(GetXPosF(x), -GetYPosF(x, y), 0);
        m_gridBackImage[x, y].layer0.depth = 0;

        //处理第二层
        if (PlayingStageData.CheckFlag(x, y, GridFlag.Stone))
        {
            newObj = GameObject.Instantiate(m_gridInstance) as GameObject;
            m_gridBackImage[x, y].layer1 = newObj.GetComponent<UISprite>();
            m_gridBackImage[x, y].layer1.spriteName = "Stone";
        }
        else if (PlayingStageData.CheckFlag(x, y, GridFlag.Chocolate))
        {
            newObj = GameObject.Instantiate(m_gridInstance) as GameObject;
            m_gridBackImage[x, y].layer1 = newObj.GetComponent<UISprite>();
            m_gridBackImage[x, y].layer1.spriteName = "Chocolate";

        }
        else if (PlayingStageData.CheckFlag(x, y, GridFlag.Cage))
        {
            newObj = GameObject.Instantiate(m_gridInstance) as GameObject;
            m_gridBackImage[x, y].layer1 = newObj.GetComponent<UISprite>();
            m_gridBackImage[x, y].layer1.spriteName = "Cage";
        }

        if (m_gridBackImage[x, y].layer1 != null)
        {
            m_gridBackImage[x, y].layer1.transform.parent = m_gridInstance.transform.parent;
            m_gridBackImage[x, y].layer1.transform.localScale = m_gridInstance.transform.localScale;
            m_gridBackImage[x, y].layer1.transform.localPosition = new Vector3(GetXPos(x), -GetYPos(x, y), -110);
            m_gridBackImage[x, y].layer1.depth = 3;
        }

        newObj = null;
        //判断圆角
        for (int i = 0; i <= (int)TDirection.EDir_LeftUp; ++i)       //向6个方向看
        {
            //每次循环判断该方向和下一方向所夹的那个角////////////////////////////////////////////////////////////////////////
            Position newPos = GoTo(new Position(x, y), (TDirection)i, 1);       //走一步
            if (CheckPosAvailable(newPos) && m_gridBackImage[newPos.x, newPos.y] != null && m_gridBackImage[newPos.x, newPos.y].hasProcessAngle)
            {
                continue;
            }
            Position newPos2 = GoTo(new Position(x, y), (TDirection)((i + 1)%6), 1);       //下个方向走一步
            if (CheckPosAvailable(newPos2) && m_gridBackImage[newPos2.x, newPos2.y] != null && m_gridBackImage[newPos2.x, newPos2.y].hasProcessAngle)
            {
                continue;
            }
            if ((!CheckPosAvailable(newPos) || PlayingStageData.GridData[newPos.x, newPos.y] == 0)
                && (!CheckPosAvailable(newPos2) || PlayingStageData.GridData[newPos2.x, newPos2.y] == 0))             //若两个方向都为空
            {
                newObj = GameObject.Instantiate(m_angleInstance) as GameObject;
                m_gridAngles.Add(newObj);
            }
            if ((!CheckPosAvailable(newPos) || PlayingStageData.GridData[newPos.x, newPos.y] == 0) 
                &&
                (CheckPosAvailable(newPos2) && PlayingStageData.GridData[newPos2.x, newPos2.y] != 0))             //若一个方向为空
            {
                newObj = GameObject.Instantiate(m_angle2Instance) as GameObject;
                m_gridAngles.Add(newObj);
            }
            if ((CheckPosAvailable(newPos) && PlayingStageData.GridData[newPos.x, newPos.y] != 0)
                &&
                (!CheckPosAvailable(newPos2) || PlayingStageData.GridData[newPos2.x, newPos2.y] == 0))             //若一个方向为空
            {
                newObj = GameObject.Instantiate(m_angle3Instance) as GameObject;
                m_gridAngles.Add(newObj);
            }

            if (newObj != null)
            {
                newObj.transform.parent = m_gridBackImage[x, y].layer0.transform;
                newObj.transform.localScale = Vector3.one;
                newObj.transform.localPosition = Vector3.zero;
                newObj.transform.localRotation = Quaternion.Euler(0, 0, -i * 60);       //旋转
            }
			newObj = null;
        }
		m_gridBackImage[x, y].hasProcessAngle = true;
    }

    public bool Help()                 //查找到一个可交换的位置
    {
        m_lastHelpTime = Timer.GetRealTimeSinceStartUp();
        for (int i = 0; i < BlockCountX; ++i)
        {
            for (int j = 0; j < BlockCountY; ++j)
            {
                if (m_blocks[i, j] == null || m_blocks[i, j].CurState != BlockState.Normal)                     //空格或空块
                {
                    continue;
                }

                Position position = new Position(i, j);

                for (TDirection dir = TDirection.EDir_Up; dir <= TDirection.EDir_LeftUp; dir = (TDirection)(dir + 1))		//遍历6个方向
                {
                    Position curPos = GoTo(position, dir, 1);
                    if (GetBlockColor(curPos) >= TBlockColor.EColor_Nut1)           //若是坚果，不参与查找
                    {
                        continue;
                    }

                    if (CheckPosAvailable(curPos))
                    {
                        if (m_blocks[curPos.x, curPos.y] == null || m_blocks[curPos.x, curPos.y].CurState != BlockState.Normal)             //空格或空块
                        {
                            continue;
                        }

                        ExchangeBlock(curPos, position);        //临时交换
                        m_saveHelpBlocks[0] = position;
                        if (IsHaveLine(curPos, true))
                        {
                            ExchangeBlock(curPos, position);        //换回
                            return true;
                        }
						
						m_saveHelpBlocks[0] = curPos;
						if (IsHaveLine(position, true))
						{
							ExchangeBlock(curPos, position);        //换回
                            return true;
						}
                        ExchangeBlock(curPos, position);        //换回
                    }

                }

            }
        }
        return false;
    }

    public void UserStopTimeItem()
    {
        IsStoppingTime = true;
        m_stoppingTimeStepLeft = 7;
    }

    public void UseStopChocoItem()
    {
        IsStopingChocoGrow = true;
        m_stoppingChocoGrowStepLeft = 7;
    }

    public void AutoResort()           //自动重排功能 Todo 没处理交换后形成消除的情况，不确定要不要处理
    {
        ClearHelpPoint();

        Position[] array = new Position[BlockCountX * BlockCountY];

        int count = 0;
        for (int i = 0; i < BlockCountX; ++i)
        {
            for (int j = 0; j < BlockCountY; ++j)
            {
                if (m_blocks[i, j] == null || m_blocks[i, j].CurState != BlockState.Normal)                     //空格或被锁块
                {
                    continue;
                }

                if (m_blocks[i, j].color > TBlockColor.EColor_Grey)                        //坚果
                {
                    continue;
                }

                array[count].Set(i, j);          //先找出可以交换的位置
                ++count;
            }
        }

        int sortcount = 0;
        while (true)
        {
            //自动重排
            Permute<Position>(array, count);       //重排
            CapBlock[,] blocks = new CapBlock[BlockCountX, BlockCountY];

            for (int i = 0; i < BlockCountX; ++i)
            {
                for (int j = 0; j < BlockCountY; ++j)
                {
                    blocks[i, j] = m_blocks[i, j];          //先复制份数据
                }
            }

            count = 0;
            for (int i = 0; i < BlockCountX; ++i)
            {
                for (int j = 0; j < BlockCountY; ++j)
                {
                    if (m_blocks[i, j] == null || m_blocks[i, j].CurState != BlockState.Normal)                     //空格或空块
                    {
                        continue;
                    }

                    if (m_blocks[i, j].color > TBlockColor.EColor_Grey)                        //坚果
                    {
                        continue;
                    }

                    m_blocks[i, j] = blocks[array[count].x, array[count].y];        //把随机内容取出来保存上
                    ++count;
                }
            }


            ++sortcount;


            if (Help() && !IsHaveLine())                                 //若有可消的，且没有直接三消的
            {
                ClearHelpPoint();
                break;
            }

            if (sortcount > 99)
            {
                break;
            }
        }
        Debug.Log("Auto Resort, count = " + sortcount);
    }

    void Permute<T>(T[] array, int count)
    {
        for (int i = 1; i < count; i++)
        {
            Swap<T>(array, i, m_random.Next(0, i));
        }
    }

    void Swap<T>(T[] array, int indexA, int indexB)
    {
        T temp = array[indexA];
        array[indexA] = array[indexB];
        array[indexB] = temp;
    }

    public void StartGame()     //开始游戏（及重新开始游戏）
    {
        long time = Timer.millisecondNow();
        m_gameStartTime = time;
		m_gameStartTimeReal = CapsApplication.Singleton.GetPlayTime ();
        m_lastHelpTime = Timer.GetRealTimeSinceStartUp();

        m_gameFlow = TGameFlow.EGameState_StartGameAnim;                //开始游戏
        m_curStateStartTime = Timer.millisecondNow();

        //播放开始游戏的动画
        TweenPosition tweenPos = m_gameArea.GetComponent<TweenPosition>();
        tweenPos.Play(true);
		
		if(PlayingStageData.Target == GameTarget.GetScore)
            AddPartile("StartGameAnim-Score", AudioEnum.Audio_None, 0, 0, false);
		if(PlayingStageData.Target == GameTarget.ClearJelly)
            AddPartile("StartGameAnim-Ice", AudioEnum.Audio_None, 0, 0, false);
		if(PlayingStageData.Target == GameTarget.BringFruitDown)
            AddPartile("StartGameAnim-Fruit", AudioEnum.Audio_None, 0, 0, false);
        if (PlayingStageData.Target == GameTarget.Collect)
            AddPartile("StartGameAnim-Collect", AudioEnum.Audio_None, 0, 0, false);
    }

    public void CheckFTUE()
    {
        if (PlayingStageData.FTUEMap.Count == 0 || m_bStopFTUE)
        {
            return;
        }

        if (!GlobalVars.DeveloperMode)
        {
            if (PlayerPrefs.GetInt("StageFTUEFinished") >= GlobalVars.CurStageNum)
            {
                return;
            }
        }

        int FTUEStepCount = GlobalVars.CurStageData.StepLimit - PlayingStageData.StepLimit;

        List<FTUEData> data;
        if (PlayingStageData.FTUEMap.TryGetValue(FTUEStepCount, out data))      //查看是否有FTUE数据
        {
            //进入FTUE状态
            UIFTUE ftue = UIWindowManager.Singleton.GetUIWindow<UIFTUE>();
            if (ftue == null)
            {
                ftue = UIWindowManager.Singleton.CreateWindow<UIFTUE>();
            }

            if (ftue.ShowFTUE(FTUEStepCount))
				m_gameFlow = TGameFlow.EGameState_FTUE;
        }
    }

    public void PlayEndGameAnim()
    {
		m_gameFlow = TGameFlow.EGameState_Clear;
        TweenPosition tweenPos = m_gameArea.GetComponent<TweenPosition>();
        tweenPos.Play(false);
    }

    void FreeAllBlocks()
    {
        //回收Blocks
        for (int i = 0; i < BlockCountX; ++i)
        {
            for (int j = 0; j < BlockCountY; ++j)
            {
                if (m_blocks[i, j] != null)
                {
                    if (m_blocks[i, j].m_shadowSprite != null)                           //已经落完的，若仍有shadowSprite, 释放
                    {
                        m_freeShadowSpriteList.AddLast(m_blocks[i, j].m_shadowSprite);   //放到空闲队列里
                        m_blocks[i, j].m_shadowSprite = null;
                    }

                    MakeSpriteFree(i, j);
                }
            }
        }
    }

    public void ClearGame()
    {
        FreeAllBlocks();

        m_cageCheckList.Clear();
		
        //隐藏shadowSprite
        foreach (UISprite sprite in m_freeShadowSpriteList)
        {
            sprite.gameObject.SetActive(false);
        }

        //处理数字
        foreach (ShowingNumberEffect number in m_showingNumberEffectList)
        {
            number.SetFree();
            m_freeNumberList.AddLast(number);
        }
        m_showingNumberEffectList.Clear();

        m_progress = 0;
        //回收粒子////////////////////////////////////////////////////////////////////////
        foreach (KeyValuePair<string, LinkedList<DelayParticle>> pair in m_particleMap)
        {
            LinkedList<DelayParticle> list = pair.Value;

            foreach (DelayParticle par in list)
            {
                par.par.Stop();
                m_freeParticleMap[pair.Key].AddLast(par.par);           //添加空闲的
                 
            }

            list.Clear();
        }

        foreach (GameObject obj in m_gridAngles)
        {
            GameObject.Destroy(obj);
        }

        m_gridAngles.Clear();

        for (int i = 0; i < BlockCountX; ++i)
        {
            for (int j = 0; j < BlockCountY; ++j)
            {
                if (m_blocks[i, j] != null)
                {
                    GameObject.Destroy(m_blocks[i, j].m_blockTransform.gameObject);
                    m_blocks[i, j] = null;
                }

                if (m_gridBackImage[i, j] != null)
                {
                    if (m_gridBackImage[i, j].layer0 != null)
                    {
                        GameObject.Destroy(m_gridBackImage[i, j].layer0.gameObject);
                    }
                    if (m_gridBackImage[i, j].layer1 != null)
                    {
                        GameObject.Destroy(m_gridBackImage[i, j].layer1.gameObject);
                    }
                    if (m_gridBackImage[i, j].layer2 != null)
                    {
                        GameObject.Destroy(m_gridBackImage[i, j].layer2.gameObject);
                    }
                    if (m_gridBackImage[i, j].layer3 != null)
                    {
                        GameObject.Destroy(m_gridBackImage[i, j].layer3.gameObject);
                    }
                    m_gridBackImage[i, j] = null;
                }
            }
        }
        m_nut1Count = 0;
        m_nut2Count = 0;
        m_curStateStartTime = 0;
        m_lastStepRewardTime = 0;
        m_lastHelpTime = 0;
        m_endingAccelarateStartTime = 0;
        m_bStopFTUE = false;

        m_gettingExtraScore = false;
        IsStopingChocoGrow = false;
        IsStoppingTime = false;

        CapBlock.DropingBlockCount = 0;
        CapBlock.EatingBlockCount = 0;

        for (int i = 0; i < BlockCountX; ++i)
        {
			m_slopeDropLock[i] = 10;         //初始化成不加锁
        }

        EasyTouch.On_SimpleTap -= OnTap;
        EasyTouch.On_Swipe -= OnTouchMove;
        EasyTouch.On_TouchStart -= OnTouchBegin;
        EasyTouch.On_TouchUp -= OnTouchEnd;

        System.GC.Collect();
    }

    public int GetXPos(int x)
    {
        return (int)(gameAreaX + x * BLOCKWIDTH + BLOCKWIDTH / 2);
    }

    public int GetYPos(int x, int y)
    {
        return (int)(gameAreaY + y * BLOCKHEIGHT + (x + 1) % 2 * BLOCKHEIGHT / 2 + BLOCKHEIGHT / 2);
    }

    float GetXPosF(int x)
    {
        return (gameAreaX + x * BLOCKWIDTH + BLOCKWIDTH / 2);
    }

    float GetYPosF(int x, int y)
    {
        return (gameAreaY + y * BLOCKHEIGHT + (x + 1) % 2 * BLOCKHEIGHT / 2 + BLOCKHEIGHT / 2);
    }

    void DrawGraphics()
    {
        if (m_gameFlow == TGameFlow.EGameState_Clear)
        {
            return;
        }

        //处理下落
        if (CapBlock.DropingBlockCount > 0)            //若有块在下落
        {
            //如果未到120毫秒，更新各方快的位置
            for (int i = 0; i < BlockCountX; i++)
            {
                for (int j = 0; j < BlockCountY; j++)
                {
                    if (m_blocks[i, j] == null)
                    {
                        continue;
                    }
                    if (m_blocks[i, j].CurState == BlockState.Moving)
                    {
                        ProcessDropPic(m_blocks[i, j].droppingFrom, new Position(i, j));
                    }
                }
            }
        }

        //for (int i = 0; i < BlockCountX; i++)
        //{
        //    UIDrawer.Singleton.DrawText("lock" + i, GetXPos(i) - 30, GetYPos(i, m_slopeDropLock[i]) - 30, m_slopeDropLock[i].ToString());
        //}

        if (GlobalVars.DeveloperMode)
        {
            for (int i = 0; i < BlockCountX; i++)
            {
                for (int j = 0; j < BlockCountY; j++)
                {
                    if (m_blocks[i, j] != null)
                    {
                        UIDrawer.Singleton.DrawText(i.ToString() + ", " + j.ToString(), GetXPos(i) - 30, GetYPos(i, j) - 30, i.ToString() + ", " + j.ToString());
                    }
                }
            }
        }
        

        //根据数据绘制Sprite
        for (int i = 0; i < BlockCountX; i++)
        {
            for (int j = 0; j < BlockCountY; j++)
            {
                if (PlayingStageData.GridData[i, j] == 0)
                {
                    continue;
                }

                if (m_blocks[i, j] != null)
                {
                    if (m_blocks[i, j].m_dropDownStartTime > 0)     //处理一次下落动画
                    {
                        if (Timer.GetRealTimeSinceStartUp() - m_blocks[i, j].m_dropDownStartTime > 1.0f)
                        {
                            m_blocks[i, j].m_animation.Stop();
                            m_blocks[i, j].m_animation.enabled = false;
                        }
                    }

                    //处理裁切
                    if (m_blocks[i, j].y_move < 0 && m_blocks[i, j].x_move == 0)      //若处于直线下落中
                    {
                        float yLenth = (-m_blocks[i, j].y_move) / (float)BLOCKHEIGHT - 0.001f;       //当前位移了几个块的距离

                        int curYPos = j - (int)Mathf.Ceil(yLenth);                          //当前块显示所在的格子

                        if (curYPos < -1)                                                   //若当前块的位置比-1还小
                        {
                            m_blocks[i, j].m_blockSprite.fillAmount = 0;                    //完全不可能显示
                        }
                        else if (curYPos == -1 || PlayingStageData.GridData[i, curYPos] == 0)   //若当前块的位置为-1或当前块为空
						{
                            if (PlayingStageData.GridData[i, curYPos + 1] != 0)                 //若下一位置是不为空
							{
                                m_blocks[i, j].m_blockSprite.fillAmount = 1.0f - (yLenth - Mathf.Floor(yLenth));  //算个裁切

                                if (PlayingStageData.CheckFlag(i, j, GridFlag.PortalEnd) && m_blocks[i, j].m_shadowSprite!=null)       //若为传送门的终点,需要处理传来位置的图片
                                {
                                    m_blocks[i, j].m_shadowSprite.fillAmount = 1.0f - m_blocks[i, j].m_blockSprite.fillAmount;
                                    Position from = PlayingStageData.PortalToMap[j * 10 + i].from;
                                    m_blocks[i, j].m_shadowSprite.transform.localPosition = new Vector3(GetXPos(from.x), -(m_blocks[i, j].y_move + GetYPos(from.x, from.y + 1)), 0);
                                }
							}
							else                                                                //若下一位置为空
							{
                                m_blocks[i, j].m_blockSprite.fillAmount = 0;                    //不显示
							}
						}
                    }
                    else
                    {
                        if (m_blocks[i, j].m_shadowSprite != null)                           //已经落完的，若仍有shadowSprite, 释放
                        {
                            m_freeShadowSpriteList.AddLast(m_blocks[i, j].m_shadowSprite);   //放到空闲队列里
                            //m_blocks[i, j].m_shadowSprite.gameObject.SetActive(false);       //释放
                            m_blocks[i, j].m_shadowSprite = null;
                        }
                        m_blocks[i, j].m_blockSprite.fillAmount = 1.0f;                     //完全显示
                    }

                    //处理位置变化
                    m_blocks[i, j].m_blockTransform.localPosition = new Vector3(GetXPos(i) + m_blocks[i, j].x_move, -(m_blocks[i, j].y_move + GetYPos(i, j)), 0);
                }

                
                //绘制水果出口
                if (PlayingStageData.Target == GameTarget.BringFruitDown && PlayingStageData.CheckFlag(i, j, GridFlag.FruitExit))
                {
                    UIDrawer.Singleton.CurDepth = 5;
                    UIDrawer.Singleton.DrawSprite("Exit" + (j * 10 + i), GetXPos(i), GetYPos(i, j), "FruitExit", 3);
                    UIDrawer.Singleton.CurDepth = 0;
                }

                if (GlobalVars.EditStageMode && PlayingStageData.CheckFlag(i, j, GridFlag.Birth))     //若在关卡编辑状态
                {
                    UIDrawer.Singleton.CurDepth = 5;
                    UIDrawer.Singleton.DrawSprite("Birth" + (j * 10 + i), GetXPos(i), GetYPos(i, j), "Birth", 3);       //出生点
                    UIDrawer.Singleton.CurDepth = 0;
                }
            }
        }
		
        //绘制传送门
        foreach (KeyValuePair<int, Portal> pair in PlayingStageData.PortalToMap)
        {
            if (pair.Value.flag == 1)               //可见传送门
            {
                UIDrawer.Singleton.DrawSprite("PortalStart" + pair.Key, GetXPos(pair.Value.from.x), GetYPos(pair.Value.from.x, pair.Value.from.y) + (int)BLOCKHEIGHT / 2, "PortalStart", 3);
                UIDrawer.Singleton.DrawSprite("PortalEnd" + pair.Key, GetXPos(pair.Value.to.x), GetYPos(pair.Value.to.x, pair.Value.to.y) - (int)BLOCKHEIGHT / 2 + 15, "PortalEnd", 3);
            }
            else if (GlobalVars.EditStageMode)      //编辑器模式，画不可见传送门
            {
                UIDrawer.Singleton.DrawSprite("InviPortalStart" + pair.Key, GetXPos(pair.Value.from.x), GetYPos(pair.Value.from.x, pair.Value.from.y), "InviPortalStart", 3);
                UIDrawer.Singleton.DrawSprite("InviPortalEnd" + pair.Key, GetXPos(pair.Value.to.x), GetYPos(pair.Value.to.x, pair.Value.to.y), "InviPortalEnd", 3);
            }
        }

        UIDrawer.Singleton.CurDepth = 5;
        if (m_gettingExtraScore)
        {
            UIDrawer.Singleton.DrawSprite(PurchasedItem.ItemPreGame_ExtraScore.ToString(), 100, 100, PurchasedItem.ItemPreGame_ExtraScore.ToString(), UIDrawer.Singleton.spriteDefaultPrefabID);       //出生点   
        }
        if (IsStopingChocoGrow)
        {
            UIDrawer.Singleton.DrawSprite(PurchasedItem.ItemInGame_ChocoStoper.ToString(), 200, 100, PurchasedItem.ItemInGame_ChocoStoper.ToString(), UIDrawer.Singleton.spriteDefaultPrefabID);       //出生点   
        }
        if (IsStoppingTime)
        {
            UIDrawer.Singleton.DrawSprite(PurchasedItem.ItemInGame_TimeStoper.ToString(), 300, 100, PurchasedItem.ItemInGame_TimeStoper.ToString(), UIDrawer.Singleton.spriteDefaultPrefabID);       //出生点   
        }
        UIDrawer.Singleton.CurDepth = 0;
    }

    void ProcessState()     //处理各个状态
    {
        //特效演出
        if (m_gameFlow == TGameFlow.EGameState_EffectTime)
        {
            long timePast = Timer.millisecondNow() - m_curStateStartTime;
            if (m_effectStateDuration > 0 && timePast > m_effectStateDuration)
            {
                m_gameFlow = TGameFlow.EGameState_Playing;                           //开始游戏
                DropDown();                                                          //开始先尝试进行一次下落
            }
            else                                                                    //这里处理各种特殊效果
            {
                if (m_curSpecialEffect == TSpecialEffect.EAllDirBig)                   //消全部方向
                {
                    if (m_effectStep == 0 && timePast > m_effectStateDuration / 4)          //若到了第一步的时间
                    {
                        m_effectStep = 1;
                        EatALLDirLine(m_curSpecialEffectPos, true, (int)TSpecialBlock.ESpecial_EatLineDir0);
                    }
                    
                    if (m_effectStep == 1 && timePast > m_effectStateDuration * 2 / 4)      //到了第二步的时间
                    {
                        m_effectStep = 2;
                        EatALLDirLine(m_curSpecialEffectPos, true, (int)TSpecialBlock.ESpecial_EatLineDir1);
                    }

                    if (m_effectStep == 2 && timePast > m_effectStateDuration * 3 / 4)      //到了第二步的时间
                    {
                        m_effectStep = 3;
                        EatALLDirLine(m_curSpecialEffectPos, true, (int)TSpecialBlock.ESpecial_EatLineDir2);
                    }
                }
                else if (m_curSpecialEffect == TSpecialEffect.EEatAColorNDBomb)     //同颜色变炸弹
                {
                    if (m_colorToBombLastTime > 0)       
                    {
						if(Timer.GetRealTimeSinceStartUp() - m_colorToBombLastTime > CapsConfig.Rainbow_Bomb_EffectAddItemInterval)		//到达了生成块间隔
						{
							for (int i = 0; i < BlockCountX; ++i)
	                        {
	                            for (int j = 0; j < BlockCountY; ++j)
	                            {
	                                CapBlock block = m_blocks[i, j];
	                                if (block != null && block.color == m_colorToBomb && block.special == TSpecialBlock.ESpecial_Normal)     //同颜色的普通块
	                                {

                                        if (PlayingStageData.CheckFlag(i, j, GridFlag.Cage))
                                        {
                                            PlayingStageData.ClearFlag(i, j, GridFlag.Cage);
                                            AddPartile("CageEffect", AudioEnum.Audio_Cage, i, j);
                                            m_scoreToShow[i, j] += CapsConfig.EatCagePoint;
                                            m_gridBackImage[i, j].layer1.gameObject.SetActive(false);
                                            m_cageCheckList.Add(new Position(i, j));                //记录一个位置，之后需要再检查一次消除
                                        }

	                                    AddPartile(CapsConfig.AddSpecialEffect, AudioEnum.Audio_itemBirth, i, j);
	                                    block.special = TSpecialBlock.ESpecial_Bomb;
	                                    block.RefreshBlockSprite(PlayingStageData.GridData[i, j]);
	                                    m_colorToBombLastTime = Timer.GetRealTimeSinceStartUp();
	                                    return;
	                                }
	                            }
	                        }
						}
						else 
						{
							return;
						}
						m_colorToBombLastTime = 0;              //时间置为零，不再进生成块的逻辑
                    }

                    if (CapBlock.EatingBlockCount == 0 && CapBlock.DropingBlockCount == 0)          //若没有在吃或在掉落中的块了
                    {
                        for (int i = 0; i < BlockCountX; ++i)
                        {
                            for (int j = 0; j < BlockCountY; ++j)
                            {
                                if (m_blocks[i, j] != null && m_blocks[i, j].special == TSpecialBlock.ESpecial_Bomb)
                                {
                                    EatBlock(new Position(i, j), CapsConfig.BombEatEffect);
                                }
                            }
                        }
						//到这里证明已经都处理完了
                    	m_gameFlow = TGameFlow.EGameState_Playing;                           //恢复状态游戏
						DropDown();                                               			 //尝试进行一次下落
                    }                    
                }
            }
        }

        if (m_gameFlow == TGameFlow.EGameState_StartGameAnim)
        {
            if (Timer.millisecondNow() - m_curStateStartTime > StartAnimTime)        //若时间到
            {
                m_gameFlow = TGameFlow.EGameState_Playing;                           //开始游戏
                DropDown();                                               //开始先尝试进行一次下落
                CheckFTUE();                                                         //检查是否有FTUE
                return;
            }
        }

        if (m_gameFlow == TGameFlow.EGameState_ResortAnim)        //正在显示“没有可交换块，需要重排”
        {
            if (Timer.millisecondNow() - m_curStateStartTime > ShowNoPossibleExhangeTextTime)       //时间已到
            {
                AutoResort();
                m_gameFlow = TGameFlow.EGameState_Playing;
            }
        }

        //处理流程////////////////////////////////////////////////////////////////////////
        if (m_gameFlow == TGameFlow.EGameState_SugarCrushAnim)        //播放sugarcrush动画状态
        {
            if (Timer.millisecondNow() - m_curStateStartTime > SugarCrushAnimTime)        //若时间到
            {
                m_gameFlow = TGameFlow.EGameState_EndEatingSpecial;                           //切下一状态
                return;
            }
        }

        //游戏结束后自动吃特殊块的状态，且当前没在消块或下落状态
        if (m_gameFlow == TGameFlow.EGameState_EndEatingSpecial && CapBlock.EatingBlockCount == 0 && CapBlock.DropingBlockCount == 0)
        {
            for (int i = 0; i < BlockCountX; ++i)
            {
                for (int j = 0; j < BlockCountY; ++j)
                {
                    if (m_blocks[i, j] != null && m_blocks[i, j].special != TSpecialBlock.ESpecial_Normal)
                    {
                        EatBlock(new Position(i, j), CapsConfig.EatEffect);
                        return;         //消一个特殊块就返回
                    }
                }
            }

            //若执行到这里证明已经没特殊块可以消了
            if (PlayingStageData.StepLimit > 0)     //若剩余步数大于0，进入步数奖励
            {
                m_gameFlow = TGameFlow.EGameState_EndStepRewarding;
                m_lastStepRewardTime = Timer.millisecondNow();
            }
            else
            {
                m_gameStartTime = 0;
                m_gameFlow = TGameFlow.EGameState_End;
                Time.timeScale = 1.0f;                                                        //恢复正常的时间比例
                m_endingAccelarateStartTime = 0;

                HideUI();

                //若达成过关条件，触发关卡结束对话，并在对话结束后出游戏结束窗口
                if (IsStageFinish())
                {
                    Timer.AddDelayFunc(0.8f, delegate()
                    {
                        UIWindowManager.Singleton.GetUIWindow<UIDialog>().TriggerDialog(GlobalVars.CurStageNum, DialogTriggerPos.StageEnd, delegate()
                        {
                            UIWindowManager.Singleton.GetUIWindow<UIRetry>().RefreshData();
                            UIWindowManager.Singleton.GetUIWindow<UIRetry>().ShowWindow();      //弹游戏结束的窗口
                        });
                    });
                }
                else //若没达成过关条件
                {
                    Timer.AddDelayFunc(2.0f, delegate()
                    {
                        UIWindowManager.Singleton.GetUIWindow<UIGameEnd>().ShowWindow();            //出游戏结束界面
                    });
                    AddPartile("EndGameFailed", AudioEnum.Audio_None, 0, 0, false);
                }
            }
            return;
        }

        //步数奖励状态
        if (m_gameFlow == TGameFlow.EGameState_EndStepRewarding)
        {
            if (Timer.millisecondNow() - m_lastStepRewardTime > StepRewardInterval)     //若到了时间间隔， 生成一个步数奖励
            {
                if (PlayingStageData.StepLimit > 0)
                {
                    Position pos = FindRandomPos(TBlockColor.EColor_None, null, true);
                    if (pos.IsAvailable())
                    {
                        m_blocks[pos.x, pos.y].special = TSpecialBlock.ESpecial_EatLineDir0 + (m_random.Next() % 3);
                        m_blocks[pos.x, pos.y].RefreshBlockSprite(PlayingStageData.GridData[pos.x, pos.y]);
                        AddPartile(CapsConfig.AddSpecialEffect, AudioEnum.Audio_itemBirth, pos.x, pos.y);
                        AddProgress(CapsConfig.SugarCrushStepReward, pos.x, pos.y);
                        --PlayingStageData.StepLimit;           //步数减一
                        m_gameBottomUI.OnChangeStep(PlayingStageData.StepLimit);
                    }
                    else
                    {
                        PlayingStageData.StepLimit = 0;         //没地方放奖励了，直接置0
                    }
                }
                else
                {
                    m_gameFlow = TGameFlow.EGameState_EndEatingSpecial;
                }

                m_lastStepRewardTime = Timer.millisecondNow();
            }
        }
    }

    public void HideUI()
    {
        UIWindowManager.Singleton.GetUIWindow<UIGameHead>().HideWindow();
        UIWindowManager.Singleton.GetUIWindow<UIGameBottom>().HideWindow();
        UIWindowManager.Singleton.GetUIWindow<UIMainMenu>().HideWindow();

        if (UIWindowManager.Singleton.GetUIWindow<UIHowToPlay>().Visible)
        {
            UIWindowManager.Singleton.GetUIWindow<UIHowToPlay>().OnClose();
            return;
        }

        if (UIWindowManager.Singleton.GetUIWindow<UINoMoreHearts>().Visible)
        {
            UIWindowManager.Singleton.GetUIWindow<UINoMoreHearts>().Close();
            return;
        }

        if (UIWindowManager.Singleton.GetUIWindow<UIPurchaseNoTarget>().Visible)
        {
            UIWindowManager.Singleton.GetUIWindow<UIPurchaseNoTarget>().OnCancelClicked();
            return;
        }

        if (UIWindowManager.Singleton.GetUIWindow<UIStageInfo>().Visible)
        {
            UIWindowManager.Singleton.GetUIWindow<UIStageInfo>().OnCloseClicked();
            return;
        }

        if (UIWindowManager.Singleton.GetUIWindow("UIMainMenuExtend").Visible)      //若主菜单开启状态
        {
            UIWindowManager.Singleton.GetUIWindow("UIMainMenuExtend").HideWindow();
            return;
        }
    }

    public void ShowUI()
    {
        UIWindowManager.Singleton.GetUIWindow<UIGameHead>().ShowWindow();
        UIWindowManager.Singleton.GetUIWindow<UIGameBottom>().ShowWindow();
        UIWindowManager.Singleton.GetUIWindow<UIMainMenu>().ShowWindow();
    }

    void EatFruit(int x, int y)
    {
        //记录吃一个坚果
        if (m_blocks[x, y].color == TBlockColor.EColor_Nut1)
        {
            PlayingStageData.Nut1Count++;
            --m_nut1Count;
        }

        if (m_blocks[x, y].color == TBlockColor.EColor_Nut2)
        {
            PlayingStageData.Nut2Count++;
            --m_nut2Count;
        }

        m_scoreToShow[x, y] += CapsConfig.FruitDropDown;
        m_blocks[x, y].Eat();

        UIWindowManager.Singleton.GetUIWindow<UIGameHead>().RefreshTarget();
    }

    //处理下落完成后的判断////////////////////////////////////////////////////////////////////////
    void ProcessMoveEnd()
    {
        bool bDroped = false;   //记录是否发生了落到底事件
        bool bEat = false;      //记录是否形成了消块
        for (int i = 0; i < BlockCountX; ++i)
        {
            for (int j = 0; j < BlockCountY; ++j)
            {
                if (m_blocks[i, j] != null && m_blocks[i, j].CurState == BlockState.MovingEnd)      //若为普通块且移动结束
                {
					Position leftDown = GoTo(new Position(i, j), TDirection.EDir_DownRight, 1);
                    Position rightDown = GoTo(new Position(i, j), TDirection.EDir_LeftDown, 1);
                    //这里判断一下是否落到底了（即不能下方向或斜方向下落）
                    if (!CheckPosCanDropDown(i, j + 1)          //
                        && (!CheckPosCanDropDown(leftDown.x, leftDown.y) || m_slopeDropLock[leftDown.x] <= leftDown.y)
                        && (!CheckPosCanDropDown(rightDown.x, rightDown.y) || m_slopeDropLock[rightDown.x] <= rightDown.y))
                    {
                        m_blocks[i, j].CurState = BlockState.Normal;        //先去掉下落状态
						--CapBlock.DropingBlockCount;                       //清理下落计数
							
						m_blocks[i, j].m_animation.enabled = true;
                        m_blocks[i, j].m_animation.Play("DropDown");                                    //播放下落动画
                        PlaySoundNextFrame(AudioEnum.Audio_Drop);
                        m_blocks[i, j].m_dropDownStartTime = Timer.GetRealTimeSinceStartUp();           //记录开始时间(用于强制停止下落动画)
							
						if (m_blocks[i, j].color > TBlockColor.EColor_Grey)                     //若为坚果
		                {
		                    //到了消失点
		                    if (PlayingStageData.CheckFlag(i, j, GridFlag.FruitExit))
		                    {
		                        EatFruit(i, j);
		                        bEat = true;
		                    }
		                }
						else
						{
							if (EatLine(new Position(i, j)))
		                    {
		                        bEat = true;
		                    }
						}
							
						bDroped = true;         //发生了落到底，就记下来
                    }
                }
            }
        }

        if (bDroped)        //落到底发生
        {
            if (CapBlock.DropingBlockCount == 0 && CapBlock.EatingBlockCount == 0)       //若下落结束
            {
                if (m_cageCheckList.Count > 0)          //若有笼子消除完了需要多检查一次消除的可能
                {
                    bEat = CheckCageAgain();
                }
				if(!bEat)                    			//到这儿就是什么未处理的消除都没了
				{
                    OnDropEnd();
				}
            }
        }

        if (bEat)
        {
            ProcessTempBlocks();            //处理正常下落后消块对场景的影响
        }
    }
	
	bool CheckCageAgain()
	{
		bool bEat = false;
		while (m_cageCheckList.Count > 0)
        {
            Position item = m_cageCheckList[m_cageCheckList.Count-1];
			if(m_blocks[item.x, item.y] != null && m_blocks[item.x, item.y].CurState != BlockState.Eating)
			{
				if(EatLine(item))
				{
					bEat = true;
				}
			}
            m_cageCheckList.Remove(item);
        }
        m_cageCheckList.Clear();
		return bEat;
	}
	

    public void Update()
    {
        //Profiler.BeginSample("GameLogic");
        //Timer.s_currentTime = Time.realtimeSinceStartup;

        ProcessState();

        if (CapBlock.DropingBlockCount > 0)
        {
            bool bFound = false;
            for (int i = 0; i < BlockCountX; ++i )
            {
                for (int j = 0; j < BlockCountY; ++j )
                {
                    if (m_blocks[i, j] != null && m_blocks[i, j].CurState == BlockState.MovingEnd)
                    {
                        bFound = true;
                    }
                }
            }
            if (bFound)     //若找到了MovingEnd的块
            {
                DropDown();                     //再处理一次下落
                ProcessMoveEnd();        //处理下落结束的块（消块）
            }
        }

        {
            //处理延迟消除
            LinkedListNode<DelayProceedGrid> node = m_delayProcessGrid.First;
            LinkedListNode<DelayProceedGrid> nodeLast = m_delayProcessGrid.Last;
            LinkedListNode<DelayProceedGrid> nodeTmp;//临时结点
            if (m_delayProcessGrid.Count > 0)
            {
                while (node != nodeLast)
                {
                    if (Timer.GetRealTimeSinceStartUp() > node.Value.startTime)       //若到了处理时间
                    {
                        ProcessEatChanges(node.Value, true, true);                          //处理延迟处理的消块

                        nodeTmp = node.Next;
                        m_delayProcessGrid.Remove(node.Value);                              //移除中间一个元素
                        node = nodeTmp;
                    }
                    else
                    {
                        node = node.Next;
                    }
                }

                if (Timer.GetRealTimeSinceStartUp() > nodeLast.Value.startTime)       //若到了处理时间
                {
                    ProcessEatChanges(nodeLast.Value, true, true);                          //处理延迟处理的消块
                    m_delayProcessGrid.Remove(nodeLast.Value);                              //移除中间一个元素
                }
            }
        }


        TimerWork();

        DrawGraphics();     //绘制图形

        //处理帮助
        if (m_gameFlow == TGameFlow.EGameState_Playing && m_lastHelpTime > 0)      //下落完成的状态
        {
            if (IsMoveAble())       //可移动状态，判断帮助
            {
                if (!m_saveHelpBlocks[2].IsAvailable())     //还没找帮助点
                {
                    if (Timer.GetRealTimeSinceStartUp() > m_lastHelpTime + CheckAvailableTimeInterval)
                    {
                        if (!Help())
                        {
                            m_curStateStartTime = Timer.millisecondNow();
                            AddPartile("ResortAnim", AudioEnum.Audio_None, 0, 0, false);                                    //显示需要重排
                            m_gameFlow = TGameFlow.EGameState_ResortAnim;
                        }
                    }
                }
                else if (Timer.GetRealTimeSinceStartUp() > m_lastHelpTime + ShowHelpTimeInterval && !m_bHidingHelp)
                {
                    Help();
                    ShowHelpAnim();
                }
            }
        }

        //处理粒子////////////////////////////////////////////////////////////////////////
        foreach (KeyValuePair<string, LinkedList<DelayParticle>> pair in m_particleMap)
        {
            LinkedList<DelayParticle> list = pair.Value;

            //处理延迟开始的粒子
            foreach (DelayParticle par in list)
            {
                if (par.startTime != 0.0f && Timer.GetRealTimeSinceStartUp() > par.startTime)       //到了开始时间
                {
                    par.par.gameObject.SetActive(true);
                    par.par.Play();
                    par.startTime = 0.0f;
                    PlaySoundNextFrame(par.audio);
                }
            }

            DelayParticle parToDelete = null;
            foreach (DelayParticle parPair in list)
            {
                if (parPair.startTime == 0.0f && parPair.par.isStopped)
                {
                    m_freeParticleMap[pair.Key].AddLast(parPair.par);           //添加空闲的
                    parToDelete = parPair;
                    parPair.par.Stop();
                    parPair.par.gameObject.SetActive(false);
                    break;                                              //每帧只处理一个
                }
            }

            if (parToDelete != null)
            {
                list.Remove(parToDelete);                               //在原列表中删除
            }
        }

        //处理数字
        //处理延迟消除
        {
            if (m_showingNumberEffectList.Count > 0)
            {
                LinkedListNode<ShowingNumberEffect> node = m_showingNumberEffectList.First;
                LinkedListNode<ShowingNumberEffect> nodeLast = m_showingNumberEffectList.Last;
                LinkedListNode<ShowingNumberEffect> nodeTmp;//临时结点
                while (node != nodeLast)
                {
                    if (node.Value.IsEnd())       //若到了处理时间
                    {
                        nodeTmp = node.Next;
                        node.Value.SetFree();
                        m_freeNumberList.AddLast(node.Value);
                        m_showingNumberEffectList.Remove(node.Value);                              //移除中间一个元素
                        node = nodeTmp;
                    }
                    else
                    {
                        node.Value.Update();
                        node = node.Next;
                    }
                }

                if (nodeLast.Value.IsEnd())       //若到了处理时间
                {
                    m_freeNumberList.AddLast(nodeLast.Value);
                    m_showingNumberEffectList.Remove(nodeLast.Value);                              //移除中间一个元素
                }
                else
                {
                    nodeLast.Value.Update();
                }
            }
        }

        if (m_gameFlow == TGameFlow.EGameState_Playing && GlobalVars.CurStageData.TimeLimit > 0 && CapBlock.DropingBlockCount == 0 && CapBlock.EatingBlockCount == 0)
        {
            ProcessCheckStageFinish();
        }

        if (m_lastGCTime > 0 && Timer.GetRealTimeSinceStartUp() - m_lastGCTime > 0.5f)      //落完0.5秒后进行GC
        {
            System.GC.Collect();
            m_lastGCTime = 0;
        }

        //处理声音
        foreach (AudioEnum audio in m_playSoundNextFrame)
        {
            PlaySound(audio);
        }
        m_playSoundNextFrame.Clear();

        UpdateSlopeLock();

        //处理飞行特效
        foreach (FlyParticle flyParticle in m_flyParticleList)
        {
            float passTime = Timer.GetRealTimeSinceStartUp() - flyParticle.startTime;
            if (passTime > flyParticle.duration)        //到了结束时间
            {
                m_freeParticleMap[flyParticle.name].AddLast(flyParticle.par);           //添加空闲的
                flyParticle.par.Stop();                 //停止
                flyParticle.par.gameObject.SetActive(false);        //隐藏

                m_flyParticleList.Remove(flyParticle);      //移除
                break;      //每次循环只结束一个
            }
            else if (passTime > 0)      //若已开始未结束
            {
                if (!flyParticle.par.gameObject.activeSelf)
                {
                    flyParticle.par.gameObject.SetActive(true);
                    flyParticle.par.Play();
                    if (flyParticle.audio != AudioEnum.Audio_None)
                    {
                        PlaySoundNextFrame(flyParticle.audio);
                    }
                }

                flyParticle.par.transform.localPosition = Vector3.Lerp(flyParticle.start, flyParticle.end, (Timer.GetRealTimeSinceStartUp() - flyParticle.startTime) / flyParticle.duration);        //指定位置
            }
        }

        //时间暂停但仍可继续游戏的状态
        if (IsStoppingTime)
        {
            m_gameStartTime += (long)(Time.deltaTime * 1000);
        }

        if (m_endingAccelarateStartTime > 0 && Timer.GetRealTimeSinceStartUp() > m_endingAccelarateStartTime)
        {
            if (Timer.GetRealTimeSinceStartUp() - m_endingAccelarateStartTime >10)
            {
                Time.timeScale = 1.4f;
            }
        }
    }

    void UpdateSlopeLock()
    {
        //计算锁的位置
        for (int i = 0; i < BlockCountX; ++i)
        {
            for (int j = 0; j < BlockCountY; ++j)
            {
                if (m_blocks[i, j] != null)     //若有下落块
                {
                    if (m_blocks[i, j].IsDroping() || m_blocks[i, j].IsEating())     //若有下落块或正在消的块
                    {
                        m_slopeDropLock[i] = j + (int)(m_blocks[i, j].y_move / BLOCKHEIGHT + 1);
                        break;
                    }
                }
                else if (PlayingStageData.CheckFlag(i, j, GridFlag.Birth) && PlayingStageData.CheckFlag(i, j, GridFlag.GenerateCap))      //若出生点为空
                {
                    m_slopeDropLock[i] = j;     //锁住出生点以下
                    break;
                }
                if (j == BlockCountY - 1)       //若到底了
                {
                    m_slopeDropLock[i] = 10;    //清掉锁
                }
            }
        }
    }

    public void ShowHelpAnim()
    {
        if (m_saveHelpBlocks[2].IsAvailable() && m_blocks[m_saveHelpBlocks[2].x, m_saveHelpBlocks[2].y] != null && !m_blocks[m_saveHelpBlocks[2].x, m_saveHelpBlocks[2].y].m_animation.isPlaying)
        {
            m_bHidingHelp = false;
            for (int i = 0; i < 3; ++i )
            {
                if (m_saveHelpBlocks[i].IsAvailable())
                {
                    m_blocks[m_saveHelpBlocks[i].x, m_saveHelpBlocks[i].y].m_animation.Play("Help");
                    m_blocks[m_saveHelpBlocks[i].x, m_saveHelpBlocks[i].y].m_dropDownStartTime = 0;
                    m_blocks[m_saveHelpBlocks[i].x, m_saveHelpBlocks[i].y].m_animation.enabled = true;
                }
            }
        }
    }

    void ChangeBackSelected()
    {
        PlaySound(AudioEnum.Audio_MoveFailed);
        ExchangeBlock(m_selectedPos[0], m_selectedPos[1]);
        timerMoveBlock.Play();
        ProcessMoveBlock(m_selectedPos[0], m_selectedPos[1], MOVE_TIME);
        m_changeBack = true;
    }

    //处理两块交换
    void ProcessExchange()
    {
        TSpecialBlock special0 = m_blocks[m_selectedPos[0].x, m_selectedPos[0].y].special;
        TSpecialBlock special1 = m_blocks[m_selectedPos[1].x, m_selectedPos[1].y].special;
        //+5当成普通块来处理
        if (special0 == TSpecialBlock.ESpecial_NormalPlus5)
        {
            special0 = TSpecialBlock.ESpecial_Normal;
        }
        if (special1 == TSpecialBlock.ESpecial_NormalPlus5)
        {
            special1 = TSpecialBlock.ESpecial_Normal;
        }

        //普通交换的情况
        if ((special0 == TSpecialBlock.ESpecial_Normal && special1 == TSpecialBlock.ESpecial_Normal)                //两个都是普通块
            || (special0 == TSpecialBlock.ESpecial_Normal && special1 != TSpecialBlock.ESpecial_EatAColor)          //一个普通块，另一个不是彩虹
            || (special1 == TSpecialBlock.ESpecial_Normal && special0 != TSpecialBlock.ESpecial_EatAColor)          //一个普通块，另一个不是彩虹
            || (m_blocks[m_selectedPos[0].x, m_selectedPos[0].y].color >= TBlockColor.EColor_Nut1 || m_blocks[m_selectedPos[1].x, m_selectedPos[1].y].color >= TBlockColor.EColor_Nut1)) //若有坚果块
        {
            bool hasEatLine1 = EatLine(m_selectedPos[0]);
            bool hasEatLine2 = EatLine(m_selectedPos[1]);


            if (!hasEatLine1 && !hasEatLine2)//如果交换不成功,播放交换回来的动画
            {
                ChangeBackSelected();
                return;                     //交换不成功这里就返回了
            }

            //若有水果可以吃掉，处理下吃水果
            if (PlayingStageData.Target == GameTarget.BringFruitDown)
            {
                if (m_blocks[m_selectedPos[0].x, m_selectedPos[0].y].color > TBlockColor.EColor_Grey &&
                PlayingStageData.CheckFlag(m_selectedPos[0].x, m_selectedPos[0].y, GridFlag.FruitExit))
                {
                    EatFruit(m_selectedPos[0].x, m_selectedPos[0].y);
                }
                if (m_blocks[m_selectedPos[1].x, m_selectedPos[1].y].color > TBlockColor.EColor_Grey &&
                    PlayingStageData.CheckFlag(m_selectedPos[1].x, m_selectedPos[1].y, GridFlag.FruitExit))
                {
                    EatFruit(m_selectedPos[1].x, m_selectedPos[1].y);
                }
            }

            if (CapBlock.EatingBlockCount == 0)			//特殊情况，移动消除后没有产生吃块，可能的情况是在笼子边上生成了块，这是需要额外去判断一次笼子附近的消除
            {
                if (m_cageCheckList.Count > 0)
                {
                    CheckCageAgain();
                }
            }
        }
        else
        {
            //两个彩虹块
            if (special0 == TSpecialBlock.ESpecial_EatAColor && special1 == TSpecialBlock.ESpecial_EatAColor)       
            {
                m_blocks[m_selectedPos[0].x, m_selectedPos[0].y].EatAnimationName = CapsConfig.RainbowEatAnim;                 //
                m_blocks[m_selectedPos[0].x, m_selectedPos[0].y].Eat();
                
                AddDelayProceedGrid(m_selectedPos[0].x, m_selectedPos[0].y, 0, m_blocks[m_selectedPos[0].x, m_selectedPos[0].y]);       ////自己消失

                m_blocks[m_selectedPos[1].x, m_selectedPos[1].y].EatAnimationName = CapsConfig.Rainbow_Rainbow_EatAnim;        //两彩虹合成动画
                m_blocks[m_selectedPos[1].x, m_selectedPos[1].y].Eat();

                AddDelayProceedGrid(m_selectedPos[1].x, m_selectedPos[1].y, 0, m_blocks[m_selectedPos[1].x, m_selectedPos[1].y]);       ////自己消失
                EatAColor(TBlockColor.EColor_None, m_selectedPos[1], true);         //消全部
            }
            //彩虹和普通块
            else if (special0 == TSpecialBlock.ESpecial_EatAColor && special1 == TSpecialBlock.ESpecial_Normal)     
            {
                EatAColor(m_blocks[m_selectedPos[1].x, m_selectedPos[1].y].color, m_selectedPos[0], true);      //吃同颜色
            }
            //彩虹和普通块
            else if (special1 == TSpecialBlock.ESpecial_EatAColor && special0 == TSpecialBlock.ESpecial_Normal)     
            {
                EatAColor(m_blocks[m_selectedPos[0].x, m_selectedPos[0].y].color, m_selectedPos[1], true);      //吃同颜色
            }
            //彩虹和条状
            else if (special0 == TSpecialBlock.ESpecial_EatAColor &&
                (special1 == TSpecialBlock.ESpecial_EatLineDir0 || special1 == TSpecialBlock.ESpecial_EatLineDir2 ||     //跟条状消除,六方向加粗
                    special1 == TSpecialBlock.ESpecial_EatLineDir1))
            {
                m_blocks[m_selectedPos[0].x, m_selectedPos[0].y].Eat();                 //自己消失
                AddDelayProceedGrid(m_selectedPos[0].x, m_selectedPos[0].y, 0, m_blocks[m_selectedPos[0].x, m_selectedPos[0].y]);       ////自己消失

                m_blocks[m_selectedPos[1].x, m_selectedPos[1].y].EatAnimationName = CapsConfig.Line_Rainbow_EatAnim;                 //彩虹条状合成动画
                m_blocks[m_selectedPos[1].x, m_selectedPos[1].y].Eat();                 //自己消失
                AddDelayProceedGrid(m_selectedPos[1].x, m_selectedPos[1].y, 0, m_blocks[m_selectedPos[1].x, m_selectedPos[1].y]);       ////自己消失



                m_gameFlow = TGameFlow.EGameState_EffectTime;                           //切换游戏状态到特效演出时间，等待特效演出
                m_curStateStartTime = Timer.millisecondNow();                           //保存下切状态的时间
                m_effectStateDuration = CapsConfig.EffectAllDirBigTime;                 //2秒的演出时间
                m_curSpecialEffect = TSpecialEffect.EAllDirBig;                         //全方向消除特效
                m_curSpecialEffectPos = m_selectedPos[1];                               //把特效位置保存起来
                m_effectStep = 0;                                                       //从第0步开始

                //EatALLDirLine(m_selectedPos[1], true);

            }
            //彩虹和条状
            else if (special1 == TSpecialBlock.ESpecial_EatAColor &&
                    (special0 == TSpecialBlock.ESpecial_EatLineDir0 || special0 == TSpecialBlock.ESpecial_EatLineDir2 ||     //跟条状消除,六方向加粗
                    special0 == TSpecialBlock.ESpecial_EatLineDir1))
            {
                m_blocks[m_selectedPos[0].x, m_selectedPos[0].y].Eat();                 //自己消失
                AddDelayProceedGrid(m_selectedPos[0].x, m_selectedPos[0].y, 0, m_blocks[m_selectedPos[0].x, m_selectedPos[0].y]);       ////自己消失

                m_blocks[m_selectedPos[1].x, m_selectedPos[1].y].EatAnimationName = CapsConfig.Line_Rainbow_EatAnim;                 //彩虹条状合成动画
                m_blocks[m_selectedPos[1].x, m_selectedPos[1].y].Eat();                 //自己消失
                AddDelayProceedGrid(m_selectedPos[1].x, m_selectedPos[1].y, 0, m_blocks[m_selectedPos[1].x, m_selectedPos[1].y]);       ////自己消失
                //EatALLDirLine(m_selectedPos[1], true);


                m_gameFlow = TGameFlow.EGameState_EffectTime;                           //切换游戏状态到特效演出时间，等待特效演出
                m_curStateStartTime = Timer.millisecondNow();                           //保存下切状态的时间
                m_effectStateDuration = CapsConfig.EffectAllDirBigTime;                 //2秒的演出时间
                m_curSpecialEffect = TSpecialEffect.EAllDirBig;                         //全方向消除特效
                m_curSpecialEffectPos = m_selectedPos[1];                               //把特效位置保存起来
                m_effectStep = 0;                                                       //从第0步开始
            }
            //两个条状交换
            else if ((special0 == TSpecialBlock.ESpecial_EatLineDir0 || special0 == TSpecialBlock.ESpecial_EatLineDir2 ||     //跟条状消除,六方向加粗
                    special0 == TSpecialBlock.ESpecial_EatLineDir1) &&
                (special1 == TSpecialBlock.ESpecial_EatLineDir0 || special1 == TSpecialBlock.ESpecial_EatLineDir2 ||     //跟条状消除,六方向加粗
                    special1 == TSpecialBlock.ESpecial_EatLineDir1))
            {
                m_blocks[m_selectedPos[0].x, m_selectedPos[0].y].Eat();                 //自己消失
                AddDelayProceedGrid(m_selectedPos[0].x, m_selectedPos[0].y, 0, m_blocks[m_selectedPos[0].x, m_selectedPos[0].y]);       ////自己消失

                m_blocks[m_selectedPos[1].x, m_selectedPos[1].y].EatAnimationName = CapsConfig.Line_Rainbow_EatAnim;                 //条状合成动画
                m_blocks[m_selectedPos[1].x, m_selectedPos[1].y].Eat();                 //自己消失
                AddDelayProceedGrid(m_selectedPos[1].x, m_selectedPos[1].y, 0, m_blocks[m_selectedPos[1].x, m_selectedPos[1].y]);       ////自己消失

                EatALLDirLine(m_selectedPos[1], false);
            }
            //炸弹跟条状
            else if (special0 == TSpecialBlock.ESpecial_Bomb && (special1 == TSpecialBlock.ESpecial_EatLineDir0 || special1 == TSpecialBlock.ESpecial_EatLineDir2 ||     //炸弹跟条状交换，单方向加粗
                    special1 == TSpecialBlock.ESpecial_EatLineDir1))
            {

                m_blocks[m_selectedPos[0].x, m_selectedPos[0].y].Eat();                 //自己消失
                AddDelayProceedGrid(m_selectedPos[0].x, m_selectedPos[0].y, 0, m_blocks[m_selectedPos[0].x, m_selectedPos[0].y]);       ////自己消失

                m_blocks[m_selectedPos[1].x, m_selectedPos[1].y].EatAnimationName = CapsConfig.Line_Bomb_EatAnim;                 //条状合成动画
                m_blocks[m_selectedPos[1].x, m_selectedPos[1].y].Eat();                 //自己消失
                AddDelayProceedGrid(m_selectedPos[1].x, m_selectedPos[1].y, 0, m_blocks[m_selectedPos[1].x, m_selectedPos[1].y]);       ////自己消失
                EatALLDirLine(m_selectedPos[1], true, (int)special1);
            }
            //炸弹跟条状
            else if (special1 == TSpecialBlock.ESpecial_Bomb && (special0 == TSpecialBlock.ESpecial_EatLineDir0 || special0 == TSpecialBlock.ESpecial_EatLineDir2 ||
                    special0 == TSpecialBlock.ESpecial_EatLineDir1))
            {
                m_blocks[m_selectedPos[0].x, m_selectedPos[0].y].Eat();                 //自己消失
                AddDelayProceedGrid(m_selectedPos[0].x, m_selectedPos[0].y, 0, m_blocks[m_selectedPos[0].x, m_selectedPos[0].y]);       ////自己消失

                m_blocks[m_selectedPos[1].x, m_selectedPos[1].y].EatAnimationName = CapsConfig.Line_Bomb_EatAnim;                 //条状合成动画
                m_blocks[m_selectedPos[1].x, m_selectedPos[1].y].Eat();                 //自己消失
                AddDelayProceedGrid(m_selectedPos[1].x, m_selectedPos[1].y, 0, m_blocks[m_selectedPos[1].x, m_selectedPos[1].y]);       ////自己消失
                EatALLDirLine(m_selectedPos[1], true, (int)special0);
            }
            //炸弹跟彩虹
            else if (special0 == TSpecialBlock.ESpecial_Bomb && special1 == TSpecialBlock.ESpecial_EatAColor)              //炸弹和彩虹交换，相同颜色变炸弹
            {
                m_blocks[m_selectedPos[1].x, m_selectedPos[1].y].EatAnimationName = CapsConfig.Rainbow_Bomb_EatAnim;
                m_blocks[m_selectedPos[1].x, m_selectedPos[1].y].Eat();
                AddDelayProceedGrid(m_selectedPos[1].x, m_selectedPos[1].y, 0, m_blocks[m_selectedPos[1].x, m_selectedPos[1].y]);       ////自己消失

                m_gameFlow = TGameFlow.EGameState_EffectTime;
				m_effectStateDuration = 0;
				m_curStateStartTime = Timer.millisecondNow();
                m_curSpecialEffect = TSpecialEffect.EEatAColorNDBomb;
				m_colorToBombLastTime = Timer.GetRealTimeSinceStartUp();
                m_curSpecialEffectPos = m_selectedPos[1];
                m_colorToBomb = m_blocks[m_selectedPos[0].x, m_selectedPos[0].y].color;
            }
            //炸弹跟彩虹
            else if (special1 == TSpecialBlock.ESpecial_Bomb && special0 == TSpecialBlock.ESpecial_EatAColor)                //炸弹和彩虹交换，相同颜色变炸弹
            {
                m_blocks[m_selectedPos[0].x, m_selectedPos[0].y].Eat();
                AddDelayProceedGrid(m_selectedPos[0].x, m_selectedPos[0].y, 0, m_blocks[m_selectedPos[0].x, m_selectedPos[0].y]);       ////自己消失

                m_gameFlow = TGameFlow.EGameState_EffectTime;
				m_effectStateDuration = 0;
				m_curStateStartTime = Timer.millisecondNow();
                m_curSpecialEffect = TSpecialEffect.EEatAColorNDBomb;
				m_colorToBombLastTime = Timer.GetRealTimeSinceStartUp();
                m_curSpecialEffectPos = m_selectedPos[0];
                m_colorToBomb = m_blocks[m_selectedPos[1].x, m_selectedPos[1].y].color;
            }
            //炸弹跟炸弹
            else if (special0 == TSpecialBlock.ESpecial_Bomb && special1 == TSpecialBlock.ESpecial_Bomb)
            {
                m_blocks[m_selectedPos[1].x, m_selectedPos[1].y].EatAnimationName = CapsConfig.Rainbow_Bomb_EatAnim;

                m_blocks[m_selectedPos[0].x, m_selectedPos[0].y].Eat();                 //自己消失
                AddDelayProceedGrid(m_selectedPos[0].x, m_selectedPos[0].y, 0, m_blocks[m_selectedPos[0].x, m_selectedPos[0].y]);       ////自己消失
                m_blocks[m_selectedPos[1].x, m_selectedPos[1].y].Eat();                 //自己消失
                AddDelayProceedGrid(m_selectedPos[1].x, m_selectedPos[1].y, 0, m_blocks[m_selectedPos[1].x, m_selectedPos[1].y]);       ////自己消失

                BigBomb(m_selectedPos[1]);
            }
        }

        //如果交换成功////////////////////////////////////////////////////////////////////////
        --PlayingStageData.StepLimit;                                                   //步数扣步数
        m_gameBottomUI.OnChangeStep(PlayingStageData.StepLimit);
        ClearSelected();                                                                //清空所选
        ProcessTempBlocks();                                                            //处理正常移动后消块对场景的影响
    }

    void TimerWork()
    {
        /*------------------处理timerEatBlock------------------*/
        if (CapBlock.EatingBlockCount > 0)      //若有在消块的
        {
            bool bEat = false;
            bool bNeedRefreshTarget = false;        //搜集关可能需要更新目标
            //消块逻辑，把正在消失的块变成粒子，原块置空
            for (int i = 0; i < BlockCountX; i++)
            {
                for (int j = 0; j < BlockCountY; j++)
                {
                    if (m_blocks[i, j] == null)     //为空块
                    {
                        continue;
                    }

                    if (m_blocks[i, j].IsEating() && Timer.GetRealTimeSinceStartUp() > m_blocks[i, j].m_eatStartTime)
                    {
                        if (!m_blocks[i, j].EatEffectPlayed)
                        {
                            m_blocks[i, j].EatEffectPlayed = true;
                            m_blocks[i, j].m_animation.enabled = true;
                            m_blocks[i, j].m_animation.Play(m_blocks[i, j].EatAnimationName);                             //播放吃块动画

                            if (PlayingStageData.Target == GameTarget.Collect)      //搜集关处理吃块
                            {
                                for (int k = 0; k < 3; ++k)
                                {
                                    if (GlobalVars.CurStageData.CollectCount[k] > 0)
                                    {
                                        if (GlobalVars.CurStageData.CollectTypes[k] <= CollectType.Collor7)
                                        {
                                            if (((int)m_blocks[i, j].color - (int)TBlockColor.EColor_White) == ((int)GlobalVars.CurStageData.CollectTypes[k] - (int)CollectType.Collor1))
                                            {
                                                ++PlayingStageData.CollectCount[k];                             //增加一个搜集数量
                                                bNeedRefreshTarget = true;
                                            }
                                        }
                                        else
                                        {
                                            if (((int)m_blocks[i, j].special - (int)TSpecialBlock.ESpecial_EatLineDir0) == ((int)GlobalVars.CurStageData.CollectTypes[k] - (int)CollectType.Line0Bomb))
                                            {
                                                ++PlayingStageData.CollectCount[k];                             //增加一个搜集数量
                                                bNeedRefreshTarget = true;
                                            }
                                        }
                                    }
                                }
                            }

                            AddPartile(m_blocks[i, j].EatEffectName, m_blocks[i, j].EatAudio, i, j);     //添加吃块特效
                        }
                    }

                    if (m_blocks[i, j].IsEating() && Timer.GetRealTimeSinceStartUp() - m_blocks[i, j].m_eatStartTime > EATBLOCK_TIME)       //若消块时间到了
                    {
                        --CapBlock.EatingBlockCount;
                        //清空block信息
                        MakeSpriteFree(i, j);
                        bEat = true;
                    }
                }
            }

            if (bNeedRefreshTarget)
            {
                UIWindowManager.Singleton.GetUIWindow<UIGameHead>().RefreshTarget();
            }

            if (bEat)               //若有块被消除，相当于产生了新的空间，要处理一次下落
            {
                DropDown();
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
                m_blocks[m_selectedPos[1].x, m_selectedPos[1].y].y_move = 0;

                if (m_changeBack)//如果处于换回状态
                {
                    m_changeBack = false;	//清除换回标志
                    //清空选择的方块
                    ClearSelected();
                    return;
                }

                ProcessExchange();      //处理两块交换
            }
            else
            {
                ProcessMoveBlock(m_selectedPos[0], m_selectedPos[1], MOVE_TIME - passTime);           //处理交换块的位移
            }
        }
    }

    void ProcessEatChanges(DelayProceedGrid processGrid, bool showProgress, bool addBlockProgress = false)                //处理吃了某位置后对场景的影响
    {
        bool jellyChanged = false;
        bool needEatBlock = false;          //是否还需要吃块本身?因为若要处理的位置是笼子等，就不需要吃块本身了

        int flag = PlayingStageData.GridData[processGrid.x, processGrid.y];
		if(flag == 0) return;

        ///处理Grid///////////////////////////////////////////////////////////////////////
        if ((flag & (int)GridFlag.Cage) > 0)           //笼子
        {
            PlayingStageData.ClearFlag(processGrid.x, processGrid.y, GridFlag.Cage);
            AddPartile("CageEffect", AudioEnum.Audio_Cage, processGrid.x, processGrid.y);
            m_scoreToShow[processGrid.x, processGrid.y] += CapsConfig.EatCagePoint;
            m_gridBackImage[processGrid.x, processGrid.y].layer1.gameObject.SetActive(false);
            m_cageCheckList.Add(new Position(processGrid.x, processGrid.y));                //记录一个位置，之后需要再检查一次消除
        }
        else if ((flag & (int)GridFlag.Chocolate) > 0)         //巧克力
        {
            PlayingStageData.ClearFlag(processGrid.x, processGrid.y, GridFlag.Chocolate);
            PlayingStageData.ClearFlag(processGrid.x, processGrid.y, GridFlag.NotGenerateCap);
            PlayingStageData.AddFlag(processGrid.x, processGrid.y, GridFlag.GenerateCap);
            --PlayingStageData.ChocolateCount;
            AddPartile("ChocolateEffect", AudioEnum.Audio_Chocolate, processGrid.x, processGrid.y);
            m_scoreToShow[processGrid.x, processGrid.y] += CapsConfig.EatChocolate;
            m_gridBackImage[processGrid.x, processGrid.y].layer1.gameObject.SetActive(false);
            m_chocolateNeedGrow = false;
        }
        else if ((flag & (int)GridFlag.Stone) > 0)            //石头
        {
            PlayingStageData.ClearFlag(processGrid.x, processGrid.y, GridFlag.Stone);
            PlayingStageData.ClearFlag(processGrid.x, processGrid.y, GridFlag.NotGenerateCap);
            PlayingStageData.AddFlag(processGrid.x, processGrid.y, GridFlag.GenerateCap);
            --PlayingStageData.StoneCount;

            AddPartile("StoneEffect", AudioEnum.Audio_Stone, processGrid.x, processGrid.y);
            m_scoreToShow[processGrid.x, processGrid.y] += CapsConfig.EatStonePoint;
            m_gridBackImage[processGrid.x, processGrid.y].layer1.gameObject.SetActive(false);
        }
        else                                                 //检查消冰块
        {
            needEatBlock = true;
            if ((flag & (int)GridFlag.JellyDouble) > 0)
            {
                PlayingStageData.ClearFlag(processGrid.x, processGrid.y, GridFlag.JellyDouble);
                PlayingStageData.AddFlag(processGrid.x, processGrid.y, GridFlag.Jelly);
                AddPartile("JellyEffect", AudioEnum.Audio_Jelly, processGrid.x, processGrid.y);
                m_scoreToShow[processGrid.x, processGrid.y] += CapsConfig.EatJellyDouble;
                m_gridBackImage[processGrid.x, processGrid.y].layer0.spriteName = "Jelly" + ((processGrid.y + (processGrid.x % 2)) % 3);
                jellyChanged = true;
            }
            else if ((flag & (int)GridFlag.Jelly) > 0)
            {
                PlayingStageData.ClearFlag(processGrid.x, processGrid.y, GridFlag.Jelly);
                AddPartile("JellyEffect", AudioEnum.Audio_Jelly, processGrid.x, processGrid.y);
                m_scoreToShow[processGrid.x, processGrid.y] += CapsConfig.EatJelly;
                if (processGrid.x % 2 == 0)
                {
                    m_gridBackImage[processGrid.x, processGrid.y].layer0.spriteName = "Grid" + (processGrid.y % 3);
                }
                else
                {
                    m_gridBackImage[processGrid.x, processGrid.y].layer0.spriteName = "Grid" + ((processGrid.y + 1) % 3);
                }
                jellyChanged = true;
            }
        }

        if (processGrid.bProceeChocolateAround)        //若有正常消除
        {
            ClearChocolateAround(processGrid.x, processGrid.y);   
        }

        if (processGrid.bProceeStoneAround)
        {
            ClearStoneAround(processGrid.x, processGrid.y);         //清周围的石块
        }

        if (jellyChanged)
        {
            UIWindowManager.Singleton.GetUIWindow<UIGameHead>().RefreshTarget();
        }

        //若块不为空
        if (processGrid.block != null)
        {
            if (needEatBlock)       //处理吃块
            {
                if (addBlockProgress && !processGrid.block.EatProgressAdded)
                {
                    processGrid.block.EatProgressAdded = true;
                    m_scoreToShow[processGrid.x, processGrid.y] += 50;                                                  //加分
                }
                processGrid.block.m_dropDownStartTime = 0;
            }
            else
            {
                --CapBlock.EatingBlockCount;                                                                        //减少计数
                processGrid.block.CurState = BlockState.Normal;                                                     // 延迟结束发现不需要消，恢复正常状态
                processGrid.block.m_eatStartTime = 0;
            }
        }

        if (showProgress && m_scoreToShow[processGrid.x, processGrid.y] > 0)
        {
            AddProgress(m_scoreToShow[processGrid.x, processGrid.y], processGrid.x, processGrid.y);
            m_scoreToShow[processGrid.x, processGrid.y] = 0;
        }
    }

    public void ProcessTempBlocks()
    {
        for (int i = 0; i < BlockCountX; ++i)
        {
            for (int j = 0; j < BlockCountY; ++j)
            {
				if (m_scoreToShow[i, j] > 0)
				{
					AddProgress(m_scoreToShow[i, j], i, j);
					m_scoreToShow[i, j] = 0;
				}
            }
        }
    }

    void ProcessMoveBlock(Position from, Position to, int moveTime)
    {
        if (from.x == -1 || to.x == -1) return;
        if (from.x != to.x)		//若x方向上的值不一样，就有x方向上的移动
        {
            m_blocks[from.x, from.y].x_move = (int)((to.x - from.x) * moveTime * BLOCKWIDTH / MOVE_TIME);
            m_blocks[to.x, to.y].x_move = (int)((from.x - to.x) * moveTime * BLOCKWIDTH / MOVE_TIME);
        }
        if (from.x - to.x == 0)
        {
            m_blocks[from.x, from.y].y_move = (int)((to.y - from.y) * moveTime * BLOCKHEIGHT / MOVE_TIME);
            m_blocks[to.x, to.y].y_move = (int)((from.y - to.y) * moveTime * BLOCKHEIGHT / MOVE_TIME);
        }
        else
        {
            if (from.y != to.y)
            {
                m_blocks[from.x, from.y].y_move = (int)((to.y - from.y) * moveTime * (BLOCKHEIGHT / 2) / MOVE_TIME);
                m_blocks[to.x, to.y].y_move = (int)((from.y - to.y) * moveTime * (BLOCKHEIGHT / 2) / MOVE_TIME);
            }
            else
            {
                if (from.x % 2 == 0)
                {
                    m_blocks[from.x, from.y].y_move = (int)(0 - moveTime * (BLOCKHEIGHT / 2) / MOVE_TIME);
                    m_blocks[to.x, to.y].y_move = (int)(moveTime * (BLOCKHEIGHT / 2) / MOVE_TIME);
                }
                else
                {
                    m_blocks[from.x, from.y].y_move = (int)(moveTime * (BLOCKHEIGHT / 2) / MOVE_TIME);
                    m_blocks[to.x, to.y].y_move = (int)(0 - moveTime * (BLOCKHEIGHT / 2) / MOVE_TIME);
                }
            }
        }
    }

    void ProcessDropPic(Position from, Position to)
    {
        float dropTime = Timer.GetRealTimeSinceStartUp() - m_blocks[to.x, to.y].DropingStartTime;

        if (from.x != to.x)		//若x方向上的值不一样，就有x方向上的移动
        {
            m_blocks[to.x, to.y].x_move = (int)((to.x - from.x) * (dropTime * SLIDE_SPEED * BLOCKWIDTH - BLOCKWIDTH));
        }
        if (from.x - to.x == 0)
        {
            m_blocks[to.x, to.y].y_move = (int)(DROP_SPEED * dropTime * BLOCKHEIGHT + dropTime * dropTime * DROP_ACC * BLOCKHEIGHT - (to.y - from.y) * BLOCKHEIGHT);           //垂直下落
        }
        else
        {
            m_blocks[to.x, to.y].y_move = (int)(SLIDE_SPEED * dropTime * (BLOCKHEIGHT / 2) - BLOCKHEIGHT / 2);
        }

        if (m_blocks[to.x, to.y].y_move >= 0)       //下落完毕
        {
            m_blocks[to.x, to.y].x_move = 0;        //清除x_move
            m_blocks[to.x, to.y].y_move = 0;        //清除y_move

            m_blocks[to.x, to.y].CurState = BlockState.MovingEnd;      //移动结束
        }
    }

    bool CheckPosCanDropDown(int x, int y)
    {
        if (!CheckPosAvailable(new Position(x, y))) return false;        //若落到底了，不掉落
        if (m_blocks[x, y] != null || PlayingStageData.GridData[x, y] == 0) return false;           //若下面有块或为空格，不掉落
        if (PlayingStageData.CheckFlag(x, y, GridFlag.Stone | GridFlag.Chocolate | GridFlag.Cage))   //若下面锁住了，不掉落
        {
            return false;
        }
        return true;
    }

    bool DropDown()
    {
        if (m_gameFlow == TGameFlow.EGameState_EffectTime)      //若在特效等待时间
        {
            return false;                                             //停止正常的下落
        }

        bool bDrop = false;             //是否有下落
        bool bNewSpace = true;          //是否有直线下落

        //计算锁的位置
        UpdateSlopeLock();

        while (bNewSpace)                //若有新空间，就循环处理下落，直到没有新的空间出现为止
        {
            bNewSpace = true;

            //循环到没有可以直线下落为止
            while (bNewSpace)
            {
                bNewSpace = false;
                for (int i = 0; i < BlockCountX; ++i)
                {
                    if (DropDownStraight(i))                //垂直下落
                    {
                        bDrop = true;                       //有可以下落的
                        bNewSpace = true;                   //有新空间形成
                    }
                }
            }

            //尝试斜下落
            for (int i = 0; i < BlockCountX; ++i)
            {
                if (DropDownIndirect(i))                //斜向下落
                {
                    bDrop = true;
                    bNewSpace = true;                       //有新空间形成
                    break;
                }
            }
	        //计算锁的位置
            UpdateSlopeLock();
        }
        return bDrop;
    }

    public bool DropDownStraight(int x)
    {
        //下落块，所有可下落区域下落一行////////////////////////////////////////////////////////////////////////
        bool tag = false;
        for (int j = BlockCountY - 1; j >= 0; j--)		//从最下面开始遍历
        {
            Position destPos = new Position();
            if (m_blocks[x, j] != null && m_blocks[x, j].IsDropDownAble())       //若有效块没在下落且没被锁定
            {
                bool bDrop = false;
                int y = j;
                if (CheckPosCanDropDown(x, y + 1))          //下面的格若能掉落
                {
                    ++y;                                         //向下一格
                    //看看可以掉落到什么地方
                    while (true)
                    {
                        if (!CheckPosCanDropDown(x, y + 1))      //向下看一格
                        {
                            break;
                        }
                        ++y;                               //向下一格
                    }
                    destPos.Set(x, y);                      //设置掉落目标点
                    bDrop = true;
                }

                if (bDrop)
                {
                    //处理下落////////////////////////////////////////////////////////////////////////
                    m_blocks[destPos.x, destPos.y] = m_blocks[x, j];             //往下移块
                    if (m_blocks[destPos.x, destPos.y].CurState != BlockState.MovingEnd)        //若为MovingEnd状态，继续下落，则不增加下落块数记录
                    {
                        ++CapBlock.DropingBlockCount;
                        m_blocks[destPos.x, destPos.y].DropingStartTime = Timer.GetRealTimeSinceStartUp();    //重新记录下落开始时间
                        m_blocks[destPos.x, destPos.y].droppingFrom.Set(x, j);       //记录从哪里落过来的
                    }
                    else
                    {
                        if (m_blocks[destPos.x, destPos.y].droppingFrom.x != destPos.x || PlayingStageData.CheckFlag(destPos.x, destPos.y, GridFlag.PortalEnd))         //若不为直线下落
                        {
                            m_blocks[destPos.x, destPos.y].DropingStartTime = Timer.GetRealTimeSinceStartUp();    //重新记录下落开始时间
                            m_blocks[destPos.x, destPos.y].droppingFrom.Set(x, j);       //记录从哪里落过来的
                        }
                    }
                    m_blocks[destPos.x, destPos.y].CurState = BlockState.Moving;
                    m_blocks[x, j] = null;                       //原块置空
                    
                    tag = true;
                }
            }
        }

        //需要补充遍历所有出生点
        for (int j = BlockCountY - 1; j >= 0; j--)		//从最下面开始遍历
        {
            if (PlayingStageData.CheckFlag(x, j, GridFlag.Birth) && !PlayingStageData.CheckFlag(x, j, GridFlag.Chocolate | GridFlag.Stone) && m_blocks[x, j] == null)     //若为出生点，且为空
            {
                int y = j;
                //看看可以掉落到什么地方
                while (true)
                {
                    if (!CheckPosCanDropDown(x, y + 1))      //向下看一格，若可以下落
                    {
                        break;
                    }
                    ++y;                               //向下一格
                }
                //处理下落////////////////////////////////////////////////////////////////////////
                for (int destY = y; destY >= j; --destY)
                {
                    CreateBlock(x, destY, false);                                           //在目标位置生成块
                    m_blocks[x, destY].CurState = BlockState.Moving;
                    m_blocks[x, destY].droppingFrom.Set(x, destY - (y - j) - 1);            //记录从哪里落过来的
                    m_blocks[x, destY].DropingStartTime = Timer.GetRealTimeSinceStartUp();    //记录下落开始时间
                    tag = true;
                    ++CapBlock.DropingBlockCount;
                }
            }
        }
        return tag;			//返回是否发生了掉落
    }

    public bool DropDownIndirect(int x)
    {
        bool bDrop = false;
        Position fromPos = new Position();
        Position toPos = new Position();
        bool bPortal = false;
        //下落块，所有可下落区域下落一行////////////////////////////////////////////////////////////////////////
        for (int j = 0; j < BlockCountY; j++)		//从上往下遍历
        {
            toPos = new Position(x, j);

            if (CheckPosCanDropDown(x, j))       //找到空格
            {
                if (PlayingStageData.CheckFlag(x, j, GridFlag.PortalEnd))       //若为传送门目标点
                {
                    fromPos = PlayingStageData.PortalToMap[toPos.ToInt()].from;
                    if (m_blocks[fromPos.x, fromPos.y] != null && m_blocks[fromPos.x, fromPos.y].IsDropDownAble())
                    {
                        bDrop = true;
                        bPortal = true;
                        break;
                    }
                }

                if (m_slopeDropLock[x] <= j)        //若锁在当前位置的上方
                {
                    continue;
                }
                Position pos = new Position(x, j);
                fromPos = GoTo(pos, TDirection.EDir_LeftUp, 1);
                if (CheckPosAvailable(fromPos) && m_blocks[fromPos.x, fromPos.y] != null && m_blocks[fromPos.x, fromPos.y].IsDropDownAble())
                {
                    bDrop = true;
                    break;
                }
                else
                {
                    fromPos = GoTo(pos, TDirection.EDir_UpRight, 1);
                    if (CheckPosAvailable(fromPos) && m_blocks[fromPos.x, fromPos.y] != null && m_blocks[fromPos.x, fromPos.y].IsDropDownAble())
                    {
                        bDrop = true;
                        break;
                    }
                }
            }
        }
        if (bDrop)
        {
            //处理下落////////////////////////////////////////////////////////////////////////
            m_blocks[x, toPos.y] = m_blocks[fromPos.x, fromPos.y];             //往下移块
            if (m_blocks[x, toPos.y].CurState != BlockState.MovingEnd)          //若为MovingEnd状态，继续下落，则不增加下落块数记录
            {
                ++CapBlock.DropingBlockCount;
            }
            m_blocks[x, toPos.y].CurState = BlockState.Moving;
            if (bPortal)
            {
                m_blocks[x, toPos.y].droppingFrom.Set(x, toPos.y - 1);       //传送门块，直接从上面一格往下落
                m_blocks[x, toPos.y].m_shadowSprite = m_freeShadowSpriteList.Last.Value;                                //指定一个影子图片
                m_blocks[x, toPos.y].m_shadowSprite.gameObject.SetActive(true);
                m_blocks[x, toPos.y].m_shadowSprite.spriteName = m_blocks[x, toPos.y].m_blockSprite.spriteName;
                m_freeShadowSpriteList.RemoveLast();
            }
            else
            {
                m_blocks[x, toPos.y].droppingFrom.Set(fromPos.x, fromPos.y);       //记录从哪里落过来的
            }
            m_blocks[fromPos.x, fromPos.y] = null;                       //原块置空
            m_blocks[x, toPos.y].DropingStartTime = Timer.GetRealTimeSinceStartUp();    //记录下落开始时间
			
            return true;
        }
        return false;			//返回是否发生了掉落
    }

    bool EatLine(Position position)
    {
        int countInSameLine = 1;					//在同一条线上相同的颜色的数量
        int totalSameCount = 1;						//总共相同颜色的数量
        int maxCountInSameDir = 1;					//最大的在同一个方向上相同颜色的数量
        Position[] eatBlockPos = new Position[10];
        eatBlockPos[0] = position;
		
		
		Position[] allEatBlockPos = new Position[20];		//
		allEatBlockPos[0] = position;
		
        Position availablePos = new Position(0, 0);
        availablePos.MakeItUnAvailable();

        TBlockColor color = GetBlockColor(position);
        if (color > TBlockColor.EColor_Grey || color == TBlockColor.EColor_None)
        {
            return false;
        }
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
            if (countInSameLine > maxCountInSameDir)
            {
                maxCountInSameDir = countInSameLine;								//记录在单行中的最大消除数量
                if (maxCountInSameDir > 3)          //若可以生成特殊块
                {
                    //先查找一个可以生成特殊块的位置
                    if (m_blocks[position.x, position.y].special == TSpecialBlock.ESpecial_Normal)
                    {
                        availablePos = position;
                    }
                    else
                    {
                        for (int i = 0; i < countInSameLine; ++i)
                        {
                            if (m_blocks[eatBlockPos[i].x, eatBlockPos[i].y].special == TSpecialBlock.ESpecial_Normal)
                            {
                                availablePos = eatBlockPos[i];
                                break;
                            }
                        }
                    }
                }
            }

            //一条线处理一次消除
            if (countInSameLine >= 3)
            {
				for(int i=1; i<countInSameLine; ++i)
				{
					allEatBlockPos[i + totalSameCount - 1] = eatBlockPos[i];			//
				}
				
                totalSameCount += (countInSameLine - 1);							//记录总的消除数量，减1是因为起始块是各条线公用的
            }
        }

        if (maxCountInSameDir < 3)		//若没产生消除，返回
        {
            return false;
        }

        int kItem = 0;                  //自然消除为0

        TSpecialBlock generateSpecial = TSpecialBlock.ESpecial_Normal;      //用来记录生成的特殊块

        //根据结果来生成道具////////////////////////////////////////////////////////////////////////
        if (maxCountInSameDir >= 5)		//若最大每行消了5个
        {
            if (m_blocks[position.x, position.y].special == TSpecialBlock.ESpecial_NormalPlus5)
            {
                AddGameTime(5);               //增加5秒时间
            }
            generateSpecial = TSpecialBlock.ESpecial_EatAColor;         //生成彩虹
            kItem = 3;
        }
        else if (totalSameCount >= 5)			//若总共消除大于等于6个（3,4消除或者多个3消）
        {
            if (m_blocks[position.x, position.y].special == TSpecialBlock.ESpecial_NormalPlus5)
            {
                AddGameTime(5);               //增加5秒时间
            }
            generateSpecial = TSpecialBlock.ESpecial_Bomb;
            kItem = 2;
        }
        else if (maxCountInSameDir == 4)		//若最大每行消了4个
        {
            if (m_blocks[position.x, position.y].special == TSpecialBlock.ESpecial_NormalPlus5)
            {
                AddGameTime(5);               //增加5秒时间
            }
            if (m_moveDirection == TDirection.EDir_Up || m_moveDirection == TDirection.EDir_Down)
            {
                generateSpecial = TSpecialBlock.ESpecial_EatLineDir0;
            }
            if (m_moveDirection == TDirection.EDir_UpRight || m_moveDirection == TDirection.EDir_LeftDown)
            {
                generateSpecial = TSpecialBlock.ESpecial_EatLineDir1;
            }
            if (m_moveDirection == TDirection.EDir_DownRight || m_moveDirection == TDirection.EDir_LeftUp)
            {
                generateSpecial = TSpecialBlock.ESpecial_EatLineDir2;
            }
            kItem = 1;
        }

        if (generateSpecial != TSpecialBlock.ESpecial_Normal)              //若生成了特殊块
        {
            if (!availablePos.IsAvailable())                                       //还没找到生成位置的情况
            {
                if (m_blocks[position.x, position.y].special == TSpecialBlock.ESpecial_Normal)
                {
                    availablePos = position;
                }
                else
                {
                    for (int i = 0; i < countInSameLine; ++i)
                    {
                        if (m_blocks[eatBlockPos[i].x, eatBlockPos[i].y].special == TSpecialBlock.ESpecial_Normal)
                        {
                            availablePos = eatBlockPos[i];          //找个位置放置
                            break;
                        }
                    }
                }
            }
            if (availablePos.IsAvailable())
            {
                m_blocks[availablePos.x, availablePos.y].special = generateSpecial;                                                 //生成特殊块
                m_blocks[availablePos.x, availablePos.y].RefreshBlockSprite(PlayingStageData.GridData[position.x, position.y]);     //刷新图标                                                                  //记录正常消除
                AddPartile(CapsConfig.AddSpecialEffect, AudioEnum.Audio_itemBirth, availablePos.x, availablePos.y);
				
				DelayProceedGrid grid = new DelayProceedGrid();
                grid.x = availablePos.x;
                grid.y = availablePos.y;
                if (PlayingStageData.StoneCount > 0)
                {
                    grid.bProceeStoneAround = true;
                }
                if (PlayingStageData.ChocolateCount > 0)
                {
                    grid.bProceeChocolateAround = true;
                }
				
                grid.startTime = Timer.GetRealTimeSinceStartUp();
                ProcessEatChanges(grid, false);     //立刻处理一次Grid
            }
        }

        for (int i = 0; i < totalSameCount; ++i )
        {
            Position p = allEatBlockPos[i];
            if (p.x != availablePos.x || p.y != availablePos.y)
            {
                EatBlock(p, "EatEffect");
            }
        }

        //TODO 记分
        ////根据结果来记分
        int kQuantity = 1;
        int kCombo = 1;
        int kLevel = 0;

        if (totalSameCount >= CapsConfig.Instance.MaxKQuanlity)
        {
            kQuantity = CapsConfig.Instance.KQuanlityTable[CapsConfig.Instance.MaxKQuanlity - 3];
        }
        else
        {
            kQuantity = CapsConfig.Instance.KQuanlityTable[totalSameCount - 3];
        }

        if (m_comboCount + 1 >= CapsConfig.Instance.MaxKCombo)
        {
            kCombo = CapsConfig.Instance.KComboTable[CapsConfig.Instance.MaxKCombo - 1];
        }
        else
        {
            kCombo = CapsConfig.Instance.KComboTable[m_comboCount];
        }

        m_scoreToShow[position.x, position.y] += 50 * kQuantity * (kCombo + kItem + kLevel + 1);

        ++m_comboCount;
		
		if(m_comboCount > 1                 //从combo2开始
			&& m_comboCount > m_comboSoundCount         //若combo比声音播放进度快
			&& Timer.GetRealTimeSinceStartUp() - m_comboSoundPlayTime > 0.3f)       //若到了时间间隔
		{
			++m_comboSoundCount;
            m_comboSoundPlayTime = Timer.GetRealTimeSinceStartUp();
			if (m_comboSoundCount > 8)
	        {
	            PlaySoundNextFrame(AudioEnum.Audio_Combo8);
	        }
	        else
	        {
	            PlaySoundNextFrame((AudioEnum)((int)AudioEnum.Audio_Combo1 + m_comboSoundCount - 1));
	        }
		}
		
		if(m_comboCount == 1)
		{
			m_comboSoundCount = 0;
			m_comboSoundPlayTime  = 0;
		}
		
        return true;
    }

    Position FindRandomPos(TBlockColor excludeColor, Position[] excludePos, bool excludeSpecial = false)       //找到某个颜色的随机一个块, 简易算法，性能不好
    {
        int ranNum = m_random.Next() % (BlockCountX * BlockCountY);
        int count = 0;
        for (int i = 0; i < BlockCountX; i++)				//遍历一行
        {
            for (int j = 0; j < BlockCountY; j++)		//遍历一列
            {
                if (count < ranNum)     //先找开始位置
                {
                    ++count;
                    continue;
                }

                if (m_blocks[i, j] == null || m_blocks[i, j].color > TBlockColor.EColor_Grey
                    || m_blocks[i, j].color == excludeColor || m_blocks[i, j].special == TSpecialBlock.ESpecial_EatAColor || m_blocks[i, j].CurState != BlockState.Normal)
                {
                    if (i == BlockCountX - 1 && j == BlockCountY - 1)//Repeat the loop till get a result
                    {
                        i = 0;
                        j = 0;
                    }
                    continue;
                }

                if (excludeSpecial && m_blocks[i, j].special != TSpecialBlock.ESpecial_Normal)      //检查不是Speical
                {
                    if (i == BlockCountX - 1 && j == BlockCountY - 1)//Repeat the loop till get a result
                    {
                        i = 0;
                        j = 0;
                    }
                    continue;
                }

                Position pos = new Position(i, j);
                bool bFind = false;
                if (excludePos != null)
                {
                    for (int k = 0; k < excludePos.Length; ++k)
                    {
                        if (excludePos[k].Equals(pos))
                        {
                            bFind = true;
                            break;
                        }
                    }
                }
                if (!bFind)
                {
                    return pos;
                }

                if (count >= ranNum + BlockCountX * BlockCountY)
                {
                    break;
                }

                if (i == BlockCountX - 1 && j == BlockCountY - 1)//Repeat the loop till get a result
                {
                    i = 0;
                    j = 0;
                }
            }
        }
        return Position.UnAvailablePos;
    }

    void ChangeColor(Position pos, TBlockColor color)
    {
        //更改颜色的操作
        m_blocks[pos.x, pos.y].color = color;
        m_blocks[pos.x, pos.y].RefreshBlockSprite(PlayingStageData.GridData[pos.x, pos.y]);
    }

    void ClearChocolateAround(int x, int y)      //清除周围的巧克力
    {
        for (int i = 0; i < 6; ++i)
        {
            Position pos = GoTo(new Position(x, y), (TDirection)i, 1);
            if (CheckPosAvailable(pos))
            {
                if (PlayingStageData.CheckFlag(pos.x, pos.y, GridFlag.Chocolate))
                {
                    PlayingStageData.ClearFlag(pos.x, pos.y, GridFlag.Chocolate);
                    PlayingStageData.ClearFlag(pos.x, pos.y, GridFlag.NotGenerateCap);
                    PlayingStageData.AddFlag(pos.x, pos.y, GridFlag.GenerateCap);
                    --PlayingStageData.ChocolateCount;

                    m_gridBackImage[pos.x, pos.y].layer1.gameObject.SetActive(false);
                    AddPartile("ChocolateEffect", AudioEnum.Audio_Chocolate, pos.x, pos.y);
					m_chocolateNeedGrow = false;
					AddProgress(CapsConfig.EatChocolate, pos.x, pos.y);
                }
            }
        }
    }

    void ClearStoneAround(int x, int y)        //消除周围的石块
    {
        for (int i = 0; i < 6; ++i)
        {
            Position pos = GoTo(new Position(x, y), (TDirection)i, 1);
            if (CheckPosAvailable(pos))
            {
                if (PlayingStageData.CheckFlag(pos.x, pos.y, GridFlag.Stone))
                {
                    PlayingStageData.ClearFlag(pos.x, pos.y, GridFlag.Stone);
                    PlayingStageData.ClearFlag(pos.x, pos.y, GridFlag.NotGenerateCap);
                    PlayingStageData.AddFlag(pos.x, pos.y, GridFlag.GenerateCap);
                    --PlayingStageData.StoneCount;

                    m_gridBackImage[pos.x, pos.y].layer1.gameObject.SetActive(false);
                    AddPartile("StoneEffect", AudioEnum.Audio_Stone, pos.x, pos.y);
					AddProgress(CapsConfig.EatStonePoint, pos.x, pos.y);
                }
            }
        }
    }

    public void HideHelp()
    {
        if (m_saveHelpBlocks[2].IsAvailable())
        {
            m_bHidingHelp = true;
            try
            {
                for (int i = 0; i < 3; ++i)
                {
                    m_blocks[m_saveHelpBlocks[i].x, m_saveHelpBlocks[i].y].m_animation.enabled = false;
                    m_blocks[m_saveHelpBlocks[i].x, m_saveHelpBlocks[i].y].m_animation.Stop();
                    m_blocks[m_saveHelpBlocks[i].x, m_saveHelpBlocks[i].y].m_animation.transform.localScale = Vector3.one;          //恢复缩放
                    m_blocks[m_saveHelpBlocks[i].x, m_saveHelpBlocks[i].y].m_addColorSprite.alpha = 0.0f;
                }
            }
            catch (System.Exception ex)
            {

            }
        }
    }

    public void ClearHelpPoint()
    {
        if (m_saveHelpBlocks[2].IsAvailable())
        {
            try
            {
                for (int i = 0; i < 3; ++i)
                {
                    m_blocks[m_saveHelpBlocks[i].x, m_saveHelpBlocks[i].y].m_animation.enabled = false;
                    m_blocks[m_saveHelpBlocks[i].x, m_saveHelpBlocks[i].y].m_animation.Stop();
                    m_blocks[m_saveHelpBlocks[i].x, m_saveHelpBlocks[i].y].m_animation.transform.localScale = Vector3.one;          //恢复缩放
                    m_blocks[m_saveHelpBlocks[i].x, m_saveHelpBlocks[i].y].m_addColorSprite.alpha = 0.0f;
                }
            }
            catch (System.Exception ex)
            {
            	
            }
            
            for (int i = 0; i < 3; ++i)
            {
                m_saveHelpBlocks[i].MakeItUnAvailable();
            }
            m_lastHelpTime = Timer.GetRealTimeSinceStartUp();
        }
    }

    void AddDelayProceedGrid(int x, int y, float delay, CapBlock block = null)     //添加一个延迟处理
    {
        DelayProceedGrid grid = new DelayProceedGrid();
        grid.x = x;
        grid.y = y;
        grid.startTime = Timer.GetRealTimeSinceStartUp() + delay;
        grid.block = block;

        if (delay == 0.0f)
        {
            if (PlayingStageData.ChocolateCount > 0)
            {
                //grid.bProceeChocolateAround = true;
            }
            if (PlayingStageData.StoneCount > 0)
            {
                grid.bProceeStoneAround = true;
            }
			
            ProcessEatChanges(grid, false);     //立刻处理一次Grid
        }
        else
        {
            if (PlayingStageData.ChocolateCount > 0)
            {
                //grid.bProceeChocolateAround = true;
            }
            m_delayProcessGrid.AddLast(grid); //增加延迟处理的计划
        }
    }

    public void EatBlock(Position position, string eatEffectName, float delay = 0)                   //吃掉块，通过EatLine或特殊道具功能被调用，会触发被吃的块的功能
    {
        if (position.x >= BlockCountX || position.y >= BlockCountY || position.x < 0 || position.y < 0)
            return;
		
        CapBlock block = m_blocks[position.x, position.y];

        if (block != null &&
            block.CurState != BlockState.Moving &&
            (block.CurState != BlockState.Eating || block.m_eatStartTime > Timer.GetRealTimeSinceStartUp() + delay) && //对于Eating的块，原预计消除的时间比delay后的晚才处理，相当于提前引爆
            block.color <= TBlockColor.EColor_Grey)       //移动不处理特殊块
        {
            block.EatEffectName = eatEffectName;         //消除特效名字，用传进来的
            block.Eat(delay);                             //在这里提前给Block的状态赋值，是为了防止重复EatBlock

            bool hasBeenCollected = false;               //消除关中用来记录是否已被收集

            //处理特殊块
            switch (block.special)
            {
                case TSpecialBlock.ESpecial_Bomb:
                    {
                        for (TDirection dir = TDirection.EDir_Up; dir <= TDirection.EDir_LeftUp; ++dir)
                        {
                            EatBlock(GoTo(position, dir, 1), CapsConfig.BombEatEffect, CapsConfig.BombEffectInterval + CapsConfig.EatBombEffectStartInterval + delay);

                            EatBlock(GoTo(position, dir, 2), CapsConfig.BombEatEffect, CapsConfig.BombEffectInterval * 2 + CapsConfig.EatBombEffectStartInterval + delay);
                        }
                        m_blocks[position.x, position.y].EatAnimationName = CapsConfig.BombEatAnim;
                        m_blocks[position.x, position.y].EatEffectName = "BombEffect";
                        m_blocks[position.x, position.y].EatAudio = AudioEnum.Audio_Bomb;
                    }
                    break;
                case TSpecialBlock.ESpecial_NormalPlus5:
                    {
                        if (m_gameFlow == TGameFlow.EGameState_EndEatingSpecial)
                        {
                            //最后阶段
                            for (TDirection dir = TDirection.EDir_Up; dir <= TDirection.EDir_LeftUp; ++dir)
                            {
                                Position newPos = GoTo(position, dir, 1);                            //第一层
                                EatBlock(newPos, CapsConfig.BombEatEffect, CapsConfig.BombEffectInterval + CapsConfig.EatBombEffectStartInterval + delay);

                                newPos = GoTo(newPos, dir, 1);                                     //第二层
                                EatBlock(newPos, CapsConfig.BombEatEffect, CapsConfig.BombEffectInterval * 2 + CapsConfig.EatBombEffectStartInterval + delay);
                            }
                            m_blocks[position.x, position.y].EatAnimationName = CapsConfig.BombEatAnim;
                            m_blocks[position.x, position.y].EatEffectName = "BombEffect";
                            m_blocks[position.x, position.y].EatAudio = AudioEnum.Audio_Bomb;
                        }
                        else
                        {
                            AddGameTime(5);               //增加5秒时间
                        }
                    }
                    break;
                case TSpecialBlock.ESpecial_EatLineDir0:
                    {
                        for (int i = 1; i < BlockCountX; ++i)
                        {
                            EatBlock(GoTo(position, TDirection.EDir_Down, i), CapsConfig.LineEatEffect, i * CapsConfig.EatLineEffectInterval + CapsConfig.EatLineEffectStartInterval + delay);
                            EatBlock(GoTo(position, TDirection.EDir_Up, i), CapsConfig.LineEatEffect, i * CapsConfig.EatLineEffectInterval + CapsConfig.EatLineEffectStartInterval + delay);
                        }
                        m_blocks[position.x, position.y].EatAnimationName = CapsConfig.LineEatAnim;
                        m_blocks[position.x, position.y].EatEffectName = "Dir0Effect";
                        m_blocks[position.x, position.y].EatAudio = AudioEnum.Audio_Line1;
                    }
                    break;
                case TSpecialBlock.ESpecial_EatLineDir1:
                    {
                        for (int i = 1; i < BlockCountX - 1; ++i)
                        {
                            EatBlock(GoTo(position, TDirection.EDir_UpRight, i), CapsConfig.LineEatEffect, i * CapsConfig.EatLineEffectInterval + CapsConfig.EatLineEffectStartInterval + delay);
                            EatBlock(GoTo(position, TDirection.EDir_LeftDown, i), CapsConfig.LineEatEffect, i * CapsConfig.EatLineEffectInterval + CapsConfig.EatLineEffectStartInterval + delay);
                        }
                        m_blocks[position.x, position.y].EatAnimationName = CapsConfig.LineEatAnim;
                        m_blocks[position.x, position.y].EatEffectName = "Dir1Effect";
                        m_blocks[position.x, position.y].EatAudio = AudioEnum.Audio_Line1;
                    }
                    break;
                case TSpecialBlock.ESpecial_EatLineDir2:
                    {
                        for (int i = 1; i < BlockCountX; ++i)
                        {
                            EatBlock(GoTo(position, TDirection.EDir_LeftUp, i), CapsConfig.LineEatEffect, i * CapsConfig.EatLineEffectInterval + CapsConfig.EatLineEffectStartInterval + delay);
                            EatBlock(GoTo(position, TDirection.EDir_DownRight, i), CapsConfig.LineEatEffect, i * CapsConfig.EatLineEffectInterval + CapsConfig.EatLineEffectStartInterval + delay);
                        }
                        m_blocks[position.x, position.y].EatAnimationName = CapsConfig.LineEatAnim;
                        m_blocks[position.x, position.y].EatEffectName = "Dir2Effect";
                        m_blocks[position.x, position.y].EatAudio = AudioEnum.Audio_Line1;
                    }
                    break;
                case TSpecialBlock.ESpecial_EatAColor:
                    {
                        EatAColor(GetRandomColor(false), position, false, delay);
                    }
                    break;
            }
            AddDelayProceedGrid(position.x, position.y, delay, block);     //添加延迟处理，处理Block和Grid
        }
        else
        {
            AddDelayProceedGrid(position.x, position.y, delay);                                       //添加延迟处理，只处理Grid
        }
        
    }

    public void AddFlyParticle(string name, AudioEnum audio, Position from, Position to, float duration, float delay)
    {
        //先看freeParticleList里面有没有可用的
        LinkedList<ParticleSystem> freeParticleList;
        if (!m_freeParticleMap.TryGetValue(name, out freeParticleList))
        {
            freeParticleList = new LinkedList<ParticleSystem>();
            m_freeParticleMap.Add(name, freeParticleList);
        }

        GameObject gameObj = null;
        ParticleSystem par = null;

        if (freeParticleList.Count > 0)     //若有,从列表里取用
        {
            par = freeParticleList.First.Value;
            gameObj = freeParticleList.First.Value.gameObject;
            freeParticleList.RemoveFirst();
            par.Clear();
        }
        else
        {
            //Todo 临时加的粒子代码
            Object obj = Resources.Load(name);
            gameObj = GameObject.Instantiate(obj) as GameObject;

            gameObj.transform.parent = m_capsPool.transform;

            par = gameObj.GetComponent<ParticleSystem>();
            par.Stop();     //这里先不播放
            gameObj.SetActive(false);           //先隐藏起来
        }

        gameObj.transform.localPosition = new Vector3(GetXPos(from.x), -GetYPos(from.x, from.y), -200);        //指定位置

        FlyParticle flyParticle = new FlyParticle();
        flyParticle.par = par;
        flyParticle.start = gameObj.transform.localPosition;
        flyParticle.end = new Vector3(GetXPos(to.x), -GetYPos(to.x, to.y), -200);
        flyParticle.duration = duration;
        flyParticle.startTime = Timer.GetRealTimeSinceStartUp() + delay;
        flyParticle.audio = audio;
        flyParticle.name = name;

        m_flyParticleList.AddLast(flyParticle);
    }

    public void AddPartile(string name, AudioEnum audio, int x, int y, bool addToGameArea = true, float delay = 0.0f)
    {
        //先看freeParticleList里面有没有可用的
        LinkedList<ParticleSystem> freeParticleList;
        if (!m_freeParticleMap.TryGetValue(name, out freeParticleList))
        {
            freeParticleList = new LinkedList<ParticleSystem>();
            m_freeParticleMap.Add(name, freeParticleList);
        }

        GameObject gameObj = null;
        ParticleSystem par = null;

        if (freeParticleList.Count > 0)     //若有,从列表里取用
        {
            par = freeParticleList.First.Value;
            gameObj = freeParticleList.First.Value.gameObject;
            freeParticleList.RemoveFirst();
        }
        else   //没有，创建新粒子
        {
            //Todo 临时加的粒子代码
            Object obj = Resources.Load(name);
            gameObj = GameObject.Instantiate(obj) as GameObject;
            if (addToGameArea)
            {
                gameObj.transform.parent = m_capsPool.transform;
            }
            else
            {
                gameObj.transform.parent = CenterAnchor.transform;
            }
            par = gameObj.GetComponent<ParticleSystem>();
            par.Stop();     //这里先不播放
        }

        if (addToGameArea)
        {
            gameObj.transform.localPosition = new Vector3(GetXPos(x), -GetYPos(x, y), -200);        //指定位置
        }
        else
        {
            gameObj.transform.localPosition = new Vector3(x, -y, -200);        //指定位置
        }
        
        gameObj.transform.localScale = new Vector3(580.0f, 580.0f, 200.0f);                 //指定位置

        //放到正在播放的列表里
        LinkedList<DelayParticle> particleList;
        if (!m_particleMap.TryGetValue(name, out particleList))
        {
            particleList = new LinkedList<DelayParticle>();
            m_particleMap.Add(name, particleList);
        }

        DelayParticle delayPar = new DelayParticle();
        delayPar.par = par;

        if (delay == 0.0f)
        {
            gameObj.SetActive(true);
            par.Play();

            if (audio != AudioEnum.Audio_None)
            {
                PlaySoundNextFrame(audio);
            }
            
            delayPar.startTime = 0.0f;
            particleList.AddLast(delayPar);
        }
        else
        {
            gameObj.SetActive(false);           //先隐藏起来
            delayPar.startTime = Timer.GetRealTimeSinceStartUp() + delay;
            delayPar.audio = audio;
            particleList.AddLast(delayPar);
        }
    }

    public bool CheckLimit()
    {
        if (GlobalVars.CurStageData.StepLimit > 0 && PlayingStageData.StepLimit == 0)            //限制步数的关卡步用完了
        {
            return true;
        }

        //时间到了
        if (GlobalVars.CurStageData.TimeLimit > 0 && (Timer.millisecondNow() - m_gameStartTime) / 1000.0f > GlobalVars.CurStageData.TimeLimit)
        {
            return true;
        }
        return false;
    }
	
	public bool CheckGetEnoughScore()
	{
		return m_progress >= PlayingStageData.StarScore[0];
	}

    public bool IsStageFinish()                  //检测关卡结束条件
    {
        if (PlayingStageData.Target == GameTarget.ClearJelly)       //若目标为清果冻，计算果冻数量
        {
            if (PlayingStageData.GetJellyCount() == 0)       //若完成目标
            {
                return true;
            }
        }
        else if (PlayingStageData.Target == GameTarget.BringFruitDown)      //看看水果有没有都落地
        {
            if (PlayingStageData.Nut1Count == GlobalVars.CurStageData.Nut1Count && PlayingStageData.Nut2Count == GlobalVars.CurStageData.Nut2Count)
            {
                return true;
            }
        }
        else if (PlayingStageData.Target == GameTarget.Collect)
        {
            for (int i = 0; i < 3; ++i )
            {
                if (GlobalVars.CurStageData.CollectCount[i] > 0 && PlayingStageData.CollectCount[i] < GlobalVars.CurStageData.CollectCount[i])
                {
                    return false;
                }
            }
            return true;
        }
        else if (PlayingStageData.Target == GameTarget.GetScore && CheckGetEnoughScore())     //分数满足最低要求了
        {
            if (GlobalVars.CurStageData.StepLimit > 0 && PlayingStageData.StepLimit == 0)            //限制步数的关卡步用完了
            {
                return true;
            }

            if (GlobalVars.CurStageData.TimeLimit > 0 && (Timer.millisecondNow() - m_gameStartTime) / 1000.0f > GlobalVars.CurStageData.TimeLimit)
            {
                return true;
            }
        }
        return false;
    }

    void ProcessCheckStageFinish()
    {
        if (IsStageFinish())                 //检查关卡是否完成，若已经达成过关条件
        {
			m_stepCountWhenReachTarget = PlayingStageData.StepLimit;		//save the step for submit data later

            bool foundSpecial = false;
            for (int i = 0; i < BlockCountX; ++i)
            {
                for (int j = 0; j < BlockCountY; ++j)
                {
                    if (m_blocks[i, j] != null && m_blocks[i, j].special != TSpecialBlock.ESpecial_Normal)
                    {
                        foundSpecial = true;
                        break;
                    }
                }
            }

            if (foundSpecial || PlayingStageData.StepLimit > 0)     //若能进SugarCrush
            {
                m_gameFlow = TGameFlow.EGameState_SugarCrushAnim;
                m_endingAccelarateStartTime = Timer.GetRealTimeSinceStartUp();
                Time.timeScale = 1.2f;                                                        //临时加快时间
                AddPartile("SugarCrushAnim", AudioEnum.Audio_None, 0, 0, false);
                ClearHelpPoint();
                m_curStateStartTime = Timer.millisecondNow();
            }
            else
            {
                //否则直接结束游戏
                m_gameStartTime = 0;

                GameLogic.Singleton.PlayEndGameAnim();		//play the end anim(move the game area out of screen)
                HideUI();

                //触发关卡结束对话，并在对话结束后切回大地图
                Timer.AddDelayFunc(0.8f, delegate()
                {
                    UIWindowManager.Singleton.GetUIWindow<UIDialog>().TriggerDialog(GlobalVars.CurStageNum, DialogTriggerPos.StageEnd, delegate()
                    {
                        UIWindowManager.Singleton.GetUIWindow<UIRetry>().RefreshData();
                        UIWindowManager.Singleton.GetUIWindow<UIRetry>().ShowWindow();
                    });
                });
            }
            return;
        }
        
        if (CheckLimit())          //若限制已到，失败
        {
            if (PlayingStageData.TimeLimit > 0)         //若是时间关
            {
                bool foundSpecial = false;
                for (int i = 0; i < BlockCountX; ++i)
                {
                    for (int j = 0; j < BlockCountY; ++j)
                    {
                        if (m_blocks[i, j] != null && m_blocks[i, j].special != TSpecialBlock.ESpecial_Normal)
                        {
                            foundSpecial = true;
                            break;
                        }
                    }
                }
                if (foundSpecial)
                {
                    m_gameFlow = TGameFlow.EGameState_SugarCrushAnim;
                    m_endingAccelarateStartTime = Timer.GetRealTimeSinceStartUp();
                    Time.timeScale = 1.2f;                                                    //临时加快时间
                    AddPartile("SugarCrushAnim", AudioEnum.Audio_None, 0, 0, false);
                    ClearHelpPoint();
                    m_curStateStartTime = Timer.millisecondNow();
                    return;
                }
            }
            //否则直接结束游戏
            m_gameStartTime = 0;
            m_gameFlow = TGameFlow.EGameState_End;
            Time.timeScale = 1.0f;                                                        //恢复正常的时间比例
            m_endingAccelarateStartTime = 0;

            if (CapsConfig.EnableGA)        //游戏结束的数据
            {
                GA.API.Design.NewEvent("Stage" + GlobalVars.CurStageNum + ":Failed:Score_Percent", (float)m_progress / PlayingStageData.StarScore[0]);  //记录当前开始的关卡的百分比
                if (PlayingStageData.Target == GameTarget.ClearJelly)
                {
                    GA.API.Design.NewEvent("Stage" + GlobalVars.CurStageNum + ":Failed:JellyCount", PlayingStageData.GetDoubleJellyCount() * 2 + PlayingStageData.GetJellyCount());  //记录失败时的果冻数
                }
				else if(PlayingStageData.Target == GameTarget.BringFruitDown)
				{
					GA.API.Design.NewEvent("Stage" + GlobalVars.CurStageNum + ":Failed:NutCount", m_nut1Count + m_nut2Count);  //记录失败时的果冻数
				}
                else if (PlayingStageData.Target == GameTarget.Collect)
                {
                    GA.API.Design.NewEvent("Stage" + GlobalVars.CurStageNum + ":Failed:Collect", 
                        (float)(GlobalVars.CurStageData.CollectCount[0] - PlayingStageData.CollectCount[0]), 
                        (GlobalVars.CurStageData.CollectCount[1] - PlayingStageData.CollectCount[1]), 
                        (GlobalVars.CurStageData.CollectCount[2] - PlayingStageData.CollectCount[2]));  //记录失败时的果冻数
                }
            }

#if UNITY_ANDROID || UNITY_IPHONE
            if (CapsConfig.EnableTalkingData)
			{
				Dictionary<string, object> param = new Dictionary<string, object>();


				if (PlayingStageData.Target == GameTarget.ClearJelly)
				{
					int jellyCount = PlayingStageData.GetDoubleJellyCount() * 2 + PlayingStageData.GetJellyCount();
					if(jellyCount > 15)
					{
						param["JellyCount"] = ">15";
					}
					else if(jellyCount > 10)
					{
						param["JellyCount"] = ">10";
					}
					else if(jellyCount > 5)
					{
						param["JellyCount"] = ">5";
					} 
					else
					{
						param["JellyCount"] = jellyCount.ToString();
					} 
				}
				else if(PlayingStageData.Target == GameTarget.BringFruitDown)
				{
					int nutCount = m_nut1Count + m_nut2Count;
					param["NutsCount"] = nutCount.ToString();
				}
                else if (PlayingStageData.Target == GameTarget.Collect)
                {

                }

				if (m_progress > PlayingStageData.StarScore[0])
				{
				    param["Score"] = ">100";
				}
				else if (m_progress > PlayingStageData.StarScore[0] * 0.8)
				{
				    param["Score"] = "80-100";
				}
				else if (m_progress > PlayingStageData.StarScore[0] * 0.6)
				{
				    param["Score"] = "60-80";
				}
				else if (m_progress > PlayingStageData.StarScore[0] * 0.4)
				{
				    param["Score"] = "40-60";
				}
				else
				{
				    param["Score"] = "<40";
				}
				
				TalkingDataPlugin.TrackEventWithParameters("Stage" + GlobalVars.CurStageNum + ":Failed", "", param);
			}

#endif

            Timer.AddDelayFunc(2.0f, delegate()
            {
                UIWindowManager.Singleton.GetUIWindow<UIGameEnd>().ShowWindow();            //出游戏结束界面
            });
            AddPartile("EndGameFailed", AudioEnum.Audio_None, 0, 0, false);
        }
    }

    void OnDropEnd()            //所有下落和移动结束时被调用
    {
        m_comboCount = 0;
        if (m_gameFlow != TGameFlow.EGameState_Playing)
        {
            return;
        }

        ProcessCheckStageFinish();

        m_lastHelpTime = Timer.GetRealTimeSinceStartUp();
        m_lastGCTime = Timer.GetRealTimeSinceStartUp();

        CheckFTUE();            //检测一次FTUE

        if (IsStopingChocoGrow)        //处理停止巧克力生长状态的更新
        {
            if (m_stoppingChocoGrowStepLeft > 0)
            {
                --m_stoppingChocoGrowStepLeft;
            }

            if (m_stoppingChocoGrowStepLeft == 0)
            {
                IsStopingChocoGrow = false;
            }
        }

        if (IsStoppingTime)             //处理停止时间状态的更新
        {
            if (m_stoppingTimeStepLeft > 0)
            {
                --m_stoppingTimeStepLeft;
            }

            if (m_stoppingTimeStepLeft == 0)
            {
                IsStoppingTime = false;
            }
        }

        if (m_chocolateNeedGrow && !IsStopingChocoGrow)        //若需要巧克力生长
        {
            ChocolateGrow();
        }
    }

    void ChocolateGrow()        //生长一个巧克力
    {
        //先找到待用的巧克力
        //从随机位置开始
        int randomPos = m_random.Next() % BlockCountX;

        for (int i = 0; i < BlockCountX; i++)
        {
            int xPos = (randomPos + i) % BlockCountX;
            for (int j = 0; j < BlockCountY; j++)
            {
                if (PlayingStageData.CheckFlag(xPos, j, GridFlag.Chocolate))        //找到块了
                {
                    //看相邻的6方向能否生成
                    for (int dir = (int)TDirection.EDir_Up; dir <= (int)TDirection.EDir_LeftUp; ++dir)
                    {
                        Position newPos = GoTo(new Position(xPos, j), (TDirection)dir, 1);
                        if (newPos.x >= BlockCountX || newPos.y >= BlockCountY || newPos.x < 0 || newPos.y < 0)
                            continue;
                        if (PlayingStageData.CheckFlag(newPos.x, newPos.y, GridFlag.Cage))                //若是笼子
                        {
                            continue;
                        }
                        if (m_blocks[newPos.x, newPos.y] == null)
                        {
                            continue;
                        }
                        if (m_blocks[newPos.x, newPos.y].color >= TBlockColor.EColor_Nut1)               //若为坚果
                        {
                            continue;
                        }
                        //找到了，生成一个巧克力
                        MakeSpriteFree(newPos.x, newPos.y);       //把块置空
                        PlayingStageData.AddFlag(newPos.x, newPos.y, GridFlag.Chocolate);       //增加巧克力标记
                        PlayingStageData.ClearFlag(newPos.x, newPos.y, GridFlag.GenerateCap);
                        PlayingStageData.AddFlag(newPos.x, newPos.y, GridFlag.NotGenerateCap);

                        if (m_gridBackImage[newPos.x, newPos.y].layer1 == null)
                        {
                            GameObject newObj = GameObject.Instantiate(m_gridInstance) as GameObject;
                            m_gridBackImage[newPos.x, newPos.y].layer1 = newObj.GetComponent<UISprite>();

                            m_gridBackImage[newPos.x, newPos.y].layer1.transform.parent = m_gridInstance.transform.parent;
                            m_gridBackImage[newPos.x, newPos.y].layer1.transform.localScale = m_gridInstance.transform.localScale;
                            m_gridBackImage[newPos.x, newPos.y].layer1.transform.localPosition = new Vector3(GetXPos(newPos.x), -GetYPos(newPos.x, newPos.y), -110);
                        }
                        m_gridBackImage[newPos.x, newPos.y].layer1.spriteName = "Chocolate";
						m_gridBackImage[newPos.x, newPos.y].layer1.gameObject.SetActive(true);
                        Animation anim = m_gridBackImage[newPos.x, newPos.y].layer1.gameObject.GetComponent<Animation>();
                        anim.Play();
						++PlayingStageData.ChocolateCount;
                        return;
                    }
                }
            }
        }
    }

    public Position GetBlockByTouch(int xPos, int yPos)     //一个点的位置获得对应的块
    {
        //先换算成逻辑上的屏幕坐标
        int x = (int)xPos * CapsApplication.Singleton.Height / Screen.height;
        int y = (int)(Screen.height - yPos) * CapsApplication.Singleton.Height / Screen.height;

        Position p = new Position();

        //不在游戏区，不处理
        if (x < gameAreaX || y < gameAreaY || x > gameAreaX + gameAreaWidth || y > gameAreaY + gameAreaHeight)
        {
            p.MakeItUnAvailable();
            return p;
        }

        //将位置换算成块的坐标
        p.x = (int)((x - gameAreaX) / BLOCKWIDTH);
        if (p.x % 2 == 0)
            p.y = (int)((y - gameAreaY - BLOCKHEIGHT / 2) / BLOCKHEIGHT);
        else
            p.y = (int)((y - gameAreaY) / BLOCKHEIGHT);

        if (p.y >= BlockCountY || p.y < 0)
        {
            p.MakeItUnAvailable();
            return p;
        }


        //精确判断，找出相邻9个点中最近的点
        float minDis = 999999.0f;
        Position newP = new Position();
        int relativeX = x - gameAreaX;
        int relativeY = y - gameAreaY;
        for (int i = p.x - 1; i <= p.x + 1; ++i )
        {
            if (i < 0 || i >= BlockCountX)
            {
                continue;
            }
            for (int j = p.y - 1; j < p.y + 1;++j )
            {
                if (j < 0 || j >= BlockCountY)
                {
                    continue;
                }
				float dis = (x - GetXPos(i)) * (x - GetXPos(i)) + (y - GetYPos(i, j)) * (y - GetYPos(i, j));
                if (minDis > dis)
                {
                    minDis = dis;
                    newP.Set(i, j);
                }
            }
        }

        return newP;
    }

    public void OnTap(Gesture ges)
    {
        if (!IsMoveAble())
        {
            return;
        }

        if (m_gameFlow == TGameFlow.EGameState_FTUE)        //在FTUE状态下，只能点击起始点
        {
            return;
        }

        if (ges.fingerIndex != 0)
        {
            return;
        }

        Position p = GetBlockByTouch((int)ges.position.x, (int)ges.position.y);
        if (!p.IsAvailable())
        {
            return;
        }

        if (GlobalVars.UsingItem == PurchasedItem.ItemInGame_Hammer)     //若正在使用锤子道具
        {
            if (m_blocks[p.x, p.y] != null && m_blocks[p.x, p.y].color < TBlockColor.EColor_Nut1 && m_blocks[p.x, p.y].CurState == BlockState.Normal)       //限制只有普通块在普通状态下才能被选中
			{
                GlobalVars.UsingItemTarget = p;
                UIWindowManager.Singleton.GetUIWindow<UIPurchaseTarget>().SetTarget(GlobalVars.UsingItemTarget);
			}
        }

        if (GlobalVars.EditState == TEditState.ChangeColor)
        {
            ChangeColor(p, GlobalVars.EditingColor);
            m_nut1Count = 0;
            m_nut2Count = 0;
            for (int i = 0; i < BlockCountX; ++i)
            {
                for (int j = 0; j < BlockCountY; ++j)
                {
                    if (m_blocks[i, j] == null) continue;
                    if (m_blocks[i, j].color == TBlockColor.EColor_Nut1)
                    {
                        m_nut1Count++;
                    }

                    if (m_blocks[i, j].color == TBlockColor.EColor_Nut2)
                    {
                        m_nut2Count++;
                    }
                }
            }
        }

        if (GlobalVars.EditState == TEditState.ChangeSpecial)
        {
            m_blocks[p.x, p.y].special = GlobalVars.EditingSpecial;
            m_blocks[p.x, p.y].RefreshBlockSprite(PlayingStageData.GridData[p.x, p.y]);
        }

        if (GlobalVars.EditState == TEditState.EditStageGrid)
        {
            PlayingStageData.GridData[p.x, p.y] = GlobalVars.EditingGrid;
            if ((GlobalVars.EditingGrid & (int)GridFlag.Cage) > 0)
            {
                m_blocks[p.x, p.y].CurState = BlockState.Locked;
            }
            if ((GlobalVars.EditingGrid & (int)GridFlag.Stone) > 0 || (GlobalVars.EditingGrid & (int)GridFlag.Chocolate) > 0)
            {
                if (m_blocks[p.x, p.y] != null)
                {
                    MakeSpriteFree(p.x, p.y);       //把块置空
                }
            }
            if (m_gridBackImage[p.x, p.y] != null)
            {
                if (m_gridBackImage[p.x, p.y].layer0 != null)
                {
                    GameObject.Destroy(m_gridBackImage[p.x, p.y].layer0.gameObject);
                }
                if (m_gridBackImage[p.x, p.y].layer1 != null)
                {
                    GameObject.Destroy(m_gridBackImage[p.x, p.y].layer1.gameObject);
                }
                if (m_gridBackImage[p.x, p.y].layer2 != null)
                {
                    GameObject.Destroy(m_gridBackImage[p.x, p.y].layer2.gameObject);
                }
                if (m_gridBackImage[p.x, p.y].layer3 != null)
                {
                    GameObject.Destroy(m_gridBackImage[p.x, p.y].layer3.gameObject);
                }
                if (PlayingStageData.GridData[p.x, p.y] == 0)
                {
                    m_gridBackImage[p.x, p.y] = null;
                }
            }
            if (PlayingStageData.GridData[p.x, p.y] != 0)
            {
                if (m_gridBackImage[p.x, p.y] == null)
                {
                    m_gridBackImage[p.x, p.y] = new GridSprites();
                }
                ProcessGridSprites(p.x, p.y);
            }
        }

        if (GlobalVars.EditState == TEditState.Eat)
        {
            ClearHelpPoint();
            EatBlock(p, CapsConfig.EatEffect);          //编辑功能
            if (CapBlock.EatingBlockCount == 0)         //若没有形成掉落
            {
                DropDown();                             //掉一次DropDown, 处理敲特殊块的情况
            }
        }

        if (GlobalVars.EditState == TEditState.EditPortal)
        {
            if (!GlobalVars.EditingPortal.from.IsAvailable())                          //在编辑第一个点
            {
                if (PlayingStageData.CheckFlag(p.x, p.y, GridFlag.PortalStart)) //若所选的位置已经是开始点了，不能编辑
                {
                    GlobalVars.EditingPortalTip = "选择了重复的开始点, 重新选择Pos1";
                    return;
                }
                GlobalVars.EditingPortal.from = p;
                GlobalVars.EditingPortalTip = "Edit Portal: 选择Pos2";
            }
            else
            {
                if (p.Equals(GlobalVars.EditingPortal.from))                         //起点终点不能重合
                {
                    GlobalVars.EditingPortalTip = "起始点和终点不能是同一点, 重新选择Pos2";
                    return;
                }

                if (PlayingStageData.CheckFlag(p.x, p.y, GridFlag.PortalEnd)) //若所选的位置已经是终点了，不能编辑
                {
                    GlobalVars.EditingPortalTip = "选择了重复的结束点, 重新选择Pos2";
                    return;
                }

                GlobalVars.EditingPortal.to = p;

                PlayingStageData.PortalToMap.Add(p.ToInt(), GlobalVars.EditingPortal);    //把在编辑的Portal存起来
                PlayingStageData.PortalFromMap.Add(GlobalVars.EditingPortal.from.ToInt(), GlobalVars.EditingPortal);    //把在编辑的Portal存起来

                PlayingStageData.AddFlag(GlobalVars.EditingPortal.from.x, GlobalVars.EditingPortal.from.y, GridFlag.Portal);
                PlayingStageData.AddFlag(GlobalVars.EditingPortal.from.x, GlobalVars.EditingPortal.from.y, GridFlag.PortalStart);
                PlayingStageData.AddFlag(GlobalVars.EditingPortal.to.x, GlobalVars.EditingPortal.to.y, GridFlag.Portal);
                PlayingStageData.AddFlag(GlobalVars.EditingPortal.to.x, GlobalVars.EditingPortal.to.y, GridFlag.PortalEnd);

                int flag = GlobalVars.EditingPortal.flag;
                GlobalVars.EditingPortal = new Portal();
                GlobalVars.EditingPortal.flag = flag;

                GlobalVars.EditingPortalTip = "Edit Portal: 添加成功， 重新选择Pos1";
            }
        }
    }

    public void OnTouchBegin(Gesture ges)
    {
        if (!IsMoveAble())
        {
            return;
        }

        if (ges.fingerIndex != 0)
        {
            return;
        }

        if (GlobalVars.UsingItem != PurchasedItem.None)
        {
            return;
        }

        touchBeginPos.MakeItUnAvailable();

        int x = (int)ges.position.x * CapsApplication.Singleton.Height / Screen.height;
        int y = (int)(Screen.height - ges.position.y) * CapsApplication.Singleton.Height / Screen.height;

        Position p = GetBlockByTouch((int)ges.position.x, (int)ges.position.y);

        if (!p.IsAvailable())
        {
            return;
        }

        if (m_gameFlow == TGameFlow.EGameState_FTUE)        //在FTUE状态下，只能点击起始点
        {
            int step = GlobalVars.CurStageData.StepLimit - PlayingStageData.StepLimit;
            UIFTUE ftue = UIWindowManager.Singleton.GetUIWindow<UIFTUE>();
            if (!ftue.CheckMoveFrom(p))
            {
                return;
            }
        }

        //如果选中一个状态处于不可移动的块，或者一个特殊块，置选中标志为空，返回
        if (m_blocks[p.x, p.y] == null || !m_blocks[p.x, p.y].SelectAble())
        {
            ClearSelected();
            return;
        }

        touchBeginPos.Set(x, y);
		touchBeginGrid.Set(p.x, p.y);
        if (m_gameFlow != TGameFlow.EGameState_FTUE)
        {
            SetHighLight(true, touchBeginGrid.x, touchBeginGrid.y, false, true);
        }
        m_selectedPos[0] = p;
        if (m_selectedPos[0].x == -1) return;
        m_selectedPos[1].x = -1;
        long clickTime = Timer.millisecondNow();
    }

    public void OnTouchEnd(Gesture ges)
    {
        if (ges.fingerIndex != 0)
        {
            return;
        }

        if (GlobalVars.UsingItem != PurchasedItem.None)
        {
            return;
        }

        if (touchBeginGrid.IsAvailable())
        {
            if (m_gameFlow != TGameFlow.EGameState_FTUE)
            {
                SetHighLight(false, touchBeginGrid.x, touchBeginGrid.y, false, true);
            }
            touchBeginGrid.MakeItUnAvailable();
        }
    }

    public void SetHighLight(bool bVal, Position p, bool bLightBackground = false, bool bLightBlock = true)
    {
        SetHighLight(bVal, p.x, p.y, bLightBackground, bLightBlock);
    }

    public void SetHighLight(bool bVal, int x, int y, bool bLightBackground = false, bool bLightBlock = true)         //设置某块高亮
    {
        if (m_blocks[x, y] == null)
        {
            return;
        }
        if (bVal)
        {
            if (bLightBackground)
            {
				m_blocks[x, y].m_addColorSprite.alpha = 0.0f;
                m_blocks[x, y].m_blockSprite.depth = 5;
                if (m_gridBackImage[x, y].layer0 != null)
                {
                    m_gridBackImage[x, y].layer0.depth = 4;


                    if (PlayingStageData.CheckFlag(x, y, GridFlag.Jelly))
                    {
                        m_gridBackImage[x, y].layer0.spriteName = "Jelly" + ((y + (x % 2)) % 3) + "_f";
                    }
                    else if (PlayingStageData.CheckFlag(x, y, GridFlag.JellyDouble))
                    {
                        m_gridBackImage[x, y].layer0.spriteName = "JellyDouble" + ((y + (x % 2)) % 3) + "_f";
                    }
                    else
                    {
                        m_gridBackImage[x, y].layer0.spriteName = "Grid" + ((y + (x % 2)) % 3) + "_f";
                    }
                }
                if (m_gridBackImage[x, y].layer1 != null)
                {
                    m_gridBackImage[x, y].layer1.depth = 6;
                }
            }
            if (bLightBlock)
			{
                m_blocks[x, y].m_addColorSprite.alpha = 1.0f;
			}
        }
        else
        {
            if (bLightBlock)
            {
                m_blocks[x, y].m_addColorSprite.alpha = 0.0f;
            }

            if (bLightBackground)
            {
                m_blocks[x, y].m_blockSprite.depth = 2;
                if (m_gridBackImage[x, y].layer0 != null)
                {
                    m_gridBackImage[x, y].layer0.depth = 0;

                    if (PlayingStageData.CheckFlag(x, y, GridFlag.Jelly))
                    {
                        m_gridBackImage[x, y].layer0.spriteName = "Jelly" + ((y + (x % 2)) % 3);
                    }
                    else if (PlayingStageData.CheckFlag(x, y, GridFlag.JellyDouble))
                    {
                        m_gridBackImage[x, y].layer0.spriteName = "JellyDouble" + ((y + (x % 2)) % 3);
                    }
                    else
                    {
                        m_gridBackImage[x, y].layer0.spriteName = "Grid" + ((y + (x % 2)) % 3);
                    }
                }
                if (m_gridBackImage[x, y].layer1 != null)
                {
                    m_gridBackImage[x, y].layer1.depth = 3;
                }
            }
        }
    }

    public bool IsMoveAble()
    {
        if (CapsConfig.Instance.GameSpeed == 0.0f)
        {
            return false;
        }
        if (m_gameFlow != TGameFlow.EGameState_Playing && m_gameFlow != TGameFlow.EGameState_FTUE)
        {
            return false;
        }
        if (timerMoveBlock.GetState() == TimerEnum.ERunning)
        {
            return false;
        }
        if (GlobalVars.CurStageData.StepLimit > 0 && PlayingStageData.StepLimit == 0)
        {
            return false;
        }
        return CapBlock.DropingBlockCount == 0 && CapBlock.EatingBlockCount == 0;
    }

    public void OnTouchMove(Gesture ges)
    {
        if (!IsMoveAble())
        {
            return;
        }

        if (ges.fingerIndex != 0)
        {
            return;
        }

        if (GlobalVars.UsingItem != PurchasedItem.None)
        {
            return;
        }
		
		Position pos = GetBlockByTouch((int)ges.position.x, (int)ges.position.y);
		if(!pos.IsAvailable())
		{
			m_selectedPos[0].MakeItUnAvailable();
			return;
		}

        int x = (int)ges.position.x * CapsApplication.Singleton.Height / Screen.height;
        int y = (int)(Screen.height - ges.position.y) * CapsApplication.Singleton.Height / Screen.height;

        //若没选好第一个块，看看移动中能否选到第一个块
        if (m_selectedPos[0].x == -1)		
        {
            if (m_gameFlow == TGameFlow.EGameState_FTUE)        //在FTUE状态下，只能从起始点开始点击
            {
                return;
            }
			if (GetBlock(pos) == null || !GetBlock(pos).SelectAble())
	        {
	            return;
	        }
			
            m_selectedPos[0] = pos;
            touchBeginPos.Set(x, y);
        }

        float lenth = Vector2.Distance(new Vector2(x, y), new Vector2(touchBeginPos.x, touchBeginPos.y));       //移动距离
        if (lenth < BLOCKWIDTH * 0.6f)             //移动距离不够不行
        {
            return;
        }

		float angle = Mathf.Atan2( touchBeginPos.y - y, x - touchBeginPos.x) * Mathf.Rad2Deg;	
		
		TDirection dir;
		if(angle > 0)
		{
			dir = (TDirection)((6 + (int)TDirection.EDir_UpRight - ((int)angle) / 60) % 6);
		}
		else
		{
			dir = (TDirection)((6 + (int)TDirection.EDir_DownRight + ((int)-angle) / 60) % 6);
		}
		

        //选中第二个
        Position p = GoTo(m_selectedPos[0], dir, 1);

        if (!CheckPosAvailable(p))
        {
            return; 
        }

        if (GetBlock(p) == null || !GetBlock(p).SelectAble())
        {
            return;
        }

        if (m_gameFlow == TGameFlow.EGameState_FTUE)        //在FTUE状态下，只能移向目标点
        {
            int step = GlobalVars.CurStageData.StepLimit - PlayingStageData.StepLimit;
            UIFTUE ftue = UIWindowManager.Singleton.GetUIWindow<UIFTUE>();
            if (!ftue.CheckMoveTo(p))
            {
                return;
            }
            ftue.HideHighLight();
        }

        m_selectedPos[1] = p;

        //取消开始触控点的高亮
        if (touchBeginGrid.IsAvailable())
        {
            SetHighLight(false, touchBeginGrid.x, touchBeginGrid.y, false, true);
            touchBeginGrid.MakeItUnAvailable();
        }

        //处理移动
        MoveSelected();

        if (m_gameFlow == TGameFlow.EGameState_FTUE)        //在FTUE状态下，产生移动后FTUE就消失了
        {
            UIFTUE ftue = UIWindowManager.Singleton.GetUIWindow<UIFTUE>();
            ftue.EndFTUE();
        }
    }

    void MoveSelected()
    {
        m_lastGCTime = 0;
        m_chocolateNeedGrow = true;
        ClearHelpPoint();
        MoveBlockPair(m_selectedPos[0], m_selectedPos[1]);
    }

    void BigBomb(Position pos)
    {
        EatBlock(pos, CapsConfig.BombEatEffect);
        for (TDirection dir = TDirection.EDir_Up; dir <= TDirection.EDir_LeftUp; ++dir)
        {
            Position newPos = GoTo(pos, dir, 1);                            //第一层
            EatBlock(newPos, CapsConfig.BombEatEffect, CapsConfig.EatBigBombEffectStartInterval + CapsConfig.BigBombEffectInterval);

            newPos = GoTo(pos, dir, 2);                                     //第二层
            EatBlock(newPos, CapsConfig.BombEatEffect, CapsConfig.EatBigBombEffectStartInterval + CapsConfig.BigBombEffectInterval * 2);

            newPos = GoTo(newPos, (TDirection)(((int)dir + 2) % 6), 1);     //第二层向下一个方向走一步
            EatBlock(newPos, CapsConfig.BombEatEffect, CapsConfig.EatBigBombEffectStartInterval + CapsConfig.BigBombEffectInterval * 2);

            Position newPos3 = GoTo(pos, dir, 3);                                     //第三层
            EatBlock(newPos3, CapsConfig.BombEatEffect, CapsConfig.EatBigBombEffectStartInterval + CapsConfig.BigBombEffectInterval * 3);

            newPos = GoTo(newPos3, (TDirection)(((int)dir + 2) % 6), 1);     //第三层向下一个方向走一步
            EatBlock(newPos, CapsConfig.BombEatEffect, CapsConfig.EatBigBombEffectStartInterval + CapsConfig.BigBombEffectInterval * 3);

            newPos = GoTo(newPos3, (TDirection)(((int)dir - 2 + 6) % 6), 1); //第三层向上一个方向走一步
            EatBlock(newPos, CapsConfig.BombEatEffect, CapsConfig.EatBigBombEffectStartInterval + CapsConfig.BigBombEffectInterval * 3);
        }

        AddPartile("BigBombEffect", AudioEnum.Audio_Bomb, pos.x, pos.y);
    }

    void EatALLDirLine(Position startPos, bool extraEat, int dir = -1)
    {
        float startDelay = CapsConfig.Line_Line_EffectStartDelay;
        float intervalTime = CapsConfig.Line_Line_EffectInterval;

        if (dir != -1)          //若为炸弹合条
        {
            startDelay = CapsConfig.Line_Bomb_EffectStartDelay;
            intervalTime = CapsConfig.Line_Bomb_EffectInterval;
        }
        
        if (dir == -1 && extraEat)      //彩虹合条
        {
            startDelay = CapsConfig.Line_Rainbow_EffectStartDelay;
            intervalTime = CapsConfig.Line_Rainbow_EffectInterval;
        }

        if (dir == -1 || dir == (int)TSpecialBlock.ESpecial_EatLineDir1)
        {
            //6方向加粗
            for (int i = 1; i < BlockCountX - 1; ++i)
            {
                Position pos = startPos;
                EatBlock(GoTo(pos, TDirection.EDir_UpRight, i), CapsConfig.LineEatEffect, i * intervalTime + startDelay);
                EatBlock(GoTo(pos, TDirection.EDir_LeftDown, i), CapsConfig.LineEatEffect, i * intervalTime + startDelay);


                if (extraEat)
                {
                    pos.Set(startPos.x, startPos.y + 1);
                    if (i == 1)
                    {
                        EatBlock(pos, CapsConfig.LineEatEffect, 0.0f);
                    }
                    EatBlock(GoTo(pos, TDirection.EDir_UpRight, i), CapsConfig.LineEatEffect, i * intervalTime + startDelay);
                    EatBlock(GoTo(pos, TDirection.EDir_LeftDown, i), CapsConfig.LineEatEffect, i * intervalTime + startDelay);

                    pos.Set(startPos.x, startPos.y - 1);
                    if (i == 1)
                    {
                        EatBlock(pos, CapsConfig.LineEatEffect, 0.0f);
                    }
                    EatBlock(GoTo(pos, TDirection.EDir_UpRight, i), CapsConfig.LineEatEffect, i * intervalTime + startDelay);
                    EatBlock(GoTo(pos, TDirection.EDir_LeftDown, i), CapsConfig.LineEatEffect, i * intervalTime + startDelay);
                }
            }
			if (extraEat)
			{
                AddPartile("Dir1BigEffect", AudioEnum.Audio_Line1, startPos.x, startPos.y);
			}
			else
			{
                AddPartile("Dir1Effect", AudioEnum.Audio_Line1, startPos.x, startPos.y);
			}
        }
        if (dir == -1 || dir == (int)TSpecialBlock.ESpecial_EatLineDir0)
        {
            for (int i = 1; i < BlockCountX - 1; ++i)
            {
                Position pos = startPos;
                EatBlock(GoTo(pos, TDirection.EDir_Up, i), CapsConfig.LineEatEffect, i * intervalTime + startDelay);
                EatBlock(GoTo(pos, TDirection.EDir_Down, i), CapsConfig.LineEatEffect, i * intervalTime + startDelay);
                if (extraEat)
                {
                    pos.Set(startPos.x + 1, startPos.y);
                    if (i == 1)
                    {
                        EatBlock(pos, CapsConfig.LineEatEffect, 0.0f);
                    }
                    EatBlock(GoTo(pos, TDirection.EDir_Up, i), CapsConfig.LineEatEffect, i * intervalTime + startDelay);
                    EatBlock(GoTo(pos, TDirection.EDir_Down, i), CapsConfig.LineEatEffect, i * intervalTime + startDelay);

                    pos.Set(startPos.x - 1, startPos.y);
                    if (i == 1)
                    {
                        EatBlock(pos, CapsConfig.LineEatEffect, 0.0f);
                    }
                    EatBlock(GoTo(pos, TDirection.EDir_Up, i), CapsConfig.LineEatEffect, i * intervalTime + startDelay);
                    EatBlock(GoTo(pos, TDirection.EDir_Down, i), CapsConfig.LineEatEffect, i * intervalTime + startDelay);
                }
            }
            if (extraEat)
            {
                AddPartile("Dir0BigEffect", AudioEnum.Audio_Line1, startPos.x, startPos.y);
            }
            else
            {
                AddPartile("Dir0Effect", AudioEnum.Audio_Line1, startPos.x, startPos.y);
            }
        }
        if (dir == -1 || dir == (int)TSpecialBlock.ESpecial_EatLineDir2)
        {
            for (int i = 1; i < BlockCountX - 1; ++i)
            {
                Position pos = startPos;
                EatBlock(GoTo(pos, TDirection.EDir_LeftUp, i), CapsConfig.LineEatEffect, i * intervalTime + startDelay);
                EatBlock(GoTo(pos, TDirection.EDir_DownRight, i), CapsConfig.LineEatEffect, i * intervalTime + startDelay);

                if (extraEat)
                {
                    pos.Set(startPos.x, startPos.y + 1);
                    if (i == 1)
                    {
                        EatBlock(pos, CapsConfig.LineEatEffect, 0.0f);
                    }
                    EatBlock(GoTo(pos, TDirection.EDir_LeftUp, i), CapsConfig.LineEatEffect, i * intervalTime + startDelay);
                    EatBlock(GoTo(pos, TDirection.EDir_DownRight, i), CapsConfig.LineEatEffect, i * intervalTime + startDelay);

                    pos.Set(startPos.x, startPos.y - 1);
                    if (i == 1)
                    {
                        EatBlock(pos, CapsConfig.LineEatEffect, 0.0f);
                    }
                    EatBlock(GoTo(pos, TDirection.EDir_LeftUp, i), CapsConfig.LineEatEffect, i * intervalTime + startDelay);
                    EatBlock(GoTo(pos, TDirection.EDir_DownRight, i), CapsConfig.LineEatEffect, i * intervalTime + startDelay);
                }
            }
            if (extraEat)
            {
                AddPartile("Dir2BigEffect", AudioEnum.Audio_Line1, startPos.x, startPos.y);
            }
            else
            {
                AddPartile("Dir2Effect", AudioEnum.Audio_Line1, startPos.x, startPos.y);
            }
        }
    }

    void EatAColor(TBlockColor color, Position startPos, bool bExchange = false, float delay = 0)
    {
        float startDelay = CapsConfig.Rainbow_EffectStartDelay;
        float intervalTime = CapsConfig.Rainbow_EffectInterval;
        float flyDuration = CapsConfig.Rainbow_EffectFlyDuration;

        if (color == TBlockColor.EColor_None)
        {
            startDelay = CapsConfig.Rainbow_Rainbow_StartDelay;
            intervalTime = CapsConfig.Rainbow_Rainbow_EffectInterval;
            flyDuration = CapsConfig.Rainbow_Rainbow_EffectFlyDuration;
        }

        int eatCount = 0;
        for (int i = 0; i < BlockCountX; ++i)
        {
            for (int j = 0; j < BlockCountY; ++j)
            {
                if (m_blocks[i, j] == null)
                {
                    continue;
                }

                if ((m_blocks[i, j].CurState != BlockState.Normal && m_blocks[i, j].CurState != BlockState.Locked) || m_blocks[i, j].color >= TBlockColor.EColor_Nut1)
                {
                    continue;
                }
                if (color != TBlockColor.EColor_None && m_blocks[i, j].special == TSpecialBlock.ESpecial_EatAColor)
                {
                    continue;
                }
                if (color == TBlockColor.EColor_None)
                {
                    EatBlock(new Position(i, j), CapsConfig.RainbowEatEffect, flyDuration + startDelay + intervalTime * eatCount);
                    AddFlyParticle("EatColorFlyEffect", AudioEnum.Audio_None, startPos, new Position(i, j), flyDuration, startDelay + intervalTime * eatCount);
                    ++eatCount;
                }
                else if (m_blocks[i, j].color == color)
                {
                    if (bExchange)          //手动交换,播放有时序的动画
                    {
                        EatBlock(new Position(i, j), CapsConfig.RainbowEatEffect, flyDuration + startDelay + intervalTime * eatCount);
                        AddFlyParticle("EatColorFlyEffect", AudioEnum.Audio_None, startPos, new Position(i, j), flyDuration, startDelay + intervalTime * eatCount);
                    }
                    else                   //被动交换
                    {
                        EatBlock(new Position(i, j), CapsConfig.RainbowEatEffect, flyDuration + delay);
                        AddFlyParticle("EatColorFlyEffect", AudioEnum.Audio_None, startPos, new Position(i, j), flyDuration, delay);
                    }

                    ++eatCount;
                }
            }
        }
        m_blocks[startPos.x, startPos.y].EatAnimationName = CapsConfig.RainbowEatAnim;
        m_blocks[startPos.x, startPos.y].EatEffectName = "EatColorEffect";
        m_blocks[startPos.x, startPos.y].EatAudio = AudioEnum.Audio_EatColor;
        m_blocks[startPos.x, startPos.y].Eat(delay);
        AddDelayProceedGrid(startPos.x, startPos.y, delay, m_blocks[startPos.x, startPos.y]);       ////自己消失

        if (bExchange)
        {
            m_gameFlow = TGameFlow.EGameState_EffectTime;
            m_effectStateDuration = (int)((flyDuration + startDelay + intervalTime * eatCount) * 1000);
            m_curStateStartTime = Timer.millisecondNow();
        }
    }

    void ChangeColorToBomb(TBlockColor color)
    {
        for (int i = 0; i < BlockCountX; ++i)
        {
            for (int j = 0; j < BlockCountY; ++j)
            {
                if (m_blocks[i, j] != null && m_blocks[i, j].color == color)
                {
                    if (m_blocks[i, j].special == TSpecialBlock.ESpecial_Normal)
                    {
                        m_blocks[i, j].special = TSpecialBlock.ESpecial_Bomb;
                    }
                }
            }
        }

        for (int i = 0; i < BlockCountX; ++i)
        {
            for (int j = 0; j < BlockCountY; ++j)
            {
                if (m_blocks[i, j] != null && m_blocks[i, j].color == color)
                {
                    EatBlock(new Position(i, j), CapsConfig.EatEffect);
                }
            }
        }
    }

    void MoveBlockPair(Position position1, Position position2)
    {
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
    }

    void ExchangeBlock(Position position1, Position position2)
    {
        CapBlock tempBlock = m_blocks[position1.x, position1.y];
        m_blocks[position1.x, position1.y] = m_blocks[position2.x, position2.y];
        m_blocks[position2.x, position2.y] = tempBlock;
    }

    CapBlock GetFreeCapBlock(TBlockColor color)
    {
        CapBlock block = m_capBlockFreeList.Last.Value;
        block.m_blockTransform.gameObject.SetActive(true);
		block.EatAnimationName = CapsConfig.EatAnim;           //消除动画名字，先用默认的
		block.EatEffectName = CapsConfig.EatEffect;
        block.EatAudio = AudioEnum.Audio_Eat;
        m_capBlockFreeList.RemoveLast();
        return block;
    }

    int m_idCount = 0;
    int m_lastPlus5Step = 0;            //上次+5的步数
    int m_plus5Count = 0;

    bool CreateBlock(int x, int y, bool avoidLine)
    {
        TBlockColor color = GetRandomColor(PlayingStageData.CheckFlag(x, y, GridFlag.Birth) && !PlayingStageData.CheckFlag(x, y, GridFlag.Cage));		//最上方获取新的方块，笼子里不能生成坚果
        m_blocks[x, y] = GetFreeCapBlock(color);            //创建新的块 Todo 变成用缓存
        m_blocks[x, y].color = color;               //设置颜色
        m_blocks[x, y].m_animation.enabled = false;

        if (Timer.millisecondNow() - m_gameStartTime > PlayingStageData.PlusStartTime * 1000)       //若超过了开始掉+5的时间
        {
            //处理+5
            if (PlayingStageData.TimeLimit > 0 && GlobalVars.CurStageData.StepLimit - PlayingStageData.StepLimit > m_lastPlus5Step + PlayingStageData.PlusStep)
            {
                m_blocks[x, y].special = TSpecialBlock.ESpecial_NormalPlus5;            //生成一个+5
                m_lastPlus5Step = GlobalVars.CurStageData.StepLimit - PlayingStageData.StepLimit;
            }
        }

        if (avoidLine)
        {
            int count = 0;
            while (IsHaveLine(new Position(x, y)))
            {
                if (count >= PlayingStageData.ColorCount)
                {
                    return false;
                }

                m_blocks[x, y].color = GetNextColor(m_blocks[x, y].color);		//若新生成瓶盖的造成消行，换一个颜色
                ++count;
            }
        }
		if(x == 2 && y == 5)
		{
			int test = 10;
			int test2 = test;
		}
        m_blocks[x, y].RefreshBlockSprite(PlayingStageData.GridData[x, y]);                                         //刷新下显示内容

        return true;
    }

    void MakeSpriteFree(int x, int y)
    {
        m_blocks[x, y].m_addColorSprite.alpha = 0.0f;
        m_blocks[x, y].EatProgressAdded = false;
        m_blocks[x, y].m_animation.Stop();
        m_blocks[x, y].m_animation.enabled = false;
        m_capBlockFreeList.AddLast(m_blocks[x, y]);
        m_blocks[x, y].m_blockTransform.gameObject.SetActive(false);
        m_blocks[x, y].Reset();
        m_blocks[x, y] = null;
    }

    void ClearSelected()
    {
        m_selectedPos[0].x = -1;
        m_selectedPos[1].x = -1;
    }

    TBlockColor GetRandomColor(bool bGenerateNut)
    {
        if (bGenerateNut && PlayingStageData.Target == GameTarget.BringFruitDown          //若为出生点，且水果下落模式，且还没掉够坚果
            && m_nut1Count + PlayingStageData.Nut1Count + m_nut2Count + PlayingStageData.Nut2Count < GlobalVars.CurStageData.Nut1Count + GlobalVars.CurStageData.Nut2Count)
        {

            bool AddNut = false;        //记录是否生成坚果的结果

            if (m_nut1Count + m_nut2Count + PlayingStageData.Nut2Count + PlayingStageData.Nut1Count < PlayingStageData.NutInitCount)     //若还没到初始数量
            {
                AddNut = true;          //生成
            }

            else if (PlayingStageData.NutMinCount > 0 && m_nut1Count + m_nut2Count < PlayingStageData.NutMinCount)
            {
                AddNut = true;
            }

            else if (PlayingStageData.NutMaxCount == 0 || m_nut1Count + m_nut2Count < PlayingStageData.NutMaxCount)       //画面上坚果数量已经小于最大数量，不生成
            {
                if (GlobalVars.CurStageData.StepLimit - PlayingStageData.StepLimit >
                    (m_nut1Count + m_nut2Count + PlayingStageData.Nut1Count + PlayingStageData.Nut2Count - PlayingStageData.NutInitCount + 1) * PlayingStageData.NutStep)      //若已经到步数了
                {
                    AddNut = true;
                }
            }

            if (AddNut)
            {
                TBlockColor nutColor;
                if (m_nut1Count + PlayingStageData.Nut1Count >= GlobalVars.CurStageData.Nut1Count)      //若已掉 够了，就不再掉
                {
                    nutColor = TBlockColor.EColor_Nut2;
                }

                else if (m_nut2Count + PlayingStageData.Nut2Count >= GlobalVars.CurStageData.Nut2Count)      //若已掉 够了，就不再掉
                {
                    nutColor = TBlockColor.EColor_Nut1;
                }
                else
                {
                    nutColor = TBlockColor.EColor_Nut1 + m_random.Next() % 2;
                }

                if (nutColor == TBlockColor.EColor_Nut1)
                {
                    ++m_nut1Count;
                }
                else
                {
                    ++m_nut2Count;
                }
                return nutColor;
            }
        }

        return TBlockColor.EColor_White + m_random.Next() % PlayingStageData.ColorCount;
    }

    TBlockColor GetNextColor(TBlockColor color)
    {
        int index = color - TBlockColor.EColor_White;
        return (TBlockColor)((index + 1) % PlayingStageData.ColorCount + TBlockColor.EColor_White);
    }

    bool IsHaveLine()
    {
        for (int i = 0; i < BlockCountX; ++i )
        {
            for (int j = 0; j < BlockCountY; ++j )
            {
                if (m_blocks[i, j] != null)
                {
                    if (IsHaveLine(new Position(i, j)))
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    bool IsHaveLine(Position position, bool saveBlocks =false)
    {
        int countInSameLine = 1;					//在同一条线上相同的颜色的数量
        TBlockColor color = GetBlockColor(position);
		if(color == TBlockColor.EColor_None)
			return false;
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
                if (saveBlocks)
                {
                    m_saveHelpBlocks[countInSameLine] = curPos;
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
                if (saveBlocks)
                {
                    m_saveHelpBlocks[countInSameLine] = curPos;
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

    public CapBlock GetBlock(Position p)							//获得某位置的块对象
    {
        return m_blocks[p.x, p.y];
    }

    bool CheckPosAvailable(Position p)							//获得某位置是否可用
    {
        if (p.x < 0 || p.x >= BlockCountX || p.y < 0 || p.y >= BlockCountY)
        {
            return false;
        }
        return true;
    }

    TBlockColor GetBlockColor(Position p)						//获得位置的颜色
    {
        if (!CheckPosAvailable(p) || m_blocks[p.x, p.y] == null)
        {
            return TBlockColor.EColor_None;
        }
        if (m_blocks[p.x, p.y].special == TSpecialBlock.ESpecial_EatAColor)         //Todo EatAColor是否变成一个Color而不是一个Specail?
        {
            return TBlockColor.EColor_None;
        }
        if (m_blocks[p.x, p.y].CurState != BlockState.Moving)
        {
            return m_blocks[p.x, p.y].color;
        }

        return TBlockColor.EColor_None;
    }

    Position GoTo(Position pos, TDirection direction, int step)
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
