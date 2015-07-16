module Api

(*** hide ***)
open System
open System.Text
open Arachne.Language
open Arachne.Uri
open Arachne.Http
open Arachne.Uri.Template
open Chiron
open Freya.Core
open Freya.Core.Operators
open Freya.Lenses.Http
open Freya.Machine
open Freya.Machine.Extensions.Http
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
            arr.[1])
    return City.Create(parts.[0]), City.Create(parts.[1]) }

(*** define: get-handler ***)
let get = freya {
    let! cities = cities
    let stage = InputReceived cities
    return runWorkflow stage } |> Freya.memo

let getHandler _ = freya {
    let! json = get
    return represent json }

(*** define: http-config ***)
let en = Freya.init [ LanguageTag.Parse "en" ]
let json = Freya.init [ MediaType.Json ]
let utf8 = Freya.init [ Charset.Utf8 ]

let common =
    freyaMachine {
        using http
        charsetsSupported utf8
        languagesSupported en
        mediaTypesSupported json }

(*** define: resource ***)
let supportedMethods =
    Freya.init [GET; OPTIONS]

let distanceCalculator =
    freyaMachine {
        including common
        methodsSupported supportedMethods
        handleOk getHandler } |> FreyaMachine.toPipeline

let app =
    freyaRouter {
        resource (UriTemplate.Parse "/") distanceCalculator
    } |> FreyaRouter.toPipeline
