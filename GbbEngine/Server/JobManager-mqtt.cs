using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using GbbEngine.Configuration;
using MQTTnet;
using GbbConnectProtocol;
using GbbLibSmall;

namespace GbbEngine.Server
{
    public partial class JobManager
    {
        private static MqttClientFactory mqttFactory = new();

        private async void OurMqttService(Configuration.Parameters Parameters, CancellationToken ct, IOurLog log)
        {

            try
            {
#if DEBUG
                // wait for server to start
                await Task.Delay(5 * 1000, ct);
#endif

                while (!ct.IsCancellationRequested)
                {
                    try
                    {
                        // start
                        //log.OurLog(LogLevel.Information, "MqttService: starting");
                        await OurMqttService_DoWork(Parameters, ct, log);

                    }
                    catch (TaskCanceledException)
                    {
                    }
                    catch (Exception ex) // eg System.Threading.Tasks.TaskCanceledException
                    {
                        // log
                        log.OurLog(LogLevel.Error, $"MqttServices: {ex}");
                    }

                    // try again after delay
                    if (!ct.IsCancellationRequested)
                    {
#if DEBUG
                        log.OurLog(LogLevel.Error, $"MqttServices: waiting 10 sec");
                        await Task.Delay(10 * 1000, ct);
#else
                        log.OurLog(LogLevel.Error, $"MqttServices: waiting 5min");
                        await Task.Delay(5*60 * 1000, ct);
#endif
                    }
                }
            }
            catch(TaskCanceledException)
            {
            }
            // log
            log.OurLog(LogLevel.Information, "MqttService: finished");
        }


        private async Task ConnectToMqtt(Parameters Parameters, Plant plant, IMqttClient client, CancellationToken ct, IOurLog log)
        {
            var b = new MqttClientOptionsBuilder()
                .WithClientId($"GbbConnect_{plant.GbbVictronWeb_PlantId.ToString()}")
                .WithCleanSession(true)
                .WithTlsOptions(new MqttClientTlsOptions()
                {
                    UseTls = true,
                    // 2023-12-15: nie ma juz potrzeby
                    //IgnoreCertificateChainErrors = true,
                })
                .WithTcpServer(Parameters.GbbVictronWeb_Mqtt_Address, Parameters.GbbVictronWeb_Mqtt_Port)
                .WithCredentials(plant.GbbVictronWeb_PlantId.ToString(), plant.GbbVictronWeb_PlantToken);
            //.WithSessionExpiryInterval(2*60); // nie wiadomo w jakich to jest jednostkach!

            // connect
            await client.ConnectAsync(b.Build(), ct);


            // subsribe
            await client.SubscribeAsync(
                mqttFactory.CreateSubscribeOptionsBuilder()
                .WithTopicFilter(q =>
                        q.WithTopic($"{plant.GbbVictronWeb_PlantId.ToString()}/datarequest")
                        .WithAtLeastOnceQoS()
                        )
                .Build()
                , ct);

        }

