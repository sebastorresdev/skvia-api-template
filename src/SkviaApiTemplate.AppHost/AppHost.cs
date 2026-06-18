var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.SkviaApiTemplate_WebApi>("skviaapitemplate-webapi");

builder.Build().Run();
