using UnityEngine;
using System.Collections;

public class UIMainMenu : UIWindow 
{
    UIWindow m_mainMenuExtend;

    UIButton m_quitBtn;
    UIButton m_optionBtn;

    public override void OnCreate()
    {
        base.OnCreate();
        m_mainMenuExtend = UIWindowManager.Singleton.CreateWindow<UIWindow>("UIMainMenuExtend", UIWindowManager.Anchor.BottomLeft);

        m_quitBtn = m_mainMenuExtend.GetChildComponent<UIButton>("QuitBtn");
        m_optionBtn = m_mainMenuExtend.GetChildComponent<UIButton>("OptionBtn");

        m_mainMenuExtend.AddChildComponentMouseClick("QuitBtn", OnQuitClicked);
        m_mainMenuExtend.AddChildComponentMouseClick("HelpBtn", delegate()
        {
            UIWindowManager.Singleton.GetUIWindow<UIHowToPlay>().ShowWindow();
            m_mainMenuExtend.HideWindow();
        });

        m_mainMenuExtend.AddChildComponentMouseClick("OptionBtn", delegate()
        {
            UIWindowManager.Singleton.GetUIWindow<UIOption>().ShowWindow();
            m_mainMenuExtend.HideWindow();
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
                m_optionBtn.gameObject.SetActive(true);
            }
            else
            {
                m_quitBtn.gameObject.SetActive(true);
                m_optionBtn.gameObject.SetActive(false);
            }
            HideWindow();
        });
    }
    public override void OnShow()
    {
        base.OnShow();
    }
    public override void OnUpdate()
    {
        base.OnUpdate();
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
        }
        else
        {
            UIWindowManager.Singleton.GetUIWindow<UIQuitConfirm>().ShowWindow();
        }
    }
}
