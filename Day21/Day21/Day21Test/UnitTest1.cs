using System.Collections.Generic;
using Xunit;

namespace Day21Test
{
    public class UnitTest1
    {
        [Theory]
        [MemberData(nameof(Part1sample))]
        [MemberData(nameof(Part1))]
        public void Part1Test(string file_name, int answer)
        {
            long result = Day21.Program.part1(file_name);
            Assert.Equal(answer, result);
        }

        [Theory]
        [MemberData(nameof(Part2sample))]
        [MemberData(nameof(Part2))]
        public void Part2Test(string file_name, string answer)
        {
            string result = Day21.Program.part2(file_name);
            Assert.Equal(answer, result);
        }

        public static IEnumerable<object[]> TestSetup(string file_name, long answer)
        {
            return new[]
            {
                new object[] {file_name, answer}
            };
        }

        public static IEnumerable<object[]> Test2Setup(string file_name, string answer)
        {
            return new[]
            {
                new object[] {file_name, answer}
            };
        }

        public static IEnumerable<object[]> Part1sample => TestSetup("sample_input.txt", 5);

        public static IEnumerable<object[]> Part1 => TestSetup("input.txt", 1685);

        public static IEnumerable<object[]> Part2sample => Test2Setup("sample_input.txt", "mxmxvkd,sqjhc,fvjkl");

        public static IEnumerable<object[]> Part2 => Test2Setup("input.txt", "ntft,nhx,kfxr,xmhsbd,rrjb,xzhxj,chbtp,cqvc");
    }
}