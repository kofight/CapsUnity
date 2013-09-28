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
        UIWindowManager.Singleton.CreateWindow<UIGame>();
        UIWindowManager.Singleton.CreateWindow<UIStageEditor>(UIWindowManager.Anchor.Right);
        UIWindowManager.Singleton.CreateWindow<UIGameEnd>();
        UIWindowManager.Singleton.CreateWindow<UIRetry>();
        UIWindowManager.Singleton.GetUIWindow<UIGame>().ShowWindow();

        m_gameLogic.Init();
        m_gameLogic.StartGame();

        GlobalVars.CurGameLogic = m_gameLogic;

        //UIWindowManager.Singleton.GetUIWindow<UIMainMenu>().ShowWindow();
    }

    public override void DoDeInitState()
    {
        base.DoDeInitState();
        UIWindowManager.Singleton.GetUIWindow<UIGame>().HideWindow();
        m_gameLogic.ClearGame();
    }

    public override void Update()
    {
        base.Update();
        m_gameLogic.Update();
    }

    public override void OnDragMove(Vector2 fingerPos, Vector2 delta)
    {
        base.OnDragMove(fingerPos, delta);
        m_gameLogic.OnTouchMove((int)fingerPos.x, (int)(fingerPos.y));
    }

    public override void OnTap(Vector2 fingerPos)
    {
        base.OnTap(fingerPos);
        m_gameLogic.OnTap((int)fingerPos.x, (int)(fingerPos.y));
    }

    public override void OnPinchMove(Vector2 fingerPos1, Vector2 fingerPos2, float delta)
    {
        base.OnPinchMove(fingerPos1, fingerPos2, delta);
    }

    public override void OnPressDown(int fingerIndex, Vector2 fingerPos)
    {
        base.OnPressDown(fingerIndex, fingerPos);
        m_gameLogic.OnTouchBegin((int)fingerPos.x, (int)(fingerPos.y));
    }

    public override void OnPressUp(int fingerIndex, Vector2 fingerPos, float holdTime)
    {
        base.OnPressUp(fingerIndex, fingerPos, holdTime);
    }

    public override void OnQuitGame()
    {
        base.OnQuitGame();
    }

}
