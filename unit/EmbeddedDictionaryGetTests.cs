using System;
using System.Collections.Generic;
using System.Globalization;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Tiger.Hal.Client;
using Xunit;
using static System.StringComparer;
using static Tiger.Hal.Client.Resources;

namespace Test
{
    /// <summary>Tests of the functionality of the <see cref="EmbeddedDictionary"/> class.</summary>
    public static class EmbeddedDictionaryGetTests
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
        public static void GetNotProvided_Match(string response)
        {
            var sut = new EmbeddedDictionary();
            var sutSetup = (IDictionary<string, JToken>)sut;
            sutSetup.Add("author", JObject.Parse(response));

            var actual = sut.Get<Author>("author");

            Assert.Equal("Alan Watts", actual.Name);
            Assert.Equal("January 6, 1915", actual.Born);
            Assert.Equal("November 16, 1973", actual.Died);
        }

        [Theory(DisplayName = "An embed can be gotten with a compatible type.")]
        [InlineData(AuthorResponse)]
        public static void GetProvided_Match(string response)
        {
            var sut = new EmbeddedDictionary();
            var sutSetup = (IDictionary<string, JToken>)sut;
            sutSetup.Add("author", JObject.Parse(response));

            var actual = sut.Get<Author>("author", new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy(false, true, true)
                }
            });

            Assert.Equal("Alan Watts", actual.Name);
            Assert.Equal("January 6, 1915", actual.Born);
            Assert.Equal("November 16, 1973", actual.Died);
        }

        [Theory(DisplayName = "An embed can't be gotten with an incompatible type.")]
        [InlineData(AuthorResponse)]
        public static void GetNotProvided_NotMatch_Throw(string response)
        {
            var sut = new EmbeddedDictionary();
            var sutSetup = (IDictionary<string, JToken>)sut;
            sutSetup.Add("author", JObject.Parse(response));

            var actual = Record.Exception(() => _ = sut.Get<Author[]>("author"));

            var ioe = Assert.IsType<InvalidOperationException>(actual);
            var expectedMessage = string.Format(CultureInfo.InvariantCulture, CannotConvert, "author", typeof(Author[]));
            Assert.Equal(expectedMessage, ioe.Message, Ordinal);
        }

        [Theory(DisplayName = "An embed can't be gotten with an incompatible type.")]
        [InlineData(AuthorResponse)]
        public static void GetProvided_NotMatch_Throw(string response)
        {
            var sut = new EmbeddedDictionary();
            var sutSetup = (IDictionary<string, JToken>)sut;
            sutSetup.Add("author", JObject.Parse(response));

            var actual = Record.Exception(() => _ = sut.Get<Author[]>("author", new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy(false, true, true)
                }
            }));

            var ioe = Assert.IsType<InvalidOperationException>(actual);
            var expectedMessage = string.Format(CultureInfo.InvariantCulture, CannotConvert, "author", typeof(Author[]));
            Assert.Equal(expectedMessage, ioe.Message, Ordinal);
        }
    }
}
