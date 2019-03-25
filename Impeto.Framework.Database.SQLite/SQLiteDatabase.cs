using Impeto.Framework.Exchange.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Impeto.Framework.Database.SQLite
{
    public class SQLiteDatabase
    {
        /// <summary>
        /// Objeto Connection Real
        /// </summary>
        private SQLiteConnection _connection;

        /// <summary>
        /// Retorna o objeto Connection somente leitura
        /// </summary>
        public SQLiteConnection Connection
        {
            get
            {
                return _connection;
            }
        }

        /// <summary>
        /// CONSTRUCTOR
        /// </summary>
        /// <param name="connectionstring"></param>
        public SQLiteDatabase(string connectionstring = "Data Source =| DataDirectory | ImpetoExchange.s3db; Version=3;New=True;")
        {
            try
            {
                _connection = new SQLiteConnection(connectionstring);
                _connection.Open();
            }
            catch
            {
                GenerateTables();
            }
        }

        #region CRIACAO
        /// <summary>
        /// CRIA as TABELAS
        /// </summary>
        public void GenerateTables()
        {
            try
            {
                Connection.Open();

                string cmdstring = @"CREATE TABLE TB_PLANO ( 
                                       COD_PLANO INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, 
                                       TXT_TITULO TEXT, 
                                       NUM_QUANTIDADE INTEGER DEFAULT 0, 
                                       DAT_INICIO_VIGENCIA TEXT DEFAULT CURRENT_TIMESTAMP, 
                                       DAT_FIM_VIGENCIA TEXT, 
                                       VAL_PRECO NUMERIC DEFAULT 0 );

                                     CREATE TABLE TB_CLIENTE ( 
                                       COD_CLIENTE INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, 
                                       TXT_NOME TEXT UNIQUE, 
                                       TXT_SMTP TEXT, 
                                       TXT_SENHA TEXT, 
                                       TXT_TOKEN  TEXT, 
                                       TXT_TIMEZONE  TEXT, 
                                       DAT_CADASTRO TEXT DEFAULT CURRENT_TIMESTAMP, 
                                       COD_PLANO INTEGER, 
                                       FOREIGN KEY FK_CLIENTE_PLANO (COD_PLANO) REFERENCES TB_PLANO(COD_PLANO) 
                                     );

                                     CREATE TABLE TB_DISPOSITIVO ( 
                                       COD_DISPOSITIVO INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, 
                                       COD_CLIENTE INTEGER NOT NULL, 
                                       TXT_NOME TEXT, 
                                       TXT_SSID_ID TEXT, 
                                       TXT_SSID_PASS TEXT, 
                                       TXT_MAC TEXT UNIQUE, 
                                       TXT_TOKEN TEXT, 
                                       NUM_TEMPO_FIXO INTEGER DEFAULT 60, 
                                       NUM_TEMPO_EVNT INTEGER DEFAULT 60, 
                                       DAT_ATIVACAO TEXT DEFAULT CURRENT_TIMESTAMP, 
                                       IND_ATIVO INTEGER DEFAULT 1, 
                                       FOREIGN KEY FK_DISPOSITIVO_CLIENTE (COD_CLIENTE) REFERENCES TB_CLIENTE(COD_CLIENTE) 
                                     );
                                   ";

                SQLiteCommand mycommand = new SQLiteCommand(cmdstring, Connection);

                mycommand.ExecuteNonQuery();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// INCLUI os valores INICIAIS para o PROTOTIPO
        /// </summary>
        public void GenerateRows()
        {
            try
            {
                Connection.Open();

                string cmdstring = @"INSERT INTO TB_PLANO (TXT_TITULO, NUM_QUANTIDADE, DAT_INICIO_VIGENCIA, VAL_PRECO)
                                                   VALUES ('Plano DISP2',2,'2017-01-01 00:00:00','2017-12-31 23:59:59',500);

                                     INSERT INTO TB_CLIENTE (TXT_NOME, TXT_SMTP,TXT_SENHA,TXT_TOKEN,TXT_TIMEZONE, COD_PLANO)
                                                     VALUES ('Impeto Informatica','andre.mesquita@impeto.com.br','senha','cid','E. South America Standard Time',1);

                                     INSERT INTO TB_DISPOSITIVO (COD_CLIENTE,TXT_NOME,TXT_SSID_ID,TXT_SSID_PASS,TXT_MAC,TXT_TOKEN,NUM_TEMPO_FIXO,NUM_TEMPO_EVNT,IND_ATIVO)
                                                         VALUES (1,'Dispositivo do Cid','ImpetoWIFI','senha','00-00-00-00-00-00','cid',60,60,1);

                                     INSERT INTO TB_DISPOSITIVO (COD_CLIENTE,TXT_NOME,TXT_SSID_ID,TXT_SSID_PASS,TXT_MAC,TXT_TOKEN,NUM_TEMPO_FIXO,NUM_TEMPO_EVNT,IND_ATIVO)
                                                         VALUES (1,'Dispositivo 2','ImpetoWIFI','senha','00-00-00-00-00-01','cid',60,60,1);
                                   ";

                SQLiteCommand mycommand = new SQLiteCommand(cmdstring, Connection);

                mycommand.ExecuteNonQuery();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region DISPOSITIVO
        /// <summary>
        /// Retorna as configurações do Dispositivo IoT
        /// </summary>
        /// <param name="cliente"></param>
        /// <param name="dispositivo"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public Dispositivo GetDispositivo(int cliente, int dispositivo, string token)
        {
            Dispositivo retorno = null;

            SQLiteDataAdapter DB;
            DataSet DS = new DataSet();
            DataTable dt = new DataTable();

            string sql = string.Format("SELECT * FROM TB_DISPOSITIVO WHERE COD_DISPOSITIVO = {1} AND COD_CLIENTE = {0} AND TXT_TOKEN='{2}'",
                                       cliente, dispositivo, token);

            // Specify command below
            DB = new SQLiteDataAdapter(sql, _connection);
            DS.Reset();
            try
            {
                DB.Fill(DS);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("missing"))
                {
                    GenerateTables();
                }
                DS = new DataSet();
                dt = new DataTable();
                DB = new SQLiteDataAdapter(sql, _connection);
                DS.Reset();
                DB.Fill(DS);
            }
            dt = DS.Tables[0];

            // THe following segment updates the listview
            DataTable dtable = new DataTable();

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                DataRow drow = dt.Rows[i];

                // Only row that have not been deleted
                if (drow.RowState != DataRowState.Deleted)
                {
                    retorno = new Dispositivo();
                    retorno.Cliente = GetCliente(cliente);
                    retorno.Codigo = Convert.ToInt32(drow["COD_DISPOSITIVO"]);
                    retorno.MAC = drow["TXT_MAC"].ToString();
                    retorno.Nome = drow["TXT_NOME"].ToString();
                    retorno.SSID_ID = drow["TXT_SSID_ID"].ToString();
                    retorno.SSID_PASS = drow["TXT_SSID_PASS"].ToString();
                    retorno.Token = drow["TXT_TOKEN"].ToString();
                    retorno.TempoFixo = Convert.ToInt32(drow["NUM_TEMPO_FIXO"]);
                    retorno.TempoEvento = Convert.ToInt32(drow["NUM_TEMPO_EVNT"]);
                    retorno.Ativo = Convert.ToBoolean(drow["IND_ATIVO"]);
                    retorno.DataAtivacao = Convert.ToDateTime(drow["DAT_ATIVACAO"]);
                    break;
                }

            }
            return retorno;
        }

        /// <summary>
        /// Retorna TODOS os DISPOSITIVOS de um CLIENTE
        /// </summary>
        /// <param name="cliente"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public List<Dispositivo> GetAllDispositivo(int cliente, string token)
        {
            List<Dispositivo> retorno = new List<Dispositivo>();

            SQLiteDataAdapter DB;
            DataSet DS = new DataSet();
            DataTable dt = new DataTable();

            string sql = string.Format("SELECT * FROM TB_DISPOSITIVO WHERE COD_CLIENTE = {0} AND TXT_TOKEN='{2}'",
                                       cliente, token);

            // Specify command below
            DB = new SQLiteDataAdapter(sql, _connection);
            DS.Reset();
            try
            {
                DB.Fill(DS);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("missing"))
                {
                    GenerateTables();
                }
                DS = new DataSet();
                dt = new DataTable();
                DB = new SQLiteDataAdapter(sql, _connection);
                DS.Reset();
                DB.Fill(DS);
            }
            dt = DS.Tables[0];

            // THe following segment updates the listview
            DataTable dtable = new DataTable();

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                DataRow drow = dt.Rows[i];

                // Only row that have not been deleted
                if (drow.RowState != DataRowState.Deleted)
                {
                    Dispositivo item = new Dispositivo();
                    item.Cliente = GetCliente(cliente);
                    item.Codigo = Convert.ToInt32(drow["COD_DISPOSITIVO"]);
                    item.MAC = drow["TXT_MAC"].ToString();
                    item.Nome = drow["TXT_NOME"].ToString();
                    item.SSID_ID = drow["TXT_SSID_ID"].ToString();
                    item.SSID_PASS = drow["TXT_SSID_PASS"].ToString();
                    item.Token = drow["TXT_TOKEN"].ToString();
                    item.TempoFixo = Convert.ToInt32(drow["NUM_TEMPO_FIXO"]);
                    item.TempoEvento = Convert.ToInt32(drow["NUM_TEMPO_EVNT"]);
                    item.Ativo = Convert.ToBoolean(drow["IND_ATIVO"]);
                    item.DataAtivacao = Convert.ToDateTime(drow["DAT_ATIVACAO"]);
                    retorno.Add(item);
                }

            }
            return retorno;
        }

        /// <summary>
        /// Retorna Todos os DISPOSITIVOS
        /// </summary>
        /// <returns></returns>
        public List<Dispositivo> GetAllDispositivo()
        {
            List<Dispositivo> retorno = new List<Dispositivo>();

            SQLiteDataAdapter DB;
            DataSet DS = new DataSet();
            DataTable dt = new DataTable();

            string sql = "SELECT * FROM TB_DISPOSITIVO";

            // Specify command below
            DB = new SQLiteDataAdapter(sql, _connection);
            DS.Reset();
            try
            {
                DB.Fill(DS);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("missing"))
                {
                    GenerateTables();
                }
                DS = new DataSet();
                dt = new DataTable();
                DB = new SQLiteDataAdapter(sql, _connection);
                DS.Reset();
                DB.Fill(DS);
            }
            dt = DS.Tables[0];

            // THe following segment updates the listview
            DataTable dtable = new DataTable();

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                DataRow drow = dt.Rows[i];

                // Only row that have not been deleted
                if (drow.RowState != DataRowState.Deleted)
                {
                    Dispositivo item = new Dispositivo();
                    item.Cliente = GetCliente(Convert.ToInt32(drow["COD_CLIENTE"]));
                    item.Codigo = Convert.ToInt32(drow["COD_DISPOSITIVO"]);
                    item.MAC = drow["TXT_MAC"].ToString();
                    item.Nome = drow["TXT_NOME"].ToString();
                    item.SSID_ID = drow["TXT_SSID_ID"].ToString();
                    item.SSID_PASS = drow["TXT_SSID_PASS"].ToString();
                    item.Token = drow["TXT_TOKEN"].ToString();
                    item.TempoFixo = Convert.ToInt32(drow["NUM_TEMPO_FIXO"]);
                    item.TempoEvento = Convert.ToInt32(drow["NUM_TEMPO_EVNT"]);
                    item.Ativo = Convert.ToBoolean(drow["IND_ATIVO"]);
                    item.DataAtivacao = Convert.ToDateTime(drow["DAT_ATIVACAO"]);
                    retorno.Add(item);
                }

            }
            return retorno;
        }

        /// <summary>
        /// INCLUI um DISPOSITIVO
        /// </summary>
        /// <param name="dispositivo"></param>
        public void IncluirDispositivo(Dispositivo dispositivo)
        {
            try
            {
                _connection.Open();
                string cmdstring = string.Format("INSERT INTO TB_DISPOSITIVO (COD_CLIENTE,TXT_NOME,TXT_SSID_ID,TXT_SSID_PASS,TXT_MAC,TXT_TOKEN,NUM_TEMPO_FIXO,NUM_TEMPO_EVNT,IND_ATIVO) VALUES ({0},{1},{2},'{3}','{4}','{5}','{6}',{7},{8})",
                                                 dispositivo.Cliente.Codigo,
                                                 dispositivo.Nome,
                                                 dispositivo.SSID_ID,
                                                 dispositivo.SSID_PASS,
                                                 dispositivo.MAC,
                                                 dispositivo.Token,
                                                 dispositivo.TempoFixo,
                                                 dispositivo.TempoEvento,
                                                 dispositivo.Ativo
                                                 );
                SQLiteCommand mycommand = new SQLiteCommand(cmdstring, _connection);
                mycommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// ALTERAR um DISPOSITIVO (#TODO)
        /// </summary>
        /// <param name="dispositivo"></param>
        public void AlterarDispositivo(Dispositivo dispositivo)
        {
            new NotImplementedException("Em Desenvolvimento");
        }

        /// <summary>
        /// EXCLUIR um DISPOSITIVO (#TODO)
        /// </summary>
        /// <param name="idDispositivo"></param>
        /// <param name="token"></param>
        public void ExcluirDispositivo(int idDispositivo, string token)
        {
            new NotImplementedException("Em Desenvolvimento");
        }
        #endregion

        #region CLIENTE
        /// <summary>
        /// Retorna os dados de UM CLIENTE
        /// </summary>
        /// <param name="cliente"></param>
        /// <returns></returns>
        public Cliente GetCliente(int cliente)
        {
            Cliente retorno = null;

            if (cliente != null) // eu não confio no hint
            {
                SQLiteDataAdapter DB;
                DataSet DS = new DataSet();
                DataTable dt = new DataTable();

                string sql = string.Format("SELECT * FROM TB_CLIENTE WHERE COD_CLIENTE = {0}", cliente);

                // Specify command below
                DB = new SQLiteDataAdapter(sql, _connection);
                DS.Reset();
                DB.Fill(DS);
                dt = DS.Tables[0];

                // THe following segment updates the listview
                DataTable dtable = new DataTable();

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DataRow drow = dt.Rows[i];

                    // Only row that have not been deleted
                    if (drow.RowState != DataRowState.Deleted)
                    {
                        retorno = new Cliente();
                        retorno.Codigo = Convert.ToInt32(drow["COD_CLIENTE"]);
                        retorno.Nome = drow["TXT_NOME"].ToString();
                        retorno.Smtp = drow["TXT_SMTP"].ToString();
                        retorno.Senha = drow["TXT_SENHA"].ToString();
                        retorno.TimeZone = drow["TXT_TIMEZONE"].ToString();
                        retorno.Plano.Codigo = Convert.ToInt32(drow["COD_PLANO"]);
                        break;
                    }
                }
            }
            return retorno;
        }

        /// <summary>
        /// Retorna TODOS os CLIENTES
        /// </summary>
        /// <returns></returns>
        public List<Cliente> GetAllCliente()
        {
            List<Cliente> retorno = new List<Cliente>();

            SQLiteDataAdapter DB;
            DataSet DS = new DataSet();
            DataTable dt = new DataTable();

            string sql = "SELECT * FROM TB_CLIENTE";

            // Specify command below
            DB = new SQLiteDataAdapter(sql, _connection);
            DS.Reset();
            DB.Fill(DS);
            dt = DS.Tables[0];

            // THe following segment updates the listview
            DataTable dtable = new DataTable();

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                DataRow drow = dt.Rows[i];

                // Only row that have not been deleted
                if (drow.RowState != DataRowState.Deleted)
                {
                    Cliente item = new Cliente();
                    item.Codigo = Convert.ToInt32(drow["COD_CLIENTE"]);
                    item.Nome = drow["TXT_NOME"].ToString();
                    item.Smtp = drow["TXT_SMTP"].ToString();
                    item.Senha = drow["TXT_SENHA"].ToString();
                    item.TimeZone = drow["TXT_TIMEZONE"].ToString();
                    item.Plano.Codigo = Convert.ToInt32(drow["COD_PLANO"]);
                    retorno.Add(item);
                }
            }
            return retorno;
        }

        /// <summary>
        /// INCLUIR um CLIENTE
        /// </summary>
        /// <param name="cliente"></param>
        public void IncluirCliente(Cliente cliente)
        {
            try
            {
                _connection.Open();
                string cmdstring = string.Format("INSERT INTO TB_CLIENTE (COD_CLIENTE, TXT_NOME, TXT_SMTP,TXT_SENHA,TXT_TOKEN,TXT_TIMEZONE,COD_PLANO) VALUES ({0},'{1}','{2}','{3}','{4}','{5}',{6})",
                                                 cliente.Codigo,
                                                 cliente.Nome,
                                                 cliente.Smtp,
                                                 cliente.Senha,
                                                 cliente.Token,
                                                 cliente.TimeZone,
                                                 cliente.Plano.Codigo);
                SQLiteCommand mycommand = new SQLiteCommand(cmdstring, _connection);
                mycommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// ALTERAR um CLIENTE (#TODO)
        /// </summary>
        /// <param name="cliente"></param>
        public void AlterarCliente(Cliente cliente)
        {
            new NotImplementedException("Em Desenvolvimento");
        }

        /// <summary>
        /// EXCLUIR um CLIENTE (#TODO)
        /// </summary>
        /// <param name="idCliente"></param>
        /// <param name="token"></param>
        public void ExcluirCliente(int idCliente, string token)
        {
            new NotImplementedException("Em Desenvolvimento");
        }
        #endregion
        
        public void IncluirPlano(Plano plano)
        {
            try
            {
                _connection.Open();

                string cmdstring = string.Format("INSERT INTO TB_PLANO (DATA_INICIO_VIGENCIA, COD_DISPOSITIVO,COD_CLIENTE,TXT_NOME,TXT_SSID_ID,TXT_SSID_PASS,TXT_MAC,TXT_TOKEN,NUM_TEMPO_FIXO,NUM_TEMPO_EVNT,IND_ATIVO) VALUES ({0},{1},{2},'{3}','{4}','{5}','{6}',{7},{8},{9})",
                                                 plano.InicioVigencia,
                                                 plano.FimVigencia,
                                                 plano.QtdeDispositivos,
                                                 plano.Titulo,
                                                 plano.Valor
                                                 );

                SQLiteCommand mycommand = new SQLiteCommand(cmdstring, _connection);

                mycommand.ExecuteNonQuery();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
