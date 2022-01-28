namespace Services

open Error
open System.Collections.Generic
open Task.Context
open Task.Context.Models
open Repository.EntityFrameworkCore
open Help
open System.Linq

type DivisionService(factory: RepositoryFactory<Context>) =
    member x.Handle (args:string[]) =
        if Array.length args > 0 then
            match args.[0] with
                | "help" -> DivisionService.HelpHandle
                | "add" -> if Array.length args > 1 then x.addHandle args[1..] else notEnoughParams "add" "division"
                | "list" -> if Array.length args > 1 then x.listHandle args[1..] else x.listHandle args[0..]
                | "edit" -> if Array.length args > 1 then x.editHandle args[1..] else notEnoughParams "edit" "division"
                | "delete" -> if Array.length args > 1 then x.deleteHandle args[1..] else notEnoughParams "delete" "division"
                | _ -> commandNotFound args.[0] "division"
        else
            DivisionService.HelpHandle

    static member HelpHandle =
        "Division module commands:\n\
        - help -\n\
        - list -\n\
        - add -\n\
        - edit -\n\
        - delete -" |> printf "%s"

    member private _.addHandle (args: string[]) =
        let addDivision name =
            use rep = factory.GetRepository<Division>()
            let division = new Division()
            division.Name <- name
            rep.AddAsync(division).Wait()
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

    member private _.listHandle (args: string[]) =
        let getDivisions : List<Division> =
            use rep = factory.GetRepository<Division>()
            rep.GetAllAsync().Result.ToList()

        if args.[0] = "help" then
            "Division list command has no parameters" |> printf "%s"
        else
            let divisions = getDivisions
            for division in divisions do printf "%s\n" <| division.ToString()

    member private _.editHandle (args: string[]) =
        let editDivision (id: int, name: string) =
            use rep = factory.GetRepository<Division>()
            let division = rep.GetByIdAsync(id).Result
            division.Name <- name
            rep.SaveAsync(division).Wait()

        if args.[0] = "help" then
            "Division edit command parameters:\n\
            -i : Division ID\n\
            -n : Name" |> printf "%s"
        else
            let id = getValue("-i",  args)
            let name = getValue("-n", args)
            if isNull id then notSetParameter "-i" "edit"
            else if isNull name then notSetParameter "-n" "edit"
            else
                editDivision(id |> int, name)
                printf "Ok"

    member private _.deleteHandle (args: string[]) =
        let getDivisions (id: option<int>) =
            use rep = factory.GetRepository<Division>()
            if id.IsNone then [|rep.GetByIdAsync(id.Value).Result|]
            else rep.GetAllAsync().Result.ToArray()

        let deleteDivisions (divisions: Division[]) =
            use rep = factory.GetRepository<Division>()
            rep.RemoveRangeAsync(divisions).Wait()
            printf "Ok"

        if args.[0] = "help" then
            "Division delete command parameters:\n\
            -i : Division ID\n\
            -a : Delete all divisions" |> printf "%s"
        else
            let id = getValue("-i", args)
            let all = Array.contains "-a" args
            if not <| isNull id then id |> int |> Some |> getDivisions |> deleteDivisions
            else if all then None |> getDivisions |> deleteDivisions
            else notEnoughParams "delete" "division"
