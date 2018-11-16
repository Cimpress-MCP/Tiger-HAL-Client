using System.Collections.Generic;
using System.Linq;
using FsCheck;
using FsCheck.Xunit;
using Test.Utility;
using Tiger.Hal.Client;
using Xunit;
using static Tiger.Hal.Client.Relations;

namespace Test
{
    /// <summary>Tests of the functionality of the <see cref="LinksDictionary"/> class.</summary>
    [Properties(
        Arbitrary = new[] { typeof(Generators) },
        QuietOnSuccess = true)]
    public static class LinksDictionaryTryGetTests
    {
        [Property(DisplayName = "Trying getting a singular link with TryGetLink succeeds.")]
        public static void TryGetLink_Object(AbsoluteUri self, AbsoluteUri author)
        {
            var sut = new LinksDictionary();
            SetUpLinks(sut, self, author);

            var success = sut.TryGetSingle("author", out var actual);

            Assert.True(success);
            Assert.NotNull(actual);
            Assert.False(actual.IsTemplated);
            Assert.Equal(author, actual.Href);
        }

        [Property(DisplayName = "Trying getting a singular link with TryGetLinks fails.")]
        public static void TryGetLinks_Object(AbsoluteUri self, AbsoluteUri author)
        {
            var sut = new LinksDictionary();
            SetUpLinks(sut, self, author);

            var success = sut.TryGetMany("author", out var actual);

            Assert.False(success);
            Assert.True(actual.IsDefault);
        }

        [Property(DisplayName = "Trying getting a plural link with TryGetLink fails.")]
        public static void GetLink_Array(AbsoluteUri self, NonEmptyArray<AbsoluteUri> author)
        {
            var sut = new LinksDictionary();
            SetUpLinks(sut, self, author.Get);

            var success = sut.TryGetSingle("author", out var actual);

            Assert.False(success);
            Assert.Null(actual);
        }

        [Property(DisplayName = "Trying getting a plural link with TryGetLinks succeeds.")]
        public static void GetLinks_Array(AbsoluteUri self, NonEmptyArray<AbsoluteUri> author)
        {
            var sut = new LinksDictionary();
            SetUpLinks(sut, self, author.Get);

            var success = sut.TryGetMany("author", out var actuals);

            Assert.True(success);
            Assert.False(actuals.IsDefault);
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
