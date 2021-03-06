using System;
using LiteArch.Trafik.Domain.Test.Theories;
using Xunit;

namespace LiteArch.Trafik.Domain.Test
{
    public class RowParsersTest
    {
        [Theory]
        [ClassData(typeof(RowParsersTestData))]
        public void Should_Parse(string row, SamplerRow expected)
        {
            var actual=RowParsers.Parse(row);
            Assert.Equal(expected,actual);
        }
    }
}