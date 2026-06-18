using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;

namespace GamxorOila.IntegrationTests;

public class FamilyCareApiTests(FamilyCareApiFactory factory) : IClassFixture<FamilyCareApiFactory>
{
    private readonly HttpClient _client = factory.CreateClient();
    private static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web);

    private static string NewDevice() => "dev-" + Guid.NewGuid().ToString("N")[..8];

    private async Task<JsonElement> PostAsync(string path, object body)
    {
        var res = await _client.PostAsJsonAsync(path, body, Json);
        res.EnsureSuccessStatusCode();
        var text = await res.Content.ReadAsStringAsync();
        return JsonDocument.Parse(text).RootElement.Clone();
    }

    [Fact]
    public async Task Bootstrap_returns_seeded_state_with_trailing_slash_route()
    {
        var device = NewDevice();
        // Mijoz trailing slash bilan yuboradi.
        var root = await PostAsync("api/bootstrap/", new { deviceId = device });

        var state = root.GetProperty("state");
        Assert.False(state.GetProperty("isLoading").GetBoolean());
        Assert.True(state.GetProperty("isBackendOnline").GetBoolean());

        // selfMember + 3 ta oila a'zosi.
        Assert.True(state.GetProperty("selfMember").GetProperty("isCurrentUser").GetBoolean());
        var members = state.GetProperty("members");
        Assert.Equal(3, members.GetArrayLength());

        // Enum qiymatlari mijoz kutgan satrlar.
        var statuses = members.EnumerateArray()
            .Select(m => m.GetProperty("status").GetString())
            .ToList();
        Assert.Contains("MOVING", statuses);
        Assert.Contains("NEEDS_ATTENTION", statuses);

        Assert.True(state.GetProperty("notifications").GetArrayLength() > 0);
        Assert.True(state.GetProperty("activityFeed").GetArrayLength() > 0);
    }

    [Fact]
    public async Task Full_auth_flow_request_verify_register()
    {
        var device = NewDevice();
        const string phone = "+998 90 123 45 67";

        var req = await PostAsync("api/auth/request-code/", new { deviceId = device, phone });
        Assert.True(req.GetProperty("success").GetBoolean());
        var reqState = req.GetProperty("state");
        Assert.True(reqState.GetProperty("otpRequested").GetBoolean());
        var code = reqState.GetProperty("otpHint").GetString()!;
        Assert.False(string.IsNullOrWhiteSpace(code));

        var verify = await PostAsync("api/auth/verify-code/", new { deviceId = device, phone, code });
        Assert.True(verify.GetProperty("success").GetBoolean());

        var register = await PostAsync("api/auth/register/",
            new { deviceId = device, fullName = "Nodir Karimov", phone });
        Assert.True(register.GetProperty("success").GetBoolean());
        var state = register.GetProperty("state");
        Assert.True(state.GetProperty("isRegistered").GetBoolean());
        Assert.True(state.GetProperty("isLoggedIn").GetBoolean());
        Assert.Equal("Nodir", state.GetProperty("caregiverName").GetString());
    }

    [Fact]
    public async Task Verify_with_wrong_code_fails()
    {
        var device = NewDevice();
        const string phone = "+998 90 000 11 22";
        await PostAsync("api/auth/request-code/", new { deviceId = device, phone });

        var verify = await PostAsync("api/auth/verify-code/",
            new { deviceId = device, phone, code = "0000-wrong" });
        Assert.False(verify.GetProperty("success").GetBoolean());
    }

    [Fact]
    public async Task Sos_trigger_then_clear()
    {
        var device = NewDevice();
        await PostAsync("api/bootstrap/", new { deviceId = device });

        var trigger = await PostAsync("api/sos/trigger/", new { deviceId = device });
        var sos = trigger.GetProperty("state").GetProperty("sosState");
        Assert.True(sos.GetProperty("isActive").GetBoolean());
        Assert.True(sos.GetProperty("contacts").GetArrayLength() > 0);
        Assert.Equal("NOTIFIED", sos.GetProperty("contacts")[0].GetProperty("state").GetString());

        var clear = await PostAsync("api/sos/clear/", new { deviceId = device });
        Assert.False(clear.GetProperty("state").GetProperty("sosState").GetProperty("isActive").GetBoolean());
    }

    [Fact]
    public async Task Invitation_send_and_accept_adds_member()
    {
        var device = NewDevice();
        var boot = await PostAsync("api/bootstrap/", new { deviceId = device });
        var initialCount = boot.GetProperty("state").GetProperty("members").GetArrayLength();

        var send = await PostAsync("api/invitations/send/",
            new { deviceId = device, name = "Sardor", relation = "Ukam", phone = "+998 90 555 44 33" });
        Assert.True(send.GetProperty("success").GetBoolean());

        // Yuborilgan taklif AppId sini topamiz (WAITING_INSTALL).
        var invitations = send.GetProperty("state").GetProperty("invitations");
        var sardor = invitations.EnumerateArray().First(i => i.GetProperty("name").GetString() == "Sardor");
        var inviteId = sardor.GetProperty("id").GetInt32();

        var accept = await PostAsync($"api/invitations/{inviteId}/accept/", new { deviceId = device });
        Assert.True(accept.GetProperty("success").GetBoolean());
        var newCount = accept.GetProperty("state").GetProperty("members").GetArrayLength();
        Assert.Equal(initialCount + 1, newCount);
    }

    [Fact]
    public async Task Notifications_mark_read_and_dismiss()
    {
        var device = NewDevice();
        var boot = await PostAsync("api/bootstrap/", new { deviceId = device });
        var first = boot.GetProperty("state").GetProperty("notifications")[0];
        var id = first.GetProperty("id").GetInt32();

        var read = await PostAsync($"api/notifications/{id}/read/", new { deviceId = device });
        var marked = read.GetProperty("state").GetProperty("notifications")
            .EnumerateArray().First(n => n.GetProperty("id").GetInt32() == id);
        Assert.True(marked.GetProperty("isRead").GetBoolean());

        var dismiss = await PostAsync($"api/notifications/{id}/dismiss/", new { deviceId = device });
        var remaining = dismiss.GetProperty("state").GetProperty("notifications")
            .EnumerateArray().Any(n => n.GetProperty("id").GetInt32() == id);
        Assert.False(remaining);
    }

    [Fact]
    public async Task Health_endpoint_is_ok()
    {
        var res = await _client.GetAsync("/health");
        Assert.Equal(HttpStatusCode.OK, res.StatusCode);
    }
}
