﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
public class GameState : State
{
    public override void DoInitState()
    {
        base.DoInitState();
        UIWindowManager.Singleton.CreateWindow<UIGame>();
        UIWindowManager.Singleton.GetUIWindow<UIGame>().ShowWindow();
        //UIWindowManager.Singleton.GetUIWindow<UIMainMenu>().ShowWindow();
    }

    public override void DoDeInitState()
    {
        base.DoDeInitState();
        UIWindowManager.Singleton.GetUIWindow<UIGame>().HideWindow();
    }

    public override void Update()
    {
        base.Update();
    }

    public override void OnDragMove(Vector2 fingerPos, Vector2 delta)
    {
        base.OnDragMove(fingerPos, delta);
    }

    public override void OnTap(Vector2 fingerPos)
    {
        base.OnTap(fingerPos);
    }

    public override void OnPinchMove(Vector2 fingerPos1, Vector2 fingerPos2, float delta)
    {
        base.OnPinchMove(fingerPos1, fingerPos2, delta);
    }

    public override void OnPressDown(int fingerIndex, Vector2 fingerPos)
    {
        base.OnPressDown(fingerIndex, fingerPos);
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
