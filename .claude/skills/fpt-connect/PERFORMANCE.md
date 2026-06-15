# PERFORMANCE.md — Chuẩn hiệu năng & mở rộng

> SLO (Bible 3.6/7.12): P95 read < 500ms, write < 800ms @ 300 RPS; map 95% < 2s với ≤ 2.000 marker cluster; 20.000 user / 5.000 concurrent / 10 triệu GPS điểm/ngày.

## Query & DB
- Tránh N+1 (`Include`/projection hợp lý); seek/cursor pagination, không offset cho bảng lớn.
- Index theo access pattern; covering index cho list hot; kiểm tra query plan trước merge.
- RoutePoints partition theo ngày; ingest batch (20–100 điểm) + xử lý async qua queue.

## Read model & cache
- Dashboard dùng read-model/aggregate có `CalculatedAtUtc`, rebuild được; cache 1–5 phút.
- Redis cache permission/settings/geocode; không cache token/secret/raw GPS.

## API & resilience
- Timeout ngắn theo dependency; retry chỉ idempotent; circuit breaker Maps/AI/notify; bulkhead cho AI.
- Export > 5.000 dòng chạy background + streaming.

## Frontend
- Map: cluster marker, list alternative, không render hàng nghìn DOM marker.
- Code-split route; skeleton; debounce search; virtual list cho timeline dài.

## Đo lường
k6 load/stress/soak đạt SLO; golden signals (traffic/error/latency/saturation) + alert.
