# Unity x Native iOS Integration Project

## 📌 Project Overview

This project demonstrates **Unity x Native iOS integration** using **Objective-C** for native communication and **C#** for Unity-side logic. It features **a 3D object with animations, a particle effect system, and a UI system** that interacts with the native iOS side for enhanced functionality.

### ✨ **Key Features**:

- **Unity ↔ Native iOS Communication** using `UnitySendMessage` and `DllImport("__Internal")`.
- **Smooth Object Rotation** with optimized touch handling.
- **Fire Spark Particle Effect** controlled from both Unity and iOS.
- **Marquee Text Animation** for real-time date and time updates.
- **Game Manager** to handle UI transitions and native interactions.
- **Singleton-based Architecture** for better maintainability.

---

## 📂 **Project Structure**

### 1️⃣ **Unity (C#) Scripts**

#### **🔹 Core Scripts**

- `GameManager.cs` → Manages UI transitions, animations, and interactions with the native iOS bridge.
- `Singleton.cs` → A generic singleton class ensuring a single instance of key managers.

#### **🔹 UI & Animation**

- `ObjectController.cs` → Handles **3D object rotation, animation control, and native communication**.
- `MarqueeText.cs` → Displays a **scrolling text** with real-time date and time updates.

#### **🔹 Native Communication**

- `NativeBridge.cs` → Bridges Unity with **Objective-C (iOS)** to send rotation data, trigger particle effects, and open native pages.

---

### 2️⃣ **iOS Native (Objective-C) Files**

#### **🔹 Core iOS Integration**

- `NativeBridge.m` → Handles **native UnityFramework interactions** (e.g., receiving rotation data, triggering particles, and opening native pages).
- `UnityFramework Instance Handling` → Ensures correct initialization and memory management.

---

## 🔗 **Unity ↔ Native iOS Communication**

### 1️⃣ **Sending Data from Unity to Native**

#### **Rotation Data (Object Rotation)**

```csharp
// Unity C#
NativeBridge.Instance.SendRotationData(new Vector3(30f, 45f, 60f));
```

```objective-c
// iOS Objective-C
void SendRotationToNative(float x, float y, float z) {
    NSLog(@"🔄 Received Rotation: %f, %f, %f", x, y, z);
    UnitySendMessage("NativeBridge", "SendRotationData", [[NSString stringWithFormat:@"%f,%f,%f", x, y, z] UTF8String]);
}
```

### 2️⃣ **Triggering Fire Spark Effect**

```csharp
// Unity C#
NativeBridge.Instance.TriggerFireSparkEmitter();
```

```objective-c
// iOS Objective-C
void TriggerFireSparkParticle() {
    NSLog(@"🔥 Fire Spark Triggered");
    dispatch_async(dispatch_get_main_queue(), ^{
        [[NativeBridge sharedInstance] triggerFireSparkParticle];
    });
}
```

---

## 🛠 **Best Practices & Optimizations**

### ✅ **Memory Management & Performance**

- **Prevent Memory Leaks**: Ensured proper deallocation of timers and singletons.
- **Optimized Native Calls**: Used `#if UNITY_IOS && !UNITY_EDITOR` to prevent unnecessary calls in the Unity Editor.
- **Thread Safety**: Used `dispatch_async(dispatch_get_main_queue(), ^{ ... })` to ensure UI updates occur on the main thread.

### ✅ **Modular & Maintainable Code**

- **Singleton Design Pattern**: Used for `GameManager`, `NativeBridge`, and other core systems.
- **Event-Driven Architecture**: Avoids tightly coupled dependencies.
- **UI State Management**: Clean separation of UI handling (`OpenSetting()`, `BackHome()`).

---

## 🚀 **Setup & Installation**

### **1️⃣ Unity Setup**

1. Open the Unity project in **Unity 6 LTS**.
2. Ensure **TextMeshPro** and **URP** are installed.
3. Run the scene and test object rotation & UI interactions.

### **2️⃣ iOS Native Setup**

1. Open **Xcode** and add `NativeBridge.m` and `UnityFramework.framework`.
2. Configure **Objective-C Bridging** for Unity communication.
3. Build and run on an **iOS device (iOS 13+)**.

---

## 🎯 **Future Improvements**

🔹 **Enhance UI Animations** using DOTween for smoother transitions.\
🔹 **Optimize Native Calls** using async communication.\
🔹 **Add More Native Features** (e.g., camera integration, ARKit support).

---

## 📞 **Contact & Support**
Makendsakechix@gmail.com
For any issues or suggestions, feel free to reach out. 🚀

