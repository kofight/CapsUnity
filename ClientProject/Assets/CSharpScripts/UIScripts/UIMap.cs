using UnityEngine;
using System.Collections;

public class UIMap : UIWindow 
{
    static readonly int Width = 2048;
    static readonly int Height = 2048;
    static readonly int Border = 100;           //地图四周留的边

    Transform m_backGroundTrans;                //背景的Transfrom
    private Vector2 m_curOffSet;
    private Vector2 m_destOffSet;
	
	SpringPanel springPanel;

    Transform[] m_stageBtns;

    UIWindow m_heartUI;

    public override void OnCreate()
    {
        base.OnCreate();

        m_heartUI = UIWindowManager.Singleton.CreateWindow<UIWindow>("UIMapHeart", UIWindowManager.Anchor.TopLeft);

        m_backGroundTrans = mUIObject.transform;
        
        m_stageBtns = new Transform[GlobalVars.TotalStageCount];

        GlobalVars.AvailabeStageCount = PlayerPrefs.GetInt("StageAvailableCount");
        if (GlobalVars.AvailabeStageCount == 0)
        {
            GlobalVars.AvailabeStageCount = 1;
        }
		GlobalVars.StageStarArray = PlayerPrefsExtend.GetIntArray("StageStars", 0, 100);
        GlobalVars.LastStage = GlobalVars.AvailabeStageCount;
		
		springPanel = mUIObject.AddComponent<SpringPanel>();
        UIPanel panel = mUIObject.GetComponent<UIPanel>();
        panel.clipRange = new Vector4(0, 0, CapsApplication.Singleton.Width, CapsApplication.Singleton.Height);
    }
    public override void OnShow()
    {
        base.OnShow();

        m_heartUI.ShowWindow();

		for (int i = 0; i < GlobalVars.TotalStageCount; ++i)
        {
            Transform transform = UIToolkits.FindChild(mUIObject.transform, "Stage" + (i + 1));      //找到对象

            if (transform == null)
            {
                Debug.LogError("There's no " + "Stage" + (i + 1).ToString() + " Button");
                continue;
            }

            if (!GlobalVars.DeveloperMode && i >= GlobalVars.AvailabeStageCount)     //隐藏超出范围的按钮
            {
                transform.gameObject.SetActive(false);
                continue;
            }
            
            transform.gameObject.SetActive(true);                                                    //显示对象

            for (int j=1; j<=3; ++j)
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

            m_stageBtns[i] = transform;

            UIButton button = m_stageBtns[i].GetComponent<UIButton>();
            EventDelegate.Set(button.onClick, OnStageClicked);
        }

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
		if(GlobalVars.HeartCount > 5) GlobalVars.HeartCount = 5;

        UISprite heartNum = m_heartUI.GetChildComponent<UISprite>("HeartNum");
        heartNum.spriteName = "Large_" + GlobalVars.HeartCount;

        if (GlobalVars.HeartCount < 5)          //若心没有满，处理心数量变化
        {
            int ticks = (int)((System.DateTime.Now.Ticks - GlobalVars.GetHeartTime.Ticks) / 10000);
            int GetHeartCount = 0;
            if (ticks > CapsConfig.Instance.GetHeartInterval * 1000)        //若已经到了得心时间
            {
                GetHeartCount = (ticks / (CapsConfig.Instance.GetHeartInterval * 1000));
                GlobalVars.HeartCount += (int)GetHeartCount;                     //增加心数
                GlobalVars.GetHeartTime = GlobalVars.GetHeartTime.AddSeconds(GetHeartCount * CapsConfig.Instance.GetHeartInterval);          //更改获取心的时间记录
            }

            if (GlobalVars.HeartCount > 5)
            {
                GlobalVars.HeartCount = 5;
            }
        }
		
		if(GlobalVars.HeartCount < 5)               //若心没满，要显示时间
		{
			int ticks = (int)((System.DateTime.Now.Ticks - GlobalVars.GetHeartTime.Ticks) / 10000);
			int ticksToGetHeart = CapsConfig.Instance.GetHeartInterval * 1000 - ticks;
			int min = ticksToGetHeart / 1000 / 60;
			int second = ticksToGetHeart / 1000 % 60;
            UIDrawer.Singleton.CurDepth = 20;
            UIDrawer.Singleton.DrawNumber("MinutesToHeart", 98, 53, min, "", 24);
            UIDrawer.Singleton.DrawSprite("Colon", 166, 53, "colon");
            UIDrawer.Singleton.DrawNumber("SecondsToHeart", 160, 53, second, "", 24);
            UIDrawer.Singleton.CurDepth = 0;
		}
        else
        {
            UIDrawer.Singleton.CurDepth = 20;
            UIDrawer.Singleton.DrawSprite("Full", 173, 53, "Full");
            UIDrawer.Singleton.CurDepth = 0;
        }
    }

    private void OnStageClicked()
    {
        if (GlobalVars.HeartCount == 0)
        {
            return;
        }
        string stageNum = UIButton.current.name.Substring(5);
        GlobalVars.CurStageNum = System.Convert.ToInt32(stageNum);
        GlobalVars.CurStageData = StageData.CreateStageData();
        GlobalVars.CurStageData.LoadStageData(GlobalVars.CurStageNum);
        UIWindowManager.Singleton.GetUIWindow<UIStageInfo>().ShowWindow();
        GlobalVars.LastStage = GlobalVars.CurStageNum;
    }

    public void MoveTo(Vector2 pos) //移动到某个位置
    {
        springPanel.target = new Vector3(-pos.x, -pos.y, 0);
    }
}