        private async Task OurMqttService_DoWork(Configuration.Parameters Parameters, CancellationToken ct, IOurLog log)
        {

            int Counter = 0;

            try
            {
                while (!ct.IsCancellationRequested)
                {
                    DateTime LoopStartTime = DateTime.Now;

                    // =====================================
                    // Connect / Reconnect
                    // =====================================
                    foreach (var plant in Parameters.Plants)
                    {
                        try
                        {
                            if (plant.PlantState!=null && plant.PlantState.MqttClient==null
                                && plant.IsDisabled == 0 && plant.GbbVictronWeb_PlantId != null && plant.GbbVictronWeb_PlantToken != null)
                            {
                                log.OurLog(LogLevel.Information, $"{plant.Name}: Starting Mqtt");
                                var client = mqttFactory.CreateMqttClient();

                                // callback
                                client.ApplicationMessageReceivedAsync += e => { return MqttClient_MessageReceivedAsync(Parameters, e, plant, log); };

                                // Use builder classes where possible in this project.
                                await ConnectToMqtt(Parameters, plant, client, ct, log);

                                // save client
                                plant.PlantState!.MqttClient = client;

                                log.OurLog(LogLevel.Information, $"{plant.Name}: Started Mqtt");
                                Counter++;
                            }


                            // reconnect
                            if (plant.PlantState != null && plant.PlantState.MqttClient != null
                                && !plant.PlantState.MqttClient.IsConnected)
                            {
                                log.OurLog(LogLevel.Information, $"{plant.Name}: Mqtt: Reconnect");
                                await ConnectToMqtt(Parameters, plant, plant.PlantState.MqttClient, ct, log);
                                log.OurLog(LogLevel.Information, $"{plant.Name}: Mqtt: Reconnected");
                            }
                        }
                        catch (TaskCanceledException)
                        {
                        }
                        catch (Exception ex)
                        {
                            log.OurLog(LogLevel.Error, $"{plant.Name}: {ex.Message}");
                        }

                        ct.ThrowIfCancellationRequested();
                    }
                    // nothing to do
                    if (Counter == 0)
                        break;


                    // ===============
                    // keep alive
                    // ===============
                    foreach (var plant in Parameters.Plants)
                    {
                        ct.ThrowIfCancellationRequested();

                        try
                        {
                            if (plant.PlantState!.MqttClient!= null && plant.PlantState!.MqttClient.IsConnected)
                            {

                                if (Parameters.IsVerboseLog)
                                {
                                    log.OurLog(LogLevel.Information, $"{plant.Name}: Mqtt: Sending keepalive");
                                }


                                await plant.PlantState!.MqttClient.PublishAsync(
                                    new MqttApplicationMessageBuilder()
                                    .WithTopic($"{plant.GbbVictronWeb_PlantId.ToString()}/keepalive")
                                    .WithQualityOfServiceLevel(MQTTnet.Protocol.MqttQualityOfServiceLevel.AtLeastOnce)
                                    .Build()
                                    , ct);
                            }
                        }
                        catch (TaskCanceledException)
                        {
                        }
                        catch (Exception ex) 
                        {
                            log.OurLog(LogLevel.Error, $"{plant.Name}: Mqtt: {ex.Message}");
                        }
                    }

                    // =====================================
                    // try keep 1min between keep-alive
                    // =====================================
                    var ms = (int)(LoopStartTime.AddMinutes(1) - DateTime.Now).TotalMilliseconds;
                    if (ms > 0)
                        await Task.Delay(ms, ct);
                }
            }
            finally
            {
                foreach(var plant in Parameters.Plants)
                {
                    if (plant.PlantState!.MqttClient != null)
                    {
                        try
                        {
                            // disconnect
                            log.OurLog(LogLevel.Information, $"{plant.Name}: Disconnecting Mqtt");
                            var mqttClientDisconnectOptions = mqttFactory.CreateClientDisconnectOptionsBuilder().Build();
                            await plant.PlantState!.MqttClient.DisconnectAsync(mqttClientDisconnectOptions, CancellationToken.None);
                        }
                        catch (Exception ex)
                        {
                            log.OurLog(LogLevel.Error, $"{plant.Name}: {ex.Message}");
                        }
                    }
                }

            }

        }


        // ======================================


        // ======================================


        private async Task MqttClient_MessageReceivedAsync(Configuration.Parameters Parameters, MqttApplicationMessageReceivedEventArgs arg, Configuration.Plant Plant, IOurLog log)
        {
            string? Operation = null;

            // options for json serialization
            var SerOpt = new JsonSerializerOptions();
            SerOpt.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;


            try
            {
                var seg = arg.ApplicationMessage.ConvertPayloadToString();
                if (Parameters.IsVerboseLog)
                {
                    log.OurLog(LogLevel.Information, $"{Plant.Name}: Mqtt: Received request: {seg}");
                }


                var ReqOpt = new JsonSerializerOptions();
                var Request = JsonSerializer.Deserialize<Request>(seg, ReqOpt);
                if (Request != null && Plant.PlantState!.MqttClient!=null)
                {
                    Operation = Request.Operation;
                    if (!Parameters.IsVerboseLog)
                    {
                        log.OurLog(LogLevel.Information, $"{Plant.Name}: Mqtt: Received request: {Operation} {Request.FromDate} {Request.ToDate}");
                    }



                    var Response = new Response();
                    Response.Operation = Operation;
                    Response.Status = "OK";

                    switch (Request.Operation)
                    {
                        case "GetSOC":
                            Response.SOC = Plant.PlantState.SOC;
                            break;

                        case "GetStatistics":
                            if (Request.FromDate == null || Request.ToDate == null)
                            {
                                Response.Status = "ERR";
                                Response.ErrDesc = "Missing FromDate or ToDate";
                            }
                            else
                                GetStatistics(Request.FromDate.Value, Request.ToDate.Value, Response, Plant, log);

                            break;

                        case "SetSchedulers":
                            // save schedulers to send to inverters
                            Plant.PlantState.Schedulers = Request.Schedulers;
                            Plant.PlantState.SchedulersReadyToProcess = true;
                            Plant.PlantState.OurSaveState();
                            break;

                        default:
                            Response.Status = "ERR";
                            Response.ErrDesc = "Uknown operation: " + Operation;
                            break;

                    }



                    var msg = JsonSerializer.Serialize(Response, SerOpt);
                    if (Parameters.IsVerboseLog)
                    {
                        log.OurLog(LogLevel.Information, $"{Plant.Name}: Mqtt: Sending response: {msg}");
                    }

                    // send response
                    await Plant.PlantState!.MqttClient.PublishAsync(
                        new MqttApplicationMessageBuilder()
                       .WithTopic($"{Plant.GbbVictronWeb_PlantId.ToString()}/dataresponse")
                       .WithPayload(msg)
                       .WithQualityOfServiceLevel(MQTTnet.Protocol.MqttQualityOfServiceLevel.AtLeastOnce)
                       .Build()
                       , CancellationToken.None);

                }

            }
            catch (Exception ex)
            {
                log.OurLog(LogLevel.Error, $"{Plant.Name}: {ex.Message}");

                if (Operation != null && Plant.PlantState!.MqttClient != null)
                {
                    try
                    {
                        // send error resonse
                        var Res = new Response();
                        Res.Operation = Operation;
                        Res.Status = "ERROR";
                        Res.ErrDesc = ex.Message;

                        await Plant.PlantState!.MqttClient.PublishAsync(
                            new MqttApplicationMessageBuilder()
                           .WithTopic($"{Plant.GbbVictronWeb_PlantId.ToString()}/dataresponse")
                           .WithPayload(JsonSerializer.Serialize(Res, SerOpt))
                           .WithQualityOfServiceLevel(MQTTnet.Protocol.MqttQualityOfServiceLevel.AtLeastOnce)
                           .Build()
                           , CancellationToken.None);

                    }
                    catch(Exception ex2)
                    {
                        log.OurLog(LogLevel.Error, $"{Plant.Name}: {ex2.Message}");
                    }

                }
                
            }


        }

