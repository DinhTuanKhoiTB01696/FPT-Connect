# DEVOPS.md — Vận hành & quan sát

> Bible 3.7/3.8 + chương 13.

## Observability
- Serilog JSON: `timestamp, level, traceId, userId(pseudonym), tenantId, endpoint, status, durationMs`. 100% request có correlation ID.
- Golden signals + alert: 5xx > 2%/5', P95 > SLO/10', GPS ingest lag > 5', job retry/dead-letter, security spike.
- Health: `/health/live` (không phụ thuộc ngoài), `/health/ready` (DB/Redis) → LB.

## Backup & recovery
- Full hằng ngày, differential mỗi 6h, log mỗi 15'; backup mã hoá, immutable/offsite; restore test hằng quý.
- Runbook: failover, corruption, credential leak, Maps/AI outage.

## Background jobs
Outbox/reminder/route-aggregation/retention qua Worker; retry exponential + jitter + dead-letter; consumer idempotent (inbox/dedup key).

## Hạ tầng khi scale
SignalR Redis backplane; ≥ 2 API instance sau LB; SQL Server HA (Always On/managed).
