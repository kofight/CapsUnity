using System;
using System.Collections;
using System.Collections.Generic;
using Unibill;
using Uniject;

namespace Unibill.Impl
{
	public class SamsungAppsBillingService : IBillingService
	{
		private IBillingServiceCallback callback;
		private ProductIdRemapper remapper;
		private UnibillConfiguration config;
		private IRawSamsungAppsBillingService rawSamsung;
		private ILogger logger;

		private HashSet<string> unknownSamsungProducts = new HashSet<string>();

		public SamsungAppsBillingService (UnibillConfiguration config, ProductIdRemapper remapper, IRawSamsungAppsBillingService rawSamsung, ILogger logger) {
			this.config = config;
			this.remapper = remapper;
			this.rawSamsung = rawSamsung;
			this.logger = logger;
		}

		public void initialise (IBillingServiceCallback biller)
		{
			this.callback = biller;
			rawSamsung.initialise (this);

			var encoder = new Dictionary<string, object>();
			encoder.Add ("mode", config.SamsungAppsMode.ToString());
			encoder.Add ("itemGroupId", config.SamsungItemGroupId);

			rawSamsung.getProductList (encoder.toJson());
		}

		public void purchase (string item)
		{
			if (unknownSamsungProducts.Contains (item)) {
				callback.logError(UnibillError.SAMSUNG_APPS_ATTEMPTING_TO_PURCHASE_PRODUCT_NOT_RETURNED_BY_SAMSUNG, item);
				callback.onPurchaseFailedEvent(item);
				return;
			}

			rawSamsung.initiatePurchaseRequest (item);
		}

		public void restoreTransactions ()
		{
			rawSamsung.restoreTransactions ();
		}

		public void onProductListReceived(string productListString) {
			Dictionary<string, object> response = (Dictionary<string, object>)Unibill.Impl.MiniJSON.jsonDecode(productListString);

			if (response.Count == 0) {
				callback.logError (UnibillError.SAMSUNG_APPS_NO_PRODUCTS_RETURNED);
				callback.onSetupComplete (false);
				return;
			}

			HashSet<PurchasableItem> productsReceived = new HashSet<PurchasableItem>();
			foreach (var identifier in response.Keys) {
				if (remapper.canMapProductSpecificId(identifier.ToString())) {
					var item = remapper.getPurchasableItemFromPlatformSpecificId(identifier.ToString());
					Dictionary<string, object> details = (Dictionary<string, object>)response[identifier];

					PurchasableItem.Writer.setLocalizedPrice(item,  details["price"].ToString());
					PurchasableItem.Writer.setLocalizedTitle(item, (string) details["localizedTitle"]);
					PurchasableItem.Writer.setLocalizedDescription(item, (string) details["localizedDescription"]);
					productsReceived.Add(item);
				} else {
					logger.LogError("Warning: Unknown product identifier: {0}", identifier.ToString());
				}
			}

			HashSet<PurchasableItem> productsNotReceived = new HashSet<PurchasableItem> (config.AllPurchasableItems);
			productsNotReceived.ExceptWith (productsReceived);
			if (productsNotReceived.Count > 0) {
				foreach (PurchasableItem product in productsNotReceived) {
					this.unknownSamsungProducts.Add(remapper.mapItemIdToPlatformSpecificId(product));
					callback.logError(UnibillError.SAMSUNG_APPS_MISSING_PRODUCT, product.Id, remapper.mapItemIdToPlatformSpecificId(product));
				}
			}

			callback.onSetupComplete (true);
		}

		public void onPurchaseFailed(string item) {
			callback.onPurchaseFailedEvent (item);
		}

		public void onPurchaseSucceeded(string json) {
			Dictionary<string, object> response = (Dictionary<string, object>)Unibill.Impl.MiniJSON.jsonDecode(json);

			callback.onPurchaseSucceeded ((string)response["productId"], (string) response ["signature"]);
		}

		public void onTransactionsRestored (string success) {
			if (bool.Parse (success)) {
				callback.onTransactionsRestoredSuccess ();
			} else {
				callback.onTransactionsRestoredFail("");
			}
		}

		public void onInitFail() {
			callback.onSetupComplete (false);
		}
	}
}