        /// <summary>
        /// Fill Responce with statistics from files
        /// </summary>
        /// <param name="FromDate"></param>
        /// <param name="ToDate"></param>
        /// <param name="Response"></param>
        /// <param name="Plant"></param>
        /// <param name="log"></param>
        private void GetStatistics(DateTime FromDate, DateTime ToDate, Response Response, Plant Plant, IOurLog log)
        {
            CultureInfo ci = CultureInfo.InvariantCulture;

            Response.Statistics = new();
            Response.FromDate = FromDate;
            Response.ToDate = ToDate;

            lock (StatisticFileLock)
            {

                for (DateTime date = FromDate.Date; date <= ToDate; date = date.AddDays(1))
                {
                    string FileName = OurGetStatFileName(Plant, date);
                    if (File.Exists(FileName))
                    {
                        try
                        {
                            string s = File.ReadAllText(FileName);
                            string[] lines = s.Split(Environment.NewLine);
                            bool FirstLine = true;
                            foreach (var line in lines)
                            {
                                if (FirstLine)
                                    // skip header
                                    FirstLine = false;
                                else
                                {
                                    string[] col = line.Split('\t');
                                    if (col.Length > 1)
                                    {
                                        int Pos = 0;
                                        var itm = new Response_Statistic();
                                        itm.Day = DateTime.ParseExact(col[Pos++], "yyyy-MM-dd", ci);
                                        itm.Hour = int.Parse(col[Pos++], ci);

                                        if (col.Length > Pos && col[Pos] != null)
                                            itm.SOC = decimal.Parse(col[Pos], ci);
                                        Pos++;

                                        if (col.Length > Pos && col[Pos] != null)
                                            itm.MinSOC = decimal.Parse(col[Pos], ci);
                                        Pos++;

                                        if (col.Length > Pos && col[Pos] != null)
                                            itm.MaxSOC = decimal.Parse(col[Pos], ci);
                                        Pos++;

                                        if (col.Length > Pos && col[Pos] != null)
                                            itm.AvrSOC = decimal.Parse(col[Pos], ci);
                                        Pos++;

                                        if (col.Length > Pos && col[Pos] != null)
                                            itm.PVProdkWh = decimal.Parse(col[Pos], ci);
                                        Pos++;

                                        if (col.Length > Pos && col[Pos] != null)
                                            itm.FromGridkWh = decimal.Parse(col[Pos], ci);
                                        Pos++;

                                        if (col.Length > Pos && col[Pos] != null)
                                            itm.ToGridkWh = decimal.Parse(col[Pos], ci);
                                        Pos++;

                                        if (col.Length > Pos && col[Pos] != null)
                                            itm.LoadskWh = decimal.Parse(col[Pos], ci);
                                        Pos++;

                                        Response.Statistics.Add(itm);

                                    }
                                }
                            }

                        }
                        catch (Exception ex)
                        {
                            log.OurLog(LogLevel.Error, $"{Plant.Name}: GetStatistics: {date:d}: ERROR: {ex.Message}");
                        }

                    }


                }

            }
        }


    }
}
