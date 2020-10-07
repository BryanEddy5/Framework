CI_PROJECT_DIR=$(shell pwd)
CI_COMMIT_SHORT_SHA=$(shell git rev-parse --short HEAD)
CI_PROJECT_NAME='webcore'
BRANCH='branch'
JOB='test'

gci:
	# 1) local gitlab-runner only pulls things that have been committed - temporarily commit
	(git add . && git commit -am "gitlab CI testing - temp commit") || true
	gitlab-runner exec docker ${JOB} \
		--env CI_COMMIT_SHORT_SHA=${CI_COMMIT_SHORT_SHA} \
		--env CI_COMMIT_REF_SLUG=${BRANCH} \
		--env CI_PROJECT_DIR=${CI_PROJECT_DIR} \
		--env CI_PROJECT_NAME=${CI_PROJECT_NAME} \
		--docker-volumes /var/run/docker.sock:/var/run/docker.sock \
		|| true # want to still run git reset after.

	# 2) then reset the changes
	git reset --soft HEAD~1 # pop last commit (don't nuke changes)

default: gci

# --env SECRETS_feature="sec/_secrets.json" \
# --docker-volumes ${CURRENT_DIR}/sec:/sec \
