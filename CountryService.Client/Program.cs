using Calzolari.Grpc.Net.Client.Validation;
using CountryService.Client;
using CountryService.Client.v1;
using CountryService.gRPC.Compression;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Grpc.Net.Client;
using Grpc.Net.Compression;
using Microsoft.Extensions.Logging;
using static CountryService.Client.v1.CountryService;

var loggerFactory = LoggerFactory.Create(logging =>
{
    logging.AddConsole();
    logging.SetMinimumLevel(LogLevel.Trace);
});

var logger = loggerFactory.CreateLogger<TracerInterceptor>();

var handler = new SocketsHttpHandler
{
    KeepAlivePingDelay = TimeSpan.FromSeconds(15),
    PooledConnectionIdleTimeout = TimeSpan.FromMinutes(5),
    // Timeout.InfiniteTimeSpan for infinite idle connection
    KeepAlivePingTimeout = TimeSpan.FromSeconds(5),
    EnableMultipleHttp2Connections = true
};

var channel = GrpcChannel.ForAddress("https://localhost:5003", new
    GrpcChannelOptions
    {
        LoggerFactory = loggerFactory,
        CompressionProviders = new List<ICompressionProvider>
        {
            new BrotliCompressionProvider()
        },
        MaxReceiveMessageSize = 6291456, // 6 MB,
        MaxSendMessageSize = 6291456, // 6 MB
        HttpHandler = handler
});
var countryClient = new CountryServiceClient(channel.Intercept(new TracerInterceptor(logger)));

//using var serverStreamingCall = countryClient.GetAll(new Empty());
//await foreach (var response in serverStreamingCall.ResponseStream.ReadAllAsync())
//{
//    Console.WriteLine($"{response.Name}: {response.Description}");
//}

//// Read headers and trailers
//var serverStreamingCallHeaders = await serverStreamingCall.ResponseHeadersAsync;
//var serverStreamingCallTrailers = serverStreamingCall.GetTrailers();
//var myHeaderValue = serverStreamingCallHeaders.GetValue("myHeaderName");
//var myTrailerValue = serverStreamingCallTrailers.GetValue("myTrailerName");

//using var clientStreamingCall = countryClient.Delete();
//var countriesToDelete = new List<CountryIdRequest>
//{
//    new CountryIdRequest {
//        Id = 1
//    },
//    new CountryIdRequest {
//        Id = 2
//    }
//};

//// Write
//foreach (var countryToDelete in countriesToDelete)
//{
//    await clientStreamingCall.RequestStream.WriteAsync(countryToDelete);
//    Console.WriteLine($"Country with Id {countryToDelete.Id} set for deletion");
//}
//// Tells server that request streaming is done
//await clientStreamingCall.RequestStream.CompleteAsync();
//// Finish the call by getting the response
//var emptyResponse = await clientStreamingCall.ResponseAsync;
//// Read headers and Trailers
//var clientStreamingCallHeaders = await clientStreamingCall.ResponseHeadersAsync;
//var clientStreamingCallTrailers = clientStreamingCall.GetTrailers();
//var myHeaderValue = clientStreamingCallHeaders.GetValue("myHeaderName");
//var myTrailerValue = clientStreamingCallTrailers.GetValue("myTrailerName");
//// var emptyResponse = await clientStreamingCall; // Works as well but cannot read headers and Trailers

//using var bidirectionalStreamingCall = countryClient.Create();
//var countriesToCreate = new List<CountryCreationRequest>
//{
//    new CountryCreationRequest {
//        Name = "France",
//        Description = "Western european country",
//        //CreateDate = Timestamp.FromDateTime(DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc))
//    },
//    new CountryCreationRequest {
//        Name = "Poland",
//        Description = "Eastern european country",
//        //CreateDate = Timestamp.FromDateTime(DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc))
//    }
//};

//// Write
//foreach (var countryToCreate in countriesToCreate)
//{
//    await bidirectionalStreamingCall.RequestStream.WriteAsync(countryToCreate);
//    Console.WriteLine($"Country {countryToCreate.Name} set for creation");
//}

