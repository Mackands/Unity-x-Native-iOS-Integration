#import <UnityFramework/UnityFramework.h>

@implementation NativeBridge : NSObject

#pragma mark - Singleton Instance
+ (instancetype)sharedInstance {
    static NativeBridge *sharedInstance = nil;
    static dispatch_once_t onceToken;
    dispatch_once(&onceToken, ^{
        sharedInstance = [[NativeBridge alloc] init];
    });
    return sharedInstance;
}

#pragma mark - Unity Framework Handling
// Get UnityFramework instance safely
static UnityFramework* GetUnityFrameworkInstance() {
    static UnityFramework* ufw = nil;
    if (!ufw) {
        NSBundle* bundle = [NSBundle bundleWithPath:[[NSBundle mainBundle] pathForResource:@"Frameworks/UnityFramework" ofType:@"framework"]];
        if (![bundle isLoaded]) {
            [bundle load];
        }
        Class unityClass = NSClassFromString(@"UnityFramework");
        if (unityClass) {
            ufw = [unityClass performSelector:@selector(getInstance)];
        }
    }
    if (!ufw) {
        NSLog(@"❌ Failed to retrieve UnityFramework instance.");
    }
    return ufw;
}

#pragma mark - Unity Communication
// Send object rotation data from Unity to Native
void SendRotationToNative(float x, float y, float z) {
    dispatch_async(dispatch_get_main_queue(), ^{
        [[NativeBridge sharedInstance] sendRotationWithX:x y:y z:z];
    });
}

- (void)sendRotationWithX:(float)x y:(float)y z:(float)z {
    NSLog(@"🔄 Object rotated: %f, %f, %f", x, y, z);
    NSString *rotationMessage = [NSString stringWithFormat:@"%f,%f,%f", x, y, z];
}

#pragma mark - Particle Effects
// Trigger fire spark effect
void TriggerFireSparkParticle() {
    dispatch_async(dispatch_get_main_queue(), ^{
        [[NativeBridge sharedInstance] triggerFireSparkParticle];
    });
}

- (void)triggerFireSparkParticle {
    NSLog(@"🔥 Unity requested fire spark trigger");
    [self setupEmitter]; // Start Fire Spark Effect
}

// Stop fire spark effect
void StopFireSparkParticle() {
    dispatch_async(dispatch_get_main_queue(), ^{
        [[NativeBridge sharedInstance] stopFireSparkParticle];
    });
}

- (void)stopFireSparkParticle {
    NSLog(@"🛑 Stopping fire spark effect");
    [self stopEmitter];
}

#pragma mark - UI Handling
// Open Native Page from Unity
void OpenNativePage() {
    dispatch_async(dispatch_get_main_queue(), ^{
        [[NativeBridge sharedInstance] openNativePage];
    });
}

- (void)openNativePage {
    UnityFramework* ufw = GetUnityFrameworkInstance();
    if (!ufw) {
        NSLog(@"❌ UnityFramework instance is NULL");
        return;
    }
    
    UIViewController *rootViewController = ufw.appController.rootViewController;
    if (!rootViewController) {
        NSLog(@"❌ Root view controller is NULL.");
        return;
    }

    UIViewController *vc = [[UIViewController alloc] init];
    vc.view.backgroundColor = [UIColor blueColor];
    [rootViewController presentViewController:vc animated:YES completion:nil];
}

#pragma mark - Particle Timer System
NSTimer *emitterTimer;

- (void)setupEmitter {
    [self stopEmitter]; // Ensure any previous timer is stopped
    [self scheduleNextRun];
}

- (void)stopEmitter {
    if (emitterTimer) {
        [emitterTimer invalidate];
        emitterTimer = nil;
    }
}

- (void)scheduleNextRun {
    int randomInterval = arc4random_uniform(5) + 1;
    emitterTimer = [NSTimer scheduledTimerWithTimeInterval:randomInterval target:self selector:@selector(triggerParticleEffect) userInfo:nil repeats:NO];
}

- (void)triggerParticleEffect {
    float r = (float)arc4random() / UINT32_MAX;
    float g = (float)arc4random() / UINT32_MAX;
    float b = (float)arc4random() / UINT32_MAX;
    NSString *colorMessage = [NSString stringWithFormat:@"%f,%f,%f", r, g, b];

    NSLog(@"🔥 Triggering particle effect in Unity with color: %@", colorMessage);
    UnitySendMessage("NativeBridge", "TriggerParticleEffect", [colorMessage UTF8String]);

    [self scheduleNextRun];
}

#pragma mark - Memory Management
- (void)dealloc {
    [self stopEmitter];
}

@end
