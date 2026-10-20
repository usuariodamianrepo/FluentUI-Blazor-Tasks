using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var apiService = builder.AddProject<BackEnd_API>("backendapi")
    .WithExternalHttpEndpoints();

builder.AddProject<FrontEnd_Blazor>("frontend")
    .WithExternalHttpEndpoints()
    .WithReference(apiService)
    .WaitFor(apiService);

builder.Build().Run();
