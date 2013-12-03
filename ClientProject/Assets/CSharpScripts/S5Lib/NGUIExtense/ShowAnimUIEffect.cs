using UnityEngine;
using System.Collections;

enum EffectState
{
	Delay,
	Showing,
	Idle,
	Hiding,
}

public class ShowAnimUIEffect : UIEffectPlayer 
{
	Animation m_animation;
	public AnimationClip ShowAnim;
	public AnimationClip HideAnim;
	public AnimationClip IdleAnim;
	
	float m_delayStartTime = 0.0f;
	
	EffectState m_state;
	
	public override void CreateEffect()
	{
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
	
	public override void ShowEffect()
    {
		if(ShowAnim != null)
		{
			if(Delay == 0)
			{
				m_state = EffectState.Showing;
				m_animation.Play("ShowAnim");
			}
			else
			{
				m_state = EffectState.Delay;
				gameObject.SetActive(false);
				m_delayStartTime = Time.realtimeSinceStartup;
			}
		}
    }

    public override void HideEffect()
	{
		m_state = EffectState.Hiding;
		if(HideAnim != null)
		{
			m_animation.Play("HideAnim");
		}
		else
		{
			m_animation.Stop();	
		}
    }

    public override bool IsPlaying()
    {
        return m_animation.isPlaying;
    }
	
	public override void Update ()
	{
		if(m_state == EffectState.Delay)
		{
			if(Time.realtimeSinceStartup - m_delayStartTime > Delay)
			{
				m_state = EffectState.Showing;
				m_delayStartTime = 0;
				gameObject.SetActive(true);
				m_animation.Play("ShowAnim");
			}
		}
		
		if(m_state == EffectState.Showing && !m_animation.isPlaying)
		{
			m_animation.Play("IdelAnim");
			m_state = EffectState.Idle;
		}
	}
}
