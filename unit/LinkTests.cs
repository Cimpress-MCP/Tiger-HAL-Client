using System;
using FsCheck.Xunit;
using Test.Utility;
using Tiger.Hal.Client;
using Xunit;
using static System.StringComparer;
using static Tiger.Hal.Client.Resources;

namespace Test
{
    /// <summary>Tests of the functionality of the <see cref="Link"/> class.</summary>
    [Properties(
        Arbitrary = new[] { typeof(Generators) },
        QuietOnSuccess = true)]
    public static class LinkTests
    {
        public static TheoryData<string> UriTemplates { get; } = new TheoryData<string>
        {
            "https://example.invalid/resources{/id}",
            "http://example.com/~{username}/",
            "http://example.com/dictionary/{term:1}/{term}",
            "http://example.com/search{?q,lang}"
        };

        public static TheoryData<string> Uris { get; } = new TheoryData<string>
        {
            "https://example.invalid/resources/1",
            "http://example.com/~fred/",
            "http://example.com/dictionary/c/cat",
            "http://example.com/search?q=cat&lang=en"
        };

        public static TheoryData<string, object, string> ResolvableUriTemplates { get; } = new TheoryData<string, object, string>
        {
            { "https://example.invalid/resources{/id}", new { id = 1 },  "https://example.invalid/resources/1" },
            { "http://example.com/~{username}/", new { username = "fred" }, "http://example.com/~fred/" },
            { "http://example.com/dictionary/{term:1}/{term}", new { term = "cat" }, "http://example.com/dictionary/c/cat" },
            { "http://example.com/search{?q,lang}", new { q = "cat", lang = "en" }, "http://example.com/search?q=cat&lang=en" }
        };

        [Property(DisplayName = "An untemplated link has an href that can be a URI.")]
        public static void Untemplated_Href(AbsoluteUri href)
        {
            var link = new Link(href.ToString(), isTemplated: false);

            Assert.Equal(href, link.Href);
        }

        [Theory(DisplayName = "A templated link has an href that cannot be a URI.")]
        [MemberData(nameof(UriTemplates))]
        public static void Templated_Href_Throws(string template)
        {
            var link = new Link(template, isTemplated: true);

            var actual = Record.Exception(() => _ = link.Href);

            var nse = Assert.IsType<NotSupportedException>(actual);
            Assert.Equal(LinkIsTemplated, nse.Message, Ordinal);
        }

        [Property(DisplayName = "An untemplated link has an href that cannot be a URI template.")]
        public static void Untemplated_TemplatedHref(AbsoluteUri href)
        {
            var link = new Link(href.ToString(), isTemplated: false);

            var actual = Record.Exception(() => _ = link.TemplatedHref);

            var nse = Assert.IsType<NotSupportedException>(actual);
            Assert.Equal(LinkIsNotTemplated, nse.Message, Ordinal);
        }

        [Theory(DisplayName = "A templated link has an href that can be a URI template.")]
        [MemberData(nameof(UriTemplates))]
        public static void Templated_TemplatedHref(string template)
        {
            var link = new Link(template, isTemplated: true);

            Assert.Equal(template, link.TemplatedHref.ToString());
        }

        [Theory(DisplayName = "An untemplated link can be resolved.")]
        [MemberData(nameof(Uris))]
        public static void Untemplated_ResolveHref(string actual)
        {
            var link = new Link(actual, isTemplated: false);

            var expected = link.ResolveHref(new { });

            Assert.Equal(expected.AbsoluteUri, actual);
        }

        [Theory(DisplayName = "A templated link can be resolved.")]
        [MemberData(nameof(ResolvableUriTemplates))]
        public static void Templated_ResolveHref(string template, object parameters, string actual)
        {
            var link = new Link(template, isTemplated: true);

            var expected = link.ResolveHref(parameters);

            Assert.Equal(expected.AbsoluteUri, actual);
        }
    }
}
