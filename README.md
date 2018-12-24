# Video Auto Splitter

The Video Auto Splitter for LiveSplit allows scripts to automatically split, start, reset, and detect loading based on visual events from a video feed. Currently in beta.

## How to use

### Requirements

* [LiveSplit 1.7.6](http://livesplit.org/) or later.
* An existing game profile.
  * If a profile for the game you want to use the video auto splitter for has not been made and you are willing to make one yourself, visit [Creating a Profile](#creating-a-profile).
* A DirectShow output of the video feed.
  * For OBS, this can be done with the [Virtual Cam plugin](https://obsproject.com/forum/resources/obs-virtualcam.539/). You don't need a webcam for this, the plugin creates a fake one. See [FAQs](#help) for how to install and use the plugin for this component.
  * XSplit also has a [Virtual Camera feature](https://www.youtube.com/watch?v=WxPJdUtEae8).
  * You can use the direct video feed from your capture device, but it may prevent your capturing software from recognizing it too.

### Installation

1. Download the [latest version](https://github.com/ROMaster2/LiveSplit.VideoAutoSplit/releases) of the component.
2. Find your LiveSplit folder and extract the contents into the Components folder. If you are updating from a previous version, replace the existing files.
3. Start LiveSplit.
4. In LiveSplit's layout editor, click the big + button, hover over `Control`, and add the `Video Auto Splitter` component.
5. Open the component's settings.
6. Click on the Browse... button and locate the game profile you wish to use.
7. Select the video capture. You will most likely want to use OBS-Camera or XSplitBroadcaster. If not, see [How do I use Virtual Cam for OBS?](#how-do-i-use-virtual-cam-for-obs) for help.
8. Verify the video capture by selecting the "Scan Region" tab. If it doesn't include the feed you're expecting, try another video capture. If none of the options have the feed, visit [Help](#help) to diagnose the problem.
9. Set the capture area of the feed. Your video feed will most likely not be one-to-one with the profile and has to be cropped to fit it. The blue boxes show the areas being watched for by the profile. Use the preview type 'Feature' with preview feature to check if the feed is aligned properly.

Unfortunately, because of how component settings are saved, **the settings are tied to the layout file**. This will be changed at a later date.

## Creating a Profile

Creating a profile is very similar to creating a regular [Auto Splitter script](https://github.com/LiveSplit/LiveSplit/blob/master/Documentation/Auto-Splitters.md), but instead of monitoring memory values, it monitors regions of a video feed and compares it against an image, or in some cases, the previous frame, and returns a confidence value. The `vas` file type is used to hold the game profiles contain three important parts: The structure, the script, and the images.

###### Don't let the file extension confuse you, it's just a renamed zip.

### Finding static reference points

Before *any* of that can be made, the references the component will told to watch for must be found. The difficulty in finding reliable references varies greatly between games. Generally, big references that change abruptly to and/or from a compared image are the most reliable.

The same base screen reference must be used throughout making the profile. This is so users only need to align the screen with their video feed to have all the references automatically fit into place. Raw, high quality gameplay footage is recommended when extracting the reference images.

There's currently no interface to help structure a profile. At this time, if you would like help building one, [ask in the #video-autosplitter channel in the Speedrun Tool Development Discord](https://discord.gg/6HD5jtQ).

The instructions for creating a profile will be expanded on during beta.

## Contributing

Any and all help with development, be it with finding or squashing bugs or adding new features, would be greatly appreciated.

### How to Compile

1. Create a clone of LiveSplit following [its instructions](https://github.com/LiveSplit/LiveSplit#contributing).
2. In the `/LiveSplit/Components` folder of the above, create a clone of the VAS component: `git clone https://github.com/ROMaster2/LiveSplit.VideoAutoSplit.git`
3. Inside Visual Studio, right click the `Components/Control` folder, hover over Add, and select "Existing Project...". Go through the folders to `/LiveSplit/Components/LiveSplit.VideoAutoSplit` and select `LiveSplit.VideoAutoSplit.csproj`.

You should now be able to debug and compile the component with LiveSplit.

## Help

Help and FAQs will be expanded on during beta. If you have problems with using the component, [request help in the #video-autosplitter channel in the Speedrun Tool Development Discord](https://discord.gg/6HD5jtQ).

### How do I use Virtual Cam for OBS?

* Download the plugin [here](https://obsproject.com/forum/resources/obs-virtualcam.539/) and run the installer.
* Follow the installation as usual.
* Once installed, there's two ways it can be used:
  * Make a virtual cam for the entire stream. This uses very little CPU but might require you to adjust both your stream layout in OBS and the scan region in the component. This can be enabled in OBS under `Tools > Virtual Cam`.
  * Make a virtual cam for only the game output. This uses more CPU but is much easier to manage in the component. This can be enabled by adding a `VirtualCam` filter to the Source of the game capture in OBS.
* Click on the start button. The default settings are fine to use. I recommend checking `AutoStart` so the virtual cam automatically starts with OBS.

### My video feed looks crooked

I've yet to find the root cause of this, but it's not the component's fault. If enough people run into this problem, I know who to contact help find the cause.

## License

This program and its source code are under The MIT License (MIT).

The [Accord](http://accord-framework.net/) binaries included in releases are under the [GNU Lesser General Public License](http://accord-framework.net/license.txt).

The [Magick.NET](https://github.com/dlemstra/Magick.NET) binaries included in releases are under Apache License 2.0.

The MIT License (MIT)

Copyright (c) 2018 ROMaster2

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
