using UnityEngine;
using System.Collections;
using System;

[AddComponentMenu("NGUI/Aoc/ShowUIEffect")]
public class AocShowUIEffect : UIEffectPlayer 
{
    public Vector2 from;
    public Vector2 to;
    public float scaleFrom = 0.4f;
    public float scaleTo = 1.0f;
    public bool moveBack = true;
    public bool useAlpha = true;
	public float moveTime = 0.2f;
	public float scaleTime = 0.25f;
    UIWidget[] mWidget;

	public override void OnShow()
	{
        mWidget = GetComponentsInChildren<UIWidget>();
        string easeType = "easeOutBack";
        if (!moveBack)
        {
            easeType = "easeOutExpo";
        }
#if UNITY_IPHONE || UNITY_ANDROID
        easeType = "easeOutExpo";
#endif
        bool needMove = (to != from || to != new Vector2(0.0f, 0.0f));
        bool needScale = (scaleTo != scaleFrom);
		if(mCurPlayingExcludeEffect != UIEffectPlayer.PlayingExcludeEffectType.None)
		{
			iTween.Stop(transform.gameObject);
			transform.localPosition = new Vector3(to.x, to.y, transform.localPosition.z);
			transform.localScale = new Vector3(scaleTo, scaleTo, scaleTo);
			OnComplete();
			return;
		}
        if (needMove)
        {
			if(mCurPlayingExcludeEffect == UIEffectPlayer.PlayingExcludeEffectType.None)		//start from beginning
			{
				transform.localPosition = new Vector3(from.x, from.y, transform.localPosition.z);
			}
            if (needScale)
            {
                iTween.MoveTo(transform.gameObject, iTween.Hash("position", new Vector3(to.x, to.y, transform.localPosition.z), "easeType", "easeOutExpo", "time", moveTime));
            }
            else
            {
                iTween.MoveTo(transform.gameObject, iTween.Hash("position", new Vector3(to.x, to.y, transform.localPosition.z), "oncomplete", "OnComplete", "oncompletetarget", this.gameObject, "easeType", "easeOutExpo", "time", moveTime));
            }
        }

        if (scaleTo != scaleFrom)
        {
			if(mCurPlayingExcludeEffect == UIEffectPlayer.PlayingExcludeEffectType.None)		//start from beginning
			{
            	transform.localScale = new Vector3(scaleFrom, scaleFrom, 1.0f);
			}
            iTween.ScaleTo(transform.gameObject, iTween.Hash("scale", new Vector3(scaleTo, scaleTo, 1.0f), "oncomplete", "OnComplete", "oncompletetarget", this.gameObject, "easeType", easeType, "time", scaleTime));
        }
		this.enabled = true;
		base.OnShow();
	}
	
	public override void OnHide()
	{
		bool bCallBackComplete = false;
		if(mCurPlayingExcludeEffect != UIEffectPlayer.PlayingExcludeEffectType.None)
		{
			iTween.Stop(transform.gameObject);
			transform.localPosition = new Vector3(from.x, from.y, transform.localPosition.z);
			transform.localScale = new Vector3(scaleFrom, scaleFrom, scaleFrom);
			OnComplete();
			base.OnHide();
			return;
		}
        if (to != from || from != new Vector2(0.0f, 0.0f))
        {
			if(mCurPlayingExcludeEffect == UIEffectPlayer.PlayingExcludeEffectType.None)		//start from beginning
			{
				transform.localPosition = new Vector3(to.x, to.y, transform.localPosition.z);
			}

            iTween.MoveTo(transform.gameObject, iTween.Hash("position", new Vector3(from.x, from.y, transform.localPosition.z), "easeType", "easeInExpo", "oncomplete", "OnComplete", "oncompletetarget", this.gameObject, "time", moveTime));
			bCallBackComplete = true;
        }

        if (scaleTo != scaleFrom)
        {
			if(mCurPlayingExcludeEffect == UIEffectPlayer.PlayingExcludeEffectType.None)		//start from beginning
			{
            	transform.localScale = new Vector3(scaleTo, scaleTo, 1.0f);
			}
			if(bCallBackComplete)
			{
				iTween.ScaleTo(transform.gameObject, iTween.Hash("scale", new Vector3(scaleFrom, scaleFrom, 1.0f), "easeType", "easeInExpo", "time", scaleTime));
			}
			else
			{
				iTween.ScaleTo(transform.gameObject, iTween.Hash("scale", new Vector3(scaleFrom, scaleFrom, 1.0f), "easeType", "easeInExpo", "oncomplete", "OnComplete", "oncompletetarget", this.gameObject, "time", scaleTime));
			}
        }
		this.enabled = true;
		base.OnHide();
	}

    protected void OnComplete()
    {
        mCurPlayingExcludeEffect = PlayingExcludeEffectType.None;
		mWidget = GetComponentsInChildren<UIWidget>();
		Update();
		this.enabled = false;
    }

    public void Restart()
    {
        iTween.Stop(transform.gameObject);
        if (mCurPlayingExcludeEffect == PlayingExcludeEffectType.ShowEffect)
        {
            OnShow();
        }
        if (mCurPlayingExcludeEffect == PlayingExcludeEffectType.HideEffect)
        {
            OnHide();
        }
    }

    public void Update()
    {
        if (!useAlpha || mWidget == null)
        {
            return;
        }

        float curAlpha = 1 - (1 - transform.localScale.x) * 2;
		curAlpha = Mathf.Clamp01(curAlpha);
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
                    color.a = curAlpha;
                    w.color = color;
                }
            }
            );
    }
}
