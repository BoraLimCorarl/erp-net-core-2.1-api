using Amazon;
using Amazon.Scheduler;
using CorarlERP.EventBridge.Dto;
using CorarlERP.Configuration;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Amazon.Scheduler.Model;

namespace CorarlERP.EventBridge
{
    public class EventBridgeManager : CorarlERPDomainServiceBase, IEventBridgeManager
    {
        private readonly IConfigurationRoot _configuration;
        //private readonly AmazonEventBridgeClient _amazonEventBridgeClient;
        //private readonly AmazonLambdaClient _awsLambdaClient;
        private readonly AmazonSchedulerClient _awsSchduleClient;
        private readonly AmazonSchedulerClient _awsSchduleClientRefreshToken;
        private readonly RegionEndpoint region = RegionEndpoint.APSoutheast1;

        private string awsAccessKeyId => _configuration.GetSection("AWS:Scheduler:AccessKey").Value;
        private string awsSecretAccessKey => _configuration.GetSection("AWS:Scheduler:SecretKey").Value;
        private string targetArn => _configuration.GetSection("AWS:Scheduler:TargetArn").Value;
        private string targetRoleArn => _configuration.GetSection("AWS:Scheduler:TargetRoleArn").Value;
        private string schedulePrefixName => _configuration.GetSection("AWS:Scheduler:Prefix").Value;
        private string scheduleGroupName => _configuration.GetSection("AWS:Scheduler:ScheduleGroupName").Value;

        public EventBridgeManager(IAppConfigurationAccessor configuration)
        {
            _configuration = configuration.Configuration;
            _awsSchduleClient = new AmazonSchedulerClient(awsAccessKeyId, awsSecretAccessKey, region);         
        }


        public async Task CreateEventBridge(CreateEventBridgeInput input)
        {
            var name = schedulePrefixName + input.JobId;
            input.Input.JobId = input.JobId;


            var scheduleRequest = new Amazon.Scheduler.Model.CreateScheduleRequest()
            {
                Name = name,
                ScheduleExpression = input.Expression,
                ScheduleExpressionTimezone = "UTC",
                FlexibleTimeWindow = new Amazon.Scheduler.Model.FlexibleTimeWindow()
                {
                    Mode = FlexibleTimeWindowMode.OFF
                },
                GroupName = scheduleGroupName,
                Target = new Amazon.Scheduler.Model.Target()
                {
                    Arn = targetArn,
                    RetryPolicy = new Amazon.Scheduler.Model.RetryPolicy()
                    {
                        MaximumRetryAttempts = 0,
                    },
                    Input = JsonConvert.SerializeObject(input.Input),
                    RoleArn = targetRoleArn
                },
                ActionAfterCompletion = ActionAfterCompletion.NONE,


            };

            await _awsSchduleClient.CreateScheduleAsync(scheduleRequest);


        }
        private async Task<bool> GetEventBridge(long jobId)
        {
            var name = schedulePrefixName + jobId;
            var input = new GetScheduleRequest
            {
                GroupName = scheduleGroupName,
                Name = name
            };
            var scheduler = await _awsSchduleClient.GetScheduleAsync(input);
            return scheduler != null && scheduler.Name == name;
        }


        public async Task DeleteEventBridge(DeleteEventBridgeInput input)
        {
            //if (!await GetEventBridge(input.JobId.Value)) return;

            var name = schedulePrefixName + input.JobId;
            var deleteScheduleRequest = new Amazon.Scheduler.Model.DeleteScheduleRequest()
            {
                GroupName = scheduleGroupName,
                Name = name
            };
            var deleteScheduler = await _awsSchduleClient.DeleteScheduleAsync(deleteScheduleRequest);

        }
    }
}
