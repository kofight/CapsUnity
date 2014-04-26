using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using unibill.Dummy;
using Uniject;

namespace Unibill.Impl {
    /// <summary>
    /// Handles Windows 8.1.
    /// </summary>
    class Win8_1BillingService : IBillingService, IWindowsIAPCallback {

        #if UNITY_METRO
        private IWindowsIAP wp8;
        private IBillingServiceCallback callback;
        private UnibillConfiguration db;
        private TransactionDatabase tDb;
        private ProductIdRemapper remapper;
        private ILogger logger;
        private HashSet<string> unknownProducts = new HashSet<string>();
        #endif

        public Win8_1BillingService(IWindowsIAP wp8,
                                 UnibillConfiguration config,
                                 ProductIdRemapper remapper,
                                 TransactionDatabase tDb,
                                 ILogger logger) {
            #if UNITY_METRO
            this.wp8 = wp8;
            this.db = config;
            this.tDb = tDb;
            this.remapper = remapper;
            this.logger = logger;
            #endif
        }

        public void initialise(IBillingServiceCallback biller) {
            #if UNITY_METRO
            this.callback = biller;
            init(0);
            #endif
        }

        private void init(int delay) {
            #if UNITY_METRO
            UnityEngine.WSA.Application.InvokeOnUIThread(() => {
                wp8.Initialise(this, delay);
            }, false);
            #endif
        }

        public void purchase(string item) {
            #if UNITY_METRO
            if (unknownProducts.Contains(item)) {
                callback.logError(UnibillError.WIN_8_1_ATTEMPTING_TO_PURCHASE_PRODUCT_NOT_RETURNED_BY_MICROSOFT, item);
                callback.onPurchaseFailedEvent(item);
                return;
            }

            UnityEngine.WSA.Application.InvokeOnUIThread(() => {
                wp8.Purchase(item);
            }, false);
            #endif
        }

        public void restoreTransactions() {
            #if UNITY_METRO
            enumerateLicenses();
            callback.onTransactionsRestoredSuccess();
            #endif
        }

        public void enumerateLicenses() {
            #if UNITY_METRO
            UnityEngine.WSA.Application.InvokeOnUIThread(() => {
                wp8.EnumerateLicenses();
            }, false);
            #endif
        }

        public void logError(string error) {
            #if UNITY_METRO
            // Uncomment to get diagnostics printed on screen.
            logger.LogError(error);
            #endif
        }

        public void OnProductListReceived(Product[] products) {
            #if UNITY_METRO
            UnityEngine.WSA.Application.InvokeOnAppThread(() => {
                if (products.Length == 0) {
                    callback.logError(UnibillError.WIN_8_1_NO_PRODUCTS_RETURNED);
                    callback.onSetupComplete(false);
                    return;
                }

                HashSet<string> productsReceived = new HashSet<string>();

                foreach (var product in products) {
                    if (remapper.canMapProductSpecificId(product.Id)) {
                        productsReceived.Add(product.Id);
                        var item = remapper.getPurchasableItemFromPlatformSpecificId(product.Id);

                        PurchasableItem.Writer.setLocalizedPrice(item, product.Price);
                        PurchasableItem.Writer.setLocalizedTitle(item, product.Title);
                        PurchasableItem.Writer.setLocalizedDescription(item, product.Description);
                    }
                    else {
                        logger.LogError("Warning: Unknown product identifier: {0}", product.Id);
                    }
                }

                unknownProducts = new HashSet<string>(db.AllNonSubscriptionPurchasableItems.Select(x => remapper.mapItemIdToPlatformSpecificId(x)));
                unknownProducts.ExceptWith(productsReceived);
                if (unknownProducts.Count > 0) {
                    foreach (var id in unknownProducts) {
                        callback.logError(UnibillError.WIN_8_1_MISSING_PRODUCT, id, remapper.getPurchasableItemFromPlatformSpecificId(id).Id);
                    }
                }

                enumerateLicenses();
                callback.onSetupComplete(true);
            }, false);
            #endif
        }

        public void log(string message) {
            #if UNITY_METRO
            UnityEngine.WSA.Application.InvokeOnAppThread(() => {
                logger.Log(message);
            }, false);
            #endif
        }

        public void OnPurchaseFailed(string productId, string error) {
            #if UNITY_METRO
            UnityEngine.WSA.Application.InvokeOnAppThread(() => {
                logger.LogError("Purchase failed: {0}, {1}", productId, error);
                callback.onPurchaseFailedEvent(productId);
            }, false);
            #endif
        }

        public void OnPurchaseCancelled(string productId) {
            #if UNITY_METRO
            UnityEngine.WSA.Application.InvokeOnAppThread(() => {
                callback.onPurchaseCancelledEvent(productId);
            }, false);
            #endif
        }

        private static int count;
        public void OnPurchaseSucceeded(string productId, string receipt) {
            #if UNITY_METRO
            UnityEngine.WSA.Application.InvokeOnAppThread(() => {
                logger.LogError("PURCHASE SUCCEEDED!:{0}", count++);
                if (!remapper.canMapProductSpecificId(productId)) {
                    logger.LogError("Purchased unknown product: {0}. Ignoring!", productId);
                    return;
                }
                var details = remapper.getPurchasableItemFromPlatformSpecificId(productId);
                switch (details.PurchaseType) {
                    case PurchaseType.Consumable:
                        callback.onPurchaseSucceeded(productId, receipt);
                        break;
                    case PurchaseType.NonConsumable:
                    case PurchaseType.Subscription:
                        var item = remapper.getPurchasableItemFromPlatformSpecificId(productId);
                        // We should only provision non consumables if they're not owned.
                        if (0 == tDb.getPurchaseHistory(item)) {
                            callback.onPurchaseSucceeded(productId, receipt);
                        }
                        break;
                }
            }, false);
            #endif
        }

        public void OnPurchaseSucceeded(string productId) {
            this.OnPurchaseSucceeded(productId, string.Empty);
        }

        // When using an incorrect product id:
        // "Exception from HRESULT: 0x805A0194"
        public void OnProductListError(string message) {
            #if UNITY_METRO
            UnityEngine.WSA.Application.InvokeOnAppThread(() => {
                if (message.Contains("801900CC")) {
                    callback.logError(UnibillError.WIN_8_1_APP_NOT_KNOWN);
                    callback.onSetupComplete(false);
                }
                else {
                    logError("Unable to retrieve product listings. Unibill will automatically retry...");
                    logError(message);
                    init(3000);
                }
            }, false);
            #endif
        }
    }
}
