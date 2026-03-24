using System.Text.Json.Serialization;

namespace RR.UI.Auth;

/// <summary>
/// Granular permissions that control access to specific application features.
/// Must match the backend <c>RR.Api.Auth.AppPermission</c> enum by name.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum AppPermission
{
    UsersRead,
    UsersWrite,
    UsersDelete,
    ReportsView,
    ReportsExport,
    DashboardView,
    SettingsManage
}

/// <summary>
/// High-level roles assigned to users.
/// Must match the backend <c>RR.Api.Auth.AppRole</c> enum by name.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum AppRole
{
    Admin,
    Manager,
    Editor,
    Viewer
}
