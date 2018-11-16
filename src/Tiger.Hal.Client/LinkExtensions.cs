// <copyright file="LinkExtensions.cs" company="Cimpress, Inc.">
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
using JetBrains.Annotations;
using Tavis.UriTemplates;
using static System.UriKind;

namespace Tiger.Hal.Client
{
    /// <summary>Extensions to the functionality of the <see cref="Link"/> class.</summary>
    public static class LinkExtensions
    {
        /// <summary>
        /// Resolves the href of a link to a <see cref="Uri"/>,
        /// irrespective of whether it is templated.
        /// </summary>
        /// <param name="link">The link whose href to resolve.</param>
        /// <param name="name">The name of the key whose value will resolve to <paramref name="value"/>.</param>
        /// <param name="value">The value to which to resolve the key named by <paramref name="name"/>.</param>
        /// <returns>The resolved <see cref="Uri"/>.</returns>
        public static Uri ResolveHref(this Link link, string name, object value) => link.IsTemplated
            ? new Uri(link.TemplatedHref.AddParameter(name, value).Resolve(), RelativeOrAbsolute)
            : link.Href;

        /// <summary>
        /// Resolves the href of a link to a <see cref="Uri"/>,
        /// irrespective of whether it is templated.
        /// </summary>
        /// <param name="link">The link whose href to resolve.</param>
        /// <param name="parameters">
        /// A key–value mapping used to resolve the href if it is templated.
        /// This value is usually passed as an object of anonymous type.
        /// </param>
        /// <returns>The resolved <see cref="Uri"/>.</returns>
        public static Uri ResolveHref(this Link link, [NotNull] object parameters) => link.IsTemplated
            ? new Uri(link.TemplatedHref.AddParameters(parameters).Resolve(), RelativeOrAbsolute)
            : link.Href;

        /// <summary>
        /// Resolves the href of a link to a <see cref="Uri"/>,
        /// irrespective of whether it is templated.
        /// </summary>
        /// <param name="link">The link whose href to resolve.</param>
        /// <param name="parameters">
        /// A key–value mapping used to resolve the href if it is templated.
        /// </param>
        /// <returns>The resolved <see cref="Uri"/>.</returns>
        public static Uri ResolveHref(this Link link, [NotNull] IDictionary<string, object> parameters) => link.IsTemplated
            ? new Uri(link.TemplatedHref.AddParameters(parameters).Resolve(), RelativeOrAbsolute)
            : link.Href;

        /// <summary>
        /// Converts this instance into a link which is unconditionally untemplated.
        /// </summary>
        /// <param name="link">The link to convert.</param>
        /// <returns>A self link.</returns>
        [NotNull]
        internal static SelfLink ToSelfLink([NotNull] this Link link)
        {
            if (link is null) { throw new ArgumentNullException(nameof(link)); }

            return new SelfLink(
                href: link.Href,
                type: link.Type,
                deprecation: link.Deprecation,
                name: link.Name,
                profile: link.Profile,
                title: link.Title,
                hrefLang: link.HrefLang);
        }
    }
}
