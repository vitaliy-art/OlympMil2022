module Error

let private commandNotFoundMessage c m =
    $"Error: command \"{c}\" not found in module \"{m}\""

let commandNotFound c m =
    printf "%s" <| commandNotFoundMessage c m

let private notEnoughParamsMessage c m =
    $"Error: not enough params for command \"{c}\" in module \"{m}\". Type \"{m} {c} help\" for details"

let notEnoughParams c m =
    printf "%s" <| notEnoughParamsMessage c m

let private notSetParameterMessage p c =
    $"Error: not set parameter \"{p}\" for command \"{c}\""

let notSetParameter p c =
    printf "%s" <| notSetParameterMessage p c
