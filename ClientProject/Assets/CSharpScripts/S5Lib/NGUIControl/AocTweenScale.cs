//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2012 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;

/// <summary>
/// Tween the object's local scale.
/// </summary>

[AddComponentMenu("NGUI/Aoc/Scale")]
public class AocTweenScale : AocUITweener
{
	public Vector3 from;
	public Vector3 to;
	public bool updateTable = false;

	Transform mTrans;
	UITable mTable;

	public Vector3 scale { get { return mTrans.localScale; } set { mTrans.localScale = value; } }

	new void Awake ()
	{
		mTrans = transform;
		if (updateTable) mTable = NGUITools.FindInParents<UITable>(gameObject);
        base.Awake();
	}

    override protected void OnUpdate(float factor, bool isFinished)
	{
		mTrans.localScale = from * (1f - factor) + to * factor;
		if (mTable != null) mTable.repositionNow = true;
	}

	/// <summary>
	/// Start the tweening operation.
	/// </summary>

    static public AocTweenScale Begin( GameObject go , float duration , Vector3 scale )
	{
        AocTweenScale comp = AocUITweener.Begin<AocTweenScale>( go , duration );
		comp.from = comp.scale;
		comp.to = scale;
		return comp;
	}
}