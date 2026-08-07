module Tests

open System
open System.IO
open VerifyTests
open VerifyXunit
open Xunit
open Argon
open LawNlp.Parser

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