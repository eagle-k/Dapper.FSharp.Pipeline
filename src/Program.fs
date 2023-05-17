module Dapper.FSharp.Pipeline

open System.Data
open System.Data.Common
open System.Threading
open System.Threading.Tasks
open Dapper

/// F# record for pipeline API to replace `CommandDefinition` in Dapper
type CommandDef =
    { CommandText: string option
      Parameters: obj option
      Transaction: IDbTransaction option
      CommandTimeout: int option
      CommandType: CommandType option
      Flags: CommandFlags option
      CancellationToken: CancellationToken option }

module CommandDef =
    let create commandText =
        { CommandText = Some commandText
          Parameters = None
          Transaction = None
          CommandTimeout = None
          CommandType = None
          Flags = None
          CancellationToken = None }

    let withParameters parameters commandDef =
        { commandDef with Parameters = Some parameters }

    let withTransaction transaction commandDef =
        { commandDef with Transaction = Some transaction }

    let withCommandTimeout commandTimeout commandDef =
        { commandDef with CommandTimeout = Some commandTimeout }

    let withCommandType commandType commandDef =
        { commandDef with CommandType = Some commandType }

    let withFlags flags commandDef = { commandDef with Flags = Some flags }

    let withCancellationToken cancellationToken commandDef =
        { commandDef with CancellationToken = Some cancellationToken }

    let internal toCommandDefinition commandDef =
        let toDefault opt =
            opt |> Option.defaultValue Unchecked.defaultof<_>

        let commandText = toDefault commandDef.CommandText
        let parameters = toDefault commandDef.Parameters
        let transaction = toDefault commandDef.Transaction
        let commandTimeout = Option.toNullable commandDef.CommandTimeout
        let commandType = Option.toNullable commandDef.CommandType
        let flags = toDefault commandDef.Flags
        let cancellationToken = toDefault commandDef.CancellationToken

        CommandDefinition(commandText, parameters, transaction, commandTimeout, commandType, flags, cancellationToken)

    let internal toCommandDefinitionAsync commandDef =
        async {
            let! cancellationTokenFromAsync = Async.CancellationToken

            let cancellationToken =
                match commandDef.CancellationToken with
                | Some cancellationToken -> cancellationToken
                | None -> cancellationTokenFromAsync

            return
                commandDef
                |> withCancellationToken cancellationToken
                |> toCommandDefinition
        }

    let execute (connection: IDbConnection) commandDef =
        commandDef
        |> toCommandDefinition
        |> connection.Execute

    let executeAsync (connection: IDbConnection) commandDef =
        async {
            let! command = toCommandDefinitionAsync commandDef

            return!
                connection.ExecuteAsync(command)
                |> Async.AwaitTask
        }

    let executeReader (connection: IDbConnection) commandDef =
        commandDef
        |> toCommandDefinition
        |> connection.ExecuteReader

    let executeReaderAsync (connection: IDbConnection) commandDef =
        async {
            let! command = toCommandDefinitionAsync commandDef

            return!
                connection.ExecuteReaderAsync(command)
                |> Async.AwaitTask
        }

    let executeScalar<'T> (connection: IDbConnection) commandDef =
        commandDef
        |> toCommandDefinition
        |> connection.ExecuteScalar<'T>

    let executeScalarAsync<'T> (connection: IDbConnection) commandDef =
        async {
            let! command = toCommandDefinitionAsync commandDef

            return!
                connection.ExecuteScalarAsync<'T>(command)
                |> Async.AwaitTask
        }

    let query<'T> (connection: IDbConnection) commandDef : seq<'T> =
        commandDef
        |> toCommandDefinition
        |> connection.Query<'T>

    let queryAsync<'T> (connection: IDbConnection) commandDef : Async<seq<'T>> =
        async {
            let! command = toCommandDefinitionAsync commandDef

            return!
                connection.QueryAsync<'T>(command)
                |> Async.AwaitTask
        }

    let queryFirst<'T> (connection: IDbConnection) commandDef =
        commandDef
        |> toCommandDefinition
        |> connection.QueryFirst<'T>

    let queryFirstAsync<'T> (connection: IDbConnection) commandDef =
        async {
            let! command = toCommandDefinitionAsync commandDef

            return!
                connection.QueryFirstAsync<'T>(command)
                |> Async.AwaitTask
        }

    let queryFirstOrDefault<'T> (connection: IDbConnection) commandDef =
        commandDef
        |> toCommandDefinition
        |> connection.QueryFirstOrDefault<'T>

    let queryFirstOrDefaultAsync<'T> (connection: IDbConnection) commandDef =
        async {
            let! command = toCommandDefinitionAsync commandDef

            return!
                connection.QueryFirstOrDefaultAsync<'T>(command)
                |> Async.AwaitTask
        }

    let queryMultiple (connection: IDbConnection) commandDef =
        commandDef
        |> toCommandDefinition
        |> connection.QueryMultiple

    let queryMultipleAsync (connection: IDbConnection) commandDef =
        async {
            let! command = toCommandDefinitionAsync commandDef

            return!
                connection.QueryMultipleAsync(command)
                |> Async.AwaitTask
        }

    let querySingle<'T> (connection: IDbConnection) commandDef =
        commandDef
        |> toCommandDefinition
        |> connection.QuerySingle<'T>

    let querySingleAsync<'T> (connection: IDbConnection) commandDef =
        async {
            let! command = toCommandDefinitionAsync commandDef

            return!
                connection.QuerySingleAsync<'T>(command)
                |> Async.AwaitTask
        }

    let querySingleOrDefault<'T> (connection: IDbConnection) commandDef =
        commandDef
        |> toCommandDefinition
        |> connection.QuerySingleOrDefault<'T>

    let querySingleOrDefaultAsync<'T> (connection: IDbConnection) commandDef =
        async {
            let! command = toCommandDefinitionAsync commandDef

            return!
                connection.QuerySingleOrDefaultAsync<'T>(command)
                |> Async.AwaitTask
        }

