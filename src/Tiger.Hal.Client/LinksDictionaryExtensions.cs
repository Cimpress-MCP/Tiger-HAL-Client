// <copyright file="LinksDictionaryExtensions.cs" company="Cimpress, Inc.">
//   Copyright 2018 Cimpress, Inc.
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
// </copyright>

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using JetBrains.Annotations;
using static System.Globalization.CultureInfo;
using static Tiger.Hal.Client.Cardinality;
using static Tiger.Hal.Client.Resources;

namespace Tiger.Hal.Client
{
    /// <summary>Extensions to the functionality of the <see cref="LinksDictionary"/> class.</summary>
    public static class LinksDictionaryExtensions
    {
        /// <summary>Gets the cardinality of the link associated with the specified relation.</summary>
        /// <param name="linksDictionary">The links dictionary from which to get.</param>
        /// <param name="rel">The relation whose link's cardinality to get.</param>
        /// <returns>The cardinality of the link with the specified rel.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="rel"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="rel"/> is not an absolute URI.</exception>
        /// <exception cref="KeyNotFoundException">Value for link relation <paramref name="rel"/> is not found.</exception>
        public static Cardinality GetCardinality([NotNull] this LinksDictionary linksDictionary, [NotNull] Uri rel)
        {
            if (linksDictionary is null) { throw new ArgumentNullException(nameof(linksDictionary)); }
            if (rel is null) { throw new ArgumentNullException(nameof(rel)); }
            if (!rel.IsAbsoluteUri) { throw new ArgumentException(string.Format(InvariantCulture, NotAbsoluteUri, rel), nameof(rel)); }

            return linksDictionary.GetCardinality(rel.AbsoluteUri);
        }

        /// <summary>
        /// Determines whether the read-only dictionary contains an element
        /// that has the specified key.
        /// </summary>
        /// <param name="linksDictionary">The links dictionary to check.</param>
        /// <param name="key">The key to locate.</param>
        /// <returns>
        /// <see langword="true"/> if the read-only dictionary contains an element
        /// that has the specified key; otherwise, <see langword="false"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="key"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="key"/> is not an absolute URI.</exception>
        public static bool ContainsKey([NotNull] this LinksDictionary linksDictionary, [NotNull] Uri key)
        {
            if (linksDictionary is null) { throw new ArgumentNullException(nameof(linksDictionary)); }
            if (key is null) { throw new ArgumentNullException(nameof(key)); }
            if (!key.IsAbsoluteUri) { throw new ArgumentException(string.Format(InvariantCulture, NotAbsoluteUri, key), nameof(key)); }

            return linksDictionary.ContainsKey(key.AbsoluteUri);
        }

        /// <summary>Gets the value that is associated with the specified key.</summary>
        /// <param name="linksDictionary">The links dictionary from which to get.</param>
        /// <param name="key">The key to locate.</param>
        /// <param name="value">
        /// When this method returns, the value associated with the specified <paramref name="key"/>,
        /// if the key is found; otherwise, the default value for the type of <paramref name="value"/>.
        /// This parameter is passed uninitialized.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the read-only dictionary contains an element that has the specified key;
        /// otherwise, <see langword="false"/>.
        /// </returns>
        public static bool TryGetValue([NotNull] this LinksDictionary linksDictionary, [NotNull] Uri key, out LinkToken value)
        {
            if (linksDictionary is null) { throw new ArgumentNullException(nameof(linksDictionary)); }
            if (key is null) { throw new ArgumentNullException(nameof(key)); }
            if (!key.IsAbsoluteUri) { throw new ArgumentException(string.Format(InvariantCulture, NotAbsoluteUri, key), nameof(key)); }

            return linksDictionary.TryGetValue(key.AbsoluteUri, out value);
        }

