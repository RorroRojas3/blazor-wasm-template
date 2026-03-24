using System.Text.Json.Serialization;

namespace RR.Common.Auth;

/// <summary>
/// High-level roles assigned to users, typically mapped to sets of permissions.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum AppRole
{
    Admin,
    Manager,
    Editor,
    Viewer
}
