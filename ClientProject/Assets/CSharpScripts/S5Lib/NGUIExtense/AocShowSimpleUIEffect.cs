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
	public float startAlpah = 0.2f;
	public float endAlpah = 1.0f;

    public TweenPosition mTweenPos;
    public TweenAlpha mTweenAlpha;
    public TweenScale mTweenScale;

    bool m_bShowing = false;            //是在显示还是在隐藏

    public void PlayTweenPosition(bool forward)
    {
        m_bShowing = forward;
        if (bTweenAlpha)
        {
            if (forward)
            {
                mTweenAlpha.delay = Delay;
            }
            else
            {
                mTweenAlpha.delay = 0;
            }
			mTweenAlpha.Play(forward);
        }
        if (bTweenScale)
        {
            if (forward)
            {
                mTweenScale.delay = Delay;
            }
            else
            {
                mTweenScale.delay = 0;
            }
			mTweenScale.Play(forward);
        }
        if (bTweenPos)
        {
            if (forward)
            {
                mTweenPos.delay = Delay;
            }
            else
            {
                mTweenPos.delay = 0;
            }
			mTweenPos.Play(forward);
            EventDelegate.Set(mTweenPos.onFinished, delegate() { PlayAnimeFinished(forward); });
        }
        else if (bTweenAlpha)
        {
            EventDelegate.Set(mTweenAlpha.onFinished, delegate() { PlayAnimeFinished(forward); });
        }
        else if (bTweenScale)
        {
            EventDelegate.Set(mTweenScale.onFinished, delegate() { PlayAnimeFinished(forward); });
        }
    }

    public void CreateComponents()
    {
        if (bTweenScale && mTweenScale == null)
		{
            float fromToVal = 1.0f - 0.2f;
            mTweenScale = TweenScale.Begin(transform.gameObject, duration, transform.localScale);
            mTweenScale.from = new Vector3(transform.localScale.x - fromToVal, transform.localScale.y - fromToVal, transform.localScale.z - fromToVal);
            mTweenScale.delay = Delay;
            mTweenScale.scale = mTweenScale.from;
		}

        if (bTweenAlpha && mTweenAlpha == null)
		{
            mTweenAlpha = TweenAlpha.Begin(transform.gameObject, duration, endAlpah);
            mTweenAlpha.from = startAlpah;
            mTweenAlpha.delay = Delay;
            mTweenAlpha.alpha = mTweenAlpha.from;
		}

        if (bTweenPos && mTweenPos == null)
        {
            float fromToValX = to.x - from.x;
            float fromToValY = to.y - from.y;

            mTweenPos = TweenPosition.Begin(transform.gameObject, duration, transform.localPosition);
            mTweenPos.from = new Vector3(transform.localPosition.x - fromToValX, transform.localPosition.y - fromToValY, transform.localPosition.z);
            mTweenPos.delay = Delay;
            mTweenPos.position = mTweenPos.from;
        }
    }

    public override void ShowEffect()
    {
        //若有按钮,播特效时让按钮停止工作,防止影响动画
        EnableButtons(false);
		CreateComponents();
        PlayTweenPosition(true);
    }

    public override void HideEffect()
	{
        //若有按钮,播特效时让按钮停止工作,防止影响动画
        EnableButtons(false);
    	CreateComponents();
        if (bTweenAlpha) mTweenAlpha.alpha = endAlpah;
        PlayTweenPosition(false);
    }

    public override bool IsPlaying()
    {
        if (mTweenAlpha != null)
        {
            if (mTweenAlpha.mStarted)
            {
                return true;
            }
        }

        if (mTweenPos != null)
        {
            if (mTweenPos.mStarted)
            {
                return true;
            }
        }

        if (mTweenScale != null)
        {
            if (mTweenScale.mStarted)
            {
                return true;
            }
        }
        return false;
    }

    protected void PlayAnimeFinished(bool forward)
    {
        if (bTweenAlpha)
        {
            mTweenAlpha.delay = 0;
        }
        if (bTweenScale)
        {
            mTweenScale.delay = 0;
        }
        if (bTweenPos)
        {
            mTweenPos.delay = 0;
        }

        EnableButtons(true);
    }

    void EnableButtons(bool bEnable)
    {
        //若有按钮,播特效时让按钮停止工作,防止影响动画
        UIButton button = transform.GetComponent<UIButton>();
        if (button != null)
        {
            button.enabled = bEnable;
        }
    }
}