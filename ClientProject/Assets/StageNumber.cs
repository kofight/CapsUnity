using UnityEngine;
using System.Collections;

public class StageNumber : MonoBehaviour {
	public UISprite m_stageNumber0;
	public UISprite m_stageNumber1;
	public UISprite m_stageNumber2;
	public int StageNum;
	public string SurName;
	public int NumberInterval = 20;
	// Use this for initialization
	void Awake () {
		StageNum = System.Convert.ToInt32(transform.name.Substring(5));
		
		int posX = 0;
		if(StageNum > 99)
		{
			posX = -NumberInterval;
			m_stageNumber2.spriteName = SurName + (StageNum / 100);
			m_stageNumber2.LocalPositionX(posX);
			posX+=NumberInterval;
		}
		else
		{
			m_stageNumber2.gameObject.SetActive(false);
			posX = -NumberInterval / 2;
		}
		if(StageNum > 9)
		{
			m_stageNumber1.spriteName = SurName + ((StageNum % 100) / 10) ;
			m_stageNumber1.LocalPositionX(posX);
			posX+=NumberInterval;
		}
		else
		{
			m_stageNumber1.gameObject.SetActive(false);
			posX = 0;
		}
		m_stageNumber0.spriteName = SurName + StageNum % 10;
		m_stageNumber0.LocalPositionX(posX);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
