﻿using RabbitMQ.Client;
using System;

namespace EventBus
{
    public interface IRabbitMQConnection : IDisposable
    {
        bool IsConnected { get; }

        bool TryConnect();

        IModel CreateModel();
    }
}