//-----------------------------------------------------------------
//  Copyright 2013 Alex McAusland and Ballater Creations
//  All rights reserved
//  www.outlinegames.com
//-----------------------------------------------------------------
using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using Unibill;
using Unibill.Impl;
using System.Diagnostics;
using Newtonsoft.Json;

public enum GooglePlayLocale {
    zh_TW,
    cs_CZ,
    da_DK,
    nl_NL,
    en_US,
    fr_FR,
    fi_FI,
    de_DE,
    iw_IL,
    hi_IN,
    it_IT,
    ja_JP,
    ko_KR,
    no_NO,
    pl_PL,
    pt_PT,
    ru_RU,
    es_ES,
    sv_SE,
    en_GB,
}

public class InventoryEditor : EditorWindow {
    bool groupEnabled;

    private const string SCREENSHOT_PATH = "Assets/Plugins/unibill/screenshots";
    private XPathDocument doc;
    private static List<GUIPurchasable> items = new List<GUIPurchasable>();
    private static List<GUIPurchasable> toRemove = new List<GUIPurchasable>();
	private string[] androidBillingPlatforms = new string[] {
		BillingPlatform.GooglePlay.ToString(),
		BillingPlatform.AmazonAppstore.ToString(),
		BillingPlatform.SamsungApps.ToString()
	};
	private UnibillCurrencyEditor currencyEditor;
    private UnibillConfiguration config;
    private int androidBillingPlatform;

	public static List<GUIPurchasable> Items {
		get {
			return items;
		}
	}

    public void OnEnable () {
		items = new List<GUIPurchasable> ();
		toRemove = new List<GUIPurchasable> ();
		InventoryPostProcessor.CreateInventoryIfNecessary ();
		AndroidManifestGenerator.CreateManifestIfNecessary ();
        using (TextReader reader = File.OpenText(InventoryPostProcessor.UNIBILL_JSON_INVENTORY_PATH)) {
            config = new UnibillConfiguration(reader.ReadToEnd(), Application.platform, new Uniject.Impl.UnityLogger());
        }
        for (int t = 0; t < androidBillingPlatforms.Count(); t++) {
            if (androidBillingPlatforms[t] == config.AndroidBillingPlatform.ToString()) {
                androidBillingPlatform = t;
                break;
            }
        }
        foreach (PurchasableItem element in config.inventory) {

            List<IPlatformEditor> editors = new List<IPlatformEditor>();
            editors.Add(new GooglePlayEditor(element));
            editors.Add(new DefaultPlatformEditor(element, BillingPlatform.AmazonAppstore));
            editors.Add(new AppleAppStoreEditor(element));
            editors.Add(new DefaultPlatformEditor(element, BillingPlatform.MacAppStore));
            editors.Add(new DefaultPlatformEditor(element, BillingPlatform.WindowsPhone8));
            editors.Add(new DefaultPlatformEditor(element, BillingPlatform.Windows8_1));
			editors.Add (new DefaultPlatformEditor (element, BillingPlatform.SamsungApps));
            items.Add(new GUIPurchasable(element, editors));
        }

		currencyEditor = new UnibillCurrencyEditor (config);
    }
    
    // Add menu item named "My Window" to the Window menu
    [MenuItem("Window/Unibill/Inventory Editor")]
    public static void ShowWindow()
    {
        //Show existing window instance. If one doesn't exist, make one.
        EditorWindow.GetWindow(typeof(InventoryEditor));
    }

