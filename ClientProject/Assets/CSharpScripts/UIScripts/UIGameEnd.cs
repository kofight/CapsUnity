using UnityEngine;
using System.Collections;

public class UIGameEnd : UIWindowNGUI 
{
    UILabel m_resultLabel;
    UILabel m_infoLabel;
    bool m_bWin;
    int m_starCount;
    UIButton m_playOnBtn;
    UIButton m_EndGameBtn;
    public override void OnCreate()
    {
        base.OnCreate();
        m_resultLabel = UIToolkits.GetChildComponent<UILabel>(mUIObject.transform, "ResultLabel");
        m_infoLabel = UIToolkits.GetChildComponent<UILabel>(mUIObject.transform,"EndInfomation");

        m_playOnBtn = UIToolkits.GetChildComponent<UIButton>(mUIObject.transform, "PlayOnBtn");
        m_EndGameBtn = UIToolkits.GetChildComponent<UIButton>(mUIObject.transform, "EndGameBtn");
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
                m_bWin = false;
                m_resultLabel.text = "Failed!";
                m_infoLabel.text = "Did not clear all jelly" + GlobalVars.CurGameLogic.PlayingStageData.GetJellyCount() + "/" + GlobalVars.CurStageData.GetJellyCount();
            }
            else
            {
                m_bWin = true;
                m_resultLabel.text = "Win!";
                m_infoLabel.text = "All jellies has been cleared";
            }
        }
        else if (GlobalVars.CurStageData.Target == GameTarget.GetScore)
        {
            if (GlobalVars.CurGameLogic.GetProgress() >= GlobalVars.CurStageData.StarScore[0])
            {
                m_bWin = true;
                m_resultLabel.text = "Win!";
                m_infoLabel.text = "You got enough score";
                
            }
            else
            {
                m_bWin = false;
                m_resultLabel.text = "Failed!";
                m_infoLabel.text = "You didn't get enough score";
            }
        }

        if (GlobalVars.CurGameLogic.GetProgress() > GlobalVars.CurStageData.StarScore[2])
        {
            m_starCount = 3;
        }
        else if (GlobalVars.CurGameLogic.GetProgress() > GlobalVars.CurStageData.StarScore[1])
        {
            m_starCount = 2;
        }
        else if (GlobalVars.CurGameLogic.GetProgress() > GlobalVars.CurStageData.StarScore[0])
        {
            m_starCount = 1;
        }

        if (m_bWin)
        {
            if (GlobalVars.AvailabeStageCount == GlobalVars.CurStageNum)
            {
                ++GlobalVars.AvailabeStageCount;        //开启下一关
            }

            PlayerPrefs.SetInt("StageAvailableCount", GlobalVars.AvailabeStageCount);       //保存进度
            PlayerPrefs.SetInt("Stage" + GlobalVars.CurStageNum + "Stars", m_starCount);      //保存几星过关

            m_playOnBtn.gameObject.SetActive(false);
        }
        else
        {
            m_playOnBtn.gameObject.SetActive(true);
        }
    }
    public override void OnUpdate()
    {
        base.OnUpdate();
    }

    private void OnEndGameClicked(object sender, UIMouseClick.ClickArgs e)
    {
        HideWindow();
        if (m_bWin)
        {
            CapsApplication.Singleton.ChangeState((int)StateEnum.Login);        //返回地图界面
            UIWindowManager.Singleton.GetUIWindow<UIMainMenu>().ShowWindow();
            UIWindowManager.Singleton.GetUIWindow<UIMap>().ShowWindow();
            LoginState.Instance.CurFlow = TLoginFlow.LoginFlow_Map;         //切换流程到显示地图
        }
        else
        {
            UIWindowManager.Singleton.GetUIWindow<UIRetry>().ShowWindow();
        }
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
