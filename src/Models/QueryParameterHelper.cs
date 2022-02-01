using System.Collections.Generic;

namespace StreamChat.Models
{
    public interface IQueryParameterConvertible
    {
        List<KeyValuePair<string, string>> ToQueryParameters();
    }
}