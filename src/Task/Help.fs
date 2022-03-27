module Help

let private helpMessage =
    "Commands:\n\
    - help -\n\
    - division -\n\
    - cadet -\n\
    - officer -"

let writeGlobalHelp =
    printf "%s" helpMessage

let getValue (valueOption: string, args: string[]) =
    match Array.tryFindIndex (fun e -> e = valueOption) args with
        | None -> null
        | index -> if index.Value + 1 >= Array.length args then null else args.[index.Value + 1]

let paramNotSet (prms:string[], args:string[]) : option<string> =
    let rec IsNotSet (prms:string[]) : option<string> =
        match getValue(prms[0], args) with
        | null -> Some(prms[0])
        | _ -> if Array.length prms > 1 then
                IsNotSet prms[1..]
                else
                None

    IsNotSet prms

let paramMissing (prms:string[], args:string[]) : bool =
    let rec isMissing (i:int) : bool =
        if i = Array.length prms then false
        else if not <| Array.contains prms.[i] args then true
        else isMissing (i + 1)

    isMissing 0
