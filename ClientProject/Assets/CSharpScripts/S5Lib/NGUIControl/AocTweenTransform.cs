//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2012 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;

/// <summary>
/// Tween the object's position, rotation and scale.
/// </summary>

[AddComponentMenu("NGUI/Aoc/Transform")]
public class AocTweenTransform : AocUITweener
{
	public Transform from;
	public Transform to;

	Transform mTrans;

    new void Awake() { mTrans = transform; base.Awake(); }

    override protected void OnUpdate(float factor, bool isFinished)
	{
		if (from != null && to != null)
		{
			mTrans.position = from.position * (1f - factor) + to.position * factor;
			mTrans.localScale = from.localScale * (1f - factor) + to.localScale * factor;
			mTrans.rotation = Quaternion.Slerp(from.rotation, to.rotation, factor);
		}
	}

	/// <summary>
	/// Start the tweening operation.
	/// </summary>

    static public AocTweenTransform Begin( GameObject go , float duration , Transform from , Transform to )
	{
        AocTweenTransform comp = AocUITweener.Begin<AocTweenTransform>( go , duration );
		comp.from = from;
		comp.to = to;
		return comp;
	}
}