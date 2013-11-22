using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
public class GameState : State
{
    GameLogic m_gameLogic = new GameLogic();

    public override void DoInitState()
    {
        base.DoInitState();
        UIWindowManager.Singleton.CreateWindow<UIGameHead>(UIWindowManager.Anchor.Top);
        UIWindowManager.Singleton.CreateWindow<UIGameBottom>(UIWindowManager.Anchor.Bottom);
        UIWindowManager.Singleton.CreateWindow<UIWindow>("UIGameBackground", UIWindowManager.Anchor.Center);
        UIWindowManager.Singleton.CreateWindow<UIGameEnd>();
        UIWindowManager.Singleton.CreateWindow<UIRetry>();

        m_gameLogic.Init();
        m_gameLogic.StartGame();

        GlobalVars.CurGameLogic = m_gameLogic;
		
		UIWindowManager.Singleton.GetUIWindow("UIGameBackground").ShowWindow();
        UIWindowManager.Singleton.GetUIWindow<UIGameHead>().ShowWindow();
        UIWindowManager.Singleton.GetUIWindow<UIGameBottom>().ShowWindow();

        //UIWindowManager.Singleton.GetUIWindow<UIMainMenu>().ShowWindow();
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
        m_gameLogic.Update();
    }

    public override void OnQuitGame()
    {
        base.OnQuitGame();
    }

}
