#!/usr/bin/python3

import os
import sys
import time

from dataclasses import dataclass

@dataclass
class TaskStatus:
    exit_code: int
    task_name: str

def build():
    command = "dotnet build --no-restore src/Store"
    print("[INFO] Running 'build' command...")
    print("[INFO] Command: ", command)

    start = time.time()

    result = os.system(command)

    end = time.time()
    print("[INFO] Elapsed time: ", end - start)

    return TaskStatus(result, "build")


def test():
    build_result = build()
    if build_result.exit_code != 0:
        return build_result

    command = "dotnet test --no-build src/Store"
    print("[INFO] Running 'test' command...")
    print("[INFO] Command: ", command)

    start = time.time()

    result = os.system(command)

    end = time.time()
    print("[INFO] Elapsed time: ", end - start)

    return TaskStatus(result, "test")

def run_command(command_name):
    if command_name == "build":
        return build()
    elif command_name == "test":
        return test()
    else:
        raise Exception("command not recognized")

if __name__ == "__main__":
    command_name = sys.argv[1] if len(sys.argv) > 1 else "build"

    start = time.time()
    status = run_command(command_name)
    end = time.time()
    print("[INFO] Total elapsed time: ", end - start)

    if status.exit_code != 0:
        print(f"[ERROR] Task '{status.task_name}' failed with code '{status.exit_code}'")
    else:
        print("[INFO] All tasks completed successfully")

    sys.exit(status.exit_code)

