using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class ShowSimpleUIEffect : UIEffectPlayer
{
    public bool bTweenAlpha = true;
    public bool bTweenScale = true;
    public bool bTweenPos = true;

    public Vector2 from;
    public Vector2 to;

    public float duration = 0.20f;
	public float startAlpah = 0.2f;
	public float endAlpah = 1.0f;
    public Vector3 startScale = new Vector3(0.1f, 0.1f, 1);
    public Vector3 endScale = new Vector3(1, 1, 1);

    public TweenPosition mTweenPos;
    public TweenAlpha mTweenAlpha;
    public TweenScale mTweenScale;

    public List<UITweener> mTweenList = new List<UITweener>();

    void AddEffectTo(GameObject gameObject)
    {
        //创建需要的特效
        if (mTweenAlpha == null)
        {
			mTweenAlpha = TweenAlpha.Begin(gameObject, duration, endAlpah);
            mTweenAlpha.alpha = startAlpah;
            mTweenAlpha.from = startAlpah;
			mTweenAlpha.to = endAlpah;
			mTweenAlpha.duration = duration;

            mTweenList.Add(mTweenAlpha);
        }
        else
        {
			TweenAlpha newAlpha = null;
			if(mTweenAlpha.gameObject != gameObject)
				newAlpha = TweenAlpha.Begin(gameObject, duration, endAlpah);
			else
				newAlpha = mTweenAlpha;

            newAlpha.animationCurve = mTweenAlpha.animationCurve;
            newAlpha.alpha = startAlpah;
            newAlpha.from = startAlpah;
			newAlpha.to = endAlpah;
			newAlpha.duration = duration;

            mTweenList.Add(newAlpha);
        }
    }

    public override void CreateEffect()
    {
        if (bTweenPos)
        {
            float fromToValX = to.x - from.x;
            float fromToValY = to.y - from.y;

			Vector3 fromPos = new Vector3(transform.localPosition.x - fromToValX, transform.localPosition.y - fromToValY, transform.localPosition.z);
			Vector3 toPos = transform.localPosition;

            //创建需要的特效
            if (mTweenPos == null)
            {
				mTweenPos = TweenPosition.Begin(transform.gameObject, duration, toPos);
            }
            
			mTweenPos.from = fromPos;
			mTweenPos.to = toPos;
			mTweenPos.duration = duration;

            mTweenList.Add(mTweenPos);
        }

        if (bTweenScale)
        {
            //创建需要的特效
            if (mTweenScale == null)
            {
                mTweenScale = TweenScale.Begin(transform.gameObject, duration, endScale);
            }
            
            mTweenScale.scale = startScale;
            mTweenScale.from = startScale;
			mTweenScale.to = endScale;
			mTweenScale.duration = duration;

            mTweenList.Add(mTweenScale);
        }

        if (bTweenAlpha)
        {
            if (GetComponent<UIPanel>() != null)            //若为 UIPanel, 添加AlphaTween到自己
            {
                AddEffectTo(gameObject);
            }
            else                                            // 否则,添加AlphaTween到所有自控件
            {
                List<UIWidget> list = new List<UIWidget>();
                UIToolkits.FindComponents<UIWidget>(transform, list);
                foreach (UIWidget widget in list)
                {
                    AddEffectTo(widget.gameObject);
                }
            }
        }
    }

    protected override void DoShowEffect()
    {
        foreach (UITweener tweener in mTweenList)
        {
            tweener.Play(true);
        }
    }

    protected override void DoHideEffect()
	{
        foreach (UITweener tweener in mTweenList)
        {
            tweener.Play(false);
        }
    }

    public override bool IsPlaying()
    {
        foreach (UITweener tweener in mTweenList)
        {
            if (tweener.mStarted) return true;
        }
        return false;
    }
}