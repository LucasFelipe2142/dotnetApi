using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using blazorback.Mock.Data;
using blazorback.Mock.Interface;

namespace blazorback.Mock
{
    public class MockGoogleSalaDeAulaAPI : IGoogleSalaDeAulaAPI
{
    private readonly Dictionary<string, Materia> _materias;
    private int _contadorId;
    private int _contadorAtividade;

    public MockGoogleSalaDeAulaAPI()
    {
        _materias = new Dictionary<string, Materia>();
        _contadorId = 1;
        _contadorAtividade = 1;

        AdicionarMateria("calculo", "Professor Calculo");
        AdicionarMateria(".net", "Professor .NET");
        AdicionarMateria("gaal", "Professor GAAL");
        AdicionarMateria("discreta", "Professor Discreta");
        AdicionarMateria("aeds", "Professor AEDS");
    }

    private void AdicionarMateria(string nome, string nomeProfessor)
    {
        var novaMateria = GetMockMateria(nome, nomeProfessor);
        _materias.Add(novaMateria.Id, novaMateria);
    }

    public Task<Aluno> ObterInformacoesAlunoAsync(string email)
{
    var materiasResumidas = _materias.ToDictionary(
        entry => entry.Key,
        entry => new MateriaResumida
        {
            Id = entry.Value.Id,
            Nome = entry.Value.Nome
        }
    );

    return Task.FromResult(new Aluno
    {
        Id = "MockUserId",
        Nome = "MockUserName",
        MateriasEmCurso = materiasResumidas
    });
}

    public Task<Materia> ObterInformacoesMateriaPorIdAsync(string materiaId)
    {
        
        if (_materias.TryGetValue(materiaId, out var materia))
        {
            return Task.FromResult(materia);
        }

        return Task.FromResult<Materia>(null);
    }

    private Materia GetMockMateria(string nome, string nomeProfessor)
    {
        return new Materia
        {
            Id = (_contadorId++).ToString(),
            Nome = nome,
            NomeProfessor = nomeProfessor,
            AtividadesSemana = new List<Atividade>
            {
                GetMockAtividade("Atividade 1", "desc1", DateTime.Now.AddDays(7), 10),
                GetMockAtividade("Atividade 2", "desc2", DateTime.Now.AddDays(14), 15)
            }
        };
    }

    private Atividade GetMockAtividade(string nome,string descricao, DateTime dataEntrega, int valor)
    {
        return new Atividade
        {
            Id = (_contadorAtividade++).ToString(),
            Nome = nome,
            Descricao = descricao,
            DataEntrega = dataEntrega,
            Valor = valor
        };
    }
}

}