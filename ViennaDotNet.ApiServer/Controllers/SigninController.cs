using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;
using System.Text.RegularExpressions;
using ViennaDotNet.ApiServer.Utils;
using ViennaDotNet.Common.Utils;

namespace ViennaDotNet.ApiServer.Controllers;

[ApiVersion("1.1")]
public partial class SigninController : ControllerBase
{
    [GeneratedRegex("^[0-9A-F]{16}$")]
    private static partial Regex GetUserIdRegex();

    [HttpPost("api/v{version:apiVersion}/player/profile/{profileID}")]
    public async Task<IActionResult> Post(string profileID, CancellationToken cancellationToken)
    {
        if (profileID != "signin")
        {
            return BadRequest();
        }

        SigninRequest? signinRequest = await Request.Body.AsJsonAsync<SigninRequest>(cancellationToken);

        string[]? parts = null;
        if (signinRequest is null || (parts = signinRequest.sessionTicket.Split('-')).Length < 2)
        {
            Log.Error($"Sign in request null or parts bad ({parts?.Length ?? -1})");
            return BadRequest();
        }

        string userId = parts[0];
        if (!GetUserIdRegex().IsMatch(userId))
        {
            Log.Error($"User id not match ({userId})");
            return BadRequest();
        }

        // TODO: check credentials

        // TODO: generate secure session token
        string token = userId.ToUpperInvariant();

        string str = JsonConvert.SerializeObject(new EarthApiResponse(new JObject(
            new JProperty("authenticationToken", token),
            new JProperty("basePath", "/1"),
            new JProperty("clientProperties", new JObject()),
            new JProperty("mixedReality", null!),
            new JProperty("mrToken", null!),
            new JProperty("streams", null!),
            new JProperty("tokens", new JObject()),
            new JProperty("updates", new JObject()))
        ));

        return Content(str, "application/json");
    }

    private sealed record SigninRequest(string sessionTicket);
}
