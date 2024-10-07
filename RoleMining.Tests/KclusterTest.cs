using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;
using RoleMining.Library;
using RoleMining.Library.Classes;
using RoleMining.Library.Algorithms;
using RoleMining.Library.Clustering;

namespace RoleMining.Tests;

public class KclusterTest
{
    [Fact]
    public void Should_not_throw_when_inputs_are_valid()
    {
        var userAccess = new List<UserAccess>
        {
            new UserAccess { UserID = "2", AccessID = "Write" },
            new UserAccess { UserID = "1", AccessID = "Read"  },
            new UserAccess { UserID = "1", AccessID = "Write" },
            new UserAccess { UserID = "3", AccessID = "Read"  },
            new UserAccess { UserID = "4", AccessID = "Write" },
            new UserAccess { UserID = "5", AccessID = "Read"  },
            new UserAccess { UserID = "6", AccessID = "Write" },
            new UserAccess { UserID = "7", AccessID = "Read"  },
            new UserAccess { UserID = "8", AccessID = "Write" }
        };

        var act = () => Kcluster.Cluster(userAccess);
        act.Should().NotThrow();
    }

    [Fact]
    public void TestNullOrEmptyValues()
    {
        throw new NotImplementedException();
    }

    [Fact]
    public void TestBasicFunctionality()
    {
        throw new NotImplementedException();
    }

    [Fact]
    public void Should_return_single_cluster_when_all_users_have_the_same_accesses()
    {
        var userAccesses = new List<UserAccess>();

        var iterations = 1000;  

        // Assign users access to "read"
        for (int i = 1; i <= iterations; i++)
        {
            userAccesses.Add(new UserAccess { UserID = i.ToString(), AccessID = $"Read" });
        }

        var result = Kcluster.Cluster(userAccesses);
        result.Should().ContainSingle();
    }
}
