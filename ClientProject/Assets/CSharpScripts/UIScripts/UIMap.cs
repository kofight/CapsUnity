using UnityEngine;
using System.Collections;

public enum StageType
{
    Time,
    Score,
    Jelly,
    Nut,
    Collect,
}

public class UIMap : UIWindow 
{
    static readonly int Width = 2048;
    static readonly int Height = 2048;
    static readonly int Border = 100;           //地图四周留的边

    Transform m_backGroundTrans;                //背景的Transfrom
    private Vector2 m_curOffSet;
    private Vector2 m_destOffSet;
	
	SpringPanel springPanel;

    UISprite m_headSprite;                      //头像位置
    UISprite m_cloudSprite;                     //云的图片
	UISprite m_cloud2Sprite;                     //云的图片

    Transform[] m_stageBtns;

    UIWindow m_heartUI;

    GameObject m_inputBlocker;

    GameObject m_timeNumber;
    NumberDrawer m_minNumber;
    NumberDrawer m_secNumber;
    NumberDrawer m_coinNumber;
    GameObject m_fullText;
    GameObject m_mapObj;

    int m_newStageNumber;                       //开启的新关卡的编号
    float m_newStageMoveTime;                  //开启新关卡的时间
    readonly static float HeadMoveTime = 3.0f;  //开启新关卡时头像的移动时间
    readonly static float HeadYOffset = 110.0f; //头像相对于按钮位置的位移

    float m_lastClickStageTime = 0;                 //上次点击关卡的时间
    GameObject m_helpParticle;                      //帮助特效

    GameObject[] m_blurSprites = new GameObject[3];

    UIStageInfo m_stageUI;

    bool m_bInFTUE;                                 //是否在FTUE中

    public override void OnCreate()
    {
        base.OnCreate();

        m_newStageNumber = -1;

        m_heartUI = UIWindowManager.Singleton.CreateWindow<UIWindow>("UIMapHeart", UIWindowManager.Anchor.TopLeft);

        m_backGroundTrans = mUIObject.transform;
        
        m_stageBtns = new Transform[GlobalVars.TotalStageCount];

        GlobalVars.AvailabeStageCount = PlayerPrefs.GetInt("StageAvailableCount");
        GlobalVars.HeadStagePos = PlayerPrefs.GetInt("HeadStagePos");

        if (GlobalVars.AvailabeStageCount == 0)
        {
            GlobalVars.AvailabeStageCount = 1;
        }
        if (GlobalVars.HeadStagePos == 0)
        {
            GlobalVars.HeadStagePos = 1;
        }
		GlobalVars.StageStarArray = PlayerPrefsExtend.GetIntArray("StageStars", 0, 100);
        GlobalVars.StageScoreArray = PlayerPrefsExtend.GetIntArray("StageScores", 0, 100);
		GlobalVars.StageFailedArray = PlayerPrefsExtend.GetIntArray("StageFailed", 0, 100);
        GlobalVars.LastStage = GlobalVars.AvailabeStageCount;
		
		if(!PlayerPrefs.HasKey("Coins"))
		{
			GlobalVars.Coins = 10;
			PlayerPrefs.SetInt("Coins", 0);
		}
		else
		{
			GlobalVars.Coins = PlayerPrefs.GetInt("Coins");
		}
		
		GlobalVars.PurchasedItemArray = PlayerPrefsExtend.GetIntArray("PurchasedItemArray", 0, 2);

        m_mapObj = mUIObject.transform.FindChild("MapObj").gameObject;
        springPanel = m_mapObj.AddComponent<SpringPanel>();
        UIPanel panel = m_mapObj.GetComponent<UIPanel>();
        //panel.baseClipRegion = new Vector4(0, 0, CapsApplication.Singleton.Width, CapsApplication.Singleton.Height);

        //心面板
        m_timeNumber = UIToolkits.FindChild(m_heartUI.mUIObject.transform, "TimeNumber").gameObject;
        m_fullText = UIToolkits.FindChild(m_heartUI.mUIObject.transform, "HeartFull").gameObject;
        m_minNumber = m_heartUI.GetChildComponent<NumberDrawer>("MinNumber");
        m_secNumber = m_heartUI.GetChildComponent<NumberDrawer>("SecNumber");
        UIButton heartBtn = m_heartUI.GetChildComponent<UIButton>("HeartBtn");
        EventDelegate.Set(heartBtn.onClick, delegate()
        {
            if (GlobalVars.HeartCount == 5)
            {
                return;
            }
            UINoMoreHearts noMoreHeartUI = UIWindowManager.Singleton.GetUIWindow<UINoMoreHearts>();
            UIStageInfo stageInfoUI = UIWindowManager.Singleton.GetUIWindow<UIStageInfo>();

            if (stageInfoUI.Visible)
            {
                stageInfoUI.HideWindow();
            }

            noMoreHeartUI.NeedOpenStageInfoAfterClose = false;
            noMoreHeartUI.ShowWindow();
        });

        //金币面板
        m_coinNumber = m_heartUI.GetChildComponent<NumberDrawer>("MoneyNumber");
        UIButton button = m_heartUI.GetChildComponent<UIButton>("StoreBtn");
        EventDelegate.Set(button.onClick, delegate()
        {
            UIStore storeUI = UIWindowManager.Singleton.GetUIWindow<UIStore>();
            UIStageInfo stageInfoUI = UIWindowManager.Singleton.GetUIWindow<UIStageInfo>();

            if (stageInfoUI.Visible)
            {
                stageInfoUI.HideWindow();
                storeUI.OnCancelFunc = delegate()
                {
                    stageInfoUI.ShowWindow();
                };
                storeUI.OnPurchaseFunc = delegate()
                {
                    stageInfoUI.ShowWindow();
                };
            }

            
            storeUI.ShowWindow();
        }
        );

        m_headSprite = GetChildComponent<UISprite>("Head");


        GameObject obj = GameObject.Find("Cloud");
        m_cloudSprite = obj.GetComponent<UISprite>();
        m_cloudSprite.gameObject.SetActive(false);

        obj = GameObject.Find("Cloud2");
        m_cloud2Sprite = obj.GetComponent<UISprite>();
        m_cloud2Sprite.gameObject.SetActive(false);

        m_inputBlocker = mUIObject.transform.Find("Blocker").gameObject;

        m_stageUI = UIWindowManager.Singleton.GetUIWindow<UIStageInfo>();

        for (int i=0; i<3; ++i)
        {
            m_blurSprites[i] = UIToolkits.FindChild( mUIObject.transform, "MapPicBlur" + i).gameObject;
            m_blurSprites[i].SetActive(false);
        }
    }

