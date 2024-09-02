namespace RoleMining
{
    using System;
    public class ExtraService
    {
        public string Role { get; set; }
        public string Service { get; set; }
        public string AccessLevel { get; set; }
        public int TotalExtraAccess { get; set; }
        public int TotalInRole { get; set; }
        public double AccessRatio
        {
            get
            {
                return TotalInRole == 0 ? 0 : Math.Round((double)TotalExtraAccess / TotalInRole, 2);
            }
        }
    }
}
