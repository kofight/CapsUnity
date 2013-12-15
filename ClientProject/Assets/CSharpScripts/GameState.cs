using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
public class GameState : State
{
    GameLogic m_gameLogic = new GameLogic();
    bool m_bGameLogicStarted = false;

    public override void DoInitState()
    {
        base.DoInitState();
        
        UIWindowManager.Singleton.CreateWindow<UIGameHead>(UIWindowManager.Anchor.Top);
        UIWindowManager.Singleton.CreateWindow<UIGameBottom>(UIWindowManager.Anchor.Bottom);
        UIWindowManager.Singleton.CreateWindow<UIWindow>("UIGameBackground", UIWindowManager.Anchor.Center);
        UIWindowManager.Singleton.CreateWindow<UIGameEnd>();
        UIWindowManager.Singleton.CreateWindow<UIRetry>();
		UIWindowManager.Singleton.CreateWindow<UIPurchase>();
		UIWindowManager.Singleton.CreateWindow<UIUseItem>();
        m_gameLogic.Init();
        m_bGameLogicStarted = false;
        GlobalVars.CurGameLogic = m_gameLogic;
		UIWindowManager.Singleton.GetUIWindow("UIGameBackground").ShowWindow();
        UIWindowManager.Singleton.GetUIWindow<UIGameHead>().ShowWindow();
        UIWindowManager.Singleton.GetUIWindow<UIGameBottom>().ShowWindow();

        UIToolkits.PlayMusic(CapsConfig.CurAudioList.GameMusic);

        UIWindowManager.Singleton.GetUIWindow("UILoading").HideWindow(delegate()
        {
            m_gameLogic.StartGame();
            m_bGameLogicStarted = true;
        });
    }

    public override void DoDeInitState()
    {
        base.DoDeInitState();
        UIWindowManager.Singleton.GetUIWindow<UIGameHead>().HideWindow();
        UIWindowManager.Singleton.GetUIWindow<UIGameBottom>().HideWindow();
        UIWindowManager.Singleton.GetUIWindow("UIGameBackground").HideWindow();
        m_gameLogic.ClearGame();
    }

    public override void Update()
    {
        base.Update();
        if (m_bGameLogicStarted)
        {
            m_gameLogic.Update();
        }
    }

    public override void OnQuitGame()
    {
        base.OnQuitGame();
    }

}
