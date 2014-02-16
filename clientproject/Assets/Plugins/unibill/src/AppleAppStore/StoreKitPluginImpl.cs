//-----------------------------------------------------------------
//  Copyright 2013 Alex McAusland and Ballater Creations
//  All rights reserved
//  www.outlinegames.com
//-----------------------------------------------------------------
using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Unibill.Impl {
    public class StoreKitPluginImpl : IStoreKitPlugin {

#if UNITY_IOS
        [DllImport("__Internal")]
        private static extern bool _storeKitPaymentsAvailable();
        
        [DllImport("__Internal")]
        private static extern void _storeKitRequestProductData (string productIdentifiers);
        
        [DllImport("__Internal")]
        private static extern void _storeKitPurchaseProduct (string productId);
        
        [DllImport("__Internal")]
        private static extern void _storeKitRestoreTransactions();
#endif
        public void initialise(AppleAppStoreBillingService svc) {
            GameObject host = new GameObject();
            host.AddComponent<AppleAppStoreCallbackMonoBehaviour>().initialise(svc);
        }

        public bool storeKitPaymentsAvailable () {
            #if UNITY_IOS
            return _storeKitPaymentsAvailable();
            #else
            throw new NotImplementedException();
            #endif
        }
        public void storeKitRequestProductData (string productIdentifiers, string[] productIds) {
            #if UNITY_IOS
            _storeKitRequestProductData(productIdentifiers);
            #else
            throw new NotImplementedException();
            #endif
        }
        public void storeKitPurchaseProduct (string productId) {
            #if UNITY_IOS
            _storeKitPurchaseProduct(productId);
            #else
            throw new NotImplementedException();
            #endif
        }
        public void storeKitRestoreTransactions () {
            #if UNITY_IOS
            _storeKitRestoreTransactions();
#else
            throw new NotImplementedException();
#endif
        }
    }
}

