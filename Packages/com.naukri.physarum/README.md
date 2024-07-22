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
        var state = State;
        // 如果 state 是可空類型的話，可以使用 `?? new()` 來設定一個預設值
        // var state = State ?? new();
        
        print(state);

        return state;
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

### 2. 訂閱機制

- 一般情況下，你應該確保所有訂閱相關（`Watch`, `Listen`）的操作都在 `Build` 方法中完成。
- Physarum 在訂閱時會先檢查目標是否已被訂閱，所以你不必擔心重複訂閱的情況發生。
- 所有 `Provider` 會在首次被消費（`Read`, `Watch`, `Listen`）時刷新 (調用 `Build()`) 一次，之後只會在其訂閱的 `Provider` 狀態變化時重建。

### 3. Behaviour 類 (XXX.Behaviour) 的生命週期

- 由於 Unity 的生命週期中相同物件的 Awake 和 Enable 會在同一階段執行，因此如果在這個階段刷新將無法保證其消費的目標已完成初始化。因此，任何操作都需要在 Start 階段（或之後）執行。
- `Provider` 和 `Consumer` 會在 Enable (首次啟用會延後到 Start) 階段執行自行刷新一次。並在 Disable 時調用 `Invalidate()` 取消所有訂閱。

### 4. 匿名 Provider / Consumer 的使用

- Physarum 提供了非繼承自 `MonoBehaviour` 的版本可供匿名使用。這可用於為已有基底類別的 `Component` 添加 Provider / Consumer 特性。實際上，Physarum 提供的 Behaviour 也是通過此方法實現的。

```csharp
// ProviderKeyOf 可以透過隱式轉換 int 和 string 簡化程式碼，
// 如果你想要使用其他型態的 Key，則需要調用 Create 方法生成。
// myProviderKey = ProviderKeyOf<StateProvider<int>>.Create("myKey");
private static readonly ProviderKeyOf<StateProvider<int>> myProviderKey = "myKey";

StateProvider<int> myProvider;

Consumer myConsumer;

public void Start()
{
    myProvider = new(
        ctx =>
        {
            return 100;
        },
        myProviderKey
    );
    myConsumer = new(ctx =>
    {
        var myProvider = ctx.Watch(myProviderKey);
        print(myProvider.State);
    });

    // 簡單派發一個 refresh 事件以初始化 Consumer
    myConsumer.Post(ctx => ctx.Refresh());

    // 在不需要時 Dispose 掉
    // myProvider.Dispose();
    // myConsumer.Dispose();
}
```

### 5. ProviderContainer 與 Resolver 

Physarum 會創建一個 `ProviderContainer` 來快取所有可用的 `Provider`。它的 Key 會在實例化時被主動註冊到 `ProviderContainer` 中並在銷毀時刪除，當通過 `ctx` 查詢時，如果 `Provider` 已存在於快取之中會直接返回快取。若不存在則會調用 `Resolver` 重建快取並再次嘗試查詢。如果仍不存在則會報錯。

一般情況下，Physarum 只會通過 `FindObjectsByType` 找到場景中所有 `Provider.Behaviour` 及其衍生類別進行快取。但你可以通過 `ProviderContainer.Resolver` 添加其他 Resolver。

### 6. Provider 的 Singleton 特性

- Physarum 默認所有 `Provider` 都具有單例（Singleton）特性，這意味著每種類型的 `Provider` 在同一時間內應該只能存在一個。
- 如果需要多個相同類型的 `Provider`想透過 `ctx` 進行查詢，則需要使用 `ProviderKey` 來區分不同的 `Provider`。

### 7. 異步處理

你可以使用 `AsyncValue<T>` 處理異步資料

```csharp
public class CounterProvider : StateProvider<AsyncValue<int>>.Behaviour
{
    protected override AsyncValue<int> Build()
    {
        // 初始狀態
        return AsyncValue<int>.Data(0);
    }

    // 模擬網路請求
    public async void AddOneRequest(bool failed)
    {
        var state = State;
        // 改為 loading 狀態
        // SetAsyncValue 可以使用擴充方法來簡化 e.g. this.SetAsyncValueToLoading();
        SetState(AsyncValue<int>.Loading()); 

        await Task.Delay(1000);

        if (failed) // 模擬失敗
        {
            SetState(AsyncValue<int>.Error(new InvalidOperationException("Add one failed")));
        }
        else // 模擬成功
        {
            SetState(AsyncValue<int>.Data(state.Value + 1));
        }
    }
}

public class CounterConsumer : Consumer.Behaviour
{
    protected override void Build()
    {
        var provider = ctx.Watch<CounterProvider>();
        var providerState = provider.State;

        // 針對個別狀況進行處理
        providerState.When(
            s => print($"New value: {s}"),
            () => print($"Loading..."),
            err => Debug.LogError($"Error: {err}")
        );
    }
}
```

## 文檔

待補

## 貢獻

歡迎任何 PR / Issue