    [MenuItem("Window/Unibill/Take Screenshot")]   
    static void screenShot () {
        DirectoryInfo d = new DirectoryInfo(SCREENSHOT_PATH);
        if (!d.Exists) {
            d.Create();
        }
        string path = Path.Combine(SCREENSHOT_PATH, ((long) new TimeSpan(DateTime.Now.Ticks).TotalSeconds).ToString() + ".png");
        Application.CaptureScreenshot(path);
        AssetDatabase.ImportAsset(path);
    }

#if UNITY_ANDROID
    [MenuItem("Window/Unibill/Install Amazon Test client")]   
    static void root () {
        Process p = new Process ();
        
        string adbLocation = Path.Combine(EditorPrefs.GetString ("AndroidSdkRoot"), "platform-tools/adb");
        
        FileInfo adb = new FileInfo (adbLocation);
        
        if (!adb.Exists) {
            adb = new FileInfo(adbLocation + ".exe");
            if (!adb.Exists) {
                UnityEngine.Debug.LogError("Unable to find adb. Verify that your Android SDK location is set correctly in Unity.");
                return;
            }
        }

        p.StartInfo.FileName = adb.FullName;
        string apkPath = new FileInfo(string.Format("Assets{0}Plugins{0}unibill{0}static{0}AmazonSDKTester.apk", Path.DirectorySeparatorChar)).FullName;

		p.StartInfo.Arguments = string.Format("install \"{0}\"", apkPath);
        p.Start();
    }
#endif

    private Vector2 scrollPosition = new Vector2();
    void OnGUI () {

		scrollPosition = EditorGUILayout.BeginScrollView (scrollPosition, false, false, 
		                                                  GUI.skin.horizontalScrollbar, GUI.skin.verticalScrollbar, GUI.skin.box);

        if (GUILayout.Button ("Save")) {
            serialise ();
        }
        EditorGUILayout.Space();

        EditorGUILayout.BeginVertical (GUI.skin.box);
        EditorGUILayout.LabelField ("Configuration settings:");
        EditorGUILayout.Space();
        
        EditorGUI.BeginChangeCheck();
        androidBillingPlatform = EditorGUILayout.Popup("Android billing platform:", androidBillingPlatform, androidBillingPlatforms, new GUILayoutOption[0]);
        config.AndroidBillingPlatform = (BillingPlatform) Enum.Parse(typeof(BillingPlatform), androidBillingPlatforms[androidBillingPlatform]);
        config.AmazonSandboxEnabled = EditorGUILayout.Toggle("Use Amazon sandbox:", config.AmazonSandboxEnabled);
        config.WP8SandboxEnabled = EditorGUILayout.Toggle("Use mock Windows Phone environment:", config.WP8SandboxEnabled);
        config.UseWin8_1Sandbox = EditorGUILayout.Toggle("Use mock Windows 8", config.UseWin8_1Sandbox);
		config.SamsungAppsMode = (SamsungAppsMode) EditorGUILayout.EnumPopup ("Samsung Apps mode:", config.SamsungAppsMode);
		config.SamsungItemGroupId = EditorGUILayout.TextField ("Samsung Apps Item Group ID:", config.SamsungItemGroupId);
        if (EditorGUI.EndChangeCheck()) {
            serialise();
        }

		EditorGUILayout.BeginHorizontal();
		config.UseHostedConfig = EditorGUILayout.Toggle("Use hosted config", config.UseHostedConfig);
		config.HostedConfigUrl = EditorGUILayout.TextField(config.HostedConfigUrl);
		EditorGUILayout.EndHorizontal();
        
        config.GooglePlayPublicKey = EditorGUILayout.TextField ("Google play public key:", config.GooglePlayPublicKey);
        config.iOSSKU = EditorGUILayout.TextField ("iOS SKU:", config.iOSSKU);

        EditorGUILayout.EndVertical();
        EditorGUILayout.Space();

        EditorGUILayout.LabelField ("Purchasable items:");
        EditorGUILayout.Space();

        foreach (GUIPurchasable item in items) {
            EditorGUILayout.BeginVertical (GUI.skin.box);
            item.OnGUI ();
            EditorGUILayout.EndVertical ();
        }
        if (GUILayout.Button ("Add item...")) {
            items.Add (GUIPurchasable.CreateInstance (config.AddItem()));
        }

		currencyEditor.onGUI ();

        EditorGUILayout.EndScrollView ();

        items.RemoveAll(x => toRemove.Contains(x));
        foreach (var item in toRemove) {
            config.inventory.Remove(item.item.item);
        }
        toRemove.Clear();
    }

