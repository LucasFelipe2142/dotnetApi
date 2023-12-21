using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace blazorback.Data
{
    public class AtualizarKanbanRequest
    {
        public string IdMateria { get; set; }
        public string IdAtividade { get; set; }
        public int ColunaKanban { get; set; }
    }
}