using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Exchange.WebServices.Data;
using Fusion.Framework.Exchange.Authentication;
using Fusion.Framework.Exchange.Models;
using System.Collections.ObjectModel;

namespace Fusion.Framework.Exchange.Service
{
    public class SalaDeReuniaoBS
    {
        // instancia a conexão com o serviço do Exchange
        //ExchangeService service = Service.ConnectToService(UserDataFromConsole.GetUserData(), new TraceListener());        
        public string email;
        public string senha;
        public string autodiscoverUrl;

        private ExchangeService service
        {
            get
            {
                return Fusion.Framework.Exchange.Authentication.Service.ConnectToService(UserDataFromConsole.GetUserData(
                                                                                                               this.email,
                                                                                                               this.senha,
                                                                                                               this.autodiscoverUrl),
                                                                                         new TraceListener());
            }
        }

        #region PUBLIC METHODS
        /// <summary>
        /// Obtem todas as salas de reunião e suas disponibilidades
        /// /*ATENCAO*/ CODIGO IMPORTANTE DA REGRA DE NEGOCIO. CUIDADO.
        /// </summary>
        /// <returns></returns>
        public List<ResultadoDisponibilidadeSalaReuniao> obterDisponibilidadeSalaReuniao()
        {
            List<ResultadoDisponibilidadeSalaReuniao> retorno = new List<ResultadoDisponibilidadeSalaReuniao>();

            try
            {
                // obtem a lista de salas de reuniao
                List<AttendeeInfo> listaParticipantes = getSalasDeReuniao(this.service);

                foreach (var salaDeReuniao in listaParticipantes)
                {
                    ResultadoDisponibilidadeSalaReuniao resultado = new ResultadoDisponibilidadeSalaReuniao();
                    resultado.Nome = salaDeReuniao.SmtpAddress;
                    resultado.Smtp = salaDeReuniao.SmtpAddress;
                    resultado.Tipo = salaDeReuniao.AttendeeType;

                    resultado.Status = obterDisponibilidadeExchange(salaDeReuniao.SmtpAddress);

                    resultado.Mensagem = resultado.Status.Mensagem;

                    retorno.Add(resultado);
                }
            }
            catch
            {
                return retorno;
            }
            return retorno;
        }

        /// <summary>
        /// Obtem todas as salas de reunião e suas disponibilidades
        /// /*ATENCAO*/ CODIGO IMPORTANTE DA REGRA DE NEGOCIO. CUIDADO.
        /// </summary>
        /// <returns></returns>
        public List<ResultadoDisponibilidadeSalaReuniao> obterDisponibilidadeSalaReuniao(string zoneId = "")
        {
            List<ResultadoDisponibilidadeSalaReuniao> retorno = new List<ResultadoDisponibilidadeSalaReuniao>();

            try
            {
                // obtem a lista de salas de reuniao
                List<AttendeeInfo> listaParticipantes = getSalasDeReuniao(this.service);

                foreach (var salaDeReuniao in listaParticipantes)
                {
                    ResultadoDisponibilidadeSalaReuniao resultado = new ResultadoDisponibilidadeSalaReuniao();
                    resultado.Nome = salaDeReuniao.SmtpAddress;
                    resultado.Smtp = salaDeReuniao.SmtpAddress;
                    resultado.Tipo = salaDeReuniao.AttendeeType;

                    if (zoneId == null || zoneId == "")
                    {
                        resultado.Status = obterDisponibilidadeExchange(salaDeReuniao.SmtpAddress);
                    }
                    else
                    {
                        resultado.Status = obterDisponibilidadeExchange(salaDeReuniao.SmtpAddress, zoneId);
                    }
                    resultado.Mensagem = resultado.Status.Mensagem;

                    retorno.Add(resultado);
                }
            }
            catch
            {
                return retorno;
            }
            return retorno;
        }

