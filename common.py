import os
import subprocess
import xml.etree.ElementTree as ET
from dataclasses import dataclass

PROJECT_ROOT = os.path.abspath(os.path.dirname(__file__))
DOTNET_PROJ = os.path.join(PROJECT_ROOT, "lox.net", "Lox", "Lox.csproj")


@dataclass
class TestCase:
    name: str
    path: str


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
        
    subprocess.run(cmd, check=True)

    proj_dir = os.path.dirname(DOTNET_PROJ)
    return os.path.join(
        proj_dir,
        "bin",
        "Release" if release else "Debug",
        get_dotnet_target_framework(),
        "Lox",
    )


def env_bool(name: str) -> bool:
    return name in os.environ and bool(os.environ[name])


def collect_tests(dir: str) -> list[TestCase]:
    test_cases = []

    for root, dirs, files in os.walk(dir):
        for file in files:
            if file.endswith(".lox"):
                lox_file_path = os.path.join(root, file)
                category = os.path.relpath(root, dir)
                test_name = os.path.join(category, file.removesuffix(".lox"))

                test_cases.append(TestCase(test_name, lox_file_path))
    return test_cases
