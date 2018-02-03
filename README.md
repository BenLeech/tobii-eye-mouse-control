# Tobii EyeX/4c Mouse Control
Global mouse control with Tobii EyeX/4c, with heavy data filters.

## Table of Contents
1. [Documentation and References](#documentation-and-references)
2. [Setting up the 4c](#setting-up-the-4c)
3. [Getting started](#getting-started)
4. [Using Tobii SDK in your own projects](#using-tobii-sdk-in-your-own-projects)
5. [Creating your own data streams from the Tobii SDK](#creating-your-own-data-streams-from-the-tobii-sdk)
6. [Smoothing Algorithm](#smoothing-algorithm)
7. [Known Issues](#known-issues)
8. [Operating System Support and Language Bindings](#operating-system-support-and-language-bindings)
9. [Tobii License Information](#tobii-license-information)

## Documentation and References
[Tobii Handbook](https://tobii.github.io/CoreSDK/articles/intro.html) (Helpful tutorials, slightly outdated)

[Tobii API Documentation](https://tobii.github.io/CoreSDK/api/index.html) (Unfinished, slightly outdated)

[Tobii Developer Forums](http://developer.tobii.com/community-forums/)

[Implementation Samples](https://github.com/Tobii/CoreSDK/tree/master/samples)

## Setting up the 4c
1. Position the eye tracker below your screen, angled slightly up.
2. Plug the eye tracker into your computer's USB slot
3. Download the [Tobii Eye Tracking Core Software](https://tobiigaming.com/getstarted/)
4. Open and follow the on-screen instructions to install.
5. Click the Tobii Eye Tracking menu from the bottom right of the Window's taskbar
6. Set-up a profile for each user (for most accurate results) and calibrate

## Getting started
To open or run this application locally, first clone/download/fork this repository.

[Cloning a repository](https://help.github.com/articles/cloning-a-repository/)

[Forking a repository](https://help.github.com/articles/fork-a-repo/)

Run the application from ./TobiiEyeXGrid/TobiiEyexGridApplication.cs, the main class.

## Using Tobii SDK in your own projects
If you start a project from scratch, you will need to add the Tobii Core SDK into your project first. Here is a method to reference the SDK using NuGet Package Manager. The installation guides from Tobii's development website are outdated, so following this guide is recommended.

#### Use Nuget Package Installation
1. Open Visual Studio
2. Ensure that Nuget Package Manager is installed by clicking the Tools menu and looking for 'Nuget Package Manager'.
   - If it is not there, go to 'Tools -> Get Tools and Features', search for NuGet Package Manager, and install.
3. Expand the Tool menu and click the Options
4. Expand the NuGet Package Manager from the left tree view
5. Choose the Package Sources
6. If nuget.org https://api.nuget.org/v3/index.json isn't listed as a package source:
   - Type org in the name field
   - Type https://api.nuget.org/v3/index.json repository URL in the source field
   - Click on Update
   - Restart Visual Studio
7. Ensure that you have the nuget package source from the above step. Close NuGet Package Manager options if you have it open.
8. Expand your project in the Solution Explorer
9. Right click on the References
10. Choose Manage Nuget Packages… from the menu
12. Click Browse
13. Choose the org package source to the right
14. Search for Tobii
15. Choose package Tobii.Interaction
16. Click the latest stable version (I am using v0.73)
17. Click Install

## Creating your own data streams from the Tobii SDK
To create any data streams using the Tobii SDK, you first need to instantiate a host from the Tobii Interaction package.
```
using Tobii.Interaction;

...

private Host host = new Host();
```
The host creates a connection between the application and the Interaction engine, andis the main provider for the various eye tracking features. Intantiating a host provides one Tobii eye tracking context, and it is possible to create more than one host in a process. 

To start a gaze point data stream, call Streams.CreateGazePointDataStream() on a host. Calling GazePoint(x,y,ts) on the data stream exposes functional access to the stream, where x is the gaze's X position relative to the top left corner of the screen, where y is the gaze's Y position relative to the top left corner of the screen, and where ts is the timestamp of when the data node was recorded.
```
GazePointDataStream gazePointDataStream = host.Streams.CreateGazePointDataStream();

gazePointDataStream.GazePoint((x, y, ts) => {
        Console.WriteLine("Gaze Position and Timestamp: {0}\t X: {1} Y:{2}", ts, x, y));
});
```

Another option on how to implement the same functionality is my using the 'Next' event on the gaze point data stream instance
```
GazePointDataStream gazePointDataStream = host.Streams.CreateGazePointDataStream();
gazePointDataStream.Next += OnGazePointData;

private OnGazePointData(object sender, StreamData<GazePointData> streamData){
       Console.WriteLine("Gaze Position and Timestamp: {0}\t X: {1} Y:{2}", streamData.Data.Timestamp, streamData.Data.X, streamData.Data.Y); 
}
```

You can also choose to unfilter or lightly filter the gaze data in the creation of the data stream. There are currently only two options, unfiltered and lightly filtered. By default it is set at lightly filtered, but you can specify to unfiltered the stream by passing the enum value in the CreateGazePointDataStream() method params.

```
GazePointDataStream gazePointDataStream = host.Streams.CreateGazePointDataStream(GazePointDataMode.Unfiltered);
```

“Lightly filtered” is an adaptive filter which is weighted based on the age of the gaze data points GazePointData and the velocity of the eye movements. This filter is designed to remove noise and in the same time being responsive to quick eye movements. It is recommended to keep the filter on.

There are also three other types of data streams currently.
- Eye Position (physical location of the eyes)
- Fixation (stream of position and time of where the gaze gets fixated)
- Head Pose (phyiscal location of the head, only works on the 4c).

You can destory a host by calling the dispose method on a host instance.
```
host.Dispose();
```

## Smoothing Algorithm
This application contains basic live data smoothing techniques and algorithms to help smooth the noise of the eye tracking data stream.

If you are interested in possibling improving performance, Professor Manu Kumar from Stanford University has published a paper 
containing pseudocode for the implementation of a smoothing algorithm specifically for eye tracking.

It can be found here:
http://hci.stanford.edu/cstr/reports/2007-03.pdf

## Libraries used in this Application

## Known Issues
##### Offset gaze coordinates and window scale
   * Window's scale setting must be set at 1:1 or 100%, otherwise gaze coordinates will be offset and not where the user is actually looking.

##### Tobii EyeX is not compatible with Tobii v2.11
   * A complete uninstall is required between each use with 2.11. There are no issues with the Tobii 4c

## Operating System Support and Language Bindings
Operating Systems Supported: Windows7, Windows8, Windows10

Language Bindings: C#

## Tobii License Information
This application and any applications developed during the hackathon event are under Tobii's Interactive Use license agreement. This means that the SDK can be used freely to develop games or other software where eye tracking data is used as a user input for interactive experiences. 

However, this application and any applications developed during the hackathon event must not be used for analytic use. Tobii defines analytical use as the following.

> Analytical Use is where eye tracking data is either (a) stored; or (b) transferred to another computing device or network; in both cases where the intent is to use or make it possible to use eye tracking data to analyze, record, visualize or interpret behavior or attention.

[Download and read the full license agreement for Tobii SDK](http://developer.tobii.com/?wpdmdl=203)

