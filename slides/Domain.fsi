module Domain

[<Measure>] type degLat
[<Measure>] type degLng

type City

type City with
    static member Create : name : string -> City

type Location

type Location with
    member Latitude : float<degLat>
    member Longitude : float<degLng>
    static member Create : lat: float<degLat> * lng: float<degLng> -> Location

type Place = { Name : City; Location : Location option }

[<Measure>] type m
[<Measure>] type ft

type LookupLocations = City * City -> Place * Place

type TryFindDistance = Place * Place -> float<m> option

type MetersToFeet = float<m> -> float<ft>

type Show = Place * Place * float<ft> option -> string

val lookupLocations : LookupLocations

val tryFindDistance : TryFindDistance

val metersToFeet : MetersToFeet

val receiveInput : City * City -> string
val calculateDistance : Place * Place -> string
val showPlacesWithDistanceInMeters : Place * Place * float<m> -> string
val showPlaces : Place * Place * float<ft> option -> string
    
type Stage =
    | AwaitingInput
    | InputReceived of start : City * dest : City
    | Located of start : Place * dest : Place
    | Calculated of Place * Place * float<m> option
    | Show of Place * Place * float<ft> option

type RunWorkflow = Stage -> string

val runWorkflow : RunWorkflow
