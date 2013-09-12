using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIDrawerData
{
    public bool IsExistMode = false;            //Draw With ExistMode Or Not, Exist mode means create and manage by hand
    public bool IsLabel = true;                 //Is Label or Sprite
    public bool DidDrawLastFrame = false;       //Did the Data Drawed Last Frame?
    public int xPos;
    public int yPos;
    public UIWidget uiWidget;
    public UIEffectPlayer effectPlayer;
	public UIWindowStateEnum playingState;

    public void Show()
    {
        effectPlayer = uiWidget.GetComponent<UIEffectPlayer>();
        if (effectPlayer != null)
        {
            effectPlayer.OnShow();
            playingState = UIWindowStateEnum.PlayingShowEffect;
        }
    }

    public void OnShowEffectPlayOver()
    {
        playingState = UIWindowStateEnum.Show;
    }

    public void Hide()
    {
        if (effectPlayer != null)
        {
            effectPlayer.OnHide();
            playingState = UIWindowStateEnum.PlayingHideEffect;
        }
    }

    public void OnHideEffectPlayOver()
    {
        playingState = UIWindowStateEnum.Hide;
    }

    public void Update()
    {
        if (playingState == UIWindowStateEnum.PlayingShowEffect)
        {
            if (!effectPlayer.IsPlaying(UIEffectPlayer.PlayingExcludeEffectType.ShowEffect))
            {
                OnShowEffectPlayOver();
            }
        }
        
        if (playingState == UIWindowStateEnum.PlayingHideEffect)
        {
            if (!effectPlayer.IsPlaying(UIEffectPlayer.PlayingExcludeEffectType.HideEffect))
            {
                OnHideEffectPlayOver();
            }
        }
    }
}

public class UIDrawer
{
    private static readonly int moveDataCapibility = 100;
    public UIWindowManager.Anchor DefaultAnchor = UIWindowManager.Anchor.TopLeft;
    #region Singleton
    public static UIDrawer Singleton { get; private set; }
    public UIDrawer()
    {
        if (Singleton == null)
        {
            Singleton = this;
            OnCreate();
        }
        else
        {
            throw new System.Exception();			//if singleton exist, throw a Exception
        }
    }
    #endregion

    string[] DataToRemove = new string[moveDataCapibility];
    Dictionary<string, UIDrawerData> mDrawDataMap = new Dictionary<string, UIDrawerData>();
    Dictionary<int, GameObject> mPrefabs = new Dictionary<int, GameObject>();
    public int fontDefaultPrefabID = 0;
    public int spriteDefaultPrefabID = 0;

    public void OnCreate()
    {

    }

    public void AddPrefab(int prefabID, GameObject prefab)
    {
        if (!prefab)
        {
            Debug.LogError("Can't add null Label when labelID : " + prefabID.ToString());
        }
        mPrefabs[prefabID] = prefab;
    }

    public UIDrawerData CreateText(string id, int x, int y, string text, int prefabID)
    {
        UIDrawerData data = CreateWidget(id, x, y, prefabID);
        UILabel label = data.uiWidget as UILabel;
        data.uiWidget.gameObject.transform.localScale = new Vector3(label.font.size, label.font.size, 1);
        data.IsLabel = true;
        label.text = text;
		data.Show();      //触发显示
        return data;
    }

    public UIDrawerData CreateSprite(string id, int x, int y, string spriteName, int prefabID)
    {
        UIDrawerData data = CreateWidget(id, x, y, prefabID);
        UISprite sprite = data.uiWidget as UISprite;
        data.IsLabel = false;
        sprite.spriteName = spriteName;
        data.uiWidget.gameObject.transform.localScale = new Vector3(sprite.GetAtlasSprite().paddingRight - sprite.GetAtlasSprite().paddingLeft, sprite.GetAtlasSprite().paddingTop - sprite.GetAtlasSprite().paddingBottom, 0);
        data.uiWidget.gameObject.transform.localPosition = new Vector3(x, -y, -100f);
		data.Show();      //触发显示
        return data;
    }

    public UIDrawerData CreateWidget(string id, int x, int y, int prefabID)
    {
        UIDrawerData data = new UIDrawerData();
        data.xPos = x;
        data.yPos = y;
        GameObject labelPrefab;
        if (!mPrefabs.TryGetValue(prefabID, out labelPrefab))
        {
            Debug.LogError("Use Undefine LabelID : " + prefabID.ToString() + " When Draw " + id);
            return null;
        }
        GameObject labelObj = GameObject.Instantiate(labelPrefab) as GameObject;
        data.uiWidget = labelObj.GetComponents<UIWidget>()[0];
        labelObj.transform.parent = UIWindowManager.Singleton.AnchorObject[(int)DefaultAnchor].transform;
        labelObj.transform.localPosition = new Vector3(x, -y, 0.0f);

        UIToolkits.SetUILayer(labelObj, 0);

        data.IsExistMode = true;
        mDrawDataMap[id] = data;
        return data;
    }

    public void DestoryObj(string id)
    {
        UIDrawerData data;
        if (!mDrawDataMap.TryGetValue(id, out data))
        {
            Debug.LogWarning("Remove an not exist obj id = " + id);
        }
        GameObject.Destroy(data.uiWidget.gameObject);
        mDrawDataMap.Remove(id);
    }

