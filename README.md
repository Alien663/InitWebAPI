# Initial Web API

一個基於 .NET 8.0 的 Web API 專案，採用分層架構設計，整合 Entity Framework Core、Azure AD 身份驗證、以及 HTTP 客戶端服務。

## 專案資訊

- **框架版本**: .NET 8.0
- **資料庫**: SQL Server 2019+
- **架構模式**: 分層架構 (Controller → Service → Repository)

## 專案結構

```
InitWebAPI/
├── WebAPI/                    # 主要 Web API 專案
│   ├── Controllers/           # API 控制器
│   ├── Filter/               # 自訂過濾器
│   ├── Options/              # 配置選項類別
│   └── Program.cs            # 應用程式進入點
├── Sample.Service1/          # 服務層 1 - 資料庫操作
│   ├── Entities/             # EF Core DbContext
│   ├── Interfaces/           # 服務介面
│   └── Services/             # 服務實作
└── Sample.Service2/          # 服務層 2 - HTTP 客戶端
    ├── Interfaces/           # HTTP 客戶端介面
    ├── Options/              # Azure AD 配置
    └── Services/             # HTTP 客戶端實作
```

## 主要功能特性

### 1. 身份驗證與授權
- **Login Filter**: 自訂授權過濾器，可套用於 Controller 或 Action 層級
- 支援 Azure AD 身份驗證 (OAuth 2.0 Client Credentials Flow)
- Bearer Token 自動管理與刷新

### 2. 資料庫存取
- 使用 **Entity Framework Core** 進行 ORM 映射
- 支援 SQL Server 連線
- DbContext 依賴注入與生命週期管理
- 可擴展的 Repository 模式

### 3. HTTP 客戶端服務
- 封裝的 HTTP 客戶端，整合 Azure AD 認證
- 支援 Proxy 配置（含驗證）
- 自動處理 Bearer Token 注入
- 查詢字串自動 URL 編碼

### 4. CORS 支援
- 預設允許 `https://localhost:3000` 跨域請求
- 支援 Credentials、Headers 和所有 HTTP Methods

### 5. SPA 整合
- 支援單頁應用程式靜態檔案服務
- ClientApp 路徑配置

### 6. API 文件
- **Swagger UI** 整合（僅開發環境）
- 自動產生 API 文件

## 安裝與設定

### 1. 環境需求
- .NET 8.0 SDK
- SQL Server 2019 或更新版本
- Visual Studio 2022 或 VS Code

### 2. 安裝相依套件
專案已包含以下 NuGet 套件：

**WebAPI 專案:**
- `Microsoft.EntityFrameworkCore.SqlServer` (9.0.12)
- `Swashbuckle.AspNetCore` (10.1.2)
- `Microsoft.AspNetCore.SpaServices.Extensions` (8.0.23)

**Sample.Service1:**
- `Microsoft.EntityFrameworkCore` (9.0.12)

**Sample.Service2:**
- `Microsoft.Identity.Client` (Azure AD 認證)

### 3. 設定檔配置

編輯 `appsettings.json`：

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=.;User ID=sa;Password=your_password;Initial Catalog=your_database;TrustServerCertificate=True;"
  },
  "Azure": {
    "Authority": "https://login.microsoftonline.com/{tenant-id}",
    "Resource": "https://api.example.com",
    "Scope": "https://api.example.com/.default",
    "ClientId": "your-client-id",
    "ClientSecret": "your-client-secret"
  },
  "Proxy": {
    "IP": "http://proxy-server:port",
    "Account": "proxy-username",
    "Password": "proxy-password"
  }
}
```

### 4. 資料庫設定
1. 確保 SQL Server 已啟動
2. 更新 `appsettings.json` 中的連線字串
3. 執行資料庫遷移（如果有）：
   ```bash
   dotnet ef database update --project WebAPI
   ```

### 5. 執行專案
```bash
cd WebAPI
dotnet run
```

專案預設執行於：
- HTTPS: `https://localhost:5001`
- HTTP: `http://localhost:5000`
- Swagger UI: `https://localhost:5001/swagger`

## API 端點範例

### ExampleController
基礎路由: `/api/Example`

需要通過 Login Filter 驗證。

| 方法 | 端點 | 說明 |
|------|------|------|
| GET | `/api/Example` | 呼叫 Service1 更新資料 |
| POST | `/api/Example` | 呼叫 Service2 發送 HTTP POST 請求 |
| PUT | `/api/Example` | 同時呼叫 Service1 更新與 Service2 發送 GET 請求 |

## 架構說明

### 依賴注入生命週期
- **Scoped**: `ISampleServices1` (資料庫服務)
- **Singleton**: `IActionContextAccessor`
- **HttpClient**: `IMyHttpClient` (含 HttpClientFactory)

### Options 模式
使用強型別配置類別：
- `AzureOption`: Azure AD 相關設定
- `ProxyOption`: Proxy 伺服器設定

### 過濾器
- **Login Filter**: 實作 `IAuthorizationFilter`，可自訂授權邏輯

## 開發指南

### 新增服務
1. 在對應的 Service 專案中建立介面與實作
2. 在 [Program.cs](WebAPI/Program.cs) 中註冊服務：
   ```csharp
   builder.Services.AddScoped<IYourService, YourService>();
   ```

### 新增 Controller
1. 在 `WebAPI/Controllers` 建立新的 Controller
2. 套用 `[Route("api/[controller]")]` 屬性
3. 視需要套用 `[Login]` 過濾器

### 擴展 Entity Framework
1. 在 `Sample.Service1/Entities` 中定義實體類別
2. 在 [SampleContext.cs](Sample.Service1/Entities/SampleContext.cs) 的 `OnModelCreatingPartial` 方法中配置關聯

## 注意事項

⚠️ **安全性提醒**:
- 請勿將敏感資訊（密碼、金鑰）提交至版本控制
- 正式環境請使用 Azure Key Vault 或環境變數管理機密
- 預設的 Login Filter 尚未實作驗證邏輯，請依需求完成

⚠️ **CORS 配置**:
- 目前僅允許 `localhost:3000`，正式環境請修改為實際的前端網域

## 待辦事項

- [ ] 實作 Login Filter 的完整授權邏輯
- [ ] 新增 Exception Middleware 進行全域錯誤處理
- [ ] 整合日誌記錄系統（Serilog / NLog）
- [ ] 加入 Email 通知機制
- [ ] 建立單元測試與整合測試
- [ ] 實作 API 版本控制

## 授權

此專案為個人側邊專案，僅供學習與參考使用。