        public List<ResultadoDisponibilidadeSalaReuniao> obterDisponibilidadeSalaReuniao(GetRoomTipo roomTipo = GetRoomTipo.ConfiguracaoPorExchange)
        {
            // FindMailboxes lê através de busca pelo AD
            //var k = FindMailboxes(7);
            //var q = FindMailboxes(8);

            List<ResultadoDisponibilidadeSalaReuniao> retorno = new List<ResultadoDisponibilidadeSalaReuniao>();

            List<AttendeeInfo> listaParticipantes = null;
            try
            {
                switch (roomTipo)
                {
                    case GetRoomTipo.ConfiguracaoPorExchange:
                        // obtem a lista de salas de reuniao
                        listaParticipantes = getSalasDeReuniao(this.service);
                        break;

                    case GetRoomTipo.ConfiguracaoPorArquivo:
                        // obtem a lista de salas de reuniao pelo web.config
                        listaParticipantes = getSalasDeReuniaoPorWebConfig();
                        break;
                }

                foreach (var sala in listaParticipantes)
                {
                    ResultadoDisponibilidadeSalaReuniao resultado = new ResultadoDisponibilidadeSalaReuniao();
                    resultado.Nome = sala.SmtpAddress;
                    resultado.Smtp = sala.SmtpAddress;
                    resultado.Tipo = MeetingAttendeeType.Room;
                    resultado.Status = obterDisponibilidadeExchange(sala.SmtpAddress);
                    resultado.Mensagem = resultado.Status.Mensagem;

                    retorno.Add(resultado);
                }
            }
            catch
            {
                return retorno;
            }

            return retorno;
        }

