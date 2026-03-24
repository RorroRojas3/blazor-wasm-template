using System.Text.Json.Serialization;

namespace RR.Common.Auth;

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
