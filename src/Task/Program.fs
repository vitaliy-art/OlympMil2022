module Task

open Help
open Error
open Services
open Task.Context

let factory = new SqliteRepositoryFactory("Filename=bd.db")

let division args =
    let service = DivisionService factory
    service.Handle args

let cadet args =
    let service = CadetService factory
    service.Handle args

let officer args =
    let service = OfficerService factory
    service.Handle args

[<EntryPoint>]
let main args =
    if Array.length args > 0 then
        match args.[0] with
        | "help" -> writeGlobalHelp
        | "division" -> if Array.length args > 1 then division args[1..] else DivisionService.HelpHandle
        | "cadet" -> if Array.length args > 1 then cadet args[1..] else CadetService.HelpHandle
        | "officer" -> if Array.length args > 1 then officer args[1..] else OfficerService.HelpHandle
        | other -> commandNotFound other "main"
    else
        writeGlobalHelp
    0
