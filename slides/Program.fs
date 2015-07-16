open System
open Microsoft.Owin.Hosting
open Freya.Core.Integration

[<Sealed>]
type Startup() =
    member __.Configuration () =
        OwinAppFunc.ofFreya Api.app

[<EntryPoint>]
let main argv = 
    let server = WebApp.Start<Startup>("http://localhost:8000/")
    printfn "Running on port 7000"
    printfn "Press <enter> to quit"
    Console.ReadLine() |> ignore
    server.Dispose()
    0 // return an integer exit code
