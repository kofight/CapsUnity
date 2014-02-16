// UnibillStorekit.h
#import <StoreKit/StoreKit.h>

@interface EBPurchase : NSObject <SKProductsRequestDelegate, SKPaymentTransactionObserver> {
    NSArray* validProducts;
    NSSet* productIds;
}

@property (strong, nonatomic) SKProductsRequest *request;
@property volatile bool retrievedProductData;
@property volatile bool receivedRequestProductsResponse;
@property (strong, atomic) NSCondition* requestCondition;

-(bool) requestProducts:(NSSet*)productId;
-(bool) purchaseProduct:(NSString*)requestedProduct;
-(bool) restorePurchase;

@end
