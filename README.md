# BASTA! Spring 2024 - Scalable .NET apps with async await - internals and practical advice

In this repo, you can find the slides and example code for Kenny Pflug's talk about .NET async await internals which was held at the BASTA! Spring 2024 conference in Frankfurt a. Main.

## Prerequisites

You require the [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0) for the code example. The code can run on multiple platforms and has been tested on Windows and Ubuntu. If you experience problems, please [create an issue](https://github.com/thinktecture-labs/basta-spring-2024-dotnet-async-await/issues/new) in this repo.

## How to use the example code

### Example 1 - scalability of the ASP.NET Core services

The first example illustrates how blocking the worker threads of the .NET Thread Pool can influence the responsiveness of an ASP.NET Core web app.

To start the backend, run the following commands in your terminal:

```bash
cd ./01-AsyncVsSync/AsyncVsSync.Backend
dotnet run -c Release
```

Please note that the backend has logging disabled as this might influence the result. Also, always run performance tests in Release mode (`-c Release`), otherwise non-optimized code will tamper the results.

To start the CLI app, open another terminal and execute the following commands:

```bash
cd ./01-AsyncVsSync/AsyncVsSync.App
dotnet run -c Release -- -n=1000 -w=1000 -t=async
```

The CLI app will run `n` requests concurrently against the target service endpoint `t` (which can be either "async" or "sync"). The endpoint will either execute `Thread.Sleep` (sync) or `await Task.Delay` for the amount of milliseconds specified in `w`. Use `dotnet run -- --help` to get a detailed explanation for all options.

You should see that the sync endpoint requires more worker threads for identical loads because the .NET Thread Pool will create new worker threads when it sees its current set of worker threads sleeping while work items still need to be processed by it. Play around with the different values for `n` and `w` to see how different levels of concurrency and wait times affect the .NET thread pool.

There is also a docker compose file which starts up the backend in resource-constrained environment (4 CPUs and 1024MB of RAM by default). In the "ß1-AsyncVsSync" folder, execute the following commands

```bash
docker compose up -d
docker exec -it async-vs-sync-app sh

# Within the async-vs-sync-app container
./AsyncVsSync.App -n=1000 -w=1000 -t=async -u="http://backend"
```

Here, you can also play around with the number of cores and the memory and how this affects the .NET thread pool.

### Example 2 - async await decompiled

This Avalonia app shows how an async method looks like when it is decompiled. The important code is in [MainWindowViewModel.cs](https://github.com/thinktecture-labs/basta-spring-2024-dotnet-async-await/blob/main/02-AsyncDecompiled/AsyncDecompiled.AvaloniaApp/MainWindowViewModel.cs). In the constructor in line 14, the `CalculateCommand` is instantiated, pointing to one of three methods:

- `CalculateSynchronously` executes long running code on the UI thread, which causes UI freezing.
- `CalculateOnBackgroundThread` moves the long-running code to a .NET Thread Pool worker via `Task.Run`.
- `CalculateOnBackgroundThreadDecompiled` does the same thing, but shows how the lowered C# code for async methods would actually look like.

Switch between the three different methods by assigning one of them to the `CalculateCommand` in line 14 and investigate how the `AsyncStateMachine` is structured.
