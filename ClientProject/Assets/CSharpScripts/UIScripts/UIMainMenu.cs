using UnityEngine;
using System.Collections;

public class UIMainMenu : UIWindow 
{
    UIWindow m_mainMenuExtend;
    public override void OnCreate()
    {
        base.OnCreate();
        m_mainMenuExtend = UIWindowManager.Singleton.CreateWindow<UIWindow>("UIMainMenuExtend", UIWindowManager.Anchor.BottomLeft);

        m_mainMenuExtend.AddChildComponentMouseClick("QuitBtn", OnQuitClicked);
        m_mainMenuExtend.AddChildComponentMouseClick("HelpBtn", delegate()
        {
            UIWindowManager.Singleton.GetUIWindow<UIHowToPlay>().ShowWindow();
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
            UIWindowManager.Singleton.GetUIWindow<UIGameEnd>().ShowWindow();   
        }
        else if (UIWindowManager.Singleton.GetUIWindow<UIMap>().Visible)
        {
            UIWindowManager.Singleton.GetUIWindow<UIMainMenu>().HideWindow();
            UIWindowManager.Singleton.GetUIWindow<UIMap>().HideWindow();
            UIWindowManager.Singleton.GetUIWindow<UILogin>().ShowWindow();
        }
        else
        {
            UIWindowManager.Singleton.GetUIWindow<UIQuitConfirm>().ShowWindow();
        }
    }
}
