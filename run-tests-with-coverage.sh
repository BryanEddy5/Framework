#!/usr/bin/env bash

# set defaults
export CI_PROJECT_DIR=${CI_PROJECT_DIR:-$PWD}
export TEST_RESULTS_DIR=${TEST_RESULTS_DIR:-$CI_PROJECT_DIR/test-results}
export COBERTURA_REPORT=${COBERTURA_REPORT:-$TEST_RESULTS_DIR/coverage.net5.0.cobertura.xml}
export COVERLET_OUTPUT=${TEST_RESULTS_DIR}/coverlet.output.txt
function run() { echo \'"$@"\'; $@; }

rm -rf $TEST_RESULTS_DIR || true # delete old dir if its there
mkdir -p $TEST_RESULTS_DIR
echo "{}" > $TEST_RESULTS_DIR/coverage.net5.0.json.json

export PATH="$PATH:$HOME/.dotnet/tools"
dotnet tool install -g dotnet-reportgenerator-globaltool || true

# The actual run.
# just in case we need the following flags, here they are :) we seem to be producing the correct numbers without any excludes.
# /p:Exclude='[*.Tests*]*' \
# /p:Exclude='*.Tests*' \
# /p:ExcludeByFile='test/*' \
run dotnet test --test-adapter-path:. -f net5.0 \
  --logger:"junit;LogFilePath=$TEST_RESULTS_DIR/{assembly}.test-result.xml;MethodFormat=Class;FailureBodyFormat=Verbose" \
  /p:CollectCoverage=true \
  /p:CoverletOutputFormat=\"json,cobertura\" \
  /p:CoverletOutput="$TEST_RESULTS_DIR/" /p:MergeWith="$TEST_RESULTS_DIR/coverage.net5.0.json" \
  || export TEST_ERROR=$? # so we don't get early exit, we can process the the report.

reportgenerator "-reports:$COBERTURA_REPORT" \
  -targetdir:"$TEST_RESULTS_DIR/coverage" \
  -reporttypes:"HTML;HTMLSummary"

# https://gitlab.com/gitlab-org/gitlab/-/issues/213444
# point <source> dir to current dir
# add correct subpath to <filename> nodes
sed -i -e "s|filename=\"|filename=\"test/|g; s|.*<source>.*|   <source>./</source>|" $COBERTURA_REPORT
# head -100 $COBERTURA_REPORT # debugging.

echo "Coverage Report in $TEST_RESULTS_DIR/coverage/index.html, summary in $TEST_RESULTS_DIR/coverage/summary.html"

## re-throw error, if there is one
if [[ -n $TEST_ERROR ]]; then
  echo "Tests failed. Exit code: $TEST_ERROR"
  exit $TEST_ERROR
fi