    public void OpenNewButton(int stageNum)
    {
        m_newStageNumber = stageNum;
        m_newStageMoveTime = HeadMoveTime;
        if (m_newStageNumber == 2)
        {
            GlobalVars.InMapFTUE = true;
            m_heartUI.HideWindow();
            //进入FTUE状态
            UIFTUE ftue = UIWindowManager.Singleton.GetUIWindow<UIFTUE>();
            if (ftue == null)
            {
                ftue = UIWindowManager.Singleton.CreateWindow<UIFTUE>();
            }

            ftue.ResetFTUEStep();

            System.Collections.Generic.List<FTUEData> ftueData = new System.Collections.Generic.List<FTUEData>();
            FTUEData data = new FTUEData();
            data.dialog = Localization.instance.Get("FTUEStep0");
            data.headImage = "Dog";
            data.from = new Position();
            data.from.MakeItUnAvailable();
            ftueData.Add(data);             //第1句话
            data = new FTUEData();
            data.dialog = Localization.instance.Get("FTUEStep1");
            data.headImage = "Dog";
            data.pointToGameObject = "Stage1";
            data.from = new Position();
            data.from.MakeItUnAvailable();
            ftueData.Add(data);             //第2句话
            data = new FTUEData();
            data.dialog = Localization.instance.Get("FTUEStep2");
            data.headImage = "Dog";
            data.from = new Position();
            data.from.MakeItUnAvailable();
            ftueData.Add(data);             //第3句话

            ftue.ShowFTUE(0, ftueData);
        }
    }

    public void RefreshButton(int stageNum)
    {
        m_stageBtns[stageNum - 1].gameObject.SetActive(true);                                                    //显示对象

        for (int j = 1; j <= 3; ++j)
        {
            if (GlobalVars.StageStarArray[stageNum - 1] >= j)       //若得到了星星
            {
                Transform starTrans = UIToolkits.FindChild(m_stageBtns[stageNum - 1], "Star" + j);
                if (starTrans)
                {
                    starTrans.gameObject.SetActive(true);
                }
            }
            else
            {
                Transform starTrans = UIToolkits.FindChild(m_stageBtns[stageNum - 1], "Star" + j);
                if (starTrans)
                {
                    starTrans.gameObject.SetActive(false);
                }
            }
        }
    }

