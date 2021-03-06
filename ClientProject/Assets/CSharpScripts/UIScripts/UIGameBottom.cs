﻿using UnityEngine;
using System.Collections;

public class UIGameBottom : UIWindow
{
    UISprite[] m_starsSprites = new UISprite[3];
    UISprite m_progressSprite;
    static readonly int ProgressLenth = 386;
    static readonly int ProgressRealLenth = 396;
    static readonly int ProgressStartX = -95;

    UISprite stageBoard;
    UISlider m_speedSlider;
    UILabel m_speedLabel;
    UISprite m_stepTextSprite;
    UISprite m_timeBar;

    NumberDrawer m_scoreDrawer;
    NumberDrawer m_stepDrawer;

    GameObject m_timeNumber;
    NumberDrawer m_minNumber;
    NumberDrawer m_secNumber;

    Animation m_stepChangeAnim;
    Animation m_scoreChangeAnim;

    GameObject m_hurryStepParticle;
    GameObject m_hurryTimeParticle;

    int m_startCount = 0;

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

        m_stepChangeAnim = m_stepDrawer.GetComponent<Animation>();
        m_scoreChangeAnim = m_scoreDrawer.GetComponent<Animation>();

        m_hurryStepParticle = UIToolkits.FindChild(mUIObject.transform, "Effect_HurryStep").gameObject;
        m_hurryStepParticle.SetActive(false);

        m_hurryTimeParticle = UIToolkits.FindChild(mUIObject.transform, "Effect_HurryTime").gameObject;
        m_hurryTimeParticle.SetActive(false);
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
            m_hurryStepParticle.SetActive(false);
        }
        else
        {
            stageBoard.spriteName = "StepBoard";
            m_timeBar.gameObject.SetActive(false);

            m_stepDrawer.gameObject.SetActive(true);
            m_timeNumber.SetActive(false);

            m_hurryTimeParticle.SetActive(false);

            OnChangeStep(GameLogic.Singleton.PlayingStageData.StepLimit);
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

        OnChangeProgress(GameLogic.Singleton.GetProgress());
    }
	
	public void Reset()
	{
        m_startCount = 0;

		if (GlobalVars.CurStageData.StarScore[2] > 0)
        {
            for (int i = 0; i < 3; ++i)
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

        if (GlobalVars.CurStageData.TimeLimit > 0)          //限制时间的关卡
        {
            int min = (int)GameLogic.Singleton.GetTimeRemain() / 60;
            int second = (int)GameLogic.Singleton.GetTimeRemain() % 60;
            m_minNumber.SetNumberRapid(min);
            m_secNumber.SetNumberRapid(second);

            if (GameLogic.Singleton.GetTimeRemain() > 0.01 && GameLogic.Singleton.GetTimeRemain() <= 15 && GameLogic.Singleton.GetGameFlow() == TGameFlow.EGameState_Playing)       //小于15秒播粒子
            {
                if (!m_hurryTimeParticle.activeSelf)
                {
                    m_hurryTimeParticle.SetActive(true);
                }
            }
            else                    //大于15秒关闭粒子
            {
                if (m_hurryTimeParticle.activeSelf)
                {
                    m_hurryTimeParticle.SetActive(false);
                }
            }
            m_timeBar.fillAmount = GameLogic.Singleton.GetTimeRemain() / GlobalVars.CurStageData.TimeLimit;
        }
    }

    public void OnValueChange()
    {
        CapsConfig.Instance.GameSpeed = UISlider.current.value;
        m_speedLabel.text = CapsConfig.Instance.GameSpeed.ToString();
    }

    public void OnChangeStep(int curStep)
    {
        m_stepDrawer.SetNumberRapid(curStep);
        m_stepChangeAnim.Play();

        if (curStep <= 5 && curStep >0 && GameLogic.Singleton.GetGameFlow() == TGameFlow.EGameState_Playing)       //1到5步时播粒子
        {
            if (!m_hurryStepParticle.activeSelf)
            {
                m_hurryStepParticle.SetActive(true);
            }
        }
        else                    //大于5步时关闭粒子
        {
            if (m_hurryStepParticle.activeSelf)
            {
                m_hurryStepParticle.SetActive(false);
            }
        }
    }

    public void OnChangeProgress(int progress)
    {
        for (int i = 0; i < 3; ++i)
        {
            if (progress >= GlobalVars.CurStageData.StarScore[i])
            {
                if (m_startCount < i + 1)
                {
                    m_starsSprites[i].spriteName = "LightStar";
                    ParticleSystem par = m_starsSprites[i].GetComponentInChildren<ParticleSystem>();
                    if (par != null)
                    {
                        par.Play();
                        if (GlobalVars.UseSFX)
                        {
                            if (i == 0)
                                NGUITools.PlaySound(CapsConfig.CurAudioList.GetStar1Clip);
                            if (i == 1)
                                NGUITools.PlaySound(CapsConfig.CurAudioList.GetStar2Clip);
                            if (i == 2)
                                NGUITools.PlaySound(CapsConfig.CurAudioList.GetStar3Clip);
                        }
                    }
					m_startCount = i + 1;
                }
            }
        }
		if(progress == 0)
		{
			m_scoreDrawer.SetNumberRapid(progress);
		}
		else
		{
			m_scoreChangeAnim.Play();
        	m_scoreDrawer.SetNumber(progress);
		}

        float completeRatio = 0.0f;
        if (GlobalVars.CurStageData.StarScore[2] > 0)
        {
            completeRatio = (float)GameLogic.Singleton.GetProgress() / GlobalVars.CurStageData.StarScore[2];
        }
        completeRatio = Mathf.Min(1.0f, completeRatio);

        if (completeRatio >= 1.0f)
        {
            completeRatio = 1.0f;
        }
        else
        {
            completeRatio = (completeRatio * ProgressLenth) / ProgressRealLenth;
        }

        if (completeRatio != 0.0f)
        {
            m_progressSprite.gameObject.SetActive(true);
            m_progressSprite.fillAmount = completeRatio;
        }
    }
}
