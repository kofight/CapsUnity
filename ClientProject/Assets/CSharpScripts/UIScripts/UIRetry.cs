using UnityEngine;
using System.Collections;

public class UIRetry : UIWindow 
{
    //UILabel m_resultLabel;
    UILabel m_infoLabel;
    bool m_bWin;
    int m_starCount;
    UISprite[] m_starsSprites = new UISprite[3];
    NumberDrawer m_stageNumber;

    public override void OnCreate()
    {
        base.OnCreate();

        //m_resultLabel = UIToolkits.FindComponent<UILabel>(mUIObject.transform, "ResultLabel");
        m_infoLabel = UIToolkits.FindComponent<UILabel>(mUIObject.transform, "EndInfomation");

        m_stageNumber = GetChildComponent<NumberDrawer>("LevelNumber");

        AddChildComponentMouseClick("CloseBtn", OnCloseClicked);
        AddChildComponentMouseClick("RetryBtn", OnRetryClicked);
        AddChildComponentMouseClick("NextLevelBtn", OnNextLevelClicked);

        for (int i = 0; i < 3; ++i)
        {
            m_starsSprites[i] = GetChildComponent<UISprite>("Star" + (i + 1));      //查找sprite
        }
    }
    public override void OnShow()
    {
        Transform nextBtn = UIToolkits.FindChild(mUIObject.transform, "NextLevelBtn");
        if (GlobalVars.CurGameLogic.IsStageFinish())         //检查关卡是否结束
        {
            nextBtn.gameObject.SetActive(true);
        }
        else
        {
            nextBtn.gameObject.SetActive(false);
        }
        base.OnShow();


        if (GlobalVars.CurGameLogic.GetProgress() < GlobalVars.CurStageData.StarScore[0])       //没到基础的分数要求的情况
        {
            m_bWin = false;
            //m_resultLabel.text = "Failed!";
            m_infoLabel.text = "Did not get any star";
        }
        else if (GlobalVars.CurStageData.Target == GameTarget.ClearJelly)
        {
            if (GlobalVars.CurGameLogic.PlayingStageData.GetJellyCount() != 0)
            {
                m_bWin = false;
                //m_resultLabel.text = "Failed!";
                m_infoLabel.text = "Did not clear all jelly" + GlobalVars.CurGameLogic.PlayingStageData.GetJellyCount() + "/" + GlobalVars.CurStageData.GetJellyCount();
            }
            else
            {
                m_bWin = true;
                //m_resultLabel.text = "Win!";
                m_infoLabel.text = "All jellies has been cleared";
            }
        }
        else if (GlobalVars.CurStageData.Target == GameTarget.GetScore)
        {
            if (GlobalVars.CurGameLogic.GetProgress() >= GlobalVars.CurStageData.StarScore[0])
            {
                m_bWin = true;
                //m_resultLabel.text = "Win!";
                m_infoLabel.text = "You got enough score";

            }
            else
            {
                m_bWin = false;
                //m_resultLabel.text = "Failed!";
                m_infoLabel.text = "You didn't get enough score";
            }
        }
        else if (GlobalVars.CurStageData.Target == GameTarget.BringFruitDown)
        {
            if (GlobalVars.CurGameLogic.PlayingStageData.Nut1Count == GlobalVars.CurStageData.Nut1Count
                && GlobalVars.CurGameLogic.PlayingStageData.Nut2Count == GlobalVars.CurStageData.Nut2Count)
            {
                m_bWin = true;
                //m_resultLabel.text = "Win!";
                m_infoLabel.text = "You've brought all fruit down";
            }
            else
            {
                m_bWin = false;
                //m_resultLabel.text = "Failed!";
                m_infoLabel.text = "You didn't brought all fruit down";
            }
        }

        if (m_bWin)
        {
            if (GlobalVars.AvailabeStageCount == GlobalVars.CurStageNum)
            {
                ++GlobalVars.AvailabeStageCount;        //开启下一关
            }

            for (int i = 2; i >= 0;--i )
            {
                if (GlobalVars.CurGameLogic.GetProgress() > GlobalVars.CurStageData.StarScore[i])
                {
                    m_starCount = i + 1;
                    break;
                }
            }

            PlayerPrefs.SetInt("StageAvailableCount", GlobalVars.AvailabeStageCount);       //保存进度
            if (m_starCount > GlobalVars.StageStarArray[GlobalVars.CurStageNum - 1])
            {
                GlobalVars.StageStarArray[GlobalVars.CurStageNum - 1] = m_starCount;            //保存星数
                PlayerPrefsExtend.SetIntArray("StageStars", GlobalVars.StageStarArray);         //保存进度
            }

            if (GlobalVars.CurGameLogic.GetProgress() > GlobalVars.StageScoreArray[GlobalVars.CurStageNum - 1])     //记录分数记录
            {
                GlobalVars.StageScoreArray[GlobalVars.CurStageNum - 1] = GlobalVars.CurGameLogic.GetProgress();
                PlayerPrefsExtend.SetIntArray("StageScores", GlobalVars.StageScoreArray);
            }
			
			if(CapsConfig.EnableGA)	//游戏过关后的记录
			{
                Debug.Log("Stage succeed GA");
				GA.API.Design.NewEvent("Stage" + GlobalVars.CurStageNum + ":Succeed", GlobalVars.CurGameLogic.GetProgress());  //分数
	            GA.API.Design.NewEvent("Stage" + GlobalVars.CurStageNum + ":Succeed:Score_Percent", (float)GlobalVars.CurGameLogic.GetProgress() / GlobalVars.CurGameLogic.PlayingStageData.StarScore[0]);  //记录当前开始的关卡的百分比
	            GA.API.Design.NewEvent("Stage" + GlobalVars.CurStageNum + ":Succeed:Score_3StarPercent", (float)GlobalVars.CurGameLogic.GetProgress() / GlobalVars.CurGameLogic.PlayingStageData.StarScore[2]);  //记录当前开始的关卡的百分比	
			}
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
        }


        m_starCount = GlobalVars.StageStarArray[GlobalVars.CurStageNum - 1];
        NumberDrawer number = GetChildComponent<NumberDrawer>("StageScore");
        number.SetNumber(GlobalVars.CurGameLogic.GetProgress());

        m_stageNumber.SetNumber(GlobalVars.CurStageNum);

        //根据starCount显示星星
        for (int i = 0; i < 3; ++i)
        {
            if (i < m_starCount)
            {
                m_starsSprites[i].spriteName = "Star_Large";
            }
            else
            {
                m_starsSprites[i].spriteName = "Grey_Star_Large";
            }
        }

        GlobalVars.CurGameLogic.EndGame();
        GlobalVars.CurGameLogic.ChangeGameFlow(TGameFlow.EGameState_Clear);           //切换到结束状态
    }
    public override void OnUpdate()
    {
        base.OnUpdate();
        if (uiWindowState == UIWindowStateEnum.Show)
        {
            UIDrawer.Singleton.DefaultAnchor = UIWindowManager.Anchor.Center;
            UIDrawer.Singleton.DrawNumber("ScoreTarget", 0, -72, GlobalVars.StageScoreArray[GlobalVars.CurStageNum - 1], "", 22);           //当前关卡的最高分
            UIDrawer.Singleton.DefaultAnchor = UIWindowManager.Anchor.TopLeft;
        }
    }

