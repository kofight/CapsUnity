using UnityEngine;
using System.Collections;



public class ShowAnimUIEffect : UIEffectPlayer 
{
	Animation m_animation;
	public AnimationClip ShowAnim;
	public AnimationClip HideAnim;
	public AnimationClip IdleAnim;

	public override void CreateEffect()
	{
        base.CreateEffect();
		m_animation = GetComponent<Animation>();
		if( m_animation == null)
		{
			m_animation = this.gameObject.AddComponent<Animation>();
			if(ShowAnim != null)
				m_animation.AddClip(ShowAnim, "ShowAnim");
			if(HideAnim != null)
				m_animation.AddClip(HideAnim, "HideAnim");
			if(IdleAnim != null)
				m_animation.AddClip(IdleAnim, "IdelAnim");
		}
	}

    protected override void DoShowEffect()
    {
		m_animation.Stop();
        if (ShowAnim != null)
        {
            m_animation.Play("ShowAnim");
        }
    }

    protected override void DoHideEffect()
    {
		m_animation.Stop();
        if (HideAnim != null)
        {
            m_animation.Play("HideAnim");
        }
    }

    protected override void DoIdleEffect()
    {
		m_animation.Stop();
        if (IdleAnim != null)
        {
            m_animation.Play("IdelAnim");
        }
    }

    public override bool IsPlaying()
    {
        if (m_state == EffectState.Hiding)          //
        {
            if (Timer.GetFixedTime() - m_curStateStartTime > StateTimeOut)       //若已经超时
            {
                m_animation.Stop();
                return false;       //无论如何都认为播放完毕
            }
        }
		if(m_state == EffectState.Idle)
		{
			return false;			//take it as stop when idle, even if the anim is playing
		}
		if(!m_animation)
			return false;
        return m_animation.isPlaying;
    }
}
