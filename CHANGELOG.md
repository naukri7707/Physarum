# Changelog

## [~0.3.0]

- 修正 Provider.Behaviour 快取到核心 Provider 錯誤
- 新增 int, string 對 ProviderKeyOf 的隱式轉換

## [0.2.0] - 2024-07-16

- 新增 `ProviderKeyOf<T>` 類型。這允許通過泛型推導自動確定 Provider 型別，減少手動指定的需求，從而降低潛在的型別錯誤風險
- `Element` 類別新增 `Post` 方法，使匿名 `Provider` 和 `Consumer` 能夠執行更複雜的操作，提高泛用性
- 將所有 `Find` 方法重命名為 `Read`，以統一命名邏輯
- 修正 `Behaviour` 類無法使用 `HandleEvent` 的問題

## [0.1.0] - 2024-07-15

首次發布
