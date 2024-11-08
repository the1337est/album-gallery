# Gallery App
This Unity project is a 3D gallery app developed with Unity 2022.3.32, using the Universal Render Pipeline (URP). It is designed to be run in either mobile (portrait) or widescreen (landscape) aspect ratios.
 
## Project Structure
Main Scene: Assets/Gaurav/Scenes/Gallery.scene
Entry Point: The app starts with the Gallery scene, which includes the gallery's interface and controls.

AppController controls the app logic
DownloadController manages download of all data from URLs
AlbumEntry controls logic for each album entry object

## Supported Aspect Ratios
To get the best experience, run the app within Unity using one of these two aspect ratios:

9:16 (Mobile, 1080x1920)
16:9 (Widescreen, 1920x1080)

## Gallery Features
Album entries can be instantiated at runtime. Each album entry can clicked to select it, deselecting previously selected album entry

White Light: Indicates that an album entry is currently selected.
Yellow Light: Indicates that an album entry is currently deselected.

## Textures / Images
Loading: loading texture is shown when Album image download is in progress
Error: error texture is shown when album image download fails, which is apparently common with the JSONPlaceholder Photos API
Image: loaded image is drawn on a quad as texture once loaded

If a download fails and error image occurs, Gallery item can be selected again to re-try download

## UI Buttons
There are five UI buttons in the scene to help interact with and manage album entries:

Scroll Left: Scrolls view to the left if possible.
Scroll Right: Scrolls to the right if possible.
Log to Console: Logs all active album entry instances to the Unity Console.
Add Album Entry: Adds a new album entry to the gallery.
Remove Album Entry: Removes the currently selected album entry from the gallery.


## How to Run

Open the project in Unity 2022.3.32.
Open the Gallery scene from Assets/Gaurav/Scenes/Gallery.scene.
Set the Game View aspect ratio to either 9:16 (Mobile) or 16:9 (Widescreen) for optimal viewing.
Play the scene to start the gallery app.
