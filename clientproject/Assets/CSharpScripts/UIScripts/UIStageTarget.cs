using UnityEngine;
using System.Collections;

public class UIStageTarget : UIWindow 
{
	Transform m_collectBoard;
	Transform m_jellyBoard;
	Transform m_stepScoreBoard;
	Transform m_timeScoreBoard;
	Transform m_nutBoard;

	UILabel nut1Label;
	UILabel nut2Label;
	UISprite nut1Icon;
	UISprite nut2Icon;

    UILabel[] collectLabel = new UILabel[3];
	UISprite [] collectIcon = new UISprite [3];

    public override void OnCreate()
    {
        base.OnCreate();

		m_collectBoard = mUIObject.transform.FindChild("CollectBoard");
		m_jellyBoard = mUIObject.transform.FindChild("JellyBoard");
		m_stepScoreBoard = mUIObject.transform.FindChild("ScoreBoard");
		m_timeScoreBoard = mUIObject.transform.FindChild("TimeBoard");
		m_nutBoard = mUIObject.transform.FindChild("NutBoard");
		
	 	nut1Label = GetChildComponent<UILabel>("NutCount1");
		nut2Label = GetChildComponent<UILabel>("NutCount2");
		nut1Icon = GetChildComponent<UISprite>("NutIcon1");
		nut2Icon = GetChildComponent<UISprite>("NutIcon2");

        for (int i = 0; i < 3; ++i )
        {
            collectLabel[i] = GetChildComponent<UILabel>("CollectCount" + (i + 1));
            collectIcon[i] = GetChildComponent<UISprite>("Icon" + (i + 1));
        }
    }
    public override void OnShow()
    {
        base.OnShow();
		StageData stage = GlobalVars.CurStageData;
		Transform curBoard;
		if(stage.Target == GameTarget.GetScore)
		{
			m_collectBoard.gameObject.SetActive(false);
			m_jellyBoard.gameObject.SetActive(false);
			m_nutBoard.gameObject.SetActive(false);
			if(stage.StepLimit > 0)
			{
				m_stepScoreBoard.gameObject.SetActive(true);
				m_timeScoreBoard.gameObject.SetActive(false);
				curBoard = m_stepScoreBoard;
			}
			else
			{
				m_timeScoreBoard.gameObject.SetActive(true);
				m_stepScoreBoard.gameObject.SetActive(false);
				curBoard = m_timeScoreBoard;
			}

			UILabel scoreLabel = GetChildComponent<UILabel>("Score");
			scoreLabel.text = stage.StarScore[0].ToString();
		}
		else if(stage.Target == GameTarget.ClearJelly)
		{
			m_collectBoard.gameObject.SetActive(false);
			m_jellyBoard.gameObject.SetActive(true);
			curBoard = m_jellyBoard;
			m_stepScoreBoard.gameObject.SetActive(false);
			m_timeScoreBoard.gameObject.SetActive(false);
			m_nutBoard.gameObject.SetActive(false);

			UILabel jellyLabel = GetChildComponent<UILabel>("JellyCount");
			jellyLabel.text = stage.GetJellyCount().ToString();
		}
		else if(stage.Target == GameTarget.BringFruitDown)
		{
			m_collectBoard.gameObject.SetActive(false);
			m_jellyBoard.gameObject.SetActive(false);
			m_stepScoreBoard.gameObject.SetActive(false);
			m_timeScoreBoard.gameObject.SetActive(false);
			m_nutBoard.gameObject.SetActive(true);
			curBoard = m_nutBoard;

			if(stage.Nut1Count > 0)
			{
				nut1Label.text = stage.Nut1Count.ToString();

				nut1Label.gameObject.SetActive(true);
				nut1Icon.gameObject.SetActive(true);
			}
			else
			{
				nut1Label.gameObject.SetActive(false);
				nut1Icon.gameObject.SetActive(false);
			}
			if(stage.Nut2Count > 0)
			{
				nut2Label.text = stage.Nut2Count.ToString();

				nut2Label.gameObject.SetActive(true);
				nut2Icon.gameObject.SetActive(true);
			}
			else
			{
				nut2Label.gameObject.SetActive(false);
				nut2Icon.gameObject.SetActive(false);
			}
		}
		else 		//Collect
		{
			m_collectBoard.gameObject.SetActive(true);
			curBoard = m_collectBoard;
			m_jellyBoard.gameObject.SetActive(false);
			m_stepScoreBoard.gameObject.SetActive(false);
			m_timeScoreBoard.gameObject.SetActive(false);
			m_nutBoard.gameObject.SetActive(false);

            for (int i = 0; i < 3; ++i )
            {
                if (stage.CollectCount[i] > 0)
                {
                    collectLabel[i].gameObject.SetActive(true);
                    collectIcon[i].gameObject.SetActive(true);

                    collectLabel[i].text = stage.CollectCount[i].ToString();

                    switch (GlobalVars.CurStageData.CollectSpecial[i])
                    {
                        case TSpecialBlock.ESpecial_Normal:
                            {
                                collectIcon[i].spriteName = "Item" + (int)(GlobalVars.CurStageData.CollectColors[i] - TBlockColor.EColor_None);
                            }
                            break;
                        case TSpecialBlock.ESpecial_NormalPlus6:
                            {
                                collectIcon[i].spriteName = "TimeAdded" + (int)(GlobalVars.CurStageData.CollectColors[i] - TBlockColor.EColor_None);
                            }
                            break;
                        case TSpecialBlock.ESpecial_EatLineDir0:
                            collectIcon[i].spriteName = "Line" + (int)(GlobalVars.CurStageData.CollectColors[i] - TBlockColor.EColor_None) + "_3";
                            break;
                        case TSpecialBlock.ESpecial_EatLineDir1:
                            collectIcon[i].spriteName = "Line" + (int)(GlobalVars.CurStageData.CollectColors[i] - TBlockColor.EColor_None) + "_1";
                            break;
                        case TSpecialBlock.ESpecial_EatLineDir2:
                            collectIcon[i].spriteName = "Line" + (int)(GlobalVars.CurStageData.CollectColors[i] - TBlockColor.EColor_None) + "_2";
                            break;
                        case TSpecialBlock.ESpecial_Bomb:
                            collectIcon[i].spriteName = "Bomb" + (int)(GlobalVars.CurStageData.CollectColors[i] - TBlockColor.EColor_None);
                            break;
                        case TSpecialBlock.ESpecial_EatAColor:
                            collectIcon[i].spriteName = "Rainbow";
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    collectLabel[i].gameObject.SetActive(false);
                    collectIcon[i].gameObject.SetActive(false);
                }
            }
		}

		if(stage.StepLimit > 0)
		{
			UILabel stepLabel = UIToolkits.FindComponent<UILabel>(curBoard, "Step");
			stepLabel.text = stage.StepLimit.ToString();
		}
		else
		{
			UILabel timeLabel = UIToolkits.FindComponent<UILabel>(curBoard, "Time");
			timeLabel.text = stage.TimeLimit.ToString();
		}
    }
}
