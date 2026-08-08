module Tests

open System
open System.IO
open VerifyTests
open VerifyXunit
open Xunit
open Argon
open LawNlp.Parser
open FSharp.Data

Environment.SetEnvironmentVariable("Verify_DisableClipboard", "true")
VerifierSettings.AddExtraSettings(fun settings -> settings.AddFSharpConverters())
VerifierSettings.UseUtf8NoBom()
let settings = new VerifySettings()
settings.UseDirectory("gold")
settings.DisableDiff()

let verify (data: obj) =
    let verifier = Verifier.BuildVerifier(settings, Path.Combine(__SOURCE_DIRECTORY__, __SOURCE_FILE__))
    verifier.Verify(data) |> Async.AwaitTask

let readSample (lang) (filename: string) =
    let path = Path.Combine(__SOURCE_DIRECTORY__, "samples", lang, filename)
    File.ReadAllText(path)

[<Fact>]
let MyTest () =
    let ruFile = readSample "ru" "V1600014606.html"
    let parsedData = разобратьHtmlАдилетЗанКз ruFile
    verify parsedData
    
[<Fact>]
let MyTest2 () =
    let ruFile = readSample "ru" "V1600014592.html"
    let parsedData = разобратьHtmlАдилетЗанКз ruFile
    verify parsedData

[<Fact>]
let MyTest3 () =
    let ruFile = readSample "kk" "V1600014606.html"
    let parsedData = разобратьHtmlАдилетЗанКз ruFile
    verify parsedData

[<Fact>]
let ``Стандартное разделение точкой`` () =
    let узел = HtmlNode.NewText("Приказ Министра национальной экономики Республики Казахстан от 23 декабря 2016 года № 525. Зарегистрирован в Министерстве юстиции Республики Казахстан 27 декабря 2016 года № 14592.")
    let parsedData = разобратьПереходы узел
    verify parsedData

[<Fact>]
let ``Разделение переходов когда используется г. для обозначения года`` () =
    let узел = HtmlNode.NewText("Приказ Министра национальной экономики Республики Казахстан от 23 декабря 2016 г. № 525. Зарегистрирован в Министерстве юстиции Республики Казахстан 27 декабря 2016 г. № 14592.")
    let parsedData = разобратьПереходы узел
    verify parsedData

[<Fact>]
let ``Разделение переходов когда используется г. без пробела`` () =
    let узел = HtmlNode.NewText("Приказ Министра национальной экономики Республики Казахстан от 23 декабря 2016г. № 525. Зарегистрирован в Министерстве юстиции Республики Казахстан 27 декабря 2016 г. № 14592.")
    let parsedData = разобратьПереходы узел
    verify parsedData

[<Fact>]
let ``Разделение переходов когда используется и.о.`` () =
    let узел = HtmlNode.NewText("Приказ Министра национальной экономики Республики Казахстан от 23 декабря 2016 г. № 525. Утратил силу приказом и.о. Министра финансов Республики Казахстан от 2 мая 2012 года № 229.")
    let parsedData = разобратьПереходы узел
    verify parsedData

[<Fact>]
let ``Разделение переходов когда используется И.о. с большой буквы`` () =
    let узел = HtmlNode.NewText("Приказ Министра национальной экономики Республики Казахстан от 23 декабря 2016 г. № 525. Утратил силу приказом И.о. Министра финансов Республики Казахстан от 2 мая 2012 года № 229.")
    let parsedData = разобратьПереходы узел
    verify parsedData

[<Fact>]
let ``Разделение переходов когда используется точка как разделитель даты`` () =
    let узел = HtmlNode.NewText("Постановление Правления Национального Банка Республики Казахстан от 9 апреля 2001 года N 89. Зарегистрировано в Министерстве юстиции Республики Казахстан 17.04.2001 г. за N 1466. Утратило силу - постановлением Правления Национального Банка РК от 13 сентября 2004 года N 119.")
    let parsedData = разобратьПереходы узел
    verify parsedData

[<Fact>]
let ``Разделение переходов когда используется см.`` () =
    let узел = HtmlNode.NewText("Приказ Министра национальной экономики Республики Казахстан от 23 декабря 2016 г. № 525. Утратил силу приказом И.о. Министра финансов Республики Казахстан от 2 мая 2012 года № 229 (извлечение из приказа см. ниже).")
    let parsedData = разобратьПереходы узел
    verify parsedData

[<Fact>]
let ``Разделение переходов когда используется ст.`` () =
    let узел = HtmlNode.NewText("Бюллетень нормативных правовых актов центральных исполнительных и иных государственных органов Республики Казахстан, 2001 г. , N 40-41, ст.524. Утратил силу - приказом Министра государственных доходов РК от 9.04.2002 № 416 (извлечение из приказа см.ниже)")
    let parsedData = разобратьПереходы узел
    verify parsedData