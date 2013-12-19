using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
public class GameState : State
{
    bool m_bGameLogicStarted = false;

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

        UIWindowManager.Singleton.GetUIWindow("UILoading").HideWindow(delegate()
        {
            UIWindowManager.Singleton.GetUIWindow<UIMainMenu>().ShowWindow();
            GameLogic.Singleton.StartGame();
            m_bGameLogicStarted = true;
        });
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
        if (m_bGameLogicStarted)
        {
            GameLogic.Singleton.Update();
        }
    }

    public override void OnQuitGame()
    {
        base.OnQuitGame();
    }

}
