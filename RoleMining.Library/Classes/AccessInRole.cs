namespace RoleMining.Library.Classes
{
    /// <summary>
    /// Class that represents the relationship between a role and its standard access.
    /// </summary>
    public class AccessInRole
    {
        /// <summary>
        /// The role ID, unique identifier for each role.
        /// </summary>
        public string RoleID { get; set; }

        /// <summary>
        /// The access level, something that is unique to each access and access level. Example: access: "Github", access level: "Admin". The access parameter would then be "Github - Admin".
        /// </summary>
        public string AccessID { get; set; }


        // FUNCTIONS USED BY HASHSET TO COMPARE OBJECTS

        /// <summary>
        /// Returns the hash code for the UserAccess object, combination of UserID, RoleID and AccessID.
        /// </summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
        public override int GetHashCode()
        {
            return $"{RoleID} - {AccessID}".GetHashCode();
        }

        /// <summary>
        /// Function to check if this UserAccess object is equal to another object.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj is AccessInRole other)
            {
                return RoleID == other.RoleID && AccessID == other.AccessID;
            }
            return false;
        }
    }
}
