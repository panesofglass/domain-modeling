module Api

(*** hide ***)
open System
open System.Text
open Arachne.Language
open Arachne.Uri
open Arachne.Http
open Arachne.Http.Cors
open Arachne.Uri.Template
open Freya.Core
open Freya.Core.Operators
open Freya.Lenses.Http
open Freya.Lenses.Http.Cors
open Freya.Machine
open Freya.Machine.Extensions.Http
open Freya.Machine.Extensions.Http.Cors
open Freya.Machine.Router
open Freya.Router
open Domain

let inline represent (x : string) =
    { Description =
        { Charset = Some Charset.Utf8
          Encodings = None
          MediaType = Some MediaType.Json
          Languages = Some [ LanguageTag.Parse "en" ] }
      Data = Encoding.UTF8.GetBytes x }

(*** define: parse-query-string ***)
let cities = freya {
    let! qs = Freya.getLens Request.query
    let parts =
        qs.Split('&')
        |> Array.map (fun q ->
            let arr = q.Split('=')
            arr.[1].Trim())
    return City.Create(parts.[0]), City.Create(parts.[1]) }

(*** define: get-handler ***)
let get = freya {
    let! cities = cities
    let stage = InputReceived cities
    return runWorkflow stage } |> Freya.memo

let getHandler _ = freya {
    let! json = get
    return represent json }

(*** define: http-config-defs ***)
let en = Freya.init [ LanguageTag.Parse "en" ]
let utf8 = Freya.init [ Charset.Utf8 ]
let supportedMethods = Freya.init [GET; OPTIONS]
let mediaTypes = Freya.init [ MediaType.Html
                              MediaType.JavaScript
                              MediaType.Css
                              MediaType.Json ]

(*** define: http-config-cors ***)
let corsOrigins = Freya.init AccessControlAllowOriginRange.Any
let corsHeaders = Freya.init [ "accept"; "content-type" ]

(*** define: http-config-common ***)
let common =
    freyaMachine {
        using http
        using httpCors
        charsetsSupported utf8
        corsHeadersSupported corsHeaders
        corsOriginsSupported corsOrigins
        languagesSupported en
        mediaTypesSupported mediaTypes }

(*** define: resource ***)
let distanceCalculator =
    freyaMachine {
        including common
        corsMethodsSupported supportedMethods
        methodsSupported supportedMethods
        handleOk getHandler } |> FreyaMachine.toPipeline

let app =
    freyaRouter {
        resource (UriTemplate.Parse "/calc") distanceCalculator
    } |> FreyaRouter.toPipeline
