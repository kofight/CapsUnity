using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Unibill.Impl {
    class RawBillingPlatformProvider : IRawBillingPlatformProvider {

        private UnibillConfiguration config;

        public RawBillingPlatformProvider(UnibillConfiguration config) {
            this.config = config;
        }

        public IRawGooglePlayInterface getGooglePlay() {
            return new RawGooglePlayInterface();
        }

        public IRawAmazonAppStoreBillingInterface getAmazon() {
            return new RawAmazonAppStoreBillingInterface(config);
        }

        public IStoreKitPlugin getStorekit() {
            if (UnityEngine.Application.platform == UnityEngine.RuntimePlatform.IPhonePlayer) {
                return new StoreKitPluginImpl();
            }

            return new OSXStoreKitPluginImpl();
        }

		public IRawSamsungAppsBillingService getSamsung() {
			return new RawSamsungAppsBillingInterface ();
		}
    }
}
