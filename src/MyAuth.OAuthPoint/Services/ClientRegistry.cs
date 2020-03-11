using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace MyAuth.OAuthPoint.Services
{
    public interface IClientRegistry
    {
        ClientEntry GetClient(string id);
    }

    public class ClientEntry
    {
        public string Id { get; set; }
        public bool Verification { get; set; }
        public string[] AllowUris { get; set; }
    }

    public class ClientListOptions
    {
        public ClientEntry[] List { get; set; }
    }
    
    class DefaultClientRegistry : IClientRegistry
    {
        private readonly Dictionary<string, ClientEntry> _clients;

        public DefaultClientRegistry(IOptions<ClientListOptions> options)
            :this(options?.Value?.List)
        {

        }

        public DefaultClientRegistry(ClientEntry[] clients)
        {
            _clients = clients != null
                ? clients.ToDictionary(c => c.Id, c => c)
                : new Dictionary<string, ClientEntry>();
        }
        
        public ClientEntry GetClient(string id)
        {
            return _clients.TryGetValue(id, out var cl)
                ? cl
                : null;
        }

        public static DefaultClientRegistry LoadFromJson(string json)
        {
            var clients = JsonConvert.DeserializeObject<ClientEntry[]>(json);
            
            return new DefaultClientRegistry(clients);
        }
    }
    
    
}