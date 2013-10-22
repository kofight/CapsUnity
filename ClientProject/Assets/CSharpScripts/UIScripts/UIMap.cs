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


    }
    public override void OnShow()
    {
        base.OnShow();
		
		for (int i = 0; i < GlobalVars.TotalStageCount; ++i)
        {
            Transform transform = UIToolkits.FindChild(mUIObject.transform, "Stage" + (i + 1));      //找到对象
            if (!GlobalVars.DeveloperMode && i >= GlobalVars.AvailabeStageCount)     //隐藏超出范围的按钮
            {
                transform.gameObject.SetActive(false);
                continue;
            }
            
            transform.gameObject.SetActive(true);                                                    //显示对象
            if (transform == null)
            {
                Debug.LogError("There's no " + "Stage" + (i + 1).ToString() + " Button");
                continue;
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
    }

    private void OnStageClicked(object sender, UIMouseClick.ClickArgs e)
    {
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
