using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace blazorback.Data
{
    public class RelatorioAlunoMateriasAtividades
    {
        public string IdAluno { get; set; }
        public string NomeAluno { get; set; }
        public List<RelatorioMateriaDetalhada> Materias { get; set; }
    }
}