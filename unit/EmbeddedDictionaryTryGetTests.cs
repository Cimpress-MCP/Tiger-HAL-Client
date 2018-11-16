using System.Collections.Generic;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Tiger.Hal.Client;
using Xunit;

namespace Test
{
    /// <summary>Tests of the functionality of the <see cref="EmbeddedDictionary"/> class.</summary>
    public static class EmbeddedDictionaryTryGetTests
    {
        const string AuthorResponse = @"
{
  ""_links"": { ""self"": { ""href"": ""/people/alan-watts"" } },
  ""name"": ""Alan Watts"",
  ""born"": ""January 6, 1915"",
  ""died"": ""November 16, 1973""
}
";

        sealed class Author
        {
            [JsonProperty("_links"), NotNull]
            public LinksDictionary Links { get; } = new LinksDictionary();

            public string Name { get; set; }

            public string Born { get; set; }

            public string Died { get; set; }
        }

        [Theory(DisplayName = "An embed can be gotten with a compatible type.")]
        [InlineData(AuthorResponse)]
        public static void TryGetNotProvided_Match(string response)
        {
            var sut = new EmbeddedDictionary();
            var sutSetup = (IDictionary<string, JToken>)sut;
            sutSetup.Add("author", JObject.Parse(response));

            var success = sut.TryGet<Author>("author", out var actual);

            Assert.True(success);
            Assert.Equal("Alan Watts", actual.Name);
            Assert.Equal("January 6, 1915", actual.Born);
            Assert.Equal("November 16, 1973", actual.Died);
        }

        [Theory(DisplayName = "An embed can be gotten with a compatible type.")]
        [InlineData(AuthorResponse)]
        public static void TryGetProvided_Match(string response)
        {
            var sut = new EmbeddedDictionary();
            var sutSetup = (IDictionary<string, JToken>)sut;
            sutSetup.Add("author", JObject.Parse(response));

            var success = sut.TryGet<Author>("author", new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy(false, true, true)
                }
            }, out var actual);

            Assert.True(success);
            Assert.Equal("Alan Watts", actual.Name);
            Assert.Equal("January 6, 1915", actual.Born);
            Assert.Equal("November 16, 1973", actual.Died);
        }

        [Theory(DisplayName = "An embed can't be gotten with an incompatible type.")]
        [InlineData(AuthorResponse)]
        public static void TryGetNotProvided_NotMatch_Throw(string response)
        {
            var sut = new EmbeddedDictionary();
            var sutSetup = (IDictionary<string, JToken>)sut;
            sutSetup.Add("author", JObject.Parse(response));

            var success = sut.TryGet<Author[]>("author", out var actual);

            Assert.False(success);
            Assert.Equal(default, actual);
        }

        [Theory(DisplayName = "An embed can't be gotten with an incompatible type.")]
        [InlineData(AuthorResponse)]
        public static void TryGetProvided_NotMatch_Throw(string response)
        {
            var sut = new EmbeddedDictionary();
            var sutSetup = (IDictionary<string, JToken>)sut;
            sutSetup.Add("author", JObject.Parse(response));

            var success = sut.TryGet<Author[]>("author", new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy(false, true, true)
                }
            }, out var actual);

            Assert.False(success);
            Assert.Equal(default, actual);
        }
    }
}
