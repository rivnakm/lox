import os
import subprocess
import time

from common import PROJECT_ROOT, TestCase, build_dotnet, collect_tests

BENCH_DIR = os.path.join(PROJECT_ROOT, "benchmarks")


def run_benchmark(test_case: TestCase, interpreter_path: str) -> int:
    start = time.perf_counter_ns()
    subprocess.run([interpreter_path, test_case.path], check=False, capture_output=True)
    end = time.perf_counter_ns()

    return end - start


def format_time_ns(nanos: int) -> str:
    if nanos < 1_000:
        value = nanos
        unit = "ns"
    elif nanos < 1_000_000:
        value = nanos / 1_000
        unit = "μs"
    elif nanos < 1_000_000_000:
        value = nanos / 1_000_000
        unit = "ms"
    else:
        value = nanos / 1_000_000_000
        unit = "s"
    return f"{value:.3f}{unit}"


def main():
    n = 5
    interpreter_path = build_dotnet(True)
    tests = collect_tests(BENCH_DIR)

    for test in tests:
        sum: int = 0
        print(f"{test.name}: ", end="", flush=True)
        for i in range(n):
            sum += run_benchmark(test, interpreter_path)

        time = int(sum / n)
        print(format_time_ns(time))


if __name__ == "__main__":
    main()
