﻿using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Infrastructure;
using Microsoft.Owin.Hosting;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace TradeAssist.Realtime.Candlestick
{
    /// <summary>
    /// Entry point for the startup of the entire Service
    /// </summary>
    public class ServiceStartup : IOnCandlesticksAction, IDisposable
    {
        IHubContext hubContext;
        IDisposable singalRStartupDisposeHandle;
        bool isDisposed;

        public async Task Startup(string address)
        {
            try
            {
                singalRStartupDisposeHandle = WebApp.Start<WebAppStartup>(address);
                Console.WriteLine("Server running");

                var dependencyResolver = GlobalHost.DependencyResolver;
                var connectionManager = dependencyResolver.Resolve<IConnectionManager>();
                hubContext = connectionManager.GetHubContext<CandleHub>();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error starting SignalR service");
                Console.WriteLine(ex);
                Dispose();
            }

            if (isDisposed)
                return;

            var candleTracker = CandlestickTracker.Current;
            candleTracker.OnCandlesticksAction = this;

            await candleTracker.Initialize();
        }

        public void Dispose()
        {
            if (isDisposed)
                return;

            isDisposed = true;
            singalRStartupDisposeHandle?.Dispose();
        }

        public void OnCandlesticks(List<Candle> pc)
        {
            var targets = hubContext?.Clients.Group("oneMinute");
            targets.onTick(pc);
        }
    }
}