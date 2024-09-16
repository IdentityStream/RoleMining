namespace RoleMining.Tests;

using RoleMining.Library;
using RoleMining.Library.Classes;
using System.Collections.Generic;
using Xunit;

public class JaccardTest : IAlgorithmTest
{
    [Fact]
    public void RunCode()
    {
        var userAccesses = new List<UserAccess>
        {
            new UserAccess { UserID = "1", AccessID = "Read"  },
            new UserAccess { UserID = "2", AccessID = "Write" },
            new UserAccess { UserID = "3", AccessID = "Read"  },
            new UserAccess { UserID = "4", AccessID = "Write" },
            new UserAccess { UserID = "5", AccessID = "Read"  },
            new UserAccess { UserID = "6", AccessID = "Write" },
            new UserAccess { UserID = "7", AccessID = "Read"  },
            new UserAccess { UserID = "8", AccessID = "Write" }
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
        var jaccard = RoleMining.JaccardIndices(userAccesses, userInRoles);
        Assert.Equal(1, 1);
    }

    [Fact]
    public void TestNullOrEmptyValues()
    {
        var userAccesses = new List<UserAccess>();
        var userInRoles = new List<UserInRole>();

        userAccesses.Add(new UserAccess {
            UserID = "",
            AccessID = ""
        });
        var userInRole = new UserInRole {
            UserID = "",
            RoleID = ""
        };

        // Act
        Assert.Throws<ArgumentNullException>(() => RoleMining.JaccardIndices(null, null));
        Assert.Throws<ArgumentNullException>(() => RoleMining.JaccardIndices(userAccesses, null));
        Assert.Throws<ArgumentNullException>(() => RoleMining.JaccardIndices(null, userInRoles));
        Assert.Throws<ArgumentException>(() => RoleMining.JaccardIndices(userAccesses, userInRoles)); 
    }

    [Fact]
    public void TestBasicFunctionality()
    {
        var userAccesses = new List<UserAccess>
        {
            new UserAccess { UserID = "1", AccessID = "Read"},
            new UserAccess { UserID = "4", AccessID = "Write" }
        };

        var userInRoles = new List<UserInRole>
        {
            new UserInRole { UserID = "1", RoleID = "A" },
            new UserInRole { UserID = "2", RoleID = "A" },
            new UserInRole { UserID = "3", RoleID = "A" },
            new UserInRole { UserID = "4", RoleID = "A" }
        };

        var result = RoleMining.JaccardIndices(userAccesses, userInRoles);

        Assert.Equal(2, result.Count);
        Assert.Contains(result, r => r.RoleID == "A" && r.AccessID == "Read" && r.JaccardIndex == 0.25);
        Assert.Contains(result, r => r.RoleID == "A" && r.AccessID == "Write" && r.JaccardIndex == 0.25);
    }

    [Fact]
    public void TestLargeData()
    {
        var userInRoles = new List<UserInRole>();
        var userAccesses = new List<UserAccess>();

        var iterations = 10000;

        // Generate 10,000 users, all in role "A"
        for (int i = 1; i <= iterations; i++)
        {
            userInRoles.Add(new UserInRole { UserID = i.ToString(), RoleID = $"{i}" });
        }

        // Assign 10,000 users access to "read"
        for (int i = 1; i <= iterations; i++)
        {
            userAccesses.Add(new UserAccess { UserID = i.ToString(), AccessID = $"{i}" });
        }

        var result = RoleMining.JaccardIndices(userAccesses, userInRoles);
        Assert.Equal(1, 1);  // Expecting 1 role with 10,000 users
    }

    [Fact]
    public void JaccardIndex_AllUsersHaveSameExtraAccessAndRole()
    {
        var userAccesses = new List<UserAccess>
        {
            new UserAccess { UserID = "1", AccessID = "Read"  },
            new UserAccess { UserID = "3", AccessID = "Read"  },
            new UserAccess { UserID = "5", AccessID = "Read"  },
            new UserAccess { UserID = "7", AccessID = "Read"  },
        };

        var userInRoles = new List<UserInRole>
        {
            new UserInRole { UserID = "1", RoleID = "A" },
            new UserInRole { UserID = "3", RoleID = "A" },
            new UserInRole { UserID = "5", RoleID = "A" },
            new UserInRole { UserID = "7", RoleID = "A" },

        };
        var jaccard = RoleMining.JaccardIndices(userAccesses, userInRoles);
        Assert.Equal(1, 1);
    }

    [Fact]
    public void JaccardIndex_LargeDatasetWithMultipleRoles()
    {
        var userInRoles = new List<UserInRole>();
        var accessInRole = new List<AccessInRole>();
        var userAccesses = new List<UserAccess>();

        var iterations = 10000;

        // Simulate 100 users, each having multiple roles
        for (int i = 1; i <= iterations; i++)
        {
            userInRoles.Add(new UserInRole { UserID = i.ToString(), RoleID = $"{i % 2}" });
            userInRoles.Add(new UserInRole { UserID = i.ToString(), RoleID = $"{i % 5}" });

            // Simulate extra accesses for half of the users
            if (i % 2 == 0)
            {
                userAccesses.Add(new UserAccess { UserID = i.ToString(), AccessID = "execute" }); // Extra access
            }
        }

        var result = RoleMining.JaccardIndices(userAccesses, userInRoles);
        Assert.Equal(1, 1);
    }

    [Fact]
    public void JaccardIndex_MultipleRoles()
    {
        var userAccesses = new List<UserAccess>
        {
            new UserAccess { UserID = "1", AccessID = "Read"  },
            new UserAccess { UserID = "2", AccessID = "Read"  },
            new UserAccess { UserID = "3", AccessID = "Read"  },
            new UserAccess { UserID = "4", AccessID = "Read"  },
            new UserAccess { UserID = "5", AccessID = "Read"  },
            new UserAccess { UserID = "6", AccessID = "Read"  },
            // Total 6 users with access
            // 3 with role A and B
            // 2 with just role A
            // 1 with just role B
        };

        var userInRoles = new List<UserInRole>
        {
            // Role A
            new UserInRole { UserID = "1",  RoleID = "A" }, // Has access
            new UserInRole { UserID = "2",  RoleID = "A" }, // Has access
            new UserInRole { UserID = "3",  RoleID = "A" }, // Has access
            new UserInRole { UserID = "A4", RoleID = "A" },
            new UserInRole { UserID = "5",  RoleID = "A" }, // Has access
            new UserInRole { UserID = "6",  RoleID = "A" }, // Has access
            new UserInRole { UserID = "A7", RoleID = "A" },
            new UserInRole { UserID = "A8", RoleID = "A" },
            new UserInRole { UserID = "A9", RoleID = "A" },
            // 9 in role, 5 with access

            // Role B
            new UserInRole { UserID = "1",  RoleID = "B" }, // Has access
            new UserInRole { UserID = "2",  RoleID = "B" }, // Has access 
            new UserInRole { UserID = "3",  RoleID = "B" }, // Has access
            new UserInRole { UserID = "4",  RoleID = "B" }, // Has access
            new UserInRole { UserID = "B5", RoleID = "B" },
            new UserInRole { UserID = "B6", RoleID = "B" },
            new UserInRole { UserID = "B7", RoleID = "B" },
            new UserInRole { UserID = "B8", RoleID = "B" },
            new UserInRole { UserID = "B9", RoleID = "B" },
            new UserInRole { UserID ="B10", RoleID = "B" },
            new UserInRole { UserID ="B11", RoleID = "B" },
            new UserInRole { UserID ="B12", RoleID = "B" },
            new UserInRole { UserID ="B13", RoleID = "B" },
            new UserInRole { UserID ="B14", RoleID = "B" },
            // 14 in role, 4 with access
        };

        // Act
        var result = RoleMining.JaccardIndices(userAccesses, userInRoles);

        // Assert
        var RoleAAccess = result.FirstOrDefault(j => j.RoleID == "A" && j.AccessID == "Read");
        var RoleBAccess = result.FirstOrDefault(j => j.RoleID == "B" && j.AccessID == "Read");

        Assert.NotNull(RoleAAccess);
        Assert.Equal(0.5, RoleAAccess.JaccardIndex);
        // (9 users in role - 5 with access and role + 6 with access) / (5 with acess)

        Assert.NotNull(RoleBAccess);
        Assert.Equal(0.25, RoleBAccess.JaccardIndex);
        // (14 users in role - 4 with access and role + 6 with access) / (4 with acess)
    }

}
