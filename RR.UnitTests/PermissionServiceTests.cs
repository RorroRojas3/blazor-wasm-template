using Microsoft.Extensions.Logging;
using RR.Common.Auth;
using RR.Service;

namespace RR.UnitTests;

/// <summary>
/// Unit tests for <see cref="PermissionService"/>.
/// </summary>
public sealed class PermissionServiceTests
{
    private readonly PermissionService _sut;

    public PermissionServiceTests()
    {
        var logger = Substitute.For<ILogger<PermissionService>>();
        _sut = new PermissionService(logger);
    }

    [Fact]
    public async Task LoadAllAsync_PopulatesCache()
    {
        await _sut.LoadAllAsync();

        var data = _sut.GetPermissions("00000000-0000-0000-0000-000000000001");

        Assert.NotNull(data);
        Assert.Contains(AppRole.Admin, data.Roles);
    }

    [Fact]
    public async Task GetPermissions_ReturnsCorrectRolesAndPermissions()
    {
        await _sut.LoadAllAsync();

        var data = _sut.GetPermissions("00000000-0000-0000-0000-000000000002");

        Assert.NotNull(data);
        Assert.Contains(AppRole.Manager, data.Roles);
        Assert.Contains(AppRole.Editor, data.Roles);
        Assert.Contains(AppPermission.UsersRead, data.Permissions);
        Assert.Contains(AppPermission.DashboardView, data.Permissions);
        Assert.DoesNotContain(AppPermission.UsersDelete, data.Permissions);
    }

    [Fact]
    public async Task GetPermissions_UnknownUser_ReturnsNull()
    {
        await _sut.LoadAllAsync();

        var data = _sut.GetPermissions("unknown-user-id");

        Assert.Null(data);
    }

    [Fact]
    public async Task InvalidateUser_RemovesFromCache()
    {
        await _sut.LoadAllAsync();

        _sut.InvalidateUser("00000000-0000-0000-0000-000000000001");

        var data = _sut.GetPermissions("00000000-0000-0000-0000-000000000001");
        Assert.Null(data);
    }

    [Fact]
    public async Task InvalidateAll_ClearsEntireCache()
    {
        await _sut.LoadAllAsync();

        _sut.InvalidateAll();

        Assert.Null(_sut.GetPermissions("00000000-0000-0000-0000-000000000001"));
        Assert.Null(_sut.GetPermissions("00000000-0000-0000-0000-000000000002"));
        Assert.Null(_sut.GetPermissions("00000000-0000-0000-0000-000000000003"));
    }

    [Fact]
    public async Task ReloadAllAsync_RefreshesCache()
    {
        await _sut.LoadAllAsync();
        _sut.InvalidateUser("00000000-0000-0000-0000-000000000001");

        await _sut.ReloadAllAsync();

        var data = _sut.GetPermissions("00000000-0000-0000-0000-000000000001");
        Assert.NotNull(data);
    }

    [Fact]
    public async Task GetPermissions_IsCaseInsensitive()
    {
        await _sut.LoadAllAsync();

        var lower = _sut.GetPermissions("00000000-0000-0000-0000-000000000001");
        var upper = _sut.GetPermissions("00000000-0000-0000-0000-000000000001".ToUpperInvariant());

        Assert.NotNull(lower);
        Assert.NotNull(upper);
    }
}
