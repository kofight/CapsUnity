using UnityEngine;
using System.Collections;

public class UIGameEnd : UIWindow 
{
    UIButton m_playOnBtn;
    UIButton m_EndGameBtn;
	NumberDrawer m_curScore;
	NumberDrawer m_targetScore;

    NumberDrawer m_curJelly;
    NumberDrawer m_targetJelly;

    NumberDrawer m_curNut1;
    NumberDrawer m_targetNut1;
    NumberDrawer m_curNut2;
    NumberDrawer m_targetNut2;

    UIToggle m_scoreCheck;
    UIToggle m_jellyCheck;
    UIToggle m_nutsCheck;
	
    public override void OnCreate()
    {
        base.OnCreate();

        m_playOnBtn = UIToolkits.FindComponent<UIButton>(mUIObject.transform, "PlayOnBtn");
        m_EndGameBtn = UIToolkits.FindComponent<UIButton>(mUIObject.transform, "EndGameBtn");
		
		m_curScore = UIToolkits.FindComponent<NumberDrawer>(mUIObject.transform, "CurScore");
        m_targetScore = UIToolkits.FindComponent<NumberDrawer>(mUIObject.transform, "TargetScore");

        m_curJelly = UIToolkits.FindComponent<NumberDrawer>(mUIObject.transform, "CurJelly");
        m_targetJelly = UIToolkits.FindComponent<NumberDrawer>(mUIObject.transform, "TargetJelly");

        m_curNut1 = UIToolkits.FindComponent<NumberDrawer>(mUIObject.transform, "CurNut1");
        m_targetNut1 = UIToolkits.FindComponent<NumberDrawer>(mUIObject.transform, "TargetNut1");
        m_curNut2 = UIToolkits.FindComponent<NumberDrawer>(mUIObject.transform, "CurNut2");
        m_targetNut2 = UIToolkits.FindComponent<NumberDrawer>(mUIObject.transform, "TargetNut2");

        m_scoreCheck = UIToolkits.FindComponent<UIToggle>(mUIObject.transform, "CheckBoxScore");
        m_jellyCheck = UIToolkits.FindComponent<UIToggle>(mUIObject.transform, "CheckBoxJelly");
        m_nutsCheck = UIToolkits.FindComponent<UIToggle>(mUIObject.transform, "CheckBoxNuts");
		
        AddChildComponentMouseClick("EndGameBtn", OnEndGameClicked);
        AddChildComponentMouseClick("PlayOnBtn", OnPlayOnClicked);
    }
    public override void OnShow()
    {
        base.OnShow();
        UILabel label = GetChildComponent<UILabel>("FailedReason");

        if (GameLogic.Singleton.CheckLimit())
        {
            if (GlobalVars.CurStageData.StepLimit > 0)
            {
                label.text = Localization.instance.Get("OutOfMove");
            }
            else if (GlobalVars.CurStageData.TimeLimit > 0)
            {
                label.text = Localization.instance.Get("OutOfTime");
            }
        }
        else
        {
            label.text = Localization.instance.Get("GamePaused");
        }

		m_curScore.SetNumber(GameLogic.Singleton.GetProgress());
		m_targetScore.SetNumber(GlobalVars.CurStageData.StarScore[0]);

        m_scoreCheck.value = (GameLogic.Singleton.GetProgress() >= GlobalVars.CurStageData.StarScore[0]);
        

        if (GlobalVars.CurStageData.Target == GameTarget.ClearJelly)
        {
            m_jellyCheck.gameObject.SetActive(true);
            m_nutsCheck.gameObject.SetActive(false);

            int totalJellyCount = GlobalVars.CurStageData.GetSingleJellyCount() + GlobalVars.CurStageData.GetDoubleJellyCount() * 2;
            int curJellyCount = GameLogic.Singleton.PlayingStageData.GetSingleJellyCount() + GameLogic.Singleton.PlayingStageData.GetDoubleJellyCount() * 2;

            m_curJelly.SetNumber(totalJellyCount - curJellyCount);
            m_targetJelly.SetNumber(totalJellyCount);

            m_jellyCheck.value = (curJellyCount == 0);
        }
        else if (GlobalVars.CurStageData.Target == GameTarget.BringFruitDown)
        {
            m_curNut1.SetNumber(GameLogic.Singleton.PlayingStageData.Nut1Count);
            m_targetNut1.SetNumber(GlobalVars.CurStageData.Nut1Count);
            m_curNut2.SetNumber(GameLogic.Singleton.PlayingStageData.Nut2Count);
            m_targetNut2.SetNumber(GlobalVars.CurStageData.Nut2Count);

            m_nutsCheck.gameObject.SetActive(true);
            m_jellyCheck.gameObject.SetActive(false);

            m_nutsCheck.value = (GameLogic.Singleton.PlayingStageData.Nut1Count == GlobalVars.CurStageData.Nut1Count && GameLogic.Singleton.PlayingStageData.Nut2Count == GlobalVars.CurStageData.Nut2Count);
        }
        else
        {
            m_nutsCheck.gameObject.SetActive(false);
            m_jellyCheck.gameObject.SetActive(false);
        }

        GameLogic.Singleton.PauseGame();
    }
    public override void OnUpdate()
    {
        base.OnUpdate();
    }

