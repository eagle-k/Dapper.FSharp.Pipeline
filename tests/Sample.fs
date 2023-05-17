module Tests

open Microsoft.Data.Sqlite
open Dapper.FSharp.Pipeline

open Expecto

[<CLIMutable>]
type Customer = { Id: int; Name: string }

let connStr =
    SqliteConnectionStringBuilder(DataSource = "customer.db")
    |> string

[<Tests>]
let tests =
    testList
        "samples"
        [ testAsync "querySingleAsync" {
              use connection = new SqliteConnection(connStr)
              do! connection |> DbConnection.openAsync

              let! actual =
                  "select * from Customer where ID = @ID"
                  |> CommandDef.create
                  |> CommandDef.withParameters {| ID = 42 |}
                  |> CommandDef.querySingleAsync<Customer> connection

              "Should successfully query a customer ID = 42"
              |> Expect.equal actual { Id = 42; Name = "John Doe" }

          }
          testAsync "QuerySingleAsync" {
              do!
                  task {
                      use connection = new SqliteConnection(connStr)
                      do! connection.OpenAsync()

                      let! actual =
                          "select * from Customer where ID = @ID"
                          |> CommandDef.create
                          |> CommandDef.withParameters {| ID = 42 |}
                          |> connection.QuerySingleAsync<Customer>

                      "Should successfully query a customer ID = 42"
                      |> Expect.equal actual { Id = 42; Name = "John Doe" }
                  }
                  |> Async.AwaitTask
          } ]