(**
- title : Domain Modeling with F#
- description : Domain Modeling with F#
- author : Ryan Riley (@panesofglass)
- theme : night
- transition : default

***

*)

(*** hide ***)
#r "System.Device.dll"
#I "packages"
#r "Aether/lib/net40/Aether.dll"
#r "Hekate/lib/net40/Hekate.dll"
#r "FParsec/lib/net40-client/FParsecCS.dll"
#r "FParsec/lib/net40-client/FParsec.dll"
#r "Chiron/lib/net45/Chiron.dll"
#r "Arachne.Core/lib/net45/Arachne.Core.dll"
#r "Arachne.Uri/lib/net45/Arachne.Uri.dll"
#r "Arachne.Uri.Template/lib/net45/Arachne.Uri.Template.dll"
#r "Arachne.Language/lib/net45/Arachne.Language.dll"
#r "Arachne.Http/lib/net45/Arachne.Http.dll"
#r "Arachne.Http.Cors/lib/net45/Arachne.Http.Cors.dll"
#r "Freya.Core/lib/net45/Freya.Core.dll"
#r "Freya.Lenses.Http/lib/net45/Freya.Lenses.Http.dll"
#r "Freya.Lenses.Http.Cors/lib/net45/Freya.Lenses.Http.Cors.dll"
#r "Freya.Recorder/lib/net45/Freya.Recorder.dll"
#r "Freya.Machine/lib/net45/Freya.Machine.dll"
#r "Freya.Machine.Extensions.Http/lib/net45/Freya.Machine.Extensions.Http.dll"
#r "Freya.Machine.Extensions.Http.Cors/lib/net45/Freya.Machine.Extensions.Http.Cors.dll"
#r "Freya.Router/lib/net45/Freya.Router.dll"
#r "Freya.Machine.Router/lib/net45/Freya.Machine.Router.dll"

(**

# Domain Modeling with F#

## [Ryan Riley](https://twitter.com/panesofglass)

***

<img alt="Tachyus logo" src="images/tachyus.png" height="100px" style="background-color:#fff;" />

***

# [@panesofglass](https://twitter.com/panesofglass)

***

# https://github.com/panesofglass

***

# [OWIN](http://owin.org/)

***

![F# logo](images/fssf.png)

***

# Objective

## Visualize distance between two cities by their geographic location

***

## Process

*)

(*** include: location-calculator-pipeline ***)

(**

' This pipeline shows that we want to take a list of two Cities as inputs,
' translate those cities into locations,
' and finally calculate the distance (in feet).

***

# Goal 1

## Define `City`

***

## Simplest thing possible (C#)

    [lang=cs]
    using City = string;

***

## Simplest thing possible (F#)

*)

type City = string

(**
***

## Does this really help?

' Maybe: you get better documentation throughout your code;
' however, you don't get any type safety.

***

## Can we do better?

***

## Single-cased union types

*)

(*** include: city ***)
(*** include: city-example ***)

(** Extract the value via pattern matching: *)
(*** include: magic ***)

(*** include: magic-city ***)
(*** include-output:magic-city ***)

(**
***

# Goal 2

## Define a `Location` type

***

## `Place` type (C#)

    [lang=cs]
    public interface ILocatable {
        float Latitude { get; }
        float Longitude { get; }
    }

    public class Place : ILocatable {
        public string Name { get; set; }
        public float Latitude { get; set; }
        public float Longitude { get; set; }
    }

***

## `Place` type (F#)

*)

type ILocatable =
    abstract Latitude : float
    abstract Longitude : float

type Place = { 
    Name : string
    Latitude : float
    Longitude : float }
    interface ILocation with
        member this.Latitude = this.Latitude
        member this.Longitude = this.Longitude

(**
***

## Can we do better?

***

## Rethinking `ILocatable`

### "Is-a" vs. "Has-a"

***

## `Latitude` and `Longitude` belong together

***

## Refactored `Place` type (C#)

    [lang=cs]
    public class Location {
        public float Latitude { get; set; }
        public float Longitude { get; set; }
    }

    public class Place {
        public string Name { get; set; }
        public Location Location { get; set; }
    }

***

## Refactored `Place` type (F#)

*)

type Location = { Latitude : float; Longitude : float }

type Place = { Name : string; Location : Location }

(**
***

## What did we gain?

Consider: an invalid city name

***

## F# `Place` with optional `Location`

*)

type Location = { Latitude : float; Longitude : float }

type Place = { Name : string; Location : Location option }

(**
***

## Note: No `null`

***

## F# `Place` with optional `ILocatable`

*)

type Place = { 
    Name : string
    Latitude : float option
    Longitude : float option }
    member this.GetLocation() : ILocatable option =
        match this.Latitude, this.Longitude with
        | Some lat, Some lng ->
            { interface ILocation with
                member this.Latitude = lat
                member this.Longitude = lng } |> Some
        | _ -> None

(**
***

## And the C# version:

    [lang=cs]
    class Locatable : ILocatable {
        public float Latitude { get; set; }
        public float Longitude { get; set; }
    }

    public class Place {
        public string Name { get; set; }
        public float? Latitude { get; set; }
        public float? Longitude { get; set; }
        public ILocatable GetLocation() {
            if (Latitude.HasValue && Longitude.HasValue) {
                return new Locatable {
                    Latitude = Latitude.Value,
                    Longitude = Longitude.Value
                };
            } else {
                return null; // Oh noes!
            }
        }
    }

***

## Yuck, right?

### Note: you could use a `Nullable` struct for `Locatable`

***

## Conclusion: Has-a often wins

### "Composition over inheritance"

***

# Questions?

***

# Resources

* TODO 1
* TODO 2

*)

(*** define: city ***)
type City = City of string

(*** define: city-example ***)
let city = City "Houston, TX"

(*** define: magic ***)
let magic (City name) = name

(*** define-output:magic-city ***)
magic city

(*** define: units-of-measure ***)
[<Measure>] type degLat
[<Measure>] type degLng
[<Measure>] type m
[<Measure>] type ft

(*** define: location ***)
type Location = {
    Latitude : float<degLat>
    Longitude : float<degLng> }
    static member Create(lat : float, lng : float) =
        if -180. <= lng && lng <= 180. && -90. <= lat && lat <= 90. then
            { Latitude = lat * 1.<degLat>; Longitude = lng * 1.<degLng>
        else failwith "Invalid location"

(*** define: place ***)
type Place = { Name : City; Location : Location }

(*** define: geo-locate ***)
let geoLocate (city: City) =
    // Get coordinates
    let location = Location.Create(0., 0.)
    location

(*** define: find-distance ***)
let findDistance : Location list -> float<ft> = function
    | [loc1; loc2] ->
        // Convert locations to GeoLocations
        // Calcualte distance in m
        // Convert to feet
        0.<ft>
    | [] | [_] -> failwith "Too few locations provided"
    | _ -> failwith "Too many locations provided"
    

(*** define: location-calculator-pipeline ***)
[start; dest]
|> List.map geoLocate
|> findDistance