    public void RefreshButtons()
    {
        for (int i = 0; i < GlobalVars.TotalStageCount; ++i)
        {
            Transform transform = UIToolkits.FindChild(mUIObject.transform, "Stage" + (i + 1));      //找到对象

            if (transform == null)
            {
                Debug.LogError("There's no " + "Stage" + (i + 1).ToString() + " Button");
                continue;
            }
			
			m_stageBtns[i] = transform;

            if (!GlobalVars.DeveloperMode && i >= GlobalVars.HeadStagePos)     //隐藏超出范围的按钮
            {
                transform.gameObject.SetActive(false);
            }
			else
			{
				transform.gameObject.SetActive(true);                                                    //显示对象	
			}

            for (int j = 1; j <= 3; ++j)
            {
                if (GlobalVars.StageStarArray[i] >= j)       //若得到了星星
                {
                    Transform starTrans = UIToolkits.FindChild(transform, "Star" + j);
                    if (starTrans)
                    {
                        starTrans.gameObject.SetActive(true);
                    }
                }
                else
                {
                    Transform starTrans = UIToolkits.FindChild(transform, "Star" + j);
                    if (starTrans)
                    {
                        starTrans.gameObject.SetActive(false);
                    }
                }
            }

            UISprite sprite = transform.FindChild("BtnBackground").GetComponent<UISprite>();
            sprite.spriteName = "StageType" + CapsConfig.StageTypeArray[i];

            UIButton button = m_stageBtns[i].GetComponent<UIButton>();
            EventDelegate.Set(button.onClick, OnStageClicked);
        }

        m_headSprite.gameObject.transform.localPosition = new Vector3(m_stageBtns[GlobalVars.AvailabeStageCount - 1].localPosition.x, m_stageBtns[GlobalVars.AvailabeStageCount - 1].localPosition.y + HeadYOffset, m_stageBtns[GlobalVars.AvailabeStageCount - 1].localPosition.z);            //移到目标点
    }

    public override void OnShow()
    {
        base.OnShow();
        m_heartUI.ShowWindow();
        m_coinNumber.SetNumber((int)Unibiller.GetCurrencyBalance("gold"));
        UIWindowManager.Singleton.GetUIWindow<UIMainMenu>().ShowWindow();

        m_cloud2Sprite.gameObject.SetActive(true);
        m_cloudSprite.gameObject.SetActive(true);
		m_lastClickStageTime = Timer.GetRealTimeSinceStartUp();     //更新关卡点击时间
        m_inputBlocker.SetActive(false);
    }

    public override void OnShowEffectPlayOver()
    {
		base.OnShowEffectPlayOver();
        if (GlobalVars.HeadStagePos < GlobalVars.AvailabeStageCount)        //若有新开的关卡，先用头像去开启关卡
        {
            OpenNewButton(GlobalVars.AvailabeStageCount);
        }
		Transform curStageTrans = UIToolkits.FindChild(mUIObject.transform, "Stage" + GlobalVars.LastStage);      //找到对象
        MoveTo(new Vector2(curStageTrans.localPosition.x, curStageTrans.localPosition.y));
    }

    public override void OnHide()
    {
        base.OnHide();
        m_cloud2Sprite.gameObject.SetActive(false);
        m_cloudSprite.gameObject.SetActive(false);
        SetStageHelp(false);
        m_heartUI.HideWindow();
        m_newStageNumber = -1;          //停止头像移动...
    }

