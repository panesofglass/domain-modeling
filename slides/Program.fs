module Program

open Freya.Core.Integration
open Suave
open Suave.Logging
open Suave.Owin

[<EntryPoint>]
let main args =
    let serverConfig = 
        { defaultConfig with
           bindings = [ HttpBinding.mkSimple HTTP "127.0.0.1" 7000 ]
           logger = Loggers.saneDefaultsFor LogLevel.Verbose
        }
    let app =
        OwinApp.ofAppFunc "/" (OwinAppFunc.ofFreya Api.app)
    startWebServer serverConfig app
    0
