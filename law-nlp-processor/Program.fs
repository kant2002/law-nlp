open LawNlp.Parser
open FSharp.Data

[<EntryPoint>]
let main argv =
    let документ = HtmlDocument.Load (argv[0])
    let разобранныйДокумент = разобратьАдилетЗанКз документ
    printfn "Разобранный документ: %A" разобранныйДокумент
    0