//// Tells server that request streaming is done
//await bidirectionalStreamingCall.RequestStream.CompleteAsync();

//// Read
//await foreach (var createdCountry in bidirectionalStreamingCall.ResponseStream.ReadAllAsync())
//{
//    Console.WriteLine($"{createdCountry.Name} has been created with Id: {createdCountry.Id}");
//}

//// Read headers and Trailers
//var bidirectionnalStreamingCallHeaders = await bidirectionalStreamingCall.ResponseHeadersAsync;
//var bidirectionnalStreamingCallTrailers = bidirectionalStreamingCall.GetTrailers();

//var myHeaderValue2 = bidirectionnalStreamingCallHeaders.GetValue("myHeaderName");
//var myTrailerValue2 = bidirectionnalStreamingCallTrailers.GetValue("myTrailerName");


//var countryCall = countryClient.GetAsync(new CountryIdRequest { Id = 1 });
//var country = await countryCall.ResponseAsync;
//Console.WriteLine($"{country.Id}: {country.Name}");

//// Read headers and Trailers
//var countryCallHeaders = await countryCall.ResponseHeadersAsync;
//var countryCallTrailers = countryCall.GetTrailers();
//var myHeaderValue = countryCallHeaders.GetValue("myHeaderName");
//var myTrailerValue = countryCallTrailers.GetValue("myTrailerName");

//var country2 = await countryClient.GetAsync(new CountryIdRequest
//{
//    Id = 1
//}); // Works as well but Headers and Trailers cannot be accessed

//var countryIdRequest = new CountryIdRequest { Id = 1 };
//try
//{
//    var countryCall = countryClient.GetAsync(countryIdRequest, deadline: DateTime.UtcNow.AddSeconds(30));
//    var country = await countryCall.ResponseAsync;
//    Console.WriteLine($"{country.Id}: {country.Name}");
//    // Read headers and Trailers
//    var countryCallHeaders = await countryCall.ResponseHeadersAsync;
//    var countryCallTrailers = countryCall.GetTrailers();
//    var myHeaderValue = countryCallHeaders.GetValue("myHeaderName");
//    var myTrailerValue = countryCallTrailers.GetValue("myTrailerName");
//}
//catch (RpcException ex) when (ex.StatusCode == StatusCode.DeadlineExceeded)
//{
//    Console.WriteLine($"Get country with Id: {countryIdRequest.Id} has timed out ");
//    var trailers = ex.Trailers;
//    var correlationId = trailers.GetValue("correlationId");
//}
//catch (RpcException ex)
//{
//    Console.WriteLine($"An error occured while getting the country with Id: {countryIdRequest.Id}");
//    var trailers = ex.Trailers;
//    var correlationId = trailers.GetValue("correlationId");
//}

using var bidirectionnalStreamingCall = countryClient.Create();

try
{
    var countriesToCreate = new List<CountryCreationRequest>
    {
        new CountryCreationRequest
        {
            Name = "Japan",
            Description = "",
            //CreateDate = Timestamp.FromDateTime(DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc))
        }
    };
    // Write
    foreach (var countryToCreate in countriesToCreate)
    {
        await bidirectionnalStreamingCall.RequestStream.WriteAsync
        (countryToCreate);
        Console.WriteLine($"Country {countryToCreate.Name} set for creation");
    }
    // Tells server that request streaming is done
    await bidirectionnalStreamingCall.RequestStream.CompleteAsync();
    // Read
    await foreach (var createdCountry in bidirectionnalStreamingCall.ResponseStream.ReadAllAsync())
    {
        Console.WriteLine($"{createdCountry.Name} has been created with Id: {createdCountry.Id}");
    }
}
catch (RpcException ex) when (ex.StatusCode == StatusCode.InvalidArgument)
{
    var errors = ex.GetValidationErrors();
    Console.WriteLine(ex.Message);
}
catch (RpcException ex)
{
    Console.WriteLine(ex.Message);
}

// Perform calls
channel.Dispose();
await channel.ShutdownAsync();