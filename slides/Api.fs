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
          Languages = Some [ LanguageTag.parse "en" ] }
      Data = Encoding.UTF8.GetBytes x }

(*** define: parse-query-string ***)
let cities = freya {
    let! qs = Freya.Optic.get Request.query_
    let parts = qs |> fst Query.pairs_
    let (Some [_, Some city1; _, Some city2]) = parts
    return City.Create city1, City.Create city2 }

(*** define: get-handler ***)
let get = freya {
    let! cities = cities
    let stage = InputReceived cities
    return runWorkflow stage } |> Freya.memo

let getHandler _ = freya {
    let! json = get
    return represent json }

(*** define: http-config-common ***)
let mediaTypes = [ MediaType.Html; MediaType.JavaScript
                   MediaType.Css;  MediaType.Json ]
let common =
    freyaMachine {
        using http
        using httpCors
        charsetsSupported Charset.Utf8
        corsHeadersSupported [ "accept"; "content-type" ]
        corsOriginsSupported AccessControlAllowOriginRange.Any
        languagesSupported (LanguageTag.parse "en")
        mediaTypesSupported mediaTypes }

(*** define: resource ***)
let distanceCalculator =
    freyaMachine {
        including common
        corsMethodsSupported [GET; OPTIONS]
        methodsSupported [GET; OPTIONS]
        handleOk getHandler }

let app =
    freyaRouter {
        resource (UriTemplate.parse "/calc") distanceCalculator
    }
