namespace Services

open Error
open Help
open Task.Context
open Task.Context.Models
open Repository.EntityFrameworkCore
open System
open System.Linq

type CadetService(factory: RepositoryFactory<Context>) =
    member this.Handle (args:string[]) =
        if Array.length args > 0 then
            match args.[0] with
                | "help" -> CadetService.HelpHandle
                | "add" -> if Array.length args > 1 then this.addHandle args[1..] else notEnoughParams "add" "cadet"
                | "list" -> if Array.length args > 1 then this.listHanlde args[1..] else this.listHanlde args[0..]
                | "edit" -> if Array.length args > 1 then this.editHandle args[1..] else notEnoughParams "edit" "cadet"
                | "delete" -> if Array.length args > 1 then this.deleteHandle args[1..] else notEnoughParams "delete" "cadet"
                | _ -> commandNotFound args.[0] "cadet"
        else
            DivisionService.HelpHandle

    static member HelpHandle =
        "Cadet module commands:\n\
        - help -\n\
        - list -\n\
        - add -\n\
        - edit -\n\
        - delete -\n" |> printf "%s"

    member private _.getNewCadetDisplayId: int =
        use rep = factory.GetRepository<Cadet>()
        let cadIds = rep.GetAllAsync().Result.Select(fun d -> d.DisplayId).ToArray()

        let rec fndIdx i =
            if not <| Array.contains i cadIds then i
            else fndIdx (i + 1)

        fndIdx 1

    member private _.getDivision (dId:int) =
        use rep = factory.GetRepository<Division>()
        let division = rep.GetAllQueryableAsync().Result.Where(fun d -> d.DisplayId = dId).FirstOrDefault()
        division

    member private _.getOfficer (offDId:int) =
        use rep = factory.GetRepository<Officer>()
        let officer = rep.GetAllQueryableAsync().Result.Where(fun o -> o.DisplayId = offDId).FirstOrDefault()
        officer

    member private this.getCadets (dId:option<int>, lName:option<string>, divDId:option<int>, offDId:option<int>, rank:option<JuniorRanks>) =
        use rep = factory.GetRepository<Cadet>()
        let mutable query = rep.GetAllQueryableAsync().Result
        match dId with
        | Some(x) -> query <- query.Where(fun c -> c.DisplayId = x)
        | _ -> ()
        match lName with
        | Some(x) -> query <- query.Where(fun c -> c.Person.LastName = x)
        | _ -> ()
        match divDId with
        | Some(x) -> query <- query.Where(fun c -> c.Division.DisplayId = x)
        | _ -> ()
        match offDId with
        | Some(x) -> query <- query.Where(fun c -> c.Division.DisplayId = this.getOfficer(x).Division.DisplayId)
        | _ -> ()
        match rank with
        | Some(x) -> query <- query.Where(fun c -> c.Rank = x)
        | _ -> ()
        query.ToArray()

    member private this.listHanlde (args: string[]) =
        if Array.length args > 0 && args.[0] = "help" then
            "Cadet list command parameters:\n\
            -i : ID\n\
            -l : last name\n\
            -d : division ID\n\
            -r : rank\n\
            -o : division officer ID\n\
            -f : filter, possible id, lastName\n\
            -p : properties, combination of i - id, r - rank, f - firstName, m - middleName, l - lastName, b - birthDate"
            |> printf "%s"
        else
            let dId: option<int> = match getValue("-i", args) with
                                    | null -> None
                                    | x -> x |> int |> Some
            let lName: option<string> = match getValue("-l", args) with
                                        | null -> None
                                        | x -> Some(x)
            let divDId: option<int> = match getValue("-d", args) with
                                        | null -> None
                                        | x -> x |> int |> Some
            let offDId: option<int> = match getValue("-o", args) with
                                        | null -> None
                                        | x -> x |> int |> Some
            let rank: option<JuniorRanks> = match getValue("-r", args) with
                                            | null -> None
                                            | x -> Enum.Parse<JuniorRanks>(x) |> Some
            let cadets = this.getCadets(dId, lName, divDId, offDId, rank)
            let filtered = match getValue("-f", args) with
                            | "id" -> cadets.OrderBy(fun c -> c.DisplayId).ToArray()
                            | "lastName" -> cadets.OrderBy(fun c -> c.Person.LastName).ToArray()
                            | _ -> cadets

            for cadet in filtered do
                match getValue("-p", args) with
                | null -> cadet.ToString() |> printf "%s\n"
                | value -> cadet.ToString(value) |> printf "%s\n"

    member private _.paramMissing (prms:string[], args:string[]) : bool =
        let rec isMissing (i:int) : bool =
            if i = Array.length prms then false
            else if not <| Array.contains prms.[i] args then true
            else isMissing (i + 1)

        isMissing 0


    member private this.addHandle (args:string[]) =
        let addCadet (cadet:Cadet) =
            use rep = factory.GetRepository<Cadet>()
            rep.AddAsync(cadet).Wait()
            printf "Ok"

        if args.[0] = "help" then
            "Cadet add command parameters:\n\
            -f : first name, required\n\
            -m : middle name, required\n\
            -l : last name, required\n\
            -b : birth date, required, format yyyy-MM-dd\n\
            -r : rank, required\n\
            -d : division ID,required\n"
            |> printf "%s"
        else
            if this.paramMissing([|"-f"; "-m"; "-l"; "-b"; "-r"; "-d"|], args) then notEnoughParams "add" "cadet"
            else
                let divId = getValue("-d", args) |> int
                let rank = Enum.Parse<JuniorRanks>(getValue("-r", args))
                let date = DateOnly.Parse <| getValue("-b", args)

                let person = Person(
                    FirstName=getValue("-f", args),
                    MiddleName=getValue("-m", args),
                    LastName=getValue("-l", args),
                    BirthDate=date
                )

                let cadet = Cadet(
                    Person=person,
                    DisplayId=this.getNewCadetDisplayId,
                    DivisionId=this.getDivision(divId).Id,
                    Rank=rank
                )

                addCadet cadet

    member private this.editHandle (args:string[]) =
        if args.[0] = "help" then
            "Cadet edit command parameters:\n\
            -i : ID, required\n\
            -f : first name\n\
            -m : middle name\n\
            -l : last name\n\
            -b : birth date, format yyyy-MM-dd\n\
            -r : rank\n\
            -d : division ID\n"
            |> printf "%s"
        else
            if this.paramMissing([|"-i"|], args) then notEnoughParams "edit" "cadet"
            else
                let dId = getValue("-i", args) |> int
                let fName = getValue("-f", args)
                let mName = getValue("-m", args)
                let lName = getValue("-l", args)
                let bDate = getValue("-b", args)
                let rank = getValue("-r", args)
                let divDId = getValue("-d", args)
                use rep = factory.GetRepository<Cadet>()
                let cadet = rep.GetAllQueryableAsync().Result.Where(fun c -> c.DisplayId = dId).First()

                match fName with
                    | null -> ()
                    | name -> cadet.Person.FirstName <- name
                match mName with
                    | null -> ()
                    | name -> cadet.Person.MiddleName <- name
                match lName with
                    | null -> ()
                    | name -> cadet.Person.LastName <- name
                match bDate with
                    | null -> ()
                    | date -> cadet.Person.BirthDate <- DateOnly.Parse(date)
                match rank with
                    | null -> ()
                    | rank -> cadet.Rank <- Enum.Parse<JuniorRanks>(rank)
                match divDId with
                    | null -> ()
                    | id -> cadet.DivisionId <- this.getDivision(id |> int).Id

                rep.SaveAsync(cadet).Wait()
                printf "Ok"

    member private this.deleteHandle (args:string[]) =
        if args.[0] = "help" then
            "Cadet delete command parameters:\n\
            -i : ID\n\
            -d : division ID\n\
            -o : division officer ID\n\
            -a : delete all cadets"
            |> printf "%s"
        else
            use rep = factory.GetRepository<Cadet>()

            if Array.contains "-a" args then
                let cadets = rep.GetAllAsync().Result
                rep.RemoveRangeAsync(cadets).Wait()
            else
                match getValue("-i", args) with
                | null -> ()
                | strId -> do
                    let id = strId |> int
                    let cadet = rep.GetAllQueryableAsync().Result.Where(fun c -> c.DisplayId = id).First()
                    rep.RemoveAsync(cadet).Wait()

                match getValue("-d", args) with
                | null -> ()
                | strId -> do
                    let id  = strId |> int |> Nullable
                    let cadets = rep.GetAllQueryableAsync().Result.Where(fun c -> c.DivisionId = id)
                    rep.RemoveRangeAsync(cadets).Wait()

                match getValue("-o", args) with
                | null -> ()
                | strId -> do
                    let oId = strId |> int
                    let dId = this.getOfficer(oId).DivisionId
                    let cadets = rep.GetAllQueryableAsync().Result.Where(fun c -> c.DivisionId = dId)
                    rep.RemoveRangeAsync(cadets).Wait()

            printf "Ok"
