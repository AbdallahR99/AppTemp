var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("cache");
var keycloakUsername = builder.AddParameter("keycloakUsername");
var keycloakPassword = builder.AddParameter("keycloakPassword", secret: true);
var keycloak = builder.AddKeycloak("keycloak", 8085, keycloakUsername, keycloakPassword).WithDataBindMount(source: @"C:\keycloak\Data");

var sqlPassword = builder.AddParameter("sqlPassword", secret: true);

var sql = builder.AddSqlServer("sql", sqlPassword)
                 .WithLifetime(ContainerLifetime.Persistent).WithDataBindMount(source: @"C:\SqlServer\Data"); ;

var db = sql.AddDatabase("database");

var apiService = builder.AddProject<Projects.AppTemp_ApiService>("apiservice")
    .WithReference(db)
       .WaitFor(db)
       .WithReference(keycloak).WaitFor(keycloak);

//builder.AddProject<Projects.AppTemp_Web>("webfrontend")
//    .WithExternalHttpEndpoints()
//    .WithReference(cache)
//    .WaitFor(cache)
//    .WithReference(apiService)
//    .WaitFor(apiService);

builder.Build().Run();
