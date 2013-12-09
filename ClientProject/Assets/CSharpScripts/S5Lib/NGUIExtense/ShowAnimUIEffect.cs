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
        if (ShowAnim != null)
        {
            m_animation.Play("ShowAnim");
        }
        else
        {
            m_animation.Stop();
        }
    }

    protected override void DoHideEffect()
    {
        if (HideAnim != null)
        {
            m_animation.Play("HideAnim");
        }
        else
        {
            m_animation.Stop();
        }
    }

    protected override void DoIdleEffect()
    {
        if (IdleAnim != null)
        {
            m_animation.Play("IdelAnim");
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
}
