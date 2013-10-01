using UnityEngine;
using System.Collections;

public class UIGameEnd : UIWindowNGUI 
{
    UILabel m_resultLabel;
    UILabel m_infoLabel;
    public override void OnCreate()
    {
        base.OnCreate();
        m_resultLabel = UIToolkits.GetChildComponent<UILabel>(mUIObject.transform, "ResultLabel");
        m_infoLabel = UIToolkits.GetChildComponent<UILabel>(mUIObject.transform,"EndInfomation");
        AddChildComponentMouseClick("EndGameBtn", OnEndGameClicked);
        AddChildComponentMouseClick("PlayOnBtn", OnPlayOnClicked);
    }
    public override void OnShow()
    {
        base.OnShow();
        if (GlobalVars.CurStageData.Target == GameTarget.ClearJelly)
        {
            if (GlobalVars.CurGameLogic.PlayingStageData.GetJellyCount() != 0)
            {
                m_resultLabel.text = "Failed!";
                m_infoLabel.text = "Did not clear all jelly" + GlobalVars.CurGameLogic.PlayingStageData.GetJellyCount() + "/" + GlobalVars.CurStageData.GetJellyCount();
            }
            else
            {
                m_resultLabel.text = "Win!";
                m_infoLabel.text = "All jellies has been cleared";
            }
        }
        else if (GlobalVars.CurStageData.Target == GameTarget.GetScore)
        {
            if (GlobalVars.CurGameLogic.GetProgress() >= GlobalVars.CurStageData.StarScore[0])
            {
                m_resultLabel.text = "Win!";
                m_infoLabel.text = "You got enough score";
            }
            else
            {
                m_resultLabel.text = "Failed!";
                m_infoLabel.text = "You didn't get enough score";
            }
        }
    }
    public override void OnUpdate()
    {
        base.OnUpdate();
    }

    private void OnEndGameClicked(object sender, UIMouseClick.ClickArgs e)
    {
        HideWindow();
        UIWindowManager.Singleton.GetUIWindow<UIRetry>().ShowWindow();
    }

    private void OnPlayOnClicked(object sender, UIMouseClick.ClickArgs e)
    {
       if (GlobalVars.CurStageData.StepLimit > 0)
       {
           if (GlobalVars.CurStageData.StepLimit > 0)     //若还有步数
           {
               HideWindow();
           }
       }
    }
}
