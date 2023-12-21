using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace blazorback.Data
{
    public class AtividadeBD
    {
        public string Id { get; set; }
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public DateTime DataEntrega { get; set; }
        public int Valor { get; set; }
        public int ColunaKanban { get; set; }
        public string IdMateria { get; set; }
    }
}