module LawNlp.Parser
open FSharp.Data

type СтатусДокумента =
    | УтратилСилу

type ДокументАдилетЗанКз = {
    Название: string
    Статус: СтатусДокумента
}

let разобратьСтатусДокумента (узел: HtmlNode) =
    match (узел.HasClass("status_yts")) with
    | true -> УтратилСилу
    | _ -> failwithf "Неизвестный статус документа: %s" (узел.InnerText())

let разобратьHtmlАдилетЗанКз (html: string): ДокументАдилетЗанКз =
    let документ = HtmlDocument.Parse html
    let название = документ.CssSelect("h1") |> List.map (fun у -> у.InnerText()) |> List.exactlyOne
    let статус = документ.CssSelect(".status") |> List.map разобратьСтатусДокумента |> List.exactlyOne
    let контейнерТекста = документ.CssSelect(".container_gamma.text") |> List.exactlyOne
    { Название = название; Статус = статус }