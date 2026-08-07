module LawNlp.Parser
open FSharp.Data
open System

type СтатусДокумента =
    | УтратилСилу

type ДокументАдилетЗанКз = {
    Название: string
    Статус: СтатусДокумента
    Переходы: string list
}

let разобратьСтатусДокумента (узел: HtmlNode) =
    match (узел.HasClass("status_yts")) with
    | true -> УтратилСилу
    | _ -> failwithf "Неизвестный статус документа: %s" (узел.InnerText())

let разобратьПереходы (узел: HtmlNode) =
    let переходы = 
        узел.InnerText().Split([|'.'|], StringSplitOptions.RemoveEmptyEntries)
        |> Array.map (fun s -> s.Trim())
        |> Array.toList
    переходы

let разобратьАдилетЗанКз (документ: HtmlDocument): ДокументАдилетЗанКз =
    let разборСss (селектор: string) (обработчик: HtmlNode -> 'a) =
        документ.CssSelect(селектор)
        |> List.map обработчик
        |> List.exactlyOne
    let название = разборСss "h1" (fun у -> у.InnerText())
    let статус = разборСss ".status" разобратьСтатусДокумента
    let переходы = разборСss ".container_alpha.slogan p" разобратьПереходы
    let контейнерТекста = разборСss ".container_gamma.text" (fun у -> у.InnerText())
    { Название = название; Статус = статус; Переходы = переходы }

let разобратьHtmlАдилетЗанКз (html: string): ДокументАдилетЗанКз =
    let документ = HtmlDocument.Parse html
    разобратьАдилетЗанКз документ