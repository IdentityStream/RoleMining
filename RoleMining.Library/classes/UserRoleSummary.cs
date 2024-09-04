namespace RoleMining.Library
{
    /// <summary>
    /// Object used to store how many users have a specific role
    /// </summary>
    internal class UserRoleSummary
    {
        /// <summary>
        /// The role ID, unique identifier for each role
        /// </summary>
        public string RoleID { get; set; }
        /// <summary>
        /// How many users have the specific role
        /// </summary>
        public int TotalUsers { get; set; }
    }
}