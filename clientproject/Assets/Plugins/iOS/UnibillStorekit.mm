#import "UnibillStorekit.h"

@implementation EBPurchase

#define UNITY_GAMEOBJECT_NAME "AppleAppStoreCallbackMonoBehaviour"


-(void) pollRequestProductData {
    
    self.retrievedProductData = false;
    [self.requestCondition lock];
    while (!self.retrievedProductData) {
        self.receivedRequestProductsResponse = false;
        
        NSLog(@"Unibill: Attempting to fetch Storekit product data...");
        // Initiate a product request of the Product ID.
        self.request = [[SKProductsRequest alloc] initWithProductIdentifiers:productIds];
        self.request.delegate = self;
        [self.request start];
        
        // Wait for signal.
        while (!self.receivedRequestProductsResponse) {
            [self.requestCondition wait];
        }
        
        [NSThread sleepForTimeInterval:2];
    }
    
    [self.requestCondition unlock];
}


-(bool) requestProducts:(NSSet*)paramIds
{
    productIds = [[NSSet alloc] initWithSet:paramIds];
    if (productIds != nil) {
        
        NSLog(@"Unibill: requestProducts:%@", productIds);
        if ([SKPaymentQueue canMakePayments]) {
            // Yes, In-App Purchase is enabled on this device.
            // Proceed to fetch available In-App Purchase items.
            NSThread *mythread = [[NSThread alloc] initWithTarget:self selector:@selector(pollRequestProductData) object:nil];
            [mythread start];
            
            return YES;
            
        } else {
            return NO;
        }
        
    } else {
        return NO;
    }
}

-(bool) purchaseProduct:(NSString*)requestedProductId
{
    SKProduct* requestedProduct = nil;
    for (SKProduct* product in validProducts) {
        if ([product.productIdentifier isEqualToString:requestedProductId]) {
            requestedProduct = product;
            break;
        }
    }
    
    if (requestedProduct != nil) {
        
        NSLog(@"Unibill purchaseProduct: %@", requestedProduct.productIdentifier);
        
        if ([SKPaymentQueue canMakePayments]) {
            
            // Yes, In-App Purchase is enabled on this device.
            // Proceed to purchase In-App Purchase item.
            
            // Assign a Product ID to a new payment request.
            SKPayment *paymentRequest = [SKPayment paymentWithProduct:requestedProduct];
            
            // Assign an observer to monitor the transaction status.
            [[SKPaymentQueue defaultQueue] addTransactionObserver:self];
            
            // Request a purchase of the product.
            [[SKPaymentQueue defaultQueue] addPayment:paymentRequest];
            
            return YES;
            
        } else {
            NSLog(@"Unibill purchaseProduct: IAP Disabled");
            
            return NO;
        }
        
    } else {
        NSString* message = [NSString stringWithFormat:@"Unknown product identifier:%@", requestedProductId];
        UnitySendMessage(UNITY_GAMEOBJECT_NAME, "onProductPurchaseFailed", message.UTF8String);
        return YES;
    }
}

-(bool) restorePurchase
{
    NSLog(@"Unibill restorePurchase");
    
    if ([SKPaymentQueue canMakePayments]) {
        // Yes, In-App Purchase is enabled on this device.
        // Proceed to restore purchases.
        
        // Assign an observer to monitor the transaction status.
        [[SKPaymentQueue defaultQueue] addTransactionObserver:self];
        
        // Request to restore previous purchases.
        [[SKPaymentQueue defaultQueue] restoreCompletedTransactions];
        
        return YES;
        
    } else {
        // Notify user that In-App Purchase is Disabled.
        return NO;
    }
}

#pragma mark -
#pragma mark SKProductsRequestDelegate Methods

