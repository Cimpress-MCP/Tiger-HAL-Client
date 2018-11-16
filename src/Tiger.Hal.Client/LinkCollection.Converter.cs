// <copyright file="LinkCollection.Converter.cs" company="Cimpress, Inc.">
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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using static Tiger.Hal.Client.Cardinality;

namespace Tiger.Hal.Client
{
    /// <summary>Serialization control.</summary>
    public sealed partial class LinkCollection
    {
        /// <summary>Controls JSON serialization of the <see cref="LinkCollection"/> class.</summary>
        sealed class Converter
            : JsonConverter<LinkCollection>
        {
            const string Message = "The provided value could not be converted into a HAL link or a collection of HAL links.";

            /// <inheritdoc/>
            public override void WriteJson(
                JsonWriter writer,
                [CanBeNull] LinkCollection value,
                [NotNull] JsonSerializer serializer)
            {
                switch (value.Cardinality)
                {
                    case Singular:
                        serializer.Serialize(writer, value.Items[0], typeof(Link));
                        return;
                    case Plural:
                        serializer.Serialize(writer, value.Items, typeof(IList<Link>));
                        return;
                }

                throw new JsonSerializationException("A catastropic error has occurred.");
            }

            /// <inheritdoc/>
            public override LinkCollection ReadJson(
                [NotNull] JsonReader reader,
                [NotNull] Type objectType,
                [CanBeNull] LinkCollection existingValue,
                bool hasExistingValue,
                [NotNull] JsonSerializer serializer)
            {
                switch (reader.TokenType)
                {
                    case JsonToken.StartArray:
                        var links = JArray.Load(reader).ToObject<List<Link>>(serializer);
                        return new LinkCollection(links);
                    case JsonToken.StartObject:
                        var link = JObject.Load(reader).ToObject<Link>(serializer);
                        return new LinkCollection(link);
                }

                throw new JsonSerializationException(Message)
                {
                    Data =
                    {
                        ["Value"] = reader.Value
                    }
                };
            }
        }
    }
}
