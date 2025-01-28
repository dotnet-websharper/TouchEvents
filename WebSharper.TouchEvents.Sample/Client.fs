namespace WebSharper.TouchEvents.Sample

open WebSharper
open WebSharper.JavaScript
open WebSharper.UI
open WebSharper.UI.Notation
open WebSharper.UI.Client
open WebSharper.UI.Templating
open WebSharper.TouchEvents

// The templates are loaded from the DOM, so you just can edit index.html
// and refresh your browser, no need to recompile unless you add or remove holes.
type IndexTemplate = Template<"wwwroot/index.html", ClientLoad.FromDocument>

[<JavaScript>]
module Client =
    let offsetX = Var.Create 0.0 // Offset between touch point and box center (X)
    let offsetY = Var.Create  0.0 // Offset between touch point and box center (Y)

    let attachTouchEvents () =
        let touchBox = JS.Document.GetElementById("touchBox") |> As<HTMLElement>
        let status = JS.Document.GetElementById("status") |> As<HTMLElement>

        // Handle touchstart event
        touchBox.AddEventListener("touchstart", fun (e: Dom.Event) ->
            let touchEvent = As<TouchEvent>(e)
            let touch = touchEvent.Touches.[0] // Get the first touch point
            let rect = touchBox.GetBoundingClientRect()

            // Calculate the offset between the touch point and the box's position
            offsetX := touch.ClientX - rect.Left
            offsetY := touch.ClientY - rect.Top

            status.TextContent <- "Touch started!"
        )

        // Handle touchmove event
        touchBox.AddEventListener("touchmove", fun (e: Dom.Event) ->
            let touchEvent = As<TouchEvent>(e)
            e.PreventDefault() // Prevent scrolling or default touch behavior

            let touch = touchEvent.Touches.[0] // Get the first touch point

            // Update the position of the box dynamically
            touchBox.Style.SetProperty("left", sprintf "%fpx" (touch.ClientX - offsetX.Value))
            touchBox.Style.SetProperty("top", sprintf "%fpx" (touch.ClientY - offsetY.Value))

            status.TextContent <- sprintf "Moving to: (%f, %f)" touch.ClientX touch.ClientY
        )

        // Handle touchend event
        touchBox.AddEventListener("touchend", fun (_: Dom.Event) ->
            status.TextContent <- "Touch ended!"
        )
    
    [<SPAEntryPoint>]
    let Main () =

        IndexTemplate.Main()
            .TouchInit(fun () -> 
                attachTouchEvents()
            )
            .Doc()
        |> Doc.RunById "main"
