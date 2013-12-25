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
            new GameLogic();
        }

        UIWindowManager.Singleton.CreateWindow<UIGameHead>(UIWindowManager.Anchor.Top);
        UIWindowManager.Singleton.CreateWindow<UIGameBottom>(UIWindowManager.Anchor.Bottom);
        UIWindowManager.Singleton.CreateWindow<UIWindow>("UIGameBackground", UIWindowManager.Anchor.Center);
        UIWindowManager.Singleton.CreateWindow<UIGameEnd>();
        UIWindowManager.Singleton.CreateWindow<UIRetry>();
		UIWindowManager.Singleton.CreateWindow<UIPurchase>();
		UIWindowManager.Singleton.CreateWindow<UIUseItem>();
        GameLogic.Singleton.Init();
        m_bGameLogicStarted = false;
		UIWindowManager.Singleton.GetUIWindow("UIGameBackground").ShowWindow();
        UIWindowManager.Singleton.GetUIWindow<UIGameHead>().ShowWindow();
        UIWindowManager.Singleton.GetUIWindow<UIGameBottom>().ShowWindow();

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
        GameLogic.Singleton.ClearGame();
    }

    public override void Update()
    {
        base.Update();
        if (m_waitLoadingStart > 0 && Time.realtimeSinceStartup - m_waitLoadingStart > 0.5f)
        {
            m_waitLoadingStart = 0.0f;
            UIWindowManager.Singleton.GetUIWindow("UILoading").HideWindow(delegate()
            {
                UIWindowManager.Singleton.GetUIWindow<UIMainMenu>().ShowWindow();
                GameLogic.Singleton.StartGame();
                m_bGameLogicStarted = true;
            });
        }
        if (m_bGameLogicStarted)
        {
            GameLogic.Singleton.Update();
        }
    }

    public override void OnBackKey()
    {
        base.OnBackKey();
        if (UIWindowManager.Singleton.GetUIWindow<UIGameEnd>().Visible)
        {
            if (GameLogic.Singleton.GetGameFlow() != TGameFlow.EGameState_End)
            {
                UIWindowManager.Singleton.GetUIWindow<UIGameEnd>().HideWindow();
                return;
            }
        }
        if (UIWindowManager.Singleton.GetUIWindow<UIRetry>().Visible)
        {
            UIWindowManager.Singleton.GetUIWindow<UIRetry>().HideWindow(delegate()
            {
                CapsApplication.Singleton.ChangeState((int)StateEnum.Login);        //返回地图界面
            });

            UIWindowManager.Singleton.GetUIWindow<UIMap>().ShowWindow();
            return;
        }
        UIWindowManager.Singleton.GetUIWindow<UIGameEnd>().ShowWindow();
    }

}
