//-----------------------------------------------------------------
//  Copyright 2013 Alex McAusland and Ballater Creations
//  All rights reserved
//  www.outlinegames.com
//-----------------------------------------------------------------
using System;
using System.Runtime.InteropServices;
using UnityEngine;
using System.Reflection;

namespace Unibill.Impl {
    public class OSXStoreKitPluginImpl : IStoreKitPlugin {

#if UNITY_STANDALONE
        [DllImport("unibillosx")]
        private static extern bool _storeKitPaymentsAvailable();
        
        [DllImport("unibillosx")]
        private static extern void _storeKitRequestProductData (string productIdentifiers);
        
        [DllImport("unibillosx")]
        private static extern void _storeKitPurchaseProduct (string productId);
        
        [DllImport("unibillosx")]
        private static extern void _storeKitRestoreTransactions();
#endif
        private static AppleAppStoreBillingService callback;

		#region IStoreKitPlugin implementation
		public void initialise (AppleAppStoreBillingService callback)
		{
			OSXStoreKitPluginImpl.callback = callback;
		}
		#endregion		
		
        public bool storeKitPaymentsAvailable () {
            #if UNITY_STANDALONE
            return _storeKitPaymentsAvailable();
            #else
            throw new NotImplementedException();
            #endif
        }
        public void storeKitRequestProductData (string productIdentifiers, string[] productIds) {
                        #if UNITY_STANDALONE
            _storeKitRequestProductData(productIdentifiers);
                        #else
            throw new NotImplementedException();
            #endif
        }
        public void storeKitPurchaseProduct (string productId) {
                        #if UNITY_STANDALONE
            _storeKitPurchaseProduct(productId);
                        #else
            throw new NotImplementedException();
            #endif
        }
        public void storeKitRestoreTransactions () {
                        #if UNITY_STANDALONE
            _storeKitRestoreTransactions();
#else
            throw new NotImplementedException();
#endif
        }
	
		// Callbacks from native.
		public static void UnibillSendMessage(string method, string argument) {
			switch (method) {
				case "onProductListReceived":
					onProductListReceived(argument);
				break;
				case "onProductPurchaseSuccess":
					onProductPurchaseSuccess(argument);
				break;
				case "onProductPurchaseCancelled":
					onProductPurchaseCancelled(argument);
				break;
				case "onProductPurchaseFailed":
					onProductPurchaseFailed(argument);
				break;
				case "onTransactionsRestoredSuccess":
					onTransactionsRestoredSuccess(argument);
				break;
				case "onTransactionsRestoredFail":
					onTransactionsRestoredFail(argument);
				break;
				case "onFailedToRetrieveProductList":
					onFailedToRetrieveProductList(argument);
				break;
			}
		}
			
		public static void onProductListReceived(string productList) {
	        callback.onProductListReceived(productList);
	    }
	
	    public static void onProductPurchaseSuccess(string productId) {
	        callback.onPurchaseSucceeded(productId);
	    }
	
	    public static void onProductPurchaseCancelled(string productId) {
	        callback.onPurchaseCancelled(productId);
	    }
	
	    public static void onProductPurchaseFailed(string productId) {
	        callback.onPurchaseFailed(productId);
	    }
	
	    public static void onTransactionsRestoredSuccess(string empty) {
	        callback.onTransactionsRestoredSuccess();
	    }
	
	    public static void onTransactionsRestoredFail(string error) {
	        callback.onTransactionsRestoredFail(error);
	    }
	
	    public static void onFailedToRetrieveProductList(string nop) {
	        callback.onFailedToRetrieveProductList();
	    }
    }
}

