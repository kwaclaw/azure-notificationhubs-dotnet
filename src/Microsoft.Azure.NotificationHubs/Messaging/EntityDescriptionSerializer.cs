//------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for 
// license information.
//------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;

namespace Microsoft.Azure.NotificationHubs.Messaging
{
    internal class EntityDescriptionSerializer
    {
        const int MaxItemsInObjectGraph = 256;

        readonly Dictionary<string, DataContractSerializer> entitySerializers;

        public EntityDescriptionSerializer()
        {
            this.entitySerializers = new Dictionary<string, DataContractSerializer>();
            this.entitySerializers.Add(
                typeof(RegistrationDescription).Name,
                this.CreateSerializer<RegistrationDescription>());

            this.entitySerializers.Add(
                typeof(WindowsRegistrationDescription).Name,
                this.CreateSerializer<WindowsRegistrationDescription>());

            this.entitySerializers.Add(
                typeof(WindowsTemplateRegistrationDescription).Name,
                this.CreateSerializer<WindowsTemplateRegistrationDescription>());

            this.entitySerializers.Add(
                typeof(AppleRegistrationDescription).Name,
                this.CreateSerializer<AppleRegistrationDescription>());

            this.entitySerializers.Add(
                typeof(AppleTemplateRegistrationDescription).Name,
                this.CreateSerializer<AppleTemplateRegistrationDescription>());

#pragma warning disable CS0618

            this.entitySerializers.Add(
                typeof(GcmRegistrationDescription).Name,
                this.CreateSerializer<GcmRegistrationDescription>());

#pragma warning restore CS0618

            this.entitySerializers.Add(
                typeof(FcmRegistrationDescription).Name,
                this.CreateSerializer<FcmRegistrationDescription>());

#pragma warning disable CS0618

            this.entitySerializers.Add(
                typeof(GcmTemplateRegistrationDescription).Name,
                this.CreateSerializer<GcmTemplateRegistrationDescription>());

#pragma warning restore CS0618

            this.entitySerializers.Add(
                typeof(FcmTemplateRegistrationDescription).Name,
                this.CreateSerializer<FcmTemplateRegistrationDescription>());

            this.entitySerializers.Add(
                typeof(FcmV1RegistrationDescription).Name,
                this.CreateSerializer<FcmV1RegistrationDescription>());

            this.entitySerializers.Add(
                typeof(FcmV1TemplateRegistrationDescription).Name,
                this.CreateSerializer<FcmV1TemplateRegistrationDescription>());

            this.entitySerializers.Add(
                typeof(MpnsRegistrationDescription).Name,
                this.CreateSerializer<MpnsRegistrationDescription>());

            this.entitySerializers.Add(
                typeof(MpnsTemplateRegistrationDescription).Name,
                this.CreateSerializer<MpnsTemplateRegistrationDescription>());

            this.entitySerializers.Add(
                typeof(AdmRegistrationDescription).Name,
                this.CreateSerializer<AdmRegistrationDescription>());

            this.entitySerializers.Add(
                typeof(AdmTemplateRegistrationDescription).Name,
                this.CreateSerializer<AdmTemplateRegistrationDescription>());

            this.entitySerializers.Add(
                typeof(BaiduRegistrationDescription).Name,
                this.CreateSerializer<BaiduRegistrationDescription>());

            this.entitySerializers.Add(
                typeof(BaiduTemplateRegistrationDescription).Name,
                this.CreateSerializer<BaiduTemplateRegistrationDescription>());

            this.entitySerializers.Add(
                typeof(NotificationHubJob).Name,
                this.CreateSerializer<NotificationHubJob>());

            this.entitySerializers.Add(
                typeof(BrowserRegistrationDescription).Name,
                this.CreateSerializer<BrowserRegistrationDescription>());
        }

        private DataContractSerializer CreateSerializer<T>()
        {
            return new DataContractSerializer(typeof(T), new DataContractSerializerSettings
            {
                MaxItemsInObjectGraph = MaxItemsInObjectGraph
            });
        }

        public EntityDescription Deserialize(XmlReader reader, string typeName)
        {
            if (reader == null)
            {
                throw new ArgumentNullException(nameof(reader));
            }
            if (string.IsNullOrEmpty(typeName))
            {
                throw new ArgumentNullException(nameof(typeName));
            }

            var serializer = GetSerializer(typeName);

            return (EntityDescription)serializer.ReadObject(reader);
        }

        public string Serialize(EntityDescription description)
        {
            var stringBuilder = new StringBuilder();
            var settings = new XmlWriterSettings
            {
                OmitXmlDeclaration = true
            };

            using (var xmlWriter = XmlWriter.Create(stringBuilder, settings))
            {
                Serialize(description, xmlWriter);
            }

            return stringBuilder.ToString();
        }

#pragma warning disable CS0618

        public void Serialize(EntityDescription description, XmlWriter writer)
        {
            // Convert FCM descriptions into their GCM counterparts
            if (description.GetType().Name == "FcmRegistrationDescription")
            {
                description = new GcmRegistrationDescription((FcmRegistrationDescription) description);
            }

            if (description.GetType().Name == "FcmTemplateRegistrationDescription")
            {
                description = new GcmTemplateRegistrationDescription((FcmTemplateRegistrationDescription) description);
            }

            DataContractSerializer serializer;
            if (description is RegistrationDescription)
            {
                serializer = GetSerializer(typeof(RegistrationDescription).Name);
            }
            else
            {
                serializer = GetSerializer(description.GetType().Name);
            }
            
            serializer.WriteObject(writer, description);
        }

#pragma warning restore CS0618

        private DataContractSerializer GetSerializer(string typeName)
        {
            if (this.entitySerializers.TryGetValue(typeName, out var serializer))
            {
                return serializer;
            }
            else
            {
                throw new InvalidOperationException($"Unknown entity type {typeName}");
            }
        }
    }
}
