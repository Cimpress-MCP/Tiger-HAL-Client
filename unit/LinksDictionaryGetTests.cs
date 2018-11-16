using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using FsCheck;
using FsCheck.Xunit;
using Test.Utility;
using Tiger.Hal.Client;
using Xunit;
using static System.StringComparer;
using static Tiger.Hal.Client.Relations;
using static Tiger.Hal.Client.Resources;

namespace Test
{
    /// <summary>Tests of the functionality of the <see cref="LinksDictionary"/> class.</summary>
    [Properties(
        Arbitrary = new[] { typeof(Generators) },
        QuietOnSuccess = true)]
    public static class LinksDictionaryGetTests
    {
        [Property(DisplayName = "Getting a singular link with GetSingle succeeds.")]
        public static void GetSingle_Singular(AbsoluteUri self, AbsoluteUri author)
        {
            var sut = new LinksDictionary();
            SetUpLinks(sut, self, author);

            var actual = sut.GetSingle("author");
            Assert.NotNull(actual);
            Assert.False(actual.IsTemplated);
            Assert.Equal(author, actual.Href);
        }

        [Property(DisplayName = "Getting a singular link with GetMany throws.")]
        public static void GetMany_Singular_Throws(AbsoluteUri self, AbsoluteUri author)
        {
            var sut = new LinksDictionary();
            SetUpLinks(sut, self, author);

            var actual = Record.Exception(() => _ = sut.GetMany("author"));
            var ioe = Assert.IsType<InvalidOperationException>(actual);
            var expectedMessage = string.Format(CultureInfo.InvariantCulture, UnexpectedObject, "author");
            Assert.Equal(expectedMessage, ioe.Message, Ordinal);
        }

        [Property(DisplayName = "Getting a plural link with GetSingle throws.")]
        public static void GetSingle_Plural_Throws(AbsoluteUri self, NonEmptyArray<AbsoluteUri> author)
        {
            var sut = new LinksDictionary();
            SetUpLinks(sut, self, author.Get);

            var actual = Record.Exception(() => _ = sut.GetSingle("author"));
            var ioe = Assert.IsType<InvalidOperationException>(actual);
            var expectedMessage = string.Format(CultureInfo.InvariantCulture, UnexpectedArray, "author");
            Assert.Equal(expectedMessage, ioe.Message, Ordinal);
        }

        [Property(DisplayName = "Getting a plural link with GetMany succeeds.")]
        public static void GetMany_Plural(AbsoluteUri self, NonEmptyArray<AbsoluteUri> author)
        {
            var sut = new LinksDictionary();
            SetUpLinks(sut, self, author.Get);

            var actuals = sut.GetMany("author");
            Assert.Equal(actuals.Length, author.Get.Length);
            Assert.All(actuals, a => Assert.False(a.IsTemplated));
            Assert.All(author.Get.Zip(actuals, (e, a) => (e, a)), p => Assert.Equal(p.e, p.a.Href));
        }

        static void SetUpLinks(IDictionary<string, LinkCollection> sut, AbsoluteUri self, AbsoluteUri author)
        {
            var selfLinkCollection = new LinkCollection(new Link(self.ToString(), isTemplated: false));
            var authorLinkCollection = new LinkCollection(new Link(author.ToString(), isTemplated: false));
            sut.Add(Self, selfLinkCollection);
            sut.Add("author", authorLinkCollection);
        }

        static void SetUpLinks(IDictionary<string, LinkCollection> sut, AbsoluteUri self, AbsoluteUri[] author)
        {
            var selfLinkCollection = new LinkCollection(new Link(self.ToString(), isTemplated: false));
            var authorLinkCollection = new LinkCollection(author.Select(a => new Link(a.ToString(), isTemplated: false)).ToList());
            sut.Add(Self, selfLinkCollection);
            sut.Add("author", authorLinkCollection);
        }
    }
}
