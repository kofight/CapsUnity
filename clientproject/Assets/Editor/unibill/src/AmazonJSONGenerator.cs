//-----------------------------------------------------------------
//  Copyright 2013 Alex McAusland and Ballater Creations
//  All rights reserved
//  www.outlinegames.com
//-----------------------------------------------------------------
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using Unibill;
using Unibill.Impl;

public class AmazonJSONGenerator {

    private ProductIdRemapper remapper;
    public AmazonJSONGenerator (ProductIdRemapper remapper) {
        this.remapper = remapper;
        remapper.initialiseForPlatform(BillingPlatform.AmazonAppstore);
    }

    public void encodeAll () {
        var result = new Dictionary<string, object>();
		foreach (PurchasableItem item in remapper.db.AllPurchasableItems) {
            result[remapper.mapItemIdToPlatformSpecificId (item)] = purchasableDetailsToHashtable (item);
        }

        var json = result.nJson();
		using (StreamWriter o = new StreamWriter("Assets/Plugins/unibill/resources/amazon.sdktester.json.txt")) {
			o.Write(json);
		}
    }

    public Hashtable purchasableDetailsToHashtable (PurchasableItem item) {
        var dic = new Hashtable();
        dic ["itemType"] = item.PurchaseType == PurchaseType.Consumable ? "CONSUMABLE" : item.PurchaseType == PurchaseType.NonConsumable ? "ENTITLED" : "SUBSCRIPTION";
        dic ["title"] = item.name == null ? string.Empty : item.name;
        dic ["description"] = item.description == null ? string.Empty : item.description;
        dic["price"] = 0.99;
        dic ["smallIconUrl"] = "http://example.com";
        if (PurchaseType.Subscription == item.PurchaseType) {
            dic["subscriptionParent"] = "does.not.exist";
        }

        return dic;
    }

}
