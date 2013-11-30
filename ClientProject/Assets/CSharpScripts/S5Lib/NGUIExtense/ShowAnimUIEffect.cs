using UnityEngine;
using System.Collections;

public class ShowAnimUIEffect : UIEffectPlayer 
{
	Animation m_animation;
	public AnimationClip ShowAnim;
	public AnimationClip HideAnim;
	
	public float Delay = 0.0f;
	
	float m_showAnimStartTime = 0.0f;
	
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
		}
	}
	
	public override void ShowEffect()
    {
		if(ShowAnim != null)
		{
			if(Delay == 0)
			{
				m_animation.Play("ShowAnim");
			}
			else
			{
				m_showAnimStartTime = Time.realtimeSinceStartup;
			}
		}
    }

    public override void HideEffect()
	{
		if(HideAnim != null)
		{
			m_animation.Play("HideAnim");
		}
    }

    public override bool IsPlaying()
    {
        return animation.isPlaying;
    }
	
	public override void Update ()
	{
		if(m_showAnimStartTime > 0)
		{
			if(Time.realtimeSinceStartup - m_showAnimStartTime > Delay)
			{
				m_animation.Play("ShowAnim");
				m_showAnimStartTime = 0;
			}
		}
	}
}
