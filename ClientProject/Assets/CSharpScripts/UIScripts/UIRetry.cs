using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
        if (GameLogic.Singleton.IsStageFinish())         //检查关卡是否结束
        {
            nextBtn.gameObject.SetActive(true);
        }
        else
        {
            nextBtn.gameObject.SetActive(false);
        }
        base.OnShow();


        if (GameLogic.Singleton.GetProgress() < GlobalVars.CurStageData.StarScore[0])       //没到基础的分数要求的情况
        {
            m_bWin = false;
            //m_resultLabel.text = "Failed!";
            m_infoLabel.text = "Did not get any star";
        }
        else if (GlobalVars.CurStageData.Target == GameTarget.ClearJelly)
        {
            if (GameLogic.Singleton.PlayingStageData.GetJellyCount() != 0)
            {
                m_bWin = false;
                //m_resultLabel.text = "Failed!";
                m_infoLabel.text = "Did not clear all jelly" + GameLogic.Singleton.PlayingStageData.GetJellyCount() + "/" + GlobalVars.CurStageData.GetJellyCount();
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
            if (GameLogic.Singleton.GetProgress() >= GlobalVars.CurStageData.StarScore[0])
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
            if (GameLogic.Singleton.PlayingStageData.Nut1Count == GlobalVars.CurStageData.Nut1Count
                && GameLogic.Singleton.PlayingStageData.Nut2Count == GlobalVars.CurStageData.Nut2Count)
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
                UIWindowManager.Singleton.GetUIWindow<UIMap>().RefreshButton(GlobalVars.AvailabeStageCount);
            }

            for (int i = 2; i >= 0;--i )
            {
                if (GameLogic.Singleton.GetProgress() > GlobalVars.CurStageData.StarScore[i])
                {
                    m_starCount = i + 1;
                    break;
                }
            }

			if(GlobalVars.StageStarArray[GlobalVars.CurStageNum] == 0)		//if it's the first time of finishing the stage
			{
				float scorePercent = (float)GameLogic.Singleton.GetProgress() / GameLogic.Singleton.PlayingStageData.StarScore[0];
				if(CapsConfig.EnableGA)	//游戏过关后的记录
				{
					GA.API.Design.NewEvent("Stage" + GlobalVars.CurStageNum + ":FirstSucceed:StepLeft", GameLogic.Singleton.m_stepCountWhenReachTarget);  //
					GA.API.Design.NewEvent("Stage" + GlobalVars.CurStageNum + ":FirstSucceed:Score_Percent", scorePercent);  //记录当前开始的关卡的百分比
					GA.API.Design.NewEvent("Stage" + GlobalVars.CurStageNum + ":FirstSucceed:StarCount", m_starCount);	
					GA.API.Design.NewEvent("Stage" + GlobalVars.CurStageNum + ":FirstSucceed:FailedTimes", GlobalVars.StageFailedArray[GlobalVars.CurStageNum]);	
					GA.API.Design.NewEvent("Stage" + GlobalVars.CurStageNum + ":FirstSucceed:PlayTime", GameLogic.Singleton.GetStagePlayTime());	
					
				}
				if(CapsConfig.EnableTalkingData)
				{
					Dictionary<string, object> param = new Dictionary<string, object>();
					if (GameLogic.Singleton.m_stepCountWhenReachTarget > 10)
					{
						param["StepLeft"] = ">10";
					}
					else if(GameLogic.Singleton.m_stepCountWhenReachTarget > 5)
					{
						param["StepLeft"] = ">5";
					}
					else
					{
						param["StepLeft"] = GameLogic.Singleton.m_stepCountWhenReachTarget.ToString();
					}

					param["StarCount"] = m_starCount.ToString();

					if(scorePercent > 5)
					{
						param["ScorePercent"] = ">5";
					}
					else if(scorePercent > 3)
					{
						param["ScorePercent"] = ">3";
					}
					else if(scorePercent > 2)
					{
						param["ScorePercent"] = ">2";
					}
					else if(scorePercent > 1.5)
					{
						param["ScorePercent"] = ">1.5";
					}
					else
					{
						param["ScorePercent"] = ">1";
					}

					float playTime = GameLogic.Singleton.GetStagePlayTime();

					if(playTime > 600)
					{
						param["PlayTime"] = ">10min";
					}
					else if(playTime > 60)
					{
						param["PlayTime"] = ((int)(playTime / 60)).ToString() + "min";
					}
					else
					{
						param["PlayTime"] = "<1min";
					}

					int failedTimes = GlobalVars.StageFailedArray[GlobalVars.CurStageNum];

					if(failedTimes > 20)
					{
						param["FailedTime"] = ">20";
					}
					else if(failedTimes > 10)
					{
						param["PlayTime"] = ">10";
					}
					else if(failedTimes > 5)
					{
						param["PlayTime"] = ">5";
					}
					else
					{
						param["PlayTime"] = failedTimes.ToString();
					}


					TalkingDataPlugin.TrackEventWithParameters("Stage" + GlobalVars.CurStageNum + ":FirstSucceed", "", param);
				}
			}

            PlayerPrefs.SetInt("StageAvailableCount", GlobalVars.AvailabeStageCount);       //保存进度
            if (m_starCount > GlobalVars.StageStarArray[GlobalVars.CurStageNum - 1])
            {
                GlobalVars.StageStarArray[GlobalVars.CurStageNum - 1] = m_starCount;            //保存星数
                PlayerPrefsExtend.SetIntArray("StageStars", GlobalVars.StageStarArray);         //保存进度
            }

            if (GameLogic.Singleton.GetProgress() > GlobalVars.StageScoreArray[GlobalVars.CurStageNum - 1])     //记录分数记录
            {
                GlobalVars.StageScoreArray[GlobalVars.CurStageNum - 1] = GameLogic.Singleton.GetProgress();
                PlayerPrefsExtend.SetIntArray("StageScores", GlobalVars.StageScoreArray);
            }

            UIWindowManager.Singleton.GetUIWindow<UIMap>().RefreshButton(GlobalVars.CurStageNum);
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

				GlobalVars.StageFailedArray[GlobalVars.CurStageNum]++;
				PlayerPrefsExtend.SetIntArray("StageFailed", GlobalVars.StageFailedArray);
            }
        }


        m_starCount = GlobalVars.StageStarArray[GlobalVars.CurStageNum - 1];
        NumberDrawer number = GetChildComponent<NumberDrawer>("StageScore");
        number.SetNumber(GameLogic.Singleton.GetProgress());

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

        GameLogic.Singleton.EndGame();
        GameLogic.Singleton.ChangeGameFlow(TGameFlow.EGameState_Clear);           //切换到结束状态
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
            if (GlobalVars.Coins > 0)
            {
                HideWindow();
                UIWindowManager.Singleton.GetUIWindow<UIBuyHeart>().ShowWindow();
            }
            return;
        }

        HideWindow();
        GameLogic.Singleton.ClearGame();
        GameLogic.Singleton.Init();
        GameLogic.Singleton.StartGame();
    }

    private void OnNextLevelClicked()
    {
        ++GlobalVars.LastStage;
        if (GlobalVars.StageStarArray[GlobalVars.LastStage-1] == 0)       //若是新开的关
        {
            HideWindow(delegate()
            {
                CapsApplication.Singleton.ChangeState((int)StateEnum.Login);        //返回地图界面
            });
            
            UIWindowManager.Singleton.GetUIWindow<UIMap>().ShowWindow();
            LoginState.Instance.CurFlow = TLoginFlow.LoginFlow_Map;         //切换流程到显示地图
        }
        else
        {
            HideWindow();
            GlobalVars.CurStageNum = GlobalVars.LastStage;                          //进下一关
            UIWindowManager.Singleton.GetUIWindow<UIStageInfo>().ShowWindow(delegate()
            {
                GameLogic.Singleton.ClearGame();
            });      //显示关卡信息

            
        }
    }

    private void OnCloseClicked()
    {
        HideWindow(delegate()
        {
            CapsApplication.Singleton.ChangeState((int)StateEnum.Login);        //返回地图界面
        });
        UIWindowManager.Singleton.GetUIWindow<UIMap>().ShowWindow();
        LoginState.Instance.CurFlow = TLoginFlow.LoginFlow_Map;         //切换流程到显示地图
    }
}
