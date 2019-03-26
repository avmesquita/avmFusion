using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Fusion.Framework.Exchange.Models
{
    public class SalaDeReuniao
    {

        // Código do Cliente prevê uma atualização do Pool em banco de dados, necessário para performance futura e load balancing para suportar o SSaS.
        // Neste caso como haverá pool de listas, seria interessante usar MongoDB

        private string _smtp;               // SMTP da Sala de Reunição (Identificador)
        private bool _haspeople;            // Indicador/Flag indicando Presença ou Ausência de Pessoas na sala
        private DateTime _updatedata;       // Data/Hora da Recepção da última informação

        /// <summary>
        /// Conta de SMTP do Office 365 da Sala de Reunião
        /// </summary>
        public string Smtp
        {
            get { return _smtp; }
            set { _smtp = value; }
        }

        /// <summary>
        /// Indica se existem pessoas na sala de reunião
        /// </summary>
        public bool HasPeople
        {
            get { return _haspeople; }
            set { _haspeople = value; }
        }

        /// <summary>
        /// Data da última atualização da informação
        /// </summary>
        public DateTime DataAtualizacao
        {
            get { return _updatedata; }
            set { _updatedata = value; }       
        }

        /// <summary>
        /// Construtor: Inicialização de atributos
        /// </summary>
        public SalaDeReuniao()        
        {            
            this._smtp = string.Empty;
            this._haspeople = false;
            this.DataAtualizacao = DateTime.MinValue;
        }
    }
}