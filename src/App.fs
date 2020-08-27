module App

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
    | ClassName of string
    | Instructions of string
    | IsColumnLayout of bool


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
    let text = Hooks.useState("") // This is where the magic happens

    Placeholder [ PlaceholderProps.Label "QR Code Generator"; IsColumnLayout true ]
        [ 
            div [ ]
                [
                    TextControl [ OnChange (fun value -> text.update( fun _ -> value))] []
               
                ]
            div [ Style [ Display DisplayOptions.Flex; JustifyContent "center" ] ]
                [
                    img [ Src (createQrCode text.current); Style [  Width "max-content" ]]
                ]
                
        ]
    )
