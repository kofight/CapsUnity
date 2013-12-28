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
        UISprite sprite = GetChildComponent<UISprite>("FailedReason");

        if (GameLogic.Singleton.CheckLimit())
        {
            if (GlobalVars.CurStageData.StepLimit > 0)
            {
                sprite.spriteName = "TextOutOfMove";
            }
            else if (GlobalVars.CurStageData.TimeLimit > 0)
            {
                sprite.spriteName = "TextOutOfTime";
            }
        }
        else
        {
            sprite.spriteName = "TextPressedMenuBtn";
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
		UIWindowManager.Singleton.GetUIWindow<UIRetry>().RefreshData();
        UIWindowManager.Singleton.GetUIWindow<UIRetry>().ShowWindow();
    }

    private void OnPlayOnClicked()
    {
        if (GameLogic.Singleton.PlayingStageData.StepLimit > 0)     //若还有步数
        {
            HideWindow(delegate()
            {
                GameLogic.Singleton.ResumeGame();
            });
            UIWindowManager.Singleton.GetUIWindow<UIMainMenu>().ShowWindow();
        }
        else                                            //若没步数了，就要购买和使用道具
        {
            HideWindow();
            if (GlobalVars.Coins > 0)
            {
                UIWindowManager.Singleton.GetUIWindow<UIPurchase>().ShowWindow();
                UIWindowManager.Singleton.GetUIWindow<UIPurchase>().OnPurchase = delegate()
                {
                    --GlobalVars.Coins;
                    GA.API.Business.NewEvent("BuyStep", "RMB", 1);
                    PlayerPrefs.SetInt("Coins", GlobalVars.Coins);
                    GameLogic.Singleton.PlayingStageData.StepLimit += 5;        //步数加5
                    GameLogic.Singleton.SetGameFlow(TGameFlow.EGameState_Playing);      //回到可以继续玩的状态
                };
            }
        }
    }
}
