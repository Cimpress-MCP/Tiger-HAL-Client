using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Tiger.Hal.Client;
using Xunit;
using static System.StringComparer;

namespace Test
{
    /// <summary>Tests of the functionality of the <see cref="EmbeddedDictionary"/> class.</summary>
    public static class EmbeddedDictionaryDeserializationTests
    {
        const string BookResponse = @"
{
  ""_links"": {
    ""self"": { ""href"": ""/books/the-way-of-zen"" },
    ""author"": { ""href"": ""/people/alan-watts"" }
  },
  ""_embedded"": {
    ""author"": {
      ""_links"": { ""self"": { ""href"": ""/people/alan-watts"" } },
      ""name"": ""Alan Watts"",
      ""born"": ""January 6, 1915"",
      ""died"": ""November 16, 1973""
    }
  }
}
";

        public static IReadOnlyCollection<string> Examples { get; } = new[] { BookResponse };

        public static TheoryData<string, T> Propertize<T>(params T[] properties)
        {
            var theoryData = new TheoryData<string, T>();
            foreach (var (example, property) in Examples.Zip(properties, ValueTuple.Create))
            {
                theoryData.Add(example, property);
            }
            return theoryData;
        }

        public static TheoryData<string, int> EmbedCount { get; } = Propertize(1);

        sealed class Resource
        {
            [JsonProperty("_embedded"), NotNull]
            public EmbeddedDictionary Embedded { get; } = new EmbeddedDictionary();
        }

        sealed class Book
        {
            [JsonProperty("_links"), NotNull]
            public LinksDictionary Links { get; } = new LinksDictionary();

            [JsonProperty("_embedded"), NotNull]
            public EmbeddedDictionary Embedded { get; } = new EmbeddedDictionary();
        }

        sealed class Author
        {
            [JsonProperty("_links"), NotNull]
            public LinksDictionary Links { get; } = new LinksDictionary();

            public string Name { get; set; }

            public string Born { get; set; }

            public string Died { get; set; }
        }

        [Theory(DisplayName = "The correct number of embeds is deserialized.")]
        [MemberData(nameof(EmbedCount))]
        public static void ExampleFromSpecification_Count(string response, int expected)
        {
            var actual = JsonConvert.DeserializeObject<Resource>(response);

            Assert.Equal(expected, actual.Embedded.Count);
        }

        [Theory(DisplayName = "The Book example fully deserializes correctly.")]
        [InlineData(BookResponse)]
        public static void Books_FullTest(string response)
        {
            var book = JsonConvert.DeserializeObject<Book>(response);
            var actual = book.Embedded.Get<Author>("author");

            Assert.Single(actual.Links);
            Assert.Equal("Alan Watts", actual.Name, Ordinal);
            Assert.Equal("January 6, 1915", actual.Born, Ordinal);
            Assert.Equal("November 16, 1973", actual.Died, Ordinal);
        }
    }
}
