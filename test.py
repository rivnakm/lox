import os
import subprocess
import xml.etree.ElementTree as ET
from dataclasses import dataclass
from enum import Enum

from termcolor import colored

PROJECT_ROOT = os.path.abspath(os.path.dirname(__file__))
DOTNET_PROJ = os.path.join(PROJECT_ROOT, "lox.net", "Lox", "Lox.csproj")
TESTS_DIR = os.path.join(PROJECT_ROOT, "tests")


@dataclass
class TestCase:
    name: str
    path: str


class TestResult(Enum):
    PASS = 0
    FAIL = 1
    SKIP = 2

    def __str__(self) -> str:
        match self:
            case TestResult.FAIL:
                return colored("FAIL", "red")
            case TestResult.PASS:
                return colored("PASS", "green")
            case TestResult.SKIP:
                return colored("SKIP", "dark_grey")


def get_dotnet_target_framework() -> str:
    tree = ET.parse(DOTNET_PROJ)
    root = tree.getroot()
    tgt = root.find(".//TargetFramework")
    if tgt is None or tgt.text is None:
        raise RuntimeError("Could not read <TargetFramework> element of csproj")
    return tgt.text


def build_dotnet(release: bool) -> str:
    cmd = ["dotnet", "build", DOTNET_PROJ]
    if release:
        cmd += ["--configuration", "Release"]

    proj_dir = os.path.dirname(DOTNET_PROJ)
    return os.path.join(
        proj_dir,
        "bin",
        "Release" if release else "Debug",
        get_dotnet_target_framework(),
        "Lox",
    )


def collect_tests() -> list[TestCase]:
    test_cases = []

    for root, dirs, files in os.walk(TESTS_DIR):
        for file in files:
            if file.endswith(".lox"):
                lox_file_path = os.path.join(root, file)
                category = os.path.relpath(root, TESTS_DIR)
                test_name = os.path.join(category, file.removesuffix(".lox"))

                test_cases.append(TestCase(test_name, lox_file_path))
    return test_cases


def run_test(test_case: TestCase, interpreter_path: str) -> TestResult:
    if os.path.exists(f"{test_case.path}.ignore"):
        return TestResult.SKIP

    run_process = subprocess.run(
        [interpreter_path, test_case.path], check=False, capture_output=True
    )

    expected_stdout = b""
    expected_stderr = b""
    if os.path.exists(f"{test_case.path}.stdout"):
        with open(f"{test_case.path}.stdout", "rb") as stdoutFile:
            expected_stdout = stdoutFile.read()

    if os.path.exists(f"{test_case.path}.stderr"):
        with open(f"{test_case.path}.stderr", "rb") as stderrFile:
            expected_stderr = stderrFile.read()

    if expected_stdout != run_process.stdout or expected_stderr != run_process.stderr:
        return TestResult.FAIL

    return TestResult.PASS


def print_results(results: list[TestResult]):
    num_passed = results.count(TestResult.PASS)
    num_failed = results.count(TestResult.FAIL)
    num_skipped = results.count(TestResult.SKIP)

    print("\nTest Results:")
    num_failed_str = "0" if num_failed == 0 else colored(num_failed, "red")
    print(
        f"{colored(num_passed, 'green')} passed, {num_failed_str} failed, {num_skipped} skipped"
    )

def env_bool(name: str) -> bool:
    return name in os.environ and bool(os.environ[name])

def main():
    interpreter_path = build_dotnet(False)
    tests = collect_tests()
    results = []
    hide_passed = env_bool("HIDE_PASSED")
    fail_fast = env_bool("FAIL_FAST")

    for test in tests:
        result = run_test(test, interpreter_path)
        results.append(result)
        if not (hide_passed and result == TestResult.PASS):
            print(f"{result} {test.name}")

        if fail_fast and result == TestResult.FAIL:
            return

    print_results(results)


if __name__ == "__main__":
    main()