        /// <summary>Gets the link associated with the specified relation.</summary>
        /// <param name="linksDictionary">The links dictionary from which to get.</param>
        /// <param name="rel">The relation whose link to get.</param>
        /// <returns>The link with the specified rel.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="rel"/> is <see langword="null"/>.</exception>
        /// <exception cref="KeyNotFoundException">Value for link relation <paramref name="rel"/> is not found.</exception>
        /// <exception cref="InvalidOperationException">Value for link relation <paramref name="rel"/> is not singular.</exception>
        [NotNull]
        public static Link GetSingle([NotNull] this LinksDictionary linksDictionary, [NotNull] string rel)
        {
            if (linksDictionary is null) { throw new ArgumentNullException(nameof(linksDictionary)); }
            if (rel is null) { throw new ArgumentNullException(nameof(rel)); }

            return linksDictionary[rel].ToSingle();
        }

        /// <summary>Gets the link associated with the specified relation.</summary>
        /// <param name="linksDictionary">The links dictionary from which to get.</param>
        /// <param name="rel">The relation whose link to get.</param>
        /// <returns>The link with the specified rel.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="rel"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="rel"/> is not an absolute URI.</exception>
        /// <exception cref="KeyNotFoundException">Value for link relation <paramref name="rel"/> is not found.</exception>
        /// <exception cref="InvalidOperationException">Value for link relation <paramref name="rel"/> is not singular.</exception>
        [NotNull]
        public static Link GetSingle([NotNull] this LinksDictionary linksDictionary, [NotNull] Uri rel)
        {
            if (linksDictionary is null) { throw new ArgumentNullException(nameof(linksDictionary)); }
            if (rel is null) { throw new ArgumentNullException(nameof(rel)); }
            if (!rel.IsAbsoluteUri) { throw new ArgumentException(string.Format(InvariantCulture, NotAbsoluteUri, rel), nameof(rel)); }

            return linksDictionary.GetSingle(rel.AbsoluteUri);
        }

        /// <summary>Gets the links associated with the specified relation.</summary>
        /// <param name="linksDictionary">The links dictionary from which to get.</param>
        /// <param name="rel">The relation whose links to get.</param>
        /// <returns>The links with the specified rel.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="rel"/> is <see langword="null"/>.</exception>
        /// <exception cref="KeyNotFoundException">Value for link relation <paramref name="rel"/> is not found.</exception>
        /// <exception cref="InvalidOperationException">Value for link relation <paramref name="rel"/> is not plural.</exception>
        [ItemNotNull]
        public static ImmutableArray<Link> GetMany([NotNull] this LinksDictionary linksDictionary, [NotNull] string rel)
        {
            if (linksDictionary is null) { throw new ArgumentNullException(nameof(linksDictionary)); }
            if (rel is null) { throw new ArgumentNullException(nameof(rel)); }

            return linksDictionary[rel].ToMany();
        }

        /// <summary>Gets the links associated with the specified relation.</summary>
        /// <param name="linksDictionary">The links dictionary from which to get.</param>
        /// <param name="rel">The relation whose links to get.</param>
        /// <returns>The links with the specified rel.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="rel"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="rel"/> is not an absolute URI.</exception>
        /// <exception cref="KeyNotFoundException">Value for link relation <paramref name="rel"/> is not found.</exception>
        /// <exception cref="InvalidOperationException">Value for link relation <paramref name="rel"/> is not plural.</exception>
        [ItemNotNull]
        public static ImmutableArray<Link> GetMany([NotNull] this LinksDictionary linksDictionary, [NotNull] Uri rel)
        {
            if (linksDictionary is null) { throw new ArgumentNullException(nameof(linksDictionary)); }
            if (rel is null) { throw new ArgumentNullException(nameof(rel)); }
            if (!rel.IsAbsoluteUri) { throw new ArgumentException(string.Format(InvariantCulture, NotAbsoluteUri, rel), nameof(rel)); }

            return linksDictionary.GetMany(rel.AbsoluteUri);
        }

