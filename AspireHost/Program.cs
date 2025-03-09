using Projects;

var builder = DistributedApplication.CreateBuilder(args);


var consApp = builder.AddProject<ConsoleApp1>("consoleApp");

builder.Build().Run();
