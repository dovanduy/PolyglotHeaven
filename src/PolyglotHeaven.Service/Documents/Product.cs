using System;
using Nest;

namespace PolyglotHeaven.Service.Documents
{
    public class Product
    {
        [ElasticProperty(Index = FieldIndexOption.NotAnalyzed)]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int Price { get; set; }
    }
}