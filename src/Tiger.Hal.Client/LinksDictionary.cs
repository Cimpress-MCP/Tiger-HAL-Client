// <copyright file="LinksDictionary.cs" company="Cimpress, Inc.">
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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using JetBrains.Annotations;
using static System.Globalization.CultureInfo;
using static Tiger.Hal.Client.Resources;

namespace Tiger.Hal.Client
{
    /// <summary>Represents the map of relations via hyperlinks from this entity to others.</summary>
    public sealed class LinksDictionary
        : IDictionary<string, LinkCollection>, IReadOnlyDictionary<string, LinkToken>
    {
        readonly IDictionary<string, LinkCollection> _links = new Dictionary<string, LinkCollection>(StringComparer.Ordinal);

        /// <summary>Gets the "self" link of this entity.</summary>
        /// <exception cref="InvalidOperationException">Value for link relation 'self' is not found.</exception>
        /// <exception cref="InvalidOperationException">Value for link relation 'self' not singular.</exception>
        /// <exception cref="InvalidOperationException">Value for link relation 'self' is templated.</exception>
        [NotNull]
        public SelfLink Self
        {
            get
            {
                /* note(cosborn)
                 * It feels odd to have this property depend on an extension method
                 * on its own type, but if extension properties existed, this would
                 * be one of them.
                 */

                Link link;
                try
                {
                    link = this.GetSingle(Relations.Self);
                }
                catch (KeyNotFoundException knfe)
                {
                    throw new InvalidOperationException(string.Format(InvariantCulture, RelationMissing, Relations.Self), knfe);
                }

                try
                {
                    return link.ToSelfLink();
                }
                catch (NotSupportedException nse)
                {
                    throw new InvalidOperationException(string.Format(InvariantCulture, UnexpectedIsTemplated, Relations.Self), nse);
                }
            }
        }

        /// <inheritdoc/>
        public int Count => _links.Count;

        /// <inheritdoc/>
        [NotNull, ItemNotNull]
        public IEnumerable<string> Keys => _links.Keys;

        /// <inheritdoc/>
        [NotNull]
        public IEnumerable<LinkToken> Values => _links.Select(kvp => new LinkToken(kvp.Key, kvp.Value));

        /// <inheritdoc/>
        bool ICollection<KeyValuePair<string, LinkCollection>>.IsReadOnly => _links.IsReadOnly;

        /// <inheritdoc/>
        ICollection<string> IDictionary<string, LinkCollection>.Keys => _links.Keys;

        /// <inheritdoc/>
        ICollection<LinkCollection> IDictionary<string, LinkCollection>.Values => _links.Values;

        /// <inheritdoc/>
        [NotNull]
        public LinkToken this[[NotNull] string key] => new LinkToken(key, _links[key]);

        /// <summary>Gets the element that has the specified key in the read-only dictionary.</summary>
        /// <param name="key">The key to locate.</param>
        /// <returns>The element that has the key in the read-only dictionary.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="key"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="key"/> is not an absolute URI.</exception>
        /// <exception cref="KeyNotFoundException">The property is retrieved and <paramref name="key"/> is not found.</exception>
        [NotNull]
        [SuppressMessage("Microsoft.Guidelines", "CA1043", Justification = "A URI is just a string with a format.")]
        public LinkToken this[[NotNull] Uri key]
        {
            get
            {
                if (key is null) { throw new ArgumentNullException(nameof(key)); }
                if (!key.IsAbsoluteUri) { throw new ArgumentException(string.Format(InvariantCulture, NotAbsoluteUri, key), nameof(key)); }

                return new LinkToken(key.AbsoluteUri, _links[key.AbsoluteUri]);
            }
        }

        /// <inheritdoc/>
        LinkCollection IDictionary<string, LinkCollection>.this[string key]
        {
            get => _links[key];
            set => _links[key] = value;
        }

        /// <summary>Gets the cardinality of the link associated with the specified relation.</summary>
        /// <param name="rel">The relation whose link's cardinality to get.</param>
        /// <returns>The cardinality of the link with the specified rel.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="rel"/> is <see langword="null"/>.</exception>
        /// <exception cref="KeyNotFoundException">Value for link relation <paramref name="rel"/> is not found.</exception>
        public Cardinality GetCardinality([NotNull] string rel)
        {
            if (rel is null) { throw new ArgumentNullException(nameof(rel)); }

            return _links[rel].Cardinality;
        }

        /// <inheritdoc/>
        public IEnumerator<KeyValuePair<string, LinkToken>> GetEnumerator() =>
            _links.ToDictionary(kvp => kvp.Key, kvp => new LinkToken(kvp.Key, kvp.Value)).GetEnumerator();

        /// <inheritdoc/>
        public bool ContainsKey(string key) => _links.ContainsKey(key);

        /// <inheritdoc/>
        public bool TryGetValue(string key, out LinkToken value)
        {
            if (!_links.TryGetValue(key, out var linkCollection))
            {
                value = default;
                return false;
            }

            value = new LinkToken(key, linkCollection);
            return true;
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_links).GetEnumerator();

        /// <inheritdoc/>
        IEnumerator<KeyValuePair<string, LinkCollection>> IEnumerable<KeyValuePair<string, LinkCollection>>.GetEnumerator() =>
            _links.GetEnumerator();

        /// <inheritdoc/>
        void ICollection<KeyValuePair<string, LinkCollection>>.Add(KeyValuePair<string, LinkCollection> item) => _links.Add(item);

        /// <inheritdoc/>
        void ICollection<KeyValuePair<string, LinkCollection>>.Clear() => _links.Clear();

        /// <inheritdoc/>
        bool ICollection<KeyValuePair<string, LinkCollection>>.Contains(KeyValuePair<string, LinkCollection> item) =>
            _links.Contains(item);

        /// <inheritdoc/>
        void ICollection<KeyValuePair<string, LinkCollection>>.CopyTo(KeyValuePair<string, LinkCollection>[] array, int arrayIndex) =>
            _links.CopyTo(array, arrayIndex);

        /// <inheritdoc/>
        bool ICollection<KeyValuePair<string, LinkCollection>>.Remove(KeyValuePair<string, LinkCollection> item) => _links.Remove(item);

        /// <inheritdoc/>
        void IDictionary<string, LinkCollection>.Add(string key, LinkCollection value) => _links.Add(key, value);

        /// <inheritdoc/>
        bool IDictionary<string, LinkCollection>.Remove(string key) => _links.Remove(key);

        /// <inheritdoc/>
        bool IDictionary<string, LinkCollection>.TryGetValue(string key, out LinkCollection value) => _links.TryGetValue(key, out value);
    }
}
