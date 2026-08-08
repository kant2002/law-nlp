module LawNlp.Parser
open FSharp.Data
open System

type СтатусДокумента =
    | УтратилСилу
    | Обновленный
    | Новый

type НомерДокумента = string
type Актор = 
| Должность of string
| Учреждение of string
| Страна of string


type ИдентификаторДокумента =
    | Бюллетень of string * string * int (* год *)
    | Закон of Актор * DateOnly * НомерДокумента
    | Кодекс of Актор * DateOnly * НомерДокумента
    | Комментарий of Актор * Актор * DateOnly * НомерДокумента * string option (*комментарий*)
    | Постановление of Актор * DateOnly * НомерДокумента
    | Приказ of Актор list * DateOnly * НомерДокумента
    | Распоряжение of Актор * DateOnly * НомерДокумента
    | Решение of Актор * DateOnly * НомерДокумента
    | СовместныйПриказ of (Актор * DateOnly * НомерДокумента) list
    | Соглашение of DateOnly
    | Указ of Актор * DateOnly * НомерДокумента

type ПереходСтатусаДокумента =
    | СозданниеДокумента of ИдентификаторДокумента
    | РегистрацияДокумента of string * DateOnly * НомерДокумента
    | УтратаСилыДокумента of ИдентификаторДокумента

type ДокументАдилетЗанКз = {
    Название: string
    Статус: СтатусДокумента
    Переходы: string list
}

let разобратьСтатусДокумента (узел: HtmlNode) =
    match (узел.HasClass("status_yts"), узел.HasClass("status_upd"), узел.HasClass("status_new")) with
    | true, false, false -> УтратилСилу
    | false, true, false -> Обновленный
    | false, false, true -> Новый
    | _ -> failwithf "Неизвестный статус документа: %s (%s)" (узел.InnerText()) (узел.ToString())

let разобратьПереходы (узел: HtmlNode) =
    let переходы = 
        узел.InnerText().Split([|'.'|], StringSplitOptions.RemoveEmptyEntries)
        |> Array.map (fun s -> s.Trim())
        |> Array.toList
    переходы |> List.fold (fun (коллектор: string list) переход ->
        if String.IsNullOrWhiteSpace(переход) then коллектор
        else
            let последнийПереход = List.tryLast коллектор
            let склеить (последнийПереход: string) (склейка: string) (переход: string) =
                let новыйПоследний = последнийПереход + склейка + переход
                List.append (List.take (List.length коллектор - 1) коллектор) [новыйПоследний]
            match последнийПереход with
            | Some последнийПереход when 
                                (" 0123456789".LastIndexOf(последнийПереход[последнийПереход.Length - 2]) <> -1 && последнийПереход.EndsWith("г"))
                                || последнийПереход.EndsWith(" см")
                                || последнийПереход.EndsWith(" ст")
                                || последнийПереход.EndsWith(" и.о")
                                || последнийПереход.EndsWith(" И.о")
                                || ("0123456789".LastIndexOf(последнийПереход[последнийПереход.Length - 1]) <> -1
                        && "N".LastIndexOf(переход[0]) <> -1)->
                склеить последнийПереход ". " переход
            | Some последнийПереход when 
                    (последнийПереход.EndsWith(" и") && переход = "о")
                    || (последнийПереход.EndsWith(" И") && переход = "о")
                    || ("0123456789".LastIndexOf(последнийПереход[последнийПереход.Length - 1]) <> -1
                        && "0123456789".LastIndexOf(переход[0]) <> -1)->
                склеить последнийПереход "." переход
            | _ -> List.append коллектор [переход]
    ) []

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