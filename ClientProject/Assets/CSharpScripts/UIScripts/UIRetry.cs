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

    Transform m_winBoard;
    Transform m_failedBoard;
	
	bool m_bStartNewStage = false;		//record if started a new stage

    public override void OnCreate()
    {
        base.OnCreate();

        //m_resultLabel = UIToolkits.FindComponent<UILabel>(mUIObject.transform, "ResultLabel");
        m_infoLabel = UIToolkits.FindComponent<UILabel>(mUIObject.transform, "EndInfomation");

        m_stageNumber = GetChildComponent<NumberDrawer>("LevelNumber");

        m_winBoard = UIToolkits.FindChild(mUIObject.transform, "WinBoard");
        m_failedBoard = UIToolkits.FindChild(mUIObject.transform, "FailedBoard");

        AddChildComponentMouseClick("CloseBtn", OnCloseClicked);
        AddChildComponentMouseClick("RetryBtn", OnRetryClicked);
        AddChildComponentMouseClick("NextLevelBtn", OnNextLevelClicked);

        for (int i = 0; i < 3; ++i)
        {
            m_starsSprites[i] = GetChildComponent<UISprite>("Star" + (i + 1));      //查找sprite
        }
    }
	
	public override void OnShow ()
	{
		base.OnShow ();
		Transform nextBtn = UIToolkits.FindChild(mUIObject.transform, "NextLevelBtn");
		if (m_bWin)
        {
			nextBtn.gameObject.SetActive(true);
			
            m_winBoard.gameObject.SetActive(true);
            m_failedBoard.gameObject.SetActive(false);

            m_starCount = GlobalVars.StageStarArray[GlobalVars.CurStageNum - 1];
            NumberDrawer number = GetChildComponent<NumberDrawer>("StageScore");
            number.SetNumber(GameLogic.Singleton.GetProgress());

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
        }
        else
        {
			nextBtn.gameObject.SetActive(false);
            m_winBoard.gameObject.SetActive(false);
            m_failedBoard.gameObject.SetActive(true);
        }
	}
	
	public void RefreshData()
	{
		m_bStartNewStage = false;
        if (GameLogic.Singleton.IsStageFinish() && GameLogic.Singleton.CheckGetEnoughScore())         //检查关卡是否结束
        {
			m_bWin = true;
        }
        else
        {
			m_bWin = false;
            if (!GameLogic.Singleton.CheckGetEnoughScore())       //没到基础的分数要求的情况
            {
                m_infoLabel.text = "Did not get any star";
            }
            else if (GlobalVars.CurStageData.Target == GameTarget.ClearJelly)
            {
                m_infoLabel.text = "Did not clear all ice block";
            }
            else if (GlobalVars.CurStageData.Target == GameTarget.BringFruitDown)
            {
                m_infoLabel.text = "You didn't brought all nuts down";
            }
        }

        if (m_bWin)
        {
			if(GlobalVars.HeartCount < 5)		//return heart back if win
			{
				++GlobalVars.HeartCount;
				if(GlobalVars.HeartCount > 5)
					GlobalVars.HeartCount = 5;
			}

            if (GlobalVars.AvailabeStageCount == GlobalVars.CurStageNum)
            {
                ++GlobalVars.AvailabeStageCount;        //开启下一关
				m_bStartNewStage = true;
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
			GlobalVars.StageFailedArray[GlobalVars.CurStageNum]++;
			PlayerPrefsExtend.SetIntArray("StageFailed", GlobalVars.StageFailedArray);
        }

        m_stageNumber.SetNumber(GlobalVars.CurStageNum);

        GameLogic.Singleton.PlayEndGameAnim();		//play the end anim(move the game area out of screen)
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
		if (GlobalVars.HeartCount == 0)         //若没有心了
        {
            HideWindow();
            GameLogic.Singleton.ClearGame();
            UIWindowManager.Singleton.GetUIWindow<UINoMoreHearts>().ShowWindow();           //显示买心界面
            return;
        }
		
		UIWindowManager.Singleton.GetUIWindow<UIStageInfo>().ShowWindow(delegate()
        {
                GameLogic.Singleton.ClearGame();
        });      //显示关卡信息 

        HideWindow();       //隐藏自己
    }

    private void OnNextLevelClicked()
    {
        ++GlobalVars.LastStage;
		GameLogic.Singleton.ClearGame();
        if (m_bStartNewStage)       //若是新开的关
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
            UIWindowManager.Singleton.GetUIWindow<UIStageInfo>().ShowWindow();   //显示关卡信息 
        }
    }

    private void OnCloseClicked()
    {
        HideWindow(delegate()
        {
            CapsApplication.Singleton.ChangeState((int)StateEnum.Login);        //返回地图界面
			GameLogic.Singleton.ClearGame();
        });
        UIWindowManager.Singleton.GetUIWindow<UIMap>().ShowWindow();
        LoginState.Instance.CurFlow = TLoginFlow.LoginFlow_Map;         //切换流程到显示地图
    }
}
