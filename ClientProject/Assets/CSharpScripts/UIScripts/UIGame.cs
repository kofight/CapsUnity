using UnityEngine;
using System.Collections;

public class UIGame : UIWindowNGUI
{
    public override void OnCreate()
    {
        base.OnCreate();

        AddChildComponentMouseClick("EditorBtn", OnEditStageClicked);
    }
    public override void OnShow()
    {
        base.OnShow();

    }
    public override void OnUpdate()
    {
        base.OnUpdate();
    }

    private void OnEditStageClicked(object sender, UIMouseClick.ClickArgs e)
    {
        if (UIWindowManager.Singleton.GetUIWindow<UIStageEditor>() == null)
        {
            UIWindowManager.Singleton.CreateWindow<UIStageEditor>(UIWindowManager.Anchor.Right);
        }
        UIWindowManager.Singleton.GetUIWindow<UIStageEditor>().ShowWindow();        //显示编辑窗口
    }
}
