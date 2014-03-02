using UnityEngine;
using System.Collections;

public class UIStageEditor : UIWindow
{
    bool editingPortal = false;         //编辑传送门的标记
    UILabel m_portalEditLabel;
    UILabel m_seedLabel;

    public override void OnCreate()
    {
        base.OnCreate();

        m_portalEditLabel = GetChildComponent<UILabel>("EditPortalTip");
        m_seedLabel = GetChildComponent<UILabel>("SeedLabel");

        AddChildComponentMouseClick("SaveBtn", OnSaveClicked);

        AddChildComponentMouseClick("RevertBtn", OnLoadClicked);

        AddChildComponentMouseClick("CloseBtn", OnCloseClicked);

        AddChildComponentMouseClick("RestartBtn", delegate()
            {
                GameLogic.Singleton.ClearGame();
				GameLogic.Singleton.Init();
                GameLogic.Singleton.StartGame();
                RefreshControlls();
            });

        AddChildComponentMouseClick("ResortBtn", delegate()
        {
            GameLogic.Singleton.AutoResort();
        });

        AddChildComponentMouseClick("HelpBtn", delegate()
        {
            GameLogic.Singleton.ClearHelpPoint();
            GameLogic.Singleton.Help();
            GameLogic.Singleton.ShowHelpAnim();
        });

        AddChildComponentMouseClick("ReSeedBtn", delegate()
        {
            GameLogic.Singleton.ClearGame();
			GameLogic.Singleton.Init((int)(Time.realtimeSinceStartup * 1000 % 1000));
            GameLogic.Singleton.StartGame();
            m_seedLabel.text = GameLogic.Singleton.PlayingStageData.Seed.ToString();
            RefreshControlls();
        });

        for (int i = 0; i < GameLogic.TotalColorCount; ++i)
        {
			int color = i + 1;
            AddChildComponentMouseClick("ChangeColor" + color, delegate()
            {
                GlobalVars.EditState = TEditState.ChangeColor;
                GlobalVars.EditingColor = (TBlockColor)(color);
            });
        }

        AddChildComponentMouseClick("ChangeToNut1", delegate()
        {
            GlobalVars.EditState = TEditState.ChangeColor;
            GlobalVars.EditingColor = TBlockColor.EColor_Nut1;
        });

        AddChildComponentMouseClick("ChangeToNut2", delegate()
        {
            GlobalVars.EditState = TEditState.ChangeColor;
            GlobalVars.EditingColor = TBlockColor.EColor_Nut2;
        });

        //开始编辑关卡旗标事件
        AddChildComponentMouseClick("EditStageBlock", delegate()
        {
            GlobalVars.EditState = TEditState.EditStageGrid;
            GlobalVars.EditingGrid = GetGridFlagsFromCheckBoxes();
        });


        for (int i = 0; i <= (int)TSpecialBlock.ESpecial_EatAColor; ++i)
        {
            int special = i;
            AddChildComponentMouseClick("ChangeSpecial" + i, delegate()
            {
                GlobalVars.EditState = TEditState.ChangeSpecial;
                GlobalVars.EditingSpecial = (TSpecialBlock)special;
            });
        }

        for (int i = 1; i <= 10; ++i)
        {
			UIToggle toggle = GetChildComponent<UIToggle>("GridFlag" + i);
			EventDelegate.Set(toggle.onChange, delegate()
            {
                GlobalVars.EditState = TEditState.EditStageGrid;
                GlobalVars.EditingGrid = GetGridFlagsFromCheckBoxes();
            });
        }

        for (int i = 0; i <= (int)GameTarget.Collect; ++i)
        {
            int targetType = i;
            AddChildComponentMouseClick("TargetMode" + i, delegate()
            {
                GameLogic.Singleton.PlayingStageData.Target = (GameTarget)targetType;
            });
        }

        AddChildComponentMouseClick("EatBlock", delegate()
            {
                GlobalVars.EditState = TEditState.Eat;
            });

        AddChildComponentMouseClick("EditPortal", delegate()
        {
            GlobalVars.EditState = TEditState.EditPortal;
            GlobalVars.EditingPortal = new Portal();
            GlobalVars.EditingPortal.flag = 1;
            GlobalVars.EditingPortalTip = "Edit Portal: 选择Pos1";
        });

        AddChildComponentMouseClick("EditPortalInvisible", delegate()
        {
            GlobalVars.EditState = TEditState.EditPortal;
            GlobalVars.EditingPortal = new Portal();
            GlobalVars.EditingPortal.flag = 0;
            GlobalVars.EditingPortalTip = "Edit Portal: 选择Pos1";
        });

        AddChildComponentMouseClick("GridNoneBtn", delegate()
        {
            GlobalVars.EditingGrid = 0;
            RefreshStageGridFlagCheckBoxes(GlobalVars.EditingGrid);
        });

        AddChildComponentMouseClick("GridJellyBtn", delegate()
        {
            GlobalVars.EditingGrid = ((int)GridFlag.Jelly) | ((int)GridFlag.GenerateCap);
            RefreshStageGridFlagCheckBoxes(GlobalVars.EditingGrid);
        });

        AddChildComponentMouseClick("GridJelly2Btn", delegate()
        {
            GlobalVars.EditingGrid = ((int)GridFlag.JellyDouble) | ((int)GridFlag.GenerateCap);
            RefreshStageGridFlagCheckBoxes(GlobalVars.EditingGrid);
        });

        AddChildComponentMouseClick("ClearAllPortalBtn", delegate()
        {
            GameLogic.Singleton.PlayingStageData.PortalFromMap.Clear();
            GameLogic.Singleton.PlayingStageData.PortalToMap.Clear();
            for (int i = 0; i < GameLogic.BlockCountX; ++i )
            {
                for (int j = 0; j < GameLogic.BlockCountY; ++j )
                {
                    GameLogic.Singleton.PlayingStageData.ClearFlag(i, j, GridFlag.Portal);
                    GameLogic.Singleton.PlayingStageData.ClearFlag(i, j, GridFlag.PortalEnd);
                    GameLogic.Singleton.PlayingStageData.ClearFlag(i, j, GridFlag.PortalStart);
                }
            }
        });


        AddChildComponentMouseClick("GridNormalBtn", delegate()
        {
            GlobalVars.EditingGrid = (int)GridFlag.GenerateCap;
            RefreshStageGridFlagCheckBoxes(GlobalVars.EditingGrid);
        });

        AddChildComponentMouseClick("GridStoneBtn", delegate()
        {
            GlobalVars.EditingGrid = ((int)GridFlag.Stone) | ((int)GridFlag.NotGenerateCap);
            RefreshStageGridFlagCheckBoxes(GlobalVars.EditingGrid);
        });

        AddChildComponentMouseClick("GridChocolateBtn", delegate()
        {
            GlobalVars.EditingGrid = ((int)GridFlag.Chocolate) | ((int)GridFlag.NotGenerateCap);
            RefreshStageGridFlagCheckBoxes(GlobalVars.EditingGrid);
        });
    }

