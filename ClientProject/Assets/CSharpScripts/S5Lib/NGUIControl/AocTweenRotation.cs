//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2012 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;

/// <summary>
/// Tween the object's rotation.
/// </summary>

[AddComponentMenu("NGUI/Aoc/Rotation")]
public class AocTweenRotation : AocUITweener
{
	public Vector3 from;
	public Vector3 to;

	Transform mTrans;

	public Quaternion rotation { get { return mTrans.localRotation; } set { mTrans.localRotation = value; } }

    new void Awake() { mTrans = transform; base.Awake(); }

    override protected void OnUpdate(float factor, bool isFinished)
	{
		mTrans.localRotation = Quaternion.Slerp(Quaternion.Euler(from), Quaternion.Euler(to), factor);
	}

	/// <summary>
	/// Start the tweening operation.
	/// </summary>

    static public AocTweenRotation Begin( GameObject go , float duration , Quaternion rot )
	{
        AocTweenRotation comp = AocUITweener.Begin<AocTweenRotation>( go , duration );
		comp.from = comp.rotation.eulerAngles;
		comp.to = rot.eulerAngles;
		return comp;
	}
}