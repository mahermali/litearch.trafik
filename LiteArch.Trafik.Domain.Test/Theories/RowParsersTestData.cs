using System.Collections;
using System.Collections.Generic;

namespace LiteArch.Trafik.Domain.Test.Theories
{
    public class RowParsersTestData: IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[]
            {
                "$:C3:172.17.0.3",
                new SamplerIp
                {
                    Ip = "172.17.0.3",
                    HostName = "C3"
                }
            };
            yield return new object[]
            {
                "$:c3.domain:172.17.0.3",
                new SamplerIp
                {
                    Ip = "172.17.0.3",
                    HostName = "c3.domain"
                }
            };
            yield return new object[]
            {
                ":c3.domain:172.17.0.3",
                null
            };
            yield return new object[]
            {
                "$:c3.domain:",
                null
            };
            yield return new object[]
            {
                "$:c3.domain:::",
                null
            };
            yield return new object[]
            {
                "#10.144.152.216.6379$10.144.152.215.41898",
                new SamplerLink
                {
                    SourceAddress = "10.144.152.216",
                    SourcePort = "6379",
                    TargetAddress = "10.144.152.215",
                    TargetPort = "41898"
                }
            };
            yield return new object[]
            {
                "#c1.domain.ca.6379$10.144.152.215.41898",
                new SamplerLink
                {
                    SourceAddress = "c1.domain.ca",
                    SourcePort = "6379",
                    TargetAddress = "10.144.152.215",
                    TargetPort = "41898"
                }
            };
            yield return new object[]
            {
                "#c1.domain.ca.6379$.41898",
                null
            };
            yield return new object[]
            {
                "c1.domain.ca.6379$10.144.152.215.41898",
               null
            };
            yield return new object[]
            {
                "#6379$10.144.152.215.41898",
               null
            };
            yield return new object[]
            {
                $"{Constants.RedisKeyPrefix}#10.144.152.216.6379$10.144.152.215.41898",
                new SamplerLink
                {
                    SourceAddress = "10.144.152.216",
                    SourcePort = "6379",
                    TargetAddress = "10.144.152.215",
                    TargetPort = "41898"
                }
            };
            yield return new object[]
            {
                $"{Constants.RedisKeyPrefix}#c1.domain.ca.6379$10.144.152.215.41898",
                new SamplerLink
                {
                    SourceAddress = "c1.domain.ca",
                    SourcePort = "6379",
                    TargetAddress = "10.144.152.215",
                    TargetPort = "41898"
                }
            };
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}