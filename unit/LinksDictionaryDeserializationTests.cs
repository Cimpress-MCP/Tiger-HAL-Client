using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Tiger.Hal.Client;
using Xunit;
using static System.StringComparer;
using static System.UriKind;
using static Tiger.Hal.Client.Cardinality;
using static Tiger.Hal.Client.Resources;

namespace Test
{
    /// <summary>Tests of the functionality of the <see cref="LinksDictionary"/> class.</summary>
    public static class LinksDictionaryDeserializationTests
    {
        const string BookResponse = @"
{
  ""_links"": {
    ""self"": { ""href"": ""/books/the-way-of-zen"" },
    ""author"": { ""href"": ""/people/alan-watts"" }
  }
}";

        const string ItemResponse = @"
{
  ""_links"": {
    ""self"": { ""href"": ""/orders/523"" },
    ""warehouse"": { ""href"": ""/warehouse/56"" },
    ""invoice"": { ""href"": ""/invoices/873"" }
  },
  ""currency"": ""USD"",
  ""status"": ""shipped"",
  ""total"": 10.20
}";

        public static IReadOnlyCollection<string> Examples { get; } = new[]
        {
            BookResponse,
            ItemResponse
        };

        public static TheoryData<string, T> Propertize<T>(params T[] properties)
        {
            var theoryData = new TheoryData<string, T>();
            foreach (var (example, property) in Examples.Zip(properties, ValueTuple.Create))
            {
                theoryData.Add(example, property);
            }
            return theoryData;
        }

        public static TheoryData<string, int> LinkCount { get; } = Propertize(
            2,
            3);

        public static TheoryData<string, string> SelfHref { get; } = Propertize(
            "/books/the-way-of-zen",
            "/orders/523");

        public static TheoryData<string, Cardinality> Cardinality { get; } = Propertize(
            Singular,
            Singular);

        sealed class Resource
        {
            [JsonProperty("_links"), NotNull]
            public LinksDictionary Links { get; } = new LinksDictionary();
        }

        sealed class Book
        {
            [JsonProperty("_links"), NotNull]
            public LinksDictionary Links { get; } = new LinksDictionary();
        }

        sealed class Item
        {
            [JsonProperty("_links"), NotNull]
            public LinksDictionary Links { get; } = new LinksDictionary();

            public string Currency { get; set; }

            public string Status { get; set; }

            public decimal Total { get; set; }
        }

        [Theory(DisplayName = "The correct number of links is deserialized.")]
        [MemberData(nameof(LinkCount))]
        public static void ExampleFromSpecification_Count(string response, int expected)
        {
            var actual = JsonConvert.DeserializeObject<Resource>(response);

            Assert.Equal(expected, actual.Links.Count);
        }

        [Theory(DisplayName = "The self link is deserialized correctly.")]
        [MemberData(nameof(SelfHref))]
        public static void ExampleFromSpecification_Self(string response, string expected)
        {
            var actual = JsonConvert.DeserializeObject<Resource>(response);

            var selfLink = actual.Links.Self;
            Assert.Equal(expected, selfLink.Href.ToString());
            Assert.False(selfLink.IsTemplated);
        }

        [Theory(DisplayName = "The cardinality of a link relation is preserved.")]
        [MemberData(nameof(Cardinality))]
        public static void ExampleFromSpecification_Cardinality(string response, Cardinality expected)
        {
            var actual = JsonConvert.DeserializeObject<Resource>(response);

            Assert.Equal(expected, actual.Links.GetCardinality(Relations.Self));
        }

        [Theory(DisplayName = "The Book example fully deserializes correctly.")]
        [InlineData(BookResponse)]
        public static void Books_FullTest(string response)
        {
            var actual = JsonConvert.DeserializeObject<Book>(response);

            Assert.Equal(new Uri("/books/the-way-of-zen", Relative), actual.Links.Self.Href);
            var authorLink = actual.Links.GetSingle("author");
            Assert.False(authorLink.IsTemplated);
            Assert.Equal(new Uri("/people/alan-watts", Relative), authorLink.Href);
        }

        [Theory(DisplayName = "The Item example fully deserializes correctly.")]
        [InlineData(ItemResponse)]
        public static void Items_FullTest(string response)
        {
            var actual = JsonConvert.DeserializeObject<Item>(response);

            Assert.Equal(new Uri("/orders/523", Relative), actual.Links.Self.Href);
            var warehouseLink = actual.Links.GetSingle("warehouse");
            Assert.False(warehouseLink.IsTemplated);
            Assert.Equal(new Uri("/warehouse/56", Relative), warehouseLink.Href);
            var invoiceLink = actual.Links.GetSingle("invoice");
            Assert.False(invoiceLink.IsTemplated);
            Assert.Equal(new Uri("/invoices/873", Relative), invoiceLink.Href);
            Assert.Equal("USD", actual.Currency, Ordinal);
            Assert.Equal("shipped", actual.Status, Ordinal);
            Assert.Equal(10.20m, actual.Total);
        }

        [Theory(DisplayName = "A templated self link throws.")]
        [InlineData(@"
{
  ""_links"": {
    ""self"": { ""href"": ""https://example.invalid/resources{/id}"", ""templated"": true }
  }
}
")]
        public static void TemplatedSelf_Throws(string response)
        {
            var datum = JsonConvert.DeserializeObject<Resource>(response);
            var actual = Record.Exception(() => { var _ = datum.Links.Self.Href; });

            var ioe = Assert.IsType<InvalidOperationException>(actual);
            var expectedMessage = string.Format(CultureInfo.InvariantCulture, UnexpectedIsTemplated, "self");
            Assert.Equal(expectedMessage, ioe.Message, InvariantCulture);
        }
    }
}
