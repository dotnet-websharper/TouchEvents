namespace WebSharper.TouchEvents.Sample

open WebSharper
open WebSharper.JavaScript
open WebSharper.UI
open WebSharper.UI.Notation
open WebSharper.UI.Client
open WebSharper.UI.Templating
open WebSharper.TouchEvents

// The template is loaded from the DOM, so editing index.html and refreshing
// the browser is enough; a rebuild is only needed when holes change.
type IndexTemplate = Template<"wwwroot/index.html", ClientLoad.FromDocument>

[<JavaScript>]
module Client =
    let offsetX = Var.Create 0.0 // Offset between touch point and box center (X)
    let offsetY = Var.Create 0.0 // Offset between touch point and box center (Y)
    let status = Var.Create "Touch or drag the box to start."

    let attachTouchEvents () =
        let touchBox = JS.Document.GetElementById("touchBox") |> As<HTMLElement>
        let touchStage = JS.Document.GetElementById("touchStage") |> As<HTMLElement>

        // Handle touchstart event
        touchBox.AddEventListener("touchstart", fun (e: Dom.Event) ->
            let touchEvent = As<TouchEvent>(e)
            let touch = touchEvent.Touches.[0] // Get the first touch point
            let rect = touchBox.GetBoundingClientRect()

            // Calculate the offset between the touch point and the box's position
            offsetX := touch.ClientX - rect.Left
            offsetY := touch.ClientY - rect.Top

            status := "Touch started!"
        )

        // Handle touchmove event
        touchBox.AddEventListener("touchmove", fun (e: Dom.Event) ->
            let touchEvent = As<TouchEvent>(e)
            e.PreventDefault() // Prevent scrolling or default touch behavior

            let touch = touchEvent.Touches.[0] // Get the first touch point
            let stageRect = touchStage.GetBoundingClientRect()

            // Update the position of the box dynamically, relative to the stage
            touchBox.Style.SetProperty("left", sprintf "%.1fpx" (touch.ClientX - stageRect.Left - offsetX.Value))
            touchBox.Style.SetProperty("top", sprintf "%.1fpx" (touch.ClientY - stageRect.Top - offsetY.Value))

            status := sprintf "Moving to: (%.1f, %.1f)" touch.ClientX touch.ClientY
        )

        // Handle touchend event
        touchBox.AddEventListener("touchend", fun (_: Dom.Event) ->
            status := "Touch ended!"
        )

    [<SPAEntryPoint>]
    let Main () =
        IndexTemplate()
            .TouchInit(fun () -> attachTouchEvents ())
            .status(status.V)
            .Bind()
