# WebSharper Touch Events API Binding

This repository provides an F# [WebSharper](https://websharper.com/) binding for the [Touch Events API](https://developer.mozilla.org/en-US/docs/Web/API/Touch_events), enabling seamless handling of touch interactions in WebSharper applications.

## Repository Structure

The repository consists of two main projects:

1. **Binding Project**:

   - Contains the F# WebSharper binding for the Touch Events API.

2. **Sample Project**:
   - Demonstrates how to use the Touch Events API with WebSharper syntax.

## Features

- WebSharper bindings for the Touch Events API.
- Easy integration of touch-based interactions in F# web applications.
- Example usage for implementing multi-touch gestures, drag-and-drop, and more.

## Installation and Building

### Prerequisites

- [.NET SDK](https://dotnet.microsoft.com/download) installed on your machine.

### Steps

1. Clone the repository:

   ```bash
   git clone https://github.com/dotnet-websharper/TouchEvents.git
   cd TouchEvents
   ```

2. Build the Binding Project:

   ```bash
   dotnet build WebSharper.TouchEvents/WebSharper.TouchEvents.fsproj
   ```

3. Build and Run the Sample Project:

   ```bash
   cd WebSharper.TouchEvents.Sample
   dotnet build
   dotnet run
   ```

4. Open the sample project in your browser to see it in action.

## Why Use the Touch Events API

The Touch Events API enables developers to:

1. **Enhance User Interactions**: Support touch-based devices for improved user experience.
2. **Implement Complex Gestures**: Build multi-touch interactions such as pinch, zoom, and swipe.
3. **Create Mobile-Friendly Apps**: Optimize applications for touch-first devices like smartphones and tablets.

Integrating the Touch Events API with WebSharper allows F# developers to create interactive and mobile-friendly applications seamlessly.

## How to Use the Touch Events API

### Example Usage

Below is an example of how to use the Touch Events API in a WebSharper project for handling touch interactions:

```fsharp
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
            // Attach touch event handlers to the elements when the page loads
            .TouchInit(fun () ->
                attachTouchEvents() // Call the function to set up touch event listeners
            )
            .Doc()
        |> Doc.RunById "main"
```

This example demonstrates how to handle basic touch interactions such as `touchstart`, `touchmove`, and `touchend`.