// Store Kit returns a response from an SKProductsRequest.
- (void)productsRequest:(SKProductsRequest *)request didReceiveResponse:(SKProductsResponse *)response {
    
    [self.requestCondition lock];
    
	// Parse the received product info.
	//self.validProduct = nil;
    self.retrievedProductData = true;
	int count = [response.products count];
	if (count>0) {
        NSLog(@"Unibill: productsRequest:didReceiveResponse:%@", response.products);
        // Record our products.
        validProducts = [[NSArray alloc] initWithArray:response.products];
        NSMutableDictionary* dic = [[NSMutableDictionary alloc] init];
        
        for (SKProduct* product in validProducts) {
            NSMutableDictionary* entry = [[NSMutableDictionary alloc] init];
            
            NSNumberFormatter *numberFormatter = [[NSNumberFormatter alloc] init];
            [numberFormatter setFormatterBehavior:NSNumberFormatterBehavior10_4];
            [numberFormatter setNumberStyle:NSNumberFormatterCurrencyStyle];
            [numberFormatter setLocale:product.priceLocale];
            NSString *formattedString = [numberFormatter stringFromNumber:product.price];
            [numberFormatter release];
            
            if (NULL == product.productIdentifier) {
                NSLog(@"Unibill: Product is missing an identifier!");
                continue;
            }
            
            if (NULL == formattedString) {
                NSLog(@"Unibill: Unable to format a localized price");
                [entry setObject:@"" forKey:@"price"];
            } else {
                [entry setObject:formattedString forKey:@"price"];
            }
            if (NULL == product.localizedTitle) {
                NSLog(@"Unibill: no localized title for: %@. Have your products been disapproved in itunes connect?", product.productIdentifier);
                [entry setObject:@"" forKey:@"localizedTitle"];
            } else {
                [entry setObject:product.localizedTitle forKey:@"localizedTitle"];
            }
            
            if (NULL == product.localizedDescription) {
                NSLog(@"Unibill: no localized description for: %@. Have your products been disapproved in itunes connect?", product.productIdentifier);
                [entry setObject:@"" forKey:@"localizedTitle"];
            } else {
                [entry setObject:product.localizedDescription forKey:@"localizedDescription"];
            }
            
            [dic setObject:entry forKey:product.productIdentifier];
        }
        
        NSData *data = [NSJSONSerialization dataWithJSONObject:dic options:0 error:nil];
        NSString* result = [[NSString alloc] initWithData:data encoding:NSUTF8StringEncoding];
        
        UnitySendMessage(UNITY_GAMEOBJECT_NAME, "onProductListReceived", [result UTF8String]);
        [result release];
	} else {
        if (0 == [response.invalidProductIdentifiers count]) {
            // It seems we got no response at all.
            self.retrievedProductData = false;
        } else {
            // Call back to Unity - fail
            UnitySendMessage(UNITY_GAMEOBJECT_NAME, "onProductListReceived", "");
        }
    }
    
    // Send signal.
    self.receivedRequestProductsResponse = true;
    [self.requestCondition signal];
    [self.requestCondition unlock];
}


#pragma mark -
#pragma mark SKPaymentTransactionObserver Methods

- (void)request:(SKRequest *)request didFailWithError:(NSError *)error {
    NSLog(@"Unibill: SKProductRequest::didFailWithError");
    [self.requestCondition lock];
    self.retrievedProductData = false;
    
    // Send signal to our polling loop.
    self.receivedRequestProductsResponse = true;
    [self.requestCondition signal];
    [self.requestCondition unlock];
}

- (void)requestDidFinish:(SKRequest *)request {
    self.request = nil;
}

// The transaction status of the SKPaymentQueue is sent here.
- (void)paymentQueue:(SKPaymentQueue *)queue updatedTransactions:(NSArray *)transactions {
    NSLog(@"Unibill: updatedTransactions");
	for(SKPaymentTransaction *transaction in transactions) {
		switch (transaction.transactionState) {
                
			case SKPaymentTransactionStatePurchasing:
				// Item is still in the process of being purchased
				break;
                
			case SKPaymentTransactionStatePurchased:
            case SKPaymentTransactionStateRestored:
				// Item was successfully purchased or restored.
                NSMutableDictionary* dic;
                dic = [[NSMutableDictionary alloc] init];
                [dic setObject:transaction.payment.productIdentifier forKey:@"productId"];
                
                NSString* receipt;
                receipt = [[NSString alloc] initWithData:transaction.transactionReceipt encoding: NSUTF8StringEncoding];

                [dic setObject:receipt forKey:@"receipt"];
                
                NSData* data;
                data = [NSJSONSerialization dataWithJSONObject:dic options:0 error:nil];
                NSString* result;
                result = [[NSString alloc] initWithData:data encoding:NSUTF8StringEncoding];
                
                UnitySendMessage(UNITY_GAMEOBJECT_NAME, "onProductPurchaseSuccess", result.UTF8String);
                
                [receipt release];
                [dic release];
                
				// After customer has successfully received purchased content,
				// remove the finished transaction from the payment queue.
				[[SKPaymentQueue defaultQueue] finishTransaction: transaction];
                break;
                
			case SKPaymentTransactionStateFailed:
				// Purchase was either cancelled by user or an error occurred.
                
                NSString* errorCode = [NSString stringWithFormat:@"%d",transaction.error.code];
				if (transaction.error.code != SKErrorPaymentCancelled) {
                    NSLog(@"Unibill: purchaseFailed: %@", errorCode);
                    UnitySendMessage(UNITY_GAMEOBJECT_NAME, "onProductPurchaseFailed", transaction.payment.productIdentifier.UTF8String);
				} else {
                    NSLog(@"Unibill: purchaseFailed: %@", errorCode);
                    UnitySendMessage(UNITY_GAMEOBJECT_NAME, "onProductPurchaseCancelled", transaction.payment.productIdentifier.UTF8String);
                }
                
				// Finished transactions should be removed from the payment queue.
				[[SKPaymentQueue defaultQueue] finishTransaction: transaction];
				break;
		}
	}
}

