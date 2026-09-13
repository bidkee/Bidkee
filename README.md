# Bidkee.Client

NuGet package / namespace: `Bidkee.Client`  
Local program: `BidkeeClient`

End users (including a Grok Bot that only consumes NuGet) query tickets and upload tickets. No `iss_` token, no `grok` subcommand, no enroll.

```bash
dotnet add package Bidkee.Client
```

```csharp
using Bidkee.Client;

using var bidkee = BidkeeGateway.Connect(new BidkeeOptions {
    BaseUrl = "https://bidkee.com"
});

var found = await bidkee.Public.SearchAsync("did:bidkee:kaspa:…");
var ticket = await bidkee.Public.GetAsync(checkCode);
var file = await bidkee.Public.DownloadAsync(checkCode, "json");

var uploaded = await bidkee.Public.UploadAsync(json, "ticket.json");
var offline = await bidkee.Public.VerifyOfflineAsync(json);
```

```bat
dotnet run --project src\BidkeeClient -- search kaspa:q
dotnet run --project src\BidkeeClient -- get <checkCode>
dotnet run --project src\BidkeeClient -- upload ticket.json
dotnet run --project src\BidkeeClient -- offline ticket.json
```

Anonymous upload follows the site daily cap. If the server has `AllowAnonymousIngest=false`, ingest returns 401 — that is Plat config, not a missing client method.

Issuer / agent-pass APIs stay under `Agents` and `Bidkee.Client.Ops`. End users should not call them.
