using BookBeat.Akamai.EdgeAuthToken;

if (args.Length!=2) {
    Console.Error.WriteLine("Usage: dotnet run <key> <url-to-unlock>");
    Environment.Exit(-1);
}

var key = args[0];
var uri = new Uri(args[1]);

var uriBuilder = new UriBuilder(uri);

var tokenConfig = new AkamaiTokenConfig
{
	Window = 86400, // 24h, Time to live (in seconds)
	Acl = uriBuilder.Path, // Access control list containing token permissions
	Key = key // Encryption key
};

var tokenGenerator = new AkamaiTokenGenerator();

var token = tokenGenerator.GenerateToken(tokenConfig);
uriBuilder.Query = "hdnea="+token;

Console.WriteLine("Unlocked uri:");
Console.WriteLine(uriBuilder.Uri.ToString());
using var hc = new HttpClient();
var req = new HttpRequestMessage(HttpMethod.Head, uriBuilder.Uri);
var res = await hc.SendAsync(req);
Console.WriteLine($"Checked : {res.StatusCode}");
System.Diagnostics.Debug.Assert(res.StatusCode == System.Net.HttpStatusCode.OK);