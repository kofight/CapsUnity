using UnityEngine;
using System.Collections;

public class UIGameBottom : UIWindowNGUI
{
    UISprite[] m_starsSprites = new UISprite[3];
    UISprite m_progressSprite;
    static readonly int ProgressLenth = 348;
    static readonly int ProgressStartX = -89;

    UISprite stageBoard;

    public override void OnCreate()
    {
        base.OnCreate();

        for (int i = 0; i < 3; ++i )
        {
            m_starsSprites[i] = GetChildComponent<UISprite>("Star" + (i + 1));      //查找sprite
        }
        m_progressSprite = GetChildComponent<UISprite>("Progress");      //查找sprite

        stageBoard = GetChildComponent<UISprite>("StageBoard");
    }
    public override void OnShow()
    {
        base.OnShow();
        if (GlobalVars.CurGameLogic.PlayingStageData.TimeLimit > 0)
        {
            stageBoard.spriteName = "TimeBoard";
        }
        else
        {
            stageBoard.spriteName = "StepBoard";
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
            UIDrawer.Singleton.DrawNumber("SetpLimit", -134, -170, GlobalVars.CurGameLogic.PlayingStageData.StepLimit, "BaseNum", 24);
        }
        if (GlobalVars.CurStageData.TimeLimit > 0)          //限制时间的关卡
        {
            int ticks = (int)((System.DateTime.Now.Ticks - GlobalVars.GetHeartTime.Ticks) / 10000);
            int ticksToGetHeart = CapsConfig.Instance.GetHeartInterval * 1000 - ticks;
            int min = (int)GlobalVars.CurGameLogic.GetTimeRemain() / 60;
            int second = (int)GlobalVars.CurGameLogic.GetTimeRemain() % 60;
            UIDrawer.Singleton.DrawNumber("MinutesLeft", -180, -170, min, "", 24);
            UIDrawer.Singleton.DrawSprite("TimeColon", -112, -170, "colon");
            UIDrawer.Singleton.DrawNumber("SecondsLeft", -120, -170, second, "", 24);
        }

        //绘制分数
        UIDrawer.Singleton.DrawNumber("ScoreText", 90, -170, GlobalVars.CurGameLogic.GetProgress(), "BaseNum", 24, 7);
        UIDrawer.Singleton.DefaultAnchor = UIWindowManager.Anchor.TopLeft;
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
