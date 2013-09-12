//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2012 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;
using System.Collections;

/// <summary>
/// Base class for all tweening operations.
/// </summary>

public abstract class AocUITweener : UITweener
{

    public void Awake()
    {
        eventReceiver = this.gameObject;
        callWhenFinished = "OnCallWhenFinished";
    }

    public System.Action CallWhenFinished;

    void OnCallWhenFinished()
    {
        if (null != CallWhenFinished)
        {
            System.Action action = CallWhenFinished;
            CallWhenFinished = null;
            action();
        }
    }



    void PlayForward()
    {
        Play( true );
    }


    void PlayBackward()
    {
        Play( false );
    }



}