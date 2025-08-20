using FI.AtividadeEntrevista.DAL;
using FI.AtividadeEntrevista.DML;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FI.AtividadeEntrevista.BLL
{
    public class BoBeneficiario
    {
        /// <summary>
        /// Inclui um novo beneficiário
        /// </summary>
        /// <param name="beneficiario">Objeto de beneficiário</param>
        public long Incluir(DML.Beneficiario beneficiario)
        {
            DaoBeneficiario dao = new DaoBeneficiario();
            return dao.Incluir(beneficiario);
        }

        /// <summary>
        /// Consulta beneficiário por ID
        /// </summary>
        /// <param name="id">ID do beneficiário</param>
        public DML.Beneficiario Consultar(long id)
        {
            DaoBeneficiario dao = new DaoBeneficiario();
            return dao.Consultar(id);
        }

        /// <summary>
        /// Lista todos os beneficiários de um cliente
        /// </summary>
        /// <param name="clienteId">ID do cliente</param>
        public List<DML.Beneficiario> ListarPorCliente(long clienteId)
        {
            DaoBeneficiario dao = new DaoBeneficiario();
            return dao.ListarPorCliente(clienteId);
        }

        /// <summary>
        /// Verifica se já existe um beneficiário com o mesmo CPF para o cliente
        /// </summary>
        /// <param name="cpf">CPF do beneficiário</param>
        /// <param name="clienteId">ID do cliente</param>
        /// <param name="beneficiarioId">ID do beneficiário atual (para exclusão na alteração)</param>
        public bool VerificarCPFDuplicado(string cpf, long clienteId, long beneficiarioId = 0)
        {
            DaoBeneficiario dao = new DaoBeneficiario();
            return dao.VerificarCPFDuplicado(cpf, clienteId, beneficiarioId);
        }

        /// <summary>
        /// Altera um beneficiário
        /// </summary>
        /// <param name="beneficiario">Objeto de beneficiário</param>
        public void Alterar(DML.Beneficiario beneficiario)
        {
            DaoBeneficiario dao = new DaoBeneficiario();
            dao.Alterar(beneficiario);
        }

        /// <summary>
        /// Exclui um beneficiário
        /// </summary>
        /// <param name="id">ID do beneficiário</param>
        public void Excluir(long id)
        {
            DaoBeneficiario dao = new DaoBeneficiario();
            dao.Excluir(id);
        }

        /// <summary>
        /// Exclui todos os beneficiários de um cliente
        /// </summary>
        /// <param name="clienteId">ID do cliente</param>
        public void ExcluirPorCliente(long clienteId)
        {
            DaoBeneficiario dao = new DaoBeneficiario();
            dao.ExcluirPorCliente(clienteId);
        }

        /// <summary>
        /// Salva a lista de beneficiários de um cliente (inclui novos, atualiza existentes e remove os excluídos)
        /// </summary>
        /// <param name="clienteId">ID do cliente</param>
        /// <param name="beneficiarios">Lista de beneficiários</param>
        public void SalvarBeneficiariosDoCliente(long clienteId, List<DML.Beneficiario> beneficiarios)
        {
            DaoBeneficiario dao = new DaoBeneficiario();
            
            // Remove todos os beneficiários existentes do cliente
            dao.ExcluirPorCliente(clienteId);
            
            // Inclui os novos beneficiários
            if (beneficiarios != null && beneficiarios.Count > 0)
            {
                foreach (var beneficiario in beneficiarios)
                {
                    beneficiario.ClienteId = clienteId;
                    dao.Incluir(beneficiario);
                }
            }
        }

        /// <summary>
        /// Valida lista de beneficiários para um cliente
        /// </summary>
        /// <param name="beneficiarios">Lista de beneficiários</param>
        /// <param name="clienteId">ID do cliente</param>
        /// <returns>Lista de erros encontrados</returns>
        public List<string> ValidarBeneficiarios(List<DML.Beneficiario> beneficiarios, long clienteId = 0)
        {
            List<string> erros = new List<string>();
            
            if (beneficiarios == null || beneficiarios.Count == 0)
            {
                return erros;
            }

            var cpfsDuplicados = beneficiarios.GroupBy(b => b.CPF)
                                            .Where(g => g.Count() > 1)
                                            .Select(g => g.Key);

            foreach (var cpfDuplicado in cpfsDuplicados)
            {
                erros.Add($"CPF {cpfDuplicado} está duplicado na lista de beneficiários.");
            }

            for (int i = 0; i < beneficiarios.Count; i++)
            {
                var beneficiario = beneficiarios[i];
                
                if (string.IsNullOrWhiteSpace(beneficiario.CPF))
                {
                    erros.Add($"CPF do beneficiário {i + 1} é obrigatório.");
                }
                else if (beneficiario.CPF.Length != 11 || !beneficiario.CPF.All(char.IsDigit))
                {
                    erros.Add($"CPF do beneficiário {i + 1} deve conter exatamente 11 dígitos numéricos.");
                }

                if (string.IsNullOrWhiteSpace(beneficiario.Nome))
                {
                    erros.Add($"Nome do beneficiário {i + 1} é obrigatório.");
                }
                else if (beneficiario.Nome.Length < 2 || beneficiario.Nome.Length > 100)
                {
                    erros.Add($"Nome do beneficiário {i + 1} deve ter entre 2 e 100 caracteres.");
                }
            }

            return erros;
        }
    }
}
