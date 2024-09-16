namespace RoleMining.Library.Classes
{
    /// <summary>
    /// Object used to store the ratio of extra accesses by each role compared to how many there are in that role.
    /// 
    /// The type in the list returned from <seealso cref="RoleMining.SummarizeRoles">RoleMining.MineRoles</seealso> method
    /// </summary>
    public class AccessInRoleSummarized
    {
        /// <summary>
        /// The role ID, unique identifier for each role
        /// </summary>
        public string RoleID { get; set; }

        /// <summary>
        /// The access level, something that is unique to each access and access level. 
        /// <br/><br/>
        /// Example: access: "Github", access level: "Admin". The access parameter would then be "Github - Admin"
        /// </summary>
        public string AccessID { get; set; }

        /// <summary>
        /// All users with specific role and extra access divided by the count of users with that role
        /// </summary>
        public double Ratio { get; set; }

        /// <summary>
        /// How many users have the specific access level as extra access, whilst also having the specific role
        /// </summary>
        public int UsersWithAccessAsExtra { get; set; } // Maybe return the names of the people with this as extra, maybe in a new class? Who doesnt have the access?

        /// <summary>
        /// Total amount of users with the specific role
        /// </summary>
        public int TotalUsers { get; set; }
    }
}
