﻿using UnityEngine;
using System.Collections;

public class UILogin : UIWindow 
{
    UIToggle m_developerMode;
    UIToggle m_englishVersion;
    UILabel m_testLabel;            //用来显示测试信息的label
    GameObject m_debugBoard;        //调试面板

    int m_clickDebugCount = 0;      //用来记录点击隐藏的debug按钮几次，点击3次才起作用

    public override void OnCreate()
    {
        base.OnCreate();

        AddChildComponentMouseClick("PlayBtn", OnPlayBtnClick);
        AddChildComponentMouseClick("ClearBtn", delegate()
        {
            PlayerPrefs.DeleteAll();            //删除进度
        });

        AddChildComponentMouseClick("RecoverHeartBtn", delegate()
        {
            GlobalVars.AddHeart(5);
        });

        AddChildComponentMouseClick("AddCoinBtn", delegate()
        {
            GlobalVars.Coins += 10;
            PlayerPrefs.SetInt("Coins", GlobalVars.Coins);
        });

        AddChildComponentMouseClick("DebugBtn", delegate()
        {
            ++m_clickDebugCount;
            if (m_clickDebugCount >= 3)
            {
                m_debugBoard.SetActive(true);
            }
        });

        m_developerMode = UIToolkits.FindComponent<UIToggle>(mUIObject.transform, "DeveloperCheck");
        m_developerMode.value = false;

        m_englishVersion = UIToolkits.FindComponent<UIToggle>(mUIObject.transform, "EnglishCheck");
        m_englishVersion.value = Localization.instance.currentLanguage == "English";
        EventDelegate.Set(m_englishVersion.onChange, delegate()
        {
            if (m_englishVersion.value)
            {
                if (Localization.instance != null)
                {
                    Localization.instance.currentLanguage = "English";
                }
            }
            else
            {
                if (Localization.instance != null)
                {
                    Localization.instance.currentLanguage = "Chinese";
                }
            }
        });

        UIToggle musicCheck = UIToolkits.FindComponent<UIToggle>(mUIObject.transform, "MusicCheck");
        musicCheck.value = GlobalVars.UseMusic;
        EventDelegate.Set(musicCheck.onChange, delegate()
        {
            GlobalVars.UseMusic = UIToggle.current.value;
            PlayerPrefs.SetInt("Music", GlobalVars.UseMusic == true ? 1 : 0);
            if (GlobalVars.UseMusic == false)       //关闭音乐
            {
                UIToolkits.StopMusic();
            }
            else                                    //播放音乐
            {
                UIToolkits.PlayMusic(CapsConfig.CurAudioList.MapMusic);
            }
        });

        UIToggle soundCheck = UIToolkits.FindComponent<UIToggle>(mUIObject.transform, "SoundCheck");
        soundCheck.value = GlobalVars.UseSFX;
        EventDelegate.Set(soundCheck.onChange, delegate()
        {
            GlobalVars.UseSFX = UIToggle.current.value;
            PlayerPrefs.SetInt("SFX", GlobalVars.UseSFX == true ? 1 : 0);
        });

        m_testLabel = GetChildComponent<UILabel>("TestNotice");
        m_debugBoard = mUIObject.transform.FindChild("DebugBoard").gameObject;
    }
    public override void OnShow()
    {
        base.OnShow();
        if (CapsApplication.Singleton.GetPlayTime() > 20 * 3600)            //如果游戏时间超过20小时
        {
            UIButton button = GetChildComponent<UIButton>("PlayBtn");
            button.gameObject.SetActive(false);                                       //让按钮无效化
            m_testLabel.gameObject.SetActive(true);
        }
        else
        {
            m_testLabel.gameObject.SetActive(false);
        }

        if (m_clickDebugCount >= 3)
        {
            m_debugBoard.SetActive(true);
        }
        else
        {
            m_debugBoard.SetActive(false);
        }
    }

    public override void OnShowEffectPlayOver()
    {
        base.OnShowEffectPlayOver();
		UIButton playBtn = GetChildComponent<UIButton>("PlayBtn");
		playBtn.GetComponent<UIEffectPlayer>().Delay = 0;
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        UILabel timeLabel = GetChildComponent<UILabel>("UsedTimeLabel");
        if (timeLabel != null)
        {
            int playSecond = (int)CapsApplication.Singleton.GetPlayTime();
            timeLabel.text = string.Format("{0}:{1}:{2}", playSecond / 3600, (playSecond % 3600) / 60, playSecond % 60);
        }
    }

    private void OnPlayBtnClick()
    {
        HideWindow();
		GlobalVars.DeveloperMode = m_developerMode.value;
		
        if (GlobalVars.AvailabeStageCount == 1)
        {
            GlobalVars.UseHeart();      //使用一颗心
            GlobalVars.CurStageNum = 1;
            GlobalVars.CurStageData = StageData.CreateStageData();
            GlobalVars.LastStage = GlobalVars.CurStageNum;
            GlobalVars.CurStageData.LoadStageData(GlobalVars.CurStageNum);
            UIWindowManager.Singleton.GetUIWindow("UILoading").ShowWindow(
            delegate()
            {
                CapsApplication.Singleton.ChangeState((int)StateEnum.Game);
            }
            );
        }
        else
        {
            UIWindowManager.Singleton.GetUIWindow<UIMap>().ShowWindow();
            if (GlobalVars.DeveloperMode)
            {
                UIWindowManager.Singleton.GetUIWindow<UIMap>().RefreshButtons();
            }
            LoginState.Instance.CurFlow = TLoginFlow.LoginFlow_Map;         //切换流程到显示地图
        }
    }
}