    public override void OnHideEffectPlayOver()
    {
        base.OnHideEffectPlayOver();
        for (int i = 0; i < 3; ++i)
        {
            m_blurSprites[i].SetActive(false);
        }
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        if (uiWindowState != UIWindowStateEnum.Show)
        {
            return;
        }

        GlobalVars.RefreshHeart();

        //显示心数和时间////////////////////////////////////////////////////////////////////////
        if (!m_heartUI.Visible)
        {
            if (!GlobalVars.InMapFTUE)
            {
                m_heartUI.ShowWindow();
            }
        }
        else
        {
            UISprite heartNum = m_heartUI.GetChildComponent<UISprite>("HeartNum");
            heartNum.spriteName = "Large_" + GlobalVars.HeartCount;
            m_coinNumber.SetNumber((int)(Unibiller.GetCurrencyBalance("gold") - m_stageUI.GetCurCost()));

            if (GlobalVars.HeartCount < 5)               //若心没满，要显示时间
            {
                if (!m_timeNumber.activeSelf)
                {
                    m_timeNumber.SetActive(true);
                }
                if (m_fullText.activeSelf)
                {
                    m_fullText.SetActive(false);
                }

                int ticks = (int)((System.DateTime.Now.Ticks - GlobalVars.GetHeartTime.Ticks) / 10000);
                int ticksToGetHeart = CapsConfig.Instance.GetHeartInterval * 1000 - ticks;
                int min = ticksToGetHeart / 1000 / 60;
                int second = ticksToGetHeart / 1000 % 60;
                m_minNumber.SetNumber(min);
                m_secNumber.SetNumber(second);
            }
            else
            {
                if (m_timeNumber.activeSelf)
                {
                    m_timeNumber.SetActive(false);
                }
                if (!m_fullText.activeSelf)
                {
                    m_fullText.SetActive(true);
                }
            }
        }

        if (GlobalVars.InMapFTUE)
        {
            return;
        }

        //处理头像在地图上移动
        if (m_newStageNumber > -1)          //若正在移动
        {
            m_newStageMoveTime -= Time.deltaTime;
            if (m_newStageMoveTime < 0)     //若移动到了
            {
                RefreshButton(m_newStageNumber);
                AddStagePartile(m_newStageNumber);
                m_headSprite.gameObject.transform.localPosition = new Vector3(m_stageBtns[m_newStageNumber - 1].localPosition.x, m_stageBtns[m_newStageNumber - 1].localPosition.y + HeadYOffset, m_stageBtns[m_newStageNumber - 1].localPosition.z);            ////移到目标点     
                GlobalVars.HeadStagePos = GlobalVars.AvailabeStageCount;        //记录头像移动
                PlayerPrefs.SetInt("HeadStagePos", GlobalVars.HeadStagePos);    //记录

                if (m_newStageNumber == 2)                                      //需要出FTUE的情况
                {
                    GlobalVars.InMapFTUE = true;
                    m_heartUI.HideWindow();
                    //进入FTUE状态
                    UIFTUE ftue = UIWindowManager.Singleton.GetUIWindow<UIFTUE>();
                    if (ftue == null)
                    {
                        ftue = UIWindowManager.Singleton.CreateWindow<UIFTUE>();
                    }

                    ftue.ResetFTUEStep();
                    System.Collections.Generic.List<FTUEData> ftueData = new System.Collections.Generic.List<FTUEData>();
                    FTUEData data = new FTUEData();
                    data.dialog = Localization.instance.Get("FTUEStep3");
                    data.headImage = "Dog";
                    data.pointToGameObject = "Stage2";
                    data.from = new Position();
                    data.from.MakeItUnAvailable();
                    ftueData.Add(data);             //第4句话
                    data = new FTUEData();

                    ftue.ShowFTUE(0, ftueData);
                }
                else                                                            //不需要出FTUE，自动点关卡按钮
                {
                    int tempNum = m_newStageNumber;
                    Timer.AddDelayFunc(1.0f, delegate()
                    {
                        UIButton.current = m_stageBtns[tempNum - 1].GetComponent<UIButton>();
                        OnStageClicked();
                    });
                }

                m_newStageNumber = -1;

                m_inputBlocker.SetActive(false);
            }
            else
            {
                if (!m_inputBlocker.activeInHierarchy)
                {
                    m_inputBlocker.SetActive(true);
                }
                Vector3 target = new Vector3(m_stageBtns[m_newStageNumber - 1].localPosition.x, m_stageBtns[m_newStageNumber - 1].localPosition.y + HeadYOffset, m_stageBtns[m_newStageNumber - 1].localPosition.z);
                m_headSprite.gameObject.transform.localPosition = Vector3.Lerp(m_stageBtns[m_newStageNumber - 2].localPosition, target, (1 - m_newStageMoveTime / HeadMoveTime));
            }
        }
        //5105 - 2045 = 3060
        //处理云的移动
        //float y = (mUIObject.transform.localPosition.y - (454.0f)) * 3054 / 3964.0f;
        //m_cloudSprite.transform.LocalPositionY(-y);

        m_cloudSprite.LocalPositionY((m_mapObj.transform.localPosition.y * 2) % 1135);
		
		if(m_cloudSprite.transform.localPosition.y > 0)
		{
            m_cloud2Sprite.transform.LocalPositionY(m_cloudSprite.transform.localPosition.y - 1135);
		}
		else
		{
            m_cloud2Sprite.transform.LocalPositionY(m_cloudSprite.transform.localPosition.y + 1135);
		}

        if (m_stageUI.Visible)
        {
            m_lastClickStageTime = Timer.GetRealTimeSinceStartUp();     //更新关卡点击时间
        }
        else if (Timer.GetRealTimeSinceStartUp() - m_lastClickStageTime > 5.0f)      //5秒没有有效操作
        {
            SetStageHelp(true);
        }

        if (m_stageUI.Visible)      //若关卡UI显示了
        {
            if (!m_blurSprites[0].activeInHierarchy)        //若模糊层没显示
            {
                for (int i = 0; i < 3;++i )
                {
                    m_blurSprites[i].SetActive(true);
                    m_blurSprites[i].GetComponent<TweenAlpha>().Play(true);
                }
                EventDelegate.Set(m_blurSprites[0].GetComponent<TweenAlpha>().onFinished, null);
            }
        }
        else
        {
            if (m_blurSprites[0].activeInHierarchy)
            {
                for (int i = 0; i < 3; ++i)
                {
                    m_blurSprites[i].GetComponent<TweenAlpha>().Play(false);
                }
                EventDelegate.Set(m_blurSprites[0].GetComponent<TweenAlpha>().onFinished, delegate()
                {
                    for (int i = 0; i < 3; ++i)
                    {
                        m_blurSprites[i].SetActive(false);
                    }
                });
            }
        }
    }

