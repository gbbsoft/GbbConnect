#define SIMULATE_SOC

using GbbEngine.Configuration;

namespace GbbEngine.Server
{
    public partial class JobManager
    {

        private CancellationTokenSource cts = new();

        private object StatisticFileLock = new object();

        public void OurStartJobs(Configuration.Parameters Parameters, GbbLib.IOurLog log)
        {
            // load plant state
            foreach (var plant in Parameters.Plants)
            {
                plant.PlantState = PlantState.OurLoadState(plant);
            }


            Task.Run(() => OurInverterService(Parameters, cts.Token, log), cts.Token);
            Task.Run(() => OurMqttService(Parameters, cts.Token, log), cts.Token);

        }


        public void OurStopJobs(Configuration.Parameters Parameters)
        {
            foreach (var plant in Parameters.Plants)
                if (plant.PlantState!=null)
                    plant.PlantState.OurSaveState();

            cts.Cancel();
        }
    }
}
