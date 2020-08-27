module App

open Fable.Core
open Fable.React
open Fable.Core.JsInterop
open Browser
open Fable.React.Props

console.log("Fable is up and running...")

type ButtonProps = 
    | IsPrimary of bool
    | OnClick of (Types.MouseEvent -> unit)

let inline Button (props : ButtonProps list) (elems : ReactElement list) : ReactElement =
    ofImport "Button" "@wordpress/components" (keyValueList CaseRules.LowerFirst props) elems

let view =
    FunctionComponent.Of(fun (props: {| initCount: int |}) ->
    let state = Hooks.useState(props.initCount) // This is where the magic happens
    div []
        [ 
        Button
            [ IsPrimary true; OnClick (fun _ -> state.update(fun s -> s + 1)) ]
            [ str "Times clicked: "; ofInt state.current ]
        ]
    )
