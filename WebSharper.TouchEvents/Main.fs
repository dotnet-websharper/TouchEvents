namespace WebSharper.TouchEvents

open WebSharper
open WebSharper.JavaScript
open WebSharper.InterfaceGenerator

module Definition =
    let TouchInit =
        Pattern.Config "TouchInit" {
            Required = [
                "identifier", T<int64>
                "target", T<Dom.EventTarget>
            ]
            Optional = [
                "clientX", T<float>
                "clientY", T<float>
                "screenX", T<float>
                "screenY", T<float>
                "pageX", T<float>
                "pageY", T<float>
                "radiusX", T<float>
                "radiusY", T<float>
                "rotationAngle", T<float>
                "force", T<float>
            ]
        }

    let TouchList = 
        Class "TouchList"

    let Touch = 
        Class "Touch" 
        |=> Nested [TouchInit]
        |+> Static [
            Constructor !?TouchInit?options
        ]
        |+> Instance [
            "identifier" =? T<int>
            "screenX" =? T<float>
            "screenY" =? T<float>
            "clientX" =? T<float>
            "clientY" =? T<float>
            "pageX" =? T<float>
            "pageY" =? T<float>
            "target" =? T<Dom.Element>
            "radiusX" =? T<float>
            "radiusY" =? T<float>
            "rotationAngle" =? T<float>
            "force" =? T<float>
        ]

    TouchList
    |+> Instance [
        "length" =? T<int> 
        "item" => T<int>?index ^-> Touch
    ] |> ignore


    let TouchEventOptions =
        Pattern.Config "TouchEventOptions" {
            Required = []
            Optional = [
                "touches", TouchList.Type
                "targetTouches", TouchList.Type
                "changedTouches", TouchList.Type
                "ctrlKey", T<bool>
                "shiftKey", T<bool>
                "altKey", T<bool>
                "metaKey", T<bool>
            ]
        }

    let TouchEvent =
        Class "TouchEvent"
        |=> Inherits T<Dom.Event> 
        |=> Nested [TouchEventOptions]
        |+> Static [
            Constructor (T<string>?``type`` * !?TouchEventOptions?options)
        ]
        |+> Instance [
            "altKey" =? T<bool>
            "changedTouches" =? TouchList
            "ctrlKey" =? T<bool>
            "metaKey" =? T<bool>
            "shiftKey" =? T<bool>
            "targetTouches" =? TouchList
            "touches" =? TouchList
        ]

    let Assembly =
        Assembly [
            Namespace "WebSharper.TouchEvents" [
                Touch
                TouchEvent
                TouchList
            ]
        ]

[<Sealed>]
type Extension() =
    interface IExtension with
        member ext.Assembly =
            Definition.Assembly

[<assembly: Extension(typeof<Extension>)>]
do ()