    private void serialise () {
        using (StreamWriter o = new StreamWriter(InventoryPostProcessor.UNIBILL_JSON_INVENTORY_PATH)) {
            var json = JsonConvert.SerializeObject(config.Serialize(), Newtonsoft.Json.Formatting.Indented);
            o.Write(json);
        }

		try {
	        AssetDatabase.ImportAsset(InventoryPostProcessor.UNIBILL_JSON_INVENTORY_PATH);
		} catch(Exception) {
		}

		UnibillInjector.GetStorekitGenerator ().writeFile ();
		UnibillInjector.GetGooglePlayCSVGenerator ().writeCSV ();
		UnibillInjector.GetAmazonGenerator ().encodeAll ();

        AssetDatabase.ImportAsset("Assets/Plugins/unibill/resources/amazon.sdktester.json.txt");
        AndroidManifestGenerator.mergeManifest();
    }

	public class GUIPurchasable {

        public WritablePurchasable item;
        public bool visible { get; private set; }

        private bool[] platformVisibility = new bool[Enum.GetNames(typeof(BillingPlatform)).Length];
        public List<IPlatformEditor> editors { get; private set; }

        public static GUIPurchasable CreateInstance(PurchasableItem item) {
            
            List<IPlatformEditor> editors = new List<IPlatformEditor>();
            editors.Add(new GooglePlayEditor(item));
            editors.Add(new DefaultPlatformEditor(item, BillingPlatform.AmazonAppstore));
            editors.Add(new AppleAppStoreEditor(item));
            editors.Add(new DefaultPlatformEditor(item, BillingPlatform.MacAppStore));
            editors.Add(new DefaultPlatformEditor(item, BillingPlatform.WindowsPhone8));
            editors.Add(new DefaultPlatformEditor(item, BillingPlatform.Windows8_1));
			editors.Add(new DefaultPlatformEditor(item, BillingPlatform.SamsungApps));
            return new GUIPurchasable(item, editors);
        }

        public GUIPurchasable(PurchasableItem item, List<IPlatformEditor> editors) {
            this.item = new WritablePurchasable(item);
            this.editors = editors;
        }

        public void OnGUI () {
            GUIStyle s = new GUIStyle (EditorStyles.foldout);
            var box = EditorGUILayout.BeginVertical ();

            Rect rect = new Rect (box.xMax - 20, box.yMin - 2, 20, 20);
            if (GUI.Button (rect, "x")) {
                toRemove.Add(this);
            }

            this.visible = EditorGUILayout.Foldout (visible, item.name, s);

            if (visible) {
                item.PurchaseType = (PurchaseType)EditorGUILayout.EnumPopup ("Purchase type:", item.PurchaseType, new GUILayoutOption[0]);
                item.Id = EditorGUILayout.TextField ("Id:", item.Id);
                item.name = EditorGUILayout.TextField ("Name:", item.name);
                item.description = EditorGUILayout.TextField ("Description:", item.description);

                int t = 0;
                foreach (var editor in editors) {
                    EditorGUILayout.BeginVertical (GUI.skin.box);
                    platformVisibility [t] = EditorGUILayout.Foldout (platformVisibility [t], editor.DisplayName());
                    if (platformVisibility [t]) {
                        editors [t].onGUI ();
                    }
                    EditorGUILayout.EndVertical ();
                    t++;
                }
            }

            EditorGUILayout.EndVertical ();
        }
    }

    public interface IPlatformEditor {
        void onGUI();

        string DisplayName();
    }

	public static string[] consumableIds() {
		return (from item in items
			   where item.item.PurchaseType == PurchaseType.Consumable
			   select item.item.Id).ToArray();
	}

    private class DefaultPlatformEditor : IPlatformEditor {
        private bool overridden;
        protected PurchasableItem item;
        private BillingPlatform platform;
        private string localId;

        public DefaultPlatformEditor(PurchasableItem item, BillingPlatform platform) {
            this.platform = platform;
            this.item = item;
            this.localId = item.LocalIds[platform];
            overridden = localId != item.Id;
        }

