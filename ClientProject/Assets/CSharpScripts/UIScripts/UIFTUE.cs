using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIFTUE : UIWindow
{
    AocTypewriterEffect m_dialogText;
    UISprite m_dialogBoardSprite;
	UISprite m_headSprite;
    UISprite m_backPic;
    UISprite m_pic;             //配的图片

    UILabel m_clickLabel;

    GameObject m_pointer;
    UIEffectPlayer m_dialogEffectPlayer;
    WindowEffectFinished m_afterDialogFunc;
	
	string [] m_dialogContents;
	int m_curDialogIndex;
	
	bool m_bLock;

    int m_FTUEIndex = 0;
	int m_curStep = 0;

    List<FTUEData> m_ftueData;
	
	public override void OnShow()
	{
		base.OnShow();	
	}

	void HideHighLight()
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
		
		HideHighLight ();

        GameLogic.Singleton.ShowUI();
    }
	
	public void ResetFTUEStep()
	{
		m_curStep = -1;
	}

    public bool ShowFTUE(int step)
    {
		if(step <= m_curStep)
		{
			return false;
		}
		
		m_curStep = step;

        if (GameLogic.Singleton.PlayingStageData.FTUEMap.TryGetValue(step, out m_ftueData))
        {
            BoxCollider collider = m_dialogBoardSprite.GetComponent<BoxCollider>();

            ShowText(m_ftueData[m_FTUEIndex].headImage, m_ftueData[m_FTUEIndex].pic, m_ftueData[m_FTUEIndex].dialog, delegate()
            {
                if (m_ftueData[m_FTUEIndex].from.IsAvailable())
                {
					m_bLock =  true;
					m_pointer.SetActive(true);
                    GameLogic.Singleton.SetHighLight(true, m_ftueData[m_FTUEIndex].from);
                }
                foreach (Position highLightPos in m_ftueData[m_FTUEIndex].highLightPosList)
                {
                    GameLogic.Singleton.SetHighLight(true, highLightPos, m_ftueData[m_FTUEIndex].bHighLightBackground, m_ftueData[m_FTUEIndex].bHighLightBlock);
                }
            });
        }

        GameLogic.Singleton.HideUI();
		
		return true;
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

    public override void OnUpdate()
    {
        base.OnUpdate();
        if (m_pointer.activeSelf)        //若箭头可见
        {
            Vector2 fromXY = new Vector2(GameLogic.Singleton.GetXPos(m_ftueData[m_FTUEIndex].from.x), GameLogic.Singleton.GetYPos(m_ftueData[m_FTUEIndex].from.x, m_ftueData[m_FTUEIndex].from.y));
            Vector2 toXY = new Vector2(GameLogic.Singleton.GetXPos(m_ftueData[m_FTUEIndex].to.x), GameLogic.Singleton.GetYPos(m_ftueData[m_FTUEIndex].to.x, m_ftueData[m_FTUEIndex].to.y));
            Vector2 pos = Vector2.Lerp(fromXY, toXY, (Timer.millisecondNow() % 1000) / 1000.0f);
            m_pointer.transform.localPosition = new Vector3(pos.x, -pos.y);
        }
    }

    public void ShowText(string head, string pic, string content, WindowEffectFinished func)
	{
		ShowWindow();
		if(head != "None")
		{
            m_headSprite.gameObject.SetActive(true);
            m_headSprite.spriteName = head;
		}
		else
		{
            m_headSprite.gameObject.SetActive(false);
		}

        if (pic != "None")
        {
            m_pic.gameObject.SetActive(true);
            m_pic.spriteName = pic;
        }
        else
        {
            m_pic.gameObject.SetActive(false);
        }
		
		m_dialogContents = content.Split('@');
		m_curDialogIndex = 0;
		
		m_bLock = true;

        m_clickLabel.gameObject.SetActive(false);

        //开始播放文字
        m_dialogText.Play(m_dialogContents[m_curDialogIndex], delegate()
        {
			m_bLock = false;
            //若是最后一段文字，调用结尾函数
            if (m_curDialogIndex == m_dialogContents.Length - 1)
            {
				if(func != null)
				{
                	func();
				}
            }

            m_clickLabel.gameObject.SetActive(true);
			BoxCollider collider = m_dialogBoardSprite.GetComponent<BoxCollider>();
            if (m_ftueData[m_FTUEIndex].from.IsAvailable() && m_curDialogIndex == m_dialogContents.Length - 1)
            {
                m_clickLabel.text = Localization.instance.Get("MoveBlock");
				collider.size = new Vector3(300, 200, 1);
            }
            else
            {
                m_clickLabel.text = Localization.instance.Get("Click");
				collider.size = new Vector3(2048, 2048, 1);
            }

			++m_curDialogIndex;
        });

        m_afterDialogFunc = func;
	}
	
    public override void OnCreate()
    {
        base.OnCreate();
		m_dialogText = GetChildComponent<AocTypewriterEffect>("DialogText");
		m_headSprite = GetChildComponent<UISprite>("Head");
        m_dialogBoardSprite = GetChildComponent<UISprite>("DialogBoard");
        m_dialogEffectPlayer = m_dialogBoardSprite.GetComponent<UIEffectPlayer>();
        m_pic = GetChildComponent<UISprite>("Picture");
        m_pointer = GameObject.Find("FTUEPointer");
        m_pointer.SetActive(false);
        m_clickLabel = GetChildComponent<UILabel>("ClickLabel");
		AddChildComponentMouseClick("DialogBoard", OnClick);
    }

    public void EndFTUE()
    {
		m_FTUEIndex = 0;
        HideWindow();                               //隐藏窗体
        GameLogic.Singleton.SetGameFlow(TGameFlow.EGameState_Playing);
    }
	
	public void OnClick()
	{
		if(m_bLock)
			return;

        if (m_curDialogIndex < m_dialogContents.Length)      //若不是最后一段文字
        {
			m_bLock = true;
            m_dialogEffectPlayer.HideEffect(delegate()
            {
                m_dialogEffectPlayer.ShowEffect();
                m_clickLabel.gameObject.SetActive(false);
                //播放下一段
                m_dialogText.Play(m_dialogContents[m_curDialogIndex], delegate()
                {
                    m_bLock = false;
                    ++m_curDialogIndex;
                    m_clickLabel.gameObject.SetActive(true);
					BoxCollider collider = m_dialogBoardSprite.GetComponent<BoxCollider>();
                    if (m_ftueData[m_FTUEIndex].from.IsAvailable())
                    {
                        m_clickLabel.text = Localization.instance.Get("MoveBlock");
						collider.size = new Vector3(300, 200, 1);
                    }
                    else
                    {
                        m_clickLabel.text = Localization.instance.Get("Click");
						collider.size = new Vector3(2048, 2048, 1);
                    }
                    if (m_afterDialogFunc != null)          //若有结束函数
                    {
                        m_afterDialogFunc();
                        m_pointer.SetActive(true);
                    }
                    else
                    {
                        m_pointer.SetActive(false);
                    }
                });
            });
        }
        else        //若是最后一段文字
        {
            if (m_FTUEIndex < m_ftueData.Count - 1)
            {
				HideHighLight();
				++m_FTUEIndex;
				HideWindow(delegate()
				{
					ShowFTUE(m_curStep);                         //若有步数，循环调用
				});
            }
            else                                            //若没有
            {
				EndFTUE();
            }
        }
	}
}
