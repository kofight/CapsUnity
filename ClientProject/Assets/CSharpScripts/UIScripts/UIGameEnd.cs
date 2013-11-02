using UnityEngine;
using System.Collections;

public class UIGameEnd : UIWindowNGUI 
{
    UIButton m_playOnBtn;
    UIButton m_EndGameBtn;
    public override void OnCreate()
    {
        base.OnCreate();

        m_playOnBtn = UIToolkits.GetChildComponent<UIButton>(mUIObject.transform, "PlayOnBtn");
        m_EndGameBtn = UIToolkits.GetChildComponent<UIButton>(mUIObject.transform, "EndGameBtn");
        AddChildComponentMouseClick("EndGameBtn", OnEndGameClicked);
        AddChildComponentMouseClick("PlayOnBtn", OnPlayOnClicked);
    }
    public override void OnShow()
    {
        base.OnShow();
    }
    public override void OnUpdate()
    {
        base.OnUpdate();
    }

    private void OnEndGameClicked(object sender, UIMouseClick.ClickArgs e)
    {
        HideWindow();
        UIWindowManager.Singleton.GetUIWindow<UIRetry>().ShowWindow();
    }

    private void OnPlayOnClicked(object sender, UIMouseClick.ClickArgs e)
    {
       if (GlobalVars.CurStageData.StepLimit > 0)
       {
           if (GlobalVars.CurStageData.StepLimit > 0)     //若还有步数
           {
               HideWindow();
           }
       }
    }
}
