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
    | IsSecondary of bool
    | IsLink of bool
    | IsSmall of bool
    | OnClick of (Types.MouseEvent -> unit)
    | ClassName of CSSProp list

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


let inline InspectorControls (props : unit) (elems : ReactElement list) : ReactElement =
    ofImport "InspectorControls" "@wordpress/block-editor" () elems    


type PanelBodyProps = 
    | Title of string
    | InitialOpen of bool

let inline PanelBody (props : PanelBodyProps list) (elems : ReactElement list) : ReactElement =
    ofImport "PanelBody" "@wordpress/components" (keyValueList CaseRules.LowerFirst props) elems    

type RangeControlProps = 
    | Label of string
    | Help of string
    | Value of int
    | Min of int
    | Max of int
    | OnChange of (int -> unit)

let inline RangeControl (props : RangeControlProps list) (elems : ReactElement list) : ReactElement =
    ofImport "RangeControl" "@wordpress/components" (keyValueList CaseRules.LowerFirst props) elems    


// // QR Code
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
    let test = QrCode.qrcode(10, "H")
    test.addData(code, "Byte") |> ignore
    test.make() |> ignore
    test.createDataURL()
    
let getImgSize (size: int): CSSProp list =
    if size = 0 then [ Width "max-content"; Height  "max-content" ]
    else [ Width (sprintf "%dpx" size); Height  (sprintf "%dpx" size) ]

type IAttributes = 
    {|
        text: string
        src: string
        size: int
    |}

let view =
    FunctionComponent.Of(fun (props: {| attributes: IAttributes; setAttributes: (IAttributes -> unit) |}) ->

    div []
        [
            InspectorControls ()
                [
                    PanelBody [ Title "Setting" ]
                        [
                            TextControl [ TextControlProps.Label "Text"; TextControlProps.Value props.attributes.text; TextControlProps.OnChange (fun value -> props.setAttributes({| text = value; src = (createQrCode value); size = props.attributes.size|}))] []
                            RangeControl [ 
                                RangeControlProps.Label "Custom Size"; 
                                RangeControlProps.Help "Set a custom size for the image generated";
                                RangeControlProps.Min 50;
                                RangeControlProps.Max 350;
                                RangeControlProps.Value props.attributes.size
                                RangeControlProps.OnChange (fun value -> props.setAttributes({| text = props.attributes.text; src = props.attributes.src; size = value |}))
                                ] [ ]

                            div [ Style [ Display DisplayOptions.Flex; JustifyContent "flex-end" ] ]
                                [
                                    Button [IsSecondary true; IsSmall true; ButtonProps.OnClick (fun _ -> props.setAttributes({| text = props.attributes.text; src = props.attributes.src; size = 0 |})) ] [ str "Reset Size"]
                                ]
                                
                        ]
                ]
            Placeholder [ Instructions "Paste a link/text to generate a QR Code" ;PlaceholderProps.Label "QR Code Generator"; IsColumnLayout true ]
                [ 
                    div [ ]
                        [
                            TextControl [ TextControlProps.Value props.attributes.text; TextControlProps.OnChange (fun value -> props.setAttributes({| text = value; src = (createQrCode value); size = props.attributes.size|}))] []
                        ]
                    div [ Style [ Display DisplayOptions.Flex; JustifyContent "center" ] ]
                        [
                            img [ Src props.attributes.src; Style (getImgSize props.attributes.size)]
                        ]
                ]
            ]
    
    )
