// <copyright file="EmbeddedDictionary.cs" company="Cimpress, Inc.">
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
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using static System.Globalization.CultureInfo;
using static Tiger.Hal.Client.Resources;

namespace Tiger.Hal.Client
{
    /// <summary>Represents the map of relations pre-cached values from this entity to others.</summary>
    public sealed class EmbeddedDictionary
        : IDictionary<string, JToken>, IReadOnlyDictionary<string, JToken>
    {
        static readonly JsonSerializer s_defaultSerializer = JsonSerializer.CreateDefault();

        readonly IDictionary<string, JToken> _embedded = new Dictionary<string, JToken>();

        /// <inheritdoc/>
        public int Count => _embedded.Count;

        /// <inheritdoc/>
        [NotNull, ItemNotNull]
        public IEnumerable<string> Keys => _embedded.Keys;

        /// <inheritdoc/>
        [NotNull, ItemNotNull]
        public IEnumerable<JToken> Values => _embedded.Values;

        /// <inheritdoc/>
        bool ICollection<KeyValuePair<string, JToken>>.IsReadOnly => _embedded.IsReadOnly;

        /// <inheritdoc/>
        ICollection<string> IDictionary<string, JToken>.Keys => _embedded.Keys;

        /// <inheritdoc/>
        ICollection<JToken> IDictionary<string, JToken>.Values => _embedded.Values;

        /// <inheritdoc/>
        [NotNull]
        public JToken this[[NotNull] string key] => _embedded[key];

        /// <inheritdoc/>
        JToken IDictionary<string, JToken>.this[string key]
        {
            get => _embedded[key];
            set => _embedded[key] = value;
        }

        /// <summary>
        /// Gets the embed associated with the specified relation
        /// using conversion rules from the default JSON serializer settings.
        /// </summary>
        /// <typeparam name="TEmbed">The type to which to convert the embed.</typeparam>
        /// <param name="rel">The relation whose embed to get.</param>
        /// <returns>The embed with the specified rel converted to a value of type <typeparamref name="TEmbed"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="rel"/> is <see langword="null"/>.</exception>
        /// <exception cref="KeyNotFoundException">Value for link relation <paramref name="rel"/> is not found.</exception>
        /// <exception cref="InvalidOperationException">
        /// Value for link relation <paramref name="rel"/> cannot be converted to a value of type <typeparamref name="TEmbed"/>.
        /// </exception>
        public TEmbed Get<TEmbed>([NotNull] string rel)
        {
            if (rel is null) { throw new ArgumentNullException(nameof(rel)); }

            return GetEmbed<TEmbed>(rel, s_defaultSerializer);
        }

        /// <summary>
        /// Gets the embed associated with the specified relation
        /// using conversion rules from the provided JSON serializer settings.
        /// </summary>
        /// <typeparam name="TEmbed">The type to which to convert the embed.</typeparam>
        /// <param name="rel">The relation whose embed to get.</param>
        /// <param name="settings">
        /// Settings to control the conversion of the embed to a value of type <typeparamref name="TEmbed"/>.
        /// If this value is <see langword="null"/>, the default JSON serializer settings will be used.
        /// </param>
        /// <returns>The embed with the specified rel converted to a value of type <typeparamref name="TEmbed"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="rel"/> is <see langword="null"/>.</exception>
        /// <exception cref="KeyNotFoundException">Value for link relation <paramref name="rel"/> is not found.</exception>
        /// <exception cref="InvalidOperationException">
        /// Value for link relation <paramref name="rel"/> cannot be converted to a value of type <typeparamref name="TEmbed"/>.
        /// </exception>
        public TEmbed Get<TEmbed>([NotNull] string rel, [CanBeNull] JsonSerializerSettings settings)
        {
            if (rel is null) { throw new ArgumentNullException(nameof(rel)); }

            return GetEmbed<TEmbed>(rel, JsonSerializer.CreateDefault(settings));
        }

        /// <summary>
        /// Gets the embed associated with the specified relation
        /// using conversion rules from the default JSON serializer settings.
        /// </summary>
        /// <typeparam name="TEmbed">The type to which to convert the embed.</typeparam>
        /// <param name="rel">The relation whose embed to get.</param>
        /// <param name="embed">
        /// When this method returns, the embed associated with the specified rel if the rel is found
        /// and if the embed can be converted to a value of type <typeparamref name="TEmbed"/>;
        /// otherwise, the default value of the embed. This parameter is passed uninitialized.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if this instance contains a value with the specified rel
        /// which is convertible to a value of type <typeparamref name="TEmbed"/>;
        /// otherwise, <see langword="false"/>.
        /// </returns>
        public bool TryGet<TEmbed>([NotNull] string rel, out TEmbed embed)
        {
            if (rel is null) { throw new ArgumentNullException(nameof(rel)); }

            return TryGetEmbed(rel, s_defaultSerializer, out embed);
        }

        /// <summary>
        /// Gets the embed associated with the specified relation
        /// using conversion rules from the provided JSON serializer settings.
        /// </summary>
        /// <typeparam name="TEmbed">The type to which to convert the embed.</typeparam>
        /// <param name="rel">The relation whose embed to get.</param>
        /// <param name="settings">
        /// Settings to control the conversion of the embed to a value of type <typeparamref name="TEmbed"/>.
        /// If this value is <see langword="null"/>, the default JSON serializer settings will be used.
        /// </param>
        /// <param name="embed">
        /// When this method returns, the embed associated with the specified rel if the rel is found
        /// and if the embed can be converted to a value of type <typeparamref name="TEmbed"/>;
        /// otherwise, the default value of the embed. This parameter is passed uninitialized.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if this instance contains a value with the specified rel
        /// which is convertible to a value of type <typeparamref name="TEmbed"/>;
        /// otherwise, <see langword="false"/>.
        /// </returns>
        public bool TryGet<TEmbed>([NotNull] string rel, [CanBeNull] JsonSerializerSettings settings, out TEmbed embed)
        {
            if (rel is null) { throw new ArgumentNullException(nameof(rel)); }

            return TryGetEmbed(rel, JsonSerializer.CreateDefault(settings), out embed);
        }

        /// <inheritdoc/>
        public bool ContainsKey(string key) => _embedded.ContainsKey(key);

        /// <inheritdoc/>
        public bool TryGetValue(string key, out JToken value) => _embedded.TryGetValue(key, out value);

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_embedded).GetEnumerator();

