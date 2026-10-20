# Banking MVVM App

> **For educational purposes only.** This is a learning sample, not a production banking system. Do not use it to manage real money or store real customer information.

A .NET 10 MAUI application that demonstrates the Model-View-ViewModel (MVVM) pattern using a simple banking workflow. The focus is on separating views, view models, and services while keeping the code approachable for students.

## What the app does

- Lists customers and searches by last name.
- Displays a selected customer's accounts and balances.
- Shows account transaction history, newest first.
- Records deposits and withdrawals after validation and confirmation.

Search runs when the trimmed search text contains at least three characters. Clearing the search shows all customers again. Transaction entry requires a reference and a positive amount; withdrawals cannot exceed the balance shown for the selected account.

The app uses the `en-ZA` culture for formatting, including currency.

## Project structure

| Location | Responsibility |
| --- | --- |
| `Models` | Customer, account, transaction, and lookup data mapped to SQLite tables. |
| `Views` | XAML layouts and minimal code-behind that connects each page to its view model. |
| `Views\BaseView.cs` | Forwards page appearance and disappearance calls to the bound view model. |
| `Views\Behaviors` | UI-specific amount-entry filtering. |
| `ViewModels` | Bindable state, commands, loading, and transaction validation. |
| `ViewModels\BaseViewModel.cs` | Shared `INotifyPropertyChanged` implementation and virtual lifecycle methods. |
| `Commands\AsyncCommand.cs` | A small `ICommand` implementation for asynchronous operations. |
| `Services` | Database access, Shell navigation, and dialogs, exposed through interfaces. |
| `Enums` | Transaction type values. |
| `Resources\Raw\banking.db` | Bundled sample SQLite database. |
| `MauiProgram.cs` | Application setup, culture, fonts, and dependency injection registrations. |
| `AppShell.xaml` and `AppShell.xaml.cs` | Starting page and navigation routes. |

## How MVVM is used

### Views and binding

Each banking page receives its view model through constructor injection and assigns it to `BindingContext`. XAML binds to the view model's properties and commands rather than implementing application logic in event handlers.

The navigation flow is:

```text
CustomerListView
    -> CustomerAccountsView
        -> BankAccountTransactionView
            -> TransactionRecordView
```

Shell passes the selected customer, account, and transaction type as navigation parameters. The destination view models receive these through `IQueryAttributable`.

### Property notifications

`BaseViewModel` implements `INotifyPropertyChanged`. View models use ordinary backing fields and explicit property setters:

```csharp
public string? Reference
{
    get { return reference; }
    set
    {
        if (reference == value) return;
        reference = value;
        OnPropertyChanged();
    }
}
```

`OnPropertyChanged()` uses `CallerMemberName` so the setter does not need to repeat its property name. Properties that affect other displayed values also notify those dependent properties explicitly.

There is no CommunityToolkit.Mvvm dependency or generated MVVM code.

### Commands

`AsyncCommand` wraps a method returning `Task` so it can be bound to a UI command. It supports a condition controlling whether the command can execute and prevents overlapping execution by default. Customer searches allow overlapping execution; a version check prevents an older result from replacing a newer one.

### Page lifecycle

`BaseView` calls `OnAppearingAsync()` and `OnDisappearingAsync()` on its bound `BaseViewModel`. The default implementations return `Task.CompletedTask`.

The customer list, account list, and transaction history view models override the appearance method to refresh their data. Returning from a transaction therefore refreshes the account balance and history.

### Services and dependency injection

| Interface | Implementation | Purpose |
| --- | --- | --- |
| `IBankingDatabaseService` | `BankingDatabaseService` | Reads and updates the local SQLite database. |
| `INavigationService` | `ShellNavigationService` | Navigates between pages and back. |
| `IDialogService` | `DialogService` | Displays alerts and transaction confirmations. |

`MauiProgram` registers services as singletons and pages and view models as transients. Constructor injection makes dependencies visible and allows service implementations to be replaced with fakes when testing view model logic.

### Amount input

`AmountInputBehavior` belongs to the view layer. It filters typed input to digits, one decimal separator (`.` or `,`), and at most two decimal places. The transaction view model separately validates the amount and reference before submitting a transaction.

## Local data

On the first database access, the app copies the bundled `banking.db` into `FileSystem.Current.AppDataDirectory` if no local copy exists. Subsequent reads and writes use that local copy, so recorded transactions persist between app launches.

Deposits are stored as positive transaction amounts and withdrawals as negative amounts. Recording a transaction updates the account balance and inserts a transaction record.

Changing the bundled database does not overwrite an existing local copy. To restore the sample data, close the app and remove only its local `banking.db` from the app data directory, or clear the app's data using the platform's tools. **This discards locally recorded transactions.**

## Getting started

### Prerequisites

- A .NET 10 SDK and compatible .NET MAUI workloads.
- An IDE with .NET MAUI support, or the .NET CLI with the appropriate platform SDKs.
- Windows for the Windows target.
- An Android SDK and emulator or connected device for Android.
- A Mac with a compatible Xcode installation for iOS and Mac Catalyst builds; Windows iOS development requires a paired Mac.

The project targets Android, iOS, Mac Catalyst, and Windows, subject to the host conditions in `BankingMvvmApp.csproj`. Target declarations do not mean every platform can be built on every host.

### Build and run

Open `BankingMvvmApp.csproj` in your IDE, select a supported platform and device, and run the app. No database server or external banking API is required.

From PowerShell in the project directory:

```powershell
dotnet restore .\BankingMvvmApp.csproj
dotnet build .\BankingMvvmApp.csproj -f net10.0-windows10.0.19041.0
```

To build and run the Windows application:

```powershell
dotnet build .\BankingMvvmApp.csproj -t:Run -f net10.0-windows10.0.19041.0
```

To build for Android:

```powershell
dotnet build .\BankingMvvmApp.csproj -f net10.0-android
```

## Unit tests

The solution includes a separate `BankingMvvmApp.Tests` xUnit project with simple
Arrange-Act-Assert tests and hand-written fake database, navigation, and dialog
services. No mocking framework, device, running app, or real database is needed.

The tests cover required fields, invalid amounts, insufficient funds, confirmed
and cancelled transactions, database failures, property notifications, customer
and account loading, navigation, and asynchronous command execution.

Run from the solution directory:

```powershell
dotnet test .\BankingMvvmApp.Tests\BankingMvvmApp.Tests.csproj
```

The test project targets plain `net10.0` and links the application's actual
commands, view models, models, and service interfaces as source files. This avoids
changing the application's platform targets or introducing another production
project just for this sample. It references MAUI Controls Core for the real
`IQueryAttributable` interface and sqlite-net-pcl for the models' mapping attributes;
it does not substitute framework types or open SQLite connections.

These are unit tests of application logic, not tests of XAML bindings, native
controls, Shell navigation itself, or database persistence.

## Educational limitations

This sample is intended to teach code organization, data binding, property notifications, commands, dependency injection, and local persistence. It is **not suitable for production banking**.

It does not provide production-grade authentication, authorization, encrypted customer storage, multi-user concurrency controls, or a tamper-resistant audit trail. Transaction validation is performed in the view model, and the balance update and transaction insert are separate database operations rather than one atomic transaction. These simplifications must not be treated as a safe design for real financial operations.

Use sample data only.
