using System.Collections.Generic;
using System.Linq;
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
    
    class DefaultClientRegistry : IClientRegistry
    {
        private readonly Dictionary<string, ClientEntry> _clients;

        public DefaultClientRegistry(ClientEntry[] clients)
        {
            _clients = clients.ToDictionary(c => c.Id, c => c);
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