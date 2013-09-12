//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2012 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;
using System;

/// <summary>
/// Tween the object's color.
/// </summary>

[AddComponentMenu("NGUI/Aoc/Color")]
public class AocTweenColor : AocUITweener
{
	public Color from = Color.white;
	public Color to = Color.white;

	Transform mTrans;
	UIWidget[] mWidget;
	Material mMat;
	Light mLight;

	/// <summary>
	/// Current color.
	/// </summary>

	public Color color
	{
		get
		{
            if( mWidget != null && mWidget[0] != null)
                return mWidget[0].color;
			if (mLight != null) return mLight.color;
			if (mMat != null) return mMat.color;
			return Color.black;
		}
		set
		{
            if( mWidget != null )
            {
                Array.ForEach( mWidget , delegate( UIWidget w ) { if( null != w ) { w.color = value; } } );
            }
			if (mMat != null) mMat.color = value;

			if (mLight != null)
			{
				mLight.color = value;
				mLight.enabled = (value.r + value.g + value.b) > 0.01f;
			}
		}
	}

	/// <summary>
	/// Find all needed components.
	/// </summary>

	new void Awake ()
	{
        mWidget = GetComponentsInChildren<UIWidget>();
		Renderer ren = renderer;
		if (ren != null) mMat = ren.material;
		mLight = light;
        base.Awake();
	}

	/// <summary>
	/// Interpolate and update the color.
	/// </summary>

    override protected void OnUpdate(float factor, bool isFinished) 
    {
        color = from * (1f - factor) + to * factor; 
    }

	/// <summary>
	/// Start the tweening operation.
	/// </summary>

    static public AocTweenColor Begin( GameObject go , float duration , Color color )
	{
        AocTweenColor comp = AocUITweener.Begin<AocTweenColor>( go , duration );
		comp.from = comp.color;
		comp.to = color;
		return comp;
	}
}