    public void DrawText(string id, int x, int y, string text, int labelID, float alpha)
    {
        UIDrawerData data;
        if (!mDrawDataMap.TryGetValue(id, out data))
        {
            data = CreateText(id, x, y, text, labelID);
            data.IsExistMode = false;
            data.uiWidget.transform.localPosition = new Vector3(x, -y, -500f);
        }
        UILabel label = data.uiWidget as UILabel;
        if (label.text != text)
        {
            label.text = text;
        }
        label.Alpha(alpha);
        if (data.xPos != x || data.yPos != y)
        {
            data.xPos = x;
            data.yPos = y;
            data.uiWidget.transform.localPosition = new Vector3(x, -y, -500f);
        }
        data.DidDrawLastFrame = true;
    }

    public void DrawText(string id, int x, int y, string text)
    {
        DrawText(id, x, y, text, fontDefaultPrefabID, 1f);
    }

    public void DrawText(string stringKeyName, int x, int y)
    {
        DrawText(stringKeyName, x, y, TextTable.Singleton.GetString(stringKeyName), fontDefaultPrefabID, 1f);
    }

    public void DrawText(string id, int x, int y, string text, float alpha)
    {
        DrawText(id, x, y, text, fontDefaultPrefabID, alpha);
    }

    public UISprite DrawSprite(string id, int x, int y, string spriteName, int prefabID)
    {
        return DrawSprite(id, new Rect(x, y, 0, 0), spriteName, prefabID, true, 0);
    }

    public UISprite DrawSprite(string id, Rect rect, string spriteName, int prefabID, bool makePixedPerfect, int depth)
    {

        var x = (int)rect.x;
        var y = (int)rect.y;
        UIDrawerData data;
        if (!mDrawDataMap.TryGetValue(id, out data))
        {
            data = CreateSprite(id, (int)rect.x, (int)rect.y, spriteName, prefabID);
            data.IsExistMode = false;
        }
        var sprite = data.uiWidget as UISprite;
        sprite.depth = depth;
        if (makePixedPerfect)
        {
            sprite.MakePixelPerfect();
        }
        else
        {
            data.uiWidget.gameObject.transform.localScale = new Vector3(rect.width, rect.height, 1f);
        }

        if (data.xPos != x || data.yPos != y)
        {
            data.xPos = x;
            data.yPos = y;
            data.uiWidget.transform.localPosition = new Vector3(x, -y, -100f);
        }
        data.DidDrawLastFrame = true;
        return sprite;
    }

    public UISprite DrawSprite(string id, Rect rect, string spriteName, int prefabID)
    {
        return DrawSprite(id, rect, spriteName, prefabID, false, 0);
    }

    public UISprite DrawSprite(string id, Rect rect, string spriteName, int depth, int prefabID)
    {
        return DrawSprite(id, rect, spriteName, prefabID, false, depth);
    }

    public UISprite DrawSprite(string id, Rect rect, string spriteName, int depth, int prefabID, bool makePixedPerfect)
    {
        return DrawSprite(id, rect, spriteName, prefabID, makePixedPerfect, depth);
    }

    public UISprite DrawSprite(string id, int x, int y, string spriteName)
    {
        return DrawSprite(id, x, y, spriteName, spriteDefaultPrefabID);
    }

    public void DrawPrefab(string id, int x, int y, int labelID)
    {
        UIDrawerData data;
        if (!mDrawDataMap.TryGetValue(id, out data))
        {
            data = CreateWidget(id, x, y, labelID);
            data.IsExistMode = false;
        }
        if (data.xPos != x || data.yPos != y)
        {
            data.xPos = x;
            data.yPos = y;
            data.uiWidget.transform.localPosition = new Vector3(x, -y, 0.0f);
        }
        data.DidDrawLastFrame = true;
    }

    public void Update()
    {
        int i = 0;
        foreach (KeyValuePair<string, UIDrawerData> pair in mDrawDataMap)
        {
            pair.Value.Update();                            //更新
            //Find All The Data Which We Want To Remove
            if (!pair.Value.IsExistMode && !pair.Value.DidDrawLastFrame)
            {
                if (pair.Value.effectPlayer != null)        //若有特效
                {
                    if (pair.Value.playingState == UIWindowStateEnum.Hide)       //若特效播放完毕
                    {
                        if (i < moveDataCapibility)
                        {
                            DataToRemove[i] = pair.Key;
                            ++i;
                        }
                    }
                    else if(pair.Value.playingState == UIWindowStateEnum.Show || pair.Value.playingState == UIWindowStateEnum.PlayingShowEffect)
                    {
						pair.Value.Hide();                      //隐藏
                    }
                }
                else
                {
                    if (i < moveDataCapibility)
                    {
                        DataToRemove[i] = pair.Key;
                        ++i;
                    }
                }
            }
            pair.Value.DidDrawLastFrame = false;            //Reset the flag
        }

        //Remove The Data
        for (int j = 0; j < i; j++)
        {
            DestoryObj(DataToRemove[j]);
        }
    }
}
