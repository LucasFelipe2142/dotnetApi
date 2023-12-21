using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using blazorback.Data;
using blazorback.Mock;
using blazorback.Mock.Data;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System.Text;

namespace blazorback.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MateriasController : ControllerBase
    {   
        private readonly ILogger<MateriasController> _logger;

        public MateriasController(MongoDBContext mongoDBContext, ILogger<MateriasController> logger)
        {
            _mongoDBContext = mongoDBContext;
            _logger = logger;
            _logger.LogInformation("MateriasController foi inicializado.");
        }

        private readonly MongoDBContext _mongoDBContext;
        MockGoogleSalaDeAulaAPI _googleSalaDeAulaAPI = new MockGoogleSalaDeAulaAPI();

        [HttpGet("BuscarMateriasPorEmail")]
        public async Task<IActionResult> BuscarMateriasPorEmail([FromQuery(Name = "emailAluno")] string emailAluno)
        {
            if (string.IsNullOrEmpty(emailAluno))
            {
                return BadRequest("O parâmetro 'emailAluno' é obrigatório.");
            }

            var materias = await _googleSalaDeAulaAPI.ObterInformacoesAlunoAsync(emailAluno);

            if (materias != null)
            {
                return Ok(materias.MateriasEmCurso);
            }
            else
            {
                return NotFound();
            }
        }


        [HttpGet("BuscarAtividadesPorIdAluno")]
        public IActionResult BuscarAtividadesPorIdAluno(int idAluno)
        {
            return Ok();
        }

        [HttpPost("atividades")]
        public async Task<IActionResult> InserirAtividadesMateria([FromBody] IdMateriaRequest request)
        {
            try
            {
                if (request == null || string.IsNullOrEmpty(request.IdMateria))
                {
                    return BadRequest("O campo 'idMateria' é obrigatório no corpo da solicitação.");
                }

                var materia = await ObterMateriaPorIdAsync(request.IdMateria);

                if (materia == null)
                {
                    return NotFound("Matéria não encontrada");
                }

                var atividadesInseridas = new List<AtividadeBD>();

                foreach (var atividade in materia.AtividadesSemana)
                {
                    var atividadeExistente = await _mongoDBContext.Atividades.Find(a => a.Id == atividade.Id).FirstOrDefaultAsync();

                    if (atividadeExistente == null)
                    {
                        var atividadeBD = new AtividadeBD
                        {
                            Id = atividade.Id,
                            Nome = atividade.Nome,
                            Descricao = atividade.Descricao,
                            DataEntrega = atividade.DataEntrega,
                            Valor = atividade.Valor,
                            ColunaKanban = 10,
                            IdMateria = request.IdMateria
                        };

                        await _mongoDBContext.Atividades.InsertOneAsync(atividadeBD);
                        atividadesInseridas.Add(atividadeBD);
                        _logger.LogInformation("Atividade inserida no banco: {NomeAtividade}", atividade.Nome);
                    }
                    else
                    {
                        _logger.LogInformation("Atividade com ID {IdAtividade} já existe no banco. Não foi reinserida.", atividade.Id);
                    }
                }

                var todasAtividades = await _mongoDBContext.Atividades.Find(a => a.IdMateria == request.IdMateria).ToListAsync();

                return Ok(todasAtividades);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao inserir atividades: {ex.Message}");
                return BadRequest($"Erro ao inserir atividades: {ex.Message}");
            }
        }

        public class IdMateriaRequest
        {
            public string IdMateria { get; set; }
        }

        private async Task<Materia> ObterMateriaPorIdAsync(string idMateria)
        {
            var materia = await _googleSalaDeAulaAPI.ObterInformacoesMateriaPorIdAsync(idMateria);
            return materia;
        }

        [HttpPut("atualizar-kanban")]
    public async Task<IActionResult> AtualizarKanbanAtividade([FromBody] AtualizarKanbanRequest request)
    {
        try
        {
            if (request == null || string.IsNullOrEmpty(request.IdMateria) || string.IsNullOrEmpty(request.IdAtividade))
            {
                return BadRequest("Os campos 'idMateria' e 'idAtividade' são obrigatórios no corpo da solicitação.");
            }

            var atividadeExistente = await _mongoDBContext.Atividades
                .Find(a => a.IdMateria == request.IdMateria && a.Id == request.IdAtividade)
                .FirstOrDefaultAsync();

            if (atividadeExistente == null)
            {
                return NotFound("Atividade não encontrada no banco de dados.");
            }

            atividadeExistente.ColunaKanban = request.ColunaKanban;

            var result = await _mongoDBContext.Atividades.ReplaceOneAsync(
                a => a.IdMateria == request.IdMateria && a.Id == request.IdAtividade,
                atividadeExistente
            );

            if (result.IsAcknowledged && result.ModifiedCount > 0)
            {
                _logger.LogInformation("Coluna Kanban da atividade atualizada com sucesso.");
                return Ok("Coluna Kanban da atividade atualizada com sucesso.");
            }
            else
            {
                _logger.LogError("Falha ao atualizar a coluna Kanban da atividade.");
                return StatusCode(500, "Falha ao atualizar a coluna Kanban da atividade.");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Erro ao atualizar a coluna Kanban da atividade: {ex.Message}");
            return BadRequest($"Erro ao atualizar a coluna Kanban da atividade: {ex.Message}");
        }
    }

    [HttpGet("relatorio")]
    public async Task<RelatorioAlunoMateriasAtividades> ObterRelatorioAlunoMateriasAtividades([FromQuery]string emailAluno)
{
    var aluno = await _googleSalaDeAulaAPI.ObterInformacoesAlunoAsync(emailAluno);

    if (aluno == null || aluno.MateriasEmCurso == null || aluno.MateriasEmCurso.Count == 0)
    {
        return null;
    }

    var relatorio = new RelatorioAlunoMateriasAtividades
    {
        IdAluno = aluno.Id,
        NomeAluno = aluno.Nome,
        Materias = new List<RelatorioMateriaDetalhada>()
    };

    foreach (var materiaResumida in aluno.MateriasEmCurso.Values)
    {
        var materiaDetalhada = await _googleSalaDeAulaAPI.ObterInformacoesMateriaPorIdAsync(materiaResumida.Id);
        if (materiaDetalhada != null)
        {
            var relatorioMateria = new RelatorioMateriaDetalhada
            {
                IdMateria = materiaDetalhada.Id,
                NomeMateria = materiaDetalhada.Nome,
                NomeProfessor = materiaDetalhada.NomeProfessor,
                Atividades = materiaDetalhada.AtividadesSemana.Select(a => new RelatorioAtividade
                {
                    IdAtividade = a.Id,
                    NomeAtividade = a.Nome,
                    DescricaoAtividade = a.Descricao,
                    DataEntregaAtividade = a.DataEntrega,
                    ValorAtividade = a.Valor
                }).ToList()
            };

            relatorio.Materias.Add(relatorioMateria);
        }
    }

    return relatorio;
}


    }


}
