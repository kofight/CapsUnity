using UnityEngine;
using System.Collections;

public class UIGame : UIWindowNGUI
{
    UISprite[] m_starsSprites = new UISprite[3];
    UISprite m_progressSprite;
    static readonly int ProgressLenth = 272;
    static readonly int ProgressStartX = 225;

    public override void OnCreate()
    {
        base.OnCreate();

        AddChildComponentMouseClick("EditorBtn", OnEditStageClicked);

        for (int i = 0; i < 3; ++i )
        {
            m_starsSprites[i] = GetChildComponent<UISprite>("Star" + (i + 1));      //查找sprite
        }
        m_progressSprite = GetChildComponent<UISprite>("Progress");      //查找sprite
    }
    public override void OnShow()
    {
        base.OnShow();
    }
	
	public void Reset()
	{
		if (GlobalVars.CurStageData.StarScore[2] > 0)
        {
            for (int i = 0; i < 2; ++i)
            {
                m_starsSprites[i].LocalPositionX(ProgressStartX + ProgressLenth * GlobalVars.CurStageData.StarScore[i] / GlobalVars.CurStageData.StarScore[2]);      //放置位置
            }
            for (int i = 0; i < 3; ++i)
            {
                m_starsSprites[i].spriteName = "Start";         //初始化成默认状态
            }
        }
		
		m_progressSprite.gameObject.SetActive(false);
	}
	
    public override void OnUpdate()
    {
        base.OnUpdate();
        float completeRatio = 0.0f;
        if (GlobalVars.CurStageData.StarScore[2] > 0)
        {
            completeRatio = (float)GlobalVars.CurGameLogic.GetProgress() / GlobalVars.CurStageData.StarScore[2];
        }
        completeRatio = Mathf.Min(1.0f, completeRatio);
        if (completeRatio != 0.0f)
        {
			m_progressSprite.gameObject.SetActive(true);
            m_progressSprite.transform.LocalScaleX(ProgressLenth * completeRatio);
        }
        //GlobalVars.CurGameLogic.GetProgress()
    }

    public void OnChangeProgress(int progress)
    {
        for (int i = 0; i < 3; ++i)
        {
            if (progress >= GlobalVars.CurStageData.StarScore[i])
            {
                m_starsSprites[i].spriteName = "LightStar";
            }
        }
    }

    private void OnEditStageClicked(object sender, UIMouseClick.ClickArgs e)
    {
        if (UIWindowManager.Singleton.GetUIWindow<UIStageEditor>() == null)
        {
            UIWindowManager.Singleton.CreateWindow<UIStageEditor>(UIWindowManager.Anchor.Right);
        }
        UIWindowManager.Singleton.GetUIWindow<UIStageEditor>().ShowWindow();        //显示编辑窗口
        GlobalVars.EditStageMode = true;        //编辑器里自动进入关卡编辑模式
    }
}