    private void OnEndGameClicked()
    {
        HideWindow();
		GameLogic.Singleton.ResumeGame();
		GameLogic.Singleton.PlayEndGameAnim();		//play the end anim(move the game area out of screen)

        GameLogic.Singleton.HideUI();

		UIWindowManager.Singleton.GetUIWindow<UIRetry>().RefreshData();
        UIWindowManager.Singleton.GetUIWindow<UIRetry>().ShowWindow();
    }

    private void OnPlayOnClicked()
    {
        if ((GameLogic.Singleton.GetGameFlow() == TGameFlow.EGameState_Playing && GlobalVars.CurStageData.StepLimit > 0 && GameLogic.Singleton.PlayingStageData.StepLimit > 0) ||          //若是限制步数的关卡，还有步数
            (GameLogic.Singleton.GetGameFlow() == TGameFlow.EGameState_Playing && GlobalVars.CurStageData.TimeLimit > 0 && GameLogic.Singleton.GetTimeRemain() > 0)            ||            //若是限制时间的关卡，还有时间
            (GameLogic.Singleton.GetGameFlow() != TGameFlow.EGameState_Clear && GameLogic.Singleton.GetGameFlow() != TGameFlow.EGameState_End && GameLogic.Singleton.GetGameFlow() != TGameFlow.EGameState_Playing)	//若不是在Playing状态
			)
        {
            //直接关闭窗口恢复游戏
            HideWindow(delegate()
            {
                GameLogic.Singleton.ResumeGame();
            });
            UIWindowManager.Singleton.GetUIWindow<UIMainMenu>().ShowWindow();
        }
        else                                            //若没步数或时间了，就要购买和使用道具
        {
            
                HideWindow(delegate()
                {
                    if (GlobalVars.CurStageData.StepLimit > 0)
                    {
                        GlobalVars.UsingItem = PurchasedItem.ItemAfterGame_PlusStep;
                    }
                    if (GlobalVars.CurStageData.TimeLimit > 0)
                    {
                        GlobalVars.UsingItem = PurchasedItem.ItemAfterGame_PlusTime;
                    }

                    if ((int)Unibiller.GetCurrencyBalance("gold") >= 70)        //是否有足够的钱购买道具
                    {
                        UIWindowManager.Singleton.GetUIWindow<UIPurchaseNoTarget>().ShowWindow();
                    }
                    else
                    {
                        UIWindow uiWindow = UIWindowManager.Singleton.GetUIWindow<UIPurchaseNotEnoughMoney>();
                        uiWindow.ShowWindow();
                    }
                    
                    
                });
        }
    }
}
