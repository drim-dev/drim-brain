# OpenTelemetry

OpenTelemetry is an Observability framework and toolkit designed to create and manage _telemetry_ data such as _traces_, _metrics, and _logs_. Crucially, OpenTelemetry is vendor- and tool-agnostic, meaning that it can be used with a broad variety of Observability backends, including open source tools like Jaeger and Prometheus, as well as commercial offerings.

OpenTelemetry is a Cloud Native Computing Foundation (CNCF) project.

OpenTelemetry does two important things:

1. Allows you to own the data that you generate rather than be stuck with a proprietary data format or tool
2. Allows you to learn a single set of APIs and conventions

OpenTelemetry consists of the following major components:

* A _specification_ for all components
* A _standard protocol_ that defines the shape of telemetry data
* Semantic conventions that define a standard naming scheme for common telemetry data types
* _APIs_ that define how to generate telemetry data
* Language SDKs that implement the specification, APIs, and export of telemetry data
* A _library ecosystem_ that implements instrumentation for common libraries and frameworks
* Automatic instrumentation components that generate telemetry data without requiring code changes
* The _OpenTelemetry Collector_, a proxy that receives, processes, and exports telemetry data
* Various other tools, such as the OpenTelemetry Operator for Kubernetes, OpenTelemetry Helm Charts, and community assets for FaaS

![OpenTelemetry](_images/opentelemetry.png)

## OpenTelemetry Protocol (OTLP)

The OpenTelemetry Protocol (OTLP) specification describes the encoding, transport, and delivery mechanism of telemetry data between telemetry sources, intermediate nodes such as collectors and telemetry backends.

OTLP is a general-purpose telemetry data delivery protocol designed in the scope of the OpenTelemetry project.

https://opentelemetry.io/docs/specs/otlp/

## Links

* https://opentelemetry.io/
* https://opentelemetry.io/docs/instrumentation/net/

#open-telemetry
