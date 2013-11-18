using UnityEngine;
using System.Collections;

public class UIGameBottom : UIWindowNGUI
{
    UISprite[] m_starsSprites = new UISprite[3];
    UISprite m_progressSprite;
    static readonly int ProgressLenth = 348;
    static readonly int ProgressStartX = -89;

    UISprite stageBoard;
    UISlider m_speedSlider;
    UILabel m_speedLabel;
    UIFilledSprite m_timeBar;

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

        m_timeBar = GetChildComponent<UIFilledSprite>("TimeBar");
    }
    public override void OnShow()
    {
        base.OnShow();
        if (GlobalVars.CurGameLogic.PlayingStageData.TimeLimit > 0)
        {
            stageBoard.spriteName = "TimeBoard";
            m_timeBar.gameObject.SetActive(true);
        }
        else
        {
            stageBoard.spriteName = "StepBoard";
            m_timeBar.gameObject.SetActive(false);
        }

        if (GlobalVars.DeveloperMode)
        {
            m_speedSlider.gameObject.SetActive(true);
            m_speedSlider.onValueChange = OnValueChange;
            m_speedSlider.sliderValue = CapsConfig.Instance.GameSpeed;
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
            completeRatio = (float)GlobalVars.CurGameLogic.GetProgress() / GlobalVars.CurStageData.StarScore[2];
        }
        completeRatio = Mathf.Min(1.0f, completeRatio);
        if (completeRatio != 0.0f)
        {
			m_progressSprite.gameObject.SetActive(true);
            m_progressSprite.transform.LocalScaleX(ProgressLenth * completeRatio);
        }
        //GlobalVars.CurGameLogic.GetProgress()

        UIDrawer.Singleton.DefaultAnchor = UIWindowManager.Anchor.Bottom;
        if (GlobalVars.CurStageData.StepLimit > 0)          //限制步数的关卡
        {
            UIDrawer.Singleton.DrawNumber("SetpLimit", -100, -114, GlobalVars.CurGameLogic.PlayingStageData.StepLimit, "", 24);
        }
        if (GlobalVars.CurStageData.TimeLimit > 0)          //限制时间的关卡
        {
            int min = (int)GlobalVars.CurGameLogic.GetTimeRemain() / 60;
            int second = (int)GlobalVars.CurGameLogic.GetTimeRemain() % 60;
            UIDrawer.Singleton.DrawNumber("MinutesLeft", -160, -114, min, "", 24);
            UIDrawer.Singleton.DrawSprite("TimeColon", -92, -114, "colon");
            UIDrawer.Singleton.DrawNumber("SecondsLeft", -100, -114, second, "", 24);

            m_timeBar.fillAmount = GlobalVars.CurGameLogic.GetTimeRemain() / GlobalVars.CurStageData.TimeLimit;
        }

        //绘制分数
        UIDrawer.Singleton.DrawNumber("ScoreText", 110, -114, GlobalVars.CurGameLogic.GetProgress(), "", 24, 7);
        UIDrawer.Singleton.DefaultAnchor = UIWindowManager.Anchor.TopLeft;
    }

    public void OnValueChange(float valueChange)
    {
        CapsConfig.Instance.GameSpeed = valueChange;
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
    }
}
