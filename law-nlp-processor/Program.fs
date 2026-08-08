open LawNlp.Parser
open FSharp.Data
open Argu

type Arguments =
    | Html of url: string
    | File of path: string

    interface IArgParserTemplate with
        member s.Usage =
            match s with
            | Html _ -> "specify a HTML URL."
            | File _ -> "specify a file path."


[<EntryPoint>]
let main argv =
    let reader = EnvironmentVariableConfigurationReader() :> IConfigurationReader
    let parser = ArgumentParser.Create<Arguments>(programName = "law-nlp-processor")
    let results = parser.Parse(argv, configurationReader = reader)
    let htmlOpt = results.TryGetResult Html
    let fileOpt = results.TryGetResult File
    match htmlOpt, fileOpt with
    | Some htmlUrl, None ->
        let документ = HtmlDocument.Load htmlUrl
        let разобранныйДокумент = разобратьАдилетЗанКз документ
        printfn "Разобранный документ: %A" разобранныйДокумент
    | None, Some filePath ->
        let документ = HtmlDocument.Load filePath
        let разобранныйДокумент = разобратьАдилетЗанКз документ
        //printfn "Разобранный документ: %A" разобранныйДокумент
        for line in разобранныйДокумент.Переходы do
            printfn "%s" line
    | _, _ ->
        printfn "Invalid arguments. Please specify either a HTML URL or a file path."
    0
