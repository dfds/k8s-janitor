﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace K8sJanitor.WebApi.Infrastructure.Messaging
{
    public class KafkaConsumerHostedService : IHostedService
    {
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private readonly ILogger<KafkaConsumerHostedService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IEnumerable<string> _topics;

        private Task _executingTask;

        public KafkaConsumerHostedService(ILogger<KafkaConsumerHostedService> logger, IServiceProvider serviceProvider)
        {
            Console.WriteLine($"Starting Kafka event consumer.");

            _logger = logger;
            _serviceProvider = serviceProvider;

            var eventRegistry = _serviceProvider.GetRequiredService<IDomainEventRegistry>();
            _topics = eventRegistry.GetAllTopics();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _executingTask = Task.Factory.StartNew(async () =>
                {
                    var consumerFactory = _serviceProvider.GetRequiredService<KafkaConsumerFactory>();

                    using (var consumer = consumerFactory.Create())
                    {
                        _logger.LogInformation(
                            $"Event consumer started. Listening to topics: {string.Join(",", _topics)}");
                        consumer.Subscribe(_topics);

                        // consume loop
                        while (!_cancellationTokenSource.IsCancellationRequested)
                        {
                            ConsumeResult<string, string> msg;
                            try
                            {
                                msg = consumer.Consume(cancellationToken);
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError($"Consumption of event failed, reason: {ex}");
                                continue;
                            }

                            using (var scope = _serviceProvider.CreateScope())
                            {
                                _logger.LogInformation(
                                    $"Received event: Topic: {msg.Topic} Partition: {msg.Partition}, Offset: {msg.Offset} {msg.Value}");

                                try
                                {
                                    var eventDispatcher = scope.ServiceProvider.GetRequiredService<IEventDispatcher>();
                                    await eventDispatcher.Send(msg.Value);
                                    await Task.Run(() => consumer.Commit(msg));
                                }
                                catch (Exception ex) when (ex is EventTypeNotFoundException || ex is EventHandlerNotFoundException )
                                {
                                    _logger.LogWarning($"Message skipped. Exception message: {ex.Message}", ex);
                                    await Task.Run(() => consumer.Commit(msg));
                                }
                                catch (Exception ex)
                                {
                                    _logger.LogError($"Error consuming event. Exception message: {ex.Message}", ex);
                                    _logger.LogError(ex.StackTrace);
                                    _cancellationTokenSource.Cancel();
                                    System.Environment.Exit(1);
                                    //throw;
                                }
                            }
                        }
                    }
                }, _cancellationTokenSource.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default)
                .ContinueWith(task =>
                {
                    if (task.IsFaulted)
                    {
                        _logger.LogError(task.Exception, "Event loop crashed");
                    }
                }, cancellationToken);

            return Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            try
            {
                _cancellationTokenSource.Cancel();
            }
            finally
            {
                await Task.WhenAny(_executingTask, Task.Delay(-1, cancellationToken));
            }

            _cancellationTokenSource.Dispose();
        }
    }
}