type IDbConnection with
    member this.Execute(commandDef: CommandDef) =
        commandDef
        |> CommandDef.toCommandDefinition
        |> this.Execute

    member this.ExecuteAsync(commandDef: CommandDef) =
        commandDef
        |> CommandDef.toCommandDefinition
        |> this.ExecuteAsync

    member this.ExecuteReader(commandDef: CommandDef) =
        commandDef
        |> CommandDef.toCommandDefinition
        |> this.ExecuteReader

    member this.ExecuteReaderAsync(commandDef: CommandDef) =
        commandDef
        |> CommandDef.toCommandDefinition
        |> this.ExecuteReaderAsync

    member this.ExecuteScalar<'T>(commandDef: CommandDef) =
        commandDef
        |> CommandDef.toCommandDefinition
        |> this.ExecuteScalar<'T>

    member this.ExecuteScalarAsync<'T>(commandDef: CommandDef) =
        commandDef
        |> CommandDef.toCommandDefinition
        |> this.ExecuteScalarAsync<'T>

    member this.Query<'T>(commandDef: CommandDef) : seq<'T> =
        commandDef
        |> CommandDef.toCommandDefinition
        |> this.Query<'T>

    member this.QueryAsync<'T>(commandDef: CommandDef) : Task<seq<'T>> =
        commandDef
        |> CommandDef.toCommandDefinition
        |> this.QueryAsync<'T>

    member this.QueryFirst<'T>(commandDef: CommandDef) =
        commandDef
        |> CommandDef.toCommandDefinition
        |> this.QueryFirst<'T>

    member this.QueryFirstAsync<'T>(commandDef: CommandDef) =
        commandDef
        |> CommandDef.toCommandDefinition
        |> this.QueryFirstAsync<'T>

    member this.QueryFirstOrDefault<'T>(commandDef: CommandDef) =
        commandDef
        |> CommandDef.toCommandDefinition
        |> this.QueryFirstOrDefault<'T>

    member this.QueryFirstOrDefaultAsync<'T>(commandDef: CommandDef) =
        commandDef
        |> CommandDef.toCommandDefinition
        |> this.QueryFirstOrDefaultAsync<'T>

    member this.QueryMultiple(commandDef: CommandDef) =
        commandDef
        |> CommandDef.toCommandDefinition
        |> this.QueryMultiple

    member this.QueryMultipleAsync(commandDef: CommandDef) =
        commandDef
        |> CommandDef.toCommandDefinition
        |> this.QueryMultipleAsync

    member this.QuerySingle<'T>(commandDef: CommandDef) =
        commandDef
        |> CommandDef.toCommandDefinition
        |> this.QuerySingle<'T>

    member this.QuerySingleAsync<'T>(commandDef: CommandDef) =
        commandDef
        |> CommandDef.toCommandDefinition
        |> this.QuerySingleAsync<'T>

    member this.QuerySingleOrDefault<'T>(commandDef: CommandDef) =
        commandDef
        |> CommandDef.toCommandDefinition
        |> this.QuerySingleOrDefault<'T>

    member this.QuerySingleOrDefaultAsync<'T>(commandDef: CommandDef) =
        commandDef
        |> CommandDef.toCommandDefinition
        |> this.QuerySingleOrDefaultAsync<'T>


module DbConnection =
    let openAsync (connection: DbConnection) =
        async {
            let! cancellationToken = Async.CancellationToken

            do!
                connection.OpenAsync(cancellationToken)
                |> Async.AwaitTask
        }

    let closeAsync (connection: DbConnection) =
        async {
            do!
                connection.CloseAsync() // CancellationToken is not available
                |> Async.AwaitTask
        }

    let disposeAsync (connection: DbConnection) =
        async {
            do!
                connection
                    .DisposeAsync() // CancellationToken is not available
                    .AsTask()
                |> Async.AwaitTask
        }

    let getSchemaAsync (connection: DbConnection) =
        async {
            let! cancellationToken = Async.CancellationToken

            return!
                connection.GetSchemaAsync(cancellationToken)
                |> Async.AwaitTask
        }

    let getSchemaAsyncWithName collectionName (connection: DbConnection) =
        async {
            let! cancellationToken = Async.CancellationToken

            return!
                connection.GetSchemaAsync(collectionName, cancellationToken)
                |> Async.AwaitTask
        }

    let getSchemaAsyncWithRestriction collectionName restrictionValues (connection: DbConnection) =
        async {
            let! cancellationToken = Async.CancellationToken

            return!
                connection.GetSchemaAsync(collectionName, restrictionValues, cancellationToken)
                |> Async.AwaitTask
        }

    let changeDatabaseAsync databaseName (connection: DbConnection) =
        async {
            let! cancellationToken = Async.CancellationToken

            do!
                connection.ChangeDatabaseAsync(databaseName, cancellationToken)
                |> Async.AwaitTask
        }

    let beginTransactionAsync (connection: DbConnection) =
        async {
            let! cancellationToken = Async.CancellationToken

            return!
                connection
                    .BeginTransactionAsync(cancellationToken)
                    .AsTask()
                |> Async.AwaitTask
        }

    let beginTransactionAsyncWith isolationLevel (connection: DbConnection) =
        async {
            let! cancellationToken = Async.CancellationToken

            return!
                connection
                    .BeginTransactionAsync(isolationLevel, cancellationToken)
                    .AsTask()
                |> Async.AwaitTask
        }