    void RefreshStageGridFlagCheckBoxes(int gridFlags)       //刷新当前编辑的GridFlag对应的CheckBox界面
    {
        for (int i = 0; i < 10; ++i )
        {
            UIToggle checkBox = UIToolkits.FindComponent<UIToggle>(mUIObject.transform, "GridFlag" + (i + 1));      //找到CheckBox
            if ((gridFlags & 1 << i) > 0)
            {
                checkBox.value = true;
            }
            else
            {
                checkBox.value = false;
            }
        }
    }

    int GetGridFlagsFromCheckBoxes()
    {
        int gridFlags = 0;
        for (int i = 0; i < 10; ++i)
        {
            UIToggle checkBox = UIToolkits.FindComponent<UIToggle>(mUIObject.transform, "GridFlag" + (i + 1));      //找到CheckBox
            if (checkBox.value)
            {
                gridFlags |= 1 << i;
            }
        }
        return gridFlags;
    }

    void RefreshControlls()
    {
        UIInput input = GetChildComponent<UIInput>("LevelInput");
        input.value = GlobalVars.CurStageNum.ToString();

        input = GetChildComponent<UIInput>("StepLimit");
        input.value = GameLogic.Singleton.PlayingStageData.StepLimit.ToString();

        input = GetChildComponent<UIInput>("TimeLimit");
        input.value = GameLogic.Singleton.PlayingStageData.TimeLimit.ToString();

        input = GetChildComponent<UIInput>("ColorCount");
        input.value = GameLogic.Singleton.PlayingStageData.ColorCount.ToString();

        input = GetChildComponent<UIInput>("Nut1");
        input.value = GlobalVars.CurStageData.Nut1Count.ToString();

        input = GetChildComponent<UIInput>("Nut2");
        input.value = GlobalVars.CurStageData.Nut2Count.ToString();

        input = GetChildComponent<UIInput>("NutInitCount");
        input.value = GlobalVars.CurStageData.NutInitCount.ToString();

        input = GetChildComponent<UIInput>("NutMaxCount");
        input.value = GlobalVars.CurStageData.NutMaxCount.ToString();

        input = GetChildComponent<UIInput>("NutMinCount");
        input.value = GlobalVars.CurStageData.NutMinCount.ToString();

        input = GetChildComponent<UIInput>("NutStep");
        input.value = GlobalVars.CurStageData.NutStep.ToString();

        input = GetChildComponent<UIInput>("PlusInitCount");
        input.value = GlobalVars.CurStageData.PlusInitCount.ToString();

        input = GetChildComponent<UIInput>("PlusMaxCount");
        input.value = GlobalVars.CurStageData.PlusMaxCount.ToString();

        input = GetChildComponent<UIInput>("PlusStep");
        input.value = GlobalVars.CurStageData.PlusStep.ToString();

        input = GetChildComponent<UIInput>("PlusStartTime");
        input.value = GlobalVars.CurStageData.PlusStartTime.ToString();

        //收集关
        for (int i = 0; i < 3; ++i )
        {
            input = GetChildComponent<UIInput>("Collects" + i.ToString() + "Input");
            input.value = GlobalVars.CurStageData.CollectCount[i].ToString() + "," + (int)GlobalVars.CurStageData.CollectColors[i] + "," + (int)GlobalVars.CurStageData.CollectSpecial[i];
        }

        for (int i = 0; i < 3; ++i)
        {
            input = GetChildComponent<UIInput>("Star" + (i + 1));
            input.value = GameLogic.Singleton.PlayingStageData.StarScore[i].ToString();
        }

        UIToggle targetCheck = GetChildComponent<UIToggle>("TargetMode" + (int)GameLogic.Singleton.PlayingStageData.Target);
        targetCheck.value = true;

        m_seedLabel.text = GameLogic.Singleton.PlayingStageData.Seed.ToString();
    }

