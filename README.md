# Dapper.FSharp.Pipeline

Experiments with Dapper's F# idiomatic API


## What does the code look like?

### F# Async style

Use `CommandDef` pipeline API

```fsharp
[<CLIMutable>]
type Customer = { Id: int; Name: string }

use cts = new CancellationTokenSource()

let coldTask =
    async {
        use connection = // ...
        do! connection |> DbConnection.openAsync

        return!
            "select Id, Name from Customer where ID = @ID"
            |> CommandDef.create
            |> CommandDef.withParameters {| ID = 42 |}
            |> CommandDef.querySingleAsync<Customer> connection
    }

Async.RunSynchronously(coldTask, cancellationToken = cts.Token)
```

### .NET Task style

Use ordinary (but overloaded) `IDbConnection` extension methods

```fsharp
[<CLIMutable>]
type Customer = { Id: int; Name: string }

use cts = new CancellationTokenSource()

let hotTask =
    task {
        use connection = // ...
        do! connection.OpenAsync(cts.Token)

        return!
            "select Id, Name from Customer where ID = @ID"
            |> CommandDef.create
            |> CommandDef.withParameters {| ID = 42 |}
            |> CommandDef.withCancellationToken cts.Token
            |> connection.QuerySingleAsync<Customer>
    }

hotTask.Result
```

## Naming convention

- Extension methods of `IDbConnection` return `Task<'T>`
    - Overload with the same name as the extension method of Dapper
- Pipeline API functions return `Async<'T>`
    - Add `Async` to the function name
    - Naming in camelCase to avoid confusion with APIs that return `Task<'T>`
