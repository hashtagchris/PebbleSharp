using System.Linq;

namespace PebbleSharp.Core.Responses
{
    [Endpoint(Endpoint.AppRunState)]
    public class AppRunStateResponse : ResponseBase
    {
        public byte Command { get; set; }
        public UUID UUID { get; set; }

        protected override void Load(byte[] payload)
        {
            Command = payload[0];
            UUID = new UUID(payload.Skip(1).Take(16).ToArray());
        }
    }
}