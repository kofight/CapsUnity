using UnityEngine;
using System.Collections;

public class UIMap : UIWindowNGUI 
{
    static readonly int Width = 2048;
    static readonly int Height = 2048;
    static readonly int Border = 100;           //地图四周留的边

    Transform m_backGroundTrans;                //背景的Transfrom
    private Vector2 m_curOffSet;
    private Vector2 m_destOffSet;

    Transform[] m_stageBtns;

    public override void OnCreate()
    {
        base.OnCreate();
        m_backGroundTrans = mUIObject.transform;
        
        m_stageBtns = new Transform[GlobalVars.TotalStageCount];

        GlobalVars.AvailabeStageCount = PlayerPrefs.GetInt("StageAvailableCount");
        if (GlobalVars.AvailabeStageCount == 0)
        {
            GlobalVars.AvailabeStageCount = 1;
        }
		GlobalVars.StageStarArray = PlayerPrefsExtend.GetIntArray("StageStars", 0, 100);
    }
    public override void OnShow()
    {
        base.OnShow();
		
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

            UIMouseClick click = transform.gameObject.GetComponent<UIMouseClick>();
            if (click != null)
            {
                GameObject.DestroyImmediate(click);
            }
            click = transform.gameObject.AddComponent<UIMouseClick>();
            click.Click += OnStageClicked;
            click.UserState = i+1;                                          //存储关卡数
        }
		
        MoveTo(new Vector2(0, 0));
    }
    public override void OnUpdate()
    {
        base.OnUpdate();
		if(GlobalVars.HeartCount > 5) GlobalVars.HeartCount = 5;
        UIDrawer.Singleton.DrawNumber("Heart", 10, 10, GlobalVars.HeartCount, "BaseNum", 20);

        if (GlobalVars.HeartCount < 5)          //若心没有满，处理心数量变化
        {
            long ticks = System.DateTime.Now.Ticks - GlobalVars.GetHeartTime.Ticks;
            int GetHeartCount = 0;
            if (ticks > (long)CapsConfig.Instance.GetHeartInterval * 10000000)        //若已经到了得心时间
            {
                GetHeartCount = (int)(ticks / (CapsConfig.Instance.GetHeartInterval * 10000000));
                GlobalVars.HeartCount += GetHeartCount;                     //增加心数
                GlobalVars.GetHeartTime = GlobalVars.GetHeartTime.AddSeconds(GetHeartCount * CapsConfig.Instance.GetHeartInterval);          //更改获取心的时间记录
            }

            if (GlobalVars.HeartCount > 5)
            {
                GlobalVars.HeartCount = 5;
            }
        }
		
		if(GlobalVars.HeartCount < 5)               //若心没满，要显示时间
		{
			long ticks = System.DateTime.Now.Ticks - GlobalVars.GetHeartTime.Ticks;
			long ticksToGetHeart = ((long)CapsConfig.Instance.GetHeartInterval * 10000000 - ticks);
			int min = (int)(ticksToGetHeart / 10000000) / 60;
			int second = (int)(ticksToGetHeart / 10000000) % 60;
            UIDrawer.Singleton.DrawNumber("MinutesToHeart", 80, 10, min, "HighDown", 14);

            UIDrawer.Singleton.DrawNumber("SecondsToHeart", 120, 10, second, "HighDown", 14);
		}
    }

    private void OnStageClicked(object sender, UIMouseClick.ClickArgs e)
    {
        if (GlobalVars.HeartCount == 0)
        {
            return;
        }
        GlobalVars.CurStageNum = (int)e.UserState;
        GlobalVars.CurStageData = StageData.CreateStageData();
        GlobalVars.CurStageData.LoadStageData(GlobalVars.CurStageNum);
        UIWindowManager.Singleton.GetUIWindow<UIStageInfo>().ShowWindow();
    }

    public void MoveTo(Vector2 pos) //移动到某个位置
    {
        m_destOffSet = new Vector2(-pos.x, -pos.y);
        //判断出界情况
        if (m_destOffSet.x > - (CapsApplication.Singleton.Width / 2 + Border))
        {
            m_destOffSet.x = -(CapsApplication.Singleton.Width / 2 + Border);
        }

        if (m_destOffSet.x < -(Width - Border - CapsApplication.Singleton.Width / 2))
        {
            m_destOffSet.x = -(Width - Border - CapsApplication.Singleton.Width / 2);
        }

        if (m_destOffSet.y > -(CapsApplication.Singleton.Height / 2 + Border))
        {
            m_destOffSet.y = -(CapsApplication.Singleton.Height / 2 + Border);
        }

        if (m_destOffSet.y < -(Height - Border - CapsApplication.Singleton.Height / 2))
        {
            m_destOffSet.y = -(Height - Border - CapsApplication.Singleton.Height / 2);
        }

        m_backGroundTrans.localPosition = new Vector3(m_destOffSet.x, m_destOffSet.y, m_backGroundTrans.position.z);
    }

    public void OnDragMove(Vector2 fingerPos, Vector2 delta)
    {
        MoveTo(new Vector2(-m_destOffSet.x - delta.x, -m_destOffSet.y - delta.y));
    }
}
