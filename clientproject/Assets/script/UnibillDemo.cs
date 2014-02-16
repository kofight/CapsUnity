//-----------------------------------------------------------------
//  Copyright 2013 Alex McAusland and Ballater Creations
//  All rights reserved
//  www.outlinegames.com
//-----------------------------------------------------------------
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using Unibill.Demo;

/// <summary>
/// An example of basic Unibill functionality.
/// </summary>
[AddComponentMenu("Unibill/UnibillDemo")]
public class UnibillDemo : MonoBehaviour {

    private ComboBox box;
    private GUIContent[] comboBoxList;
    private GUIStyle listStyle;
    private int selectedItemIndex;
    private PurchasableItem[] items;
    public Font font;

    void Start () {

        if (UnityEngine.Resources.Load ("unibillInventory.json") == null) {
            Debug.LogError("You must define your purchasable inventory within the inventory editor!");
            this.gameObject.SetActive(false);
            return;
        }

        // We must first hook up listeners to Unibill's events.
        Unibiller.onBillerReady += onBillerReady;
        Unibiller.onTransactionsRestored += onTransactionsRestored;
        Unibiller.onPurchaseCancelled += onCancelled;
    Unibiller.onPurchaseFailed += onFailed;
        Unibiller.onPurchaseComplete += onPurchased;

        // Now we're ready to initialise Unibill.
        Unibiller.Initialise();

        initCombobox();
    }
    
    /// <summary>
    /// This will be called when Unibill has finished initialising.
    /// </summary>
    private void onBillerReady(UnibillState state) {
        UnityEngine.Debug.Log("onBillerReady:" + state);
    }

    /// <summary>
    /// This will be called after a call to Unibiller.restoreTransactions().
    /// </summary>
    private void onTransactionsRestored (bool success) {
        Debug.Log("Transactions restored.");
    }

    /// <summary>
    /// This will be called when a purchase completes.
    /// </summary>
    private void onPurchased(PurchasableItem item) {
        Debug.Log("Purchase OK: " + item.Id);
        Debug.Log(string.Format ("{0} has now been purchased {1} times.",
                                 item.name,
                                 Unibiller.GetPurchaseCount(item)));
    }

    /// <summary>
    /// This will be called if a user opts to cancel a purchase
    /// after going to the billing system's purchase menu.
    /// </summary>
    private void onCancelled(PurchasableItem item) {
        Debug.Log("Purchase cancelled: " + item.Id);
    }
    
    /// <summary>
    /// This will be called is an attempted purchase fails.
    /// </summary>
    private void onFailed(PurchasableItem item) {
    Debug.Log("Purchase failed: " + item.Id);
    }

    private void initCombobox() {
        box = new ComboBox();
        items = Unibiller.AllPurchasableItems;
        comboBoxList = new GUIContent[items.Length];
        for (int t = 0; t < items.Length; t++) {
            comboBoxList[t] = new GUIContent(string.Format("{0} - {1}", items[t].localizedTitle, items[t].localizedPriceString));
        }
        
        listStyle = new GUIStyle();
        listStyle.font = font;
        listStyle.normal.textColor = Color.white; 
        listStyle.onHover.background =
            listStyle.hover.background = new Texture2D(2, 2);
        listStyle.padding.left =
            listStyle.padding.right =
                listStyle.padding.top =
                listStyle.padding.bottom = 4;
    }

    public void Update() {
        for (int t = 0; t < items.Length; t++) {
            comboBoxList[t] = new GUIContent(string.Format("{0} - {1} - {2}", items[t].name, items[t].localizedTitle, items[t].localizedPriceString));
        }
    }

    void OnGUI () {
        selectedItemIndex = box.GetSelectedItemIndex ();
        selectedItemIndex = box.List (new Rect (0, 0, Screen.width, Screen.width / 20.0f), comboBoxList [selectedItemIndex].text, comboBoxList, listStyle);
        if (GUI.Button (new Rect (0, Screen.height - Screen.width / 6.0f, Screen.width / 2.0f, Screen.width / 6.0f), "Buy")) {
            Unibiller.initiatePurchase(items[selectedItemIndex]);
        }

        if (GUI.Button (new Rect (Screen.width / 2.0f, Screen.height - Screen.width / 6.0f, Screen.width / 2.0f, Screen.width / 6.0f), "Restore transactions")) {
            Unibiller.restoreTransactions();
        }

        // Draw the purchase names for our various purchasables.
        int start = (int) (Screen.height - Screen.width / 6.0f) - 50;
        foreach (PurchasableItem item in Unibiller.AllNonConsumablePurchasableItems) {
            GUI.Label(new Rect(0, start, 500, 50), item.Id, listStyle);
			GUI.Label(new Rect(Screen.width - Screen.width * 0.1f, start, 500, 50), Unibiller.GetPurchaseCount(item).ToString(), listStyle);
            start -= 30;
        }
		
		foreach (string currencyId in Unibiller.AllCurrencies) {
            GUI.Label(new Rect(0, start, 500, 50), currencyId, listStyle);
			GUI.Label(new Rect(Screen.width - Screen.width * 0.1f, start, 500, 50), Unibiller.GetCurrencyBalance(currencyId).ToString(), listStyle);
            start -= 30;
        }

        GUI.Label(new Rect(0, start - 10, 500, 50), "Item", listStyle);


        GUI.Label(new Rect(Screen.width - Screen.width * 0.2f, start - 10, 500, 50), "Count", listStyle);
    }
}
