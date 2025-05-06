var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres");
var postgresdb = postgres.AddDatabase("postgresdb");


var redis = builder.AddContainer("redis", "redis")
    .WithLifetime(ContainerLifetime.Persistent);

var apiService = builder.AddProject<Projects.GlobalAzureNetAspire_ApiService>("apiservice")
    .WithHttpsHealthCheck("/health")
    .WithHttpCommand(
        path: "/cache/clear",
        displayName: "Clear cache",
        commandOptions: new HttpCommandOptions()
        {
            Description = "Clear cache",
            IconName = "DocumentLightning",
            IsHighlighted = true
        });

builder.AddProject<Projects.GlobalAzureNetAspire_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithHttpsHealthCheck("/health")
    .WithReference(apiService)
    .WaitFor(apiService);

builder.AddProject<Projects.GlobalAzureNetAspire_DatabaseService>("databaseservice")
    .WithReference(postgresdb)
    .WaitFor(postgresdb);

builder.Build().Run();
