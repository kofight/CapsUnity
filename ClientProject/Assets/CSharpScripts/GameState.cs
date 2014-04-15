using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
public class GameState : State
{
    bool m_bGameLogicStarted = false;
    float m_waitLoadingStart;

    public override void DoInitState()
    {
        base.DoInitState();

        if (GameLogic.Singleton == null)
        {
			UIWindowManager.Singleton.CreateWindow<UIGameHead>(UIWindowManager.Anchor.Top);
			UIWindowManager.Singleton.CreateWindow<UIGameBottom>(UIWindowManager.Anchor.Bottom);
			UIWindowManager.Singleton.CreateWindow<UIWindow>("UIGameBackground", UIWindowManager.Anchor.Center);
			UIWindowManager.Singleton.CreateWindow<UIGameEnd>();
			UIWindowManager.Singleton.CreateWindow<UIRetry>();
			UIWindowManager.Singleton.CreateWindow<UIPurchaseNoTarget>();
            UIWindowManager.Singleton.CreateWindow<UIPurchaseTarget>(UIWindowManager.Anchor.Bottom);

            new GameLogic();
        }

        
        GameLogic.Singleton.Init();
        m_bGameLogicStarted = false;
		UIWindowManager.Singleton.GetUIWindow("UIGameBackground").ShowWindow();

        if (GlobalVars.UseMusic)
        {
            UIToolkits.PlayMusic(CapsConfig.CurAudioList.GameMusic);
        }

        m_waitLoadingStart = Time.realtimeSinceStartup;
    }

    public override void DoDeInitState()
    {
        base.DoDeInitState();
        UIWindowManager.Singleton.GetUIWindow<UIGameHead>().HideWindow();
        UIWindowManager.Singleton.GetUIWindow<UIGameBottom>().HideWindow();
        UIWindowManager.Singleton.GetUIWindow("UIGameBackground").HideWindow();
    }

    public override void Update()
    {
        base.Update();
        if (!m_bGameLogicStarted && m_waitLoadingStart > 0 && Time.realtimeSinceStartup - m_waitLoadingStart > 0.5f)
        {
            m_waitLoadingStart = 0.0f;
            UIWindowManager.Singleton.GetUIWindow("UILoading").HideWindow(delegate()
            {
                UIWindowManager.Singleton.GetUIWindow<UIDialog>().TriggerDialog(GlobalVars.CurStageNum, DialogTriggerPos.StageStart, delegate()
                {
                    UIWindowManager.Singleton.GetUIWindow<UIGameHead>().ShowWindow();
                    UIWindowManager.Singleton.GetUIWindow<UIGameBottom>().ShowWindow();
                    UIWindowManager.Singleton.GetUIWindow<UIMainMenu>().ShowWindow();
                    GameLogic.Singleton.PlayStartEffect();
                    m_bGameLogicStarted = true;
                });
            });
        }
        if (m_bGameLogicStarted)
        {
            GameLogic.Singleton.Update();
        }
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        if (m_bGameLogicStarted)
        {
            GameLogic.Singleton.FixedUpdate();
        }
    }

    public override void OnBackKey()
    {
        base.OnBackKey();

        //////////////////前面弹出窗口//////////////////////////////////////

        if (UIWindowManager.Singleton.GetUIWindow<UIMessageBox>().Visible)
        {
            UIWindowManager.Singleton.GetUIWindow<UIMessageBox>().HideWindow();
            return;
        }

        if (UIWindowManager.Singleton.GetUIWindow<UIDialog>().Visible)
        {
            UIWindowManager.Singleton.GetUIWindow<UIDialog>().EndDialog();
            return;
        }

        if (UIWindowManager.Singleton.GetUIWindow<UIHowToPlay>().Visible)
        {
            UIWindowManager.Singleton.GetUIWindow<UIHowToPlay>().OnClose();
            return;
        }

        if (UIWindowManager.Singleton.GetUIWindow<UINoMoreHearts>().Visible)
        {
            UIWindowManager.Singleton.GetUIWindow<UINoMoreHearts>().Close();
            return;
        }

        if (UIWindowManager.Singleton.GetUIWindow<UIPurchaseNoTarget>().Visible)
        {
            UIWindowManager.Singleton.GetUIWindow<UIPurchaseNoTarget>().OnCancelClicked();
            return;
        }

        if (UIWindowManager.Singleton.GetUIWindow<UIPurchaseTarget>().Visible)
        {
            UIWindowManager.Singleton.GetUIWindow<UIPurchaseTarget>().OnCancelClicked();
            return;
        }

        if (UIWindowManager.Singleton.GetUIWindow<UIPurchaseNotEnoughMoney>().Visible)
        {
            UIWindowManager.Singleton.GetUIWindow<UIPurchaseNotEnoughMoney>().OnCloseBtn();
            return;
        }

        if (UIWindowManager.Singleton.GetUIWindow<UIStore>().Visible)
        {
            UIWindowManager.Singleton.GetUIWindow<UIStore>().OnCloseBtn();
            return;
        }

        if (UIWindowManager.Singleton.GetUIWindow<UIStageInfo>().Visible)
        {
            UIWindowManager.Singleton.GetUIWindow<UIStageInfo>().OnCloseClicked();
            return;
        }

        if (UIWindowManager.Singleton.GetUIWindow<UIFTUE>() != null && UIWindowManager.Singleton.GetUIWindow<UIFTUE>().Visible)
        {
            //GameLogic.Singleton.StopFTUE();
            //UIWindowManager.Singleton.GetUIWindow<UIFTUE>().EndFTUE();
            return;
        }

        if (UIWindowManager.Singleton.GetUIWindow("UIMainMenuExtend").Visible)      //若主菜单开启状态
        {
            UIWindowManager.Singleton.GetUIWindow("UIMainMenuExtend").HideWindow();
            UIWindowManager.Singleton.GetUIWindow<UIMainMenu>().ShowWindow();       //显示主菜单按钮
            return;
        }

        if (UIWindowManager.Singleton.GetUIWindow<UIGameEnd>().Visible)
        {
            if (GameLogic.Singleton.GetGameFlow() != TGameFlow.EGameState_End)
            {
                UIWindowManager.Singleton.GetUIWindow<UIGameEnd>().HideWindow();
                UIWindowManager.Singleton.GetUIWindow<UIMainMenu>().ShowWindow();       //显示主菜单按钮
                GameLogic.Singleton.ResumeGame();
                return;
            }
        }
        if (UIWindowManager.Singleton.GetUIWindow<UIRetry>().Visible)
        {
            GameLogic.Singleton.ClearGame();
            UIWindowManager.Singleton.GetUIWindow<UIRetry>().HideWindow(delegate()
            {
                CapsApplication.Singleton.ChangeState((int)StateEnum.Login);        //返回地图界面
            });

            UIWindowManager.Singleton.GetUIWindow<UIMap>().ShowWindow();
            LoginState.Instance.CurFlow = TLoginFlow.LoginFlow_Map;         //切换流程到显示地图
            return;
        }
        if (UIWindowManager.Singleton.GetUIWindow<UIMainMenu>().Visible)
        {
            UIWindowManager.Singleton.GetUIWindow<UIMainMenu>().OnQuitClicked();
        }
    }

}
