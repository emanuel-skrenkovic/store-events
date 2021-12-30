#!/usr/bin/python3

import os
import sys
import time

def build():
    command = "dotnet build --no-restore src/Store"
    print("[INFO] Running 'build' command...")
    print("[INFO] Command: ", command)

    start = time.time()

    os.system(command)

    end = time.time()
    print("[INFO] Elapsed time: ", end - start)


def test():
    build()
    command = "dotnet test --no-build src/Store"
    print("[INFO] Running 'test' command...")
    print("[INFO] Command: ", command)

    start = time.time()

    os.system(command)

    end = time.time()
    print("[INFO] Elapsed time: ", end - start)

def run_command(command_name):
    if command_name == "build":
        build()
    elif command_name == "test":
        test()
    else:
        error("command not recognized")

if __name__ == "__main__":
    command_name = sys.argv[1] if len(sys.argv) > 1 else "build"
    run_command(command_name)
