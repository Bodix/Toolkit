# Toolkit.Mcp

Editor actions for an AI agent that drives the Unity editor through Unity MCP. They know nothing about a game, so every project uses the same ones. Editor only, Unity 2022.3 and newer.

## What is inside

- `McpScenario` plays steps one after another (compile, start Play Mode, click, wait, call a method, take a screenshot, stop Play Mode) and saves a report to a file. One command and one wait in the shell replace a command per action. Its description lists the steps.
- `McpConsole` gives the first console error in one short answer and clears the console.
- `McpUi` clicks buttons and toggles by the names of their objects.
- `McpGameView` sets the size of the Game view and shrinks saved screenshots.

How to call each one is written in the description of its class and methods.

## How a project uses it

- A project keeps its own class of ready-made actions and passes it to `McpScenario.Run` as the tools class: the `Do` step calls its methods.
- A game that loads after Play Mode starts sets `McpScenario.PlayReady`. A game that prepares something before Play Mode sets `McpScenario.StartPlay`. Both are set on every domain load, for example in a static constructor of a class with `[InitializeOnLoad]`.
