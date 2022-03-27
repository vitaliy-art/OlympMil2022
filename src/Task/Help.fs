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
