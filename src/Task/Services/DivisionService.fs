namespace Services

open Error
open Task.Context
open Task.Context.Models
open Repository.EntityFrameworkCore

type DivisionService(factory: RepositoryFactory<Context>) =
    member x.Handle (args:string[]) =
        if Array.length args > 0 then
            match args.[0] with
                | "help" -> DivisionService.HelpHandle
                | "add" -> if Array.length args > 1 then x.addHandle args[1..] else notEnoughParams "add" "division"
                | _ -> commandNotFound args.[0] "division"
        else
            DivisionService.HelpHandle

    static member HelpHandle =
        "Division module commands:\n\
        - help -\n\
        - add -\n\
        - edit -\n\
        - delete -" |> printf "%s"

    member private _.addHandle args =
        let addDivision name =
            use rep = factory.GetRepository<Division>()
            let division = new Division()
            division.Name <- name
            async { rep.AddAsync division |> ignore } |> Async.RunSynchronously
            printf "Ok"

        if args.[0] = "help" then
            "Division add command parameters:\n\
            -n : division name, required" |> printf "%s"
        else
            if not <| Array.contains "-n" args then
                notEnoughParams "add" "division"
            else
                let nameIndex = (Array.findIndex (fun e -> e = "-n") args) + 1
                if Array.length args <= nameIndex then notSetParameter "-n" "add"
                else
                    let name = args.[nameIndex]
                    addDivision name



