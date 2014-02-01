using UnityEngine;
using System.Collections;

public class UIGameBottom : UIWindow
{
    UISprite[] m_starsSprites = new UISprite[3];
    UISprite m_progressSprite;
    static readonly int ProgressLenth = 386;
    static readonly int ProgressStartX = -89;

    UISprite stageBoard;
    UISlider m_speedSlider;
    UILabel m_speedLabel;
    UISprite m_timeBar;

    NumberDrawer m_scoreDrawer;
    NumberDrawer m_stepDrawer;

    GameObject m_timeNumber;
    NumberDrawer m_minNumber;
    NumberDrawer m_secNumber;

    public override void OnCreate()
    {
        base.OnCreate();

        for (int i = 0; i < 3; ++i )
        {
            m_starsSprites[i] = GetChildComponent<UISprite>("Star" + (i + 1));      //查找sprite
        }
        m_progressSprite = GetChildComponent<UISprite>("Progress");      //查找sprite

        stageBoard = GetChildComponent<UISprite>("StageBoard");

        m_speedSlider = GetChildComponent<UISlider>("SpeedSlider");
        m_speedLabel = GetChildComponent<UILabel>("SpeedLabel");

        m_timeBar = GetChildComponent<UISprite>("TimeBar");

        m_scoreDrawer = GetChildComponent<NumberDrawer>("ScoreDrawer");
        m_stepDrawer = GetChildComponent<NumberDrawer>("StepDrawer");

        m_timeNumber = UIToolkits.FindChild(mUIObject.transform, "TimeNumber").gameObject;
        m_minNumber = GetChildComponent<NumberDrawer>("MinNumber");
        m_secNumber = GetChildComponent<NumberDrawer>("SecNumber");
        
    }
    public override void OnShow()
    {
        base.OnShow();
        if (GameLogic.Singleton.PlayingStageData.TimeLimit > 0)
        {
            stageBoard.spriteName = "TimeBoard";
            m_timeBar.gameObject.SetActive(true);

            m_stepDrawer.gameObject.SetActive(false);
            m_timeNumber.SetActive(true);
        }
        else
        {
            stageBoard.spriteName = "StepBoard";
            m_timeBar.gameObject.SetActive(false);

            m_stepDrawer.gameObject.SetActive(true);
            m_timeNumber.SetActive(false);
        }

        if (GlobalVars.DeveloperMode)
        {
            m_speedSlider.gameObject.SetActive(true);
            EventDelegate.Set(m_speedSlider.onChange, OnValueChange);
            m_speedSlider.value = CapsConfig.Instance.GameSpeed;
            m_speedLabel.gameObject.SetActive(true);
        }
        else
        {
            m_speedSlider.gameObject.SetActive(false);
            m_speedLabel.gameObject.SetActive(false);
        }
    }
	
	public void Reset()
	{
		if (GlobalVars.CurStageData.StarScore[2] > 0)
        {
            for (int i = 0; i < 2; ++i)
            {
                m_starsSprites[i].LocalPositionX(ProgressStartX + ProgressLenth * GlobalVars.CurStageData.StarScore[i] / GlobalVars.CurStageData.StarScore[2]);      //放置位置
            }
            for (int i = 0; i < 3; ++i)
            {
                m_starsSprites[i].spriteName = "Start";         //初始化成默认状态
            }
        }
		
		m_progressSprite.gameObject.SetActive(false);
	}
	
    public override void OnUpdate()
    {
        base.OnUpdate();
        float completeRatio = 0.0f;
        if (GlobalVars.CurStageData.StarScore[2] > 0)
        {
            completeRatio = (float)GameLogic.Singleton.GetProgress() / GlobalVars.CurStageData.StarScore[2];
        }
        completeRatio = Mathf.Min(1.0f, completeRatio);
        if (completeRatio != 0.0f)
        {
			m_progressSprite.gameObject.SetActive(true);
            m_progressSprite.width = (int)(ProgressLenth * completeRatio);
        }

        if (GlobalVars.CurStageData.TimeLimit > 0)          //限制时间的关卡
        {
            int min = (int)GameLogic.Singleton.GetTimeRemain() / 60;
            int second = (int)GameLogic.Singleton.GetTimeRemain() % 60;
            m_minNumber.SetNumber(min);
            m_secNumber.SetNumber(second);

            m_timeBar.fillAmount = GameLogic.Singleton.GetTimeRemain() / GlobalVars.CurStageData.TimeLimit;
        }
        if (GlobalVars.CurStageData.StepLimit > 0)          //限制步数的关卡
        {
            m_stepDrawer.SetNumber(GameLogic.Singleton.PlayingStageData.StepLimit);
        }

        m_scoreDrawer.SetNumber(GameLogic.Singleton.GetProgress());
    }

    public void OnValueChange()
    {
        CapsConfig.Instance.GameSpeed = UISlider.current.value;
        m_speedLabel.text = CapsConfig.Instance.GameSpeed.ToString();
    }

    public void OnChangeProgress(int progress)
    {
        for (int i = 0; i < 3; ++i)
        {
            if (progress >= GlobalVars.CurStageData.StarScore[i])
            {
                m_starsSprites[i].spriteName = "LightStar";
            }
        }
        m_scoreDrawer.SetNumber(progress);
    }
}
