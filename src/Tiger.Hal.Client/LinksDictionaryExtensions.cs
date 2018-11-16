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
using static Tiger.Hal.Client.Cardinality;

namespace Tiger.Hal.Client
{
    /// <summary>Extensions to the functionality of the <see cref="LinksDictionary"/> class.</summary>
    public static class LinksDictionaryExtensions
    {
        /// <summary>Gets the link associated with the specified relation.</summary>
        /// <param name="linkDictionary">The link dictionary from which to get.</param>
        /// <param name="rel">The relation whose link to get.</param>
        /// <returns>The link with the specified rel.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="rel"/> is <see langword="null"/>.</exception>
        /// <exception cref="KeyNotFoundException">Value for link relation <paramref name="rel"/> is not found.</exception>
        /// <exception cref="InvalidOperationException">Value for link relation <paramref name="rel"/> is not singular.</exception>
        [NotNull]
        public static Link GetSingle([NotNull] this LinksDictionary linkDictionary, [NotNull] string rel)
        {
            if (linkDictionary is null) { throw new ArgumentNullException(nameof(linkDictionary)); }
            if (rel is null) { throw new ArgumentNullException(nameof(rel)); }

            return linkDictionary[rel].ToSingle();
        }

        /// <summary>Gets the links associated with the specified relation.</summary>
        /// <param name="linkDictionary">The link dictionary from which to get.</param>
        /// <param name="rel">The relation whose links to get.</param>
        /// <returns>The links with the specified rel.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="rel"/> is <see langword="null"/>.</exception>
        /// <exception cref="KeyNotFoundException">Value for link relation <paramref name="rel"/> is not found.</exception>
        /// <exception cref="InvalidOperationException">Value for link relation <paramref name="rel"/> is not plural.</exception>
        [ItemNotNull]
        public static ImmutableArray<Link> GetMany([NotNull] this LinksDictionary linkDictionary, [NotNull] string rel)
        {
            if (linkDictionary is null) { throw new ArgumentNullException(nameof(linkDictionary)); }
            if (rel is null) { throw new ArgumentNullException(nameof(rel)); }

            return linkDictionary[rel].ToMany();
        }

        /// <summary>Gets the link associated with the specified relation.</summary>
        /// <param name="linkDictionary">The link dictionary from which to get.</param>
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
        public static bool TryGetSingle([NotNull] this LinksDictionary linkDictionary, [NotNull] string rel, out Link link)
        {
            if (linkDictionary is null) { throw new ArgumentNullException(nameof(linkDictionary)); }
            if (rel is null) { throw new ArgumentNullException(nameof(rel)); }

            if (!linkDictionary.TryGetValue(rel, out var token) || token.Cardinality != Singular)
            {
                link = default;
                return false;
            }

            link = token.ToSingle();
            return true;
        }

        /// <summary>Gets the links associated with the specified rel.</summary>
        /// <param name="linkDictionary">The link dictionary from which to get.</param>
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
        public static bool TryGetMany([NotNull] this LinksDictionary linkDictionary, string rel, out ImmutableArray<Link> links)
        {
            if (linkDictionary is null) { throw new ArgumentNullException(nameof(linkDictionary)); }
            if (rel is null) { throw new ArgumentNullException(nameof(rel)); }

            if (!linkDictionary.TryGetValue(rel, out var token) || token.Cardinality != Plural)
            {
                links = default;
                return false;
            }

            links = token.ToMany();
            return true;
        }
    }
}
