using FI.AtividadeEntrevista.DML;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Data.SqlClient;

namespace FI.AtividadeEntrevista.DAL
{
    /// <summary>
    /// Classe de acesso a dados de Beneficiário
    /// </summary>
    internal class DaoBeneficiario : AcessoDados
    {
        /// <summary>
        /// Inclui um novo beneficiário
        /// </summary>
        /// <param name="beneficiario">Objeto de beneficiário</param>
        internal long Incluir(DML.Beneficiario beneficiario)
        {
            List<System.Data.SqlClient.SqlParameter> parametros = new List<System.Data.SqlClient.SqlParameter>();

            parametros.Add(new System.Data.SqlClient.SqlParameter("ClienteId", beneficiario.ClienteId));
            parametros.Add(new System.Data.SqlClient.SqlParameter("CPF", beneficiario.CPF));
            parametros.Add(new System.Data.SqlClient.SqlParameter("Nome", beneficiario.Nome));

            DataSet ds = base.Consultar("FI_SP_IncBeneficiario", parametros);
            long ret = 0;
            if (ds.Tables[0].Rows.Count > 0)
                long.TryParse(ds.Tables[0].Rows[0][0].ToString(), out ret);
            return ret;
        }

        /// <summary>
        /// Consulta beneficiário por ID
        /// </summary>
        /// <param name="Id">ID do beneficiário</param>
        internal DML.Beneficiario Consultar(long Id)
        {
            List<System.Data.SqlClient.SqlParameter> parametros = new List<System.Data.SqlClient.SqlParameter>();

            parametros.Add(new System.Data.SqlClient.SqlParameter("Id", Id));

            DataSet ds = base.Consultar("FI_SP_ConsBeneficiario", parametros);
            List<DML.Beneficiario> beneficiarios = Converter(ds);

            return beneficiarios.FirstOrDefault();
        }

        /// <summary>
        /// Lista todos os beneficiários de um cliente
        /// </summary>
        /// <param name="clienteId">ID do cliente</param>
        internal List<DML.Beneficiario> ListarPorCliente(long clienteId)
        {
            List<System.Data.SqlClient.SqlParameter> parametros = new List<System.Data.SqlClient.SqlParameter>();

            parametros.Add(new System.Data.SqlClient.SqlParameter("ClienteId", clienteId));

            DataSet ds = base.Consultar("FI_SP_ListBeneficiariosPorCliente", parametros);
            List<DML.Beneficiario> beneficiarios = Converter(ds);

            return beneficiarios;
        }

        /// <summary>
        /// Verifica se já existe um beneficiário com o mesmo CPF para o cliente
        /// </summary>
        /// <param name="cpf">CPF do beneficiário</param>
        /// <param name="clienteId">ID do cliente</param>
        /// <param name="beneficiarioId">ID do beneficiário atual (para exclusão na alteração)</param>
        internal bool VerificarCPFDuplicado(string cpf, long clienteId, long beneficiarioId = 0)
        {
            List<System.Data.SqlClient.SqlParameter> parametros = new List<System.Data.SqlClient.SqlParameter>();

            parametros.Add(new System.Data.SqlClient.SqlParameter("CPF", cpf));
            parametros.Add(new System.Data.SqlClient.SqlParameter("ClienteId", clienteId));
            parametros.Add(new System.Data.SqlClient.SqlParameter("BeneficiarioId", beneficiarioId));

            DataSet ds = base.Consultar("FI_SP_VerificaCPFBeneficiario", parametros);

            return ds.Tables[0].Rows.Count > 0;
        }

        /// <summary>
        /// Altera um beneficiário
        /// </summary>
        /// <param name="beneficiario">Objeto de beneficiário</param>
        internal void Alterar(DML.Beneficiario beneficiario)
        {
            List<System.Data.SqlClient.SqlParameter> parametros = new List<System.Data.SqlClient.SqlParameter>();

            parametros.Add(new System.Data.SqlClient.SqlParameter("Id", beneficiario.Id));
            parametros.Add(new System.Data.SqlClient.SqlParameter("ClienteId", beneficiario.ClienteId));
            parametros.Add(new System.Data.SqlClient.SqlParameter("CPF", beneficiario.CPF));
            parametros.Add(new System.Data.SqlClient.SqlParameter("Nome", beneficiario.Nome));

            base.Executar("FI_SP_AltBeneficiario", parametros);
        }

        /// <summary>
        /// Exclui um beneficiário
        /// </summary>
        /// <param name="Id">ID do beneficiário</param>
        internal void Excluir(long Id)
        {
            List<System.Data.SqlClient.SqlParameter> parametros = new List<System.Data.SqlClient.SqlParameter>();

            parametros.Add(new System.Data.SqlClient.SqlParameter("Id", Id));

            base.Executar("FI_SP_DelBeneficiario", parametros);
        }

        /// <summary>
        /// Exclui todos os beneficiários de um cliente
        /// </summary>
        /// <param name="clienteId">ID do cliente</param>
        internal void ExcluirPorCliente(long clienteId)
        {
            List<System.Data.SqlClient.SqlParameter> parametros = new List<System.Data.SqlClient.SqlParameter>();

            parametros.Add(new System.Data.SqlClient.SqlParameter("ClienteId", clienteId));

            base.Executar("FI_SP_DelBeneficiariosPorCliente", parametros);
        }

        /// <summary>
        /// Converte DataSet para lista de beneficiários
        /// </summary>
        /// <param name="ds">DataSet</param>
        private List<DML.Beneficiario> Converter(DataSet ds)
        {
            List<DML.Beneficiario> lista = new List<DML.Beneficiario>();
            if (ds != null && ds.Tables != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    DML.Beneficiario beneficiario = new DML.Beneficiario();
                    beneficiario.Id = row.Field<long>("Id");
                    beneficiario.ClienteId = row.Field<long>("ClienteId");
                    beneficiario.CPF = row.Field<string>("CPF");
                    beneficiario.Nome = row.Field<string>("Nome");
                    lista.Add(beneficiario);
                }
            }

            return lista;
        }

        /// <summary>
        /// Salva múltiplos beneficiários para um cliente
        /// </summary>
        /// <param name="clienteId">ID do cliente</param>
        /// <param name="beneficiarios">Lista de beneficiários</param>
        internal void SalvarBeneficiarios(long clienteId, List<Beneficiario> beneficiarios)
        {
            if (beneficiarios != null && beneficiarios.Count > 0)
            {
                foreach (var beneficiario in beneficiarios)
                {
                    if (!string.IsNullOrEmpty(beneficiario.CPF) && !string.IsNullOrEmpty(beneficiario.Nome))
                    {
                        SalvarBeneficiarioIndividual(clienteId, beneficiario.CPF, beneficiario.Nome);
                    }
                }
            }
        }

        /// <summary>
        /// Salva um beneficiário individual usando o padrão AcessoDados
        /// </summary>
        private void SalvarBeneficiarioIndividual(long clienteId, string cpf, string nome)
        {
            List<SqlParameter> parametros = new List<SqlParameter>();
            
            parametros.Add(new SqlParameter("@IDCLIENTE", clienteId));
            parametros.Add(new SqlParameter("@CPF", cpf));
            parametros.Add(new SqlParameter("@NOME", nome));

            base.Executar("FI_SP_IncBeneficiario", parametros);
        }
    }
}
