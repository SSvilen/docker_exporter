﻿using Prometheus;
using System;

namespace DockerExporter
{
    sealed class ContainerTrackerStateMetrics : IDisposable
    {
        public Gauge.Child RestartCount { get; private set; }
        public Gauge.Child RunningState { get; private set; }
        public Gauge.Child StartTime { get; private set; }

        public ContainerTrackerStateMetrics(string displayName, string hostName)
        {
            RestartCount = BaseRestartCount.WithLabels(displayName, hostName);
            RunningState = BaseRunningState.WithLabels(displayName, hostName);
            StartTime = BaseStartTime.WithLabels(displayName, hostName);
        }

        public void Dispose()
        {
            RestartCount.Remove();
            RunningState.Remove();
            StartTime.Remove();
        }

        public void Unpublish()
        {
            RestartCount.Unpublish();
            RunningState.Unpublish();
            StartTime.Unpublish();
        }

        private static readonly Gauge BaseRestartCount = Metrics
            .CreateGauge("docker_container_restart_count", "Number of times the runtime has restarted this container without explicit user action, since the container was last started.", ConfigureGauge());

        private static readonly Gauge BaseRunningState = Metrics
            .CreateGauge("docker_container_running_state", "Whether the container is running (1), restarting (0.5) or stopped (0).", ConfigureGauge());

        private static readonly Gauge BaseStartTime = Metrics
            .CreateGauge("docker_container_start_time_seconds", "Timestamp indicating when the container was started. Does not get reset by automatic restarts.", ConfigureGauge());

        private static GaugeConfiguration ConfigureGauge() => new GaugeConfiguration
        {
            LabelNames = new[] { "name" , "dockerhost"},
            SuppressInitialValue = true
        };
    }
}