    void SetStageHelp(bool vis)
    {
        if (m_helpParticle == null)
        {
			Object obj = Resources.Load("LevelHelpEffect");
	        m_helpParticle = GameObject.Instantiate(obj) as GameObject;	
			m_helpParticle.SetActive(false);
        }

		if(m_helpParticle.activeSelf == vis) return;
		
        if (vis)
        {
            m_helpParticle.SetActive(true);
            m_helpParticle.transform.parent = m_stageBtns[GlobalVars.AvailabeStageCount - 1];
            m_helpParticle.transform.localPosition = Vector3.zero;
            m_helpParticle.transform.localScale = new Vector3(580.0f, 580.0f, 200.0f);                 //指定位置
            m_helpParticle.GetComponent<ParticleSystem>().Play(true);
        }
        else
        {
            m_helpParticle.GetComponent<ParticleSystem>().Stop();
            m_helpParticle.SetActive(false);
        }
    }

    void AddStagePartile(int stageNumber)
    {
        //Todo 临时加的粒子代码
        Object obj = Resources.Load("LevelOpenedEffect");
        GameObject gameObj= GameObject.Instantiate(obj) as GameObject;

        gameObj.transform.parent = m_stageBtns[stageNumber - 1];
        gameObj.transform.localPosition = Vector3.zero;
        gameObj.transform.localScale = new Vector3(580.0f, 580.0f, 200.0f);                 //指定位置
		gameObj.GetComponent<ParticleSystem>().Play(true);
    }

    private void OnStageClicked()
    {
        m_lastClickStageTime = Timer.GetRealTimeSinceStartUp();     //更新关卡点击时间
        SetStageHelp(false);

        NGUITools.PlaySound(CapsConfig.CurAudioList.ButtonClip);
        string stageNum = UIButton.current.name.Substring(5);
        GlobalVars.CurStageNum = System.Convert.ToInt32(stageNum);
        GlobalVars.CurStageData = StageData.CreateStageData();
        GlobalVars.LastStage = GlobalVars.CurStageNum;

        if (GlobalVars.HeartCount == 0)         //若没有心了
        {
            UIWindowManager.Singleton.GetUIWindow<UINoMoreHearts>().NeedOpenStageInfoAfterClose = true;
            UIWindowManager.Singleton.GetUIWindow<UINoMoreHearts>().ShowWindow();
            return;
        }

        GlobalVars.CurStageData.LoadStageData(GlobalVars.CurStageNum);
        UIWindowManager.Singleton.GetUIWindow<UIStageInfo>().ShowWindow();
    }

    public void MoveTo(Vector2 pos) //移动到某个位置
    {
        int bottom = (2046 - CapsApplication.Singleton.Height) / 2;
        if (-pos.y > bottom)
        {
            pos.y = -bottom;
        }
        springPanel.target = new Vector3(0, -pos.y, 0);
		springPanel.enabled = true;
    }
}
