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

    Transform[] m_stageBtns;

    UIWindow m_heartUI;

    GameObject m_timeNumber;
    NumberDrawer m_minNumber;
    NumberDrawer m_secNumber;
    NumberDrawer m_coinNumber;
    GameObject m_fullText;

    int m_newStageNumber;                       //开启的新关卡的编号
    float m_newStageStartTime;                  //开启新关卡的时间
    readonly static float HeadMoveTime = 3.0f;  //开启新关卡时头像的移动时间
    readonly static float HeadYOffset = 110.0f; //头像相对于按钮位置的位移

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
		
		springPanel = mUIObject.AddComponent<SpringPanel>();
        UIPanel panel = mUIObject.GetComponent<UIPanel>();
        //panel.baseClipRegion = new Vector4(0, 0, CapsApplication.Singleton.Width, CapsApplication.Singleton.Height);

        m_timeNumber = UIToolkits.FindChild(m_heartUI.mUIObject.transform, "TimeNumber").gameObject;
        m_fullText = UIToolkits.FindChild(m_heartUI.mUIObject.transform, "HeartFull").gameObject;
        m_minNumber = m_heartUI.GetChildComponent<NumberDrawer>("MinNumber");
        m_secNumber = m_heartUI.GetChildComponent<NumberDrawer>("SecNumber");

        //金币面板
        m_coinNumber = m_heartUI.GetChildComponent<NumberDrawer>("MoneyNumber");
        UIButton button = m_heartUI.GetChildComponent<UIButton>("StoreBtn");
        EventDelegate.Set(button.onClick, delegate()
        {
            UIStore storeUI = UIWindowManager.Singleton.GetUIWindow<UIStore>();
            storeUI.ShowWindow();
        }
        );

        m_headSprite = GetChildComponent<UISprite>("Head");
        m_cloudSprite = GetChildComponent<UISprite>("Cloud");
    }

    public void OpenNewButton(int stageNum)
    {
        m_newStageNumber = stageNum;
        m_newStageStartTime = Time.realtimeSinceStartup;
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
        if (GlobalVars.HeadStagePos < GlobalVars.AvailabeStageCount)        //若有新开的关卡，先用头像去开启关卡
        {
            OpenNewButton(GlobalVars.AvailabeStageCount);
        }
    }

    public override void OnShowEffectPlayOver()
    {
		base.OnShowEffectPlayOver();
		Transform curStageTrans = UIToolkits.FindChild(mUIObject.transform, "Stage" + GlobalVars.LastStage);      //找到对象
        MoveTo(new Vector2(curStageTrans.localPosition.x, curStageTrans.localPosition.y));
    }

    public override void OnHide()
    {
        base.OnHide();
        m_heartUI.HideWindow();
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        GlobalVars.RefreshHeart();

        //显示心数和时间////////////////////////////////////////////////////////////////////////
        UISprite heartNum = m_heartUI.GetChildComponent<UISprite>("HeartNum");
        heartNum.spriteName = "Large_" + GlobalVars.HeartCount;
		
		if(GlobalVars.HeartCount < 5)               //若心没满，要显示时间
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

        //处理头像在地图上移动
        if (m_newStageNumber > -1)          //若正在移动
        {
            float passTime = Time.realtimeSinceStartup - m_newStageStartTime;
            if (passTime >= HeadMoveTime)     //若移动到了
            {
                RefreshButton(m_newStageNumber);
                AddStagePartile(m_newStageNumber);
                m_headSprite.gameObject.transform.localPosition = new Vector3(m_stageBtns[m_newStageNumber - 1].localPosition.x, m_stageBtns[m_newStageNumber - 1].localPosition.y + HeadYOffset, m_stageBtns[m_newStageNumber - 1].localPosition.z);            ////移到目标点     
                GlobalVars.HeadStagePos = GlobalVars.AvailabeStageCount;        //记录头像移动
                PlayerPrefs.SetInt("HeadStagePos", GlobalVars.HeadStagePos);    //记录

                int tempNum = m_newStageNumber;
                Timer.AddDelayFunc(1.0f, delegate()
                {
                    UIButton.current = m_stageBtns[tempNum - 1].GetComponent<UIButton>();
                    OnStageClicked();
                });

                m_newStageNumber = -1;
            }
            else
            {
                Vector3 target = new Vector3(m_stageBtns[m_newStageNumber - 1].localPosition.x, m_stageBtns[m_newStageNumber - 1].localPosition.y + HeadYOffset, m_stageBtns[m_newStageNumber - 1].localPosition.z);
                m_headSprite.gameObject.transform.localPosition = Vector3.Lerp(m_stageBtns[m_newStageNumber - 2].localPosition, target, passTime / HeadMoveTime);
            }
        }
        //5105 - 2045 = 3060
        //处理云的移动
        float y = (mUIObject.transform.localPosition.y - (454.0f)) * 3054 / 3964.0f;
        m_cloudSprite.transform.LocalPositionY(-y);
    }

    void AddStagePartile(int stageNumber)
    {
        //Todo 临时加的粒子代码
        Object obj = Resources.Load("LevelOpenedEffect");
        GameObject gameObj= GameObject.Instantiate(obj) as GameObject;

        gameObj.transform.parent = m_stageBtns[stageNumber - 1];
        gameObj.transform.localPosition = Vector3.zero;
        gameObj.transform.localScale = new Vector3(580.0f, 580.0f, 200.0f);                 //指定位置
    }

    private void OnStageClicked()
    {
        NGUITools.PlaySound(CapsConfig.CurAudioList.ButtonClip);
        string stageNum = UIButton.current.name.Substring(5);
        GlobalVars.CurStageNum = System.Convert.ToInt32(stageNum);
        GlobalVars.CurStageData = StageData.CreateStageData();
        GlobalVars.LastStage = GlobalVars.CurStageNum;

        if (GlobalVars.HeartCount == 0)         //若没有心了
        {
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
