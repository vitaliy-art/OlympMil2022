namespace Services

open Error
open Help
open Task.Context
open Task.Context.Models
open Repository.EntityFrameworkCore
open System
open System.Linq

type OfficerService(factory: RepositoryFactory<Context>) =
    member this.Handle (args:string[]) =
        if Array.length args > 0 then
            match args.[0] with
            | "help" -> OfficerService.HelpHandle
            | "add" -> if Array.length args > 1 then this.addHandle args[1..] else notEnoughParams "add" "officer"
            | "list" -> if Array.length args > 1 then this.listHandle args[1..] else this.listHandle args[0..]
            | "edit" -> if Array.length args > 1 then this.editHandle args[1..] else notEnoughParams "edit" "officer"
            | "delete" -> if Array.length args > 1 then this.deleteHandle args[1..] else notEnoughParams "delete" "officer"
            | other -> commandNotFound other "officer"
        else OfficerService.HelpHandle

    static member HelpHandle =
        "Officer module commands:\n\
        - help -\n\
        - list -\n\
        - add -\n\
        - edit -\n\
        - delete -"
        |> printf "%s"

    member private _.getNewOfficerDisplayId : int =
        use rep = factory.GetRepository<Officer>()
        let offIds = rep.GetAllAsync().Result.Select(fun o -> o.DisplayId).ToArray()

        let rec fndIdx i =
            if not <| Array.contains i offIds then i
            else fndIdx (i + 1)

        fndIdx 1

    member private _.getDivision (dId:int) =
        use rep = factory.GetRepository<Division>()
        let division = rep.GetAllQueryableAsync().Result.Where(fun d -> d.DisplayId = dId).First()
        division

    member private _.getCadet (dId:int) =
        use rep = factory.GetRepository<Cadet>()
        let cadet = rep.GetAllQueryableAsync().Result.Where(fun c -> c.DisplayId = dId).First()
        cadet

    member private this.getOfficers (dId:option<int>, lName:option<string>, divDId:option<int>, cadDId:option<int>, rank:option<SeniorRanks>) =
        use rep = factory.GetRepository<Officer>()
        let mutable query = rep.GetAllQueryableAsync().Result
        match dId with
        | Some(id) -> query <- query.Where(fun o -> o.DisplayId = id)
        | _ -> ()
        match lName with
        | Some(name) -> query <- query.Where(fun o -> o.Person.LastName = name)
        | _ -> ()
        match divDId with
        | Some(id) -> query <- query.Where(fun o -> o.Division.DisplayId = id)
        | _ -> ()
        match cadDId with
        | Some(id) -> query <- query.Where(fun o -> o.Division.DisplayId = int(this.getCadet(id).DivisionId))
        | _ -> ()
        match rank with
        | Some(r) -> query <- query.Where(fun o -> o.Rank = r)
        | _ -> ()
        query.ToArray()

    member private this.listHandle (args:string[]) =
        if Array.length args > 0 && args.[0] = "help" then
            "Officer list command parameters:\n\
            -i : ID\n\
            -l : last name\n\
            -d : division ID\n\
            -r : rank\n\
            -c : cadet ID\n\
            -s : sorting, possible id, lastName\n\
            -p : properties view, combination of i - id, r - rank, f - firstName, m - middleName, l - lastName, b - birthDate"
            |> printf "%s"
        else
            let dId = match getValue("-i", args) with
                        | null -> None
                        | x -> x |> int |> Some
            let lName = match getValue("-l", args) with
                        | null -> None
                        | x -> x |> Some
            let divDId = match getValue("-d", args) with
                            | null -> None
                            | x -> x |> int |> Some
            let rank = match getValue("-r", args) with
                        | null -> None
                        | x -> Enum.Parse<SeniorRanks>(x) |> Some
            let cadDId = match getValue("-c", args) with
                            | null -> None
                            | x -> x |> int |> Some
            let officers = this.getOfficers(dId, lName, divDId, cadDId, rank)
            let sorted = match getValue("-s", args) with
                            | "id" -> officers.OrderBy(fun o -> o.DisplayId).ToArray()
                            | "lastName" -> officers.OrderBy(fun o -> o.Person.LastName).ToArray()
                            | _ -> officers
            for officer in sorted do
                match getValue("-p", args) with
                | null -> officer.ToString() |> printf "%s\n"
                | value -> officer.ToString(value) |> printf "%s\n"

    member private this.addHandle (args:string[]) =
        let addOfficer (officer:Officer) =
            use rep = factory.GetRepository<Officer>()
            rep.AddAsync(officer).Wait()
            printf "Ok"

        if args.[0] = "help" then
            "Officer add command parameters:\n\
            -f : first name, required\n\
            -m : middle name, required\n\
            -l : last name, required\n\
            -b : birth date, required, format yyyy-MM-dd\n\
            -r : rank, required\n\
            -d : division ID"
            |> printf "%s"
        else
            if paramMissing([|"-f"; "-m"; "-l"; "-b"; "-r"|], args) then notEnoughParams "add" "officer"
            else match paramNotSet([|"-f"; "-m"; "-l"; "-b"; "-r"; "-d"|], args) with
                 | Some(x) -> notSetParameter x "add"
                 | None ->
                    let divId = match getValue("-d", args) with
                                | null -> None
                                | id -> id |> int |> Some
                    let rank = Enum.Parse<SeniorRanks>(getValue("-r", args))
                    let date = DateOnly.Parse <| getValue("-b", args)

                    let person = Person(
                        FirstName=getValue("-f", args),
                        MiddleName=getValue("-m", args),
                        LastName=getValue("-l", args),
                        BirthDate=date
                    )

                    let officer = Officer(
                        Person=person,
                        DisplayId=this.getNewOfficerDisplayId,
                        Rank=rank
                    )

                    if divId.IsSome then
                        officer.DivisionId <- this.getDivision(divId.Value).Id

                    addOfficer officer

    member private this.editHandle (args:string[]) =
        if args.[0] = "help" then
            "Officer edit command parameters:\n\
            -i : ID, required\n\
            -f : first name\n\
            -m : middle name\n\
            -l : last name\n\
            -b : birth date, format yyyy-MM-dd\n\
            -r : rank\n\
            -d : division ID"
            |> printf "%s"
        else
            if paramMissing([|"-i"|], args) then notEnoughParams "edit" "officer"
            else match paramNotSet([|"-f"; "-m"; "-l"; "-b"; "-r"; "-d"|], args) with
                 | Some(x) -> notSetParameter x "edit"
                 | None ->
                    let dId = getValue("-i", args) |> int
                    let fName = getValue("-f", args)
                    let mName = getValue("-m", args)
                    let lName = getValue("-l", args)
                    let bDate = getValue("-b", args)
                    let rank = getValue("-r", args)
                    let divDId = getValue("-d", args)
                    use rep = factory.GetRepository<Officer>()
                    let officer = rep.GetAllQueryableAsync().Result.Where(fun o -> o.DisplayId = dId).First()

                    if [|fName; mName; lName; bDate; rank; divDId|].All(fun x -> x = null) then
                        notEnoughParams "edit" "officer"
                    else
                        match fName with
                        | null -> ()
                        | name -> officer.Person.FirstName <- name
                        match mName with
                        | null -> ()
                        | name -> officer.Person.MiddleName <- name
                        match lName with
                        | null -> ()
                        | name -> officer.Person.LastName <- name
                        match bDate with
                        | null -> ()
                        | date -> officer.Person.BirthDate <- DateOnly.Parse(date)
                        match rank with
                        | null -> ()
                        | rank -> officer.Rank <- Enum.Parse<SeniorRanks>(rank)
                        match divDId with
                        | null -> ()
                        | id -> officer.DivisionId <- this.getDivision(id |> int).Id

                        rep.SaveAsync(officer).Wait()
                        printf "Ok"

    member private _.deleteHandle (args:string[]) =
        if args.[0] = "help" then
            "Officer delete command parameters:\n\
            -i : ID\n\
            -a : delete all officers"
            |> printf "%s"
        else
            use rep = factory.GetRepository<Officer>()

            if Array.contains "-a" args then
                let officers = rep.GetAllAsync().Result
                rep.RemoveRangeAsync(officers).Wait()
            else
                match getValue("-i", args) with
                | null -> ()
                | strId -> do
                    let id = strId |> int
                    let officer = rep.GetAllQueryableAsync().Result.Where(fun o -> o.DisplayId = id).First()
                    rep.RemoveAsync(officer).Wait()

            printf "Ok"