        /// <summary>
        /// Obtem a disponibilidade de um usuário smtp
        /// </summary>
        /// <param name="smtp"></param>
        /// <returns></returns>
        public Status obterDisponibilidadeExchange(string smtp, string zoneId = "E. South America Standard Time")
        {
            // SE O SMTP NÃO ENVIAR O ENDEREÇO DO HOST "@XPTO.COM.BR", PREENCHE AUTOMATICAMENTE COM O DOMINIO DA CONTA CONFIGURADA
            if (!smtp.Contains("@"))
            {
                var config = new ConfiguracaoBS().obterConfiguracao();
                if (config != null)
                {
                    string host = config.EmailAddress.Split('@')[1];
                    smtp = smtp + '@' + host;
                }
            }
            // Instancia o objeto de retorno de status
            Status retorno = new Status();
            try
            {
                // Atribui a janela de tempo da Pesquisa
                // Precisamos de salas de reunião que já tenham iniciado seu cronômetro em 15 minutos
                // O prazo de 6 horas é necessário para cumprir especificação técnica do método.
                DateTime startTime = DateTime.Now.ToUniversalTime();

                var diferencaParaOutroDia = 24 - DateTime.Now.ToUniversalTime().Hour;

                var janelaDeTempo = new TimeWindow(startTime,
                                                   startTime.AddHours(diferencaParaOutroDia));

                var janelaDeSugestao = new TimeWindow(startTime,
                                                      startTime.AddHours(diferencaParaOutroDia));
                // Atribui os parametros de busca
                var options = new AvailabilityOptions
                {
                    MeetingDuration = 30
                    ,
                    RequestedFreeBusyView = FreeBusyViewType.Detailed
                    ,
                    MergedFreeBusyInterval = 5
                    ,
                    MaximumSuggestionsPerDay = 1
                    ,
                    GoodSuggestionThreshold = 25
                    ,
                    CurrentMeetingTime = startTime //DateTime.Now
                    ,
                    DetailedSuggestionsWindow = janelaDeSugestao
                    ,
                    MinimumSuggestionQuality = SuggestionQuality.Poor
                };

                // Para a consulta, é obrigatorio enviar uma lista de participantes da reuniao
                // Como a única informação que precisamos é a da Sala, enviamos apenas um elemento
                List<AttendeeInfo> listaUnicoElemento = new List<AttendeeInfo>();
                AttendeeInfo UnicoElemento = new AttendeeInfo(); // instancia o elemento unico para a lista obrigatória
                UnicoElemento.SmtpAddress = smtp; // preenche com o smtp da checagem          
                listaUnicoElemento.Add(UnicoElemento);

                // INICIO - Efetua a busca no exchange com os parâmetros selecionados
                GetUserAvailabilityResults resultadoDisponibilidade = this.service.GetUserAvailability(listaUnicoElemento,
                                                                                                       janelaDeTempo,
                                                                                                       AvailabilityData.FreeBusyAndSuggestions,
                                                                                                       options);
                // FIM - busca Exchange

                // PRIMEIRA TENTATIVA DE OBTENCAO DA DISPONIBILIDADE - ATRAVÉS DO RESULTADO DA DISPONIBILIDADE

                foreach (AttendeeAvailability avail in resultadoDisponibilidade.AttendeesAvailability)
                {

                    string erro = string.Empty;
                    if (avail.Result == ServiceResult.Error)
                    {
                        erro = (avail.Result != null ? "RESULT = " + avail.Result.ToString() + " | " : "") +
                            (avail.ErrorCode != null ? "ErrorCode = " + avail.ErrorCode.ToString() + " | " : "") +
                            (avail.ErrorDetails != null ? "ErrorDetais = " + avail.ErrorDetails.ToString() + " | " : "") +
                            (avail.ErrorMessage != null ? "ErrorMessage = " + avail.ErrorMessage.ToString() + " | " : "") +
                            (avail.ErrorProperties.Count() > 0 ? "ErrorProperties = " + avail.ErrorProperties.ToString() : "");
                    }

                    var calendarios = avail.CalendarEvents.Where(x => x.StartTime < DateTime.Now.ToUniversalTime()
                                                                   && x.EndTime > DateTime.Now.ToUniversalTime()).ToList();

                    if (calendarios.Count() > 0)
                    {
                        foreach (CalendarEvent calItem in calendarios)
                        {

                            if (calItem.FreeBusyStatus == LegacyFreeBusyStatus.Busy ||
                                calItem.FreeBusyStatus == LegacyFreeBusyStatus.OOF ||
                                calItem.FreeBusyStatus == LegacyFreeBusyStatus.Tentative ||
                                calItem.FreeBusyStatus == LegacyFreeBusyStatus.WorkingElsewhere)
                            {
                                // Se a data de fim da reuniao já tiver passado, então a sala está livre
                                if (calItem.EndTime < DateTime.Now.ToUniversalTime())
                                {
                                    retorno.StatusDisponibilidade = StatusDisponibilidade.Livre;
                                    retorno.Mensagem = "";                                    
                                }
                                // Se já passou 30 minutos de iniciada uma reunião e não terminou
                                else if ((DateTime.Now.ToUniversalTime() >= calItem.StartTime.ToUniversalTime().AddMinutes(30)) &&
                                         (DateTime.Now.ToUniversalTime() <= calItem.EndTime.ToUniversalTime()))
                                {
                                    retorno.StatusDisponibilidade = StatusDisponibilidade.EmReuniao30;

                                    TimeZoneInfo zonaFuso = TimeZoneInfo.FindSystemTimeZoneById(zoneId);
                                    var dataFimZonaFuso = TimeZoneInfo.ConvertTimeFromUtc(calItem.EndTime, zonaFuso);

                                    retorno.Mensagem = "Reunião já inicada há mais de 30 minutos com término às " + dataFimZonaFuso.ToString(); // calItem.EndTime.ToLocalTime().ToString();
                                    // inclui na lista de eventos de calendários do retorno
                                    retorno.ListaEventos.Add(converterCalendarEventParaCalendarioEvento(calItem));
                                }
                                // Se já passou 15 minutos de iniciada uma reunião e não terminou
                                else if ((DateTime.Now.ToUniversalTime() >= calItem.StartTime.ToUniversalTime().AddMinutes(15)) &&
                                         (DateTime.Now.ToUniversalTime() <= calItem.EndTime.ToUniversalTime()))
                                {
                                    TimeZoneInfo zonaFuso = TimeZoneInfo.FindSystemTimeZoneById(zoneId);
                                    var dataFimZonaFuso = TimeZoneInfo.ConvertTimeFromUtc(calItem.EndTime, zonaFuso);

                                    retorno.StatusDisponibilidade = StatusDisponibilidade.EmReuniao;
                                    retorno.Mensagem = "Reunião já inicada há mais de 15 minutos com término às " + dataFimZonaFuso.ToString(); // calItem.EndTime.ToLocalTime().ToString();
                                    // inclui na lista de eventos de calendários do retorno
                                    retorno.ListaEventos.Add(converterCalendarEventParaCalendarioEvento(calItem));
                                }
                                else
                                {
                                    TimeZoneInfo zonaFuso = TimeZoneInfo.FindSystemTimeZoneById(zoneId);
                                    var dataInicioZonaFuso = TimeZoneInfo.ConvertTimeFromUtc(calItem.StartTime, zonaFuso);
                                    var dataFimZonaFuso = TimeZoneInfo.ConvertTimeFromUtc(calItem.EndTime, zonaFuso);

                                    retorno.StatusDisponibilidade = StatusDisponibilidade.Ocupado;
                                    retorno.Mensagem = " Ocupado de " + dataInicioZonaFuso.ToString() + " até " + dataFimZonaFuso.ToString();
                                    //retorno.Mensagem = " Ocupado de " + calItem.StartTime.ToLocalTime().ToString() + " até " + calItem.EndTime.ToLocalTime().ToString();
                                    // inclui na lista de eventos de calendários do retorno
                                    retorno.ListaEventos.Add(converterCalendarEventParaCalendarioEvento(calItem));
                                }
                                break;
                            }
                            else
                            {
                                TimeZoneInfo zonaFuso = TimeZoneInfo.FindSystemTimeZoneById(zoneId);
                                var dataInicioZonaFuso = TimeZoneInfo.ConvertTimeFromUtc(calItem.StartTime, zonaFuso);

                                retorno.StatusDisponibilidade = StatusDisponibilidade.Livre;
                                retorno.Mensagem = "Livre a partir de " + dataInicioZonaFuso.ToString(); //calItem.StartTime.ToString();
                            }
                        }
                    }
                    else
                    {
                        // O Status ainda é desconhecido
                        // Se for usar a opção de lógica por sugestão, manter desconhecido
                        retorno.StatusDisponibilidade = StatusDisponibilidade.Livre; // StatusDisponibilidade.Desconhecido;
                        // Mensagem ao usuário
                        retorno.Mensagem = "Não encontramos eventos de calendário para esta sala.";

                        if (!erro.Equals(string.Empty))
                        {
                            retorno.StatusDisponibilidade = StatusDisponibilidade.Desconhecido;
                            // Mensagem ao usuário
                            retorno.Mensagem = "[ERROR] " + erro;
                        }
                    }
                }

                // SUGESTÃO DE DATA MAIS PRÓXIMA DA DATA ATUAL
                if (resultadoDisponibilidade.Suggestions.Count > 0)
                {
                    // PEGAR A SUGESTAO MAIS CEDO
                    var sugestaoMaisCedo = resultadoDisponibilidade.Suggestions.OrderBy(x => x.Date).FirstOrDefault();

                    // SE POR VENTURA HOUVE SUGESTOES DE HORA, ELA TEM PRIORIDADE NO RETORNO
                    if (sugestaoMaisCedo.TimeSuggestions.Count > 0)
                    {
                        var horaDoEncontro = sugestaoMaisCedo.TimeSuggestions.OrderBy(x => x.MeetingTime).FirstOrDefault().MeetingTime;

                        TimeZoneInfo zonaFuso = TimeZoneInfo.FindSystemTimeZoneById(zoneId);
                        horaDoEncontro = TimeZoneInfo.ConvertTimeFromUtc(horaDoEncontro, zonaFuso);


                        retorno.SugestaoProximaReuniao = horaDoEncontro.ToLocalTime();
                    }
                    else
                    {
                        TimeZoneInfo zonaFuso = TimeZoneInfo.FindSystemTimeZoneById(zoneId);
                        var sugestao = TimeZoneInfo.ConvertTimeFromUtc(sugestaoMaisCedo.Date, zonaFuso);

                        retorno.SugestaoProximaReuniao = sugestaoMaisCedo.Date;
                    }
                }

            }
            catch (Exception ex)
            {
                string msgDisponibilidade = "[EXCEPTION] " +
                                            (ex.Message != null ? "MESSAGE = " + ex.Message.ToString() + " | " : "") +
                                            (ex.InnerException != null ? "INNER = " + ex.InnerException.ToString() + " | " : "") +
                                            (ex.StackTrace != null ? "STACKTRACE = " + ex.StackTrace.ToString() + " | " : "") +
                                            (ex.Source != null ? "SOURCE = " + ex.Source.ToString() : "");
                retorno.Mensagem = msgDisponibilidade;

                retorno.StatusDisponibilidade = StatusDisponibilidade.Desconhecido;
            }
            return retorno;
        }

