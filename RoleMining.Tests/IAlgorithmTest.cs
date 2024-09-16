namespace RoleMining.Tests
{
    public interface IAlgorithmTest
    {
        void RunCode();
        void TestNullOrEmptyValues();
        void TestBasicFunctionality();
        void TestLargeData();
    }
}
