module App

open System
open System.Text
open Fable.Core
open Fable.React
open Fable.Core.JsInterop
open Browser
open Fable.React.Props

console.log("Fable is up and running...")
 // IMPORTS
type ButtonProps = 
    | IsPrimary of bool
    | OnClick of (Types.MouseEvent -> unit)

let inline Button (props : ButtonProps list) (elems : ReactElement list) : ReactElement =
    ofImport "Button" "@wordpress/components" (keyValueList CaseRules.LowerFirst props) elems


type PlaceholderProps = 
    | Label of string

let inline Placeholder (props : PlaceholderProps list) (elems : ReactElement list) : ReactElement =
    ofImport "Placeholder" "@wordpress/components" (keyValueList CaseRules.LowerFirst props) elems    


type TextControlProps = 
    | Label of string
    | Type of string
    | Value of string 
    | OnChange of (string -> unit)

let inline TextControl (props : TextControlProps list) (elems : ReactElement list) : ReactElement =
    ofImport "TextControl" "@wordpress/components" (keyValueList CaseRules.LowerFirst props) elems    

// QR Code
type IQRCode = 
    abstract addData: string * string -> unit
    abstract make: unit -> unit
    abstract createImgTag: unit -> string
    abstract createASCII: unit -> string
    abstract createDataURL: unit -> string


type IQRCodeGenerator = 
    abstract qrcode: int * string -> IQRCode

[<ImportAll("./qrcode.js")>]
let QrCode: IQRCodeGenerator = jsNative


let createQrCode (code: string) = 
    let test = QrCode.qrcode(10, "L")
    test.addData(code, "Byte") |> ignore
    test.make() |> ignore
    test.createDataURL()

let view =
    FunctionComponent.Of(fun (props: {| initCount: int |}) ->
    let state = Hooks.useState(props.initCount) // This is where the magic happens
    let text = Hooks.useState("") // This is where the magic happens

    Placeholder [ PlaceholderProps.Label "QR Code Generator"]
        [ 
            TextControl [ Label "Code"; OnChange (fun value -> text.update( fun _ -> value))] []
            Button
                [ IsPrimary true; OnClick (fun _ -> state.update(fun s -> s + 1)) ]
                [ str "Times clicked: "; ofInt state.current ]
            
            img [ Src (createQrCode text.current) ]
                
        ]
    )
