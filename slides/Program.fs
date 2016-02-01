open System
open Microsoft.Owin.Hosting
open Freya.Core.Integration
open Suave
open Suave.Files
open Suave.Logging
open Suave.Operators
open Suave.Owin
open Suave.Web

[<Sealed>]
type Startup() =
    member __.Configuration () =
        OwinAppFunc.ofFreya Api.app

[<EntryPoint>]
let main argv = 
    // Katana HttpListener
//    let server = WebApp.Start<Startup>("http://localhost:7000/")
//    printfn "Running on port 7000"
//    printfn "Press <enter> to quit"
//    Console.ReadLine() |> ignore
//    server.Dispose()

    // Suave
    let serverConfig = 
        { defaultConfig with
           homeFolder = Some (IO.Path.GetFullPath(__SOURCE_DIRECTORY__))
           bindings = [ HttpBinding.mkSimple HTTP "0.0.0.0" 7000 ]
           logger = Loggers.saneDefaultsFor LogLevel.Verbose
        }
    let app =
      choose [
        OwinApp.ofAppFunc "/api" (OwinAppFunc.ofFreya Api.app)
        Writers.setHeader "Cache-Control" "no-cache, no-store, must-revalidate"
        >=> Writers.setHeader "Pragma" "no-cache"
        >=> Writers.setHeader "Expires" "0"
        >=> browseHome ]
    startWebServer serverConfig app

    0 // return an integer exit code
