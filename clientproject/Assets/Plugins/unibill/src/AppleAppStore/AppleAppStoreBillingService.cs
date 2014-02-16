//-----------------------------------------------------------------
//  Copyright 2013 Alex McAusland and Ballater Creations
//  All rights reserved
//  www.outlinegames.com
//-----------------------------------------------------------------
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unibill.Impl {

    /// <summary>
    /// App Store implementation of <see cref="IBillingService"/>.
    /// This class has platform specific logic to handle errors from Storekit,
    /// such as a nil product list being returned, and print helpful information.
    /// </summary>
    public class AppleAppStoreBillingService : IBillingService {

        private IBillingServiceCallback biller;
        private ProductIdRemapper remapper;
        private HashSet<PurchasableItem> products;
        private HashSet<string> productsNotReturnedByStorekit = new HashSet<string>();
        public IStoreKitPlugin storekit { get; private set; }

        public AppleAppStoreBillingService(UnibillConfiguration db, ProductIdRemapper mapper, IStoreKitPlugin storekit) {
            this.storekit = storekit;
            this.remapper = mapper;
            storekit.initialise(this);
            products = new HashSet<PurchasableItem>(db.AllPurchasableItems);
        }

        public void initialise (IBillingServiceCallback biller) {
            this.biller = biller;
            bool available = storekit.storeKitPaymentsAvailable ();
            if (available) {
                string[] platformSpecificProductIds = remapper.getAllPlatformSpecificProductIds();
                storekit.storeKitRequestProductData (string.Join (",", platformSpecificProductIds), platformSpecificProductIds);
            } else {
                biller.logError(UnibillError.STOREKIT_BILLING_UNAVAILABLE);
                biller.onSetupComplete(false);
            }
        }

        public void purchase (string item) {
            if (productsNotReturnedByStorekit.Contains (item)) {
                biller.logError(UnibillError.STOREKIT_ATTEMPTING_TO_PURCHASE_PRODUCT_NOT_RETURNED_BY_STOREKIT, item);
                biller.onPurchaseFailedEvent(item);
                return;
            }
            storekit.storeKitPurchaseProduct(item);
        }

        public void restoreTransactions () {
            storekit.storeKitRestoreTransactions();
        }

        public void onProductListReceived (string productListString) {
            if (productListString.Length == 0) {
                biller.logError (UnibillError.STOREKIT_RETURNED_NO_PRODUCTS);
                biller.onSetupComplete (false);
                return;
            }

            Dictionary<string, object> response = (Dictionary<string, object>)Unibill.Impl.MiniJSON.jsonDecode(productListString);
            HashSet<PurchasableItem> productsReceived = new HashSet<PurchasableItem>();
            foreach (var identifier in response.Keys) {
                var item = remapper.getPurchasableItemFromPlatformSpecificId(identifier.ToString());
                Dictionary<string, object> details = (Dictionary<string, object>)response[identifier];

                PurchasableItem.Writer.setLocalizedPrice(item, details["price"].ToString());
                PurchasableItem.Writer.setLocalizedTitle(item, details["localizedTitle"].ToString());
                PurchasableItem.Writer.setLocalizedDescription(item, details["localizedDescription"].ToString());
                productsReceived.Add(item);
            }

            HashSet<PurchasableItem> productsNotReceived = new HashSet<PurchasableItem> (products);
            productsNotReceived.ExceptWith (productsReceived);
            if (productsNotReceived.Count > 0) {
                foreach (PurchasableItem product in productsNotReceived) {
                    biller.logError(UnibillError.STOREKIT_REQUESTPRODUCTS_MISSING_PRODUCT, product.Id, remapper.mapItemIdToPlatformSpecificId(product));
                }
            }

            this.productsNotReturnedByStorekit = new HashSet<string>(productsNotReceived.Select(x => remapper.mapItemIdToPlatformSpecificId(x)));

            // We should complete so long as we have at least one purchasable product.
            biller.onSetupComplete(true);
        }
        
        public void onPurchaseSucceeded(string data) {
            Dictionary<string, object> response = (Dictionary<string, object>)Unibill.Impl.MiniJSON.jsonDecode(data);
            biller.onPurchaseSucceeded((string) response["productId"], (string) response["receipt"]);
        }
        
        public void onPurchaseCancelled(string productId) {
            biller.onPurchaseCancelledEvent(productId);
        }
        
        public void onPurchaseFailed(string productId) {
            biller.onPurchaseFailedEvent(productId);
        }
        
        public void onTransactionsRestoredSuccess() {
            biller.onTransactionsRestoredSuccess();
        }
        
        public void onTransactionsRestoredFail(string error) {
            biller.onTransactionsRestoredFail(error);
        }

        public void onFailedToRetrieveProductList() {
            biller.logError(UnibillError.STOREKIT_FAILED_TO_RETRIEVE_PRODUCT_DATA);
            biller.onSetupComplete(true); // We should still be able to buy things, assuming they are correctly setup.
        }
    }
}
