# ShadowDEX (Alpha V0.0.1)

A competitive version of who's that pokemon to be played with friends. It is missing a lot of Quality of Life and Stability Features, but is running well.

## Instructions for Build and Use

[Software Demo](Put_Your_Video_Link_Here)

Steps to build and/or run the software:

1. Clone the repo to your local machine.
2. Once that is done cd to the Host directory and run dotnet run to start.
3. Once running, run the frontend using a website host of your choosing (I just use VScode, live server).

Instructions for using the software:

1. On the webpage, fill out the create game form. This will give you a join code to let others connect.
2. For other players to join, give them the join code and have them fill out the join game form on the webpage.
3. Once joined, any player can start a game and play.

## Development Environment

To recreate the development environment, you need the following software and/or libraries with the specified versions:

* Dotnet 10.0 with signalR

## Useful Websites to Learn More

I found these websites useful in developing this software:

* [Getting Started with SignalR]([Get started with ASP.NET Core SignalR | Microsoft Learn](https://learn.microsoft.com/en-us/aspnet/core/tutorials/signalr?view=aspnetcore-10.0&WT.mc_id=dotnet-35129-website&tabs=visual-studio-code))

## Future Work

The following items I plan to fix, improve, and/or add to this project in the future:

* [ ] Make the front end look pretty.
* [ ] Move Rendering to server side.
* [ ] Add better disconnect and error handleing.
* [ ] Add config file for various settings including allowed origins.