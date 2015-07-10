(**
- title : Domain Modeling with Types
- description : Domain Modeling with Types (using F#)
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

type City = City of string

(** Extract the value via pattern matching: *)
let cityName (City name) = name

(*** define-output:city-name ***)
cityName (City "Houston, TX")

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

## What could go wrong?

1. Latitude or longitude not provided
2. Latitude and longitude mixed up
3. Invalid city and location values

***

## 1. Handling missing values

Examples:

* Atlantis
* Camelot

***

## C# version:

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

## Yuck, right?

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

type Place = { Name : string; Location : Location option }

(**
***

## Note: No `null`

***

## Has-a preferred to Is-a

### "Composition over inheritance"

***

## 2. Mixing up latitude and longitude

***

## Sadly we will have to leave C# behind

***

## Units of measure

*)

[<Measure>] type degLat
[<Measure>] type degLng

let degreesLatitude = (*) 1.<degLat>
let degreesLongitude = (*) 1.<degLng>

(**
***

## Correct `Location`

*)

type Location = {
    Latitude : float<degLat>
    Longitude : float<degLng>
}

type Place = { Name : City; Location : Location }

(**
***

## Make invalid values impossible

***

## Types can only get you so far

1. `Place` allows `null` and `""`
2. `Location` allows latitude < -90 and > 90
3. `Location` allows longitude < -`80 and > 180

***

## 3. Valid values

***

## Revisiting `City`

*)

type City =
    City of name : string
    static member Create (name : string) =
        match name with
        | null | "" ->
            invalidArg "Invalid city"
                "The city name cannot be null or empty."
        | x -> City x

(**

' Without a city lookup, we are stuck with a minimal
' validation for the city name. We could add a bit
' more, such as requiring one or more commas, but
' this provides the general idea.

***

## Better than nothing

***

## Revisiting `Location`

*)

type Location =
    private { latitude : float<degLat>; longitude : float<degLng> }
    member this.Latitude = this.latitude
    member this.Longitude = this.longitude
    static member Create (lat, lng) =
        if -90.<degLat> > lat || lat > 90.<degLat> then
            invalidArg "lat"
                "Latitude must be within the range -90 to 90."
        elif -180.<degLng> > lng && lng > 180.<degLng> then
            invalidArg "lng"
                "Longitude must be within the range -180 to 180."
        else { latitude = lat; longitude = lng }

(**

' This may look strange as we are now hiding
' the record we previously used. However, we
' retain immutability and add validation.

***

## Revisiting `Place`

*)

type Place = { Name : City; Location : Location option }

(**

' Place doesn't really change except to swap
' Name : string for Name : City

***

## Further study: Dependent Types

* Code Contracts
* F*
* Idris

' We have seen that the F# type system can
' get us pretty far along our way. 
' We had to resort to constructor functions
' to provide more rigid validation.
'
' Note that we can still create an invalid `City`.
' However, if we privatize the union, we can't
' retrieve the name via pattern matching.
'
' Some languages offer additional type system
' constructs to do these validations. Code contracts,
' originally from Eiffel, offer one form of this.
' Newer languages have also been working on a concpt
' called dependent types or refinement types. These
' allow you to further specify the range of values
' allowed, as we have done above using our
' constructor functions. I've listed a few examples
' in case you wish to study these further.

***

# Converting `City` to `Place`

***

## UI input: two cities

## Process input: two places

## How do we translate?

***

## Retrieving values from other sources

' For the purposes of our demo application,
' let's assume we have a database of city names
' with their corresponding geographic locations.
' We will use the FSharp.SqlClient type provider
' to provide us the types we need to access the data.

' Why not use a third-party service such as Google Maps?
' I thought of doing this, but I wanted to keep this
' example close to what we are actually doing at Tachyus.
' Also, the SqlClient type providers are really quite cool,
' and I like showing it off. It's also a way we can
' further constrain the available cities, should we have
' time to explore that.

***



***

# Questions?

***

# Resources

* TODO 1
* TODO 2

*)

(*** define: units-of-measure ***)
[<Measure>] type m
[<Measure>] type ft

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

