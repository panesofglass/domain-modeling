module Domain

(*** define: location-measure-types ***)
[<Measure>] type degLat
[<Measure>] type degLng

let degreesLatitude x = x * 1.<degLat>
let degreesLongitude x = x * 1.<degLng>

(*** define: city ***)
type City = City of name : string
    with
    static member Create (name : string) =
        match name with
        | null | "" ->
            invalidArg "Invalid city"
                "The city name cannot be null or empty."
        | x -> City x

(*** define: location ***)
type Location =
    { latitude : float<degLat>; longitude : float<degLng> }
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

(*** define: place ***)
type Place = { Name : City; Location : Location option }

(*** define: units-of-measure ***)
[<Measure>] type m
[<Measure>] type ft

(*** hide ***)
open FSharp.Data

[<Literal>]
let connStr = """Data Source=(LocalDB)\v11.0;Initial Catalog=Database1;Integrated Security=True;Connect Timeout=10"""

(*** define: sql-command-provider ***)
type GetCityLocation = SqlCommandProvider<"
    SELECT City, Latitude, Longitude
    FROM [dbo].[CityLocations]
    WHERE City = @city
    ", connStr, SingleRow = true>

(*** hide ***)
open System.Collections.Generic

(*** define: workflow-process-types ***)
type LookupLocations = City * City -> Place * Place

type TryFindDistance = Place * Place -> float<m> option

type MetersToFeet = float<m> -> float<ft>

type Show = Place * Place * float<ft> option -> string

(*** define: lookup-locations ***)
let lookupLocation (City name) =
    use cmd = new GetCityLocation()
    let result = cmd.Execute(name)
    match result with
    | Some p ->
        match p.Latitude, p.Longitude with
        | Some lat, Some lng ->
            { Name = City.Create(p.City)
              Location = Location.Create(
                            degreesLatitude lat,
                            degreesLongitude lng) |> Some }
        | _, _ ->
            { Name = City.Create(p.City); Location = None }
    | None -> { Name = City.Create(name); Location = None }

let lookupLocations (start, dest) =
    lookupLocation start, lookupLocation dest

(*** define: find-distance ***)
open System.Device.Location
let toGeoCoordinate = function
    { latitude = lat; longitude = lng } ->
        GeoCoordinate(lat / 1.<degLat>, lng / 1.<degLng>)

let findDistance (start: Location, dest: Location) : float<m> =
    (start |> toGeoCoordinate).GetDistanceTo(dest |> toGeoCoordinate)
    * 1.<m>

let tryFindDistance : TryFindDistance = function
    | { Location = Some start }, { Location = Some dest } ->
        findDistance (start, dest) |> Some
    | _, _ -> None

(*** define: meters-to-feet ***)
let metersToFeet (input: float<m>) =
    input * 3.2808399<ft/m>
    
(*** define: distance-calculator-pipeline ***)
let workflow =
    lookupLocations
    >> tryFindDistance
    >> Option.map metersToFeet

(*** define: serialize ***)
let serializePlace = function
    | { Name = City name; Location = Some loc } ->
        sprintf """{"name":"%s","latitude":%f,"longitude":%f}""" name loc.Latitude loc.Longitude
    | _ -> "null"

let serializeResult place1 place2 distance =
    let place1' = serializePlace place1
    let place2' = serializePlace place2
    match distance with
    | Some d ->
        sprintf """{"start":%s,"dest":%s,"distance":%f}""" place1' place2' d
    | None -> sprintf """{"start":%s,"dest":%s}""" place1' place2'

(*** define: distance-workflow ***)
let rec receiveInput(start, dest) =
    let start', dest' = lookupLocations(start, dest)
    calculateDistance(start', dest')
and calculateDistance(start, dest) =
    match tryFindDistance(start, dest) with
    | Some distance ->
        showPlacesWithDistanceInMeters(start, dest, distance)
    | None -> showPlaces(start, dest, None)
and showPlacesWithDistanceInMeters(start, dest, distance) =
    showPlaces(start, dest, Some(metersToFeet distance))
and showPlaces(start, dest, distance) =
    serializeResult start dest distance

(*** define: stages ***)
type Stage =
    | AwaitingInput
    | InputReceived of start : City * dest : City
    | Located of start : Place * dest : Place
    | Calculated of Place * Place * float<m> option
    | Show of Place * Place * float<ft> option

(*** define: processing-stages ***)
type RunWorkflow = Stage -> string

let rec runWorkflow = function
    | AwaitingInput -> runWorkflow AwaitingInput
    | InputReceived(start, dest) ->
        runWorkflow (Located(lookupLocations(start, dest)))
    | Located(start, dest) ->
        runWorkflow (Calculated(start, dest, tryFindDistance(start, dest)))
    | Calculated(start, dest, distance) ->
        runWorkflow (Show(start, dest, distance |> Option.map metersToFeet))
    | Show(start, dest, distance) -> serializeResult start dest distance
