using System.Collections;
using System.Collections.Generic;

namespace LiteArch.Trafik.Domain.Test.Theories
{
    public class RowParsersTestData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[]
            {
                "$10.144.152.123:172.17.0.6:",
                new SamplerDockerMetadata
                {
                    Task = null,
                    HostIp = "10.144.152.123",
                    DockerIp = "172.17.0.6"
                }
            };
            
            yield return new object[]
            {
                "$10.144.152.123:172.17.0.6:NOMAD_TASK_NAME=portal",
                new SamplerDockerMetadata
                {
                    DockerIp = "172.17.0.6",
                    HostIp = "10.144.152.123",
                    Task = "portal"
                }
            };
            yield return new object[]
            {
                "$10.144.152.123::NOMAD_TASK_NAME=fabio",
                null
            };
            yield return new object[]
            {
                "$:c3.domain:",
                null
            };
            
            yield return new object[]
            {
                "$:10.144.152.123:::",
                null
            };
            yield return new object[]
            {
                "#10.144.152.216.6379:10.144.152.215.41898|",
                new SamplerLink
                {
                    SourceAddress = "10.144.152.216",
                    SourcePort = "6379",
                    TargetAddress = "10.144.152.215",
                    TargetPort = "41898",
                    SourceTask = null,
                    TargetTask = null
                }
            };
            yield return new object[]
            {
                "#c1.domain.ca.6379:10.144.152.215.41898|",
                new SamplerLink
                {
                    SourceAddress = "c1.domain.ca",
                    SourcePort = "6379",
                    TargetAddress = "10.144.152.215",
                    TargetPort = "41898",
                    SourceTask = null,
                    TargetTask = null
                }
            };
            yield return new object[]
            {
                "#c1.domain.ca.6379:.41898|",
                null
            };
            yield return new object[]
            {
                "c1.domain.ca.6379:10.144.152.215.41898|",
                null
            };
            yield return new object[]
            {
                "#6379:10.144.152.215.41898|",
                null
            };
            yield return new object[]
            {
                $"{Constants.RedisKeyPrefix}#10.144.152.216.6379:10.144.152.215.41898|",
                new SamplerLink
                {
                    SourceAddress = "10.144.152.216",
                    SourcePort = "6379",
                    TargetAddress = "10.144.152.215",
                    TargetPort = "41898",
                    SourceTask = null,
                    TargetTask = null
                }
            };
            yield return new object[]
            {
                $"{Constants.RedisKeyPrefix}#c1.domain.ca.6379:10.144.152.215.41898|",
                new SamplerLink
                {
                    SourceAddress = "c1.domain.ca",
                    SourcePort = "6379",
                    TargetAddress = "10.144.152.215",
                    TargetPort = "41898",
                    SourceTask = null,
                    TargetTask = null
                }
            };
            yield return new object[]
            {
                $"{Constants.RedisKeyPrefix}#c1.domain.ca.6379:10.144.152.215.41898|fabio:",
                new SamplerLink
                {
                    SourceAddress = "c1.domain.ca",
                    SourcePort = "6379",
                    TargetAddress = "10.144.152.215",
                    TargetPort = "41898",
                    SourceTask = "fabio",
                    TargetTask = null
                }
            };
            yield return new object[]
            {
                $"#c1.domain.ca.6379:10.144.152.215.41898|fabio:",
                new SamplerLink
                {
                    SourceAddress = "c1.domain.ca",
                    SourcePort = "6379",
                    TargetAddress = "10.144.152.215",
                    TargetPort = "41898",
                    SourceTask = "fabio",
                    TargetTask = null
                }
            };
            yield return new object[]
            {
                $"#c1.domain.ca.6379:10.144.152.215.41898|fabio:portal",
                new SamplerLink
                {
                    SourceAddress = "c1.domain.ca",
                    SourcePort = "6379",
                    TargetAddress = "10.144.152.215",
                    TargetPort = "41898",
                    SourceTask = "fabio",
                    TargetTask = "portal"
                }
            };
            yield return new object[]
            {
                $"{Constants.RedisKeyPrefix}#c1.domain.ca.6379:10.144.152.215.41898|fabio:portal",
                new SamplerLink
                {
                    SourceAddress = "c1.domain.ca",
                    SourcePort = "6379",
                    TargetAddress = "10.144.152.215",
                    TargetPort = "41898",
                    SourceTask = "fabio",
                    TargetTask = "portal"
                }
            };
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}