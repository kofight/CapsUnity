using UnityEngine;
using System.Collections;
using System;

public class AocShowSimpleUIEffect : UIEffectPlayer
{
    public bool bTweenAlpha = true;
    public bool bTweenScale = true;
    public bool bTweenPos = true;

    public Vector2 from;
    public Vector2 to;

    public float duration = 0.20f;
    public float delay = 0.0f;
	public bool delayOnHide = false;
	public float startAlpah = 0.2f;
	public float endAlpah = 1.0f;

    AocTweenPosition mTweenPos;
    AocTweenAlpha mTweenAlpha;
    AocTweenScale mTweenScale;

    public delegate void EffectFinished();
    public EffectFinished HideEffectFinishedFunc;
    public EffectFinished ShowEffectFinishedFunc;

    void Update()
    {

    }
	
	public void Awake()
	{
		
	}

    public void PlayTweenPosition(bool forward)
    {
        if (!PlayWhileShowWindow && !forward)		//若Show时不播特效,播Hide时重置Factor
        {
            if (bTweenAlpha)
                mTweenAlpha.tweenFactor = 1.0f;
            if (bTweenScale)
                mTweenScale.tweenFactor = 1.0f;
            if (bTweenPos)
                mTweenPos.tweenFactor = 1.0f;
        }

        if (!PlayWhileHideWindow && forward)			//若Hide时不播特效,播Show时重置Factor
        {
            if (bTweenAlpha)
                mTweenAlpha.tweenFactor = 0.0f;
            if (bTweenScale)
                mTweenScale.tweenFactor = 0.0f;
            if (bTweenPos)
                mTweenPos.tweenFactor = 0.0f;
        }

        if (bTweenAlpha)
        {
            mTweenAlpha.Play(forward);
        }
        if (bTweenScale)
        {
            mTweenScale.Play(forward);
        }
        if (bTweenPos)
        {
            mTweenPos.Play(forward);
            mTweenPos.CallWhenFinished = delegate() { PlayAnimeFinished(forward); };
        }
        else if (bTweenAlpha)
        {
            mTweenAlpha.CallWhenFinished = delegate() { PlayAnimeFinished(forward); };
        }
        else if (bTweenScale)
        {
            mTweenScale.CallWhenFinished = delegate() { PlayAnimeFinished(forward); };
        }
    }

    public void CreateComponents()
    {
        if (bTweenScale && mTweenScale == null)
		{
            mTweenScale = transform.gameObject.AddComponent<AocTweenScale>();
			mTweenScale.enabled = false;

            float fromToVal = 1.0f - 0.2f;
            mTweenScale.from = new Vector3(transform.localScale.x - fromToVal, transform.localScale.y - fromToVal, transform.localScale.z - fromToVal);
            mTweenScale.to = transform.localScale;

            mTweenScale.delay = delay;
            mTweenScale.duration = duration;
		}

        if (bTweenAlpha && mTweenAlpha == null)
		{
            mTweenAlpha = transform.gameObject.AddComponent<AocTweenAlpha>();
			mTweenAlpha.enabled = false;
            mTweenAlpha.delay = delay;
            mTweenAlpha.from = startAlpah;
            mTweenAlpha.to = endAlpah;
            mTweenAlpha.duration = duration;
		}

        if (bTweenPos && mTweenPos == null)
        {
            mTweenPos = transform.gameObject.AddComponent<AocTweenPosition>();
            mTweenPos.enabled = false;
            float fromToValX = to.x - from.x;
            float fromToValY = to.y - from.y;

            mTweenPos.to = transform.localPosition;
            mTweenPos.from = new Vector3(mTweenPos.to.x - fromToValX, mTweenPos.to.y - fromToValY, transform.localPosition.z);
            mTweenPos.delay = delay;

            mTweenPos.duration = duration;
        }
    }


    public override void ResetWidget()
    {
        base.ResetWidget();
        mTweenAlpha.ResetWidget();
    }

    public override void OnShow()
    {
		CreateComponents();
        if (bTweenAlpha)
        {
            mTweenAlpha.alpha = 0.0f;
            mTweenAlpha.mStarted = false;
        }

        if (bTweenScale)
        {
            mTweenScale.mStarted = false;
        }

        if (bTweenPos)
        {
            mTweenPos.mStarted = false;
        }

        mCurPlayingExcludeEffect = PlayingExcludeEffectType.ShowEffect;
        PlayTweenPosition(true);
		
        //若有按钮,播特效时让按钮停止工作,防止影响动画
		UIButton button = transform.GetComponent<UIButton>();
		if(button != null)
		{
			button.enabled = false;
		}
    }

    public override void OnHide()
	{
    	CreateComponents();
        if (bTweenAlpha) mTweenAlpha.alpha = endAlpah;
        if (delayOnHide)
        {
            if(bTweenPos)mTweenPos.mStarted = false;
            if(bTweenScale)mTweenScale.mStarted = false;
            if (bTweenAlpha) mTweenAlpha.mStarted = false;
        }

        mCurPlayingExcludeEffect = PlayingExcludeEffectType.HideEffect;
        PlayTweenPosition(false);

        //若有按钮,播特效时让按钮停止工作,防止影响动画
		UIButton button = transform.GetComponent<UIButton>();
		if(button != null)
		{
			button.enabled = false;
		}
    }

    protected virtual void PlayAnimeFinished(bool forward)
    {
        if (mCurPlayingExcludeEffect == PlayingExcludeEffectType.ShowEffect)
        {
            if (bTweenAlpha) mTweenAlpha.alpha = endAlpah;
			UIButton button = transform.GetComponent<UIButton>();
			if(button != null)
			{
				button.enabled = true;
			}
            if (ShowEffectFinishedFunc != null)
            {
                ShowEffectFinishedFunc();
            }
        }
        else if (mCurPlayingExcludeEffect == PlayingExcludeEffectType.HideEffect)
        {
            if (bTweenAlpha) mTweenAlpha.alpha = 0.0f;
            //若有按钮,播特效时让按钮停止工作,防止影响动画
			UIButton button = transform.GetComponent<UIButton>();
			if(button != null)
			{
				button.enabled = false;
			}
            if (HideEffectFinishedFunc != null)
            {
                HideEffectFinishedFunc();
            }
        }
        mCurPlayingExcludeEffect = PlayingExcludeEffectType.None;
    }
}