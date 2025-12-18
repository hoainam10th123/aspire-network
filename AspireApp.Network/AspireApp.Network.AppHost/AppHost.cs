
var builder = DistributedApplication.CreateBuilder(args);

var keycloak = builder.AddKeycloak("keycloakauth", 8080)
    .WithDataVolume("keycloak-data");
//.WithRealmImport("../infra/realms");

var postgres = builder.AddPostgres("postgres", port: 5432)
    .WithDataVolume("postgres-data")
    .WithPgAdmin();

var postDb = postgres.AddDatabase("postdb");

var rabbitmq = builder.AddRabbitMQ("messaging")
    .WithDataVolume("rabbitmq-data")
    .WithManagementPlugin(port: 15672);

var postService = builder.AddProject<Projects.PostService>("postservice")
    .WithReference(postDb)
    .WithReference(keycloak)
    .WithReference(rabbitmq)
    .WaitFor(keycloak)
    .WaitFor(rabbitmq)
    .WaitFor(postDb);

var typesense = builder.AddContainer("typesense", "typesense/typesense", "29.0")
    .WithArgs("--data-dir", "/data", "--api-key", "abc", "--enable-cors")
    .WithVolume("typesense-data", "/data")
    .WithHttpEndpoint(8108, 8108, name: "typesense");

var typesenseContainer = typesense.GetEndpoint("typesense");

var searchService = builder.AddProject<Projects.SearchService>("searchservice")
    .WithEnvironment("typesense-api-key", "abc")
    .WithReference(typesenseContainer)
    .WithReference(rabbitmq)
    .WaitFor(rabbitmq)
    .WaitFor(typesense);

var roomDb = postgres.AddDatabase("roomdb");

var roomService = builder.AddProject<Projects.RoomService>("roomservice")
    .WithReference(roomDb)
    .WithReference(keycloak)// loi 401 ,neu khong co
    .WaitFor(roomDb)
    .WaitFor(keycloak);

var chatDb = postgres.AddDatabase("chatdb");

var chatService = builder.AddProject<Projects.ChatService>("chatservice")
    .WithReference(rabbitmq)
    .WaitFor(rabbitmq)
    .WithReference(chatDb)
    .WaitFor(chatDb);

//var notificationDb = mysql.AddDatabase("notificationdb");

//builder.AddProject<Projects.NotificationService>("notificationservice")
//    .WithReference(notificationDb)
//    .WithReference(rabbitmq)
//    .WaitFor(notificationDb)
//    .WaitFor(rabbitmq);


var yarp = builder.AddYarp("gateway")
    .WithHostPort(5002)
    .WithConfiguration(yarpBuilder =>
    {
        yarpBuilder.AddRoute("/posts/{**catch-all}", postService);

        //http://localhost:5002/UploadedFiles/387324e2-e0b4-43e8-b53f-d48e70fb42f2.jpg
        //forward image url to postservice
        yarpBuilder.AddRoute("/UploadedFiles/{**catch-all}", postService);
        yarpBuilder.AddRoute("/searchpost/{**catch-all}", searchService);        
        yarpBuilder.AddRoute(chatService);
        yarpBuilder.AddRoute("/rooms/{**catch-all}", roomService);
    });

builder.Build().Run();
