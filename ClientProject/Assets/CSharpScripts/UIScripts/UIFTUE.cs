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

    WindowEffectFinished m_afterDialogFunc;
	
	string [] m_dialogContents;
	int m_curDialogIndex;
	
	bool m_bLock;

    int m_FTUEIndex = 0;
	int m_curStep = 0;

    List<FTUEData> m_ftueData;

    public override void OnHide()
    {
        base.OnHide();
		
		if(m_ftueData[m_FTUEIndex].from.IsAvailable())
		{
        	GameLogic.Singleton.SetHighLight(false, m_ftueData[m_FTUEIndex].from);
	        foreach (Position highLightPos in m_ftueData[m_FTUEIndex].highLightPosList)
	        {
	            GameLogic.Singleton.SetHighLight(false, highLightPos);
	        }
		}

        GameLogic.Singleton.ShowUI();
    }

    public void ShowFTUE(int step)
    {
		m_curStep = step;
        if (GameLogic.Singleton.PlayingStageData.FTUEMap.TryGetValue(step, out m_ftueData))
        {
            if (!m_ftueData[m_FTUEIndex].from.IsAvailable())
            {
                ShowText(m_ftueData[m_FTUEIndex].headImage, m_ftueData[m_FTUEIndex].pic, m_ftueData[m_FTUEIndex].dialog, null);
            }
            else
            {
                ShowText(m_ftueData[m_FTUEIndex].headImage, m_ftueData[m_FTUEIndex].pic, m_ftueData[m_FTUEIndex].dialog, delegate()
                {
                    GameLogic.Singleton.SetHighLight(true, m_ftueData[m_FTUEIndex].from);
                    foreach (Position highLightPos in m_ftueData[m_FTUEIndex].highLightPosList)
                    {
                        GameLogic.Singleton.SetHighLight(true, highLightPos);
                    }
                });
            }
        }

        GameLogic.Singleton.HideUI();
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
					m_bLock = true;
				}
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
        m_pic = GetChildComponent<UISprite>("Picture");
		AddChildComponentMouseClick("DialogBoard", OnClick);
    }

    public void EndFTUE()
    {
        HideWindow();                               //隐藏窗体
        GameLogic.Singleton.SetGameFlow(TGameFlow.EGameState_Playing);
    }
	
	public void OnClick()
	{
		if(m_bLock)
			return;

        if (m_curDialogIndex < m_dialogContents.Length)      //若不是最后一段文字
        {
            //播放下一段
            m_dialogText.Play(m_dialogContents[m_curDialogIndex], delegate()
            {
                ++m_curDialogIndex;
                if (m_curDialogIndex == m_dialogContents.Length)        //下一段若是最后一段文字, 结束播放
                {
                    if (m_afterDialogFunc != null)          //若有结束函数
                    {
                        m_afterDialogFunc();
                        m_bLock = true;
                    }
                    else                                    //若无结束函数，直接进下一步或关闭
                    {
                        if (m_FTUEIndex < m_ftueData.Count - 1)
                        {
                            ++m_FTUEIndex;
                            ShowFTUE(m_curStep);                         //若有步数，循环调用
                        }
                        else                                            //若没有
                        {
                            EndFTUE();
                        }
                    }
                }
                else
                {
                    m_bLock = false;
                }
            });
        }
        else        //若是最后一段文字
        {
            if (m_FTUEIndex < m_ftueData.Count - 1)
            {
                ++m_FTUEIndex;
                ShowFTUE(m_curStep);                         //若有步数，循环调用
            }
            else                                            //若没有
            {
                HideWindow();                               //隐藏窗体
                GameLogic.Singleton.SetGameFlow(TGameFlow.EGameState_Playing);
            }
        }
	}
}
