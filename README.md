# Physarum

Physarum 是一個受 Riverpod 啟發，適用於 Unity 的響應式框架。透過 Physarum 可以為你完成狀態管理、依賴注入和服務定位等功能。創建可擴展且易於維護的遊戲架構。

## 特色

- **提供者-消費者模式**: 實現依賴解耦和關注點分離，提供者專注於狀態管理，消費者自動響應狀態變化，降低組件間的耦合度。
- **響應式程式設計**: 自動回應狀態變化，減少手動同步的需求。
- **靈活的狀態管理**: 輕鬆建立和管理複雜狀態，具有型別安全的提供者。
- **事件系統**: 內建事件系統，用於應用程式不同部分之間的通訊。

## 安裝

1. 在您的 Unity 專案中，打開 Package Manager。
2. 點擊 "+" 按鈕，選擇 "Add package from git URL"。
3. 輸入 `https://github.com/naukri7707/Physarum.git`。
4. 點擊 "Add"。

## 快速開始

這裡提供一個計數器的範例。

1. 創建一個提供者：

```csharp
public class CounterProvider : StateProvider<int>.Behaviour
{
    protected override int Build()
    {
        // 初始狀態
        return 0;
    }

    public void AddOne()
    {
        // 更新並回傳一個新的狀態實例
        SetState(s => s + 1);
    }
}
```

2. 創建一個消費者：

```csharp
public class CounterConsumer : Consumer.Behaviour
{
    protected override void Build()
    {
        // 建構時尋找並監聽提供者，框架會排除已經訂閱過的 Provider 所以你不用擔心重複註冊的問題
        var provider = ctx.Watch<CounterProvider>();
        print(provider.State);
    }
}
```

3. 最後將他們掛載到場景就完成了，之後在 `Start` 時期 `Consumer` 會調用一次 `Build` 完成訂閱和首次建構。之後由於訂閱了 `CounterProvider` 的原因每當你調用 `CounterProvider.AddOne()` 時系統便會再次呼叫 `CounterConsumer.Build()` 以更新狀態。
4. 你也可以使用同時具有兩者特性的 `ViewController` 合併邏輯以減少腳本數量，這在 UI 設計時很方便。

```csharp
public class CounterViewController : ViewController<int>.Behaviour
{
    public void AddOne()
    {
        SetState(s => s + 1);
    }

    protected override int Build()
    {
        // ViewController 會監聽自己本身的狀態變化，所以這裡雖然沒有監聽相關的程式碼，但 CounterViewController 仍會在狀態變化時重建
        print(State);
        return State;
    }
}
```

5. 值得注意的是， `Provider` 也具有 `Consumer` 的特性。這意味著 `Provider` 可以監聽其他 `Provider` 的狀態變化，實現更複雜的狀態管理邏輯。例如：

```csharp
public class CompositeProvider : StateProvider<int>.Behaviour
{
    protected override int Build()
    {
        var counterProvider = ctx.Watch<CounterProvider>();
        return counterProvider.State * 2;
    }
}
```

## 注意事項

遵循這些注意事項將幫助您更有效地使用 Physarum 框架，並避免常見的陷阱和錯誤。

### 1. 永遠使用 SetState 更新狀態

- 在更新狀態時，應該始終使用 `SetState` 方法返回一個新的狀態實例，而不是直接修改從 `State` 獲得的當前狀態實例的成員。
- 對於複雜的類型，建議使用 C# 的 `record` 類型和 `with` 關鍵字來輔助完成狀態更新。這樣可以確保狀態的不可變性，並提高代碼的可讀性。

例如：

```csharp
public record MyState(int Count, string Name);

public class MyProvider : StateProvider<MyState>.Behaviour
{
    protected override MyState Build() => new MyState(0, "Initial");

    public void UpdateName(string newName)
    {
        SetState(s => s with { Name = newName });
    }
}
```

### 2. 匿名 Provider / Consumer 的使用

- Physarum 提供了非繼承自 `MonoBehaviour` 的匿名版本。這可用於為已有基底類別的物件添加 Provider / Consumer 特性。實際上，Physarum 提供的 Behaviour 也是通過此方法實現的。
- 使用匿名版本時需注意，其 Key 不會被加入到快取中。因此，你需要直接通過實例來操作，或定義專用的 Resolver 和 `ProviderKey` 來從 `ProviderContainer` 快取中獲取實例。

```csharp
StateProvider<int> myProvider;
Consumer myConsumer;

public void Start()
{
    myProvider = new StateProvider<int>(ctx =>
    {
        return 0;
    });

    myConsumer = new Consumer(ctx =>
    {
        // 直接監聽 Provider 實例
        ctx.Listen(myProvider);
        print(myProvider.State);
    });

    // 在不需要時 Dispose
    // myProvider.Dispose();
    // myConsumer.Dispose();
}
```

### 3. ProviderContainer 與 Resolver 

Physarum 會創建一個 `ProviderContainer` 來快取所有可用的 `Provider`。當通過 `ctx` 查詢時，如果 `Provider` 已存在於快取中，則會直接返回快取。否則會調用 `Resolver` 重建快取並嘗試再次查詢。如果仍不存在則會報錯。

一般情況下，Physarum 只會通過 `FindObjectsByType` 找到場景中所有 `Provider.Behaviour` 及其衍生類別。你可以通過 `ProviderContainer.Resolver` 添加其他 Resolver。

### 4. Provider 的 Singleton 特性

- Physarum 默認所有 `Provider` 都具有單例（Singleton）特性，這意味著每種類型的 `Provider` 在同一時間內應該只能存在一個。
- 如果需要多個相同類型的 `Provider`，則需要使用 `ProviderKey` 來區分不同的 `Provider`。

### 5. 訂閱機制

- 一般情況下，你應該確保所有訂閱相關（`Watch`, `Listen`）的操作都在 `Build` 方法中完成。
- Physarum 在訂閱時會先檢查目標是否已被訂閱，所以你不必擔心重複訂閱的情況發生。
- 如果物件被禁用（Disable），則不會收到通知。

### 6. 生命週期注意事項

由於 Unity 的生命週期中 `Awake` 和 `OnEnable` 會在同一階段執行，因此如果在這個階段建構物件，可能會導致無法保證監聽目標已完成初始化的情況。

因此，任何與 `ctx` 相關的事件都需要在 `Start` 階段（或之後）執行。

## 文檔

待補

## 貢獻

歡迎任何 PR / Issue