        public string DisplayName() {
            return platform.ToString();
        }

        public virtual void onGUI () {
            overridden = EditorGUILayout.BeginToggleGroup ("Override", overridden);
            if (!overridden) {
                localId = item.Id;
            }

            localId = EditorGUILayout.TextField("Id:", localId);
            string key = string.Format("{0}.Id", platform);
            if (overridden) {
                item.platformBundles[platform][key] = localId;
            }
            else {
                if (item.platformBundles[platform].ContainsKey(key)) {
                    item.platformBundles[platform].Remove(key);
                }
            }
            EditorGUILayout.EndToggleGroup ();
        }

        public virtual XElement serialise () {
            throw new NotImplementedException();
        }
    }

    private class AppleAppStoreEditor : DefaultPlatformEditor {
        private Texture2D screenshot;
        private PurchasableItem rootItem;
        public AppleAppStoreEditor(PurchasableItem rootItem) : base(rootItem, BillingPlatform.AppleAppStore) {
            this.rootItem = rootItem;
            var bundle = rootItem.platformBundles[BillingPlatform.AppleAppStore];
            if (bundle.ContainsKey("screenshotPath")) {
                var screenshotPath = (string)bundle["screenshotPath"];
                if (null != screenshotPath) {
                    screenshot = (Texture2D)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(screenshotPath), typeof(Texture2D));
                }
            }
        }

        public override void onGUI () {
            base.onGUI ();
			int priceTier = 1;
			int.TryParse(rootItem.platformBundles[BillingPlatform.AppleAppStore].getString("appleAppStorePriceTier"), out priceTier);
			rootItem.platformBundles[BillingPlatform.AppleAppStore]["appleAppStorePriceTier"] = EditorGUILayout.IntSlider("Price tier:", priceTier, 0, 85);
            screenshot = (Texture2D)EditorGUILayout.ObjectField ("Screenshot:", screenshot, typeof(Texture2D), false);
            rootItem.platformBundles[BillingPlatform.AppleAppStore]["screenshotPath"] = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(screenshot));
        }

        public override XElement serialise () {
            throw new NotImplementedException();
        }
    }

    private class GooglePlayEditor : DefaultPlatformEditor {

        private decimal priceInLocalCurrency;
        private GooglePlayLocale defaultLocale;

        public GooglePlayEditor(PurchasableItem rootItem) : base(rootItem, BillingPlatform.GooglePlay) {

            var bundle = rootItem.platformBundles[BillingPlatform.GooglePlay];
            {
                this.priceInLocalCurrency = 1;
                if (bundle.ContainsKey("priceInLocalCurrency")) {
                    decimal.TryParse(bundle["priceInLocalCurrency"].ToString(), out priceInLocalCurrency);
                }
            }

            {
                if (bundle.ContainsKey("defaultLocale")) {
                    this.defaultLocale = (GooglePlayLocale) Enum.Parse(typeof(GooglePlayLocale), (string) bundle["defaultLocale"]);
                } else {
                    this.defaultLocale = GooglePlayLocale.en_US;
                }
            }
        }

        public override void onGUI () {
            base.onGUI ();
            priceInLocalCurrency = (decimal) EditorGUILayout.FloatField ("Price in your local currency:", (float) priceInLocalCurrency);
            item.platformBundles[BillingPlatform.GooglePlay]["priceInLocalCurrency"] = priceInLocalCurrency;
            this.defaultLocale = (GooglePlayLocale) EditorGUILayout.EnumPopup ("Default locale:", defaultLocale);
            item.platformBundles[BillingPlatform.GooglePlay]["defaultLocale"] = defaultLocale.ToString();
        }
        
        public override XElement serialise () {
            XElement element = base.serialise ();
            element.Add(new XElement("priceInLocalCurrency", priceInLocalCurrency));
            element.Add(new XElement("defaultLocale", defaultLocale));
            return element;
        }
    }
}