    private void OnRetryClicked()
    {
        if (GlobalVars.HeartCount == 0)
        {
            return;
        }

        HideWindow();
        GlobalVars.CurGameLogic.ClearGame();
        GlobalVars.CurGameLogic.Init();
        GlobalVars.CurGameLogic.StartGame();
    }

    private void OnNextLevelClicked()
    {
        HideWindow();
        ++GlobalVars.LastStage;
        if (GlobalVars.StageStarArray[GlobalVars.LastStage] == 0)       //若是新开的关
        {
            CapsApplication.Singleton.ChangeState((int)StateEnum.Login);        //返回地图界面
            UIWindowManager.Singleton.GetUIWindow<UIMainMenu>().ShowWindow();
            UIWindowManager.Singleton.GetUIWindow<UIMap>().ShowWindow();
            LoginState.Instance.CurFlow = TLoginFlow.LoginFlow_Map;         //切换流程到显示地图
        }
        else
        {
            GlobalVars.CurStageNum = GlobalVars.LastStage;                          //进下一关
            UIWindowManager.Singleton.GetUIWindow<UIStageInfo>().ShowWindow();      //显示关卡信息
            GlobalVars.CurGameLogic.ClearGame();
        }
    }

    private void OnCloseClicked()
    {
        HideWindow();
        UIWindowManager.Singleton.GetUIWindow<UIMainMenu>().HideWindow();
        CapsApplication.Singleton.ChangeState((int)StateEnum.Login);        //返回地图界面
        UIWindowManager.Singleton.GetUIWindow<UIMainMenu>().ShowWindow();
        UIWindowManager.Singleton.GetUIWindow<UIMap>().ShowWindow();
        LoginState.Instance.CurFlow = TLoginFlow.LoginFlow_Map;         //切换流程到显示地图
    }
}
