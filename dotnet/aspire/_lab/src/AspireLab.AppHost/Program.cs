var builder = DistributedApplication.CreateBuilder(args);

var loanDb = builder.AddPostgresContainer("postgres").AddDatabase("LoanServiceDb");

var blockchainService = builder.AddProject<Projects.BlockchainService>("blockchain-service");

var accountService = builder.AddProject<Projects.AccountService>("account-service")
    .WithReference(blockchainService);

var loanService = builder.AddProject<Projects.LoanService>("loan-service")
    .WithReference(loanDb);

var apiGateway = builder.AddProject<Projects.ApiGateway>("api-gateway")
    .WithReference(accountService)
    .WithReference(loanService);

builder.Build().Run();
