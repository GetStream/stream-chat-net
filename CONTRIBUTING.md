# :recycle: Contributing

We welcome code changes that improve this library or fix a problem, please make sure to follow all best practices and add tests if applicable before submitting a Pull Request on Github. We are very happy to merge your code in the official repository. Make sure to sign our [Contributor License Agreement (CLA)](https://docs.google.com/forms/d/e/1FAIpQLScFKsKkAJI7mhCr7K9rEIOpqIDThrWxuvxnwUq2XkHyG154vQ/viewform) first. See our license file for more details.

## Getting started

### Restore dependencies and build

Most IDEs automatically prompt you to restore the dependencies but if not, use this command:

```shell
$ dotnet restore
```

Or use the Makefile:

```shell
$ make restore
```

Building:

```shell
$ dotnet build
```

Or use the Makefile:

```shell
$ make build
```

### Run tests

The tests we have are full fledged integration tests, meaning they will actually reach out to a Stream app. Hence the tests require at least two environment variables: `STREAM_KEY` and `STREAM_SECRET`.

#### Using .runsettings file

To have these env vars available for the test running, you need to set up a `.runsettings` file in the root of the project (don't worry, it's gitignored).

> :bulb Microsoft has a super detailed [documentation](https://docs.microsoft.com/en-us/visualstudio/test/configure-unit-tests-by-using-a-dot-runsettings-file) about .runsettings file.
  
It needs to look like this:

```xml
<?xml version="1.0" encoding="utf-8"?>
<RunSettings>
  <RunConfiguration>
    <EnvironmentVariables>
      <STREAM_KEY>api_key_here</STREAM_KEY>
      <STREAM_SECRET>secret_key_here</STREAM_SECRET>
    </EnvironmentVariables>
  </RunConfiguration>
</RunSettings>
```

You can generate this file using the Makefile:

```shell
$ make generate_runsettings STREAM_KEY=api_key_here STREAM_SECRET=secret_key_here
```

#### Running tests

In CLI:
```shell
$ dotnet test -s .runsettings
```

Using the Makefile:
```shell
$ make test
```

To run specific tests:
```shell
$ make test_filtered TEST_FILTER="FullyQualifiedName~StreamChatTests.StreamClientFactoryTests"
```

#### Running tests in Docker

You can also run tests in Docker using the Makefile:

```shell
$ make test_with_docker
```

To run specific tests in Docker:
```shell
$ make test_filtered_with_docker TEST_FILTER="FullyQualifiedName~StreamChatTests.StreamClientFactoryTests"
```

#### Running tests with a mock server

If you have a mock server running, you can run tests against it by overriding the STREAM_CHAT_URL:

```shell
$ make test STREAM_KEY=mock STREAM_SECRET=mock STREAM_CHAT_URL=http://localhost:3030
```

Or in Docker:
```shell
$ make test_with_docker STREAM_KEY=mock STREAM_SECRET=mock STREAM_CHAT_URL=http://host.docker.internal:3030
```

> **Note:** When running tests in Docker, we use `host.docker.internal:3030` instead of `localhost:3030` because Docker containers cannot access the host machine's localhost directly. The special DNS name `host.docker.internal` is automatically resolved to the internal IP address used by the host, allowing the Docker container to communicate with services running on the host machine. This is why we also add the `--add-host=host.docker.internal:host-gateway` flag to the Docker run command in the Makefile.

Go to the next section to see how to use it in IDEs.

## Recommended tools for day-to-day development

### VS Code

For VS Code, the recommended extensions are:
- [C#](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csharp) by Microsoft
- [.NET Core Test Explorer](https://marketplace.visualstudio.com/items?itemName=formulahendry.dotnet-test-explorer) by Jun Huan

Recommended settings (`.vscode/settings.json`):
```json
{
    "omnisharp.testRunSettings": ".runsettings"
}
```

### Visual Studio

Follow [Microsoft's documentation](https://docs.microsoft.com/en-us/visualstudio/test/configure-unit-tests-by-using-a-dot-runsettings-file?view=vs-2022#specify-a-run-settings-file-in-the-ide) on how to set up `.runsettings` file.

### Rider

Follow [Jetbrain's documentation](https://www.jetbrains.com/help/rider/Reference__Options__Tools__Unit_Testing__MSTest.html) on how to set up `.runsettings` file.
