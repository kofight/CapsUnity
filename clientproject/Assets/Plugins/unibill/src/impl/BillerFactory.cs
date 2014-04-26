using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unibill;
using Unibill.Impl;
using UnityEngine;
using Uniject;
using Uniject.Impl;
using unibill.Dummy;

namespace Unibill.Impl {
    public class BillerFactory {

        private IResourceLoader loader;
        private ILogger logger;
        private IStorage storage;
        private IRawBillingPlatformProvider platformProvider;
        private UnibillConfiguration config;

        public BillerFactory(IResourceLoader resourceLoader, ILogger logger, IStorage storage, IRawBillingPlatformProvider platformProvider, UnibillConfiguration config) {
            this.loader = resourceLoader;
            this.logger = logger;
            this.storage = storage;
            this.platformProvider = platformProvider;
            this.config = config;
        }

        public Biller instantiate() {
            IBillingService svc = instantiateBillingSubsystem();

            var biller = new Biller(config, getTransactionDatabase(), svc, getLogger(), getHelp(), getMapper(), getCurrencyManager());
            return biller;
        }

        private IBillingService instantiateBillingSubsystem() {
            switch (config.CurrentPlatform) {
                case BillingPlatform.AppleAppStore:
                    return new AppleAppStoreBillingService(config, getMapper(), platformProvider.getStorekit());
                case BillingPlatform.AmazonAppstore:
                    return new AmazonAppStoreBillingService(platformProvider.getAmazon(), getMapper(), config, getTransactionDatabase(), getLogger());
                case BillingPlatform.GooglePlay:
                    return new GooglePlayBillingService(platformProvider.getGooglePlay(), config, getMapper(), getLogger());
                case BillingPlatform.MacAppStore:
                    return new AppleAppStoreBillingService(config, getMapper(), platformProvider.getStorekit());
                case BillingPlatform.WindowsPhone8:
                    var result = new WP8BillingService(unibill.Dummy.Factory.Create(config.WP8SandboxEnabled, GetDummyProducts()), config, getMapper(), getTransactionDatabase(), getLogger());
                    new GameObject().AddComponent<WP8Eventhook>().callback = result;
                    return result;
                case BillingPlatform.Windows8_1:
                    var win8 = new Win8_1BillingService(unibill.Dummy.Factory.Create(config.UseWin8_1Sandbox, GetDummyProducts()), config, getMapper(), getTransactionDatabase(), getLogger());
                    new GameObject().AddComponent<Win8Eventhook>().callback = win8;
                    return win8;
				case BillingPlatform.SamsungApps:
					return new SamsungAppsBillingService (config, getMapper (), platformProvider.getSamsung (), getLogger ());
            }
			return new Tests.FakeBillingService(getMapper());
        }

        private CurrencyManager _currencyManager;
        private CurrencyManager getCurrencyManager() {
            if (null == _currencyManager) {
                _currencyManager = new CurrencyManager(config, getStorage());
            }
            return _currencyManager;
        }

        private unibill.Dummy.Product[] GetDummyProducts() {
            var products = config.AllPurchasableItems.Where((x) => x.PurchaseType != PurchaseType.Subscription).Select((x) => {
                return new Product() {
                    Consumable = x.PurchaseType == PurchaseType.Consumable,
                    Description = x.description,
                    Id = x.LocalId,
                    Price = "$123.45",
                    Title = x.name
                };
            });

            return products.ToArray();
        }

        private TransactionDatabase _tDb;
        private TransactionDatabase getTransactionDatabase() {
            if (null == _tDb) {
                _tDb = new TransactionDatabase(getStorage(), getLogger());
            }
            return _tDb;
        }

        private IStorage getStorage() {
            return storage;
        }

        private HelpCentre _helpCentre;
        private HelpCentre getHelp() {
            if (null == _helpCentre) {
                _helpCentre = new HelpCentre(loader.openTextFile("unibillStrings.json").ReadToEnd());
            }

            return _helpCentre;
        }

        private ProductIdRemapper _remapper;
        private ProductIdRemapper getMapper() {
            if (null == _remapper) {
                _remapper = new ProductIdRemapper(config);
            }

            return _remapper;
        }

        private ILogger getLogger() {
            return logger;
        }

        private IResourceLoader getResourceLoader() {
            return loader;
        }
    }
}