    public override void OnShow()
    {
        base.OnShow();
        RefreshControlls();
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        if (GlobalVars.EditState == TEditState.EditPortal)
        {
            m_portalEditLabel.text = GlobalVars.EditingPortalTip;
        }
        else
        {
            m_portalEditLabel.text = string.Empty;
        }
    }

    private void OnCloseClicked()
    {
        HideWindow();
        GlobalVars.EditState = TEditState.None;
        GlobalVars.EditStageMode = false;
    }

    private void OnSaveClicked()
    {
        UIInput input = GetChildComponent<UIInput>("StepLimit");
        int stepLimit = (int)System.Convert.ChangeType(input.value, typeof(int));
        GameLogic.Singleton.PlayingStageData.StepLimit = stepLimit;



        input = GetChildComponent<UIInput>("TimeLimit");
        int timeLimit = (int)System.Convert.ChangeType(input.value, typeof(int));
        GameLogic.Singleton.PlayingStageData.TimeLimit = timeLimit;

        input = GetChildComponent<UIInput>("ColorCount");
        int colorCount = (int)System.Convert.ChangeType(input.value, typeof(int));
        GameLogic.Singleton.PlayingStageData.ColorCount = colorCount;

        for (int i = 0; i < 3; ++i )
        {
            input = GetChildComponent<UIInput>("Star" + (i + 1));
            int score = (int)System.Convert.ChangeType(input.value, typeof(int));
            GameLogic.Singleton.PlayingStageData.StarScore[i] = score;
        }

        input = GetChildComponent<UIInput>("Nut1");
        int nut1Count = (int)System.Convert.ChangeType(input.value, typeof(int));
        GameLogic.Singleton.PlayingStageData.Nut1Count = nut1Count;

        input = GetChildComponent<UIInput>("Nut2");
        int nut2Count = (int)System.Convert.ChangeType(input.value, typeof(int));
        GameLogic.Singleton.PlayingStageData.Nut2Count = nut2Count;

        input = GetChildComponent<UIInput>("NutInitCount");
        int nutInitCount = (int)System.Convert.ChangeType(input.value, typeof(int));
        GameLogic.Singleton.PlayingStageData.NutInitCount = nutInitCount;

        input = GetChildComponent<UIInput>("NutMaxCount");
        int nutMaxCount = (int)System.Convert.ChangeType(input.value, typeof(int));
        GameLogic.Singleton.PlayingStageData.NutMaxCount = nutMaxCount;

        input = GetChildComponent<UIInput>("NutMinCount");
        int nutMinCount = (int)System.Convert.ChangeType(input.value, typeof(int));
        GameLogic.Singleton.PlayingStageData.NutMinCount = nutMinCount;

        input = GetChildComponent<UIInput>("NutStep");
        int nutStep = (int)System.Convert.ChangeType(input.value, typeof(int));
        GameLogic.Singleton.PlayingStageData.NutStep = nutStep;

        input = GetChildComponent<UIInput>("PlusInitCount");
        int plusInitCount = (int)System.Convert.ChangeType(input.value, typeof(int));
        GameLogic.Singleton.PlayingStageData.PlusInitCount = plusInitCount;

        input = GetChildComponent<UIInput>("PlusMaxCount");
        int plusMaxCount = (int)System.Convert.ChangeType(input.value, typeof(int));
        GameLogic.Singleton.PlayingStageData.PlusMaxCount = plusMaxCount;

        input = GetChildComponent<UIInput>("PlusStartTime");
        int plusStartTime = (int)System.Convert.ChangeType(input.value, typeof(int));
        GameLogic.Singleton.PlayingStageData.PlusStartTime = plusStartTime;

        input = GetChildComponent<UIInput>("PlusStep");
        int plusStep = (int)System.Convert.ChangeType(input.value, typeof(int));
        GameLogic.Singleton.PlayingStageData.PlusStep = plusStep;

        input = GetChildComponent<UIInput>("LevelInput");
        int levelNum = (int)System.Convert.ChangeType(input.value, typeof(int));

        if (GameLogic.Singleton.PlayingStageData.Target == GameTarget.Collect)
        {
            //收集关
            for (int i = 0; i < 3; ++i)
            {
                input = GetChildComponent<UIInput>("Collects" + i.ToString() + "Input");
                if (input.value == System.String.Empty)
                {
                    continue;
                }
                string[] tokens = input.value.Split(',');

                GameLogic.Singleton.PlayingStageData.CollectCount[i] = System.Convert.ToInt32(tokens[0]);
                GameLogic.Singleton.PlayingStageData.CollectColors[i] = (TBlockColor)System.Convert.ToInt32(tokens[1]);
                GameLogic.Singleton.PlayingStageData.CollectSpecial[i] = (TSpecialBlock)System.Convert.ToInt32(tokens[2]);
                
            }
        }

        GameLogic.Singleton.PlayingStageData.SaveStageData(levelNum);
        GlobalVars.CurStageNum = levelNum;

        int stageType = 0;
        if (GameLogic.Singleton.PlayingStageData.TimeLimit > 0)     //若为时间关卡
        {
            stageType = (int)StageType.Time;
        }
        else if (GameLogic.Singleton.PlayingStageData.Target == GameTarget.GetScore)
        {
            stageType = (int)StageType.Score;
        }
        else if (GameLogic.Singleton.PlayingStageData.Target == GameTarget.ClearJelly)
        {
            stageType = (int)StageType.Jelly;
        }
        else if (GameLogic.Singleton.PlayingStageData.Target == GameTarget.BringFruitDown)
        {
            stageType = (int)StageType.Nut;
        }
        else if (GameLogic.Singleton.PlayingStageData.Target == GameTarget.Collect)
        {
            stageType = (int)StageType.Collect;
        }

        CapsConfig.StageTypeArray[levelNum - 1] = stageType;     //关卡类型

#if UNITY_EDITOR
        CapsConfig.Instance.Save();

        UnityEditor.AssetDatabase.Refresh();
#endif
        

        GlobalVars.CurStageData.LoadStageData(levelNum);

        RefreshControlls();
    }

    private void OnLoadClicked()
    {
        UIInput input = GetChildComponent<UIInput>("LevelInput");
        int levelNum = (int)System.Convert.ChangeType(input.value, typeof(int));
        GameLogic.Singleton.PlayingStageData = StageData.CreateStageData();
        GameLogic.Singleton.PlayingStageData.LoadStageData(levelNum);
    }
}
