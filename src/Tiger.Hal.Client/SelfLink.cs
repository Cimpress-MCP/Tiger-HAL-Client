// <copyright file="SelfLink.cs" company="Cimpress, Inc.">
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
using JetBrains.Annotations;

namespace Tiger.Hal.Client
{
    /// <summary>Represents the identifier of the context.</summary>
    public sealed class SelfLink
        : LinkBase
    {
        /// <summary>Initializes a new instance of the <see cref="SelfLink"/> class.</summary>
        /// <param name="href">The location of the target resource.</param>
        /// <param name="type">
        /// A hint to indicate the media type expected when dereferencing the target resource.
        /// </param>
        /// <param name="deprecation">
        /// Whether the link has been deprecated – that is,
        /// whether it will be removed at a future date.
        /// </param>
        /// <param name="name">
        /// A secondary key for selecting links which share the same relation type.
        /// </param>
        /// <param name="profile">A hint about the profile of the target resource.</param>
        /// <param name="title">A human-readable identifier for the link.</param>
        /// <param name="hrefLang">The language of the target resource.</param>
        public SelfLink(
            [NotNull] Uri href,
            [CanBeNull] string type,
            [CanBeNull] Uri deprecation,
            [CanBeNull] string name,
            [CanBeNull] Uri profile,
            [CanBeNull] string title,
            [CanBeNull] string hrefLang)
            : base(false, type, deprecation, name, profile, title, hrefLang)
        {
            Href = href;
        }

        /// <summary>Gets the location of the target resource.</summary>
        public Uri Href { get; }
    }
}
