﻿using System;

namespace PebbleSharp.Core.Responses
{
    [AttributeUsage( AttributeTargets.Class, AllowMultiple = true )]
    internal class EndpointAttribute : Attribute
    {
        public EndpointAttribute()
        {
        }

        public EndpointAttribute( Endpoint endpoint )
        {
            Endpoint = endpoint;
        }

        public EndpointAttribute( Endpoint endpoint, byte payloadType )
        {
            Endpoint = endpoint;
            PayloadType = payloadType;
        }

        public Endpoint Endpoint { get; set; }
        public byte? PayloadType { get; set; }

        public Func<byte[], bool> GetPredicate()
        {
            if (PayloadType != null)
                return bytes => bytes != null && bytes.Length > 0 && bytes[0] == PayloadType;
            return null;
        }
    }
}