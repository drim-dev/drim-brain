var builder = DistributedApplication.CreateBuilder(args);

var loanServiceDb = builder.AddPostgresContainer("postgres")
    .AddDatabase("LoanServiceDb");

var redis = builder.AddRedisContainer("redis");

var blockchainService = builder.AddProject<Projects.BlockchainService>("blockchain-service");

var accountService = builder.AddProject<Projects.AccountService>("account-service")
    .WithReference(blockchainService)
    .WithReplicas(2);

var loanService = builder.AddProject<Projects.LoanService>("loan-service")
    .WithReference(loanServiceDb)
    .WithReplicas(2);

var apiGateway = builder.AddProject<Projects.ApiGateway>("api-gateway")
    .WithReference(accountService)
    .WithReference(loanService)
    .WithReference(redis)
    .WithReplicas(2);

builder.Build().Run();
