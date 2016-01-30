module Program

open System
open Freya.Core.Integration
open Microsoft.Owin.Hosting
open Suave
open Suave.Logging
open Suave.Owin

[<Sealed>]
type Startup() =
    member __.Configuration () =
        OwinAppFunc.ofFreya Api.app

[<EntryPoint>]
let main args =
    // Suave
//    let serverConfig = 
//        { defaultConfig with
//           bindings = [ HttpBinding.mkSimple HTTP "127.0.0.1" 7000 ]
//           logger = Loggers.saneDefaultsFor LogLevel.Verbose
//        }
//    let app =
//        OwinApp.ofAppFunc "/" (OwinAppFunc.ofFreya Api.app)
//    startWebServer serverConfig app

    // Katana HttpListener
    let server = WebApp.Start<Startup>("http://localhost:7000/")
    printfn "Running on port 7000"
    printfn "Press <enter> to quit"
    Console.ReadLine() |> ignore
    server.Dispose()

    0
