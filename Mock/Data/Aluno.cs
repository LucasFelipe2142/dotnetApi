using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace blazorback.Mock.Data
{
    public class Aluno
    {
        public string Id { get; set; }
    public string Nome { get; set; }
    public Dictionary<string, MateriaResumida> MateriasEmCurso { get; set; }
    }
}