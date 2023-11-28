using System.Collections.Generic;

public class UnitTest1
{
    [Fact]
    public void StringTEST()
    {
        string actual = "ABCDEFGHIA";
        actual.Should().StartWith("AB").And.EndWith("HIA").And.Contain("EF").And.HaveLength(10);
    }

    [Fact]
    public void TestArray()
    {
        IEnumerable<int> numbers = new[] { 1, 2, 3 };

        numbers.Should().OnlyContain(n => n > 0);
        numbers.Should().HaveCount(3, "because we thought we put four items in the collection");
    }
}