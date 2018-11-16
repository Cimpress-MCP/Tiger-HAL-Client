// <copyright file="LinkCollection.cs" company="Cimpress, Inc.">
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
using System.Collections.ObjectModel;
using JetBrains.Annotations;
using Newtonsoft.Json;
using static Tiger.Hal.Client.Cardinality;

namespace Tiger.Hal.Client
{
    /// <summary>Represents a collection of links, with possible special handling for collections of one.</summary>
    [JsonConverter(typeof(Converter))]
    public sealed partial class LinkCollection
        : ReadOnlyCollection<Link>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LinkCollection"/> class
        /// that is a read-only wrapper around a singleton list containing the specified value.
        /// </summary>
        /// <param name="link">The value to wrap.</param>
        /// <exception cref="ArgumentNullException"><paramref name="link"/> is <see langword="null"/>.</exception>
        public LinkCollection(Link link)
            : base(new[] { link })
        {
            if (link is null) { throw new ArgumentNullException(nameof(link)); }

            Cardinality = Singular;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LinkCollection"/> class
        /// that is a read-only wrapper around the specified list.
        /// </summary>
        /// <param name="links">The list to wrap.</param>
        /// <exception cref="ArgumentNullException"><paramref name="links"/> is <see langword="null"/>.</exception>
        public LinkCollection([NotNull] IList<Link> links)
            : base(links)
        {
            Cardinality = Plural;
        }

        /// <summary>
        /// Gets a value indicating whether this collection represents
        /// a single item rather than a collection of one.
        /// </summary>
        public Cardinality Cardinality { get; }
    }
}