        /// <inheritdoc/>
        IEnumerator<KeyValuePair<string, JToken>> IEnumerable<KeyValuePair<string, JToken>>.GetEnumerator() =>
            _embedded.GetEnumerator();

        /// <inheritdoc/>
        void ICollection<KeyValuePair<string, JToken>>.Add(KeyValuePair<string, JToken> item) => _embedded.Add(item);

        /// <inheritdoc/>
        void ICollection<KeyValuePair<string, JToken>>.Clear() => _embedded.Clear();

        /// <inheritdoc/>
        bool ICollection<KeyValuePair<string, JToken>>.Contains(KeyValuePair<string, JToken> item) =>
            _embedded.Contains(item);

        /// <inheritdoc/>
        void ICollection<KeyValuePair<string, JToken>>.CopyTo(KeyValuePair<string, JToken>[] array, int arrayIndex) =>
            _embedded.CopyTo(array, arrayIndex);

        /// <inheritdoc/>
        bool IDictionary<string, JToken>.Remove(string key) => _embedded.Remove(key);

        /// <inheritdoc/>
        void IDictionary<string, JToken>.Add(string key, JToken value) => _embedded.Add(key, value);

        /// <inheritdoc/>
        bool ICollection<KeyValuePair<string, JToken>>.Remove(KeyValuePair<string, JToken> item) => _embedded.Remove(item);

        TEmbed GetEmbed<TEmbed>([NotNull] string rel, [NotNull] JsonSerializer jsonSerializer)
        {
            try
            {
                return _embedded[rel].ToObject<TEmbed>(jsonSerializer);
            }
            catch (JsonSerializationException jse)
            {
                throw new InvalidOperationException(string.Format(InvariantCulture, CannotConvert, rel, typeof(TEmbed)), jse);
            }
            catch (JsonReaderException jre)
            {
                throw new InvalidOperationException(string.Format(InvariantCulture, CannotConvert, rel, typeof(TEmbed)), jre);
            }
        }

        bool TryGetEmbed<TEmbed>([NotNull] string rel, [NotNull] JsonSerializer serializer, out TEmbed embed)
        {
            if (!_embedded.TryGetValue(rel, out var token))
            {
                embed = default;
                return false;
            }

            try
            {
                embed = token.ToObject<TEmbed>(serializer);
            }
            catch (JsonSerializationException)
            {
                embed = default;
                return false;
            }
            catch (JsonReaderException)
            {
                embed = default;
                return false;
            }

            return true;
        }
    }
}
