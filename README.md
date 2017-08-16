# retrolib-unity
A library of code that I use for my personal projects inside Unity3D


# PrefabLibrary
A scriptable object to hold prefabs in a key-value pair.  This is a quick workaround to loading prefab sets dynamically though a key ID.  A good use of this is for quick object theming.  This is recomended instead of resources.load.

# RetroConsole
A console window to display the unity log in the game view.  This is helpful when debugging mobile and you want to see the log. 

Console commands are also available through this console window.  Use the Retro.Command.RetroCommand attribute on any function that returns void and has primitive data types as arguments(e.g. string, int, float, etc).  

With functionality to simulate UE4's console in terms of ease of adding commands and toggling it, you can press the grave key (\`) to open the console.

# RetroEvents
An basic event system.  Is a requirement of RetroInput.  You can browse through that code to figure out how to use it.

# RetroFSM
A Finite State Machine Library implementation that allows configureable states, overridable states, and dynamic loading and unloading of states. Allows FSMs to be created and linked together by class attributes. See the example project for core usage. 

# RetroInput
A dynamic input system to map keyboard and controller buttons to gameplay-related commands.  Allows different controller layouts to be set to help with cross platform button mapping.  Require RetroEvents.  Is a fairly slow mess of a system, but is great for small to medium cross platform projects.

# RetroLog
An in-editor console output window that allows for output taggings(similar to UE4 or Android's logcat).  You can filter logs based on the tag and the log level(warning, error, etc).  Also allows to click to jump to each step of a log's callstack.  

Created with Unity Pro's theme, so the colors and basic functionality is fairly discusting in the Free Version. 

It is very slow when a lot of logs have been logged, it has a big memory footprint, and wont autoclear.  It is recomended to only use when needed.

# RetroPath
A very basic barebone node-based pathing system.

# RetroTypes
Contains a Serializeable dictionary for the inspector.  Generally I odnt use this anymore, but sometimes I need it so I keep it with me.

# RetroGrid
A basic grid/tile based implementation for creating basic grid based games.  Contains a basic grid navigator for A* pathing

