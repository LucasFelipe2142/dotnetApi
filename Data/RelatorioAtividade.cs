using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace blazorback.Data
{
    public class RelatorioAtividade
    {
        public string IdAtividade { get; set; }
        public string NomeAtividade { get; set; }
        public string DescricaoAtividade { get; set; }
        public DateTime DataEntregaAtividade { get; set; }
        public int ValorAtividade { get; set; }
    }
}