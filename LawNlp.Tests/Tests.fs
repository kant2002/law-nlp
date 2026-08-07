module Tests

open System
open System.IO
open VerifyTests
open VerifyXunit
open Xunit
open Argon
open LawNlp.Parser

Environment.SetEnvironmentVariable("Verify_DisableClipboard", "true");
VerifierSettings.AddExtraSettings(fun settings -> settings.AddFSharpConverters())
VerifierSettings.UseUtf8NoBom();
let settings = new VerifySettings()
settings.UseDirectory("gold");
settings.DisableDiff();
let verifier = Verifier.BuildVerifier(settings, Path.Combine(__SOURCE_DIRECTORY__, __SOURCE_FILE__));

[<Fact>]
let MyTest () =
    let ruFile = File.ReadAllText(Path.Combine(__SOURCE_DIRECTORY__, "samples", "ru", "V1600014606.html"))
    let parsedData = разобратьHtmlАдилетЗанКз ruFile
    verifier.Verify(parsedData) |> Async.AwaitTask
