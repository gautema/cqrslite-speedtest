# Performance testing of CqrsLite

## .net full framework
- 0.3.4   - exec: ~57s. per ms: ~1100
- 0.9.9   - exec: ~37s. per ms: ~1700
- 0.11.36 - exec: ~63s. per ms: ~980
- 0.12.0  - exec: ~65s. per ms: ~965
- 0.13.2  - exec: ~10s. per ms: ~6200
- 0.14.4  - exec: ~10s. per ms: ~6200
- 0.15.1  - exec: ~8s.  per ms: ~8000
- 0.16.0  - exec: ~8s.  per ms: ~8200

## dotnet core
- 0.12.0  - exec: ~79s. per ms: ~800
- 0.13.2  - exec: ~8.5s. per ms: ~7400
- 0.14.4  - exec: ~8.5s. per ms: ~7400
- 0.15.1  - exec: ~6.3s. per ms: ~10000
- 0.16.0  - exec: ~6.3s. per ms: ~10000