// Called when one or more transactions have been removed from the queue.
- (void)paymentQueue:(SKPaymentQueue *)queue removedTransactions:(NSArray *)transactions
{
    NSLog(@"Unibill removedTransactions");
    
    if (transactions.count == 0) {
        // Release the transaction observer since transaction is finished/removed.
        [[SKPaymentQueue defaultQueue] removeTransactionObserver:self];
    }
}

// Called when SKPaymentQueue has finished sending restored transactions.
- (void)paymentQueueRestoreCompletedTransactionsFinished:(SKPaymentQueue *)queue {
    
    NSLog(@"Unibill paymentQueueRestoreCompletedTransactionsFinished");
    
    if ([queue.transactions count] == 0) {
        // Queue does not include any transactions, so either user has not yet made a purchase
        // or the user's prior purchase is unavailable, so notify app (and user) accordingly.
        
        NSLog(@"Unibill restore queue.transactions count == 0");
        
        // Release the transaction observer since no prior transactions were found.
        [[SKPaymentQueue defaultQueue] removeTransactionObserver:self];
        
    } else {
        // Queue does contain one or more transactions, so return transaction data.
        // App should provide user with purchased product.
        
        for(SKPaymentTransaction *transaction in queue.transactions) {
            
            // Item was successfully purchased or restored.
            NSMutableDictionary* dic;
            dic = [[NSMutableDictionary alloc] init];
            [dic setObject:transaction.payment.productIdentifier forKey:@"productId"];
            
            NSString* receipt;
            receipt = [[NSString alloc] initWithData:transaction.transactionReceipt encoding: NSUTF8StringEncoding];
            [dic setObject:receipt forKey:@"receipt"];
            
            NSData* data;
            data = [NSJSONSerialization dataWithJSONObject:dic options:0 error:nil];
            NSString* result;
            result = [[NSString alloc] initWithData:data encoding:NSUTF8StringEncoding];
            
            UnitySendMessage(UNITY_GAMEOBJECT_NAME, "onProductPurchaseSuccess", result.UTF8String);
            
            [receipt release];
            [dic release];
        }
    }
    
    UnitySendMessage(UNITY_GAMEOBJECT_NAME, "onTransactionsRestoredSuccess", "");
}

// Called if an error occurred while restoring transactions.
- (void)paymentQueue:(SKPaymentQueue *)queue restoreCompletedTransactionsFailedWithError:(NSError *)error
{
    NSLog(@"restoreCompletedTransactionsFailedWithError");
    // Restore was cancelled or an error occurred, so notify user.
    
    UnitySendMessage(UNITY_GAMEOBJECT_NAME, "onTransactionsRestoredFail", error.localizedDescription.UTF8String);
    
}


#pragma mark - Internal Methods & Events

- (id)init {
    if ( self = [super init] ) {
        validProducts = nil;
        self.requestCondition = [[NSCondition alloc] init];
    }
    return self;
}

- (void)dealloc
{
    [super dealloc];
}

@end

// Converts C style string to NSString
NSString* UnibillCreateNSString (const char* string)
{
	if (string)
		return [NSString stringWithUTF8String: string];
	else
		return [NSString stringWithUTF8String: ""];
}

EBPurchase* _instance = NULL;

EBPurchase* _getInstance() {
    if (NULL == _instance) {
        _instance = [[EBPurchase alloc] init];
    }
    return _instance;
}

// When native code plugin is implemented in .mm / .cpp file, then functions
// should be surrounded with extern "C" block to conform C function naming rules
extern "C" {
    
	bool _storeKitPaymentsAvailable () {
        return [SKPaymentQueue canMakePayments];
	}
	
	void _storeKitRequestProductData (const char* productIdentifiers) {
        NSLog(@"Unibill: requestProductData");
        NSString* sIds = [NSString stringWithUTF8String:productIdentifiers];
        NSArray* splits = [[NSArray alloc] init];
        splits = [sIds componentsSeparatedByString:@","];
        NSSet* ids = [NSSet setWithArray:splits];
        [_getInstance() requestProducts:ids];
        
        NSLog(@"Unibill: Traceout: requestProductData");
	}
	
	void _storeKitPurchaseProduct (const char* productId) {
        NSLog(@"Unibill: _storeKitPurchaseProduct");
        NSString* str = UnibillCreateNSString(productId);
        [_getInstance() purchaseProduct:str];
        NSLog(@"Unibill: Traceout: _storeKitPurchaseProduct");
	}
    
    void _storeKitRestoreTransactions() {
        NSLog(@"Unibill: _storeKitRestoreTransactions");
        [_getInstance() restorePurchase];
        NSLog(@"Unibill: Traceout: _storeKitRestoreTransactions");
    }
	
}

