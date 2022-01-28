module Task

open Help
open Error
open Services
open Task.Context

let factory = new SqliteRepositoryFactory("Filename=bd.db")

let division args =
    let service = DivisionService factory
    service.Handle args

[<EntryPoint>]
let main args =
    if Array.length args > 0 then
        match args.[0] with
            | "help" -> writeGlobalHelp
            | "division" -> if Array.length args > 1 then division args[1..] else DivisionService.HelpHandle
            | _ -> commandNotFound args.[0] "main"
    else
        writeGlobalHelp
    0
