using System.Text.Json.Serialization;

namespace RR.Api.Auth;

/// <summary>
/// Granular permissions that control access to specific application features.
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
