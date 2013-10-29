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
    UISprite[] m_starsSprites = new UISprite[3];
    public override void OnCreate()
    {
        base.OnCreate();
        m_resultLabel = UIToolkits.GetChildComponent<UILabel>(mUIObject.transform, "ResultLabel");
        m_infoLabel = UIToolkits.GetChildComponent<UILabel>(mUIObject.transform,"EndInfomation");

        m_playOnBtn = UIToolkits.GetChildComponent<UIButton>(mUIObject.transform, "PlayOnBtn");
        m_EndGameBtn = UIToolkits.GetChildComponent<UIButton>(mUIObject.transform, "EndGameBtn");
        AddChildComponentMouseClick("EndGameBtn", OnEndGameClicked);
        AddChildComponentMouseClick("PlayOnBtn", OnPlayOnClicked);

        for (int i = 0; i < 3; ++i)
        {
            m_starsSprites[i] = GetChildComponent<UISprite>("Star" + (i + 1));      //查找sprite
        }
    }
    public override void OnShow()
    {
        base.OnShow();
        if (GlobalVars.CurGameLogic.GetProgress() < GlobalVars.CurStageData.StarScore[0])       //没到基础的分数要求的情况
        {
            m_bWin = false;
            m_resultLabel.text = "Failed!";
            m_infoLabel.text = "Did not get any star";
        }
        else if (GlobalVars.CurStageData.Target == GameTarget.ClearJelly)
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
		else if(GlobalVars.CurStageData.Target == GameTarget.BringFruitDown)
		{
            if (GlobalVars.CurGameLogic.PlayingStageData.Nut1Count == GlobalVars.CurStageData.Nut1Count
                && GlobalVars.CurGameLogic.PlayingStageData.Nut2Count == GlobalVars.CurStageData.Nut2Count)
            {
                m_bWin = true;
                m_resultLabel.text = "Win!";
                m_infoLabel.text = "You've brought all fruit down";
            }
            else
            {
                m_bWin = false;
                m_resultLabel.text = "Failed!";
                m_infoLabel.text = "You didn't brought all fruit down";
            }
		}

        if (GlobalVars.CurGameLogic.GetProgress() >= GlobalVars.CurStageData.StarScore[2])
        {
            m_starCount = 3;
        }
        else if (GlobalVars.CurGameLogic.GetProgress() >= GlobalVars.CurStageData.StarScore[1])
        {
            m_starCount = 2;
        }
        else if (GlobalVars.CurGameLogic.GetProgress() >= GlobalVars.CurStageData.StarScore[0])
        {
            m_starCount = 1;
        }

        //根据starCount显示星星
        for (int i = 0; i < 3; ++i )
        {
            if (i < m_starCount)
            {
                m_starsSprites[i].gameObject.SetActive(true);
            }
            else
            {
                m_starsSprites[i].gameObject.SetActive(false);
            }
        }

        if (m_bWin)
        {
            if (GlobalVars.AvailabeStageCount == GlobalVars.CurStageNum)
            {
                ++GlobalVars.AvailabeStageCount;        //开启下一关
            }

            PlayerPrefs.SetInt("StageAvailableCount", GlobalVars.AvailabeStageCount);       //保存进度
            if (m_starCount > GlobalVars.StageStarArray[GlobalVars.CurStageNum - 1])
            {
                GlobalVars.StageStarArray[GlobalVars.CurStageNum - 1] = m_starCount;            //保存星数
                PlayerPrefsExtend.SetIntArray("StageStars", GlobalVars.StageStarArray);
            }

            m_playOnBtn.gameObject.SetActive(false);
        }
        else
        {
            if (GlobalVars.HeartCount > 0)
            {
                if (GlobalVars.HeartCount == 5)     //若还没用过心
                {
                    GlobalVars.GetHeartTime = System.DateTime.Now;          //初始化获得心的时间
                }
                --GlobalVars.HeartCount;            //消耗一个心
            }

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
