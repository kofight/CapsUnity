//-----------------------------------------------------------------
//  Copyright 2013 Alex McAusland and Ballater Creations
//  All rights reserved
//  www.outlinegames.com
//-----------------------------------------------------------------
using System;
using UnityEngine;
using System.IO;

namespace Unibill.Impl {
    public class RawAmazonAppStoreBillingInterface : IRawAmazonAppStoreBillingInterface {

#if UNITY_ANDROID
        private AndroidJavaObject amazon;
#endif

        public RawAmazonAppStoreBillingInterface (UnibillConfiguration config) {
#if UNITY_ANDROID
            if (config.CurrentPlatform == BillingPlatform.AmazonAppstore && config.AmazonSandboxEnabled) {
                string json = ((TextAsset)Resources.Load ("amazon.sdktester.json")).text;
                File.WriteAllText ("/sdcard/amazon.sdktester.json", json);
            }

            using (var pluginClass = new AndroidJavaClass("com.outlinegames.unibillAmazon.Unibill" )) {
                amazon = pluginClass.CallStatic<AndroidJavaObject> ("instance");
            }
#endif
        }

        public void initialise (AmazonAppStoreBillingService amazon) {
#if UNITY_ANDROID
            new GameObject().AddComponent<AmazonAppStoreCallbackMonoBehaviour>().initialise(amazon);
#endif
        }

        public void initiateItemDataRequest (string[] productIds) {
#if UNITY_ANDROID
            var initMethod = AndroidJNI.GetMethodID(amazon.GetRawClass(), "initiateItemDataRequest", "([Ljava/lang/String;)V" );
            AndroidJNI.CallVoidMethod(amazon.GetRawObject(), initMethod, AndroidJNIHelper.CreateJNIArgArray( new object[] { productIds }));
#endif
        }

        public void initiatePurchaseRequest (string productId) {
#if UNITY_ANDROID
            amazon.Call("initiatePurchaseRequest", productId);
#endif
        }

        public void restoreTransactions() {
#if UNITY_ANDROID
            amazon.Call("restoreTransactions");
#endif
        }
    }
}
