[<EntryPoint>]
let main args =
    let operation = args.[0]
    let a = args.[1] |> int
    let b = args.[2] |> int

    match operation with
    | "+" -> a + b |> printf "%d"
    | "-" -> a - b |> printf "%d"
    | "*" -> a * b |> printf "%d"
    | _ -> ()

    0