        /// <summary>Gets the link associated with the specified relation.</summary>
        /// <param name="linksDictionary">The links dictionary from which to get.</param>
        /// <param name="rel">The relation whose link to get.</param>
        /// <param name="link">
        /// When this method returns, the link associated with the specified rel if the rel is found;
        /// otherwise, the default value for a link. This parameter is passed uninitialized.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if this instance contains a value with the specified rel
        /// which is a link; otherwise, <see langword="false"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="rel"/> is <see langword="null"/>.</exception>
        [ContractAnnotation("=>false,link:null; =>true,link:notnull")]
        public static bool TryGetSingle([NotNull] this LinksDictionary linksDictionary, [NotNull] string rel, out Link link)
        {
            if (linksDictionary is null) { throw new ArgumentNullException(nameof(linksDictionary)); }
            if (rel is null) { throw new ArgumentNullException(nameof(rel)); }

            if (!linksDictionary.TryGetValue(rel, out var token) || token.Cardinality != Singular)
            {
                link = default;
                return false;
            }

            link = token.ToSingle();
            return true;
        }

        /// <summary>Gets the link associated with the specified relation.</summary>
        /// <param name="linksDictionary">The links dictionary from which to get.</param>
        /// <param name="rel">The relation whose link to get.</param>
        /// <param name="link">
        /// When this method returns, the link associated with the specified rel if the rel is found;
        /// otherwise, the default value for a link. This parameter is passed uninitialized.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if this instance contains a value with the specified rel
        /// which is a link; otherwise, <see langword="false"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="rel"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="rel"/> is not an absolute URI.</exception>
        [ContractAnnotation("=>false,link:null; =>true,link:notnull")]
        public static bool TryGetSingle([NotNull] this LinksDictionary linksDictionary, [NotNull] Uri rel, out Link link)
        {
            if (linksDictionary is null) { throw new ArgumentNullException(nameof(linksDictionary)); }
            if (rel is null) { throw new ArgumentNullException(nameof(rel)); }
            if (!rel.IsAbsoluteUri) { throw new ArgumentException(string.Format(InvariantCulture, NotAbsoluteUri, rel), nameof(rel)); }

            return linksDictionary.TryGetSingle(rel.AbsoluteUri, out link);
        }

        /// <summary>Gets the links associated with the specified rel.</summary>
        /// <param name="linksDictionary">The links dictionary from which to get.</param>
        /// <param name="rel">The relation whose links to get.</param>
        /// <param name="links">
        /// When this method returns, the links associated with the specified rel if the rel is found;
        /// otherwise, the default value of the collection. This parameter is passed uninitialized.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if this instance contains a value with the specified rel
        /// which is a collection of links; otherwise, <see langword="false"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="rel"/> is <see langword="null"/>.</exception>
        public static bool TryGetMany([NotNull] this LinksDictionary linksDictionary, [NotNull] string rel, out ImmutableArray<Link> links)
        {
            if (linksDictionary is null) { throw new ArgumentNullException(nameof(linksDictionary)); }
            if (rel is null) { throw new ArgumentNullException(nameof(rel)); }

            if (!linksDictionary.TryGetValue(rel, out var token) || token.Cardinality != Plural)
            {
                links = default;
                return false;
            }

            links = token.ToMany();
            return true;
        }

        /// <summary>Gets the links associated with the specified rel.</summary>
        /// <param name="linksDictionary">The links dictionary from which to get.</param>
        /// <param name="rel">The relation whose links to get.</param>
        /// <param name="links">
        /// When this method returns, the links associated with the specified rel if the rel is found;
        /// otherwise, the default value of the collection. This parameter is passed uninitialized.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if this instance contains a value with the specified rel
        /// which is a collection of links; otherwise, <see langword="false"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="rel"/> is <see langword="null"/>.</exception>
        public static bool TryGetMany([NotNull] this LinksDictionary linksDictionary, [NotNull] Uri rel, out ImmutableArray<Link> links)
        {
            if (linksDictionary is null) { throw new ArgumentNullException(nameof(linksDictionary)); }
            if (rel is null) { throw new ArgumentNullException(nameof(rel)); }
            if (!rel.IsAbsoluteUri) { throw new ArgumentException(string.Format(InvariantCulture, NotAbsoluteUri, rel), nameof(rel)); }

            return linksDictionary.TryGetMany(rel.AbsoluteUri, out links);
        }
    }
}
