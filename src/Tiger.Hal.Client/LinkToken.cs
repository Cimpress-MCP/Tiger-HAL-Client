// <copyright file="LinkToken.cs" company="Cimpress, Inc.">
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
    /// <summary>Represents a token which can be exchanged for a strongly-typed link or link collection.</summary>
    public readonly struct LinkToken
        : IEquatable<LinkToken>
    {
        readonly LinkCollection _linkCollection;

        /// <summary>Initializes a new instance of the <see cref="LinkToken"/> struct.</summary>
        /// <param name="rel">The relation associated with this token.</param>
        /// <param name="linkCollection">The collection of links from which to solidify the relation.</param>
        public LinkToken([NotNull] string rel, [NotNull, ItemNotNull] LinkCollection linkCollection)
        {
            Rel = rel ?? throw new ArgumentNullException(nameof(rel));
            _linkCollection = linkCollection ?? throw new ArgumentNullException(nameof(linkCollection));
        }

        /// <summary>Gets the relation associated with this token.</summary>
        [NotNull]
        public string Rel { get; }

        /// <summary>
        /// Gets a value indicating whether this token represents
        /// a singular link or a plural collection thereof.
        /// </summary>
        public Cardinality Cardinality => _linkCollection?.Cardinality ?? Plural;

        /// <summary>
        /// Compares two instances of the <see cref="LinkToken"/> struct for value equality.
        /// </summary>
        /// <param name="left">The left instance of the <see cref="LinkToken"/> struct.</param>
        /// <param name="right">The right instance of the <see cref="LinkToken"/> struct.</param>
        /// <returns>
        /// <see langword="true"/> if <paramref name="left"/> and <paramref name="right"/>
        /// represent the same value; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool operator ==(LinkToken left, LinkToken right) => left.Equals(right);

        /// <summary>
        /// Compares two instances od the <see cref="LinkToken"/> struct for value inequality.
        /// </summary>
        /// <param name="left">The left instance of the <see cref="LinkToken"/> struct.</param>
        /// <param name="right">The right instance of the <see cref="LinkToken"/> struct.</param>
        /// <returns>
        /// <see langword="true"/> if <paramref name="left"/> and <paramref name="right"/>
        /// do not represent the same value; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool operator !=(LinkToken left, LinkToken right) => !(left == right);

        /// <summary>Exchanges the token for a singular link.</summary>
        /// <returns>The singular link associated with <see cref="Rel"/>.</returns>
        /// <exception cref="InvalidOperationException">Value for link relation <see cref="Rel"/> is not singular.</exception>
        [NotNull]
        public Link ToSingle()
        {
            if (Cardinality != Singular)
            {
                throw new InvalidOperationException(string.Format(InvariantCulture, UnexpectedArray, Rel));
            }

            return _linkCollection[0];
        }

        /// <summary>Exchanges the token for a plural collection of links.</summary>
        /// <returns>The plural collection of links associated with <see cref="Rel"/>.</returns>
        /// <exception cref="InvalidOperationException">Value for link relation <see cref="Rel"/> is not plural.</exception>
        [ItemNotNull]
        public ImmutableArray<Link> ToMany()
        {
            if (Cardinality != Plural)
            {
                throw new InvalidOperationException(string.Format(InvariantCulture, UnexpectedObject, Rel));
            }

            return _linkCollection is null || _linkCollection.Count == 0
                ? ImmutableArray<Link>.Empty
                : ImmutableArray.CreateRange(_linkCollection);
        }

        /// <inheritdoc/>
        public override string ToString() => $"{Rel}: {Cardinality}";

        /// <inheritdoc/>
        public override bool Equals(object obj) => obj is LinkToken linkToken && Equals(linkToken);

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            unchecked
            {
                var hash = 17;
                hash = hash * 23 + EqualityComparer<LinkCollection>.Default.GetHashCode(_linkCollection);
                return hash * 23 + StringComparer.Ordinal.GetHashCode(Rel);
            }
        }

        /// <inheritdoc/>
        public bool Equals(LinkToken other) =>
            EqualityComparer<LinkCollection>.Default.Equals(_linkCollection, other._linkCollection)
                && StringComparer.Ordinal.Equals(Rel, other.Rel);
    }
}
