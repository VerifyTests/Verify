#!/usr/bin/env bash
# Run each test class on net10 in isolation with a 60s timeout.
# Anything that takes longer is presumed hung; report and continue.

set +e

CLASSES_FILE=/tmp/test-classes.txt
LOG=/tmp/per-class.log
> "$LOG"

dotnet test src/Verify.Tests/Verify.Tests.csproj --no-build --nologo -f net10.0 --list-tests 2>&1 | grep "^    " | sed 's/^    //;s/\..*//' | sort -u > "$CLASSES_FILE"

while read class; do
  echo "=== $class ===" | tee -a "$LOG"
  start=$(date +%s)
  # Wrap with timeout via bash background+kill
  ( dotnet test src/Verify.Tests/Verify.Tests.csproj --no-build --nologo -f net10.0 --filter "FullyQualifiedName~${class}." > /tmp/c.txt 2>&1 ) &
  TEST_PID=$!
  for i in 1 2 3 4 5 6; do
    sleep 10
    if ! kill -0 $TEST_PID 2>/dev/null; then
      break
    fi
  done
  if kill -0 $TEST_PID 2>/dev/null; then
    echo "$class HUNG (>60s)" | tee -a "$LOG"
    kill -9 $TEST_PID 2>/dev/null
    # Best-effort cleanup of any spawned testhost/dotnet child processes
    taskkill //F //IM testhost.exe 2>/dev/null
  else
    end=$(date +%s)
    elapsed=$((end-start))
    result=$(grep -E "Failed!|Passed!  -" /tmp/c.txt | tail -1)
    echo "$class ${elapsed}s $result" | tee -a "$LOG"
  fi
done < "$CLASSES_FILE"