        /// <summary>
        /// Obtém os endereços dos building rooms
        /// </summary>
        /// <returns></returns>
        public EmailAddressCollection getRoomLists()
        {
            EmailAddressCollection rooms = service.GetRoomLists();
            return rooms;
        }

        /// <summary>
        /// Obtem as Salas de Reunião de um Building Room
        /// </summary>
        /// <param name="smtp"></param>
        /// <returns></returns>
        public Collection<EmailAddress> getRooms(string smtp)
        {
            Collection<EmailAddress> rooms = service.GetRooms(smtp);

            return rooms;
        }

        /// <summary>
        /// Cancela um evento a apartir de um storeId e um smtp
        /// </summary>
        /// <param name="storeId"></param>
        /// <param name="smtp"></param>
        /// <returns></returns>
        public bool CancelarEvento(string storeId, string smtp)
        {
            var convertedId = (AlternateId)service.ConvertId(new AlternateId(IdFormat.HexEntryId, storeId, smtp), IdFormat.EwsId);

            var appointment = Appointment.Bind(this.service, new ItemId(convertedId.UniqueId));

            if (appointment != null)
            {
                // Cancela o apontamento

                // PLANO A - FUROU.

                // Só pode ser feito por um "organizer".
                // Se não fosse por este impedimento do serviço, o código comentado abaixo resolveria o problema

                //var results = appointment.CancelMeeting("Evento '" + appointment.Subject + "' foi cancelado por falta de pessoas na sala.");
                //if (results.Appointment.IsCancelled)
                //{
                //    return true;
                //}

                // PLANO B
                try
                {
                    // Avisa para todo mundo da mudança
                    SendInvitationsOrCancellationsMode mode = SendInvitationsOrCancellationsMode.SendOnlyToAll;

                    // termina a reunião no minuto anterior
                    appointment.End = DateTime.Now.AddMinutes(-1);

                    // altera o assunto do apontamento
                    appointment.Subject = "[CANCELADO] " + appointment.Subject;

                    // altera o corpo da mensagem do apontamento
                    appointment.Body.Text = "Este evento foi cancelado pelo Fusion Sala de Reunião." +
                                             Environment.NewLine + Environment.NewLine + Environment.NewLine +
                                             appointment.Body.Text;

                    // obtém a lista de pessoas da reunião
                    string listaEmails = "";
                    // primeiro carrega as pessoas obrigatórias da reunião
                    foreach (var pessoa in appointment.RequiredAttendees)
                    {
                        listaEmails += pessoa.Address.ToString() + ";";
                    }
                    // depois carrega as pessoas opcionais que também precisam saber do cancelamento
                    foreach (var pessoa in appointment.OptionalAttendees)
                    {
                        listaEmails += pessoa.Address.ToString() + ";";
                    }
                    // Atualiza o apontamento
                    appointment.Update(ConflictResolutionMode.AlwaysOverwrite, mode);

                    // carrega o apontamento novamente para certificar alguma alteração
                    // appointment = Appointment.Bind(this.service, new ItemId(convertedId.UniqueId));

                    // Envia e-mail aos participantes informando o cancelamento da reunião
                    SendmailEWS(appointment.Subject, appointment.Body.Text, listaEmails);

                    // Apaga apontamento da Sala de Reunião
                    appointment.Delete(DeleteMode.MoveToDeletedItems);

                    // Se tentar usar o método abaixo, dá erro porque não existe mais
                    // mas só não existe mais nos apontamentos da entidade da sala.
                    // appointment = Appointment.Bind(this.service, new ItemId(convertedId.UniqueId));

                    return true;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return false;
        }

        public void AlertaSalaVazia(string storeId, string smtp)
        {
            var convertedId = (AlternateId)service.ConvertId(new AlternateId(IdFormat.HexEntryId, storeId, smtp), IdFormat.EwsId);

            var appointment = Appointment.Bind(this.service, new ItemId(convertedId.UniqueId));

            if (appointment != null)
            {
                // Avisa para todo mundo da mudança
                // SendInvitationsOrCancellationsMode mode = SendInvitationsOrCancellationsMode.SendOnlyToAll;

                // termina a reunião no minuto anterior
                // appointment.End = DateTime.Now.AddMinutes(-1);

                // altera o corpo da mensagem do apontamento
                //appointment.Body.Text = "Este evento foi auditado pelo Fusion Conference Auditor." +
                //                         Environment.NewLine + Environment.NewLine + Environment.NewLine +
                //                         appointment.Body.Text;

                // obtém a lista de pessoas da reunião
                string listaEmails = "";
                // primeiro carrega as pessoas obrigatórias da reunião
                foreach (var pessoa in appointment.RequiredAttendees)
                {
                    listaEmails += pessoa.Address.ToString() + ";";
                }
                // depois carrega as pessoas opcionais que também precisam saber do cancelamento
                foreach (var pessoa in appointment.OptionalAttendees)
                {
                    listaEmails += pessoa.Address.ToString() + ";";
                }
                // Atualiza o apontamento
                // appointment.Update(ConflictResolutionMode.AlwaysOverwrite, mode);

                // Cria uma mensagem para alertar aos participantes que a sala está vazia
                string msg = string.Format("A reunião sobre o tema '{0}' deveria ter começado na sala.", appointment.Subject.Trim(), appointment.Location.Trim()) + Environment.NewLine + Environment.NewLine;
                msg += "A sala encontra-se vazia neste momento." + Environment.NewLine;

                // Envia e-mail aos participantes informando o cancelamento da reunião
                SendmailEWS(appointment.Subject, msg, listaEmails);
            }
        }

        #endregion

        #region PRIVATE METHODS
        private List<AttendeeInfo> getSalasDeReuniao(ExchangeService service)
        {
            List<AttendeeInfo> listaParticipantes = new List<AttendeeInfo>();

            EmailAddressCollection rooms = service.GetRoomLists();

            foreach (var item in rooms)
            {
                var salas = service.GetRooms(item.Address);

                foreach (var sala in salas)
                {
                    AttendeeInfo participante = new AttendeeInfo(sala.Address, MeetingAttendeeType.Room, false);
                    listaParticipantes.Add(participante);
                }
            }
            return listaParticipantes;
        }

        private List<AttendeeInfo> getSalasDeReuniaoPorWebConfig()
        {
            List<AttendeeInfo> listaParticipantes = new List<AttendeeInfo>();

            string EmailsWebConfig = new ConfiguracaoBS().obterConfiguracao().SalasEstaticas;

            var arrayEmails = EmailsWebConfig.Split(';').ToArray();

            foreach (var sala in arrayEmails)
            {
                AttendeeInfo participante = new AttendeeInfo(sala, MeetingAttendeeType.Room, false);
                listaParticipantes.Add(participante);
            }

            return listaParticipantes;
        }

        private List<String> getSalasDeReuniaoListString(ExchangeService service)
        {
            List<String> lista = new List<String>();

            EmailAddressCollection rooms = service.GetRoomLists();

            foreach (var item in rooms)
            {
                var salas = service.GetRooms(item.Address);

                lista.Add(item.Name + "<" + item.Address + "> <MailboxType='" + item.MailboxType.ToString() + "' Value='" + item.MailboxType.Value.ToString() + "'>");
                foreach (var sala in salas)
                {
                    lista.Add("  > " + sala.Name + "<" + sala.Address + "> <MailboxType='" + sala.MailboxType.ToString() + "' Value='" + sala.MailboxType.Value.ToString() + "'>");
                }
            }
            return lista;
        }

        private string SugestoesParaTexto(System.Collections.ObjectModel.Collection<TimeSuggestion> sugestoes)
        {
            string resultado = "";
            foreach (var item in sugestoes)
            {
                resultado = "IsWorkTime = " + item.IsWorkTime.ToString() + " | " +
                            "MeetingTime = " + item.MeetingTime.ToString() + " | " +
                            "Quality = " + item.Quality.ToString() + " | " +
                            "Conflicts = " + ConflitosParaTexto(item.Conflicts);
            }

            return resultado;
        }
        private string ConflitosParaTexto(System.Collections.ObjectModel.Collection<Conflict> conflitos)
        {
            string resultado = "";
            foreach (var item in conflitos)
            {
                resultado = "ConflictType = " + item.ConflictType.ToString() + " | " +
                            "FreeBusyStatus = " + item.FreeBusyStatus.ToString() + " | " +
                            "NumberOfMembers = " + item.NumberOfMembers.ToString() + " | " +
                            "" + item.NumberOfMembersAvailable.ToString() + " | " +
                            "" + item.NumberOfMembersWithConflict.ToString() + " | " +
                            "" + item.NumberOfMembersWithNoData.ToString();
            }
            return resultado;

        }

        private List<SugestaoDataDisponivel> ConverterDeTimeSuggestionsParaSugestaoDataDisponivel(System.Collections.ObjectModel.Collection<Microsoft.Exchange.WebServices.Data.TimeSuggestion> colecao)
        {
            List<SugestaoDataDisponivel> retorno = new List<SugestaoDataDisponivel>();

            foreach (var item in colecao)
            {
                SugestaoDataDisponivel sugestao = new SugestaoDataDisponivel();
                sugestao.DataEvento = item.MeetingTime;
                sugestao.IsHorarioComercial = item.IsWorkTime;
                sugestao.Qualidade = (SugestaoQualidade)item.Quality;
                foreach (var conflict in item.Conflicts)
                {
                    Conflito conflito = new Conflito();
                    conflito.NumberOfMembers = conflict.NumberOfMembers;
                    conflito.NumberOfMembersAvailable = conflict.NumberOfMembersAvailable;
                    conflito.NumberOfMembersWithConflict = conflict.NumberOfMembersWithConflict;
                    conflito.NumberOfMembersWithNoData = conflict.NumberOfMembersWithNoData;
                    conflito.StatusLegadoLivreOcupado = (StatusLegadoLivreOcupado)conflict.FreeBusyStatus;
                    conflito.TipoConflito = (TipoConflito)conflict.ConflictType;
                    sugestao.Conflitos.Add(conflito);
                }
                retorno.Add(sugestao);
            }

            return retorno;
        }

        private CalendarioEvento converterCalendarEventParaCalendarioEvento(CalendarEvent _event)
        {
            if (_event != null)
            {
                CalendarioEvento retorno = new CalendarioEvento();

                retorno.StartTime = _event.StartTime;
                retorno.EndTime = _event.EndTime;
                retorno.FreeBusyStatus = (StatusLegadoLivreOcupado)_event.FreeBusyStatus;
                if (_event.Details != null)
                {
                    retorno.Details = new CalendarioEventoDetalhe();
                    retorno.Details.IsException = _event.Details.IsException;
                    retorno.Details.IsMeeting = _event.Details.IsMeeting;
                    retorno.Details.IsPrivate = _event.Details.IsPrivate;
                    retorno.Details.IsRecurring = _event.Details.IsRecurring;
                    retorno.Details.IsReminderSet = _event.Details.IsReminderSet;
                    retorno.Details.Location = _event.Details.Location;
                    retorno.Details.StoreId = _event.Details.StoreId;
                    retorno.Details.Subject = _event.Details.Subject;                    
                }
                return retorno;
            }

            return null;
        }

        // anotações
        private void Teste(string storeId, string smtp)
        {
            var convertedId = (AlternateId)service.ConvertId(new AlternateId(IdFormat.HexEntryId, storeId, smtp), IdFormat.EwsId);

            var appointment = Appointment.Bind(service, new ItemId(convertedId.UniqueId));

            foreach (var requiredAttendee in appointment.RequiredAttendees)
            {
                Console.WriteLine(requiredAttendee.Address);
            }

            ItemId appointmentId = new ItemId(convertedId.UniqueId);
            ItemId meetingId = new ItemId(convertedId.UniqueId);

            // Instantiate an appointment object by binding to it by using the ItemId.
            // As a best practice, limit the properties returned to only the ones you need.
            Appointment appointment2 = Appointment.Bind(this.service,
                                                       appointmentId,
                                                       new PropertySet(AppointmentSchema.Subject, AppointmentSchema.Start, AppointmentSchema.End));

            Appointment meeting = Appointment.Bind(this.service, meetingId, new PropertySet(AppointmentSchema.IsMeeting,
                                                                                       AppointmentSchema.ICalUid,
                                                                                       AppointmentSchema.Location,
                                                                                       AppointmentSchema.OptionalAttendees,
                                                                                       AppointmentSchema.RequiredAttendees,
                                                                                       AppointmentSchema.Resources));

            if (appointment == null)
                appointment = new Appointment(this.service);

            appointment.Load(new PropertySet(AppointmentSchema.IsMeeting,
                                                                AppointmentSchema.ICalUid,
                                                                AppointmentSchema.Location,
                                                                AppointmentSchema.OptionalAttendees,
                                                                 AppointmentSchema.RequiredAttendees,
                                                                    AppointmentSchema.Resources));





            string oldSubject = appointment.Subject;

            // Update properties on the appointment with a new subject, start time, and end time.
            appointment.Subject = appointment.Subject + " foi cancelado por falta de pessoas na sala.";

            //appointment.Start.AddHours(25);
            //appointment.End.AddHours(25);

            // Unless explicitly specified, the default is to use SendToAllAndSaveCopy.
            // This can convert an appointment into a meeting. To avoid this,
            // explicitly set SendToNone on non-meetings.
            SendInvitationsOrCancellationsMode mode = appointment.IsMeeting ?
                SendInvitationsOrCancellationsMode.SendToAllAndSaveCopy : SendInvitationsOrCancellationsMode.SendToNone;

            // Send the update request to the Exchange server.

            appointment.Update(ConflictResolutionMode.AlwaysOverwrite, mode);

            // Verify the update.
            //Console.WriteLine("Subject for the appointment was \"" + oldSubject + "\". The new subject is \"" + appointment.Subject + "\"");        

        }

        public void SendmailEWS(string subject, string body, string emails)
        {
            EmailMessage message = new EmailMessage(this.service);
            message.Subject = subject;
            message.Body = body;
            foreach (var item in emails.Split(';'))
            {
                if (item != string.Empty)
                {
                    message.ToRecipients.Add(item);
                }
            }
            message.Send();
        }
        #endregion

    }
}
