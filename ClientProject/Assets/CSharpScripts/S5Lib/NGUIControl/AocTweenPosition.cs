//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2012 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;

/// <summary>
/// Tween the object's position.
/// </summary>

[AddComponentMenu("NGUI/Aoc/Position")]
public class AocTweenPosition : AocUITweener
{
	public Vector3 from;
	public Vector3 to;

	Transform mTrans;

	public Vector3 position { get { return mTrans.localPosition; } set { mTrans.localPosition = value; } }

    new void Awake() { mTrans = transform; base.Awake(); }

    override protected void OnUpdate(float factor, bool isFinished) 
    {
        mTrans.localPosition = from * (1f - factor) + to * factor; 
    }

	/// <summary>
	/// Start the tweening operation.
	/// </summary>

	static public AocTweenPosition Begin (GameObject go, float duration, Vector3 pos)
	{
		AocTweenPosition comp = AocUITweener.Begin<AocTweenPosition>(go, duration);
		comp.from = comp.position;
		comp.to = pos;
		return comp;
	}
}