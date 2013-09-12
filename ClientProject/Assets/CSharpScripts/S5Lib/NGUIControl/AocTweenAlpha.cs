//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2012 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;
using System;

/// <summary>
/// Tween the object's color.
/// </summary>

[AddComponentMenu("NGUI/Aoc/Alpha")]
public class AocTweenAlpha : AocUITweener
{
    public float from = 0.0f;
    public float to = 1.0f;

    Transform mTrans;
    UIWidget[] mWidget;
    Material mMat;
    Light mLight;
    /// <summary>
    /// Current color.
    /// </summary>

    public float alpha
    {
        //get
        //{
        //    if( mWidget != null && mWidget[0] != null )
        //        return mWidget[0].color.a;
        //    return 0.0f;
        //}
        set
        {
            if (mWidget == null)
            {
                return;
            }

            Array.ForEach(mWidget,
                delegate(UIWidget w)
                {
                    if (null == w)
                    {
                        return;
                    }

                    if (w.tag == "IgnoreAlphaTween")      //略过持有IgnoreAlphaTween标记的
                    {

                    }
                    else
                    {
                        Color color = w.color;
                        color.a = value;
                        w.color = color;
                    }

                }
                );
        }
    }

    /// <summary>
    /// Find all needed components.
    /// </summary>

    new void Awake()
    {
        mWidget = GetComponentsInChildren<UIWidget>();
        base.Awake();
    }

    public void ResetWidget()
    {
        mWidget = GetComponentsInChildren<UIWidget>();
    }

    /// <summary>
    /// Interpolate and update the color.
    /// </summary>

    override protected void OnUpdate(float factor, bool isFinished)
    {
        alpha = from * (1f - factor) + to * factor;
    }

    /// <summary>
    /// Start the tweening operation.
    /// </summary>

    static public AocTweenColor Begin(GameObject go, float duration, Color color)
    {
        AocTweenColor comp = AocUITweener.Begin<AocTweenColor>(go, duration);
        comp.from = comp.color;
        comp.to = color;
        return comp;
    }
}