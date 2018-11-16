using System;
using System.Collections.Generic;
using FsCheck;
using JetBrains.Annotations;
using static JetBrains.Annotations.ImplicitUseTargetFlags;

namespace Test.Utility
{
    [UsedImplicitly(Members)]
    static class Generators
    {
        [NotNull]
        public static Arbitrary<AbsoluteUri> AbsoluteUri { get; } = Arb.From(
                from scheme in Gen.Elements("http", "https")
                from hn in Arb.Generate<HostName>()
                select new UriBuilder(scheme, hn.ToString()) into ub
                select ub.Uri)
            .Convert(uri => new AbsoluteUri(uri), au => au.ToUri());

        [NotNull]
        public static Arbitrary<UnequalNonNullPair<T>> UnequalNonNullPair<T>()
            where T : class => Arb.Generate<NonNull<T>>()
            .Two().Select(nn => (left: nn.Item1.Get, right: nn.Item2.Get))
            .Where(t => !EqualityComparer<T>.Default.Equals(t.left, t.right))
            .ToArbitrary()
            .Convert(t => new UnequalNonNullPair<T>(t), unnp => (unnp.Left, unnp.Right));
    }
}
