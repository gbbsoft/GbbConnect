#define SIMULATE_SOC

using GbbEngine.Configuration;

namespace GbbEngine.Server
{
    public partial class JobManager
    {

        private CancellationTokenSource cts = new();

        public void OurStartJobs(Configuration.Parameters Parameters, GbbLib.IOurLog log)
        {
            // load plant state
            foreach (var plant in Parameters.Plants)
            {
                plant.PlantState = PlantState.OurLoadState(plant);

#if DEBUG && SIMULATE_SOC
                var rnd = new Random();
                plant.PlantState.SOC = rnd.Next(100);
#endif
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
