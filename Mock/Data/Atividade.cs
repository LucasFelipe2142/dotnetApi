using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace blazorback.Mock.Data
{
    public class Atividade
    {
        public string Id { get; set; }
        public DateTime DataEntrega { get; set; }
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public int Valor { get; set; }
    }
}