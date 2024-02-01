using System;
using Furion.EventBus;
using Microsoft.Extensions.Logging;

namespace NB.Application.EventBus.Test
{
    public class ToDoEventSubscriber : IEventSubscriber, ISingleton
    {
        private readonly ILogger<ToDoEventSubscriber> _logger;
        public ToDoEventSubscriber(ILogger<ToDoEventSubscriber> logger)
        {
            _logger = logger;
        }

        [EventSubscribe("ToDo:Create")]
        public async Task CreateToDo(EventHandlerExecutingContext context)
        {
            var todo = context.Source;
            _logger.LogInformation("创建一个 ToDo：{Name}", todo.Payload);
            await Task.CompletedTask;
        }

    }
}

