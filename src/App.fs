module App

open Fable.Core
open Fable.React
open Fable.Core.JsInterop
open Browser
open Fable.React.Props


(* --------------------------------- WordPress Imports --------------------------------- *)
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


let inline InspectorControls (props : unit list) (elems : ReactElement list) : ReactElement =
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


type SelectOption = 
    {| 
        label: string
        value: string 
    |}

type SelectControlProps = 
    | Label of string
    | Help of string
    | Value of string
    | Options of SelectOption array
    | OnChange of (string -> unit)

let inline SelectControl (props : SelectControlProps list) (elems : ReactElement list) : ReactElement =
    ofImport "SelectControl" "@wordpress/components" (keyValueList CaseRules.LowerFirst props) elems    

(* --------------------------------- QR Code Import --------------------------------- *)
type IQRCode = 
    abstract addData: string * string -> unit
    abstract make: unit -> unit
    abstract createImgTag: unit -> string
    abstract createASCII: unit -> string
    abstract createDataURL: int -> string


type IQRCodeGenerator = 
    abstract qrcode: int * string -> IQRCode

[<ImportAll("./qrcode.js")>]
let QrCode: IQRCodeGenerator = jsNative

let createQrCode (code: string) (size: int) (correctionLevel: string)  = 
    let test = QrCode.qrcode(0, correctionLevel)
    test.addData(code, "Byte") |> ignore
    test.make() |> ignore
    test.createDataURL size
    

(* --------------------------------- Helpers --------------------------------- *)
[<Emit("$0 === undefined")>]
let isUndefined (x: 'a) : bool = jsNative

type IGenerate = 
    | Code of string
    | Size of int
    | CorrectionLevel of string

type IAttributes = 
    {|
        text: string
        src: string
        size: int
        correctionLevel: string
    |}

let view =
    FunctionComponent.Of((fun (props: {| attributes: IAttributes; setAttributes: (IAttributes -> unit) |}) ->

    let editMode = Hooks.useState( isUndefined props.attributes.src )

    let attributes = props.attributes
    let setAttributes = props.setAttributes

    let generate (value: IGenerate) =
        match value with 
        | Code(x) -> (createQrCode x attributes.size attributes.correctionLevel) 
        | Size(x) -> (createQrCode attributes.text x attributes.correctionLevel) 
        | CorrectionLevel(x) -> (createQrCode attributes.text attributes.size x )

    div []
        [
            InspectorControls [ ]
                [
                    PanelBody [ Title "Setting" ]
                        [
                            TextControl [ 
                                TextControlProps.Label "Text"; 
                                TextControlProps.Value attributes.text; 
                                TextControlProps.OnChange (fun value -> setAttributes({| attributes with text = value; src = (generate (Code value)) |}))
                                ] []
                            SelectControl [ 
                                SelectControlProps.Label "Error Correction Level"
                                SelectControlProps.Help "Raising this level improves error correction capability but also increases the amount of data"
                                SelectControlProps.Value attributes.correctionLevel
                                SelectControlProps.Options [| 
                                    {| label = "L (7%)" ; value = "L"|} ;
                                    {| label = "M (15%)"; value = "M"|} ;
                                    {| label = "Q (25%)"; value = "Q"|} ;
                                    {| label = "H (30%)"; value = "H"|} ;
                                |]
                                SelectControlProps.OnChange (fun value -> setAttributes({| attributes with src = (generate (CorrectionLevel value)); correctionLevel = value |})) 
                            ] [ ]
                            RangeControl [ 
                                RangeControlProps.Label "Size"; 
                                RangeControlProps.Help "Set a custom size for the generated image";
                                RangeControlProps.Min 2;
                                RangeControlProps.Max 20;
                                RangeControlProps.Value attributes.size
                                RangeControlProps.OnChange (fun value -> setAttributes({| attributes with src = (generate (Size value)); size = value; |}))
                                ] [ ]  
                        ]
                ]
           
            if editMode.current then 
                Placeholder [ Instructions "Paste a link/text to generate a QR Code"; PlaceholderProps.Label "QR Code Generator"; IsColumnLayout true ]
                    [ 
                        div [ ]
                            [
                                TextControl [ 
                                    TextControlProps.Value attributes.text; 
                                    TextControlProps.OnChange (fun value -> props.setAttributes({| attributes with text = value; src = (generate (Code value));|}))
                                    ] []
                            ]
                        
                        div [ ]
                            [
                                div [ Style [ Display DisplayOptions.Flex; JustifyContent "center"; ] ]
                                    [
                                        img [ Src attributes.src; Style [ Width "max-content"; Height  "max-content" ]; Alt attributes.text]
                                    ]
                            ]
                        if not (isUndefined attributes.src) then
                            div [  Style [ Display DisplayOptions.Flex; JustifyContent "center"; ] ]
                                [
                                    Button [ IsPrimary true; OnClick (fun _ -> editMode.update(fun _ -> false)) ]
                                        [str "Looks Good"]
                                ]
                   
                  
                    ]
            else 
                div [ Style [ Display DisplayOptions.Flex; JustifyContent "center"; ] ]
                    [
                        img [ Src attributes.src; Style [ Width "max-content"; Height  "max-content" ]; Alt attributes.text]
                    ]
        ]
    
    ), memoizeWith = equalsButFunctions)
