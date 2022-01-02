#!/usr/bin/python3

import os
import sys
import time

from dataclasses import dataclass


@dataclass
class TaskStatus:
    exit_code: int
    task_name: str


class TaskRunner:
    def __init__(self):
        self.tasks = {}

    def register_task(self, name, command_text, depends_on = []):
        depend_tasks = [lambda: self.tasks[task_name] for task_name in depends_on]
        self.tasks[name] = lambda: self._execute_task(name, command_text, depends_on = depend_tasks)

    def run(self, name):
        task = self.tasks[name]

        if not task:
            raise Exception(f"Command '{command_name}' is not registered.")

        return task()

    def _execute_task(self, name, command_text, **kwargs):
        depends_on = kwargs.get("depends_on")
        if depends_on:
            for prerequisite in depends_on:
                prerequisite_status = prerequisite()()

                if prerequisite_status.exit_code != 0:
                    return prerequisite_status

        print(f"[INFO] Running '{name}' command...")
        print("[INFO] Command: ", command_text)

        start = time.time()
        exit_code = os.system(command_text)
        end = time.time()

        print(f"[INFO] Elapsed time: {end - start}\n")

        return TaskStatus(exit_code, name)
    

# Register new tasks here, the above is just infrastructure for running tasks.
runner = TaskRunner()
runner.register_task("build", "dotnet build --no-restore src/Store")
runner.register_task("test", "dotnet test --no-build src/Store", ["build"])


if __name__ == "__main__":
    command_name = sys.argv[1] if len(sys.argv) > 1 else "build"

    start = time.time()
    status = runner.run(command_name)
    end = time.time()
    print("[INFO] Total elapsed time: ", end - start)

    if status.exit_code != 0:
        print(f"[ERROR] Task '{status.task_name}' failed with code '{status.exit_code}'")
    else:
        print("[INFO] All tasks completed successfully")

    sys.exit(status.exit_code)

