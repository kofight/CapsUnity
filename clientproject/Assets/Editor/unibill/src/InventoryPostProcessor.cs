//-----------------------------------------------------------------
//  Copyright 2013 Alex McAusland and Ballater Creations
//  All rights reserved
//  www.outlinegames.com
//-----------------------------------------------------------------
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Unibill;
using Unibill.Impl;
using System.IO;
using System.Xml;
using Newtonsoft.Json;

public class InventoryPostProcessor : AssetPostprocessor {
	
	public const string UNIBILL_XML_INVENTORY_PATH = "Assets/Plugins/unibill/resources/unibillInventory.xml";
    public const string UNIBILL_JSON_INVENTORY_PATH = "Assets/Plugins/unibill/resources/unibillInventory.json.txt";
	private const string UNIBILL_BACKUP_PATH = "Assets/Plugins/unibill/resources/old_inventory_delete_me.xml";
	
    static void OnPostprocessAllAssets (string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromPath) {
	
		CreateInventoryIfNecessary ();

        foreach (var s in importedAssets) {
			try {
	            if (s.Contains("unibillInventory")) {
					UnibillInjector.GetStorekitGenerator ().writeFile ();
					UnibillInjector.GetGooglePlayCSVGenerator ().writeCSV ();
					UnibillInjector.GetAmazonGenerator ().encodeAll ();
	            }
			} catch (NullReferenceException) {
				// Unity insists on throwing this on first import.
			}
        }
    }

	public static void CreateInventoryIfNecessary() {
        PortXMLInventoryIfNecessary();
		if (!File.Exists(UNIBILL_JSON_INVENTORY_PATH) && ShouldWriteInventory()) {
			AssetDatabase.CopyAsset("Assets/Plugins/unibill/static/InventoryTemplate.json", UNIBILL_JSON_INVENTORY_PATH);
		}
	}

    private static void PortXMLInventoryIfNecessary() {
        if (File.Exists(UNIBILL_XML_INVENTORY_PATH) && ! File.Exists(UNIBILL_JSON_INVENTORY_PATH)) {
            var doc = new XmlDocument();
            doc.Load(UNIBILL_XML_INVENTORY_PATH);
            var json = JsonConvert.SerializeXmlNode(doc);
            json = SimplifySchema(json);
            using (StreamWriter o = new StreamWriter(UNIBILL_JSON_INVENTORY_PATH)) {
                o.Write(json);
            }
			File.Move (UNIBILL_XML_INVENTORY_PATH, UNIBILL_BACKUP_PATH);
        }
    }

    public static string SimplifySchema(string json) {
        var dic = (Dictionary<string, object>)Unibill.Impl.MiniJSON.jsonDecode(json);
        {
            dic = (Dictionary<string, object>)dic["inventory"];
            var items = (List<object>)dic["item"];
            dic.Remove("item");

            var newItems = new Dictionary<string, object>();
            foreach (Dictionary<string, object> item in items) {
                var id = (string)item["@id"];
                item.Remove("@id");
                newItems.Add(id, item);
            }

            dic.Add("purchasableItems", newItems);
        }
        {
			if (dic.ContainsKey("currencies") && ((Dictionary<string, object>) dic["currencies"]) != null) {
	            var currencyRoot = ((Dictionary<string, object>)dic["currencies"])["currency"];
				var curs = new List<object>();
				if (currencyRoot is List<object>) {
					curs = currencyRoot as List<object>;
				} else {
					curs  = new List<object>() { currencyRoot };
				}
	            dic.Remove("currencies");
	            var currencies = new Dictionary<string, object>();
	            dic.Add("currencies", currencies);
	            foreach (Dictionary<string, object> currency in curs) {
	                var id = (string)currency["currencyId"];
	                currency.Remove("currencyId");

	                var mappings = (List<object>) ((Dictionary<string, object>)currency["mappings"])["mapping"];
	                Dictionary<string, decimal> newMappings = new Dictionary<string, decimal>();
	                foreach (Dictionary<string, object> mapping in mappings) {
	                    newMappings.Add(mapping["id"].ToString(), decimal.Parse(mapping["amount"].ToString()));
	                }
	                currency.Remove("mappings");
	                currencies.Add(id, newMappings);
	            }
			}
        }
        return Newtonsoft.Json.JsonConvert.SerializeObject(dic, Newtonsoft.Json.Formatting.Indented);
    }

    /// <summary>
    /// You may be wondering what on earth this is for.
    /// This is to finally solve the problem of people's 
    /// inventory being overwritten when they update the plugin.
    /// Given that a unitypackage is a dumb directory of files
    /// that is imported, blatting anything already there, and there
    /// is no way of excluding files when uploading to the asset store,
    /// I have to stop them existing in the directory on my machine only!
    /// 
    /// One day Unity may build a proper package management system.
    /// </summary>
    public static bool ShouldWriteInventory() {
        try {
            if (File.Exists("/tmp/B1R5SxGBA7UnmxSaW5U6qlUdOfVoa7oDV")) {
                return false;
            }
        } catch (Exception) {
        }

        return true;
    }
}
