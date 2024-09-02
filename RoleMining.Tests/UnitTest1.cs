namespace RoleMining.Tests;

using RoleMining.Library;
using System.Collections.Generic;
using Xunit;

public class RoleMiningTests
{
    [Fact]
    public void TestBasicFunctionality()
    {
        var userAccesses = new List<UserAccess>
    {
        new UserAccess { UserID = "1", RoleID = "A", AccessLevelID = "Read", IsExtraAccess = true },
        new UserAccess { UserID = "2", RoleID = "A", AccessLevelID = "Write", IsExtraAccess = false },
        new UserAccess { UserID = "3", RoleID = "A", AccessLevelID = "Read", IsExtraAccess = false },
        new UserAccess { UserID = "4", RoleID = "A", AccessLevelID = "Write", IsExtraAccess = true }
    };

        var userInRoles = new List<UserInRole>
    {
        new UserInRole { UserID = "1", RoleID = "A" },
        new UserInRole { UserID = "2", RoleID = "A" },
        new UserInRole { UserID = "3", RoleID = "A" },
        new UserInRole { UserID = "4", RoleID = "A" }
    };

        var result = RoleMining.MineRoles(userAccesses, userInRoles);

        Assert.Equal(2, result.Count);
        Assert.Contains(result, r => r.Role == "A" && r.Access == "Read" && r.Ratio == 0.25);
        Assert.Contains(result, r => r.Role == "A" && r.Access == "Write" && r.Ratio == 0.25);
    }

    [Fact]
    public void TestDuplicateRemoval()
    {
        var userAccesses = new List<UserAccess>
        {
            new UserAccess { UserID = "1", RoleID = "Admin", AccessLevelID = "Read", IsExtraAccess = true },
            new UserAccess { UserID = "1", RoleID = "Admin", AccessLevelID = "Write", IsExtraAccess = false },
            new UserAccess { UserID = "2", RoleID = "Admin", AccessLevelID = "Read", IsExtraAccess = true },
            new UserAccess { UserID = "2", RoleID = "User", AccessLevelID = "Read", IsExtraAccess = false },
            new UserAccess { UserID = "3", RoleID = "User", AccessLevelID = "Read", IsExtraAccess = false },
            new UserAccess { UserID = "3", RoleID = "User", AccessLevelID = "Write", IsExtraAccess = true },
            new UserAccess { UserID = "4", RoleID = "User", AccessLevelID = "Write", IsExtraAccess = true },
            new UserAccess { UserID = "4", RoleID = "Admin", AccessLevelID = "Read", IsExtraAccess = true },
            new UserAccess { UserID = "4", RoleID = "Admin", AccessLevelID = "Read", IsExtraAccess = true }, // Duplicate
            new UserAccess { UserID = "5", RoleID = "Admin", AccessLevelID = "Write", IsExtraAccess = false },
            new UserAccess { UserID = "6", RoleID = "User", AccessLevelID = "Read", IsExtraAccess = false }
        };

        var userInRoles = new List<UserInRole>
        {
            new UserInRole { UserID = "1", RoleID = "Admin" },
            new UserInRole { UserID = "2", RoleID = "Admin" },
            new UserInRole { UserID = "2", RoleID = "User" },
            new UserInRole { UserID = "3", RoleID = "User" },
            new UserInRole { UserID = "4", RoleID = "User" },
            new UserInRole { UserID = "4", RoleID = "Admin" },
            new UserInRole { UserID = "5", RoleID = "Admin" },
            new UserInRole { UserID = "6", RoleID = "User" }
        };

        var result = RoleMining.MineRoles(userAccesses, userInRoles);
        // We want Role A with Read and Write
        // We want Role B with Read and Write

        // Assert
        Assert.NotNull(result);
        Assert.Contains(result, r => r.Role == "Admin" && r.Access == "Read" && r.Ratio == 0.75);
        Assert.Contains(result, r => r.Role == "User" && r.Access == "Write" && r.Ratio == 0.5);
    }

