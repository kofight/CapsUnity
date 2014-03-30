using UnityEngine;
using System.Collections;

public class UIMainMenu : UIWindow 
{
    UIWindow m_mainMenuExtend;

    UIButton m_quitBtn;
    UIButton m_soundBtn;
    UIButton m_musicBtn;

    UISprite m_soundIcon;
    UISprite m_musicIcon;

    public override void OnCreate()
    {
        base.OnCreate();
        m_mainMenuExtend = UIWindowManager.Singleton.CreateWindow<UIWindow>("UIMainMenuExtend", UIWindowManager.Anchor.Left);

        m_quitBtn = m_mainMenuExtend.GetChildComponent<UIButton>("QuitBtn");
        m_soundBtn = m_mainMenuExtend.GetChildComponent<UIButton>("SoundBtn");
        m_musicBtn = m_mainMenuExtend.GetChildComponent<UIButton>("MusicBtn");

        m_soundIcon = m_mainMenuExtend.GetChildComponent<UISprite>("SoundIcon");
        m_musicIcon = m_mainMenuExtend.GetChildComponent<UISprite>("MusicIcon");

        m_mainMenuExtend.AddChildComponentMouseClick("QuitBtn", OnQuitClicked);
        m_mainMenuExtend.AddChildComponentMouseClick("HelpBtn", delegate()
        {
            UIWindowManager.Singleton.GetUIWindow<UIHowToPlay>().ShowWindow();
            m_mainMenuExtend.HideWindow();
        });

        m_mainMenuExtend.AddChildComponentMouseClick("SoundBtn", delegate()
        {
            GlobalVars.UseSFX = !GlobalVars.UseSFX;
            PlayerPrefs.SetInt("SFX", GlobalVars.UseSFX == true ? 1 : 0);
            RefreshIcons();
        });

        m_mainMenuExtend.AddChildComponentMouseClick("MusicBtn", delegate()
        {
            GlobalVars.UseMusic = !GlobalVars.UseMusic;
            PlayerPrefs.SetInt("Music", GlobalVars.UseMusic == true ? 1 : 0);
            if (GlobalVars.UseMusic == false)       //关闭音乐
            {
                UIToolkits.StopMusic();
            }
            else                                    //播放音乐
            {
                if (!UIToolkits.IsPlayingMusic())
                {
                    UIToolkits.PlayMusic(CapsConfig.CurAudioList.MapMusic);
                }
            }
            RefreshIcons();
        });

        m_mainMenuExtend.AddChildComponentMouseClick("HideBtn", delegate()
        {
            m_mainMenuExtend.HideWindow();      //隐藏窗口
            ShowWindow();
        });



        AddChildComponentMouseClick("MainBtn", delegate()
        {
            m_mainMenuExtend.ShowWindow();
            if (CapsApplication.Singleton.CurStateEnum != StateEnum.Game && !UIWindowManager.Singleton.GetUIWindow<UIMap>().Visible)        //Login画面
            {
                m_quitBtn.gameObject.SetActive(false);
            }
            else
            {
                m_quitBtn.gameObject.SetActive(true);
            }
            HideWindow();
        });
    }

    void RefreshIcons()
    {
        if (GlobalVars.UseMusic)
        {
            m_musicIcon.spriteName = "Button_MusicOn_MainMenu";
        }
        else
        {
            m_musicIcon.spriteName = "Button_MusicOff_MainMenu";
        }

        if (GlobalVars.UseSFX)
        {
            m_soundIcon.spriteName = "Button_SoundOn_MainMenu";
        }
        else
        {
            m_soundIcon.spriteName = "Button_SoundOff_MainMenu";
        }
    }

    public override void OnShow()
    {
        base.OnShow();
        RefreshIcons();
    }

    private void OnQuitClicked()
    {
        m_mainMenuExtend.HideWindow();
        if (CapsApplication.Singleton.CurStateEnum == StateEnum.Game)
        {
            if (UIWindowManager.Singleton.GetUIWindow<UIStageEditor>() != null && UIWindowManager.Singleton.GetUIWindow<UIStageEditor>().Visible)
            {
                UIWindowManager.Singleton.GetUIWindow<UIStageEditor>().HideWindow();
            }

            //若非时间关且没消耗步数，不用弹GameEnd界面，直接返回并恢复心
            if (GlobalVars.CurStageData.StepLimit > 0 && GameLogic.Singleton.PlayingStageData.StepLimit == GlobalVars.CurStageData.StepLimit)
            {
                GlobalVars.AddHeart(1);

                GameLogic.Singleton.PlayEndGameAnim();
                GameLogic.Singleton.ClearGame();
                CapsApplication.Singleton.ChangeState((int)StateEnum.Login);        //返回地图界面
                UIWindowManager.Singleton.GetUIWindow<UIMap>().ShowWindow();
                LoginState.Instance.CurFlow = TLoginFlow.LoginFlow_Map;         //切换流程到显示地图
            }
            else
            {
                UIWindowManager.Singleton.GetUIWindow<UIGameEnd>().ShowWindow();   
            }
        }
        else if (UIWindowManager.Singleton.GetUIWindow<UIMap>().Visible)
        {
            UIWindowManager.Singleton.GetUIWindow<UIMap>().HideWindow();
            UIWindowManager.Singleton.GetUIWindow<UILogin>().ShowWindow();
            LoginState.Instance.CurFlow = TLoginFlow.LoginFlow_LoginScreen;
        }
        else
        {
            UIWindowManager.Singleton.GetUIWindow<UIQuitConfirm>().ShowWindow();
        }
    }
}
