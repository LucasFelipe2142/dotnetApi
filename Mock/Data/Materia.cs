using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace blazorback.Mock.Data
{
    public class Materia
    {
         public string Id { get; set; }
    public string Nome { get; set; }
    public string NomeProfessor { get; set; }
    public List<Atividade> AtividadesSemana { get; set; }
    }
}