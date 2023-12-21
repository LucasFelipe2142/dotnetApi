using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using blazorback.Mock.Data;

namespace blazorback.Mock.Interface
{
    public interface IGoogleSalaDeAulaAPI
{
    public Task<Aluno> ObterInformacoesAlunoAsync(string email);
    Task<Materia> ObterInformacoesMateriaPorIdAsync(string materiaId);
}
}