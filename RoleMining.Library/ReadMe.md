# RoleMining.Library

This is a work-in-progress C# library designed for mining roles from a Role-Based Access Control (RBAC) system. 
The library aims to provide various algorithms that help reduce the number of extra accesses granted to users. It achieves this by:

- **Standard Accesses:** Suggestions for accesses that can be made standard within a given role, thereby reducing the number of extra accesses assigned to users.

- **Role Creation:** Recommendations for new roles to be created based on user access patterns, enhancing the overall efficiency of the access control system.

As the library is still under development, additional features and enhancements will be added over time.

## Features (For now)
- **JaccardIndices** method that takes a list of UserAccess and UserInRole objects as input and outputs a list of JaccardIndex objects. This is useful for getting an overview of which extra accesses fit each role.
- **UserAccess** and **UserInRole** classes for input.

## Syntax
**JaccardIndices**
```csharp
	RoleMining.JaccardIndices(IEnumerable<UserAccess> userAccesses, IEnumerable<UserInRole> userInRoles)
	// Return type is Jaccard
```

Classes
```csharp
	public class UserAccess
	{
		public string UserID { get; set; }
		public string AccessLevelID { get; set; }
		public string AccessID { get; set; }
	}

	public class UserInRole
	{
		public string RoleID { get; set; }
		public string UserID { get; set; }
	}

	public class JaccardIndex
	{
		public double JaccardIndex { get; set; }
		public string RoleID { get; set; }
		public string AccessID { get; set; }
		public int UsersWithAccess { get; set; }
		public int UsersWithoutAccess { get; set; }
	}
```