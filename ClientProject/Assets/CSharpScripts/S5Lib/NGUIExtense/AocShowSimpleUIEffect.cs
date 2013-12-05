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
    public Vector3 startScale = new Vector3(0, 0, 1);
    public Vector3 endScale = new Vector3(1, 1, 1);

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
            mTweenAlpha.from = startAlpah;
            mTweenAlpha.to = endAlpah;
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
            mTweenScale.from = startScale;
            mTweenScale.to = endScale;
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
            mTweenScale = TweenScale.Begin(transform.gameObject, duration, endScale);
            mTweenScale.scale = startScale;
		}

        if (bTweenAlpha && mTweenAlpha == null)
		{
            mTweenAlpha = TweenAlpha.Begin(transform.gameObject, duration, endAlpah);
            mTweenAlpha.alpha = startAlpah;
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