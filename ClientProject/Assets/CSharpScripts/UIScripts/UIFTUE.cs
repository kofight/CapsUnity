using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIFTUE : UIWindow
{
    AocTypewriterEffect m_dialogText;
    UISprite m_dialogBoardSprite;
	UISprite m_leftHeadSprite;
	UISprite m_rightHeadSprite;
    UISprite m_backPic;
    UISprite [] m_pic = new UISprite [9];             //配的图片

    Transform m_dialogTrans;

    UILabel m_clickLabel;

    public GameObject m_pointer;
    Transform m_gameAreaTrans;
    UISprite m_pointerSprite;
    long m_pointerStartTime;                         //开始播放箭头的时间

    UIEffectPlayer m_dialogEffectPlayer;
    WindowEffectFinished m_afterDialogFunc;
	
	string [] m_dialogContents;
	int m_curDialogIndex;
	
	bool m_bLock;

    int m_FTUEIndex = 0;
	int m_curStep = -1;
	int m_finishedStep = -1;

    List<FTUEData> m_ftueData;
	
	public override void OnShow()
	{
		base.OnShow();	
	}

	public void HideHighLight()
	{
		if(m_ftueData[m_FTUEIndex].from.IsAvailable())
		{
			GameLogic.Singleton.SetHighLight(false, m_ftueData[m_FTUEIndex].from);
		}
		
		foreach (Position highLightPos in m_ftueData[m_FTUEIndex].highLightPosList)
		{
            GameLogic.Singleton.SetHighLight(false, highLightPos, m_ftueData[m_FTUEIndex].bHighLightBackground, m_ftueData[m_FTUEIndex].bHighLightBlock);
		}
		
		m_pointer.SetActive(false);
	}

    public override void OnHide()
    {
        base.OnHide();

        if (!GlobalVars.InMapFTUE)
        {
            HideHighLight();
            GameLogic.Singleton.ShowUI();
        }
    }
	
	public void ResetFTUEStep()
	{
		m_curStep = -1;
		m_finishedStep = -1;
	}

    public bool ShowFTUE(int step, List<FTUEData> ftueData)
    {
		if(step <= m_finishedStep)
		{
			return false;
		}
		
		m_curStep = step;
        if (ftueData != null)
        {
            m_ftueData = ftueData;
        }

        //if (GameLogic.Singleton.PlayingStageData.FTUEMap.TryGetValue(step, out m_ftueData))
        {
            BoxCollider collider = m_dialogBoardSprite.GetComponent<BoxCollider>();

            ShowText(m_ftueData[m_FTUEIndex].headImage, m_ftueData[m_FTUEIndex].pic, m_ftueData[m_FTUEIndex].dialog, delegate()
            {
                bool bShowPointer = false;                  //是否显示了手指

                if (!GlobalVars.InMapFTUE)        //若游戏存在
                {
                    if (m_ftueData[m_FTUEIndex].from.IsAvailable())
                    {
                        m_bLock = true;
                        bShowPointer = true;
                        m_pointer.transform.parent = m_gameAreaTrans;
                        m_pointer.SetActive(true);
                        RefreshPointer();
                        m_pointer.GetComponent<TweenScale>().enabled = false;
                        m_pointerStartTime = Timer.millisecondNow();
                        GameLogic.Singleton.SetHighLight(true, m_ftueData[m_FTUEIndex].from);
                    }
                    foreach (Position highLightPos in m_ftueData[m_FTUEIndex].highLightPosList)
                    {
                        GameLogic.Singleton.SetHighLight(true, highLightPos, m_ftueData[m_FTUEIndex].bHighLightBackground, m_ftueData[m_FTUEIndex].bHighLightBlock);
                    }
                }

                if (m_ftueData[m_FTUEIndex].pointToGameObject != null)      //若有指向的物件
                {
                    GameObject obj = GameObject.Find(m_ftueData[m_FTUEIndex].pointToGameObject) as GameObject;      //先找到物件
                    bShowPointer = true;
                    m_pointer.transform.parent = obj.transform;                 //把手指绑在找到的物件上
                    m_pointer.transform.localPosition = new Vector3(0, 0, 0);       
                    m_pointer.transform.parent = m_gameAreaTrans;               //把父结点设回来
                    m_pointer.SetActive(true);
                    RefreshPointer();
                    m_pointer.GetComponent<TweenScale>().enabled = true;
                    m_pointerStartTime = Timer.millisecondNow();
                }

                if (!bShowPointer)
                {
                    m_pointer.SetActive(false);
                }
            });
        }

        if (!GlobalVars.InMapFTUE)
        {
            GameLogic.Singleton.HideUI();
        }
		
		return true;
    }

    public void PushFTUEData(FTUEData data)
    {
        m_ftueData.Add(data);
    }

    public bool CheckMoveTo(Position to)
    {
        if (to.x == m_ftueData[m_FTUEIndex].to.x && to.y == m_ftueData[m_FTUEIndex].to.y)
        {
            return true;
        }

        return false;
    }

    public bool CheckMoveFrom(Position from)
    {
        if (from.x == m_ftueData[m_FTUEIndex].from.x && from.y == m_ftueData[m_FTUEIndex].from.y)
        {
            return true;
        }

        return false;
    }

    void RefreshPointer()
    {
        if (m_pointer.activeSelf)        //若箭头可见
        {
            if (CapsApplication.Singleton.CurStateEnum == StateEnum.Game)
            {
                Vector2 fromXY = new Vector2(GameLogic.Singleton.GetXPos(m_ftueData[m_FTUEIndex].from.x), GameLogic.Singleton.GetYPos(m_ftueData[m_FTUEIndex].from.x, m_ftueData[m_FTUEIndex].from.y));
                Vector2 toXY = new Vector2(GameLogic.Singleton.GetXPos(m_ftueData[m_FTUEIndex].to.x), GameLogic.Singleton.GetYPos(m_ftueData[m_FTUEIndex].to.x, m_ftueData[m_FTUEIndex].to.y));

                long curLoopTime = (Timer.millisecondNow() - m_pointerStartTime) % 2500;     //当前循环的时间

                if (curLoopTime <= 300)                  //前0.3秒原地Alpha
                {
                    m_pointerSprite.alpha = curLoopTime / 300.0f;
                    m_pointer.transform.localPosition = new Vector3(fromXY.x, -fromXY.y);
                }
                else if (curLoopTime <= 500)            //停一下
                {

                }
                else if (curLoopTime <= 1500)           //移动
                {
                    Vector2 pos = Vector2.Lerp(fromXY, toXY, ((curLoopTime - 500) % 1000) / 1000.0f);
                    m_pointer.transform.localPosition = new Vector3(pos.x, -pos.y);
                }
                else if (curLoopTime <= 1700)
                {

                }
                else if (curLoopTime <= 2000)
                {
                    m_pointerSprite.alpha = 1.0f - (curLoopTime - 1700) / 300.0f;
                    m_pointer.transform.localPosition = new Vector3(toXY.x, -toXY.y);
                }
                else
                {
                    m_pointerSprite.alpha = 0.0f;
                    m_pointer.transform.localPosition = new Vector3(toXY.x, -toXY.y);
                }
            }
            else
            {

            }
        }
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        RefreshPointer();
    }

    public void ShowText(string head, string pic, string content, WindowEffectFinished func)
	{
		ShowWindow();

        if (pic != "None")
        {
            for (int i = 0; i < 9; ++i )
            {
                if (i != m_ftueData[m_FTUEIndex].picturePos - 1)
                {
                    m_pic[i].gameObject.SetActive(false);
                }
                else
                {
                    m_pic[i].gameObject.SetActive(true);
                    m_pic[i].spriteName = pic;
                }
            }
        }
        else
        {
            for (int i = 0; i < 9; ++i)
            {
                m_pic[i].gameObject.SetActive(false);
            }
        }

        if (m_ftueData[m_FTUEIndex].dialogPos == 0)     //左上
        {
            m_dialogBoardSprite.spriteName = "TextBox";
            m_dialogTrans.localPosition = new Vector3(8, 306, -1);
            m_dialogText.transform.localPosition = new Vector3(-177, 147, 1);
            m_clickLabel.transform.localPosition = new Vector3(178, -76, 1);
        }
        else if (m_ftueData[m_FTUEIndex].dialogPos == 1)    //左下
        {
            m_dialogBoardSprite.spriteName = "TextBox3";
            m_dialogTrans.localPosition = new Vector3(72, -317, -1);
            m_dialogText.transform.localPosition = new Vector3(-147, 135, 1);
            m_clickLabel.transform.localPosition = new Vector3(174, -133, 1);
        }
        else if (m_ftueData[m_FTUEIndex].dialogPos == 2)     //右上
        {
            m_dialogBoardSprite.spriteName = "TextBoxMir";
            m_dialogTrans.localPosition = new Vector3(8, 306, -1);
            m_dialogText.transform.localPosition = new Vector3(-177, 147, 1);
            m_clickLabel.transform.localPosition = new Vector3(178, -76, 1);
        }
		
		if(m_ftueData[m_FTUEIndex].dialogPos == 0 || m_ftueData[m_FTUEIndex].dialogPos == 1)
		{
			if(head != "None")
			{
	            m_leftHeadSprite.gameObject.SetActive(true);
	            m_leftHeadSprite.spriteName = head;
			}
			else
			{
	            m_leftHeadSprite.gameObject.SetActive(false);
			}
			m_rightHeadSprite.gameObject.SetActive(false);
		}
		else
		{
			if(head != "None")
			{
	            m_rightHeadSprite.gameObject.SetActive(true);
	            m_rightHeadSprite.spriteName = head;
			}
			else
			{
	            m_rightHeadSprite.gameObject.SetActive(false);
			}
			m_leftHeadSprite.gameObject.SetActive(false);
		}
		
		m_dialogContents = content.Split('@');
		m_curDialogIndex = 0;
		
		m_bLock = true;

        m_clickLabel.gameObject.SetActive(false);

        m_afterDialogFunc = func;

        //开始播放文字
        NextDialog();
	}
	
    public override void OnCreate()
    {
        base.OnCreate();
		m_dialogText = GetChildComponent<AocTypewriterEffect>("DialogText");
		m_leftHeadSprite = GetChildComponent<UISprite>("LeftHead");
		m_rightHeadSprite = GetChildComponent<UISprite>("RightHead");
        m_dialogBoardSprite = GetChildComponent<UISprite>("DialogBoard");
        m_dialogEffectPlayer = m_dialogBoardSprite.GetComponent<UIEffectPlayer>();
        m_pointer = GameObject.Find("FTUEPointer");
        m_pointerSprite = m_pointer.GetComponent<UISprite>();
        m_pointer.SetActive(false);
        m_gameAreaTrans = GameObject.Find("GameArea").transform;
        m_clickLabel = GetChildComponent<UILabel>("ClickLabel");

        m_dialogTrans = mUIObject.transform.FindChild("DialogBoard");

		AddChildComponentMouseClick("DialogBoard", OnClick);

        for (int i=0; i<9; ++i)
        {
            m_pic[i] = GetChildComponent<UISprite>("Picture" + (i+1).ToString());
        }
    }

    public void EndFTUE()
    {
        Debug.Log("EndFTUE");
		m_finishedStep = m_curStep;
		m_FTUEIndex = 0;
        m_pointer.SetActive(false);
        HideWindow(delegate()
		{
			if (!GlobalVars.InMapFTUE)
	        {
	            GameLogic.Singleton.SetGameFlow(TGameFlow.EGameState_Playing);
	        }
	        if (GlobalVars.InMapFTUE)
	        {
	            GlobalVars.InMapFTUE = false;
	        }
		});                               //隐藏窗体
    }

    public void NextDialog()
    {
        m_bLock = true;         //先锁上点击
        BoxCollider collider = m_dialogBoardSprite.GetComponent<BoxCollider>();
        collider.size = new Vector3(2048, 2048, 1);

            m_dialogEffectPlayer.ShowEffect();          //打开新窗口
            m_clickLabel.gameObject.SetActive(false);   //关闭操作提示

            m_dialogText.Play(m_dialogContents[m_curDialogIndex], delegate()            //播放下一段文字
            {
                m_bLock = false;                                                        //播放完毕后解锁
                m_clickLabel.gameObject.SetActive(true);                                //显示操作提示
			
				
			
                if (m_curDialogIndex < m_dialogContents.Length - 1)
                {
                    m_clickLabel.text = Localization.instance.Get("Click");           //把索引指向下一段文字
                }
                else                                                                    //若已经播到了最后一条
                {
                    if (m_ftueData[m_FTUEIndex].from.IsAvailable())     //看看是否需要操作，需要的话显示划动提示
                    {
                        m_clickLabel.text = Localization.instance.Get("MoveBlock");
                        collider.size = new Vector3(300, 200, 1);
                        m_pointer.SetActive(true);
                        m_pointer.GetComponent<TweenScale>().enabled = false;
                        m_pointerStartTime = Timer.millisecondNow();
                    }
                    else                                                //不需要操作的话显示点击提示
                    {
                        m_clickLabel.text = Localization.instance.Get("Click");
                        m_pointer.SetActive(false);
                    }
                    if (m_afterDialogFunc != null)
                    {
                        m_afterDialogFunc();
                    }
                }
                ++m_curDialogIndex;
            });
    }
	
	public void OnClick()
	{
		if(m_bLock)
			return;

        if (m_curDialogIndex < m_dialogContents.Length)      //若不是最后一段文字
        {
            m_bLock = true;
            m_dialogEffectPlayer.HideEffect(delegate()      //关闭当前窗口
            {
                NextDialog();
            });
        }
        else        //若是最后一段文字,且可以通过点击结束，则进到这里
        {
            if (m_FTUEIndex < m_ftueData.Count - 1)         //检测FTUE有多条的情况
            {
                if (!GlobalVars.InMapFTUE)
                {
                    HideHighLight();
                }
				
				++m_FTUEIndex;
                m_bLock = true;
				
                m_dialogEffectPlayer.HideEffect(delegate()      //关闭当前窗口
                {
                     ShowFTUE(m_curStep, null);                         //若有步数，循环调用
                });
            }
            else                                            //若没有
            {
				EndFTUE();                                  //结束当前这个FTUE
            }
        }
	}
}
