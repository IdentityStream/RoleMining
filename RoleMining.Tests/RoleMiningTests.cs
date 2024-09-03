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
        new UserAccess { UserID = "1", RoleID = "A", AccessID = "Read", IsExtraAccess = true },
        new UserAccess { UserID = "2", RoleID = "A", AccessID = "Write", IsExtraAccess = false },
        new UserAccess { UserID = "3", RoleID = "A", AccessID = "Read", IsExtraAccess = false },
        new UserAccess { UserID = "4", RoleID = "A", AccessID = "Write", IsExtraAccess = true }
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
        Assert.Contains(result, r => r.RoleID == "A" && r.AccessID == "Read" && r.Ratio == 0.25);
        Assert.Contains(result, r => r.RoleID == "A" && r.AccessID == "Write" && r.Ratio == 0.25);
    }

    [Fact]
    public void TestDuplicateRemoval()
    {
        var userAccesses = new List<UserAccess>
        {
            new UserAccess { UserID = "1", RoleID = "Admin", AccessID = "Read", IsExtraAccess = true },
            new UserAccess { UserID = "1", RoleID = "Admin", AccessID = "Write", IsExtraAccess = false },
            new UserAccess { UserID = "2", RoleID = "Admin", AccessID = "Read", IsExtraAccess = true },
            new UserAccess { UserID = "2", RoleID = "User", AccessID = "Read", IsExtraAccess = false },
            new UserAccess { UserID = "3", RoleID = "User", AccessID = "Read", IsExtraAccess = false },
            new UserAccess { UserID = "3", RoleID = "User", AccessID = "Write", IsExtraAccess = true },
            new UserAccess { UserID = "4", RoleID = "User", AccessID = "Write", IsExtraAccess = true },
            new UserAccess { UserID = "4", RoleID = "Admin", AccessID = "Read", IsExtraAccess = true },
            new UserAccess { UserID = "4", RoleID = "Admin", AccessID = "Read", IsExtraAccess = true }, // Duplicate
            new UserAccess { UserID = "5", RoleID = "Admin", AccessID = "Write", IsExtraAccess = false },
            new UserAccess { UserID = "6", RoleID = "User", AccessID = "Read", IsExtraAccess = false }
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
        Assert.Contains(result, r => r.RoleID == "Admin" && r.AccessID == "Read" && r.Ratio == 0.75);
        Assert.Contains(result, r => r.RoleID == "User" && r.AccessID == "Write" && r.Ratio == 0.5);
    }

    [Fact]
    public void TestNoSpecialAccesses()
    {
        var userAccesses = new List<UserAccess>
        {
            new UserAccess { UserID = "1", RoleID = "A", AccessID = "Read", IsExtraAccess = false },
            new UserAccess { UserID = "2", RoleID = "A", AccessID = "Write", IsExtraAccess = false },
            new UserAccess { UserID = "3", RoleID = "B", AccessID = "Read", IsExtraAccess = false },
            new UserAccess { UserID = "4", RoleID = "B", AccessID = "Write", IsExtraAccess = false },
            new UserAccess { UserID = "5", RoleID = "C", AccessID = "Read", IsExtraAccess = false },
            new UserAccess { UserID = "6", RoleID = "C", AccessID = "Write", IsExtraAccess = false },
            new UserAccess { UserID = "7", RoleID = "D", AccessID = "Read", IsExtraAccess = false },
            new UserAccess { UserID = "8", RoleID = "D", AccessID = "Write", IsExtraAccess = false }
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
        new UserAccess { UserID = "1", RoleID = "A", AccessID = "Read", IsExtraAccess = true },
        new UserAccess { UserID = "2", RoleID = "A", AccessID = "Write", IsExtraAccess = false },
        new UserAccess { UserID = "3", RoleID = "B", AccessID = "Read", IsExtraAccess = true },
        new UserAccess { UserID = "4", RoleID = "B", AccessID = "Write", IsExtraAccess = false },
        new UserAccess { UserID = "5", RoleID = "C", AccessID = "Read", IsExtraAccess = true },
        new UserAccess { UserID = "6", RoleID = "C", AccessID = "Write", IsExtraAccess = false },
        new UserAccess { UserID = "7", RoleID = "D", AccessID = "Read", IsExtraAccess = true },
        new UserAccess { UserID = "8", RoleID = "D", AccessID = "Write", IsExtraAccess = false }
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
        Assert.Contains(result, r => r.RoleID == "A" && r.AccessID == "Read" && r.Ratio == 0.5);
        Assert.Contains(result, r => r.RoleID == "B" && r.AccessID == "Read" && r.Ratio == 0.5);
        Assert.Contains(result, r => r.RoleID == "C" && r.AccessID == "Read" && r.Ratio == 0.5);
        Assert.Contains(result, r => r.RoleID == "D" && r.AccessID == "Read" && r.Ratio == 0.5);
    }
}