# Performance testing of CqrsLite
Running on Dell Precision T3610 with 
specs:
Intel Xeon E5-1650v2@3.5 GHz
32 GB DDR ram
Samsung SSD SM841 512GB
Windows 10

## .net full framework
- 0.3.4   - exec: ~57s. per ms: ~1100
- 0.9.9   - exec: ~37s. per ms: ~1700
- 0.11.36 - exec: ~63s. per ms: ~980
- 0.12.0  - exec: ~65s. per ms: ~965
- 0.13.2  - exec: ~10s. per ms: ~6200
- 0.14.4  - exec: ~10s. per ms: ~6200
- 0.15.1  - exec: ~8s.  per ms: ~8000
- 0.16.0  - exec: ~8s.  per ms: ~8200
- 0.17.0  - exec: ~6.2s.  per ms: ~10000

## dotnet core
- 0.12.0  - exec: ~79s. per ms: ~800
- 0.13.2  - exec: ~8.5s. per ms: ~7400
- 0.14.4  - exec: ~8.5s. per ms: ~7400
- 0.15.1  - exec: ~6.3s. per ms: ~10000
- 0.16.0  - exec: ~6.3s. per ms: ~10000
- 0.17.0  - exec: ~5.0s.  per ms: ~12500
