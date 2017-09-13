module Site


open Elmish


// Types


type Route = Index
           | BlogIndex


type Model =
    { counter : int
      route : Route
      query : string }


type Msg = Increment
         | Decrement
         | RouteUrl of Route


// Routing


open Elmish.Browser.UrlParser
open Elmish.Browser.Navigation


let route =
    oneOf
        [ map Index (top)
          map BlogIndex (s "blog") ]


let toPath route =
    match route with

    | Index ->
        "/"

    | BlogIndex ->
        "/blog/"


let urlUpdate (result : Option<Route>) model =
    match result with

    | Some page ->
        { model with route = page; query = "" }, Cmd.none

    | None ->
        ( model, Navigation.modifyUrl (toPath model.route) )


// view helpers


open Fable.Inferno.Props
module R = Fable.Inferno


let viewLink route description dispatch =
    R.a [ Href (toPath route)
          OnClick (fun e -> dispatch <| RouteUrl route // route our application
                            e.preventDefault() // prevent brower from routing to server-rendered version
                            ) ]
        [ R.str description ]


let menu model dispatch =
  R.nav []
      [ viewLink Index "Index" dispatch
        viewLink BlogIndex "Blog" dispatch ]


// program


let initialModel =
    { counter = 0
      route = Index
      query = "" }


let init result =
    let (model, cmd) = urlUpdate result initialModel
    initialModel, Cmd.none


let update (msg:Msg) model =
    match msg with

    | Increment ->
        { model with counter = model.counter + 1 }, Cmd.none

    | Decrement ->
        { model with counter = model.counter - 1 }, Cmd.none

    | RouteUrl route ->
        { model with route = route }, Navigation.newUrl (toPath route)


let view model dispatch =
  R.div []
      [ menu model dispatch
        R.button [ OnClick (fun _ -> dispatch Decrement) ] [ R.str "-" ]
        R.div [] [ R.str (sprintf "%A" model) ]
        R.button [ OnClick (fun _ -> dispatch Increment) ] [ R.str "+" ] ]



open Elmish.Inferno


Program.mkProgram init update view
|> Program.withInferno "elmish-app"
|> Program.toNavigable (parsePath route) urlUpdate
|> Program.run
