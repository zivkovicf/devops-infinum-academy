#!/usr/bin/env bash

set -e

export PATH=$PATH:/usr/local/bin

# Store and return last failure from fmt so this can validate every directory passed before exiting
LINT_ERROR=0

for file in "$@"; do
  jsonlint -i "$file" || LINT_ERROR=$?
done

exit ${LINT_ERROR}