    [Fact]
    public void TestNoSpecialAccesses()
    {
        var userAccesses = new List<UserAccess>
        {
            new UserAccess { UserID = "1", RoleID = "A", AccessLevelID = "Read", IsExtraAccess = false },
            new UserAccess { UserID = "2", RoleID = "A", AccessLevelID = "Write", IsExtraAccess = false },
            new UserAccess { UserID = "3", RoleID = "B", AccessLevelID = "Read", IsExtraAccess = false },
            new UserAccess { UserID = "4", RoleID = "B", AccessLevelID = "Write", IsExtraAccess = false },
            new UserAccess { UserID = "5", RoleID = "C", AccessLevelID = "Read", IsExtraAccess = false },
            new UserAccess { UserID = "6", RoleID = "C", AccessLevelID = "Write", IsExtraAccess = false },
            new UserAccess { UserID = "7", RoleID = "D", AccessLevelID = "Read", IsExtraAccess = false },
            new UserAccess { UserID = "8", RoleID = "D", AccessLevelID = "Write", IsExtraAccess = false }
        };

        var userInRoles = new List<UserInRole>
        {
            new UserInRole { UserID = "1", RoleID = "A" },
            new UserInRole { UserID = "2", RoleID = "A" },
            new UserInRole { UserID = "3", RoleID = "B" },
            new UserInRole { UserID = "4", RoleID = "B" },
            new UserInRole { UserID = "5", RoleID = "C" },
            new UserInRole { UserID = "6", RoleID = "C" },
            new UserInRole { UserID = "7", RoleID = "D" },
            new UserInRole { UserID = "8", RoleID = "D" }
        };

        var result = RoleMining.MineRoles(userAccesses, userInRoles);

        Assert.Empty(result);
    }

    [Fact]
    public void TestEmptyInputs()
    {
        var userAccesses = new List<UserAccess>();
        var userInRoles = new List<UserInRole>();

        var exception = Assert.Throws<ArgumentException>(() => RoleMining.MineRoles(userAccesses, userInRoles));
        Assert.Equal("userAccesses", exception.ParamName); ;
    }

    [Fact]
    public void TestNullInputs()
    {
        var exception = Assert.Throws<ArgumentNullException>(() => RoleMining.MineRoles(null, null));
        Assert.Equal("userAccesses", exception.ParamName);
    }

    [Fact]
    public void TestMultipleRoles()
    {
        var userAccesses = new List<UserAccess>
    {
        new UserAccess { UserID = "1", RoleID = "A", AccessLevelID = "Read", IsExtraAccess = true },
        new UserAccess { UserID = "2", RoleID = "A", AccessLevelID = "Write", IsExtraAccess = false },
        new UserAccess { UserID = "3", RoleID = "B", AccessLevelID = "Read", IsExtraAccess = true },
        new UserAccess { UserID = "4", RoleID = "B", AccessLevelID = "Write", IsExtraAccess = false },
        new UserAccess { UserID = "5", RoleID = "C", AccessLevelID = "Read", IsExtraAccess = true },
        new UserAccess { UserID = "6", RoleID = "C", AccessLevelID = "Write", IsExtraAccess = false },
        new UserAccess { UserID = "7", RoleID = "D", AccessLevelID = "Read", IsExtraAccess = true },
        new UserAccess { UserID = "8", RoleID = "D", AccessLevelID = "Write", IsExtraAccess = false }
    };

        var userInRoles = new List<UserInRole>
    {
        new UserInRole { UserID = "1", RoleID = "A" },
        new UserInRole { UserID = "2", RoleID = "A" },
        new UserInRole { UserID = "3", RoleID = "B" },
        new UserInRole { UserID = "4", RoleID = "B" },
        new UserInRole { UserID = "5", RoleID = "C" },
        new UserInRole { UserID = "6", RoleID = "C" },
        new UserInRole { UserID = "7", RoleID = "D" },
        new UserInRole { UserID = "8", RoleID = "D" }
    };

        var result = RoleMining.MineRoles(userAccesses, userInRoles);

        Assert.Equal(4, result.Count);
        Assert.Contains(result, r => r.Role == "A" && r.Access == "Read" && r.Ratio == 0.5);
        Assert.Contains(result, r => r.Role == "B" && r.Access == "Read" && r.Ratio == 0.5);
        Assert.Contains(result, r => r.Role == "C" && r.Access == "Read" && r.Ratio == 0.5);
        Assert.Contains(result, r => r.Role == "D" && r.Access == "Read" && r.Ratio == 0.5);
    }
}