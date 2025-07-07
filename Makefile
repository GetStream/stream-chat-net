STREAM_KEY ?= NOT_EXIST
STREAM_SECRET ?= NOT_EXIST
DOTNET_VERSION ?= 6.0
STREAM_CHAT_URL ?= https://chat.stream-io-api.com
TEST_FILTER ?= 

# Fixed constants
RUNSETTINGS_FILE := .runsettings

# These targets are not files
.PHONY: help build test test_with_docker test_filtered test_filtered_with_docker restore clean generate_runsettings

# Define a function to generate runsettings content
define generate_runsettings_content
	@echo "<?xml version=\"1.0\" encoding=\"utf-8\"?>" > $(1)
	@echo "<RunSettings>" >> $(1)
	@echo "  <RunConfiguration>" >> $(1)
	@echo "    <EnvironmentVariables>" >> $(1)
	@echo "      <STREAM_KEY>$(STREAM_KEY)</STREAM_KEY>" >> $(1)
	@echo "      <STREAM_SECRET>$(STREAM_SECRET)</STREAM_SECRET>" >> $(1)
	@echo "      <STREAM_CHAT_URL>$(STREAM_CHAT_URL)</STREAM_CHAT_URL>" >> $(1)
	@echo "    </EnvironmentVariables>" >> $(1)
	@echo "  </RunConfiguration>" >> $(1)
	@echo "</RunSettings>" >> $(1)
endef

help: ## Display this help message
	@echo "Please use \`make <target>\` where <target> is one of"
	@grep -E '^[a-zA-Z_-]+:.*?## .*$$' $(MAKEFILE_LIST) | sort | awk 'BEGIN {FS = ":.*?## "}; \
	{printf "\033[36m%-40s\033[0m %s\n", $$1, $$2}'

restore: ## Restore dependencies
	dotnet restore

build: restore ## Build the project
	dotnet build

generate_runsettings: ## Generate .runsettings file with current environment variables
	$(call generate_runsettings_content,$(RUNSETTINGS_FILE))
	@echo "Generated $(RUNSETTINGS_FILE) with current environment variables"

test: generate_runsettings ## Run tests
	dotnet test -s $(RUNSETTINGS_FILE)

test_filtered: generate_runsettings ## Run tests with a filter (set TEST_FILTER to specify test name)
	dotnet test -s $(RUNSETTINGS_FILE) --filter "$(TEST_FILTER)"

clean: ## Clean build artifacts
	dotnet clean
	rm -f $(RUNSETTINGS_FILE).tmp

lint: ## Run linting
	dotnet build --verbosity quiet

test_with_docker: ## Run tests in Docker (set DOTNET_VERSION to change .NET version)
	$(call generate_runsettings_content,$(RUNSETTINGS_FILE).tmp)
	docker run -t -i -w /code -v $(PWD):/code --add-host=host.docker.internal:host-gateway \
		mcr.microsoft.com/dotnet/sdk:$(DOTNET_VERSION) \
		sh -c "dotnet restore && dotnet test -s $(RUNSETTINGS_FILE).tmp"
	rm -f $(RUNSETTINGS_FILE).tmp

test_filtered_with_docker: ## Run filtered tests in Docker (set TEST_FILTER to specify test name)
	$(call generate_runsettings_content,$(RUNSETTINGS_FILE).tmp)
	docker run -t -i -w /code -v $(PWD):/code --add-host=host.docker.internal:host-gateway \
		mcr.microsoft.com/dotnet/sdk:$(DOTNET_VERSION) \
		sh -c "dotnet restore && dotnet test -s $(RUNSETTINGS_FILE).tmp --filter \"$(TEST_FILTER)\""
	rm -f $(RUNSETTINGS_FILE).tmp