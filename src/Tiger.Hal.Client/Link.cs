// <copyright file="Link.cs" company="Cimpress, Inc.">
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
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Tavis.UriTemplates;
using static System.UriKind;
using static Tiger.Hal.Client.Resources;

namespace Tiger.Hal.Client
{
    /// <summary>
    /// Represents a relationship between resources irrespective
    /// of whether that link is templated.
    /// </summary>
    public sealed class Link
        : LinkBase
    {
        readonly string _rawHref;
        readonly Lazy<Uri> _href;

        /// <summary>Initializes a new instance of the <see cref="Link"/> class.</summary>
        /// <param name="href">The location of the target resource.</param>
        /// <param name="isTemplated">
        /// A value indicating whether <paramref name="href"/> is a template which,
        /// when bound, will be the location of the location of the target resource.
        /// </param>
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
        public Link(
            [CanBeNull] string href,
            [JsonProperty("templated")] bool isTemplated,
            string type = null,
            Uri deprecation = null,
            string name = null,
            Uri profile = null,
            string title = null,
            [JsonProperty("hreflang")] string hrefLang = null)
            : base(isTemplated, type, deprecation, name, profile, title, hrefLang)
        {
            _rawHref = href;
            _href = new Lazy<Uri>(() =>
            {
                if (IsTemplated) { throw new NotSupportedException(LinkIsTemplated); }

                return new Uri(_rawHref, RelativeOrAbsolute);
            });
        }

        /// <summary>Gets the location of the target resource as a <see cref="Uri"/>.</summary>
        /// <exception cref="NotSupportedException"><see cref="LinkBase.IsTemplated"/> is <see langword="true"/>.</exception>
        public Uri Href => _href.Value;

        /// <summary>Gets the location of the target resource as a <see cref="UriTemplate"/>.</summary>
        /// <exception cref="NotSupportedException"><see cref="LinkBase.IsTemplated"/> is <see langword="false"/>.</exception>
        public UriTemplate TemplatedHref
        {
            get
            {
                /* note(cosborn)
                 * This isn't parallel to Href, but for a reasonable reason.
                 * UriTemplate isn't immutable, so returning a precalculated or cached
                 * instance can make life difficult for clients. This is a leeeetle bit
                 * of a performance hit, but I think it's worth it.
                 * Imagine an immutable UriTemplate.
                 */
                if (!IsTemplated) { throw new NotSupportedException(LinkIsNotTemplated); }

                return new UriTemplate(_rawHref);
            }
        }
    }
}
