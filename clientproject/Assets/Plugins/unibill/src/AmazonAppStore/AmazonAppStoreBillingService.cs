//-----------------------------------------------------------------
//  Copyright 2013 Alex McAusland and Ballater Creations
//  All rights reserved
//  www.outlinegames.com
//-----------------------------------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;
using Unibill.Impl;
using Uniject;

namespace Unibill.Impl {
    public class AmazonAppStoreBillingService : IBillingService {
        #region IBillingService implementation

        private IBillingServiceCallback callback;
        private ProductIdRemapper remapper;
        private UnibillConfiguration db;
        private ILogger logger;
        private IRawAmazonAppStoreBillingInterface amazon;
        private HashSet<string> unknownAmazonProducts = new HashSet<string>();
        private TransactionDatabase tDb;

        public AmazonAppStoreBillingService(IRawAmazonAppStoreBillingInterface amazon, ProductIdRemapper remapper, UnibillConfiguration db, TransactionDatabase tDb, ILogger logger) {
            this.remapper = remapper;
            this.db = db;
            this.logger = logger;
            logger.prefix = "UnibillAmazonBillingService";
            this.amazon = amazon;
            this.tDb = tDb;
        }

        public void initialise (IBillingServiceCallback biller) {
            this.callback = biller;
            amazon.initialise(this);
            amazon.initiateItemDataRequest(remapper.getAllPlatformSpecificProductIds());
        }

        public void purchase (string item) {
            if (unknownAmazonProducts.Contains (item)) {
                callback.logError(UnibillError.AMAZONAPPSTORE_ATTEMPTING_TO_PURCHASE_PRODUCT_NOT_RETURNED_BY_AMAZON, item);
                callback.onPurchaseFailedEvent(item);
                return;
            }
            amazon.initiatePurchaseRequest(item);
        }

        public void restoreTransactions () {
            amazon.restoreTransactions();
        }

        #endregion

        public void onSDKAvailable (string isSandbox) {
            bool sandbox = bool.Parse (isSandbox);
            logger.Log("Running against {0} Amazon environment", sandbox ? "SANDBOX" : "PRODUCTION");
        }

        public void onGetItemDataFailed() {
            this.callback.logError(UnibillError.AMAZONAPPSTORE_GETITEMDATAREQUEST_FAILED);
            callback.onSetupComplete(true);
        }

        public void onProductListReceived (string productListString) {

            Dictionary<string, object> response = (Dictionary<string, object>)Unibill.Impl.MiniJSON.jsonDecode(productListString);

            if (response.Count == 0) {
                callback.logError (UnibillError.AMAZONAPPSTORE_GETITEMDATAREQUEST_NO_PRODUCTS_RETURNED);
                callback.onSetupComplete (false);
                return;
            }

            HashSet<PurchasableItem> productsReceived = new HashSet<PurchasableItem>();
            foreach (var identifier in response.Keys) {
                var item = remapper.getPurchasableItemFromPlatformSpecificId(identifier.ToString());
                Dictionary<string, object> details = (Dictionary<string, object>)response[identifier];
                
                PurchasableItem.Writer.setLocalizedPrice(item, details["price"].ToString());
                PurchasableItem.Writer.setLocalizedTitle(item, (string) details["localizedTitle"]);
                PurchasableItem.Writer.setLocalizedDescription(item, (string) details["localizedDescription"]);
                productsReceived.Add(item);
            }
            
            HashSet<PurchasableItem> productsNotReceived = new HashSet<PurchasableItem> (db.AllPurchasableItems);
            productsNotReceived.ExceptWith (productsReceived);
            if (productsNotReceived.Count > 0) {
                foreach (PurchasableItem product in productsNotReceived) {
                    this.unknownAmazonProducts.Add(remapper.mapItemIdToPlatformSpecificId(product));
                    callback.logError(UnibillError.AMAZONAPPSTORE_GETITEMDATAREQUEST_MISSING_PRODUCT, product.Id, remapper.mapItemIdToPlatformSpecificId(product));
                }
            }

            callback.onSetupComplete(true);
        }

        public void onUserIdRetrieved (string userId) {
            tDb.UserId = userId;
        }

        public void onTransactionsRestored (string successString) {
            bool success = bool.Parse(successString);
            if (success) {
                callback.onTransactionsRestoredSuccess();
            } else {
                callback.onTransactionsRestoredFail(string.Empty);
            }
        }

        public void onPurchaseFailed(string item) {
            callback.onPurchaseFailedEvent(item);
        }

        public void onPurchaseCancelled(string item) {
            callback.onPurchaseCancelledEvent(item);
        }

        public void onPurchaseSucceeded(string json) {
            Dictionary<string, object> response = (Dictionary<string, object>)Unibill.Impl.MiniJSON.jsonDecode(json);

            string productId = (string) response ["productId"];
            string token = (string) response ["purchaseToken"];
            callback.onPurchaseSucceeded (productId, token);
        }

        public void onPurchaseUpdateFailed () {
            logger.LogWarning("AmazonAppStoreBillingService: onPurchaseUpdate() failed.");
        }

        public void onPurchaseUpdateSuccess (string data) {
            var revoked = new List<string>();
            var purchased = new List<string>();
            parsePurchaseUpdates(revoked, purchased, data);
            onPurchaseUpdateSucceeded(revoked, purchased);
        }

        public void onPurchaseUpdateSucceeded (List<string> revoked, List<string> purchased) {
            foreach (string r in revoked) {
                callback.onPurchaseRefundedEvent(r);
            }

            foreach (string p in purchased) {
                callback.onPurchaseSucceeded(p);
            }
        }

        public static void parsePurchaseUpdates(List<string> revoked, List<string> purchased, string data) {
            string[] splits = data.Split('|');
            revoked.AddRange(splits[0].Split(','));
            purchased.AddRange(splits[1].Split(','));
            revoked.RemoveAll(x => x == string.Empty);
            purchased.RemoveAll(x => x == string.Empty);
        }
    }
}
