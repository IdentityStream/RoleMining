namespace RoleMining
{
    public class RatioAccessLevel
    {
        public string Role { get; set; }
        public string Access { get; set; }
        public double Ratio { get; set; }
        public int UsersWithAccessAsExtra { get; set; }
        public int TotalUsers { get; set; }
    }
}
