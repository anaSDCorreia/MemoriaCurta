using MemoriaCurtaAPI.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MemoriaCurtaAPI.Services
{
    public interface IClassifierService
    {
        Task Inicializer();
        Task Inicializer(string modelPath);

        Task<List<MCQuote>> GetQuotes(string data);

    }
}
